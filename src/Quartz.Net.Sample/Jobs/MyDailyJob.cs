using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Models.DTO;
using Quartz.Net.Sample.Services;
using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Daily Job")]
public class MyDailyJob : BaseJob<MyDailyJob>, IJob
{
    private readonly HelloWorldService ts;

    public MyDailyJob(
            ILogger<MyDailyJob> logger,
            IInteractiveMode im,
            IOptions<AppSetting> configuration,
            MyTaskResolver taskResolver) : base(logger, im, configuration)
    {
        this.ts = taskResolver(nameof(HelloWorldService)) as HelloWorldService;
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
