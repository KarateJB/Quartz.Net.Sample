namespace Quartz.Net.Sample.Services;

public class MyTaskService : IMyTaskService
{
    public async Task<bool> RunAsync()
    {
        // Do something
        return true;
    }
}
