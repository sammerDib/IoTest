using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Profile1D;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Profile1D;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Trench
{
    public class MeasureTrench : MeasureBase<TrenchSettings, TrenchPointResult>, IMeasureDCProvider
    {
        private readonly MeasureTrenchConfiguration _measureConfig;

        public MeasureTrench() : base(ClassLocator.Default.GetInstance<ILogger<MeasureTrench>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureTrenchConfiguration>().SingleOrDefault();
            if (_measureConfig is null)
            {
                throw new Exception("Trench measure configuration is missing");
            }
        }

        protected override MeasurePointDataResultBase Process(TrenchSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            var flowResult = ExecuteProfile1DFlow(measureSettings, measureContext, cancelToken);
            return AnalyseProfile(flowResult.Profile, measureSettings);
        }

        internal TrenchPointData AnalyseProfile(Profile2d profile, TrenchSettings settings)
        {
            var parameters = new ProfileTrenchAnalyserParameters(
                settings.DepthTarget.Micrometers,
                settings.DepthTolerance.GetAbsoluteTolerance(settings.DepthTarget).Micrometers,
                settings.WidthTarget.Micrometers,
                settings.WidthTolerance.GetAbsoluteTolerance(settings.WidthTarget).Micrometers
            );

            parameters.AddTrenchUpExclusionZone(new ExclusionZone(settings.BottomEdgeExclusionSize.Millimeters, settings.TopEdgeExclusionSize.Millimeters));
            parameters.AddTrenchDownExclusionZone(new ExclusionZone(settings.TopEdgeExclusionSize.Millimeters, settings.BottomEdgeExclusionSize.Millimeters));

            var algoResult = new ProfileTrenchAnalyser(parameters).Process(profile) as ProfileTrenchAnalyserResult;

            return new ProfileTrenchAnalyserResultToTrenchPointData(settings).Convert(profile, algoResult);
        }

        private Profile1DFlowResult ExecuteProfile1DFlow(TrenchSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            var measurePoint = measureContext.MeasurePoint.Position;

            var startPosition = new XYPosition(new WaferReferential())
            {
                X = measurePoint.X - Math.Cos(measureSettings.ScanAngle.Radians - (Math.PI / 2.0)) * measureSettings.ScanSize.Millimeters / 2.0,
                Y = measurePoint.Y - Math.Sin(measureSettings.ScanAngle.Radians - (Math.PI / 2.0)) * measureSettings.ScanSize.Millimeters / 2.0,
            };
            var endPosition = new XYPosition(new WaferReferential())
            {
                X = measurePoint.X + Math.Cos(measureSettings.ScanAngle.Radians - (Math.PI / 2.0)) * measureSettings.ScanSize.Millimeters / 2.0,
                Y = measurePoint.Y + Math.Sin(measureSettings.ScanAngle.Radians - (Math.PI / 2.0)) * measureSettings.ScanSize.Millimeters / 2.0,
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

        protected override MeasureToolsBase GetMeasureToolsInternal(TrenchSettings measureSettings)
        {
  
            var measureTools = new TrenchMeasureTools();
            measureTools.Probes = new List<ProbeWithObjectivesMaterial>();


            // TODO We must return the real compatible probes and objectives depending on the Trench Depth and width
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

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(TrenchSettings measureSettings, Exception ex)
        {
            return new TrenchPointData()
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message
            };
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var trenchMeasureSettings = measureSettings as TrenchSettings;
            var metroResult = new TrenchResult();
            metroResult.Settings.DepthTarget = trenchMeasureSettings.DepthTarget;
            metroResult.Settings.DepthTolerance = trenchMeasureSettings.DepthTolerance;
            metroResult.Settings.WidthTarget = trenchMeasureSettings.WidthTarget;
            metroResult.Settings.WidthTolerance = trenchMeasureSettings.WidthTolerance;
            return metroResult;
       }

        public override MeasureType MeasureType => MeasureType.Trench;

        #region IMeasureDCProvider

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            if (!(measurePointResult is TrenchPointResult trenchPointResult))
            {
                return null;
            }

            var dcPointsMeasureData = new List<DCPointMeasureData>();
            var trenchPointResultDataList = trenchPointResult.Datas?.OfType<TrenchPointData>().ToList();
            bool isWidthMeasured = (measureSettings as TrenchSettings).IsWidthMeasured;

            if ((trenchPointResultDataList is null) || (trenchPointResultDataList.Count == 0))
            {
                dcPointsMeasureData.Add(GetPointMeasureData(trenchPointResult, null, isWidthMeasured, siteId, dieRow, dieCol));
            }
            else
            {
                dcPointsMeasureData.AddRange(trenchPointResultDataList.Select(trenchData => GetPointMeasureData(trenchPointResult, trenchData, isWidthMeasured, siteId, dieRow, dieCol)));
            }

            return dcPointsMeasureData;
        }

        private DCPointMeasureData GetPointMeasureData(TrenchPointResult measurePointResult, TrenchPointData trenchData, bool isWidthMeasured, int siteId, int? dieRow = null, int? dieCol = null)
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

            var dcDepth = new DCDataDouble()
            {
                Name = "Trench Depth",
                IsMeasured = trenchData != null && (trenchData.State != MeasureState.NotMeasured),
                Value = trenchData?.Depth?.Micrometers ?? double.NaN,
                Unit = "um"
            };
            dcPointMeasureData.PointMeasuresData.Add(dcDepth);

            if (isWidthMeasured)
            {
                var dcCDWidth = new DCDataDouble()
                {
                    Name = "Trench Width",
                    IsMeasured = trenchData != null && (trenchData.State != MeasureState.NotMeasured),
                    Value = trenchData?.Width?.Micrometers ?? double.NaN,
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
            if (!(measureResult is TrenchResult trenchResult))
            {
                return null;
            }

            var waferStatistics = new List<DCData>();

            var dcMeanDepth = new DCDataDouble()
            {
                Name = "Trench Height Wafer Average",
                IsMeasured = trenchResult.DepthStat.State != MeasureState.NotMeasured,
                Value = trenchResult.DepthStat?.Mean?.Micrometers ?? double.NaN,
                Unit = "um"
            };
            waferStatistics.Add(dcMeanDepth);

            if ((measureSettings as TrenchSettings).IsWidthMeasured)
            {
                var dcMeanCDWidth = new DCDataDouble()
                {
                    Name = "Trench Width Wafer Average",
                    IsMeasured = trenchResult.WidthStat.State != MeasureState.NotMeasured,
                    Value = trenchResult.WidthStat?.Mean?.Micrometers ?? double.NaN,
                    Unit = "um"
                };
                waferStatistics.Add(dcMeanCDWidth);
            }

            return waferStatistics;
        }

 

        #endregion IMeasureDCProvider
    }
}
