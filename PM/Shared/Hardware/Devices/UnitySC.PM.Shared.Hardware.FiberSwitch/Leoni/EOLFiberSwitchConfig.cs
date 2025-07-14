using UnitySC.PM.Shared.Hardware.Service.Interface.Common;

namespace UnitySC.PM.Shared.Hardware.FiberSwitch
{
    public class EOLFiberSwitchConfig : FiberSwitchConfig
    {
        public SerialCom SerialCommunication;
        public OpcCom OpcCommunication;
    }
}
