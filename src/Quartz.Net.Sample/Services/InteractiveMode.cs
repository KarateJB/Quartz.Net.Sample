using System.ComponentModel;
using System.Reflection;
using Quartz.Net.Sample.Models.DTO;
using Spectre.Console;

namespace Quartz.Net.Sample.Services;

public class InteractiveMode : IInteractiveMode
{
    private const bool ShutDown = true;
    private const string Quit = "Quit";
    private readonly ILogger<InteractiveMode> logger;

    private readonly IScheduler scheduler = null;
    private readonly IEnumerable<JobMenu> jobMenus = null;

    public InteractiveMode(
            ILogger<InteractiveMode> logger,
            ISchedulerFactory schedulerFactory)
    {
        this.logger = logger;

        // Quartz scheduler
        this.scheduler = schedulerFactory.GetScheduler().Result;

        // Fetch the implementations (types) of Quartz.IJob
        this.jobMenus = this.GetJobImplementations();
    }

    public async Task<bool> PromptAsync()
    {
        while (true)
        {
            var reply = AnsiConsole.Prompt(new SelectionPrompt<JobMenu>().HighlightStyle(new Style(foreground: Color.Green))
                                .Title("[blue] Choose a task to execute: [/]")
                                .AddChoices(this.jobMenus));

            switch (reply.Type)
            {
                case null:
                    return ShutDown;
                default:
                    var jobKey = new JobKey(reply.Type.Name);
                    if (scheduler.CheckExists(jobKey).Result)
                    {
                        await scheduler.Start();
                        await scheduler.TriggerJob(jobKey);
                        AnsiConsole.MarkupLine($"[yellow]{DateTime.Now.ToString()} \"{reply.Type.Name}\" started, do not close this window until it is completed.[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]{DateTime.Now.ToString()} No related job found for \"{reply.Type.Name}\"!\n\n");
                    }

                    break;
            }

            await Task.Delay(2000);
        }
    }

    public async Task ConsoleLogAsync(string msg, Models.Enums.Colors color)
    {
        AnsiConsole.MarkupLine($"[{color.ToString().ToLower()}]{DateTime.Now.ToString()} {msg}\n[/]");
        await Task.Delay(500);
    }

    private IEnumerable<JobMenu> GetJobImplementations()
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(Quartz.IJob).IsAssignableFrom(t) && t.IsClass);

        foreach (var type in types)
        {
            // Get the Description attribute of the class
            var descriptionAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(type, typeof(DescriptionAttribute));
            string description = descriptionAttribute?.Description;
            var jobMenu = new JobMenu
            {
                Type = type,
                Description = description ?? type.Name
            };

            yield return jobMenu;
        }

        yield return new JobMenu { Type = null, Description = "Quit" };
    }
}
