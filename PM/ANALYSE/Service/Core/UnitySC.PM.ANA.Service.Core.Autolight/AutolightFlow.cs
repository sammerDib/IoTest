using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Autolight
{
    /// <summary>
    /// Compute the optimal intensity of a given light for a camera.
    /// This optimum is based on the saturation of the captured image.
    /// </summary>
    public class AutolightFlow : FlowComponent<AutolightInput, AutolightResult, AutolightConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly ICameraManager _cameraManager;

        private readonly ImageOperators _imageOperatorsLib;

        public AutolightFlow(AutolightInput input, ImageOperators imageOperatorsLib = null) : base(input, "AutolightFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();

            _imageOperatorsLib = imageOperatorsLib != null ? imageOperatorsLib : new ImageOperators();
        }

        protected override void Process()
        {
            CheckConsistency();
            if (!HardwareUtils.TurnOffAllLights(_hardwareManager))
            {
                Logger.Error($"{LogHeader} TurnOffAllLights failed");
                throw new Exception($"Auto light failed because TurnOfAllLights failed.");
            }
            double quality = 0;
            double maxValue = 0;
            double optimalLightPower = 0;
            List<double> contrastValues = new List<double>();
            List<double> lightPowerValues = new List<double>();
            var imageAtOptimalLightPower = new ServiceImage();

            for (double lightPower = Input.LightPower.Min; lightPower <= Input.LightPower.Max; lightPower = Math.Round(lightPower + Input.LightPower.Step, 1))
            {
                CheckCancellation();

                if (!HardwareUtils.SetLightIntensity(_hardwareManager, Input.LightId, lightPower, 10000))
                {
                    Logger.Error($"{LogHeader} SetLightIntensity failed : {Input.LightId}={lightPower:0.000} (timeout)");
                    throw new Exception($"Auto light failed because SetLightIntensity failed.");
                }
                var img = HardwareUtils.AcquireCameraImage(_hardwareManager, _cameraManager, Input.CameraId);
                double contrastValue = _imageOperatorsLib.ComputeContrastMeasure(img);
                double saturationValue = _imageOperatorsLib.ComputeSaturationMeasure(img);

                lightPowerValues.Add(lightPower);
                contrastValues.Add(contrastValue);

                Logger.Debug($"{LogHeader} Light : {lightPower:0.000}; Contrast : {contrastValue:0.000000000}; Sat : {saturationValue:0.00000}");

                if (saturationValue > Configuration.SaturationMax)
                {
                    break;
                }

                if (contrastValue > maxValue)
                {
                    maxValue = contrastValue;
                    optimalLightPower = lightPower;
                    imageAtOptimalLightPower = img;
                    quality = ComputeQuality(saturationValue);
                }

                if (maxValue - contrastValue >= 0.3)
                {
                    Logger.Debug($"{LogHeader} Early return : optimal light power has been exceeded.");
                    break;
                }
            }

            if (Configuration.IsAnyReportEnabled())
            {
                SignalReport.WriteSignalInCSVFormat("Light power", "Contrast", lightPowerValues, contrastValues, Path.Combine(ReportFolder, $"contrast_function_of_light_power_csharp.csv"));
                ImageReport.SaveImage(imageAtOptimalLightPower, Path.Combine(ReportFolder, $"image_at_optimal_light_power_{optimalLightPower}_csharp.png"));
            }

            if (quality == 0)
            {
                throw new Exception($"Auto light failed because the quality is not good enough : {quality}.");
            }

            Logger.Information($"{LogHeader} Autolight was successful at light power : {optimalLightPower} & exposure time {Input.ExposureTimeMs}.");

            Result.LightPower = optimalLightPower;
            Result.QualityScore = quality;

            HardwareUtils.SetLightIntensity(_hardwareManager, Input.LightId, optimalLightPower, 100);
        }

        private void CheckConsistency()
        {
            CameraBase camera;
            if (!_hardwareManager.Cameras.TryGetValue(Input.CameraId, out camera))
            {
                throw new Exception($"Provided camera ID ({Input.CameraId}) cannot be found in hardware manager.");
            }
        }

        private void SetAllLightsIntensityToZero()
        {
            var currentLights = _hardwareManager.Lights;
            foreach (var light in currentLights)
            {
                currentLights[light.Key].SetIntensity(0);
            }
        }

        private double ComputeQuality(double saturationValue)
        {
            return 1 - (Math.Abs(saturationValue - 0.5) * 2);
        }
    }
}
