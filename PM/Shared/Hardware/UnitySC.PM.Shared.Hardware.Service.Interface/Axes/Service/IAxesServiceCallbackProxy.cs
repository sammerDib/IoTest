using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public interface IAxesServiceCallbackProxy
    {
        void PositionChanged(PositionBase position);

        void StateChanged(AxesState state);

        void EndMove(bool targetReached);
    }
}
