using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Statuses
{
    public class CarrierIdStatus
    {
        public CarrierIdStatus(Constants.Port port, string carrierId)
        {
            Port      = port;
            CarrierId = carrierId;
        }

        public Constants.Port Port { get; }

        public string CarrierId { get; }
    }
}
