using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Statuses
{
    public class CarrierTypeStatus
    {
        public CarrierTypeStatus(Constants.Port port, uint carrierType)
        {
            Port      = port;
            CarrierType = carrierType;
        }

        public Constants.Port Port { get; }

        public uint CarrierType { get; }
    }
}
