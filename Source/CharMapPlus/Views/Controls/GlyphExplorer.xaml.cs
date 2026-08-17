using CharMapPlus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CharMapPlus.Views.Controls;

public sealed partial class GlyphExplorer : UserControl
{
    public CharMapViewModel ViewModel { get; }

    public GlyphExplorer()
    {
        InitializeComponent();
        ViewModel = App.Services.GetRequiredService<CharMapViewModel>();
    }

    private void ToggleButton_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        ViewModel.CopySelectionCommand.Execute(null);
    }

    private void ToggleButton_Unchecked(object sender, RoutedEventArgs _)
    {
        if (sender is ToggleButton { Tag: string utf8Code } toggleButton &&
            ViewModel.SelectedGlyph?.Utf8Code == utf8Code)
        {
            toggleButton.IsChecked = true;
        }
    }
}
