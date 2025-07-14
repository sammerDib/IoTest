using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{
    public interface IChuckLoadingPosition
    {
        bool IsInLoadingPosition();

        void SetChuckInLoadingPosition(bool loadingPosition);
    }
}
