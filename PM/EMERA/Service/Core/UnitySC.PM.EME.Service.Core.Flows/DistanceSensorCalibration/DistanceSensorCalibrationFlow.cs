using System;
using System.IO;
using System.Linq;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Interface;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.AxesSpace;
using UnitySC.PM.Shared.Hardware.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.DistanceSensorCalibration
{
    public class DistanceSensorCalibrationFlow : FlowComponent<DistanceSensorCalibrationInput,
        DistanceSensorCalibrationResult, DistanceSensorCalibrationConfiguration>
    {
        private readonly IEmeraCamera _camera;
        private readonly PositionTracker _distancePositionTracker;
        private readonly EmeHardwareManager _hardwareManager;
        private readonly DistanceSensorBase _hardwareManagerDistanceSensor;
        private readonly PhotoLumAxes _motionAxes;
        private readonly PatternRecFlow _patternRecFlow;
        private readonly IReferentialManager _referentialManager;
        private readonly PositionTracker _xPositionTracker;
        private readonly PositionTracker _yPositionTracker;

        public DistanceSensorCalibrationFlow(DistanceSensorCalibrationInput input, IEmeraCamera camera,
            PatternRecFlow patternRecFlow = null, PositionTracker distancePositionTracker = null) : base(input, "DistanceSensorCalibrationFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            _camera = camera;
            _patternRecFlow = patternRecFlow ?? new PatternRecFlow(new PatternRecInput(), _camera);

            if (_hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }

            if (_hardwareManager.Cameras.IsNullOrEmpty())
            {
                throw new Exception("No camera found.");
            }

            _hardwareManagerDistanceSensor = _hardwareManager.DistanceSensor;
            _xPositionTracker =
                new PositionTracker(GetTimestampedXPosition, Configuration.TrackingPeriodInMilliseconds);
            _yPositionTracker =
                new PositionTracker(GetTimestampedYPosition, Configuration.TrackingPeriodInMilliseconds);
            _distancePositionTracker = distancePositionTracker ?? new PositionTracker(GetTimestampedDistance, Configuration.TrackingPeriodInMilliseconds);
        }

        protected override void Process()
        {
            var initialPosition = _motionAxes.GetPosition() as XYZPosition;

            try
            {
                _hardwareManager.EMELights["3"].SwitchOn(true);
                _hardwareManager.EMELights["3"].SetPower(100.0);
                GoToReferencePosition();
                ScanXY();
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during the Distance Sensor Calibration Flow : {ex.Message}");
                throw;
            }
            finally
            {
                _motionAxes.GoToPosition(initialPosition);
                _distancePositionTracker.StopTracking();
                _xPositionTracker.StopTracking();
                _yPositionTracker.StopTracking();

                _distancePositionTracker.Dispose();
                _xPositionTracker.Dispose();
                _yPositionTracker.Dispose();
            }
        }

        private void GoToReferencePosition()
        {
            _motionAxes.GoToPosition(Configuration.ReferencePosition, AxisSpeed.Fast);
            _motionAxes.WaitMotionEnd(30000);
            //Stop the streaming before the pattern rec to not capture any blurry images (Remove this when we figure out a way to
            //                                                                            stream while capturing images after a move
            //                                                                            without any blur)
            _camera.StopAcquisition();
            LoadReferenceImage();
            var result = _patternRecFlow.Execute();
            _camera.StartAcquisition();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pattern rec failed in DistanceSensorCalibrationFlow");
            }

            var currentPos = _motionAxes.GetPosition() as XYZPosition;
            var referencePosition = currentPos;
            referencePosition.X += result.ShiftX.Millimeters;
            referencePosition.Y += result.ShiftY.Millimeters;
            _motionAxes.GoToPosition(referencePosition, AxisSpeed.Fast);
            _motionAxes.WaitMotionEnd(3000);
        }
        
        private void ScanXY()
        {
            var xAxisConfig = _motionAxes.AxesConfiguration.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.X);
            var yAxisConfig = _motionAxes.AxesConfiguration.AxisConfigs.Find(a => a.MovingDirection == MovingDirection.Y);

            var startPosition =
                _referentialManager.ConvertTo(_motionAxes.GetPosition(), ReferentialTag.Wafer) as XYZPosition;

            var maxPositions = new XYZPosition(new MotorReferential(), xAxisConfig.PositionMax.Millimeters, yAxisConfig.PositionMax.Millimeters, 0.0);
            var maxPositionsWaferReferential = _referentialManager.ConvertTo(maxPositions, ReferentialTag.Wafer).ToXYPosition();

            double endX = Math.Min(startPosition.X - Configuration.MaximumSensorOffsetX.Millimeters, maxPositionsWaferReferential.X);
            double endY = startPosition.Y - Configuration.MaximumSensorOffsetY.Millimeters;
            double yStep = Configuration.ReferenceSmallestSideSize.Millimeters / 2;

            _distancePositionTracker.Reset();
            _xPositionTracker.Reset();
            _yPositionTracker.Reset();

            _distancePositionTracker.StartTracking();
            _xPositionTracker.StartTracking();
            _yPositionTracker.StartTracking();

            for (double y = startPosition.Y; y > endY; y -= yStep)
            {
                var positionToMoveTo = new XYPosition(new WaferReferential(), endX, y);
                _motionAxes.GoToPosition(positionToMoveTo, AxisSpeed.Normal);
                _motionAxes.WaitMotionEnd(3000);
                positionToMoveTo.X = startPosition.X;
                positionToMoveTo.Y = y - yStep;
                _motionAxes.GoToPosition(positionToMoveTo, AxisSpeed.Normal);
                _motionAxes.WaitMotionEnd(3000);
            }

            _distancePositionTracker.StopTracking();
            _xPositionTracker.StopTracking();
            _yPositionTracker.StopTracking();

            var distancesCloseToReference = _distancePositionTracker.TimeOrderedPositions.Where(x =>
                Configuration.DistanceTolerance.IsInTolerance(x.Value.Value,
                    Configuration.ApproximateReferenceDistance));

            if (!distancesCloseToReference.Any())
            {
                throw new Exception("Could not find the reference using the distance sensor");
            }

            long firstCloseDistanceTimestamp = distancesCloseToReference.First().Key;
            var referencePositionX = _xPositionTracker.GetPositionAtTime(firstCloseDistanceTimestamp);
            var referencePositionY = _yPositionTracker.GetPositionAtTime(firstCloseDistanceTimestamp);

            Result.OffsetX = (Math.Abs(startPosition.X) - Math.Abs(referencePositionX.Value)).Millimeters();
            Result.OffsetY = (Math.Abs(startPosition.Y) - Math.Abs(referencePositionY.Value)).Millimeters();
        }

        private TimestampedPosition GetTimestampedDistance()
        {
            double distance = _hardwareManagerDistanceSensor.GetDistanceSensorHeight();
            var posLength = new Length(distance, LengthUnit.Micrometer);
            return new TimestampedPosition(posLength, DateTime.UtcNow);
        }

        private TimestampedPosition GetTimestampedXPosition()
        {
            var pos = _motionAxes.GetPosition();
            var posWaferReferential = _referentialManager.ConvertTo(pos, ReferentialTag.Wafer).ToXYZPosition();
            var posLength = new Length(posWaferReferential.X, LengthUnit.Millimeter);
            return new TimestampedPosition(posLength, DateTime.UtcNow);
        }

        private TimestampedPosition GetTimestampedYPosition()
        {
            var pos = _motionAxes.GetPosition();
            var posWaferReferential = _referentialManager.ConvertTo(pos, ReferentialTag.Wafer).ToXYZPosition();
            var posLength = new Length(posWaferReferential.Y, LengthUnit.Millimeter);
            return new TimestampedPosition(posLength, DateTime.UtcNow);
        }

        private void LoadReferenceImage()
        {
            string pathImageRef =
                Path.Combine(ClassLocator.Default.GetInstance<IEMEServiceConfigurationManager>().CalibrationFolderPath,
                    Configuration.ReferenceImageName);
            try
            {
                if (!File.Exists(pathImageRef))
                {
                    throw new FileNotFoundException($"[DistanceSensorCalibrationFlow] File not found: {pathImageRef}");
                }

                var refImage = new ServiceImage();
                refImage.LoadFromFile(pathImageRef);
                _patternRecFlow.Input = new PatternRecInput(new PatternRecognitionData
                {
                    PatternReference = refImage.ToExternalImage(), Gamma = 0.3
                });
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception($"LoadFromFile failed in DistanceSensorCalibrationFlow: {ex.Message}");
            }
        }
    }
}
