using ConsoleApp.Interfaces;

namespace ConsoleApp.Tasks;

public class ThreadBackendTask : IBackendTask {
  protected record ThreadTaskItemConfig(int Number);
  protected record ThreadTaskItemResult(string Message);
  
  protected const int ItemsCount = 100;
  
  public virtual async Task RunAsync() {
    var configs = CreateConfigs(ItemsCount);
    var results = await ExecuteAsync(configs);
    WriteResults(results);
  }

  protected virtual IEnumerable<ThreadTaskItemConfig> CreateConfigs(int count) {
    return Enumerable.Range(0, count).Select(CreateConfig);
  }
  
  protected virtual ThreadTaskItemConfig CreateConfig(int number) {
    return new ThreadTaskItemConfig(number);
  }
  
  protected virtual async Task<IEnumerable<ThreadTaskItemResult>> ExecuteAsync(
      IEnumerable<ThreadTaskItemConfig> configs) {
    var tasks = configs.Select(ExecuteAsync);
    return await Task.WhenAll(tasks);
  }
  
  protected virtual async Task<ThreadTaskItemResult> ExecuteAsync(ThreadTaskItemConfig config) {
    var message = $"Message {config.Number}";
    await Task.Delay(100);
    return new ThreadTaskItemResult(message);
  }
  
  protected virtual void WriteResults(IEnumerable<ThreadTaskItemResult> results) {
    foreach (var result in results) {
      WriteResult(result);
    }
  }
  
  protected virtual void WriteResult(ThreadTaskItemResult result) {
    Console.WriteLine(result.Message);
  }
}
