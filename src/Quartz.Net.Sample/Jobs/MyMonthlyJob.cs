using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Services;
using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Monthly Job")]
public class MyMonthlyJob : IJob
{
    private readonly string jobClass = string.Empty;
    private readonly ILogger<MyMonthlyJob> logger;
    private readonly AppSetting appSetting;
    private readonly HelloWorldService ts;

    public MyMonthlyJob(
            ILogger<MyMonthlyJob> logger,
            IOptions<AppSetting> configuration,
            MyTaskResolver taskResolver)
    {
        this.jobClass = nameof(MyMonthlyJob);
        this.logger = logger;
        this.appSetting = configuration.Value;
        this.ts = taskResolver(nameof(HelloWorldService)) as HelloWorldService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        bool isSuccess = false;

        #region Retry
        if (context.RefireCount > this.appSetting.Quartz.RetryMaxTimes)
        {
            this.logger.LogError($"{jobClass} retried for more than {this.appSetting.Quartz.RetryMaxTimes} but all failed, stop retrying.");
            return;
        }
        #endregion

        try
        {

        }
        catch (System.Exception ex)
        {
            isSuccess = false;
            this.logger.LogError($"{jobClass} failed. {ex.Message}");
            throw new Quartz.JobExecutionException(msg: $"Start retrying {jobClass}", cause: ex, refireImmediately: true);
        }
        finally
        {
            this.logger.LogInformation($"{jobClass} {(isSuccess ? "succeeded" : "failed")}");
        }
    }
}
