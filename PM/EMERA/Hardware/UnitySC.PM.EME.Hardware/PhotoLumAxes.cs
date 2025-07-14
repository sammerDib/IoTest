using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Hardware
{
    public class PhotoLumAxes : MotionAxes
    {
        private XYZPosition _position;

        public PhotoLumAxes(PhotoLumAxesConfig config, Dictionary<string, MotionControllerBase> controllers,
          IGlobalStatusServer globalStatusServer, ILogger logger, IReferentialManager referentialManager)
          : base(config, controllers, globalStatusServer, logger, referentialManager)
        {
            _position = new XYZPosition(new MotorReferential());
        }

        public override void NotifyPositionUpdated(PositionBase position)
        {
            var xyzPosition = (XYZPosition)GetPosition();
            switch (position)
            {
                case XYZPosition _:
                    UpdatePositions(ref _position); break;
                case XPosition xPosition:
                    if (_position.Referential != xPosition.Referential)
                    {
                        throw new Exception($"Cannot merge positions from referential {xPosition.Referential} to {_position.Referential}");
                    }
                    _position.X = double.IsNaN(xPosition.Position) ? _position.X : xPosition.Position;
                    _position.Y = xyzPosition.Y;
                    _position.Z = xyzPosition.Z;
                    PositionChangedNotify(_position);
                    break;

                case YPosition yPosition:
                    if (_position.Referential != yPosition.Referential)
                    {
                        throw new Exception($"Cannot merge positions from referential {yPosition.Referential} to {_position.Referential}");
                    }
                    _position.Y = double.IsNaN(yPosition.Position) ? _position.Y : yPosition.Position;
                    _position.X = xyzPosition.X;
                    _position.Z = xyzPosition.Z;
                    PositionChangedNotify(_position);
                    break;

                case ZPosition zPosition:
                    if (_position.Referential != zPosition.Referential)
                    {
                        throw new Exception($"Cannot merge positions from referential {zPosition.Referential} to {_position.Referential}");
                    }
                    _position.Z = double.IsNaN(zPosition.Position) ? _position.Z : zPosition.Position;
                    _position.X = xyzPosition.X;
                    _position.Y = xyzPosition.Y;
                    PositionChangedNotify(_position);
                    break;
            }
        }

        private void PositionChangedNotify(PositionBase position)
        {
            position = ReferentialManager.ConvertTo(position, ReferentialTag.Wafer);
            var motionAxesServiceCallback = ClassLocator.Default.GetInstance<IMotionAxesServiceCallbackProxy>();
            motionAxesServiceCallback.PositionChanged(position);
            Messenger.Send(new AxesPositionChangedMessage() { Position = position });
        }

        public override PositionBase GetPosition()
        {
            CheckAxesInitialization();
            foreach (var axesController in MotionControllers)
            {
                var axeslist = axesController.GetAxesControlledFromList(Axes);
                if (axeslist.Count > 0)
                    axesController.RefreshCurrentPos(axeslist);
            }
            var xyzPosition = new XYZPosition(new MotorReferential());
            UpdatePositions(ref xyzPosition);

            return xyzPosition;
        }

        private void UpdatePositions(ref XYZPosition xyzPosition)
        {
            foreach (var axis in Axes)
            {
                switch (axis.AxisID)
                {
                    case "X": xyzPosition.X = axis.CurrentPos.Millimeters; break;
                    case "Y": xyzPosition.Y = axis.CurrentPos.Millimeters; break;
                    case "Z": xyzPosition.Z = axis.CurrentPos.Millimeters; break;
                }
            }
        }

        public override void TriggerUpdateEvent()
        {
            foreach (var controller in MotionControllers)
                controller.TriggerUpdateEvent();
        }

        public override void GoToPosition(PositionBase position, AxisSpeed speed = AxisSpeed.Normal)
        {
            var targetPosition = GetTargetEmeraPosition(position);
            var positionInMotorReferential = ConvertTo(targetPosition, ReferentialTag.Motor);

            CheckAxesInitialization();

            var targetMoves = new List<PMAxisMove>();

            var xPosition = positionInMotorReferential is XYPosition && Axes.Exists(axis => axis.AxisID == "X")
                ? new XPosition(position.Referential, (positionInMotorReferential as XYPosition).X)
                : null;
            AddPositionToMovesIfValid(xPosition, ref targetMoves);

            var yPosition = positionInMotorReferential is XYPosition && Axes.Exists(axis => axis.AxisID == "Y")
                ? new YPosition(position.Referential, (positionInMotorReferential as XYPosition).Y)
                : null;
            AddPositionToMovesIfValid(yPosition, ref targetMoves);

            var zPosition = positionInMotorReferential is XYZPosition && Axes.Exists(axis => axis.AxisID == "Z")
                ? new ZPosition(position.Referential, (positionInMotorReferential as XYZPosition).Z)
                : null;
            AddPositionToMovesIfValid(zPosition, ref targetMoves);

            targetMoves.ForEach(move => move.Speed = GetSpeedFromAxisConfig(speed, GetAxisConfigById(move.AxisId)));

            Move(targetMoves.ToArray());
        }

        private XYZPosition GetTargetEmeraPosition(PositionBase targetPosition)
        {
            var currentPosition = ConvertTo(GetPosition(), targetPosition.Referential.Tag) as XYZPosition;
            switch (targetPosition)
            {
                case XYZPosition xyzPosition:
                    return xyzPosition;

                case XYPosition xyPosition:
                    return new XYZPosition(targetPosition.Referential, xyPosition.X, xyPosition.Y, currentPosition.Z);

                default:
                    throw new ArgumentException("Unhandled position of type '" + targetPosition.GetType().Name);
            }
        }
    }
}
