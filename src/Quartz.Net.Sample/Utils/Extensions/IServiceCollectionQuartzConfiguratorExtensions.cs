using Microsoft.Extensions.Configuration;
using Quartz.Net.Sample.Jobs;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Utils.Constants;

namespace Quartz.Net.Sample.Utils.Extensions;

public static class IServiceCollectionQuartzConfiguratorExtensions
{
    public static IServiceCollectionQuartzConfigurator UseQuartzJobs(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration)
    {
        var jobName = string.Empty;

        #region Testing Quartz Trigger
#if DEBUG
        // Mock system current time to test Trigger.
        var mockOption = new MockOption();
        configuration.Bind("Mock", mockOption);
        var mockCurrentDateTime = mockOption.CurrentDateTime;
        if (!string.IsNullOrEmpty(mockCurrentDateTime))
        {
            SystemTime.UtcNow = () => DateTime.Parse(mockOption.CurrentDateTime);
        }
        // Or simply use below.
        // SystemTime.UtcNow = () => new DateTime(2023, 08, 31, 02, 0, 0);
#endif
        #endregion

        #region Add job and trigger

        quartz.AddJobAndTrigger<SampleJob>(configuration);
        quartz.AddJobAndTrigger<MyDailyJob>(configuration);
        quartz.AddJobAndTrigger<MyWeeklyJob>(configuration);
        quartz.AddJobAndTrigger<MyMonthlyJob>(configuration);

        #endregion

        return quartz;
    }

    public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration) where T : IJob
    {
        string defaultJobName = typeof(T).Name;

        // Get the cron expression from config file
        var configKey = $"Quartz:Job:{defaultJobName}";
        var cronExpression = configuration[configKey];
        if (string.IsNullOrEmpty(cronExpression))
        {
            throw new Exception($"The cron expression for job {defaultJobName} could not be found! Make sure ${configKey} is set in the config file.");
        }

        // Register the job
        var jobKey = new JobKey(defaultJobName);
        quartz.AddJob<T>(opts => opts.WithIdentity(jobKey).StoreDurably(true));

        // Register trigger (can set multiple triggers to a job
        if (Environment.GetEnvironmentVariable(EnvConstants.InteractiveMode) == EnvConstants.False)
        {
            quartz.AddTrigger(opts => opts.ForJob(jobKey).WithIdentity($"{defaultJobName}-trigger").WithCronSchedule(cronExpression));
        }
    }
}
