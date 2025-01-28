using LibreHardwareMonitor.Hardware;
using System.Collections.Generic;

namespace SimpleHardwareMonitor.Models
{
    public class HardwareInfo
    {
        public List<IHardware> GPUs { get; set; } = [];
        public List<ISensor?> GPUsTemperature { get; set; } = [];
        public List<ISensor?> GPUsLoad { get; set; } = [];
        public List<ISensor?> GPUsMemoryTotal { get; set; } = [];
        public List<ISensor?> GPUsMemoryFree { get; set; } = [];
        public List<ISensor?> GPUsMemoryUsed { get; set; } = [];

        public IHardware? CPU { get; set; }
        public ISensor? CPUTemperature { get; set; }
        public ISensor? CPULoad { get; set; }
        public List<ISensor> ThreadsLoad { get; set; } = [];

        public IHardware? Memory { get; set; }
        public ISensor? MemoryTemperature { get; set; }
        public ISensor? MemoryUsed { get; set; }
        public ISensor? MemoryFree { get; set; }
        public ISensor? MemoryLoad { get; set; }
        public int? MemoryTotal { get; set; }

        public List<IHardware> Drives { get; set; } = [];
        public List<ISensor?> DrivesTemperature { get; set; } = [];
        public List<ISensor?> DrivesActivity { get; set; } = [];

        public IHardware? Motherboard { get; set; }
        public List<ISensor?> MotherboardTemperature { get; set; } = [];
    }
}
