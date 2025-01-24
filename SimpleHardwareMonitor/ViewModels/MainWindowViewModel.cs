using CommunityToolkit.Mvvm.ComponentModel;
using SimpleHardwareMonitor.Models;
using SimpleHardwareMonitor.Services;

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
        }
        [ObservableProperty]
        private HardwareInfo _hardware;
    }
}
