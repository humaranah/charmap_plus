using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace CharMapPlus;

public static class Startup
{
    public static IServiceProvider CreateServiceProvider()
    {
        return new ServiceCollection()
            .AddApplicationServices()
            .BuildServiceProvider();
    }

    public static ILogger CreateLogger()
    {
        return new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
            .WriteTo.Debug(outputTemplate: "[{Level:u3}] [{ThreadId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
#else
            .MinimumLevel.Information()
#endif
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.FromLogContext()
            .WriteTo.File("logs\\charmapplus.log",
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{ThreadId}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10_485_760, // 10 MB
                retainedFileCountLimit: 31)
            .CreateLogger();
    }
}
