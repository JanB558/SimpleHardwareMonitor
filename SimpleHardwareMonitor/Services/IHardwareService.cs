using SimpleHardwareMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Services
{
    public interface IHardwareService
    {
        public HardwareInfo GetHardwareInfo();
    }
}
