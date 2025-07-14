using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.CalibFlow.ProbeSpotPosition;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class LiseHFSpotCalibrationFlowDummy : LiseHFSpotCalibrationFlow
    {
        private AnaHardwareManager _hardwareManager;

        public LiseHFSpotCalibrationFlowDummy(LiseHFSpotCalibrationInput input) : base(input)
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        protected override void Process()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var ObjectiveIds = Input.ObjectiveIds;
            Task.Delay(1000,CancellationToken).GetAwaiter().GetResult();
            CheckCancellation();

            var rnd = new Random();
            int count = 0;
            DateTime processDate = DateTime.Now;
            try
            {

                foreach (var objectiveId in Input.ObjectiveIds)
                {
                    var liseHFSpotCalibration = new LiseHFObjectiveSpotCalibration(objectiveId, new FlowStatus(FlowState.InProgress, $"[{objectiveId}] In progress"));
                    Result.SpotCalibPositions.Add(liseHFSpotCalibration);
                    SetProgressMessage($"Start Calibration of {objectiveId} [{++count}/{Input.ObjectiveIds.Count}]", Result);
                    int waitrandom = (int)(2000.0 * (1.0 + rnd.NextDouble()));
                    Task.Delay(waitrandom,CancellationToken).GetAwaiter().GetResult(); 
                    if (rnd.NextDouble() > 0.8) //random calibration failure
                    {
                        liseHFSpotCalibration.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Dummy Random calibration failure");
                        SetProgressMessage($"Calibration Done with Error for {objectiveId} - Random simulate", Result);
                    }
                    else
                    {
                        liseHFSpotCalibration.Status = new FlowStatus(FlowState.Success, $"[{objectiveId}] Done");
                        liseHFSpotCalibration.Date = processDate;
                        liseHFSpotCalibration.PixelSizeX = 1.Micrometers();
                        liseHFSpotCalibration.PixelSizeY = 1.Micrometers();
                        liseHFSpotCalibration.XOffset = (20.0 * rnd.NextDouble() - 10.0).Micrometers();
                        liseHFSpotCalibration.YOffset = (20.0 * rnd.NextDouble() - 10.0).Micrometers();
                        liseHFSpotCalibration.CamExposureTime_ms = (45 * rnd.NextDouble() + 5.0);

                        // advanced prm
                        liseHFSpotCalibration.SpotShape = 1.0 -  (0.1 * rnd.NextDouble());
                        liseHFSpotCalibration.SpotDiameter = (6.0 - (0.5 * rnd.NextDouble())).Micrometers();
                        liseHFSpotCalibration.SpotIntensity = (int)( 205.0 - (100.0 * rnd.NextDouble()));

                        SetProgressMessage($"Calibration Done for {objectiveId}", Result);
                    }

                    Task.Delay(500, CancellationToken).GetAwaiter().GetResult();
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
                if (spotobjcalibResult != null)
                    spotobjcalibResult.Status = new FlowStatus(FlowState.Error, $"[{spotobjcalibResult.ObjectiveDeviceId}] Exception - {ex.Message}");

                FinalizeUnfinishedResults();
                SetProgressMessage($"Calibration failed : {ex.Message}", Result);
            }
            finally
            {

            }
            stopwatch.Stop();
            Logger.Information($"{LogHeader} Calibration done in {stopwatch.Elapsed}");
        }
    }
}
