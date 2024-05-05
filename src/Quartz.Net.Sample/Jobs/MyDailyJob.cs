using System.ComponentModel;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Models.DTO;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Daily Job")]
public class MyDailyJob : BaseJob<MyDailyJob>, IJob
{
    private readonly HelloWorldService ts;

    public MyDailyJob(
            // MyTaskResolver taskResolver,
            ILogger<MyDailyJob> logger,
            IServiceProvider serviceProvider,
            IInteractiveMode im,
            IOptions<AppSetting> configuration,
            [FromKeyedServices(nameof(HelloWorldService))] IMyTaskService myTaskService) : base(logger, im, configuration)
    {
        // TODO: Inject service check
        this.ts = serviceProvider.GetKeyedService<IMyTaskService>(nameof(HelloWorldService)) as HelloWorldService;
        // this.ts = myTaskService as HelloWorldService;

        // Old way: use Resolver
        // this.ts = taskResolver(nameof(HelloWorldService)) as HelloWorldService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Func<JobResult, Task> doJob = async (jobResult) =>
        {
            jobResult.ExecutedResult = await this.ts.RunAsync();
        };

        // Exectute
        await base.ExecuteAsync(context, doJob);
    }
}
