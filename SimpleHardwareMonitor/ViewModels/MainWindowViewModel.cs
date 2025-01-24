using SimpleHardwareMonitor.Services;

namespace SimpleHardwareMonitor.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private readonly IHardwareService _hardwareService;
        public MainWindowViewModel(IHardwareService hardwareService)
        {
            _hardwareService = hardwareService;
        }
        public string Greeting { get; } = "Welcome to Avalonia!";
    }
}
