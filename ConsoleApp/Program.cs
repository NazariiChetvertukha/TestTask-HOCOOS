using ConsoleApp.DependencyInjection;
using ConsoleApp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();
services.AddConsoleAppInternalDependencies(configuration);

var serviceProvider = services.BuildServiceProvider();

var expressionTask = serviceProvider.GetKeyedService<IBackendTask>("ExpressionBackendTask");
await expressionTask.RunAsync();

var threadTask = serviceProvider.GetKeyedService<IBackendTask>("ThreadBackendTask");
await threadTask.RunAsync();

var limitedThreadsTask = serviceProvider.GetKeyedService<IBackendTask>("LimitedThreadsBackendTask");
await limitedThreadsTask.RunAsync();

var customExpressionTask = serviceProvider.GetKeyedService<IBackendTask>("FieldsExpressionBackendTask");
await customExpressionTask.RunAsync();