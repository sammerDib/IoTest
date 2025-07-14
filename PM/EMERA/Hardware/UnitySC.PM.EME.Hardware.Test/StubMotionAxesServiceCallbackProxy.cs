using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.EME.Hardware.Test
{
    internal class StubMotionAxesServiceCallbackProxy : IMotionAxesServiceCallbackProxy
    {
        public void PositionChanged(PositionBase position)
        {
        }

        public void StateChanged(AxesState state)
        {
        }

        public void EndMove(bool targetReached)
        {
        }
    }
}
