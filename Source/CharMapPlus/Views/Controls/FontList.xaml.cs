using CharMapPlus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

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

    private void FontsScrollViewer_GettingFocus(UIElement sender, GettingFocusEventArgs e)
    {
        if (e.OldFocusedElement is ToggleButton || ViewModel?.SelectedFont is null)
            return;

        for (var i = 0; i < ViewModel.FilteredFonts.Count; i++)
        {
            var element = FontsRepeater.TryGetElement(i);
            if (element is ToggleButton toggleButton &&
                toggleButton.IsChecked == true)
            {
                e.NewFocusedElement = toggleButton;
                e.Handled = true;
                return;
            }
        }
    }

    private void ToggleButton_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleButton toggleButton)
        {
            toggleButton.IsChecked = true;
        }
    }
}
