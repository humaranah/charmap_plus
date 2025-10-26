using CharMapPlus.Core.Abstrations;
using CharMapPlus.Infrastructure;
using CharMapPlus.Infrastructure.DirectWrite;
using CharMapPlus.Services;
using CharMapPlus.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CharMapPlus;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: false))
            .AddSingleton<IDWriteFontCollectionFactory, DWriteFontCollectionFactory>()
            .AddSingleton<IFontCollectionProvider, DwFontCollectionProvider>()
            .AddSingleton<IFontService, FontService>()
            .AddSingleton<IClipboardService, WinUiClipboardService>()
            .AddTransient<CharMapViewModel>()
            .AddTransient<MainWindow>();
    }
}
