using SimpleHardwareMonitor.Models;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Services
{
    public interface IHardwareService
    {
        public HardwareInfo GetHardwareInfo();
        public Task RunRefresh();
    }
}
