using CharMapPlus.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CharMapPlus.ViewModels;

public partial class CharMapViewModel : ObservableObject
{
    private readonly IFontService _fontService;

    public CharMapViewModel(IFontService fontService)
    {
        _fontService = fontService;
        LoadFonts();
    }

    [ObservableProperty]
    private List<string> _fonts = [];

    [ObservableProperty]
    private string? _selectedFont;

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
        var characters = _fontService.GetFontSupportedCharacters(fontName);
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

    private void LoadFonts()
    {
        Fonts = [.. _fontService.GetAllFonts()];
        if (Fonts.Count > 0)
            SelectedFont = Fonts[0];
    }
}
