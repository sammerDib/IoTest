using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UI.Main
{
    public enum ConnexionState
    {
        Opened,
        Faulted,
        Closed
    }
    public class ConnexionStateForActor
    {
        public ActorType Actor;
        public ConnexionState Status;
    }
}
