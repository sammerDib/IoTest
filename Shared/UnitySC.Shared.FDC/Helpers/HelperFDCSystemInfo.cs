using System.Linq;
using System.Management;

namespace UnitySC.Shared.FDC.Helpers
{
    public static class HelperFDCSystemInfo
    {
        public static double GetMemoryUsagePercentage()
        {
            var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");

            var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
            {
                FreePhysicalMemory = double.Parse(mo["FreePhysicalMemory"].ToString()),
                TotalVisibleMemorySize = double.Parse(mo["TotalVisibleMemorySize"].ToString())
            }).FirstOrDefault();

            if (memoryValues != null && memoryValues.TotalVisibleMemorySize > 0)
            {
                return ((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100;
            }

            return 0.0;
        }

        public static double GetCPUUsagePercentageVMI()
        {
            // The LoadPercentage property specifies each processor's load capacity averaged over the last second.
            // The term 'processor loading' refers to the total computing burden each processor carries at one time.
            var wmiObject = new ManagementObjectSearcher("select * from Win32_Processor");

            var cpuValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
            {
                LoadPercentage = int.Parse(mo["LoadPercentage"].ToString())
            }).FirstOrDefault();

            return cpuValues != null ? cpuValues.LoadPercentage : 0d;
        }
    }
}
