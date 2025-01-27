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
            => new ObservableCollection<IndexValue>(
                Hardware.ThreadsLoad.Select((x, index) => new IndexValue
                {
                    Value = x.Value ?? 0f,
                    Index = index
                }).ToList() ?? new List<IndexValue>()
                );
        #endregion
        #region mobo
        public ObservableCollection<IndexValue> MotherboardTemperature
            => new ObservableCollection<IndexValue>(
                Hardware.MotherboardTemperature.Select((x, index) => new IndexValue
                {
                    Value = x.Value ?? 0f,
                    Index = index
                }).ToList() ?? new List<IndexValue>()
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
        private ObservableCollection<string> GpuNames
            => new(Hardware.GPUs.Select(x => x?.Name ?? string.Empty).ToList());
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
        }

        private ICollection<GpuData> CreateGpuData()
        {
            List<GpuData> output = [];
            for (int i = 0; i < GpuNames.Count; i++)
            {
                var model = new GpuData();
                model.Name = GpuNames[i];
                model.Temperature = GpuTemperature[i];
                model.Load = GpuLoad[i];
                model.MemoryTotal = GpuMemoryTotal[i];
                model.MemoryFree = GpuMemoryFree[i];
                model.MemoryUsed = GpuMemoryUsed[i];
                output.Add(model);
            }
            return output;
        }
    }
}
