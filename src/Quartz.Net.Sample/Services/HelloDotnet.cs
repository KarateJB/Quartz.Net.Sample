using Microsoft.Extensions.Logging;

namespace Quartz.Net.Sample.Services;

public class HelloDotnetService : IMyTaskService
{
    private readonly ILogger<HelloDotnetService> logger;

    public HelloDotnetService(ILogger<HelloDotnetService> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> RunAsync()
    {
        this.logger.LogInformation($"{nameof(HelloDotnetService)}: Hello, Dotnet!");
        return true;
    }
}
