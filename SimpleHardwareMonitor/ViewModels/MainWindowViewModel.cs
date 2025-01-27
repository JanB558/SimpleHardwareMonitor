using CommunityToolkit.Mvvm.ComponentModel;
using SimpleHardwareMonitor.Models;
using SimpleHardwareMonitor.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IHardwareService _hardwareService;
        private readonly System.Threading.Timer _refreshTimer;
        public MainWindowViewModel(IHardwareService hardwareService)
        {
            _hardwareService = hardwareService;
            Hardware = _hardwareService.GetHardwareInfo();

            _ = _hardwareService.RunRefresh();
            _refreshTimer = new System.Threading.Timer(RefreshHardwareInfo, null, 0, 1000);
        }
        [ObservableProperty]
        private HardwareInfo _hardware;

        //These properties are needed because when _hardware properties are updated,
        //user interface is not informed about the update.
        //Implementing INotifyPropertyChanged is too complicated since ISensor 
        //comes from nuget and cannot be directly edited (maybe could be with wrapper).
        //There are probably other solutions to this problem.
        #region cpu
        public float? CpuTemperature => Hardware?.CPUTemperature?.Value;
        public float? CpuLoad => Hardware?.CPULoad?.Value;
        public ObservableCollection<IndexValue> ThreadsLoad
            => new(
                Hardware.ThreadsLoad.Select((x, index) => new IndexValue
                {
                    Value = x.Value ?? 0f,
                    Index = index
                }).ToList() ?? []
                );
        #endregion
        #region mobo
        public ObservableCollection<IndexValue> MotherboardTemperature
            => new(
                Hardware.MotherboardTemperature.Select((x, index) => new IndexValue
                {
                    Value = x?.Value ?? 0f,
                    Index = index
                }).ToList() ?? []
                );
        #endregion
        #region memory
        public float? MemoryTemperature => Hardware?.MemoryTemperature?.Value;
        public float? MemoryUsed => Hardware?.MemoryUsed?.Value;
        public float? MemoryFree => Hardware?.MemoryFree?.Value;
        public float? MemoryLoad => Hardware?.MemoryLoad?.Value;
        public int? MemoryTotal => Hardware?.MemoryTotal;
        #endregion
        #region gpu
        private ObservableCollection<string?> GpuNames
            => new(Hardware.GPUs.Select(x => x?.Name).ToList());
        private ObservableCollection<float?> GpuTemperature
            => new(Hardware.GPUsTemperature.Select(x => x?.Value).ToList());
        private ObservableCollection<float?> GpuLoad
            => new(Hardware.GPUsLoad.Select(x => x?.Value).ToList());
        private ObservableCollection<float?> GpuMemoryTotal
            => new(Hardware.GPUsMemoryTotal.Select(x => x?.Value).ToList());
        private ObservableCollection<float?> GpuMemoryFree
            => new(Hardware.GPUsMemoryFree.Select(x => x?.Value).ToList());
        private ObservableCollection<float?> GpuMemoryUsed
            => new(Hardware.GPUsMemoryUsed.Select(x => x?.Value).ToList());
        public ObservableCollection<GpuData> GpuDataCombined
            => new(CreateGpuData().ToList());
        #endregion
        #region drives
        private ObservableCollection<string?> DriveNames
            => new(Hardware.Drives.Select(x => x?.Name).ToList());  
        private ObservableCollection<float?> DriveTemperature
            => new(Hardware.DrivesTemperature.Select(x => x?.Value).ToList());
        private ObservableCollection<float?> DriveActivity
            => new(Hardware.DrivesActivity.Select(x => x?.Value).ToList());
        public ObservableCollection<DriveData> DriveDataCombined
            => new(CreateDriveData().ToList());
        #endregion

        private void RefreshHardwareInfo(object? state)
        {
            OnPropertyChanged(nameof(CpuTemperature));
            OnPropertyChanged(nameof(CpuLoad));
            OnPropertyChanged(nameof(ThreadsLoad));
            OnPropertyChanged(nameof(MotherboardTemperature));
            OnPropertyChanged(nameof(MemoryTemperature));
            OnPropertyChanged(nameof(MemoryUsed));
            OnPropertyChanged(nameof(MemoryFree));
            OnPropertyChanged(nameof(MemoryLoad));
            OnPropertyChanged(nameof(GpuDataCombined));
            OnPropertyChanged(nameof(DriveDataCombined));
        }

        private ICollection<GpuData> CreateGpuData()
        {
            List<GpuData> output = [];
            for (int i = 0; i < GpuNames.Count; i++)
            {
                var model = new GpuData
                {
                    Name = GpuNames[i],
                    Temperature = GpuTemperature[i],
                    Load = GpuLoad[i],
                    MemoryTotal = GpuMemoryTotal[i],
                    MemoryFree = GpuMemoryFree[i],
                    MemoryUsed = GpuMemoryUsed[i]
                };
                output.Add(model);
            }
            return output;
        }

        private ICollection<DriveData> CreateDriveData()
        {
            List<DriveData> output = [];
            for(int i = 0;i < DriveNames.Count;i++)
            {
                var model = new DriveData
                {
                    Name = DriveNames[i],
                    Temperature = DriveTemperature[i],
                    Activity = DriveActivity[i]
                };
                output.Add(model);
            }
            return output;
        }
    }
}
