using Microsoft.Extensions.DependencyInjection;
using SimpleHardwareMonitor.Services;

namespace SimpleHardwareMonitor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCommonServices(this IServiceCollection collection)
        {
            collection.AddSingleton<IHardwareService, LibreHardwareService>();
        }
    }
}
