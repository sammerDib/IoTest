using System;
using System.Diagnostics;
using System.IO;

namespace ADCControls
{
    public class PCRessourcesMonitor
    {
        private PerformanceCounter _perfCounter_Processor = null;
        private PerformanceCounter _perfCounter_Memory = null;

        public PCRessourcesMonitor()
        {
            // English language
            if (PerformanceCounterCategory.CounterExists("% Processor Time", "Processor"))
                _perfCounter_Processor = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            // French language
            if ((_perfCounter_Processor == null) && (PerformanceCounterCategory.CounterExists("% temps processeur", "Processeur")))
                _perfCounter_Processor = new PerformanceCounter("Processeur", "% temps processeur", "_Total");
            // English language
            if (PerformanceCounterCategory.CounterExists("Available MBytes", "Memory"))
                _perfCounter_Memory = new PerformanceCounter("Memory", "Available MBytes");
            // French language
            if ((_perfCounter_Memory == null) && PerformanceCounterCategory.CounterExists("Mégaoctets disponibles", "Mémoire"))
                _perfCounter_Memory = new PerformanceCounter("Mémoire", "Mégaoctets disponibles");
        }

        public FDCInfo GetInfo_Snapshot()
        {
            FDCInfo info = new FDCInfo();
            info.Module_Type = "ADC";
            info.PC_Name = System.Environment.MachineName;
            Process proc = Process.GetCurrentProcess();

            info.UsageCPU_percent = _perfCounter_Processor.NextValue();
            info.Memory_MB = _perfCounter_Memory.NextValue();

            DriveInfo[] disks = DriveInfo.GetDrives();
            foreach (var disk in disks)
            {
                if (disk.DriveType == DriveType.Fixed)
                {
                    FDCInfo_Disk newInfoDisk = new FDCInfo_Disk();
                    newInfoDisk.Name = disk.Name;
                    newInfoDisk.FreeSpace = Math.Ceiling(Convert.ToDouble(disk.TotalFreeSpace) / (double)1000000);
                    info.DrivesInfo.Add(newInfoDisk);
                }
            }

            return info;
        }
    }
}
