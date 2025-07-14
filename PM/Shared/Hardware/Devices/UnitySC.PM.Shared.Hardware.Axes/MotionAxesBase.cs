using System;
using System.Collections.Generic;
using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public abstract class MotionAxesBase : DeviceBase, IMotionAxes
    {
        protected readonly IReferentialManager ReferentialManager;
        public AxesConfig AxesConfiguration { get; }

        public override DeviceFamily Family { get => DeviceFamily.Axes; }

        protected MotionAxesBase(AxesConfig config, IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager) :
            base(globalStatusServer, logger)
        {
            AxesConfiguration = config;
            ReferentialManager = referentialManager;
        }

        public AxisConfig GetAxisConfigById(string axisId)
        {
            return AxesConfiguration.AxisConfigs.Find(x => x.AxisID == axisId);
        }

        public abstract void Init(List<Message> initErrors);

        public abstract void Move(params PMAxisMove[] moves);

        public abstract void RelativeMove(params PMAxisMove[] moves);

        public abstract void Home(AxisSpeed speed);

        public abstract void WaitMotionEnd(int timeout_ms, bool waitStabilization = true);

        public abstract void StopAllMotion();

        public abstract bool IsAtPosition(PositionBase position);

        public abstract bool CheckIfPositionReached(PositionBase position);

        public abstract PositionBase GetPosition();

        public abstract AxesState GetCurrentState();

        public void GoToPosition(PositionBase position)
        {
            GoToPosition(position, AxisSpeed.Normal);
        }

        public virtual void GoToPosition(PositionBase position, AxisSpeed speed)
        {
            if (position is OneAxisPosition pos)
            {
                var targetMoves = new List<PMAxisMove>() { };
                AddPositionToMovesIfValid(pos, ref targetMoves);

                targetMoves.ForEach(move => move.Speed = GetSpeedFromAxisConfig(speed, GetAxisConfigById(move.AxisId)));
                Move(targetMoves.ToArray());
            }
            else
            {
                string errMsg = "GotoPosition - Received unsupported position which is not a valid known position type";
                throw new Exception(errMsg);
            }
        }

        protected Speed GetSpeedFromAxisConfig(AxisSpeed speed, AxisConfig config)
        {
            switch (speed)
            {
                case AxisSpeed.Slow:
                    return config.SpeedSlow.MillimetersPerSecond();
                case AxisSpeed.Normal:
                    return config.SpeedNormal.MillimetersPerSecond();
                case AxisSpeed.Fast:
                    return config.SpeedFast.MillimetersPerSecond();
                case AxisSpeed.Measure:
                    return config.SpeedMeasure.MillimetersPerSecond();
                default:
                    throw new ArgumentOutOfRangeException(nameof(speed), speed, null);
            }
        }


        protected void AddPositionToMovesIfValid(OneAxisPosition axisPosition, ref List<PMAxisMove> targetMoves)
        {
            if ((axisPosition != null) && !double.IsNaN(axisPosition.Position))
            {
                //TODO : PMAXISMOVES TAKES A LENGTH BUT THE POSITION IS A DOUBLE
                var move = new PMAxisMove(axisPosition.AxisID, axisPosition.Position.Millimeters());
                targetMoves.Add(move);
            }
        }

        public virtual void TriggerUpdateEvent()
        {
        }
        protected PositionBase ConvertTo(PositionBase positionToConvert, ReferentialTag referentialTo)
        {
            return ReferentialManager.ConvertTo(positionToConvert, referentialTo);
        }
    }
}
