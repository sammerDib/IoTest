using UnitySC.EFEM.Controller.HostInterface;

namespace UnitySC.EquipmentController.Simulator.Driver.Statuses
{
    public class CarrierPresenceStatus
    {
        public CarrierPresenceStatus(Constants.Port port, bool presence, bool placement, bool handOffButtonPressed)
        {
            Port                 = port;
            Presence             = presence;
            Placement            = placement;
            HandOffButtonPressed = handOffButtonPressed;
        }

        public Constants.Port Port { get; }

        public bool Presence { get; }

        public bool Placement { get; }

        public bool HandOffButtonPressed { get; }
    }
}
