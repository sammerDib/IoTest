using System;
using System.Threading;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Algo.GetZFocus;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.AxisOrthogonality
{
    public class AxisOrthogonalityFlow : FlowComponent<AxisOrthogonalityInput, AxisOrthogonalityResult, AxisOrthogonalityConfiguration>
    {
        private readonly PhotoLumAxes _motionAxes;
        private readonly IEmeraCamera _camera;
        private readonly PatternRecFlow _patternRecFlow;
        private readonly AutoExposureFlow _autoExposureFlow;
        private readonly FlowsConfiguration _flowsConfiguration;

        public AxisOrthogonalityFlow(AxisOrthogonalityInput input, IEmeraCamera camera, PatternRecFlow patternRecFlow = null, AutoExposureFlow autoExposureFlow = null) : base(input, "AxisOrthogonalityFlow")
        {
            _flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>() as FlowsConfiguration;
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _camera = camera;
            _patternRecFlow = patternRecFlow ?? new PatternRecFlow(new PatternRecInput(), _camera);
            _autoExposureFlow = autoExposureFlow ?? new AutoExposureFlow(new AutoExposureInput(), _camera);

            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            
            else
            {
                throw new Exception($"MotionAxes should be PhotoLumAxes");
            }

            if (hardwareManager.Cameras.IsNullOrEmpty())
            {
                throw new Exception("No camera found.");
            }
        }
        protected override void Process()
        {
            var initialPosition = _motionAxes.GetPosition() as XYZPosition;
            double initialExposureTime = _camera.GetCameraExposureTime();

            try
            {                
                GoToCrossAndGetPattern();
                CheckAxesOrthogonality();
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during the Axis Orthogonality Flow : {ex.Message}");
                throw;
            }
            finally
            {              
                _motionAxes.GoToPosition(initialPosition);
                _camera.SetCameraExposureTime(initialExposureTime);
            }
        }

        private double GetExposureTime()
        {
            var result = _autoExposureFlow.Execute();
            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Exposure time Failed.");
            }
            CheckCancellation();
            return result.ExposureTime;
        }

        private void GoToCrossAndGetPattern()
        {
            _motionAxes.GoToPosition(Configuration.ReferenceCrossPosition);
            _motionAxes.WaitMotionEnd(30000);
            Thread.Sleep(3000);
            //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
            //double exposureTime = GetExposureTime();
            //_camera.SetCameraExposureTime(exposureTime);
            double imageResolutionScale = _flowsConfiguration.ImageScale;
            var serviceImage = _camera.SingleScaledAcquisition(Int32Rect.Empty, imageResolutionScale);
            serviceImage = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(serviceImage);

            _patternRecFlow.Input = CreatePatternRecInput(serviceImage, Input.GetZFocusInput);
        }

        private Angle GetXAngle()
        {
            var xAnglePosition = new XYZPosition(Configuration.ReferenceCrossPosition.Referential,
                                                         Configuration.ReferenceCrossPosition.X + Configuration.ShiftLength.Millimeters,
                                                         Configuration.ReferenceCrossPosition.Y,
                                                         Configuration.ReferenceCrossPosition.Z);
            _motionAxes.GoToPosition(xAnglePosition);
            _motionAxes.WaitMotionEnd(30000);
            Thread.Sleep(3000);
            //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
            //double exposureTime = GetExposureTime();
            //_camera.SetCameraExposureTime(exposureTime);
            var result = _patternRecFlow.Execute();

            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pattern rec for the X axis failed");
            }

            double angleInRads = Math.Atan2(-result.ShiftY.Millimeters, -result.ShiftX.Millimeters);
            double angleInDegrees = angleInRads * (180 / Math.PI);
            var angle = new Angle(angleInDegrees, AngleUnit.Degree);
            return angle;
        }
        private Angle GetYAngle()
        {
            var yAnglePosition = new XYZPosition(Configuration.ReferenceCrossPosition.Referential,
                                                         Configuration.ReferenceCrossPosition.X,
                                                         Configuration.ReferenceCrossPosition.Y + Configuration.ShiftLength.Millimeters,
                                                         Configuration.ReferenceCrossPosition.Z);
            _motionAxes.GoToPosition(yAnglePosition);
            _motionAxes.WaitMotionEnd(30000);
            Thread.Sleep(3000);
            //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
            //double exposureTime = GetExposureTime();
            //_camera.SetCameraExposureTime(exposureTime);
            var result = _patternRecFlow.Execute();

            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pattern rec for the Y axis failed");
            }

            double angleInRads = Math.Atan2(-result.ShiftY.Millimeters, -result.ShiftX.Millimeters);
            double angleInDegrees = angleInRads * (180 / Math.PI);
            angleInDegrees -= 90.0;
            var angle = new Angle(angleInDegrees, AngleUnit.Degree);
            return angle;
        }

        private void CheckAxesOrthogonality()
        {
            Result.XAngle = GetXAngle();
            Result.YAngle = GetYAngle();

            if (Result.XAngle.Abs() > Configuration.AngleThreshold)
            {
                throw new Exception("The computed angle for the X axis is outside the threshold");
            }
            if (Result.YAngle.Abs() > Configuration.AngleThreshold)
            {
                throw new Exception("The computed angle for the Y axis is outside the threshold");
            }
        }

        private PatternRecInput CreatePatternRecInput(ServiceImage refImage, GetZFocusInput getZFocusInput = null)
        {
            bool runAutoFocus = !(getZFocusInput is null);

            var patternRecData = new PatternRecognitionData
            {
                PatternReference = refImage.ToExternalImage(), Gamma = 0.3
            };

            return new PatternRecInput(patternRecData, runAutoFocus, getZFocusInput);
        }
    }
}
