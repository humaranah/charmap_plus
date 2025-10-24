using CharMapPlus.Core.Abstrations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CharMapPlus.ViewModels;

public partial class CharMapViewModel : ObservableObject
{
    private readonly IFontService _fontService;
    private readonly IClipboardService _clipboardService;

    public CharMapViewModel(
        IFontService fontService,
        IClipboardService clipboardService)
    {
        _fontService = fontService;
        _clipboardService = clipboardService;
    }

    [ObservableProperty]
    private List<string> _fonts = [];

    [ObservableProperty]
    private string? _selectedFont;

    [ObservableProperty]
    private bool _isLoading = false;

    partial void OnSelectedFontChanged(string? value)
    {
        if (value is not null)
        {
            FillCharacters(value);
        }
    }

    [ObservableProperty]
    private List<CharViewModel> _characters = [];

    [ObservableProperty]
    private CharViewModel? _selectedCharacter;

    [ObservableProperty]
    private string _selectionText = string.Empty;

    private void FillCharacters(string fontName)
    {
        var characters = _fontService.GetFontSupportedGlyphs(fontName);
        Characters = [.. characters
            .Select(c => new CharViewModel()
            {
                Character = c.Character,
                Utf8Code = $"U+{c.CodePoint:X4}",
                FontName = fontName
            })];
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
            _clipboardService.SetText(SelectionText);
        }
    }

    [RelayCommand]
    private async Task LoadFonts()
    {
        IsLoading = true;
        try
        {
            await _fontService.LoadFontsAsync();
            var fonts = _fontService.ListFonts();
            Fonts = [.. fonts.Select(f => f.Name)];
            if (Fonts.Count > 0)
                SelectedFont = Fonts[0];
        }
        finally
        {
            IsLoading = false;
        }
    }
}
