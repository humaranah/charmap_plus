using CharMapPlus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CharMapPlus;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public CharMapViewModel ViewModel { get; }

    public MainWindow()
    {
        ViewModel = App.Services.GetRequiredService<CharMapViewModel>();
        InitializeComponent();
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;

        if (AppWindowTitleBar.IsCustomizationSupported())
        {
            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.SetIcon(@"Assets\Icon.ico");
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ViewModel.Characters) && ViewModel.Characters.Count > 0)
        {
            GlyphScrollViewer.ChangeView(0, 0, null);
        }
    }

    private void Border_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is Border border && border.Tag is CharViewModel viewModel)
        {
            foreach (var item in ViewModel.Characters)
            {
                item.IsSelected = false;
            }
            viewModel.IsSelected = true;
            ViewModel.SelectedCharacter = viewModel;
        }
    }
}
