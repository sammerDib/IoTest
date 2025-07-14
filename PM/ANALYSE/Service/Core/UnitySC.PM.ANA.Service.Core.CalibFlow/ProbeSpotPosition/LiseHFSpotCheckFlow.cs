using System;
using System.Linq;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.PM.ANA.Hardware.Probe.LiseHF;
using System.Threading.Tasks;
using System.Threading;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.Shared.Image;
using System.Diagnostics;
using System.IO;

namespace UnitySC.PM.ANA.Service.Core.CalibFlow.ProbeSpotPosition
{
    public class LiseHFSpotCheckFlow : FlowComponent<LiseHFSpotCheckInput, LiseHFSpotCheckResult, LiseHFSpotCheckConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;

        public LiseHFSpotCheckFlow(LiseHFSpotCheckInput input) : base(input, "LiseHFSpotCheckFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            Result = new LiseHFSpotCheckResult();

        }

        protected override void Process()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var probeHW = HardwareUtils.GetProbeFromID<IProbe>(_hardwareManager, Input.ProbeId);
            if (probeHW == null)
            {
                throw new Exception($"Probe HW id=<{Input.ProbeId}> has not be found");
            }

            var probeLiseHF = probeHW as ProbeLiseHF;
            if (probeLiseHF == null)
            {
                throw new Exception($"HW Probe is not a Probe Lise HF");
            }

            if (probeLiseHF.Status == ProbeStatus.Uninitialized)
                throw new Exception($"Probe Lis HF is not Initialized");

            CheckCancellation();

            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            if (calibrationManager == null)
            {
                throw new Exception($"Calibration manager is Null");
            }

            var cameraManager = ClassLocator.Default.GetInstance<ICameraManager>();
            if (cameraManager == null)
            {
                throw new Exception($"Camera manager is Null");
            }


            var camera = _hardwareManager.GetMainCamera();
            var cameraId = camera.DeviceID;
            var initial_exposureTime_ms = camera.GetExposureTimeMs();
            var initial_framerate = camera.GetFrameRate();
            var initial_sliderid = probeLiseHF.GetSliderIndex();

            int WaitExpoIterTimeout_ms = 5000;
            int waitDelayExpo_ms = 500;
            int nbitertimeout = WaitExpoIterTimeout_ms / waitDelayExpo_ms;
            int nbiterwaitexpo = 0;

