using System;
using System.Windows;

using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Hardware.Camera;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Service.Core.Flows.AutoExposure
{
    public class AutoExposureFlow : FlowComponent<AutoExposureInput, AutoExposureResult, AutoExposureConfiguration>
    {
        private readonly IEmeraCamera _camera;
        private readonly double _imageResolutionScale;

        public AutoExposureFlow(AutoExposureInput input, IEmeraCamera camera) : base(input, "AutoExposureFlow")
        {
            var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
            if (hardwareManager.Cameras.IsNullOrEmpty())
            {
                throw new Exception("No camera found.");
            }

            _camera = camera;

            var flowsConfiguration = ClassLocator.Default.GetInstance<IFlowsConfiguration>() as FlowsConfiguration;
            if (flowsConfiguration != null) _imageResolutionScale = flowsConfiguration.ImageScale;
        }

        protected override void Process()
        {
            int targetBrightness = (int)(Configuration.TargetBrightness * 255);
            var tolerance = Configuration.ToleranceBrightness;

            double exposureTime = 100;

            var image = SingleAcquisition(exposureTime);
            int brightness = GetBrightness(image);

            for (int i = 0; i < Configuration.MaxIteration && !tolerance.IsInTolerance(brightness, targetBrightness); i++)
            {
                if (brightness > 5 && brightness < 250)
                    exposureTime *= ((double)targetBrightness / brightness);
                else if (brightness <= 5)
                    exposureTime *= 2;
                else
                    exposureTime /= 2;

                if (exposureTime > 1000)
                {
                    throw new Exception($"Predicted exposure time is too high, check you filter / light setup and retry");
                }

                image = SingleAcquisition(exposureTime);
                brightness = GetBrightness(image);
            }


            if (!tolerance.IsInTolerance(brightness, targetBrightness))
            {
                throw new Exception($"Failed to find an exposure time for a target brightness of {targetBrightness} with a tolerance of {tolerance.Value} ({tolerance.Unit}) in {Configuration.MaxIteration} tries, result is {brightness}.");
            }

            Result = new AutoExposureResult()
            {
                Brightness = (double)brightness / 255,
                ExposureTime = exposureTime,
            };
        }

        private int GetBrightness(ServiceImage image)
        {
            var imageIn8Bits = AlgorithmLibraryUtils.Convert16BitServiceImageTo8Bit(image);
            var imgData = AlgorithmLibraryUtils.CreateImageData(imageIn8Bits);
            int brightness = UnitySCSharedAlgosOpenCVWrapper.ImageOperators.GrayscaleMedianComputation(imgData);
            CheckCancellation();
            return brightness;
        }



        private ServiceImage SingleAcquisition(double exposureTimeMs)
        {
            _camera.SetCameraExposureTime(exposureTimeMs);
            var image = _camera.SingleScaledAcquisition(Int32Rect.Empty, _imageResolutionScale);
            CheckCancellation();
            return image;
        }
    }
}
