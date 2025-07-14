using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Statuses
{
    public class WaferSizeStatus
    {
        public WaferSizeStatus(Constants.Port port, int waferSize)
        {
            Port      = port;
            WaferSize = waferSize;
        }

        public Constants.Port Port { get; }

        public int WaferSize { get; }
    }
}
