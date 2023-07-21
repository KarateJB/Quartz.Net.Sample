using Microsoft.Extensions.DependencyInjection;
using Quartz.Net.Sample.Services;

namespace Quartz.Net.Sample.Utils.Extensions;

public static class IServiceCollectionExtensions
{
    public delegate IMyTaskService MyTaskResolver(string className);

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        #region Various implementations on same interface

        // Resolver
        services.AddSingleton<MyTaskResolver>(sp => className =>
                {
                    var myTask = sp.GetServices<IMyTaskService>().Where(x => x.GetType().Name.Equals(className)).FirstOrDefault();
                    return myTask;
                });

        // Inject various task implementations.
        services.AddSingleton<IMyTaskService, HelloWorldService>();
        services.AddSingleton<IMyTaskService, HelloVimService>();
        services.AddSingleton<IMyTaskService, HelloDotnetService>();
        #endregion

        services.AddScoped<IInteractiveMode, InteractiveMode>();
        return services;
    }
}
