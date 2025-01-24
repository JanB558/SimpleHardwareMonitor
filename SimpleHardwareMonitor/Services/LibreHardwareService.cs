using LibreHardwareMonitor.Hardware;
using SimpleHardwareMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Services
{
    public class LibreHardwareService : IHardwareService
    {
        private readonly Computer _computer;

        public LibreHardwareService()
        {
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
            Init();
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

        #region GPU
        private void GetGPUs()
        {
            var gpuTypes = new[] { HardwareType.GpuIntel, HardwareType.GpuAmd, HardwareType.GpuNvidia };
            HardwareInfo.GPUs = gpuTypes.SelectMany(type => _computer.Hardware.Where(h => h.HardwareType == type)).ToList();
        }
        private void GetGPUsSensors()
        {
            foreach (var gpu in HardwareInfo.GPUs)
            {
                var gpuTemp = gpu.Sensors.FirstOrDefault(x => x.SensorType.Equals(SensorType.Temperature));
                var gpuLoad = gpu.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Contains("D3D 3D"));
                var gpuMemoryTotal = gpu.Sensors.Where(x => x.SensorType == SensorType.SmallData).FirstOrDefault(x => x.Name.Contains("GPU Memory Total"));
                var gpuMemoryUsed = gpu.Sensors.Where(x => x.SensorType == SensorType.SmallData).FirstOrDefault(x => x.Name.Contains("GPU Memory Used"));
                var gpuMemoryFree = gpu.Sensors.Where(x => x.SensorType == SensorType.SmallData).FirstOrDefault(x => x.Name.Contains("GPU Memory Free"));
                HardwareInfo.GPUsTemperature.Add(gpuTemp);
                HardwareInfo.GPUsLoad.Add(gpuLoad);
                HardwareInfo.GPUsMemoryTotal.Add(gpuMemoryTotal);
                HardwareInfo.GPUsMemoryFree.Add(gpuMemoryFree);
                HardwareInfo.GPUsMemoryUsed.Add(gpuMemoryUsed);
            }
        }
        #endregion
        #region CPU
        private void GetCPU()
        {
            HardwareInfo.CPU = _computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.Cpu);
        }
        private void GetCPUSensors()
        {
            if (HardwareInfo.CPU is not null)
            {
                var t = HardwareInfo.CPU.Sensors.Where(x => x.SensorType == SensorType.Temperature)
                            .Where(x => !x.Name.Contains("Core")).ToList();

                if (t.Count > 1) { throw new Exception("Unexpected count of CPU Temperature sensors."); }

                HardwareInfo.CPUTemperature = t.FirstOrDefault();
                HardwareInfo.CPULoad = HardwareInfo.CPU.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Contains("Total"));
                HardwareInfo.ThreadsLoad = HardwareInfo.CPU.Sensors.Where(x => x.SensorType == SensorType.Load).Where(x => x.Name.Contains("CPU Core #")).ToList();
            }
        }
        #endregion
        #region Memory
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
            }
        }
        #endregion
        #region Drives
        private void GetDrives()
        {
            HardwareInfo.Drives = _computer.Hardware.Where(x => x.HardwareType == HardwareType.Storage).ToList();
        }
        private void GetDrivesSensors()
        {
            foreach (var drive in HardwareInfo.Drives)
            {
                var ts = drive.Sensors.FirstOrDefault(x => x.SensorType == SensorType.Temperature);
                var acs = drive.Sensors.Where(x => x.SensorType == SensorType.Load).FirstOrDefault(x => x.Name.Equals("Total Activity"));
                HardwareInfo.DrivesTemperature.Add(ts);
                HardwareInfo.DrivesActivity.Add(acs);
            }
        }
        #endregion
        #region Motherboard
        private void GetMotherboard()
        {
            HardwareInfo.Motherboard = _computer.Hardware.FirstOrDefault(x => x.HardwareType == HardwareType.Motherboard);
        }
        private void GetMotherboardSensors()
        {
            if (HardwareInfo.Motherboard is not null)
                HardwareInfo.MotherboardTemperature = HardwareInfo.Motherboard.Sensors.Where(x => x.SensorType == SensorType.Temperature).ToList();
        }
        #endregion
    }
}
