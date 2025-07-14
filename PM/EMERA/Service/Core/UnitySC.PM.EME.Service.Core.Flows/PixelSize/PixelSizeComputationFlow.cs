using System;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Flows.PatternRec;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Service.Core.Flows.PixelSize
{
    public class PixelSizeComputationFlow : FlowComponent<PixelSizeComputationInput, PixelSizeComputationResult,
        PixelSizeComputationConfiguration>
    {
        private readonly AutoExposureFlow _autoExposureFlow;
        private readonly ICalibrationService _calibration;
        private readonly IEmeraCamera _camera;
        private readonly PhotoLumAxes _motionAxes;
        private readonly PatternRecFlow _patternRecFlow;
        private XYPosition _initialPosition;

        public PixelSizeComputationFlow(PixelSizeComputationInput input, IEmeraCamera camera,
            PatternRecFlow patternRecFlow = null, AutoExposureFlow autoExposureFlow = null) : base(input,
            "PixelSizeComputationFlow")
        {
            var flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>() as FlowsConfiguration;
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _camera = camera;
            _patternRecFlow = patternRecFlow ?? new PatternRecFlow(new PatternRecInput(), _camera);
            _autoExposureFlow = autoExposureFlow ?? new AutoExposureFlow(new AutoExposureInput(), _camera);
            _calibration = ClassLocator.Default.GetInstance<ICalibrationService>();
            if (flowsConfiguration != null) ImageScale = flowsConfiguration.ImageScale;

            if (hardwareManager.MotionAxes is PhotoLumAxes motionAxes)
            {
                _motionAxes = motionAxes;
            }
            else
            {
                throw new Exception("MotionAxes should be PhotoLumAxes");
            }

            if (hardwareManager.Cameras.IsNullOrEmpty())
            {
                throw new Exception("No camera found.");
            }
        }

        public double ImageScale { get; set; }

        protected override void Process()
        {
            _initialPosition = _motionAxes.GetPosition() as XYPosition;
            double initialExposureTime = _camera.GetCameraExposureTime();

            try
            {
                PreparePatternRec();
                CheckCancellation();
                MoveAndGetXShift();
                CheckCancellation();
                ComputePixelSize();
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during the Pixel Size Computation Flow : {ex.Message}");
                throw;
            }
            finally
            {
                _motionAxes.GoToPosition(_initialPosition);
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

        private void PreparePatternRec()
        {
            //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
            //double exposureTime = GetExposureTime();
            //_camera.SetCameraExposureTime(exposureTime);
            var serviceImage = _camera.SingleScaledAcquisition(Int32Rect.Empty, ImageScale);
            serviceImage = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(serviceImage);

            _patternRecFlow.Input = CreatePatternRecInput(serviceImage);
        }

        private void MoveAndGetXShift()
        {
            var shiftedPosition = new XYPosition(_initialPosition.Referential,
                _initialPosition.X + Configuration.ShiftLength.Millimeters,
                _initialPosition.Y);

            _motionAxes.GoToPosition(shiftedPosition);
            _motionAxes.WaitMotionEnd(3000);
            //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
            //double exposureTime = GetExposureTime();
            //_camera.SetCameraExposureTime(exposureTime);
            var result = _patternRecFlow.Execute();

            if (result.Status.State == FlowState.Error)
            {
                throw new Exception("Pattern rec for the pixel size computation failed");
            }
        }

        private void ComputePixelSize()
        {
            var oldPixelSize = _calibration.GetCameraCalibrationData().Result.PixelSize / ImageScale;
            double pixelSize = Math.Abs(Configuration.ShiftLength.Micrometers /
                                        _patternRecFlow.Result.ShiftX.ToPixels(oldPixelSize));
            pixelSize *= ImageScale;
            Result.PixelSize = pixelSize.Micrometers();
        }

        private PatternRecInput CreatePatternRecInput(ServiceImage refImage)
        {
            var patternRecData = new PatternRecognitionData
            {
                PatternReference = refImage.ToExternalImage(), Gamma = Configuration.PatternRecGamma
            };

            return new PatternRecInput(patternRecData);
        }
    }
}
