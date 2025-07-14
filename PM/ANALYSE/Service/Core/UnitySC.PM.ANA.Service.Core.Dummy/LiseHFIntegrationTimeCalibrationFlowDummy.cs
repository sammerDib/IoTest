using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Dummy
{
    public class LiseHFIntegrationTimeCalibrationFlowDummy : LiseHFIntegrationTimeCalibrationFlow
    {
        private AnaHardwareManager _hardwareManager;

        public LiseHFIntegrationTimeCalibrationFlowDummy(LiseHFIntegrationTimeCalibrationInput input) : base(input)
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
                    var liseHFSpectroCalibration = new LiseHFObjectiveIntegrationTimeCalibration(objectiveId, new FlowStatus(FlowState.InProgress, $"[{objectiveId}] In progress"));
                    Result.CalibIntegrationTimes.Add(liseHFSpectroCalibration);
                    SetProgressMessage($"Start Calibration of {objectiveId} [{++count}/{Input.ObjectiveIds.Count}]", Result);

                    int waitrandom = (int)(2000.0 * (1.0 + rnd.NextDouble()));
                    Task.Delay(waitrandom, CancellationToken).GetAwaiter().GetResult();
                    if (rnd.NextDouble() > 0.95) //random calibration failure
                    {
                        liseHFSpectroCalibration.Status = new FlowStatus(FlowState.Error, $"[{objectiveId}] Dummy Random calibration failure");
                        SetProgressMessage($"Calibration Done with Error for {objectiveId} - Random simulate", Result);
                    }
                    else
                    {
                        liseHFSpectroCalibration.Status = new FlowStatus(FlowState.Success, $"[{objectiveId}] Done");
                        liseHFSpectroCalibration.Date = processDate;

                        liseHFSpectroCalibration.StandardFilterBaseCount = rnd.Next(48500, 53500);
                        liseHFSpectroCalibration.StandardFilterIntegrationTime_ms = 5 * rnd.NextDouble();

                        liseHFSpectroCalibration.LowIllumFilterBaseCount = rnd.Next(48500, 53500);
                        liseHFSpectroCalibration.LowIllumFilterIntegrationTime_ms = 5 * rnd.NextDouble();
                        SetProgressMessage($"Calibration Done for {objectiveId}", Result);
                    }

                    Task.Delay(500,CancellationToken).GetAwaiter().GetResult();
                    CheckCancellation();
                }
            }
            catch (TaskCanceledException)
            {
                FinalizeUnfinishedResultsWithStatus(FlowState.Canceled,"Canceled");
                SetProgressMessage($"Calibration Cancelleld", Result);
            }
            catch (Exception ex)
            {
                FinalizeUnfinishedResultsWithStatus(FlowState.Error, $"{ex.Message}");
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
