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
