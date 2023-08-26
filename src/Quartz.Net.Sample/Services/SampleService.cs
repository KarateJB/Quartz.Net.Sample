using Microsoft.Extensions.Logging;

namespace Quartz.Net.Sample.Services;

public class SampleService : IMyTaskService
{
    private readonly ILogger<SampleService> logger;

    public SampleService(ILogger<SampleService> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> RunAsync()
    {
        this.logger.LogInformation($"{nameof(SampleService)}: do something.");
        return true;
    }
}
