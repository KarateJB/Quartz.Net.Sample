namespace Quartz.Net.Sample.Models.Config;

public class AppSetting
{
    public QuartzOption Quartz { get; set; }
}

public class QuartzOption
{
    public int RetryMaxTimes { get; set; }

    // Skip strong typing, see "IServiceCollectionQuartzConfiguratorExtensions.AddJobAndTrigger".
    // public List<JobOption> Job {get;set;} 
}
