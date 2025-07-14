using System.Collections.Generic;

namespace UnitySC.PM.Shared.Hardware.Communication
{
    public class PlcProcessModule
    {
        public Dictionary<string, List<OpcDevice>> PlcPms { get; set; }
    }
}