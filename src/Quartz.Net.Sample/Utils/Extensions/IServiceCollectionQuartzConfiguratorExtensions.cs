using Microsoft.Extensions.Configuration;
using Quartz.Net.Sample.Jobs;
using Quartz.Net.Sample.Utils.Constants;

namespace Quartz.Net.Sample.Utils.Extensions;

public static class IServiceCollectionQuartzConfiguratorExtensions
{
    public static IServiceCollectionQuartzConfigurator UseQuartzJobs(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration)
    {
        var jobName = string.Empty;

        #region Add job and trigger

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
