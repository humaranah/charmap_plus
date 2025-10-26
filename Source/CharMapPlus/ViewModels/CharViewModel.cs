using CommunityToolkit.Mvvm.ComponentModel;

namespace CharMapPlus.ViewModels;

public partial class CharViewModel : ObservableObject
{
    [ObservableProperty]
#pragma warning disable MVVMTK0045 // Using [ObservableProperty] on fields is not AOT compatible for WinRT
    private string _character = string.Empty;

    [ObservableProperty]
    private string _utf8Code = string.Empty;

    [ObservableProperty]
    private string _fontName = string.Empty;

    [ObservableProperty]
    private bool _isSelected = false;
#pragma warning restore MVVMTK0045 // Using [ObservableProperty] on fields is not AOT compatible for WinRT
}
