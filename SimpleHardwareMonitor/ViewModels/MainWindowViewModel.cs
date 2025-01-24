using CommunityToolkit.Mvvm.ComponentModel;
using SimpleHardwareMonitor.Models;
using SimpleHardwareMonitor.Services;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IHardwareService _hardwareService;
        public MainWindowViewModel(IHardwareService hardwareService)
        {
            _hardwareService = hardwareService;
            Hardware = _hardwareService.GetHardwareInfo();

            _ = _hardwareService.RunRefresh();
            _ = RunRefresh();
        }
        [ObservableProperty]
        private HardwareInfo _hardware;
        private async Task RunRefresh()
        {
            while(true)
            {
                Hardware = _hardwareService.GetHardwareInfo();
                await Task.Delay(1000);
            }
        }
    }
}
