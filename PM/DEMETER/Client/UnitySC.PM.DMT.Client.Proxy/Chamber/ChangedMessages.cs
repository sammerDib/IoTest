using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Client.Proxy.Chamber
{
    public class IsInMaintenanceChangedMessage
    {
        public bool IsInMaintenance { get; set; }
    }

    public class ArmNotExtendedChangedMessage
    {
        public bool ArmNotExtended { get; set; }
    }

    public class EfemSlitDoorOpenPositionChangedMessage
    {
        public bool EfemSlitDoorOpenPosition { get; set; }
    }

    public class IsReadyToLoadUnloadChangedMessage
    {
        public bool IsReadyToLoadUnload { get; set; }
    }

    public class InterlockChangedMessage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public uint InterlockID { get; set; }
        public bool State { get; set; }
    }

    public class SlitDoorPositionChangedMessage
    {
        public SlitDoorPosition SlitDoorPosition { get; set; }
    }

    public class SlitDoorOpenPositionChangedMessage
    {
        public bool SlitDoorOpenPosition { get; set; }
    }

    public class SlitDoorClosePositionChangedMessage
    {
        public bool SlitDoorClosePosition { get; set; }
    }    

    public class CdaPneumaticValveChangedMessage
    { 
        public bool ValveIsOpened { get; set; }
    }

    
}
