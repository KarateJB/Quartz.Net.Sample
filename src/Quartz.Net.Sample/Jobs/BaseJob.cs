using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Models.DTO;
using Quartz.Net.Sample.Utils.Constants;

namespace Quartz.Net.Sample.Jobs;

public class BaseJob<T>
{
    protected readonly string jobClass = string.Empty;
    protected readonly ILogger<T> logger;
    protected readonly IInteractiveMode im;
    protected readonly AppSetting appSetting;
    protected JobResult jobResult = null;

    public BaseJob(
            ILogger<T> logger,
            IInteractiveMode im,
            IOptions<AppSetting> configuration)
    {
        this.jobClass = typeof(T).Name;
        this.logger = logger;
        this.im = im;
        this.appSetting = configuration.Value;
    }

    public bool IsSuccess { get { return this.jobResult.IsSuccess; } }

    public async Task ExecuteAsync(IJobExecutionContext context, Func<JobResult, Task> jobAction, Func<string> genMsgFunc = null)
    {
        await this.cleanResultAsync();

        // Initialize callback
        if (genMsgFunc is null)
        {
            genMsgFunc = () => $"\"{this.jobClass}\" {(this.IsSuccess ? "succeeded" : "failed")}";
        }

        #region Retry
        if (context.RefireCount > this.appSetting.Quartz.RetryMaxTimes)
        {
            this.logger.LogError($"{jobClass} retried for more than {this.appSetting.Quartz.RetryMaxTimes} but all failed, stop retrying.");
            return;
        }
        #endregion

        try
        {
            await jobAction(this.jobResult);
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
            // Logging
            this.logger.LogInformation(msg);
            // stdout (interactive mode)
            if (Environment.GetEnvironmentVariable(EnvConstants.InteractiveMode) == EnvConstants.True)
            {
                await this.im.ConsoleLogAsync(msg, IsSuccess ? Models.Enums.Colors.Yellow : Models.Enums.Colors.Red);
            }
        }
    }

    private async Task cleanResultAsync()
    {
        this.jobResult = new JobResult();
        await Task.CompletedTask;
    }
}
