using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    public interface IPSDChamber : ISlitDoor, ICdaPneumaticValve, IChamberFFUControl, IChamberInterlocks
    {
    }
}
