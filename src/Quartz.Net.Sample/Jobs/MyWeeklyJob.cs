using System.ComponentModel;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Models.DTO;
// using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Weekly Job")]
public class MyWeeklyJob : BaseJob<MyWeeklyJob>, IJob
{
    private readonly HelloDotnetService ts;

    public MyWeeklyJob(
            // MyTaskResolver taskResolver,
            ILogger<MyWeeklyJob> logger,
            IServiceProvider serviceProvider,
            IInteractiveMode im,
            IOptions<AppSetting> configuration,
            [FromKeyedServices(nameof(HelloDotnetService))] IMyTaskService myTaskService) : base(logger, im, configuration)
    {
        this.ts = serviceProvider.GetKeyedService<IMyTaskService>(nameof(HelloDotnetService)) as HelloDotnetService;
        // this.ts = myTaskService as HelloDotnetService;

        // Old way: use Resolver
        // this.ts = taskResolver(nameof(HelloDotnetService)) as HelloDotnetService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Func<JobResult, Task> doJob = async (jobResult) =>
        {
            jobResult.ExecutedResult = await this.ts.RunAsync();
        };

        Func<string> genMsg = () => $"\"{base.jobClass}\" returned {base.jobResult.ExecutedResult}.";

        // Exectute
        await base.ExecuteAsync(context, doJob, genMsg);
    }
}
