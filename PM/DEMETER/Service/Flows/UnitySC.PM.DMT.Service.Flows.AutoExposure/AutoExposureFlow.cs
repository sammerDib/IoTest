using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Media;

using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Hardware.Screen;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools.Collection;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.AutoExposure
{
    public class AutoExposureFlow : FlowComponent<AutoExposureInput, AutoExposureResult, AutoExposureConfiguration>
    {
        protected const int NbLoops = 10;
        private const int MaxSaturationThreshold = 240;
        private const int MinSaturationThreshold = 5;

        private const string ReportCSVFilename = "AE_iterationsReport.csv";

        protected CameraBase Camera;
        protected readonly IDMTInternalCameraMethods CameraManager;
        protected readonly DMTHardwareManager HardwareManager;

        private ScreenBase _screen;

        public const int MaximumNumberOfSteps = 2 + NbLoops;

        public AutoExposureFlow(AutoExposureInput input, DMTHardwareManager hardwareManager,
            IDMTInternalCameraMethods cameraService) : base(input, "AutoExposure")
        {
            HardwareManager = hardwareManager;
            CameraManager = cameraService;
        }

        protected override void Process()
        {
            Result.TotalSteps = NbLoops;
            Result.WaferSide = Input.Side;

            Camera = HardwareManager.CamerasBySide[Input.Side];
            _screen = HardwareManager.ScreensBySide[Input.Side];
            SetProgressMessage($"Starting {Input.Side}side Auto-Exposure flow for {Input.MeasureType} {Input.Side}");
            CheckCancellation();

            SetScreenForAutoExposure();
            CheckCancellation();

            AutoExposureLoop();
        }

        private void SetScreenForAutoExposure()
        {
            switch (Input.DisplayImageType)
            {
                case AcquisitionScreenDisplayImage.FringeImage:
                    int largestPeriod = Input.Fringe.Periods.Max();
                    if (largestPeriod > _screen.Height / 4)
                    {
                        SetProgressMessage($"Set {Input.Side} screen image to {Colors.White.ToString()} color");
                        _screen.ClearAsync(Colors.White).Wait();
                    }
                    else
                    {
                        SetProgressMessage($"Set {Input.Side} screen image to fringe image");
                        _screen.DisplayImage(Input.ScreenImage);
                    }

                    break;

                case AcquisitionScreenDisplayImage.HighAngleDarkFieldMask:
                    SetProgressMessage($"Set {Input.Side} screen image to high angle dark-field mask image");
                    _screen.DisplayImage(Input.ScreenImage);
                    break;

                case AcquisitionScreenDisplayImage.Color:
                    SetProgressMessage($"Set {Input.Side} screen image to {Input.Color.ToString()} color");
                    _screen.ClearAsync(Input.Color);
                    break;
            }
        }

        private void AutoExposureLoop()
        {
            var flowConfig = Configuration
                .DefaultAutoExposureSetting.Find(setting =>
                    setting.Measure == Input.MeasureType && setting.WaferSide == Input.Side);
            try
            {
                CheckCancellation();
                //---------------------------------------------------------
                // Prepare
                //---------------------------------------------------------
                var chrono = new Stopwatch();
                chrono.Start();

                Result.ExposureTimeMs =
                    Input.InitialAutoExposureTimeMs.GetValueOrDefault(flowConfig.InitialExposureTimeMs);
                double effectiveExposureTimeMs = double.NaN;

                CheckCancellation();

                // Mask
                //.......
                using (USPImageMil maskImage = CameraManager.CreateMask(Input.Side, Input.RoiForMask, Input.IgnorePerspectiveCalibration))
                {
                    if (Configuration.IsAnyReportEnabled() && !ReportFolder.IsNullOrEmpty())
                    {
                        maskImage.Save(Path.Combine(ReportFolder, "AE_mask.tif"));
                    }
                    //---------------------------------------------------------
                    // Loop
                    //---------------------------------------------------------
                    bool success = false;
                    bool lastIterationOutOfCameraLimits = false;
                    int i = 0;
                    while (i < NbLoops && !success)
                    {
                        CheckCancellation();

                        // Exposure time
                        //..............
                        Result.CurrentStep = i + 1;
                        SetProgressMessage($"Auto exposure iteration #{i}", Result);

                        effectiveExposureTimeMs = CameraManager.SetExposureTime(Camera, Result.ExposureTimeMs);
                        Logger.Information(
                            $"{LogHeader} calibrated={effectiveExposureTimeMs:0.000}ms effective={effectiveExposureTimeMs:0.000}ms");

                        // Acquisition
                        //............
                        var grabbedImage = CameraManager.GrabNextImage(Camera);
                        Result.ResultImage = grabbedImage.ToServiceImage();

                        // Saturation computation
                        //.......................
                        double sat = ComputeSaturation(grabbedImage, flowConfig.RatioSaturated, maskImage);
                        CheckCancellation();
                        Logger.Debug($"{LogHeader}saturation={sat}");

                        SaveAutoExposureIterationImageWithSaturationWhenReportEnabled(i, grabbedImage, sat);

                        success = Math.Abs(Input.TargetSaturation - sat) <=
                                    flowConfig.SaturationTolerance;
                        // Computing next exposure time
                        //.............................
                        if (!success)
                        {
                            AdjustExposureTimeForSaturation(sat, out bool outOfCameraLimits);
                            if (outOfCameraLimits && lastIterationOutOfCameraLimits)
                            {
                                break;
                            }

                            lastIterationOutOfCameraLimits = outOfCameraLimits;
                        }

                        i++;
                        CheckCancellation();
                    }

                    //---------------------------------------------------------
                    // Result
                    //---------------------------------------------------------
                    CheckCancellation();
                    if (!success)
                    {
                        if (lastIterationOutOfCameraLimits)
                        {
                            throw new Exception("Auto-exposure failed because exposure time is out of hardware bounds");
                        }

                        throw new Exception("Auto-exposure failed");
                    }

                    chrono.Stop();
                    Logger.Debug(
                        $"{LogHeader} AutoExposure complete in {chrono.ElapsedMilliseconds}ms, calibrated={effectiveExposureTimeMs}ms actual={effectiveExposureTimeMs}ms");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{LogHeader} Error during Auto exposure loop: {ex.Message}");
                throw;
            }
            finally
            {
                _screen.Clear();
            }
        }

        private void SaveAutoExposureIterationImageWithSaturationWhenReportEnabled(int iteration,
            USPImageMil iterationImage, double saturation)
        {
            if (Configuration.IsAnyReportEnabled() && !ReportFolder.IsNullOrEmpty())
            {
                if (iteration == 0)
                {
                    using (var writer = File.CreateText(Path.Combine(ReportFolder, ReportCSVFilename)))
                    {
                        var csvSb = new CSVStringBuilder();
                        csvSb.AppendLine("Iteration", "Exposure time (s)", "Saturation");
                        writer.Write(csvSb.ToString());
                    }
                }

                using (var writer = File.AppendText(Path.Combine(ReportFolder, ReportCSVFilename)))
                {
                    var csvSb = new CSVStringBuilder();
                    csvSb.AppendLine(iteration.ToString(), $"\"{Result.ExposureTimeMs}\"",
                        $"\"{saturation:0.000}\"");
                    writer.Write(csvSb.ToString());
                }

                iterationImage.Save(Path.Combine(ReportFolder,
                    $"AE_iteration_{iteration}.tif"));
            }
        }

        private void AdjustExposureTimeForSaturation(double saturation, out bool outOfCameraLimits)
        {
            outOfCameraLimits = false;
            if (saturation > MaxSaturationThreshold)
            {
                Logger.Warning($"{LogHeader} Completely white image. Saturation computation is not very reliable.");
                Result.ExposureTimeMs /= 2;
            }
            else
            {
                if (saturation < MinSaturationThreshold)
                {
                    Logger.Warning($"{LogHeader} Completely black image. Saturation computation is not very reliable");
                }

                Result.ExposureTimeMs = Result.ExposureTimeMs * Input.TargetSaturation / saturation;
            }

            if (Result.ExposureTimeMs > Camera.MaxExposureTimeMs)
            {
                outOfCameraLimits = true;
                Logger.Warning(
                    $"{LogHeader} Auto Exposure did not converge: Exposure time exceeds the camera's maximum exposure time ({Camera.MaxExposureTimeMs}ms)");
                Result.ExposureTimeMs = Camera.MaxExposureTimeMs;
            }

            if (Result.ExposureTimeMs < Camera.MinExposureTimeMs)
            {
                outOfCameraLimits = true;
                Logger.Warning(
                    $"{LogHeader} Automatic exposure did not converge: Exposure time is below camera's minimum exposure time ({Camera.MinExposureTimeMs}ms");
                Result.ExposureTimeMs = Camera.MinExposureTimeMs;
            }
        }

        private static double ComputeSaturation(USPImageMil grabbedImage, double autoExposureRatioSaturated,
            USPImageMil maskImage = null)
        {            
            var grabbedImageData = ImageUtils.CreateImageDataFromUSPImageMil(grabbedImage);
            ImageData maskImageData = null;
            if (!(maskImage is null))
            {
                maskImageData = ImageUtils.CreateImageDataFromUSPImageMil(maskImage);
            }

            // Saturation computation
            //........................
            return maskImageData is null
                ? ImageOperators.ComputeGreyLevelSaturation(grabbedImageData, (float)autoExposureRatioSaturated)
                : ImageOperators.ComputeGreyLevelSaturation(grabbedImageData, maskImageData, (float)autoExposureRatioSaturated);
        }
    }
}
