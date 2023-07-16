using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Net.Sample.Services;
using Quartz.Net.Sample.Utils.Constants;
using NLog;

namespace Quartz.Net.Sample;
public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && args[0].Equals("-i"))
        {
            Environment.SetEnvironmentVariable(EnvConstants.InteractiveMode, "1");
            var host = (await CreateHostBuilderAsync(args, isInteractive: true)).Build();
            using (var scope = host.Services.CreateScope())
            {
                var ia = scope.ServiceProvider.GetRequiredService<IInteractiveMode>();
                if (await ia.PromptAsync())
                {
                    Environment.Exit(0);
                }
            }
            // host.Run();
        }
        else
        {
            Environment.SetEnvironmentVariable(EnvConstants.InteractiveMode, "0");
            (await CreateHostBuilderAsync(args)).Build().Run();
        }
    }

    public static async Task<IHostBuilder> CreateHostBuilderAsync(string[] args, bool isInteractive = false)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            });

        return await Task.FromResult(hostBuilder);
    }
}

