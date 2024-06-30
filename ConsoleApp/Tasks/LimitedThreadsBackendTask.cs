using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Tasks
{
    public class LimitedThreadsBackendTask(ILogger<LimitedThreadsBackendTask> logger, IConfiguration configuration)
        : ThreadBackendTask
    {
        private readonly ILogger<LimitedThreadsBackendTask> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private int _maxActiveThreads = configuration.GetValue<int>("ThreadsSettings:MaxThreadsPerTaskScope");

        public override async Task RunAsync()
        {
            _logger.LogInformation("ThreadSafeBackendTask started");

            var configs = CreateConfigs(ItemsCount);
            var results = await ExecuteWithLimitedConcurrencyAsync(configs, 5);
            WriteResults(results);

            _logger.LogInformation("ThreadSafeBackendTask completed");
        }

        private async Task<IEnumerable<ThreadTaskItemResult>> ExecuteWithLimitedConcurrencyAsync(
            IEnumerable<ThreadTaskItemConfig> configs, int maxDegreeOfParallelism)
        {
            ArgumentNullException.ThrowIfNull(configs);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxDegreeOfParallelism);

            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);
            var tasks = configs.Select(async config =>
            {
                await semaphore.WaitAsync();
                try
                {
                    Interlocked.Increment(ref _maxActiveThreads);
                    _logger.LogInformation("Thread {ConfigNumber} started. Active threads: {ActiveThreads}", config.Number, _maxActiveThreads);

                    var result = await ExecuteAsync(config);
                    return result;
                }
                finally
                {
                    Interlocked.Decrement(ref _maxActiveThreads);
                    _logger.LogInformation("Thread {ConfigNumber} completed. Active threads: {ActiveThreads}", config.Number, _maxActiveThreads);
                    semaphore.Release();
                }
            });
            return await Task.WhenAll(tasks);
        }

        protected override async Task<ThreadTaskItemResult> ExecuteAsync(ThreadTaskItemConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);

            var message = $"Message {config.Number} executed on thread {Environment.CurrentManagedThreadId}";
            await Task.Delay(100);
            return new ThreadTaskItemResult(message);
        }

        protected override void WriteResult(ThreadTaskItemResult result)
        {
            _logger.LogInformation(result.Message);
        }
    }
}
