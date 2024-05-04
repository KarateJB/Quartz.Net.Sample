using NLog.Web;
using NLog;
using Quartz.Net.Sample.Models.Config;
using Quartz.Net.Sample.Utils.Constants;
using Quartz.Net.Sample.Utils.Extensions;

namespace Quartz.Net.Sample;

public class Program
{
    public static async Task Main(string[] args)
    {
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
            .UseEnvironment(Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development")
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);

                WriteLine($"Current environment: {context.HostingEnvironment.EnvironmentName}");
                if (context.HostingEnvironment.IsDevelopment() && !isInteractive)
                {
                    logging.AddConsole();
                }
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

                    // Inject services
                    services.AddServices();

                    services.AddQuartz(q =>
                            {
                                // Register an IJobFactory to create jobs from DI container.
                                q.UseMicrosoftDependencyInjectionJobFactory();
                                // Register the custom jobs.
                                q.UseQuartzJobs(hostContext.Configuration);
                            });

                    // Add Quartz hosted service.
                    services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                })
        .UseNLog();

        return await Task.FromResult(hostBuilder);
    }
}
