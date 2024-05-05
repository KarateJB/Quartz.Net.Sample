namespace Quartz.Net.Sample.Services;

public class HelloWorldService : IMyTaskService
{
    private readonly ILogger<HelloWorldService> logger;

    public HelloWorldService(ILogger<HelloWorldService> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> RunAsync()
    {
        this.logger.LogInformation($"{nameof(HelloWorldService)}: Hello, world!");
        return await Task.FromResult(true);
    }
}
