using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using System;

namespace CharMapPlus;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    private Window? _window;
    private static ILogger Logger => Log.Logger.ForContext<App>();

    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        try
        {
            Log.Logger = Startup.CreateLogger();
            Logger.Information("Application starting...");

            InitializeComponent();
            Services = Startup.CreateServiceProvider();

            UnhandledException += App_UnhandledException;
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Application start-up failed");
            Log.CloseAndFlush();
            throw;
        }
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            // Resolver MainWindow desde DI container
            _window = Services.GetRequiredService<MainWindow>();
            _window.Closed += OnMainWindowClosed;
            _window.Activate();

            Logger.Information("Main window activated successfully");
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex, "Failed to launch main window");
            CleanupResources();
            Environment.Exit(1);
        }
    }

    private static void OnMainWindowClosed(object sender, WindowEventArgs args)
    {
        CleanupResources();
        Environment.Exit(0);
    }

    private static void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Logger.Fatal(e.Exception, "Unhandled exception occurred");
        e.Handled = true;
    }

    private static void CleanupResources()
    {
        try
        {
            Logger.Information("Application shutting down...");

            if (Services is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error during cleanup");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
