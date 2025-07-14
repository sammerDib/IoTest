using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Service.Interface.Axes.Enum;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    public interface IAxes : IMotionAxes, IDevice
    {
        AxesConfig AxesConfiguration { get; set; }

        List<IAxis> Axes { get; }

        List<IAxesController> AxesControllers { get; set; }

        new void Init(List<Message> initErrors);

        /// <summary>
        /// Return current raw position of axes,
        /// ie a position in Referential.Raw.
        /// </summary>
        /// <returns></returns>
        PositionBase GetPos();

        TimestampedPosition GetAxisPosWithTimestamp(IAxis axis);

        AxesState GetState();

        void MoveIncremental(IncrementalMoveBase move, AxisSpeed speed);

        void LinearMotion(PositionBase position, AxisSpeed speed);      

        void GotoHomePos(AxisSpeed speed);

        /// <summary>
        /// Move according given motions.
        /// Each motion can be skipped by providing null or double.NAN Position.
        /// </summary>
        void GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom);

        void GotoPosition(PositionBase position, AxisSpeed speed);

        void StopAllMoves();

        new void WaitMotionEnd(int timeout, bool waitStabilization = true);

        void StopLanding();

        void Land();

        bool IsAtPosition(PositionBase position);

        void ResetAxis();

        void AcknowledgeResetAxis();

        void InitZTopFocus();

        void InitZBottomFocus();
    }
}
