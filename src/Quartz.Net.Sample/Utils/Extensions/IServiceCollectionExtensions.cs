using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Quartz.Net.Sample.Services;

namespace Quartz.Net.Sample;

public static class IServiceCollectionExtensions
{
    public delegate IMyTaskService MyTaskResolver(string className);

    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDataAccessRepositories(this IServiceCollection services)
        {


            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            #region Various implementations on same interface

            // Resolver
            services.AddSingleton<>(sp => className =>
                    {
                        var myTask = sp.GetServices<IDataAccessRepository>().Where(x => x.GetType().Name.Equals(className)).FirstOrDefault();
                        return myTask;
                    });

            // Inject various MyTask implementations.
            services.AddSingleton<IMyTaskService, MyTaskService>();
            #endregion

            services.AddScoped<IInteractiveMode, InteractiveMode>();
            return services;
        }
    }

}
