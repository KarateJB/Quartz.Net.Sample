using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Services;
using Quartz.Net.Sample.Utils.Constants;
using static Quartz.Net.Sample.Utils.Extensions.IServiceCollectionExtensions;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("Scheduled job demo")]
public class SampleJob : IJob
{
    private readonly string jobClass = string.Empty;
    private readonly ILogger<SampleJob> logger;
    private readonly IInteractiveMode im;
    private readonly AppSetting appSetting;
    private readonly SampleService ts;

    public SampleJob(
            ILogger<SampleJob> logger,
            IInteractiveMode im,
            IOptions<AppSetting> configuration,
            MyTaskResolver taskResolver)
    {
        this.jobClass = nameof(SampleJob);
        this.logger = logger;
        this.im = im;
        this.appSetting = configuration.Value;
        this.ts = taskResolver(nameof(SampleService)) as SampleService;
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
            await this.ts.RunAsync();
            isSuccess = true;
        }
        catch (System.Exception ex)
        {
            isSuccess = false;
            this.logger.LogError($"\"{jobClass}\" failed. {ex.Message}");
            throw new Quartz.JobExecutionException(msg: $"Start retrying {jobClass}", cause: ex, refireImmediately: true);
        }
        finally
        {
            var msg = $"\"{jobClass}\" {(isSuccess ? "succeeded" : "failed")}";
            // Logging
            this.logger.LogInformation(msg);
            // stdout (interactive mode)
            if (Environment.GetEnvironmentVariable(EnvConstants.InteractiveMode) == EnvConstants.True)
            {
                await this.im.ConsoleLogAsync(msg, isSuccess ? Models.Enums.Colors.Yellow : Models.Enums.Colors.Red);
            }
        }
    }
}
