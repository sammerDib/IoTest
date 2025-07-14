using System;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Flows.AutoExposure;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.EME.Service.Core.Flows.Distortion
{
    public class DistortionFlow : FlowComponent<DistortionInput, DistortionResult, DistortionConfiguration>
    {
        private readonly AutoExposureFlow _autoExposureFlow;
        private readonly IEmeraCamera _camera;
        private readonly PhotoLumAxes _motionAxes;

        public DistortionFlow(DistortionInput input, IEmeraCamera camera, AutoExposureFlow autoExposureFlow = null) :
            base(input, "DistortionFlow")
        {
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            _camera = camera;
            _autoExposureFlow = autoExposureFlow ?? new AutoExposureFlow(new AutoExposureInput(), _camera);

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

            _autoExposureFlow = autoExposureFlow;
        }

        protected override void Process()
        {
            var initialPosition = _motionAxes.GetPosition();
            double initialExposureTime = _camera.GetCameraExposureTime();

            try
            {
                GoToPatternPosition();

                ComputeDistortion();
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

        private void ComputeDistortion()
        {
            var serviceImage = _camera.SingleScaledAcquisition(Int32Rect.Empty, 0.2);
            var imageData = AlgorithmLibraryUtils.CreateImageData(serviceImage);

            var theoreticalPositions = new Point2f[Configuration.CalibrationPatternCircleNumber];

            int sqrtOfCircleNb = (int)Math.Sqrt(Configuration.CalibrationPatternCircleNumber);

            //Creating pseudo theoretical coordinates
            int index = 0;
            for (int i = 0; i < sqrtOfCircleNb; ++i)
            {
                for (int j = 0; j < sqrtOfCircleNb; ++j)
                {
                    theoreticalPositions[index++] = new Point2f(i, j);
                }
            }

            double gaussianSigma = Input.GaussianSigma;

            var distortionResult =
                DistortionCalibration.ComputeDistortion(imageData, theoreticalPositions, gaussianSigma);

            var distortionData = new DistortionData
            {
                NewOptimalCameraMat = distortionResult.NewOptimalCameraMat,
                CameraMat = distortionResult.CameraMat,
                DistortionMat = distortionResult.DistortionMat,
                RotationVec = distortionResult.RotationVec,
                TranslationVec = distortionResult.TranslationVec
            };

            Result.DistortionData = distortionData;
        }

        private void GoToPatternPosition()
        {
            //UNCOMMENT WHEN EXPOSURETIMEFLOW WORKS AGAIN
            //double exposureTime = GetExposureTime();
            //_camera.SetCameraExposureTime(exposureTime);
            _motionAxes.GoToPosition(Configuration.PatternPosition);
        }
    }
}
