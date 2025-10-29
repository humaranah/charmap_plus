using CommunityToolkit.Mvvm.ComponentModel;

namespace CharMapPlus.ViewModels;

#pragma warning disable MVVMTK0045 // Using [ObservableProperty] on fields is not AOT compatible for WinRT
public partial class FontViewModel : ObservableObject
{
    [ObservableProperty]
    private string _fontName = string.Empty;

    [ObservableProperty]
    private bool _isSelected = false;
}
