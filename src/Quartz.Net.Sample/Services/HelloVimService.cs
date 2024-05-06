namespace Quartz.Net.Sample.Services;

public class HelloVimService : IMyTaskService
{
    private readonly ILogger<HelloVimService> logger;

    public HelloVimService(ILogger<HelloVimService> logger)
    {
        this.logger = logger;
    }

    public async Task<bool> RunAsync()
    {
        this.logger.LogInformation($"{nameof(HelloVimService)}: Hello, Vim!");
        return await Task.FromResult(true);
    }
}
