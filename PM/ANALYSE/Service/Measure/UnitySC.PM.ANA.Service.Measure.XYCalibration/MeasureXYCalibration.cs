using System;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.PatternRec;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.XYCalibration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.XYCalibration
{
    public class MeasureXYCalibration : MeasureBase<XYCalibrationSettings, MeasurePointResult>
    {
        public override MeasureType MeasureType => MeasureType.XYCalibration;

        public MeasureXYCalibration() : base(ClassLocator.Default.GetInstance<ILogger<MeasureXYCalibration>>())
        {
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(XYCalibrationSettings measureSettings)
        {
            var calibrationMeasureTools = new XYCalibrationMeasureTools();
            var objectivesUp = HardwareManager.GetObjectiveConfigsByPosition(ModulePositions.Up);
            calibrationMeasureTools.RecommendedObjectiveId = objectivesUp.Where(x => x.ObjType == Interface.ObjectiveConfig.ObjectiveType.NIR).OrderByDescending(x => x.Magnification).FirstOrDefault()?.DeviceID;
            calibrationMeasureTools.CompatibleObjectiveIds = objectivesUp.Where(x => x.ObjType == Interface.ObjectiveConfig.ObjectiveType.NIR).Select(x => x.DeviceID).ToList();
            return calibrationMeasureTools;
        }

        protected override MeasurePointDataResultBase Process(XYCalibrationSettings calibrationMeasureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            Logger.Information("Start XYCalibration");
            PatternRecResult xyResult = null;
            if (FlowsAreSimulated)
            {
                // Don't just use PatternRecFlowDummy as we want different results for each point
                int seed = HardwareUtils.GetAxesPosition(HardwareManager.Axes).GetHashCode();
                var rand = new Random(seed);
                Length shiftX = 0.Micrometers();
                Length shiftY = 0.Micrometers();
                if (!calibrationMeasureSettings.CalibFlag.HasFlag(CalibrationFlag.PreAlign)) // NOT prealign
                {
                    if (calibrationMeasureSettings.CalibFlag.HasFlag(CalibrationFlag.Test)) // test
                    {
                        // lower shift
                        shiftX = (rand.Next(-7, 7)).Micrometers();
                        shiftY = (rand.Next(-7, 7)).Micrometers();
                    }
                    else        // en calib
                    {
                        shiftX = (rand.Next(-30, 30)).Micrometers();
                        shiftY = (rand.Next(-35, 35)).Micrometers();
                    }
                }

                xyResult = new PatternRecResult(new FlowStatus(FlowState.Success), 0.95, shiftX, shiftY, null);
            }
            else
            {
                var patternRecInput = new PatternRecInput(calibrationMeasureSettings.PatternRecognitionData);
                var patternRecFlow = new PatternRecFlow(patternRecInput);
                patternRecFlow.CancellationToken = cancelToken;
                patternRecFlow.StatusChanged += PatternRecFlow_StatusChanged;
                xyResult = patternRecFlow.Execute();
            }

            var resData = new XYCalibrationPointData();
            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
            if (xyResult.Status.State == FlowState.Success)
            {
                resData.State = MeasureState.Success;
                resData.ShiftX = xyResult.ShiftX;
                resData.ShiftY = xyResult.ShiftY;
                resData.QualityScore = xyResult.Confidence;
                if (resData.QualityScore < 0.5)
                {
                    // not sufficent confidence
                    resData.State = MeasureState.Partial;
                    resData.Message = $"warning skip measure too Low result confidence";
                }

                if (!(xyResult.ControlImage is null))
                {
                    if (calibrationMeasureSettings.CalibFlag.HasFlag(CalibrationFlag.PreAlign))
                        resData.ResultImageFileName = SaveServiceImage(xyResult.ControlImage, $"XYCalibPreAlign.png", measureContext);
                    else
                        resData.ResultImageFileName = SaveServiceImage(xyResult.ControlImage, $"XYCalib.png", measureContext);
                }
                else
                {
                    if (!FlowsAreSimulated)
                        resData.State = MeasureState.NotMeasured;
                    resData.Message = xyResult.Status.Message;
                }
            }
            else
            {
                resData.State = MeasureState.Error;
                resData.Message = $"unexpected Flow status = {xyResult.Status.State}";
            }
            return resData;
        }

        private void PatternRecFlow_StatusChanged(FlowStatus status, PatternRecResult _)
        {
            NotifyMeasureProgressChanged(new MeasurePointProgress() { Message = $"[XYCalibration] ${status.State} - ${status.Message}" });
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(XYCalibrationSettings measureSettings, Exception ex)
        {
            return new XYCalibrationPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message
            };
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            return new UnitySC.Shared.Format.XYCalibration.XYCalibrationResult();
        }
    }
}
