﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHardwareMonitor.Models
{
    public class GpuData
    {
        public string? Name { get; set; }
        public float? Temperature { get; set; }
        public float? Load { get; set; }
        public float? MemoryTotal { get; set; }
        public float? MemoryFree { get; set; }
        public float? MemoryUsed { get; set; }
    }
}
