using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Models
{
    public class DriveData
    {
        public string? Name { get; set; }
        public float? Temperature { get; set; }
        public float? Activity { get; set; }
    }
}
