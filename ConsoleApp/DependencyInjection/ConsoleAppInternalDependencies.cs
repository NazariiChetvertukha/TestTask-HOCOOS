using ConsoleApp.Interfaces;
using ConsoleApp.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.DependencyInjection;

public static class ConsoleAppInternalDependencies
{
    public static void AddConsoleAppInternalDependencies(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.SetMinimumLevel(LogLevel.Information);
        });
    
        services.AddSingleton(configuration);
        services.AddKeyedScoped<IBackendTask, ExpressionBackendTask>("ExpressionBackendTask");
        services.AddKeyedScoped<IBackendTask, ThreadBackendTask>("ThreadBackendTask");
        services.AddKeyedScoped<IBackendTask, LimitedThreadsBackendTask>("LimitedThreadsBackendTask");
        services.AddKeyedScoped<IBackendTask, FieldsExpressionBackendTask>("FieldsExpressionBackendTask");
    }
}