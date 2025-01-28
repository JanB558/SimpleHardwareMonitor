using LibreHardwareMonitor.Hardware;
using SimpleHardwareMonitor.Models;
using SimpleHardwareMonitor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Services
{
    public class LibreHardwareService : IHardwareService, IDisposable
    {
        private readonly Computer _computer;
        private readonly UpdateVisitor _visitor;
        private readonly CancellationTokenSource _cts;
        private bool _disposed;

        public LibreHardwareService()
        {
            _cts = new CancellationTokenSource();

            _computer = new Computer
            {
                IsCpuEnabled = true,
                IsGpuEnabled = true,
                IsMemoryEnabled = true,
                IsMotherboardEnabled = true,
                IsStorageEnabled = true,

                IsBatteryEnabled = false,
                IsControllerEnabled = false,
                IsNetworkEnabled = false,
                IsPsuEnabled = false
            };

            HardwareInfo = new HardwareInfo();

            _computer.Open();
            _visitor = new UpdateVisitor();
            _computer.Accept(_visitor);

            Init();
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _cts.Cancel();
            _computer.Close();
            _cts.Dispose();
            GC.SuppressFinalize(this);
        }

        public HardwareInfo HardwareInfo { get; set; }

        public HardwareInfo GetHardwareInfo()
        {
            return HardwareInfo;
        }

        private void Init()
        {
            GetGPUs();
            GetGPUsSensors();
            GetCPU();
            GetCPUSensors();
            GetMemory();
            GetMemorySensors();
            GetDrives();
            GetDrivesSensors();
            GetMotherboard();
            GetMotherboardSensors();
        }

        public async Task RunRefresh()
        {
            await Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    _computer.Traverse(_visitor);
                    await Task.Delay(1000, _cts.Token);
                }
            }, _cts.Token);
        }

        private void GetGPUs()
        {
            var gpuTypes = new[] { HardwareType.GpuIntel, HardwareType.GpuAmd, HardwareType.GpuNvidia };
            HardwareInfo.GPUs = gpuTypes.SelectMany(type => _computer.Hardware.Where(h => h.HardwareType == type)).ToList();
        }
        private void GetGPUsSensors()
        {
            foreach (var gpu in HardwareInfo.GPUs)
            {
                HardwareInfo.GPUsTemperature.Add(gpu.Sensors.FirstOrDefault(x => x.SensorType.Equals(SensorType.Temperature)));
                HardwareInfo.GPUsLoad.Add(gpu.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Contains("D3D 3D")));
                HardwareInfo.GPUsMemoryTotal.Add(gpu.Sensors.Where(x => x.SensorType == SensorType.SmallData).FirstOrDefault(x => x.Name.Contains("GPU Memory Total")));
                HardwareInfo.GPUsMemoryUsed.Add(gpu.Sensors.Where(x => x.SensorType == SensorType.SmallData).FirstOrDefault(x => x.Name.Contains("GPU Memory Used")));
                HardwareInfo.GPUsMemoryFree.Add(gpu.Sensors.Where(x => x.SensorType == SensorType.SmallData).FirstOrDefault(x => x.Name.Contains("GPU Memory Free")));
            }
        }

        private void GetCPU()
        {
            HardwareInfo.CPU = _computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.Cpu);
        }
        private void GetCPUSensors()
        {
            if (HardwareInfo.CPU is not null)
            {
                try
                {
                    var t = HardwareInfo.CPU.Sensors.Where(x => x.SensorType == SensorType.Temperature)
                                .Where(x => !x.Name.Contains("Core")).ToList();
                    if (t.Count > 1) { throw new Exception("Unexpected count of CPU Temperature sensors."); }
                    HardwareInfo.CPUTemperature = t.FirstOrDefault();
                }catch(Exception e)
                {
                    Logger.LogError(e.Message);
                }
                HardwareInfo.CPULoad = HardwareInfo.CPU.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Contains("Total"));
                HardwareInfo.ThreadsLoad = HardwareInfo.CPU.Sensors.Where(x => x.SensorType == SensorType.Load).Where(x => x.Name.Contains("CPU Core #")).ToList();
            }
        }

        private void GetMemory()
        {
            HardwareInfo.Memory = _computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.Memory);
        }
        private void GetMemorySensors()
        {
            if (HardwareInfo.Memory is not null)
            {
                HardwareInfo.MemoryTemperature = HardwareInfo.Memory.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature);
                HardwareInfo.MemoryUsed = HardwareInfo.Memory.Sensors.Where(x => x.SensorType == SensorType.Data).FirstOrDefault(x => x.Name.Contains("Memory Used") && !x.Name.Contains("Virtual"));
                HardwareInfo.MemoryFree = HardwareInfo.Memory.Sensors.Where(x => x.SensorType == SensorType.Data).FirstOrDefault(x => x.Name.Contains("Memory Available") && !x.Name.Contains("Virtual"));
                HardwareInfo.MemoryLoad = HardwareInfo.Memory.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Contains("Memory") && !x.Name.Contains("Virtual"));
                if (HardwareInfo.MemoryUsed is not null && HardwareInfo.MemoryFree is not null)
                {
                    var total = HardwareInfo.MemoryUsed.Value + HardwareInfo.MemoryFree.Value;
                    HardwareInfo.MemoryTotal = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(total)));
                }
                else
                {
                    HardwareInfo.MemoryTotal = null;
                }
            }
        }

        private void GetDrives()
        {
            HardwareInfo.Drives = _computer.Hardware.Where(x => x.HardwareType == HardwareType.Storage).ToList();
        }
        private void GetDrivesSensors()
        {
            foreach (var drive in HardwareInfo.Drives)
            {
                HardwareInfo.DrivesTemperature.Add(drive.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature));
                HardwareInfo.DrivesActivity.Add(drive.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Equals("Total Activity")));
            }
        }

        private void GetMotherboard()
        {
            HardwareInfo.Motherboard = _computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.Motherboard);
        }
        private void GetMotherboardSensors()
        {
            if (HardwareInfo.Motherboard is not null)
                HardwareInfo.MotherboardTemperature = HardwareInfo.Motherboard.Sensors.Where(x => x.SensorType == SensorType.Temperature).ToList();
        }
    }
}
