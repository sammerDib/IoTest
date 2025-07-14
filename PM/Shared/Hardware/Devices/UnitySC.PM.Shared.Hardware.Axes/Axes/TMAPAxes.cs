using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class TMAPAxes : AxesBase
    {
        private AnaPosition _currentPosition { get; } = new AnaPosition(new MotorReferential(), 0, 0, 0, 0, new List<ZPiezoPosition>());

        public TMAPAxes(
            TMAPAxesConfig config,
            Dictionary<string, ControllerBase> controllers,
            IGlobalStatusServer globalStatusServer,
            ILogger logger,
            IReferentialManager referentialManager
        ) : base(config,
            controllers,
            globalStatusServer,
            logger,
            referentialManager
        )
        {
        }

        public override void Init(List<Message> initErrors)
        {
            base.Init(initErrors);
            GotoPosition(_currentPosition, AxisSpeed.Normal);
            WaitMotionEnd(30_000);
        }

        public override PositionBase GetPos()
        {
            CheckAxesInitialization();

            foreach (var axesController in AxesControllers)
            {
                var axeslist = axesController.GetAxesControlledFromList(Axes);
                if (axeslist.Count > 0)
                {
                    axesController.RefreshCurrentPos(axeslist);
                }
            }

            var anaPosition = new AnaPosition(new MotorReferential());
            UpdateXYZTopPositions(ref anaPosition);
            return anaPosition;
        }

        // TODO: Refactor this (make it more readable, remove magic numbers, add missing information in exception...)
        private List<IAxis> XYZTopZBottomAxes
        {
            get
            {
                var xyZTopZBottomIDs = new List<string>() { "X", "Y", "ZTop", "ZBottom" };

                var xyZTopZBottomAxes = Axes.Where(axe => xyZTopZBottomIDs.Contains(axe.AxisID)).ToList();
                if (xyZTopZBottomAxes.Count != xyZTopZBottomIDs.Count)
                {
                    throw new Exception($"Inconsistent number of axes: expected={xyZTopZBottomIDs.Count}, actual={xyZTopZBottomAxes.Count}");
                }

                return xyZTopZBottomAxes;
            }
        }

        // TODO: Refactor this (replace output parameter by returned value, magic values by consts, remove duplication...)
        private void UpdateXYZTopPositions(ref AnaPosition anaPosition)
        {
            foreach (var axis in XYZTopZBottomAxes)
            {
                switch (axis.AxisID)
                {
                    case "X": anaPosition.X = axis.CurrentPos.Millimeters; break;
                    case "Y": anaPosition.Y = axis.CurrentPos.Millimeters; break;
                    case "ZTop": anaPosition.ZTop = axis.CurrentPos.Millimeters; break;
                    case "ZBottom": anaPosition.ZBottom = axis.CurrentPos.Millimeters; break;
                    default: throw new Exception($"Unknown axis with id {axis.AxisID}");
                }
            }
        }

        public override void GotoPointCustomSpeedAccel(
            AxisMove moveX,
            AxisMove moveY,
            AxisMove moveZTop,
            AxisMove moveZBottom
        )
        {
            var axes = new List<IAxis>();
            var positions = new List<double>();
            var speeds = new List<double>();
            var accelerations = new List<double>();

            foreach (var motion in new Dictionary<string, AxisMove>
            {
                { "X", moveX }, { "Y", moveY }, { "ZTop", moveZTop }, { "ZBottom", moveZBottom }
            })
            {
                string axisId = motion.Key;
                var move = motion.Value;

                if (move == null || double.IsNaN(move.Position))
                {
                    continue;
                }

                axes.Add(GetAxis(axisId));
                positions.Add(move.Position);
                speeds.Add(move.Speed);
                accelerations.Add(move.Accel);
            }

            SetPosAxisWithSpeedAndAccelToAllControllers(axes, positions, speeds, accelerations);
        }

        public override void NotifyPositionUpdated(PositionBase position)
        {
            switch (position)
            {
                case XYPosition xyPosition:
                    _currentPosition.Merge(xyPosition);
                    break;

                case ZTopPosition zTopPosition:
                    _currentPosition.Merge(zTopPosition);
                    break;

                case ZBottomPosition zBottomPosition:
                    _currentPosition.Merge(zBottomPosition);
                    break;

                case ZPiezoPosition zPiezoPosition:
                    _currentPosition.AddOrUpdateZPiezoPosition(zPiezoPosition);
                    break;
            }

            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            var stagePosition = ReferentialManager.ConvertTo(_currentPosition, ReferentialTag.Stage);
            axesServiceCallback.PositionChanged(stagePosition);
            Messenger.Send<AxesPositionChangedMessage>(new AxesPositionChangedMessage() { Position = stagePosition });
        }

        // TODO: factorize with NSTAxes.GotoPosition()
        public override void GotoPosition(PositionBase position, AxisSpeed speed)
        {
            XYZTopZBottomPosition xyzPosition;
            switch (position)
            {
                case XYZTopZBottomPosition xyzTopZBottomPosition:
                    xyzPosition = xyzTopZBottomPosition;
                    break;

                case XYPosition xyPosition:
                    var currentPosition = ConvertTo(GetPos(), position.Referential.Tag) as XYZTopZBottomPosition;
                    xyzPosition = new XYZTopZBottomPosition(
                        position.Referential,
                        xyPosition.X,
                        xyPosition.Y,
                        currentPosition.ZTop,
                        currentPosition.ZBottom
                    );
                    break;

                case ZTopPosition zTopPosition1:
                    var currentPosition2 = ConvertTo(GetPos(), position.Referential.Tag) as XYZTopZBottomPosition;
                    xyzPosition = new XYZTopZBottomPosition(
                        position.Referential,
                        currentPosition2.X,
                        currentPosition2.Y,
                        zTopPosition1.Position,
                        currentPosition2.ZBottom
                    );
                    break;

                default:
                    throw new ArgumentException("Unhandled position of type '" +
                                                position.GetType().Name +
                                                "'. Only XYPosition and XYZTopZBottomPosition are supported"
                    );
            }

            var positionInMotorReferential = ConvertTo(xyzPosition, ReferentialTag.Motor);

            XPosition xPosition;
            YPosition yPosition;
            ZTopPosition zTopPosition = null;
            ZBottomPosition zBottomPosition = null;
            var zPiezoPositions = new List<OneAxisPosition>();

            switch (positionInMotorReferential)
            {
                case AnaPosition anaPosition:
                    xPosition = new XPosition(anaPosition.Referential, anaPosition.X);
                    yPosition = new YPosition(anaPosition.Referential, anaPosition.Y);
                    zTopPosition = new ZTopPosition(anaPosition.Referential, anaPosition.ZTop);
                    zBottomPosition = new ZBottomPosition(anaPosition.Referential, anaPosition.ZBottom);
                    zPiezoPositions = new List<OneAxisPosition>(anaPosition.ZPiezoPositions);
                    break;

                case XYZTopZBottomPosition xyZTopZBottomPosition:
                    xPosition = new XPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.X);
                    yPosition = new YPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.Y);
                    zTopPosition = new ZTopPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.ZTop);
                    zBottomPosition =
                        new ZBottomPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.ZBottom);
                    break;

                case XYPosition xyPosition:
                    xPosition = new XPosition(xyPosition.Referential, xyPosition.X);
                    yPosition = new YPosition(xyPosition.Referential, xyPosition.Y);
                    break;

                default:
                    base.GotoPosition(position, speed);
                    return;
            }

            CheckAxesInitialization();

            var targetAxisList = new List<IAxis>() { };
            var targetAxisPositions = new List<double>() { };
            var targetAxisSpeeds = new List<AxisSpeed>() { };

            AddPositionToListsIfValid(xPosition,
                speed,
                ref targetAxisList,
                ref targetAxisPositions,
                ref targetAxisSpeeds
            );
            AddPositionToListsIfValid(yPosition,
                speed,
                ref targetAxisList,
                ref targetAxisPositions,
                ref targetAxisSpeeds
            );
            AddPositionToListsIfValid(zTopPosition,
                speed,
                ref targetAxisList,
                ref targetAxisPositions,
                ref targetAxisSpeeds
            );
            AddPositionToListsIfValid(zBottomPosition,
                speed,
                ref targetAxisList,
                ref targetAxisPositions,
                ref targetAxisSpeeds
            );
            foreach (var zPiezoPosition in zPiezoPositions)
            {
                AddPositionToListsIfValid(zPiezoPosition,
                    speed,
                    ref targetAxisList,
                    ref targetAxisPositions,
                    ref targetAxisSpeeds
                );
            }

            if (targetAxisPositions.Count <= 0)
                throw (new Exception("Function GotoPosition called without coordinates"));

            SetPosAxisToAllControllers(targetAxisList, targetAxisPositions, targetAxisSpeeds);
        }

        public override void MoveIncremental(IncrementalMoveBase movement, AxisSpeed speed)
        {
            if (movement is XYZTopZBottomMove move)
            {
                double xStep = move.X;
                double yStep = move.Y;
                double zTopStep = move.ZTop;
                double zBottomStep = move.ZBottom;
                Logger.Information(FormatMessage($"MoveIncremental Speed: {speed} xStepMillimeters: {xStep}  yStepMillimeters: {yStep} zTopStepMillimeters: {zTopStep} zBottomStepMillimeters: {zBottomStep}"));

                CheckAxesInitialization();
                var axesList_XYZTopZBottom = XYZTopZBottomAxes;

                foreach (var axesController in AxesControllers)
                {
                    var axesList = axesController.GetAxesControlledFromList(axesList_XYZTopZBottom);
                    foreach (var axis in axesList)
                    {
                        switch (axis.AxisID)
                        {
                            case "X": if (!double.IsNaN(xStep)) axesController.MoveIncremental(axis, speed, xStep); break;
                            case "Y": if (!double.IsNaN(yStep)) axesController.MoveIncremental(axis, speed, yStep); break;
                            case "ZTop": if (!double.IsNaN(zTopStep)) axesController.MoveIncremental(axis, speed, zTopStep); break;
                            case "ZBottom": if (!double.IsNaN(zBottomStep)) axesController.MoveIncremental(axis, speed, zBottomStep); break;
                        }
                    }
                    axesList_XYZTopZBottom.RemoveAll(a1 => axesList.Exists(a2 => a2.AxisID == a1.AxisID));
                }
                if (axesList_XYZTopZBottom.Count != 0)
                {
                    string axisListText = GetAxesListText(axesList_XYZTopZBottom);
                    string errorMsg = FormatMessage("Function MoveIncremental called - No controller found to move this axes list [" + axisListText + "]");
                    throw new Exception(errorMsg);
                }
            }
            else
            {
                base.MoveIncremental(movement, speed);
            }
        }

        public override TimestampedPosition GetAxisPosWithTimestamp(IAxis axis)
        {
            throw new NotImplementedException();
        }
    }
}
