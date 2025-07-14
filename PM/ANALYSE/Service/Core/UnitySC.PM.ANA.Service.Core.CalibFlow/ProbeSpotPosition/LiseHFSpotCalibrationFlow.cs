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

    public class LiseHFSpotCalibrationFlow : FlowComponent<LiseHFSpotCalibrationInput, LiseHFSpotCalibrationResults, LiseHFSpotCalibrationConfiguration>
    {
        private readonly AnaHardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;

        public LiseHFSpotCalibrationFlow(LiseHFSpotCalibrationInput input) : base(input, "LiseHFSpotCalibrationFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            _referentialManager = ClassLocator.Default.GetInstance<IReferentialManager>();

            //Configuration.WriteReportMode = FlowReportConfiguration.AlwaysWrite;

            Result = new LiseHFSpotCalibrationResults();

        }

        private int GetMaximalGreyLevelAroundCenter(ServiceImage img, double percentageNearByCenter = 0.333)
        {
            if (img == null || img.Data == null)
                return 0;

            // we assume here taht images is a greyscale image with a 8 bits depth, if not return 0
            if (img.Depth != 8)
                return 0;

            double halfRoiSizeX = 0.5 * percentageNearByCenter * (double)img.DataWidth;
            double halfRoiSizeY = 0.5 * percentageNearByCenter * (double)img.DataHeight;
            double centerX = (double)img.DataWidth * 0.5;
            double centerY = (double)img.DataHeight * 0.5;
            int minRoiX = Math.Max((int)Math.Floor(centerX - halfRoiSizeX), 0);
            int maxRoiX = Math.Min((int)Math.Ceiling(centerX + halfRoiSizeX), img.DataWidth - 1);
            int minRoiY = Math.Max((int)Math.Floor(centerY - halfRoiSizeY), 0);
            int maxRoiY = Math.Min((int)Math.Ceiling(centerY + halfRoiSizeY), img.DataHeight - 1);

            int stride = img.DataWidth;
            int[] hist = new int[256];
            for (int j = minRoiY; j <= maxRoiY; j++)
            {
                for (int i = minRoiX; i <= maxRoiX; i++)
                {
                    hist[img.Data[i + j * stride]]++;
                }
            }

            for (int k = 255; k > 0; k--)
            {
                if (hist[k] != 0)
                    return k;
            }
            return 0;
        }

        protected override void Process()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var probeHW= HardwareUtils.GetProbeFromID<IProbe>(_hardwareManager, Input.ProbeId);
            if (probeHW == null)
            {
                throw new Exception($"Probe HW id=<{Input.ProbeId}> has not be found");
            }

            var probeLiseHF = probeHW as ProbeLiseHF;
            if (probeLiseHF == null)
            {
                throw new Exception($"HW Probe is not a Probe Lise HF");
            }

            if(probeLiseHF.Status == ProbeStatus.Uninitialized)
                throw new Exception($"Probe Lis HF is not Initialized");

            // stop any continous aquisition and close shutter
            probeLiseHF.StopContinuousAcquisition();
            probeLiseHF.CloseShutterAndWait(2000);

            CheckCancellation();

            var calibrationManager = ClassLocator.Default.GetInstance<CalibrationManager>();
            if (calibrationManager == null)
            {
                throw new Exception($"Caalibration manager is Null");
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

            DateTime processDate = DateTime.Now;
            try
            {

                var chuckConfig = _hardwareManager.Chuck.Configuration as ANAChuckConfig;
                var reflist = chuckConfig.ReferencesList;

                // retreive REFERENCE SILICON CALIBRATION
                var RefSi = reflist.Find(x => x.ReferenceName == "REF SI");
                HardwareUtils.MoveAxesToOpticalReference(_hardwareManager, RefSi, AxisSpeed.Fast);

                // move to Filter Slider to CustomDB (Slot index 3)  
                probeLiseHF.SetCustomDBSlider();

                // turn all the light off
                HardwareUtils.TurnOffAllLights(_hardwareManager, 2000);

                bool bWriteReport = Configuration.IsAnyReportEnabled();

                double exposureTime_Step_ms = Input.StepExposureTime_ms;
                double start_exposure_time_ms = Input.StartExposureTime_ms;
                double calibrationFrameRate = Configuration.CameraFrameRate;

                int targetGreyLevel = Configuration.TargetGreyLevel;
                int maxtargetGreyLevel = targetGreyLevel + Configuration.TargetGLTolerance;
                int mintargetGreyLevel = targetGreyLevel - Configuration.TargetGLTolerance;

                // We should diminish frame rate in order to have greater exposition time
                camera.SetFrameRate(calibrationFrameRate);

                int nbIterExpo = (int) Math.Floor(start_exposure_time_ms / exposureTime_Step_ms);
                int count = 0;
                foreach (var objectiveId in Input.ObjectiveIds)
                {

                    var liseHFSpotCalibration = new LiseHFObjectiveSpotCalibration(objectiveId, new FlowStatus(FlowState.InProgress, $"[{objectiveId}] In progress"));
                    Result.SpotCalibPositions.Add(liseHFSpotCalibration);

                    SetProgressMessage($"Start Calibration of {objectiveId} [{++count}/{Input.ObjectiveIds.Count}]", Result);

                    var objectiveCalibration = HardwareUtils.GetObjectiveParameters(calibrationManager, objectiveId);
                    if (objectiveCalibration == null || objectiveCalibration.Image == null || objectiveCalibration.Image.PixelSizeX == null || objectiveCalibration.Image.PixelSizeX == null)
                    {
                        liseHFSpotCalibration.Status= new FlowStatus(FlowState.Error, $"[{objectiveId}] Image PixelSize is not correctly set");
                        SetProgressMessage($"Calibration Done with Error for {objectiveId} - BadPixelSize", Result);
                        continue; // to the next opbejctive
                    }

                    double pixelsizeX = objectiveCalibration.Image.PixelSizeX.Micrometers;
                    double pixelsizeY = objectiveCalibration.Image.PixelSizeY.Micrometers;

                    //Selection de l'objectif 
                    var objSelecorId = _hardwareManager.GetObjectiveSelectorOfObjective(objectiveId);
                    HardwareUtils.SetNewObjective(objSelecorId.DeviceID, objectiveId, applyObjectiveOffset: true, Logger, _referentialManager, _hardwareManager);

                    // Wait Objective Selector to Stabilize - avoid shaking due to linmot
                    Task.Delay(2000, CancellationToken).GetAwaiter().GetResult();


                    if (!probeLiseHF.OpenShutterAndWait(2500))
                    {
                        liseHFSpotCalibration.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Shutter Open Timeout");
                        SetProgressMessage($"Calibration Done with Error for {objectiveId} - Shutter TimeOut", Result);
                        continue; // to the next opbejctive
                    }

                    int niter = 0;
                    double currentexpo_ms = start_exposure_time_ms;
                    bool greylevelreached = false;
                    bool darkImage = false;
                    ServiceImage img;
                    do
                    {
                        camera.SetExposureTimeMs(currentexpo_ms);
                        Thread.Sleep(500);

                        int WaitExpoIterTimeout_ms = 5000;
                        int waitDelayExpo = 500;
                        int nbitertimeout = WaitExpoIterTimeout_ms / waitDelayExpo;
                        int nbiterwaitexpo = 0;
                        Logger.Debug($"[{objectiveId}] Waiting cam Expo_ms={currentexpo_ms} to reached");
                        while ( !currentexpo_ms.Near(camera.GetExposureTimeMs(), 0.5) && nbiterwaitexpo < nbitertimeout)
                        {
                            CheckCancellation();
                            Thread.Sleep(waitDelayExpo);
                            ++nbiterwaitexpo;
                        }
                        Logger.Debug($"[{objectiveId}] cam Expo_ms={camera.GetExposureTimeMs()} reached");

                        Thread.Sleep(200);
                        img = HardwareUtils.AcquireCameraImage(_hardwareManager, cameraManager, cameraId);

#if DEBUG 
                        if (false)// only for development purpose - set true to see all iteration images in flows report folder
                        {
                            // only for development purpose
                            ImageReport.SaveImage(img, Path.Combine(ReportFolder, $"{objectiveId}_DBG_SpotImg_{currentexpo_ms:0.##}_ms.png"));
                        }
#endif

                        CheckCancellation();

                        int maxgreylevel = GetMaximalGreyLevelAroundCenter(img);
                        if (maxgreylevel == 0 || currentexpo_ms > start_exposure_time_ms)
                        {
                            Logger.Error($"[{objectiveId}] cannot found maxGL => GL={maxgreylevel}");
                            darkImage = true;
                            break;
                        }
                        else
                            Logger.Debug($"[{objectiveId}] Try Expo_ms={currentexpo_ms} => maxGL={maxgreylevel}");



                        if (mintargetGreyLevel <= maxgreylevel && maxgreylevel <= maxtargetGreyLevel)
                        {
                            Logger.Debug($"[{objectiveId}] Found sufficient Expo_ms={currentexpo_ms} maxGL={maxgreylevel}");
                            greylevelreached = true;
                        }
                        else
                        {
                            if (maxgreylevel > maxtargetGreyLevel)
                                currentexpo_ms -= exposureTime_Step_ms;
                            else
                                currentexpo_ms += exposureTime_Step_ms / 3.0;
                        }

                        niter++;
                    }
                    while (!greylevelreached && !darkImage && niter < nbIterExpo);


                    if (bWriteReport)
                    {
                        var imageFileName = $"{objectiveId}_SpotImg_{currentexpo_ms:0.##}_ms.png";
                        try
                        {
                            ImageReport.SaveImage(img, Path.Combine(ReportFolder, imageFileName));
                        }
                        catch (Exception)
                        {
                            Logger.Error($"[LiseHFSpotCalibration] Failed to save the image: {imageFileName}");
                        }
                    }

                    if (darkImage)
                    {
                        liseHFSpotCalibration.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] DarkImage");
                        probeLiseHF.CloseShutterAndWait(1000);
                        SetProgressMessage($"Calibration Done with Error for {objectiveId} - DarkImage", Result);
                        continue; // to the next objective
                    }

                    // call Beamprofiler catcher from liseHF dll
                    if (!probeLiseHF.FindSpotPosition(img, pixelsizeX, pixelsizeY, out double spotOffsetX, out double spotOffsetY, 
                                                        out double spotShape, out double spotDiameter_um, out double spotIntensity, out string errmessage))
                    {
                        Logger.Error($"[{objectiveId}] BeamProfiler ==> could not find spot position : {errmessage}");
                        liseHFSpotCalibration.Status= new FlowStatus(FlowState.Error, $"[{objectiveId}] could not find spot position {errmessage}");

                        SetProgressMessage($"Calibration Done with Error for {objectiveId} - Spot Not Detected - {errmessage}", Result);
                    }
                    else
                    {
                        Logger.Debug($"[{objectiveId}] BeamProfiler ==> Pixel({spotOffsetX / pixelsizeX},{spotOffsetY / pixelsizeY})\n     => Micrometers ({spotOffsetX},{spotOffsetY})");

                        liseHFSpotCalibration.Status = new FlowStatus(FlowState.Success, $"[{objectiveId}] Done");
                        liseHFSpotCalibration.Date = processDate;
                        liseHFSpotCalibration.PixelSizeX = pixelsizeX.Micrometers();
                        liseHFSpotCalibration.PixelSizeY = pixelsizeY.Micrometers();
                        liseHFSpotCalibration.XOffset = spotOffsetX.Micrometers();
                        liseHFSpotCalibration.YOffset = spotOffsetY.Micrometers();
                        liseHFSpotCalibration.CamExposureTime_ms = currentexpo_ms;
                        // advanced prm
                        liseHFSpotCalibration.SpotShape = spotShape;
                        liseHFSpotCalibration.SpotDiameter = spotDiameter_um.Micrometers();
                        liseHFSpotCalibration.SpotIntensity = (int) spotIntensity;

                        SetProgressMessage($"Calibration Done for {objectiveId}", Result);
                    }

                    // reset exposition time for next objective
                    camera.SetExposureTimeMs(start_exposure_time_ms);

                    probeLiseHF.CloseShutterAndWait(1000);

                    CheckCancellation();
                }
            }
            catch (TaskCanceledException)
            {
                FinalizeUnfinishedResults();
                SetProgressMessage($"Calibration Cancelleld", Result);
            }
            catch (Exception ex)
            {
                var spotobjcalibResult = Result.SpotCalibPositions.LastOrDefault();
                if(spotobjcalibResult != null)
                    spotobjcalibResult.Status = new FlowStatus(FlowState.Error, $"[{spotobjcalibResult.ObjectiveDeviceId}] Exception - {ex.Message}");

                FinalizeUnfinishedResults();
                SetProgressMessage($"Calibration failed : {ex.Message}", Result);
            }
            finally
            {
                probeLiseHF.CloseShutterAndWait(1000);

                camera.SetFrameRate(initial_framerate);
                camera.SetExposureTimeMs(initial_exposureTime_ms);
            }

            stopwatch.Stop();
            Logger.Information($"{LogHeader} Calibration done in {stopwatch.Elapsed}");
        }

        protected void FinalizeUnfinishedResults()
        {
            //Set status of "in progress" calibration to canceled
            foreach (var calib in Result.SpotCalibPositions.Where(s=>s.Status.State==FlowState.InProgress))
            {
                calib.Status = new FlowStatus(FlowState.Canceled, $"[{calib.ObjectiveDeviceId}] Cancelled");
            }
            //Add result canceled for all calib not treat
            foreach (var objectiveId in Input.ObjectiveIds.Except(Result.SpotCalibPositions.Select(x => x.ObjectiveDeviceId)))
            {
                Result.SpotCalibPositions.Add(new LiseHFObjectiveSpotCalibration(objectiveId,
                new FlowStatus(FlowState.Canceled, $"[{objectiveId}] Cancelled")));
            }
        }
    }
}
