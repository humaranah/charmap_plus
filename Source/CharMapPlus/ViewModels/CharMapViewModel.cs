using CharMapPlus.Core.Abstrations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CharMapPlus.ViewModels;

public sealed partial class CharMapViewModel(
    IFontService fontService,
    IClipboardService clipboardService,
    ILogger<CharMapViewModel> logger) : ObservableObject, IDisposable
{
    private CancellationTokenSource? _loadCharactersCts;

    [ObservableProperty]
#pragma warning disable MVVMTK0045 // Using [ObservableProperty] on fields is not AOT compatible for WinRT
    private List<string> _fonts = [];

    [ObservableProperty]
    private string _fontSearchText = string.Empty;

    partial void OnFontSearchTextChanged(string value)
    {
        FilterFonts(value);
    }

    [ObservableProperty]
    private List<string> _filteredFonts = [];

    [ObservableProperty]
    private string? _selectedFont;

    [ObservableProperty]
    private bool _isLoadingFonts = false;

    [ObservableProperty]
    private bool _isLoadingGlyphs = false;

    async partial void OnSelectedFontChanged(string? value)
    {
        if (value is not null)
        {
            await LoadGlyphsAsync(value);
        }
    }

    [ObservableProperty]
    private List<CharViewModel> _glyphs = [];

    [ObservableProperty]
    private CharViewModel? _selectedCharacter;

    [ObservableProperty]
    private string _selectionText = string.Empty;
#pragma warning restore MVVMTK0045 // Using [ObservableProperty] on fields is not AOT compatible for WinRT

    public void Dispose()
    {
        _loadCharactersCts?.Cancel();
        _loadCharactersCts?.Dispose();
        logger.LogDebug("CharMapViewModel disposed");
    }

    [RelayCommand]
    private void CopySelection()
    {
        if (SelectedCharacter is not null)
        {
            SelectionText += SelectedCharacter.Character;
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
                .Select(font => font.Name)
                .OrderBy(name => name)];

            if (Fonts.Count > 0)
                SelectedFont = Fonts[0];
        }
        finally
        {
            IsLoadingFonts = false;
        }
    }

    [RelayCommand]
    public void SelectCharacter(CharViewModel character)
    {
        foreach (var glyph in Glyphs)
        {
            glyph.IsSelected = false;
        }

        character.IsSelected = true;
        SelectedCharacter = character;
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
                    });
            }, token);
            if (!token.IsCancellationRequested)
            {
                Glyphs = [.. characters];
                logger.LogInformation("Loaded {CharacterCount} characters for font '{FontName}'",
                    Glyphs.Count, fontName);
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

    private void FilterFonts(string searchText)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FilteredFonts = Fonts;
            }
            else
            {
                var lowerSearch = searchText.ToLowerInvariant();
                FilteredFonts = [.. Fonts
                    .Where(f => f.Contains(lowerSearch, StringComparison.InvariantCultureIgnoreCase))];
            }
            logger.LogDebug("Filtered fonts with search text '{SearchText}': {FilteredCount} items",
                searchText, FilteredFonts.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error filtering fonts with search text '{SearchText}'", searchText);
            FilteredFonts = Fonts;
        }
    }
}