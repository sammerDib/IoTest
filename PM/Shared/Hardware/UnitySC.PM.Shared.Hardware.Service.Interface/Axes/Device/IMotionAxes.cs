using System.Collections.Generic;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public interface IMotionAxes
    {
        void Init(List<Message> initErrors);

        void Move(params PMAxisMove[] moves);

        void RelativeMove(params PMAxisMove[] moves);

        void Home(AxisSpeed speed);

        void WaitMotionEnd(int timeout_ms, bool waitStabilization = true);

        void StopAllMotion();

        PositionBase GetPosition();
    }
}
