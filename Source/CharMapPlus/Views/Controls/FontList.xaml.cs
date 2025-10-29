using CharMapPlus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CharMapPlus.Views.Controls;

public sealed partial class FontList : UserControl
{
    public CharMapViewModel ViewModel { get; }

    public FontList()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CharMapViewModel>();

        _ = ViewModel.LoadFontsCommand.ExecuteAsync(null);
    }

    private void ToggleButton_Unchecked(object sender, RoutedEventArgs _)
    {
        if (sender is ToggleButton { Tag: string fontName } toggleButton &&
            ViewModel.SelectedFont?.FontName == fontName)
        {
            toggleButton.IsChecked = true;
        }
    }
}
