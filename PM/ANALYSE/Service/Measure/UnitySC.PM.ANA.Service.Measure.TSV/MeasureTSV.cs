using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.TSV;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.TSV
{
    public class MeasureTSV : MeasureBase<TSVSettings, UnitySC.Shared.Format.Metro.TSV.TSVPointResult>, IMeasureDCProvider
    {
        public override MeasureType MeasureType => MeasureType.TSV;

        public MeasureTSV() : base(ClassLocator.Default.GetInstance<ILogger<MeasureTSV>>())
        {
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(TSVSettings measureSettings)
        {
            var tsvMeasureTools = new TSVMeasureTools();
            tsvMeasureTools.Probes = new List<ProbeWithObjectivesMaterial>();

            // TODO We must return the real compatible probes and objectives depending on the TSV Width and TSV Depth
            var objectivesUp = HardwareManager.GetObjectiveConfigsByPosition(ModulePositions.Up);
            var objectivesUpNirId = objectivesUp.Where(x => x.ObjType == ObjectiveConfig.ObjectiveType.VIS).Select(x => x.DeviceID).ToList();

            foreach (var topProbe in HardwareManager.GetProbesConfigsByPosition(ModulePositions.Up))
            {
                if (!(topProbe is ProbeDualLiseConfig))
                {
                    var probe = new ProbeWithObjectivesMaterial
                    {
                        CompatibleObjectives = new List<string>(),
                        ProbeId = topProbe.DeviceID
                    };

                    if (topProbe is ProbeLiseHFConfig)
                    {
                        var objectiveList = objectivesUp.Where(x => x.ObjType == ObjectiveConfig.ObjectiveType.VIS || x.ObjType == ObjectiveConfig.ObjectiveType.NIR).Select(x => x.DeviceID).ToList();
                        probe.CompatibleObjectives = objectiveList;
                    }
                    else
                    {
                        var objectiveList = objectivesUp.Where(x => x.ObjType == ObjectiveConfig.ObjectiveType.NIR).Select(x => x.DeviceID).ToList();
                        probe.CompatibleObjectives = objectiveList;
                    }
                    tsvMeasureTools.Probes.Add(probe);
                }
            }

            return tsvMeasureTools;
        }

        protected override MeasurePointDataResultBase Process(TSVSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            Logger.Information("Start TSV");

            var input = new TSVInput(
                measureSettings.CameraId,
                measureSettings.Probe,
                measureSettings.Shape,
                measureSettings.DepthTarget,
                measureSettings.LengthTarget,
                measureSettings.WidthTarget,
                measureSettings.DepthTolerance.GetAbsoluteTolerance(measureSettings.DepthTarget),
                measureSettings.LengthTolerance.GetAbsoluteTolerance(measureSettings.LengthTarget),
                measureSettings.WidthTolerance.GetAbsoluteTolerance(measureSettings.WidthTarget),
                measureSettings.Strategy,
                measureSettings.Precision,
                measureSettings.ROI,
                measureSettings.PhysicalLayers,
                measureSettings.ShapeDetectionMode)
            {
                InitialContext = measureSettings.MeasureContext
            };

            TSVFlow tsvFlow = FlowsAreSimulated ? new TSVFlowDummy(input) : new TSVFlow(input);
            tsvFlow.CancellationToken = cancelToken;
            tsvFlow.StatusChanged += TsvFlow_StatusChanged;
            var tsvResult = tsvFlow.Execute();
            tsvFlow.StatusChanged -= TsvFlow_StatusChanged;

            var resData = new UnitySC.Shared.Format.Metro.TSV.TSVPointData();
            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
            if ((tsvResult.Status.State == FlowState.Success) || (tsvResult.Status.State == FlowState.Partial))
            {
                resData.Message = tsvResult.Status.Message;

                // We apply the correction to the results
                tsvResult.Length = (tsvResult.Length is null) ? tsvResult.Length : measureSettings.LengthCorrection.ApplyCorrection(tsvResult.Length);
                tsvResult.Width = (tsvResult.Width is null) ? tsvResult.Width : measureSettings.WidthCorrection.ApplyCorrection(tsvResult.Width);
                tsvResult.Depth = (tsvResult.Depth is null) ? tsvResult.Depth : measureSettings.DepthCorrection.ApplyCorrection(tsvResult.Depth);

                resData.DepthState = measureSettings.GetDepthMeasureState(tsvResult.Depth);
                resData.LengthState = measureSettings.GetLengthMeasureState(tsvResult.Length);
                resData.WidthState = measureSettings.GetWidthMeasureState(tsvResult.Width);
                resData.State = MeasureStateComputer.CombineInternalMeasurementsStates(resData.LengthState, resData.WidthState, resData.DepthState);

                resData.Length = tsvResult.Length;
                resData.Width = tsvResult.Width;
                resData.Depth = tsvResult.Depth;
                resData.QualityScore = tsvResult.QualityScore;
                if (tsvResult.ResultImage != null)
                    resData.ResultImageFileName = SaveServiceImage(tsvResult.ResultImage, "TSV.png", measureContext);
                if(tsvResult.DepthRawSignal != null)
                    resData.DepthRawSignal = tsvResult.DepthRawSignal;
            }
            else
            {
                resData.State = MeasureState.NotMeasured;
                resData.Message = tsvResult.Status.Message;
                if (tsvResult.ResultImage != null)
                    resData.ResultImageFileName = SaveServiceImage(tsvResult.ResultImage, "TSV.png", measureContext);
                if (tsvResult.DepthRawSignal != null)
                    resData.DepthRawSignal = tsvResult.DepthRawSignal;
            }
            return resData;
        }

        private void TsvFlow_StatusChanged(FlowStatus status, TSVResult _)
        {
            NotifyMeasureProgressChanged(new MeasurePointProgress() { Message = $"[TSV] ${status.State} - ${status.Message}" });
        }
        public override bool PrepareExecution(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default)
        {

            if (IsCalibrationNeeded(measureSettings))
            {
                var settings = measureSettings as TSVSettings;
                if (settings.Probe is LiseHFSettings liseHFSettings)
                {
                    var inputParams = LiseHFInputParamsFactory.FromLiseHFSettings(liseHFSettings);

                    var calibrationManager = ClassLocator.Default.GetInstance<AnaHardwareManager>().Probes[liseHFSettings.ProbeId].CalibrationManager;
                    if (calibrationManager.GetCalibration(true, liseHFSettings.ProbeId, inputParams, null) == null)
                    {
                        Logger.Error($"Failed to get calibration for probe {liseHFSettings.ProbeId} - [{inputParams.ObjectiveId}] {(inputParams.IsLowIlluminationPower ? "LowIllum" : string.Empty)}");
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsCalibrationNeeded(MeasureSettingsBase measureSettingsBase)
        {
            if (measureSettingsBase is TSVSettings tsvSettings)
            {
                if (tsvSettings.Probe is LiseHFSettings)
                {
                    return true;
                }
            }
            return false;
        }

        public override void MeasureTerminatedInRecipe(MeasureSettingsBase measureSettingsBase)
        {
            if (measureSettingsBase is TSVSettings tsvSettings && tsvSettings.Probe is LiseHFSettings probeSettings)
            {
                var probe = HardwareManager.Probes[probeSettings.ProbeId];
                probe.CalibrationManager?.MeasureExecutionTerminated();
            }
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(TSVSettings measureSettings, Exception ex)
        {
            return new UnitySC.Shared.Format.Metro.TSV.TSVPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message
            };
        }

        public override MeasureDieResult CreateMetroDieResult()
        {
            return new UnitySC.Shared.Format.Metro.TSV.TSVDieResult();
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var tsvMeasureSettings = measureSettings as TSVSettings;
            var metroResult = new UnitySC.Shared.Format.Metro.TSV.TSVResult();
            metroResult.Settings.WidthTarget = tsvMeasureSettings.WidthTarget;
            metroResult.Settings.WidthTolerance = tsvMeasureSettings.WidthTolerance;
            metroResult.Settings.DepthTarget = tsvMeasureSettings.DepthTarget;
            metroResult.Settings.DepthTolerance = tsvMeasureSettings.DepthTolerance;
            metroResult.Settings.LengthTarget = tsvMeasureSettings.LengthTarget;
            metroResult.Settings.LengthTolerance = tsvMeasureSettings.LengthTolerance;
            metroResult.Settings.Shape = tsvMeasureSettings.Shape;
            return metroResult;
        }

        public static void ConvertSetting(MeasureSettingsBase measureSettings, Version currentVersion, Version targetVersion)
        {
            if (!(measureSettings is TSVSettings))
                return;

            if (currentVersion < new Version("1.0.3") && measureSettings is TSVSettings tsvSettingsToUpdate)
            {
                if (tsvSettingsToUpdate.Probe is LiseHFSettings probeSettingFrom)
                {
                    var calibManager = ClassLocator.Default.GetInstance<CalibrationManager>();
                    var integrationTimeCalibration = calibManager.GetLiseHFObjectiveIntegrationTime(probeSettingFrom.ProbeObjectiveContext.ObjectiveId);

                    var integrationTimeFromITTCalib = probeSettingFrom.IsLowIlluminationPower
                        ? integrationTimeCalibration.LowIllumFilterIntegrationTime_ms
                        : integrationTimeCalibration.StandardFilterIntegrationTime_ms;

                    probeSettingFrom.IntensityFactor = probeSettingFrom.IntegrationTimems / integrationTimeFromITTCalib;
                    probeSettingFrom.IntegrationTimems = double.NaN;
                }
            }
        }

        #region IMeasureDCProvider

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            var dcPointsMeasureData = new List<DCPointMeasureData>();

            var tsvPointResult = measurePointResult as UnitySC.Shared.Format.Metro.TSV.TSVPointResult;
            if (tsvPointResult is null)
                return null;
            var tsvPointResultDataList = tsvPointResult.TSVDatas;
            if ((tsvPointResultDataList is null) || (tsvPointResultDataList.Count == 0))
            {
                dcPointsMeasureData.Add(GetPointMeasureData(tsvPointResult, measureSettings, null, siteId, dieRow, dieCol));
            }
            else
            {
                dcPointsMeasureData.AddRange(tsvPointResultDataList.Select(tsvData => GetPointMeasureData(tsvPointResult, measureSettings, tsvData, siteId, dieRow, dieCol)));
            }
            return dcPointsMeasureData;
        }

        private DCPointMeasureData GetPointMeasureData(UnitySC.Shared.Format.Metro.TSV.TSVPointResult measurePointResult, MeasureSettingsBase measureSettings, UnitySC.Shared.Format.Metro.TSV.TSVPointData tsvData, int siteId, int? dieRow = null, int? dieCol = null)
        {
            var dcPointMeasureData = new DCPointMeasureData
            {
                CoordinateX = measurePointResult.WaferRelativeXPosition.Millimeters().Micrometers,
                CoordinateY = measurePointResult.WaferRelativeYPosition.Millimeters().Micrometers,
                PointMeasuresData = new List<DCData>(),
                DieColumnIndex = dieCol ?? 0,
                DieRowIndex = dieRow ?? 0,
                SiteId = siteId
            };

            var dcDepth = new DCDataDouble() { Name = (measureSettings as TSVSettings).DColTSVDepthLabel, IsMeasured = (tsvData != null && (tsvData.DepthState != MeasureState.NotMeasured)), Value = tsvData?.Depth?.Micrometers ?? double.NaN, Unit = "um" };
            dcPointMeasureData.PointMeasuresData.Add(dcDepth);
            var dcCDWidth = new DCDataDouble() { Name = (measureSettings as TSVSettings).DColTSVCDWidthLabel, IsMeasured = (tsvData != null && (tsvData.WidthState != MeasureState.NotMeasured)), Value = tsvData?.Width?.Micrometers ?? double.NaN, Unit = "um" };
            dcPointMeasureData.PointMeasuresData.Add(dcCDWidth);
            var dcCDLength = new DCDataDouble() { Name = (measureSettings as TSVSettings).DColTSVCDLengthLabel, IsMeasured = (tsvData != null && (tsvData.LengthState != MeasureState.NotMeasured)), Value = tsvData?.Length?.Micrometers ?? double.NaN, Unit = "um" };
            dcPointMeasureData.PointMeasuresData.Add(dcCDLength);
            return dcPointMeasureData;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var allDiesStatistics = new List<DCDieStatistics>();
            var tsvResult = measureResult as UnitySC.Shared.Format.Metro.TSV.TSVResult;
            if (tsvResult is null)
                return null;

            if ((tsvResult.Dies is null) || (tsvResult.Dies.Count == 0))
                return null;

            var tsvSettings = measureSettings as TSVSettings;

            foreach (var die in tsvResult.Dies)
            {
                var dieStatistics = new DCDieStatistics() { RowIndex = die.RowIndex, ColumnIndex = die.ColumnIndex, DieStatistics = new List<DCData>() };

                // Calculate Means
                double depthMean = double.NaN;
                double lengthMean = double.NaN;
                double widthMean = double.NaN;

                double depthTotal = 0;
                double lengthTotal = 0;
                double widthTotal = 0;

                int nbDepths = 0;
                int nbWidths = 0;
                int nbLengths = 0;
                foreach (UnitySC.Shared.Format.Metro.TSV.TSVPointResult point in die.Points.OfType<UnitySC.Shared.Format.Metro.TSV.TSVPointResult>())
                {
                    if (point.DepthTsvStat?.Mean != null)
                    {
                        depthTotal += point.DepthTsvStat.Mean.Micrometers;
                        nbDepths++;
                    }
                    if (point.LengthTsvStat?.Mean != null)
                    {
                        lengthTotal += point.LengthTsvStat.Mean.Micrometers;
                        nbLengths++;
                    }
                    if (point.WidthTsvStat?.Mean != null)
                    {
                        widthTotal += point.WidthTsvStat.Mean.Micrometers;
                        nbWidths++;
                    }
                }

                if (nbDepths > 0)
                {
                    depthMean = depthTotal / nbDepths;
                }
                if (nbLengths > 0)
                {
                    lengthMean = lengthTotal / nbLengths;
                }
                if (nbWidths > 0)
                {
                    widthMean = widthTotal / nbWidths;
                }

                var dcMeanDepth = new DCDataDouble() { Name = $"{tsvSettings.DColTSVDepthLabel} Die Average", IsMeasured = (nbDepths > 0), Value = depthMean, Unit = "um" };
                dieStatistics.DieStatistics.Add(dcMeanDepth);
                var dcMeanCDWidth = new DCDataDouble() { Name = $"{tsvSettings.DColTSVCDWidthLabel} Die Average", IsMeasured = (nbWidths > 0), Value = widthMean, Unit = "um" };
                dieStatistics.DieStatistics.Add(dcMeanCDWidth);
                var dcMeanCDLength = new DCDataDouble() { Name = $"{tsvSettings.DColTSVCDLengthLabel} Die Average", IsMeasured = (nbLengths > 0), Value = lengthMean, Unit = "um" };
                dieStatistics.DieStatistics.Add(dcMeanCDLength);

                allDiesStatistics.Add(dieStatistics);
            }
            return allDiesStatistics;
        }

        List<DCData> IMeasureDCProvider.GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var waferStatistics = new List<DCData>();

            var tsvResult = measureResult as UnitySC.Shared.Format.Metro.TSV.TSVResult;
            if (tsvResult is null)
                return null;
            var tsvSettings = measureSettings as TSVSettings;

            var dcMeanDepth = new DCDataDouble() { Name = $"{tsvSettings.DColTSVDepthLabel} Wafer Average", IsMeasured = (tsvResult.DepthTsvStat.State != MeasureState.NotMeasured), Value = tsvResult.DepthTsvStat?.Mean?.Micrometers ?? double.NaN, Unit = "um" };
            waferStatistics.Add(dcMeanDepth);
            var dcMeanCDWidth = new DCDataDouble() { Name = $"{tsvSettings.DColTSVCDWidthLabel} Wafer Average", IsMeasured = (tsvResult.WidthTsvStat.State != MeasureState.NotMeasured), Value = tsvResult.WidthTsvStat?.Mean?.Micrometers ?? double.NaN, Unit = "um" };
            waferStatistics.Add(dcMeanCDWidth);
            var dcMeanCDLength = new DCDataDouble() { Name = $"{tsvSettings.DColTSVCDLengthLabel} Wafer Average", IsMeasured = (tsvResult.LengthTsvStat.State != MeasureState.NotMeasured), Value = tsvResult.LengthTsvStat?.Mean?.Micrometers ?? double.NaN, Unit = "um" };
            waferStatistics.Add(dcMeanCDLength);
            return waferStatistics;
        }

        #endregion IMeasureDCProvider
    }
}
