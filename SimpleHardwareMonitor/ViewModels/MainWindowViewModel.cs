using Avalonia.Threading;
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
        private readonly System.Threading.Timer _refreshTimer; //no need to dispose since it's one page application
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
        public float? CpuTemperature => Hardware.CPUTemperature?.Value;
        public float? CpuLoad => Hardware.CPULoad?.Value;
        private ObservableCollection<IndexValue> _threadsLoad = new();
        public ObservableCollection<IndexValue> ThreadsLoad => _threadsLoad;
        #endregion
        #region mobo
        private ObservableCollection<IndexValue> _motherboardTemperature = new();
        public ObservableCollection<IndexValue> MotherboardTemperature => _motherboardTemperature;

        #endregion
        #region memory
        public float? MemoryTemperature => Hardware.MemoryTemperature?.Value;
        public float? MemoryUsed => Hardware.MemoryUsed?.Value;
        public float? MemoryFree => Hardware.MemoryFree?.Value;
        public float? MemoryLoad => Hardware.MemoryLoad?.Value;
        public int? MemoryTotal => Hardware.MemoryTotal;
        #endregion
        #region gpu
        private ObservableCollection<GpuData> _gpuDataCombined = new();
        public ObservableCollection<GpuData> GpuDataCombined => _gpuDataCombined;
        #endregion
        #region drives
        private ObservableCollection<DriveData> _driveDataCombined = new();
        public ObservableCollection<DriveData> DriveDataCombined => _driveDataCombined;
        #endregion

        private void RefreshHardwareInfo(object? state)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                //collections need to be updated on UI thread to avoid duplication
                UpdateThreadsLoad();
                UpdateMotherboardTemperature();
                UpdateGpuDataCombined();
                UpdateDriveDataCombined();

                OnPropertyChanged(nameof(CpuTemperature));
                OnPropertyChanged(nameof(CpuLoad));
                OnPropertyChanged(nameof(ThreadsLoad));

                OnPropertyChanged(nameof(MemoryTemperature));
                OnPropertyChanged(nameof(MemoryUsed));
                OnPropertyChanged(nameof(MemoryFree));
                OnPropertyChanged(nameof(MemoryLoad));
            });
        }

        #region collection_handling
        private void UpdateThreadsLoad()
        {
            var newData = Hardware.ThreadsLoad.Select((x, index) => new IndexValue
            {
                Value = x.Value,
                Index = index
            }).ToList();
            _threadsLoad.Clear();
            foreach (var item in newData) _threadsLoad.Add(item);
        }
        private void UpdateMotherboardTemperature()
        {
            var newData = Hardware.MotherboardTemperature.Select((x, index) => new IndexValue
            {
                Value = x?.Value,
                Index = index
            }).ToList();
            _motherboardTemperature.Clear();
            foreach (var item in newData) _motherboardTemperature.Add(item);

        }
        private void UpdateGpuDataCombined()
        {
            var gpuNames = Hardware.GPUs.Select(x => x?.Name).ToList();
            var gpuTemperature = Hardware.GPUsTemperature.Select(x => x?.Value).ToList();
            var gpuLoad = Hardware.GPUsLoad.Select(x => x?.Value).ToList();
            var gpuMemoryTotal = Hardware.GPUsMemoryTotal.Select(x => x?.Value).ToList();
            var gpuMemoryFree = Hardware.GPUsMemoryFree.Select(x => x?.Value).ToList();
            var gpuMemoryUsed = Hardware.GPUsMemoryUsed.Select(x => x?.Value).ToList();
            _gpuDataCombined.Clear();
            for (int i = 0; i < gpuNames.Count; i++)
            {
                var model = new GpuData
                {
                    Name = gpuNames[i],
                    Temperature = gpuTemperature[i],
                    Load = gpuLoad[i],
                    MemoryTotal = gpuMemoryTotal[i],
                    MemoryFree = gpuMemoryFree[i],
                    MemoryUsed = gpuMemoryUsed[i]
                };
                _gpuDataCombined.Add(model);
            }
        }
        private void UpdateDriveDataCombined()
        {
            var driveNames = Hardware.Drives.Select(x => x?.Name).ToList(); //this can be limited to run once
            var driveTemperature = Hardware.DrivesTemperature.Select(x => x?.Value).ToList();
            var driveActivity = Hardware.DrivesActivity.Select(x => x?.Value).ToList();
            _driveDataCombined.Clear();
            for (int i = 0; i < driveNames.Count; i++)
            {
                var model = new DriveData
                {
                    Name = driveNames[i],
                    Temperature = driveTemperature[i],
                    Activity = driveActivity[i]
                };
                _driveDataCombined.Add(model);
            }
        }
        #endregion
    }
}
