namespace Quartz.Net.Sample.Services;

public interface IMyTaskService
{
    Task<bool> RunAsync();
}

