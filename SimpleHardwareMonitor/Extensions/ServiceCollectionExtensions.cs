using Microsoft.Extensions.DependencyInjection;
using SimpleHardwareMonitor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