            DateTime processDate = DateTime.Now;
            try
            {
                //  do we have to force flow to be executed at  REFERENCE SILICON CALIBRATION ?
                var chuckConfig = _hardwareManager.Chuck.Configuration as ANAChuckConfig;
                var reflist = chuckConfig.ReferencesList;

                // retreive REFERENCE SILICON CALIBRATION
                var RefSi = reflist.Find(x => x.ReferenceName == "REF SI");
                HardwareUtils.MoveAxesToOpticalReference(_hardwareManager, RefSi);

                var objectiveId = Input.ObjectiveId;
                Result.SpotPosition = new LiseHFObjectiveSpotCalibration(objectiveId, new FlowStatus(FlowState.InProgress, $"[{objectiveId}] In progress"));

                var objectiveCalibration = HardwareUtils.GetObjectiveParameters(calibrationManager, objectiveId);
                if (objectiveCalibration == null || objectiveCalibration.Image == null || objectiveCalibration.Image.PixelSizeX == null || objectiveCalibration.Image.PixelSizeX == null)
                {
                    Result.SpotPosition.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Image PixelSize is not correctly set");
                    throw new Exception($"Error for {objectiveId} - BadPixelSize");
                }

                // We should diminish frame rate in order to have greater exposition time
                camera.SetFrameRate(Configuration.CameraFrameRate);
                // turn all the light off
                HardwareUtils.TurnOffAllLights(_hardwareManager, 2000);
                // set Exposure time
                camera.SetExposureTimeMs(Input.ExposureTime_ms);
                // move to Filter Slider to CustomDB (Slot index 3)  
                probeLiseHF.SetCustomDBSlider();

                double pixelsizeX = objectiveCalibration.Image.PixelSizeX.Micrometers;
                double pixelsizeY = objectiveCalibration.Image.PixelSizeY.Micrometers;

                //Selection de l'objectif - normally it should not change here
                var objSelecorId = _hardwareManager.GetObjectiveSelectorOfObjective(objectiveId);
                HardwareUtils.SetNewObjective(objSelecorId.DeviceID, objectiveId, applyObjectiveOffset: true, Logger, _referentialManager, _hardwareManager);

                if (!probeLiseHF.OpenShutterAndWait(2500))
                {
                    Result.SpotPosition.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Shutter Open Timeout");
                    throw new Exception($"Error for {objectiveId} - Shutter Open TimeOut");
                }

                // wait for exposure time to be reached
                double currentexpo_ms = Input.ExposureTime_ms;
                Logger.Debug($"[{objectiveId}] Waiting cam Expo_ms={currentexpo_ms} to reached");
                while (!currentexpo_ms.Near(camera.GetExposureTimeMs(), 0.5) && nbiterwaitexpo < nbitertimeout)
                {
                    CheckCancellation();
                    Thread.Sleep(waitDelayExpo_ms);
                    ++nbiterwaitexpo;
                }
                Logger.Debug($"[{objectiveId}] cam Expo_ms={camera.GetExposureTimeMs()} reached");

                ServiceImage img;
                img = HardwareUtils.AcquireCameraImage(_hardwareManager, cameraManager, cameraId);

                if (Configuration.IsAnyReportEnabled())
                {
                    var imageFileName = $"{objectiveId}_SpotImg_{currentexpo_ms:0.##}_ms.png";
                    try
                    {
                        ImageReport.SaveImage(img, Path.Combine(ReportFolder, imageFileName));
                    }
                    catch (Exception)
                    {
                        Logger.Error($"[LiseHFSpotCheck] Failed to save the image: {imageFileName}");
                    }
                }

                // call Beamprofiler catcher from liseHF dll
                double spotOffsetX = 0.0;
                double spotOffsetY = 0.0;
                if (!probeLiseHF.FindSpotPosition(img, pixelsizeX, pixelsizeY, out spotOffsetX, out spotOffsetY,
                                  out double spotShape, out double spotDiameter_um, out double spotIntensity, out string errmessage))
                {
                    Logger.Error($"[{objectiveId}] BeamProfiler ==> could not find spot position : {errmessage}");
                    Result.SpotPosition.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] could not find spot position {errmessage}");
                    SetProgressMessage($"Check LiseHF Spot Done with Error for {objectiveId} - Spot Not Detected - {errmessage}", Result);
                }
                else
                {
                    Logger.Debug($"[{objectiveId}] BeamProfiler ==> Pixel({spotOffsetX / pixelsizeX},{spotOffsetY / pixelsizeY})\n     => Micrometers ({spotOffsetX},{spotOffsetY})");

                    Result.SpotPosition.Status = new FlowStatus(FlowState.Success, $"[{objectiveId}] Done");
                    Result.SpotPosition.Date = processDate;
                    Result.SpotPosition.PixelSizeX = pixelsizeX.Micrometers();
                    Result.SpotPosition.PixelSizeY = pixelsizeY.Micrometers();
                    Result.SpotPosition.XOffset = spotOffsetX.Micrometers();
                    Result.SpotPosition.YOffset = spotOffsetY.Micrometers();
                    Result.SpotPosition.CamExposureTime_ms = currentexpo_ms;
                    // advanced prm
                    Result.SpotPosition.SpotShape = spotShape;
                    Result.SpotPosition.SpotDiameter = spotDiameter_um.Micrometers();
                    Result.SpotPosition.SpotIntensity = (int)spotIntensity;

                    SetProgressMessage($"Check LiseHF Spot for {objectiveId}", Result);
                }
            }
            catch (TaskCanceledException)
            {
                SetProgressMessage($"Check LiseHF Spot Cancelleld", Result);
            }
            catch (Exception ex)
            {
                SetProgressMessage($"Check LiseHF Spot  failed : {ex.Message}", Result);
            }
            finally
            {
                camera.SetExposureTimeMs(initial_exposureTime_ms); // should be before SetFrameRate
                probeLiseHF.CloseShutterAndWait(1000);
                camera.SetFrameRate(initial_framerate);// should be after SetExposureTimeMs
                probeLiseHF.SetSliderIndex(initial_sliderid);
            }

            stopwatch.Stop();
            Logger.Information($"{LogHeader} done in {stopwatch.Elapsed}");
        }
    }
}
