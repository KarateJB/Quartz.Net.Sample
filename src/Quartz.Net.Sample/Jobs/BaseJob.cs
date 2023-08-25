using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Services;
using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

public class BaseJob<T>
{
    protected readonly string jobClass = string.Empty;
    protected readonly ILogger<MyDailyJob> logger;
    protected readonly AppSetting appSetting;
    protected readonly JobResult jobResult = null;

    public BaseJob(
            ILogger<T> logger,
            IOptions<AppSetting> configuration)
    {
        this.jobClass = nameof(T);
        this.logger = logger;
        this.appSetting = configuration.Value;
    }

    public bool IsSuccess { get { return this.jobResult.IsSuccess; } }

    public async Task ExecuteAsync(IJobExecutionContext context, Action<JobResult> jobAction, Func<string> genMsgFunc)
    {
        await this.cleanResultAsync();

        #region Retry
        if (context.RefireCount > this.appSetting.Quartz.RetryMaxTimes)
        {
            this.logger.LogError($"{jobClass} retried for more than {this.appSetting.Quartz.RetryMaxTimes} but all failed, stop retrying.");
            return;
        }
        #endregion

        try
        {
            jobAction();
            this.jobResult.IsSuccess = true;
        }
        catch (System.Exception ex)
        {
            this.jobResult.IsSuccess = false;
            this.logger.LogError($"{jobClass} failed. {ex.Message}");
            throw new Quartz.JobExecutionException(msg: $"Start retrying {jobClass}", cause: ex, refireImmediately: true);
        }
        finally
        {
            var msg = genMsgFunc();
            this.logger.LogInformation(msg);
        }
    }

    private async Task cleanResultAsync()
    {
        this.jobResult = new JobResult();
        await Task.CompletedTask;
    }
}
