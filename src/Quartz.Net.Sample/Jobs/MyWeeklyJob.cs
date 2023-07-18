using System.ComponentModel;
using Quartz;

namespace Quartz.Net.Sample.Jobs;

[DisallowConcurrentExecution]
[Description("My Daily Job")]
public class MyWeeklyJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {

    }
}
