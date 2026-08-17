using CharMapPlus.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

namespace CharMapPlus;

/// <summary>
/// Main application window.
/// </summary>
public sealed partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;

    public CharMapViewModel ViewModel { get; }

    public MainWindow(CharMapViewModel viewModel, ILogger<MainWindow> logger)
    {
        _logger = logger;
        ViewModel = viewModel;

        try
        {
            _logger.LogInformation("Initializing MainWindow");

            InitializeComponent();

            ConfigureWindow();

            _logger.LogInformation("MainWindow initialized successfully");
        }
#pragma warning disable S2139 // Exceptions should be either logged or rethrown but not both
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize MainWindow");
            throw;
        }
#pragma warning restore S2139 // Exceptions should be either logged or rethrown but not both
    }

    private void ConfigureWindow()
    {
        if (!AppWindowTitleBar.IsCustomizationSupported())
        {
            _logger.LogWarning("AppWindowTitleBar customization not supported");
            return;
        }

        try
        {
            _logger.LogDebug("Configuring window");

            var hwnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            ConfigureAppWindow(appWindow);

            _logger.LogDebug("Window configured successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to configure window");
        }
    }

    private static void ConfigureAppWindow(AppWindow appWindow)
    {
        appWindow.SetIcon(@"Assets\Icon.ico");

        var presenter = appWindow.Presenter as OverlappedPresenter;
        if (presenter is not null)
        {
            presenter.IsMinimizable = true;
            presenter.IsMaximizable = true;
        }
    }
}
