using LibreHardwareMonitor.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Models
{
    public class HardwareInfo
    {
        public List<IHardware> GPUs = [];
        public List<ISensor?> GPUsTemperature = [];
        public List<ISensor?> GPUsLoad = [];
        public List<ISensor?> GPUsMemoryTotal = [];
        public List<ISensor?> GPUsMemoryFree = [];
        public List<ISensor?> GPUsMemoryUsed = [];

        public IHardware? CPU;
        public ISensor? CPUTemperature;
        public ISensor? CPULoad;
        public List<ISensor> ThreadsLoad = [];

        public IHardware? Memory;
        public ISensor? MemoryTemperature;
        public ISensor? MemoryUsed;
        public ISensor? MemoryFree;
        public ISensor? MemoryLoad;
        public int MemoryTotal;

        public List<IHardware> Drives = [];
        public List<ISensor?> DrivesTemperature = [];
        public List<ISensor?> DrivesActivity = [];

        public IHardware? Motherboard;
        public List<ISensor?> MotherboardTemperature = [];
    }
}
