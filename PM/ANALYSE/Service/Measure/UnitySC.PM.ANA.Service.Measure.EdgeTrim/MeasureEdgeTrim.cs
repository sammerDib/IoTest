using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Profile1D;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.EdgeTrim
{
    public class MeasureEdgeTrim : MeasureBase<EdgeTrimSettings, EdgeTrimPointResult>, IMeasureDCProvider
    {
        private readonly MeasureEdgeTrimConfiguration _measureConfig;

        public MeasureEdgeTrim() : base(ClassLocator.Default.GetInstance<ILogger<MeasureEdgeTrim>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureEdgeTrimConfiguration>().SingleOrDefault();
            if (_measureConfig is null)
            {
                throw new Exception("Edge Trim measure configuration is missing");
            }
        }

        protected override MeasurePointDataResultBase Process(EdgeTrimSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            var flowResult = ExecuteProfile1DFlow(measureSettings, measureContext, cancelToken);
            return AnalyseProfile(flowResult.Profile, measureSettings);
        }

        internal EdgeTrimPointData AnalyseProfile(Profile2d profile, EdgeTrimSettings settings)
        {
            var parameters = new ProfileEdgeTrimAnalyserParameters(
                ProfileEdgeTrimAnalyserParameters.KindStep.Down,
                settings.HeightTarget.Micrometers,
                settings.HeightTolerance.GetAbsoluteTolerance(settings.HeightTarget).Micrometers,
                settings.WidthTarget.Micrometers,
                settings.WidthTolerance.GetAbsoluteTolerance(settings.WidthTarget).Micrometers
            );

            parameters.AddStepExclusionZone(new ExclusionZone(settings.TopEdgeExclusionSize.Millimeters, settings.BottomEdgeExclusionSize.Millimeters));

            var algoResult = new ProfileEdgeTrimAnalyser(parameters).Process(profile) as ProfileEdgeTrimAnalyserResult;

            return new ProfileEdgeTrimAnalyserResultToEdgeTrimPointData(settings).Convert(profile, algoResult);
        }

        private Profile1DFlowResult ExecuteProfile1DFlow(EdgeTrimSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            var measurePoint = measureContext.MeasurePoint.Position;
            double angle = Math.Atan2(measurePoint.Y, measurePoint.X);
            var startPosition = new XYPosition(new WaferReferential())
            {
                X = measurePoint.X - Math.Cos(angle) * measureSettings.PreEdgeScanSize.Millimeters,
                Y = measurePoint.Y - Math.Sin(angle) * measureSettings.PreEdgeScanSize.Millimeters,
            };
            var endPosition = new XYPosition(new WaferReferential())
            {
                X = measurePoint.X + Math.Cos(angle) * measureSettings.PostEdgeScanSize.Millimeters,
                Y = measurePoint.Y + Math.Sin(angle) * measureSettings.PostEdgeScanSize.Millimeters,
            };
            var input = new Profile1DFixedStepInput
            {
                InitialContext = (measureSettings.ProbeSettings as SingleLiseSettings)?.ProbeObjectiveContext,
                LiseData = new LiseInput(measureSettings.ProbeSettings.ProbeId, (measureSettings.ProbeSettings as SingleLiseSettings).LiseGain, _measureConfig.NbAveragingLise),
                StepLength = measureSettings.StepSize,
                StartPosition = startPosition,
                EndPosition = endPosition,
            };

            var flow = FlowsAreSimulated ? new Profile1DFixedStepFlowDummy(input) : new Profile1DFixedStepFlow(input)
            {
                CancellationToken = cancelToken
            };

            flow.Execute();
            return flow.Result;
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(EdgeTrimSettings measureSettings)
        {
  
            var measureTools = new EdgeTrimMeasureTools();
            measureTools.Probes = new List<ProbeWithObjectivesMaterial>();


            // TODO We must return the real compatible probes and objectives depending on the EdgeTrim Height and width
            var objectivesUp = HardwareManager.GetObjectiveConfigsByPosition(ModulePositions.Up);
            var objectivesUpNirId = objectivesUp.Where(x => x.ObjType == ObjectiveConfig.ObjectiveType.VIS).Select(x => x.DeviceID).ToList();

            foreach (var topProbe in HardwareManager.GetProbesConfigsByPosition(ModulePositions.Up))
            {
                if (topProbe is ProbeLiseConfig)
                {
                    var probe = new ProbeWithObjectivesMaterial
                    {
                        CompatibleObjectives = new List<string>(),
                        ProbeId = topProbe.DeviceID
                    };
      
                    var objectiveList = objectivesUp.Where(x => x.ObjType == ObjectiveConfig.ObjectiveType.NIR).Select(x => x.DeviceID).ToList();
                    probe.CompatibleObjectives = objectiveList;
                    measureTools.Probes.Add(probe);
                }
            }
            return measureTools;
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(EdgeTrimSettings measureSettings, Exception ex)
        {
            return new EdgeTrimPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message
            };
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var edgeTrimMeasureSettings = measureSettings as EdgeTrimSettings;
            var metroResult = new EdgeTrimResult();
            metroResult.Settings.HeightTarget = edgeTrimMeasureSettings.HeightTarget;
            metroResult.Settings.HeightTolerance = edgeTrimMeasureSettings.HeightTolerance;
            metroResult.Settings.WidthTarget = edgeTrimMeasureSettings.WidthTarget;
            metroResult.Settings.WidthTolerance = edgeTrimMeasureSettings.WidthTolerance;
            return metroResult;
       }

        public override MeasureType MeasureType => MeasureType.EdgeTrim;

        #region IMeasureDCProvider

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            if (!(measurePointResult is EdgeTrimPointResult edgeTrimPointResult))
            {
                return null;
            }

            var dcPointsMeasureData = new List<DCPointMeasureData>();
            var edgeTrimPointResultDataList = edgeTrimPointResult.Datas?.OfType<EdgeTrimPointData>().ToList();
            bool isWidthMeasured = (measureSettings as EdgeTrimSettings).IsWidthMeasured;

            if ((edgeTrimPointResultDataList is null) || (edgeTrimPointResultDataList.Count == 0))
            {
                dcPointsMeasureData.Add(GetPointMeasureData(edgeTrimPointResult, null, isWidthMeasured, siteId, dieRow, dieCol));
            }
            else
            {
                dcPointsMeasureData.AddRange(edgeTrimPointResultDataList.Select(edgeTrimData => GetPointMeasureData(edgeTrimPointResult, edgeTrimData, isWidthMeasured, siteId, dieRow, dieCol)));
            }

            return dcPointsMeasureData;
        }

        private DCPointMeasureData GetPointMeasureData(EdgeTrimPointResult measurePointResult, EdgeTrimPointData edgeTrimData, bool isWidthMeasured, int siteId, int? dieRow = null, int? dieCol = null)
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

            var dcHeight = new DCDataDouble()
            {
                Name = "Edge Trim Height",
                IsMeasured = edgeTrimData != null && (edgeTrimData.State != MeasureState.NotMeasured),
                Value = edgeTrimData?.Height?.Micrometers ?? double.NaN,
                Unit = "um"
            };
            dcPointMeasureData.PointMeasuresData.Add(dcHeight);

            if (isWidthMeasured)
            {
                var dcCDWidth = new DCDataDouble()
                {
                    Name = "Edge Trim Width",
                    IsMeasured = edgeTrimData != null && (edgeTrimData.State != MeasureState.NotMeasured),
                    Value = edgeTrimData?.Width?.Micrometers ?? double.NaN,
                    Unit = "um"
                };
                dcPointMeasureData.PointMeasuresData.Add(dcCDWidth);
            }

            return dcPointMeasureData;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCData> GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            if (!(measureResult is EdgeTrimResult edgeTrimResult))
            {
                return null;
            }

            var waferStatistics = new List<DCData>();

            var dcMeanHeight = new DCDataDouble()
            {
                Name = "Edge Trim Height Wafer Average",
                IsMeasured = edgeTrimResult.HeightStat.State != MeasureState.NotMeasured,
                Value = edgeTrimResult.HeightStat?.Mean?.Micrometers ?? double.NaN,
                Unit = "um"
            };
            waferStatistics.Add(dcMeanHeight);

            if ((measureSettings as EdgeTrimSettings).IsWidthMeasured)
            {
                var dcMeanCDWidth = new DCDataDouble()
                {
                    Name = "Edge Trim Width Wafer Average",
                    IsMeasured = edgeTrimResult.WidthStat.State != MeasureState.NotMeasured,
                    Value = edgeTrimResult.WidthStat?.Mean?.Micrometers ?? double.NaN,
                    Unit = "um"
                };
                waferStatistics.Add(dcMeanCDWidth);
            }

            return waferStatistics;
        }

        #endregion IMeasureDCProvider
    }
}
