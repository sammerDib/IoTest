using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.UC
{
    public interface IPmUc
    {
        ActorType ActorType { get; }

        void Init(bool isStandalone);
    }
}
