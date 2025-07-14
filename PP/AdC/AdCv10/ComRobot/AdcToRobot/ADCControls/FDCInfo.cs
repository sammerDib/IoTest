using System;
using System.Collections.Generic;
using System.Linq;

namespace ADCControls
{
    [Serializable]
    public class FDCInfo_Disk
    {
        public double FreeSpace;
        public String Name;
    }

    [Serializable]
    public class FDCInfo : ICloneable
    {
        // Idenfication
        public String Module_Type; // Module type to identify it in tool (ACQUISITION, ADC, Robot)
        public String PC_Name; // PC Name in windows to identify it in network

        // Data
        public float UsageCPU_percent;
        public double Memory_MB;
        public List<FDCInfo_Disk> DrivesInfo = new List<FDCInfo_Disk>();

        public object Clone()
        {
            FDCInfo clone = new FDCInfo();
            clone.DrivesInfo = DrivesInfo.ToList();
            clone.Memory_MB = Memory_MB;
            clone.UsageCPU_percent = UsageCPU_percent;
            return clone;
        }
    }
}