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
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.AxesSpace
{
    public class NSTAxes : AxesBase
    {
        public NSTAxes(NSTAxesConfig config, Dictionary<string, ControllerBase> controllersDico,
            IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
            : base(config, controllersDico, globalStatusServer, logger, referentialManager)
        {
        }

        public override void GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            CheckAxesInitialization();

            var targetAxisList = new List<IAxis>();
            var targetAxisCoords = new List<double>();
            var targetAxisSpeeds = new List<double>();
            var targetAxisAccels = new List<double>();

            AddMoveToListsIfValid(moveX, "X", ref targetAxisList, ref targetAxisCoords, ref targetAxisSpeeds, ref targetAxisAccels);
            AddMoveToListsIfValid(moveY, "Y", ref targetAxisList, ref targetAxisCoords, ref targetAxisSpeeds, ref targetAxisAccels);
            AddMoveToListsIfValid(moveZTop, "ZTop", ref targetAxisList, ref targetAxisCoords, ref targetAxisSpeeds, ref targetAxisAccels);
            AddMoveToListsIfValid(moveZBottom, "ZBottom", ref targetAxisList, ref targetAxisCoords, ref targetAxisSpeeds, ref targetAxisAccels);

            if (targetAxisCoords.Count <= 0)
                throw (new Exception("Function GotoPointCustomSpeedAccel called without coordinates"));

            SetPosAxisWithSpeedAndAccelToAllControllers(targetAxisList, targetAxisCoords, targetAxisSpeeds, targetAxisAccels);
        }

        public override TimestampedPosition GetAxisPosWithTimestamp(IAxis axis)
        {
            CheckAxesInitialization();

            TimestampedPosition posAndTime = null;
            var axesController = AxesControllers.FirstOrDefault(c => c.DeviceID == axis.AxisConfiguration.ControllerID);
            posAndTime = axesController.GetCurrentAxisPosWithTimestamp(axis);

            return posAndTime;
        }

        public override PositionBase GetPos()
        {
            CheckAxesInitialization();

            foreach (var axesController in AxesControllers)
            {
                var axeslist = axesController.GetAxesControlledFromList(Axes);
                if (axeslist.Count > 0)
                    axesController.RefreshCurrentPos(axeslist);
            }

            var anaPosition = new AnaPosition(new MotorReferential());
            UpdateXYZTopZBottomPositions(ref anaPosition);
            UpdateZPiezoPositions(ref anaPosition);

            return anaPosition;
        }

        private void UpdateXYZTopZBottomPositions(ref AnaPosition anaPosition)
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

        private void UpdateZPiezoPositions(ref AnaPosition anaPosition)
        {
            foreach (var piezoAxis in PiezoAxes)
            {
                string axisID = piezoAxis.AxisID;
                var position = piezoAxis.CurrentPos ?? new Length(0, LengthUnit.Micrometer);

                var piezoPosition = new ZPiezoPosition(new MotorReferential(), axisID, position);
                anaPosition.AddOrUpdateZPiezoPosition(piezoPosition);
            }
        }

        private List<IAxis> XYZTopZBottomAxes
        {
            get
            {
                var xyZTopZBottomIDs = new List<string>() { "X", "Y", "ZTop", "ZBottom" };

                var xyZTopZBottomAxes = Axes.Where(axe => xyZTopZBottomIDs.Contains(axe.AxisID)).ToList();
                if (xyZTopZBottomAxes.Count != xyZTopZBottomIDs.Count) throw new Exception($"Inconsistent number of axes");

                return xyZTopZBottomAxes;
            }
        }

        private List<IAxis> PiezoAxes
        {
            get => AxesControllers
                .Where(controller => controller is PiezoController)
                .SelectMany(piezoController => piezoController.AxesList)
                .ToList();
        }

        public void GotoPointZTop(double zTopCoord, AxisSpeed speed)
        {
            Logger.Information(FormatMessage($"GotoPointZTop Speed: {speed}  zTopCoord: {zTopCoord}"));
            var zTopPos = new ZTopPosition(new MotorReferential(), zTopCoord);
            GotoPosition(zTopPos, speed);
        }

        public void GotoPointZBottom(double zBottomCoord, AxisSpeed speed)
        {
            Logger.Information(FormatMessage($"GotoPointZBottom Speed: {speed}  zBottomCoord: {zBottomCoord}"));
            var zBottomPos = new ZTopPosition(new MotorReferential(), zBottomCoord);
            GotoPosition(zBottomPos, speed);
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
                            default: break;
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

        public override void NotifyPositionUpdated(PositionBase position)
        {
            var updatedPosition = new AnaPosition(new MotorReferential());
            switch (position)
            {
                case AnaPosition anaPosition:
                    updatedPosition = anaPosition;
                    break;

                case XYZTopZBottomPosition xyZTopZBottomPosition:
                    updatedPosition.Merge(xyZTopZBottomPosition);
                    UpdateZPiezoPositions(ref updatedPosition);
                    break;

                case ZPiezoPosition _:
                    UpdateXYZTopZBottomPositions(ref updatedPosition);
                    UpdateZPiezoPositions(ref updatedPosition);
                    break;
            }

            var axesServiceCallback = ClassLocator.Default.GetInstance<IAxesServiceCallbackProxy>();
            axesServiceCallback.PositionChanged(updatedPosition);
            Messenger.Send<AxesPositionChangedMessage>(new AxesPositionChangedMessage() { Position = updatedPosition });
        }

        public override void LinearMotion(PositionBase position, AxisSpeed speed)
        {
            if (position is XYZTopZBottomPosition pos)
            {
                double xPos = pos.X;
                double yPos = pos.Y;
                double zTopPos = pos.ZTop;
                double zBottomPos = pos.ZBottom;
                Logger.Information(FormatMessage($"LinearMotion Speed: {speed} x: {xPos}  y: {yPos} zTop: {zTopPos} zBottom: {zBottomPos}"));

                CheckAxesInitialization();
                var axesList_XYZTopZBottom = XYZTopZBottomAxes;

                foreach (var axesController in AxesControllers)
                {
                    var axesList = axesController.GetAxesControlledFromList(axesList_XYZTopZBottom);
                    if (axesList.Count > 1)
                    {
                        var validatedList = new List<IAxis>();
                        var coordsList = new List<double>();
                        foreach (var axis in axesList)
                        {
                            switch (axis.AxisID)
                            {
                                case "X": if (!double.IsNaN(xPos)) { validatedList.Add(axis); coordsList.Add(xPos); } break;
                                case "Y": if (!double.IsNaN(yPos)) { validatedList.Add(axis); coordsList.Add(yPos); } break;
                                case "ZTop": if (!double.IsNaN(zTopPos)) { validatedList.Add(axis); coordsList.Add(zTopPos); } break;
                                case "ZBottom": if (!double.IsNaN(zBottomPos)) { validatedList.Add(axis); coordsList.Add(zBottomPos); } break;
                                default: break;
                            }
                        }
                        axesController.LinearMotionMultipleAxis(validatedList, speed, coordsList);
                    }
                    else
                    {
                        var axis = axesList[0];
                        switch (axis.AxisID)
                        {
                            case "X": if (!double.IsNaN(xPos)) axesController.LinearMotionSingleAxis(axis, speed, xPos); break;
                            case "Y": if (!double.IsNaN(yPos)) axesController.LinearMotionSingleAxis(axis, speed, yPos); break;
                            case "ZTop": if (!double.IsNaN(zTopPos)) axesController.LinearMotionSingleAxis(axis, speed, zTopPos); break;
                            case "ZBottom": if (!double.IsNaN(zBottomPos)) axesController.LinearMotionSingleAxis(axis, speed, zBottomPos); break;
                            default: break;
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
                base.LinearMotion(position, speed);
            }
        }

        public void GotoToPointXY(double xCoord, double yCoord, AxisSpeed speed)
        {
            Logger.Information(FormatMessage($"GotoToPointXY Speed: {speed}  xCoord: {xCoord}  yCoord: {yCoord}"));

            var xyPosition = new XYPosition(new MotorReferential(), xCoord, yCoord);
            GotoPosition(xyPosition, speed);
        }

        public override void GotoPosition(PositionBase position, AxisSpeed speed)
        {
            // Convert position to xyzPosition because it's the only thing handled by ReferentialManager.ConvertTo
            XYZTopZBottomPosition xyzPosition;
            switch (position)
            {
                case XYZTopZBottomPosition xyzTopZBottomPosition:
                    xyzPosition = xyzTopZBottomPosition;
                    break;

                case XYPosition xyPosition:
                    {
                        var currentPosition = ConvertTo(GetPos(), position.Referential.Tag) as XYZTopZBottomPosition;
                        xyzPosition = new XYZTopZBottomPosition(
                            position.Referential,
                            xyPosition.X,
                            xyPosition.Y,
                            currentPosition.ZTop,
                            currentPosition.ZBottom
                        );
                        break;
                    }
                case ZPiezoPosition _:
                    {
                        var currentPosition = ConvertTo(GetPos(), position.Referential.Tag) as XYZTopZBottomPosition;
                        xyzPosition = new XYZTopZBottomPosition(
                            position.Referential,
                           currentPosition.X,
                            currentPosition.Y,
                            currentPosition.ZTop,
                            currentPosition.ZBottom
                            );
                        break;
                    }
                default:
                    throw new ArgumentException("Unhandled position of type '" + position.GetType().Name +
                                                "'. Only XYPosition and XYZTopZBottomPosition are supported");
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
                    zBottomPosition = new ZBottomPosition(xyZTopZBottomPosition.Referential, xyZTopZBottomPosition.ZBottom);
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

            AddPositionToListsIfValid(xPosition, speed, ref targetAxisList, ref targetAxisPositions, ref targetAxisSpeeds);
            AddPositionToListsIfValid(yPosition, speed, ref targetAxisList, ref targetAxisPositions, ref targetAxisSpeeds);
            AddPositionToListsIfValid(zTopPosition, speed, ref targetAxisList, ref targetAxisPositions, ref targetAxisSpeeds);
            AddPositionToListsIfValid(zBottomPosition, speed, ref targetAxisList, ref targetAxisPositions, ref targetAxisSpeeds);
            foreach (var zPiezoPosition in zPiezoPositions)
            {
                AddPositionToListsIfValid(zPiezoPosition, speed, ref targetAxisList, ref targetAxisPositions, ref targetAxisSpeeds);
            }

            if (targetAxisPositions.Count <= 0)
                throw (new Exception("Function GotoPosition called without coordinates"));

            SetPosAxisToAllControllers(targetAxisList, targetAxisPositions, targetAxisSpeeds);
        }
    }
}
