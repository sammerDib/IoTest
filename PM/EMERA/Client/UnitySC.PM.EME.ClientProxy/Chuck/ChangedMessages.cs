using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.EME.Client.Proxy.Chuck
{
    public class WaferPresenceChangedMessage
    {
        public MaterialPresence WaferPresence;
    }

    public class ChuckLoadingPositionChangedMessage
    {
        public bool ChuckIsInLoadingPosition { get; set; }
    }
}
