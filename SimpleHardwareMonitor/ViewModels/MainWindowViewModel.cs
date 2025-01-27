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
        private System.Threading.Timer _refreshTimer;
        public MainWindowViewModel(IHardwareService hardwareService)
        {
            _hardwareService = hardwareService;
            Hardware = _hardwareService.GetHardwareInfo();

            _ = _hardwareService.RunRefresh();
            _refreshTimer = new System.Threading.Timer(RefreshHardwareInfo, null, 0, 1000);
        }
        [ObservableProperty]
        private HardwareInfo _hardware;

        public float? CpuTemperature => Hardware?.CPUTemperature?.Value;
        public float? CpuLoad => Hardware?.CPULoad?.Value;

        private void RefreshHardwareInfo(object? state)
        {
            OnPropertyChanged(nameof(CpuTemperature));
            OnPropertyChanged(nameof(CpuLoad));
        }
    }
}
