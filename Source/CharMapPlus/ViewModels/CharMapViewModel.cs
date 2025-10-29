using CharMapPlus.Core.Abstrations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CharMapPlus.ViewModels;

#pragma warning disable MVVMTK0045 // Using [ObservableProperty] on fields is not AOT compatible for WinRT
public sealed partial class CharMapViewModel(
    IFontService fontService,
    IClipboardService clipboardService,
    ILogger<CharMapViewModel> logger) : ObservableObject, IDisposable
{
    private CancellationTokenSource? _loadCharactersCts;
    private CancellationTokenSource? _filterFontsCts;

    [ObservableProperty]
    private bool _isLoadingFonts = false;

    [ObservableProperty]
    private bool _isLoadingGlyphs = false;

    [ObservableProperty]
    private List<FontViewModel> _fonts = [];

    partial void OnFontsChanged(List<FontViewModel>? oldValue, List<FontViewModel> newValue)
    {
        foreach (var font in oldValue ?? [])
            font.PropertyChanged -= Font_PropertyChanged;

        foreach (var font in newValue ?? [])
            font.PropertyChanged += Font_PropertyChanged;
    }

    [ObservableProperty]
    private string _fontSearchText = string.Empty;

    async partial void OnFontSearchTextChanged(string value)
    {
        await FilterFontsAsync(value);
    }

    [ObservableProperty]
    private List<FontViewModel> _filteredFonts = [];

    public FontViewModel? SelectedFont => Fonts.FirstOrDefault(f => f.IsSelected);

    [ObservableProperty]
    private List<CharViewModel> _glyphs = [];

    partial void OnGlyphsChanged(List<CharViewModel>? oldValue, List<CharViewModel> newValue)
    {
        foreach (var glyph in oldValue ?? [])
            glyph.PropertyChanged -= Glyph_PropertyChanged;

        foreach (var glyph in newValue ?? [])
            glyph.PropertyChanged += Glyph_PropertyChanged;
    }

    public CharViewModel? SelectedGlyph => Glyphs.FirstOrDefault(g => g.IsSelected);

    [ObservableProperty]
    private string _selectionText = string.Empty;

    public void Dispose()
    {
        foreach (var font in Fonts ?? [])
            font.PropertyChanged -= Font_PropertyChanged;

        foreach (var glyph in Glyphs ?? [])
            glyph.PropertyChanged -= Glyph_PropertyChanged;

        _loadCharactersCts?.Cancel();
        _loadCharactersCts?.Dispose();
        _filterFontsCts?.Cancel();
        _filterFontsCts?.Dispose();
        logger.LogDebug("CharMapViewModel disposed");
    }

    [RelayCommand]
    private void CopySelection()
    {
        if (SelectedGlyph is not null)
        {
            SelectionText += SelectedGlyph.Character;
        }
    }

    [RelayCommand]
    private void CopySelectedText()
    {
        if (!string.IsNullOrEmpty(SelectionText))
        {
            clipboardService.SetText(SelectionText);
        }
    }

    [RelayCommand]
    private async Task LoadFonts()
    {
        IsLoadingFonts = true;
        try
        {
            await fontService.LoadFontsAsync();
            var fonts = fontService.ListFonts();
            Fonts = [.. fonts
                .Select(font => new FontViewModel { FontName = font.Name })
                .OrderBy(vm => vm.FontName)];

            FilteredFonts = Fonts;
            if (FilteredFonts.Count > 0)
            {
                FilteredFonts[0].IsSelected = true;
                await LoadGlyphsAsync(FilteredFonts[0].FontName);
            }
        }
        finally
        {
            IsLoadingFonts = false;
        }
    }

    private void Font_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not FontViewModel font)
            return;
        if (e.PropertyName == nameof(FontViewModel.IsSelected))
        {
            if (font.IsSelected)
            {
                // Deseleccionar otras fuentes
                foreach (var otherFont in Fonts.Where(f => f != font && f.IsSelected))
                {
                    otherFont.IsSelected = false;
                }

                logger.LogDebug("Font '{FontName}' selected", font.FontName);
                _ = LoadGlyphsAsync(font.FontName);
            }
            OnPropertyChanged(nameof(SelectedFont));
        }
    }

    private void Glyph_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not CharViewModel glyph)
            return;
        if (e.PropertyName == nameof(CharViewModel.IsSelected))
        {
            if (glyph.IsSelected)
            {
                // Deseleccionar otros glifos
                foreach (var otherGlyph in Glyphs.Where(g => g != glyph && g.IsSelected))
                {
                    otherGlyph.IsSelected = false;
                }
                logger.LogDebug("Character '{Character}' selected", glyph.Character);
            }
            OnPropertyChanged(nameof(SelectedGlyph));
        }
    }

    private async Task LoadGlyphsAsync(string fontName)
    {
        if (_loadCharactersCts != null)
        {
            await _loadCharactersCts.CancelAsync();
            _loadCharactersCts.Dispose();
        }
        _loadCharactersCts = new CancellationTokenSource();

        var token = _loadCharactersCts.Token;

        IsLoadingGlyphs = true;
        try
        {
            logger.LogDebug("Loading characters for font '{FontName}'", fontName);
            var characters = await Task.Run(() =>
            {
                var glyphs = fontService.GetFontSupportedGlyphs(fontName);
                return glyphs
                    .Select(c => new CharViewModel()
                    {
                        Character = c.Character,
                        Utf8Code = $"U+{c.CodePoint:X4}",
                        FontName = fontName
                    })
                    .ToList();
            }, token);
            if (!token.IsCancellationRequested)
            {
                Glyphs = characters;
                logger.LogInformation("Loaded {CharacterCount} characters for font '{FontName}'",
                    Glyphs.Count, fontName);
                Glyphs[0].IsSelected = true;
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogDebug(ex, "Loading characters for font '{FontName}' was canceled", fontName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading characters for font '{FontName}'", fontName);
            Glyphs = [];
        }
        finally
        {
            IsLoadingGlyphs = false;
        }
    }

    private async Task FilterFontsAsync(string searchText)
    {
        if (_filterFontsCts != null)
        {
            await _filterFontsCts.CancelAsync();
            _filterFontsCts.Dispose();
        }
        _filterFontsCts = new CancellationTokenSource();

        var token = _filterFontsCts.Token;

        try
        {
            await Task.Delay(150, token);

            var filtered = await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return Fonts;
                }
                else
                {
                    var lowerSearch = searchText.ToLowerInvariant();
                    return [..
                        Fonts.Where(f => f.FontName.Contains(lowerSearch, StringComparison.InvariantCultureIgnoreCase))
                    ];
                }
            }, token);

            if (!token.IsCancellationRequested)
            {
                FilteredFonts = filtered;
                logger.LogDebug("Filtered fonts with search text '{SearchText}': {FilteredCount} items",
                    searchText, FilteredFonts.Count);
            }
        }
        catch (OperationCanceledException)
        {
            // Filtering was canceled
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error filtering fonts with search text '{SearchText}'", searchText);
            FilteredFonts = Fonts;
        }
    }
}