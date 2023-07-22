namespace Quartz.Net.Sample.Services;

public interface IInteractiveMode
{
    Task<bool> PromptAsync();
}
