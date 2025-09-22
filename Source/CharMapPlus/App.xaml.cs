using CharMapPlus.Core;
using CharMapPlus.Infrastructure;
using CharMapPlus.Infrastructure.Abstractions;
using CharMapPlus.Infrastructure.DirectWrite;
using CharMapPlus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CharMapPlus
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        public static IServiceProvider Services { get; private set; } = null!;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Services = new ServiceCollection()
                .AddSingleton<IDwFontFactoryWrapper, DwFontFactoryWrapper>()
                .AddSingleton<IFontCollectionProvider, DWriteFontCollectionProvider>()
                .AddSingleton<IFontService, FontService>()
                .AddSingleton<CharMapViewModel>()
                .BuildServiceProvider();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
