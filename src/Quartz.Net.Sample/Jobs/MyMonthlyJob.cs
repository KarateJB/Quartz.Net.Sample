using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Models.DTO;
using Quartz.Net.Sample.Services;
using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Monthly Job")]
public class MyMonthlyJob : BaseJob<MyMonthlyJob>, IJob
{
    private readonly HelloVimService ts;

    public MyMonthlyJob(
            ILogger<MyMonthlyJob> logger,
            IInteractiveMode im,
            IOptions<AppSetting> configuration,
            MyTaskResolver taskResolver) : base(logger, im, configuration)
    {
        this.ts = taskResolver(nameof(HelloVimService)) as HelloVimService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Action<JobResult> doJob = async (jobResult) =>
        {
            jobResult.ExecutedResult = await this.ts.RunAsync();
        };

        Func<string> genMsg = () => $"\"{base.jobClass}\" {(base.IsSuccess ? "succeeded" : "failed")}";

        // Exectute
        await base.ExecuteAsync(context, doJob, genMsg);
    }
}
