using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz.Net.Sample.Services;
using Quartz.Net.Sample.Utils.Constants;
using NLog;
using NLog.Web;
using Microsoft.Extensions.Configuration;
using Quartz.Net.Sample.Models.Config;

namespace Quartz.Net.Sample;
public class Program
{
    public static async Task Main(string[] args)
    {
        // var logger = LogManager.GetCurrentClassLogger();
        var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
        logger.Debug($"Program started at {DateTime.Now.ToString()}...");

        try
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
        catch (System.Exception ex)
        {
            logger.Error(ex);
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }

    public static async Task<IHostBuilder> CreateHostBuilderAsync(string[] args, bool isInteractive = false)
    {
        var hostBuilder = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
            })
            .ConfigureAppConfiguration((context, config) =>
                    {
                        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true)
                        .AddEnvironmentVariables();
                    })
        .ConfigureServices((hostContext, services) =>
                {
                    // Inject configurations
                    services.AddOptions();
                    services.Configure<AppSetting>(hostContext.Configuration);

                })
        .UseNLog();

        return await Task.FromResult(hostBuilder);
    }
}

