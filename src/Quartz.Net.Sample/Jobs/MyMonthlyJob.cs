using System.ComponentModel;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Models.DTO;
// using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Monthly Job")]
public class MyMonthlyJob : BaseJob<MyMonthlyJob>, IJob
{
    private readonly HelloVimService ts;

    public MyMonthlyJob(
            // MyTaskResolver taskResolver,
            ILogger<MyMonthlyJob> logger,
            IServiceProvider serviceProvider,
            IInteractiveMode im,
            IOptions<AppSetting> configuration,
            [FromKeyedServices(nameof(HelloVimService))] IMyTaskService myTaskService) : base(logger, im, configuration)
    {
        this.ts = serviceProvider.GetKeyedService<IMyTaskService>(nameof(HelloVimService)) as HelloVimService;
        // this.ts = myTaskService as HelloVimService;

        // Old way: use Resolver
        // this.ts = taskResolver(nameof(HelloVimService)) as HelloVimService;
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
