using CommunityToolkit.Mvvm.ComponentModel;

namespace CharMapPlus.ViewModels;

public partial class CharViewModel : ObservableObject
{
    [ObservableProperty]
    private string _character = string.Empty;

    [ObservableProperty]
    private string _utf8Code = string.Empty;

    [ObservableProperty]
    private string _fontName = string.Empty;

    [ObservableProperty]
    private bool _isSelected = false;
}
