namespace Quartz.Net.Sample.Services;

public interface IInteractiveMode
{
    Task<bool> PromptAsync();

    Task ConsoleLogAsync(string msg, Models.Enums.Colors color);
}
