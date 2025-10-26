using CharMapPlus.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.ComponentModel;
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
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            ConfigureWindow();
            RegisterKeyboardAccelerators();

            Closed += MainWindow_Closed;

            _logger.LogInformation("MainWindow initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to initialize MainWindow");
            throw;
        }
    }

    private void ConfigureWindow()
    {
        _ = ViewModel.LoadFontsCommand.ExecuteAsync(null);

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

    private void RegisterKeyboardAccelerators()
    {
        try
        {
            // Ctrl+C for copy
            var copyAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.C,
                Modifiers = Windows.System.VirtualKeyModifiers.Control
            };
            copyAccelerator.Invoked += (_, _) =>
            {
                _logger.LogDebug("Keyboard shortcut: Ctrl+C");
                ViewModel.CopySelectedTextCommand.Execute(null);
            };
            Content.KeyboardAccelerators.Add(copyAccelerator);

            // Enter for select
            var selectAccelerator = new KeyboardAccelerator
            {
                Key = Windows.System.VirtualKey.Enter
            };
            selectAccelerator.Invoked += (_, _) =>
            {
                _logger.LogDebug("Keyboard shortcut: Enter");
                ViewModel.CopySelectionCommand.Execute(null);
            };
            Content.KeyboardAccelerators.Add(selectAccelerator);

            _logger.LogDebug("Keyboard accelerators registered");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to register keyboard accelerators");
        }
    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        try
        {
            if (e.PropertyName == nameof(ViewModel.Glyphs) && ViewModel.Glyphs.Count > 0)
            {
                _logger.LogDebug("Characters loaded ({Count} items), resetting scroll", ViewModel.Glyphs.Count);
                GlyphScrollViewer.ChangeView(0, 0, null);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ViewModel property change");
        }
    }

    private void Border_Tapped(object sender, TappedRoutedEventArgs e)
    {
        try
        {
            if (sender is Border border && border.Tag is CharViewModel viewModel)
            {
                _logger.LogDebug("Character tapped: {Char} ({Code})", viewModel.Character, viewModel.Utf8Code);
                ViewModel.SelectCharacterCommand.Execute(viewModel);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling character selection");
        }
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        _logger.LogInformation("MainWindow closing, performing cleanup");

        ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        Closed -= MainWindow_Closed;
    }
}
