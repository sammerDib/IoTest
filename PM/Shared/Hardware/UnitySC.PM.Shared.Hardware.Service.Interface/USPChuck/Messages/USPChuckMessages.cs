using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck
{
    public class StateMessage
    {
        public DeviceState State;
    }

    public class WaferPresenceMessage
    {
        public Length Diameter;
        public MaterialPresence WaferPresence;        
    }
    public class DataAttributesChuckMessage
    {
        public ChuckState State;
    }

    public class ChuckIsInLoadingPositionMessage
    {
        public bool IsInLoadingPosition;
    }
}
