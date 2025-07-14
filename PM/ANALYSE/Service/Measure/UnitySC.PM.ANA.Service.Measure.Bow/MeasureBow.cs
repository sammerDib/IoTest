using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

using static UnitySC.PM.ANA.Service.Measure.Shared.ReferencePlanePositionsHelper;

namespace UnitySC.PM.ANA.Service.Measure.Bow
{
    public class MeasureBow : MeasureBase<BowSettings, BowPointResult>, IMeasureDCProvider
    {
        public override MeasureType MeasureType => MeasureType.Bow;
        private MeasureBowConfiguration _measureConfig;

        // When we need an action on the clamp during the measure, we should run it last because
        // clamping/unclamping can have an effect on the wafer position.
        public override bool WaferUnclampedMeasure => HardwareUtils.ShouldEnsureUnclampedAirGapMeasure(HardwareManager);

        public MeasureBow() : base(ClassLocator.Default.GetInstance<ILogger<MeasureBow>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureBowConfiguration>().SingleOrDefault();
            if (_measureConfig is null)
            {
                throw new Exception("Bow measure configuration is missing");
            }
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var bowSettings = measureSettings as BowSettings;
            var metroResult = new BowResult();
            metroResult.Settings.BowTargetMin = bowSettings.BowMin;
            metroResult.Settings.BowTargetMax = bowSettings.BowMax;
            return metroResult;
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(BowSettings measureSettings, Exception ex)
        {
            return new BowTotalPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message
            };
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(BowSettings measureSettings)
        {
            var measureTools = new BowMeasureTools();
            Length distanceFromCenter = measureSettings.WaferCharacteristic.Diameter / 2 - _measureConfig.DefaultReferencePlanePointsDistanceFromWaferEdge;
            measureTools.DefaultReferencePlanePositions = ReferencePlanePositionsHelper.GenerateDefaultReferencePlanePositions(_measureConfig.DefaultReferencePlanePointsAngularPositions, distanceFromCenter);
            if (ReferencePlanePositionsHelper.AreDefaultReferencePlanePositionsUnreachable(HardwareManager, measureSettings.ProbeSettings.ProbeId))
            {
                ReferencePlanePositionsHelper.RotateUnreachableDefaultReferencePlanePositions(measureTools.DefaultReferencePlanePositions, _measureConfig.ReferencePlanePointsRotationWhenDefaultUnreachable);
            }
            measureTools.CompatibleProbes = GetCompatibleProbes();
            return measureTools;
        }

        private List<ProbeMaterialBase> GetCompatibleProbes()
        {
            return MeasureToolsHelper.GetLiseTopAndBottomWithObjectives(HardwareManager, ModulePositions.Up, ModulePositions.Down).Cast<ProbeMaterialBase>().ToList();
        }

        protected override MeasurePointDataResultBase ComputeMeasureFromSubMeasures(BowSettings measureSettings, MeasureContext measureContext, List<MeasurePointResult> subResults, CancellationToken cancelToken)
        {
            var resData = new BowTotalPointData();
            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;

            try
            {
                // We will identify the nearest point to X=0 & Y=0 coordinates in order to use it for bow calculation
                BowPointData centerBowPointData = null;
                var planeBowPointData = new List<BowPointData>();

                var planeAirGaps = new List<AirGapWithPosition>();
                Length minTT = double.PositiveInfinity.Micrometers();
                Length maxTT = double.NegativeInfinity.Micrometers();
                double minQualityScore = 1d;
                foreach (var bowPointResult in subResults.Cast<BowPointResult>())
                {
                    var bowPointData = bowPointResult.Datas[0] as BowPointData;
                    if (bowPointData != null && bowPointData.State != MeasureState.NotMeasured)
                    {
                        planeBowPointData.Add(bowPointData);
                        if (centerBowPointData is null)
                        {
                            centerBowPointData = bowPointData;
                        }
                        else
                        {
                            if (IsPointClosestToZeroZeroThanCurrentCenter(bowPointData.XPosition, bowPointData.YPosition, centerBowPointData.XPosition, centerBowPointData.YPosition))
                            {
                                centerBowPointData = bowPointData;
                            }
                        }
                    }
                }

                foreach (var bowPointData in planeBowPointData)
                {
                    // for reference plane we use all sub measures points and their airgap, except the center point
                    if (bowPointData.XPosition != centerBowPointData.XPosition || bowPointData.YPosition != centerBowPointData.YPosition)
                    {
                        planeAirGaps.Add(new AirGapWithPosition(bowPointData.AirGap, bowPointData.XPosition, bowPointData.YPosition));
                    }
                }

                ReferencePlanePositionsHelper.ThrowIfMaxReferencePlaneAngleExceeded(planeAirGaps, _measureConfig.MaxReferencePlaneAngle);

                Plane plane = PlaneDetector.FindLeastSquarePlane(planeAirGaps.Select(p => new Point3d(p.XPosition, p.YPosition, p.AirGap.Value)).ToArray());

                if (plane is null)
                {
                    resData.State = MeasureState.NotMeasured;
                    resData.Message = "Air Gap Error - Reference plane is not computable";
                }
                else
                {
                    // Positive bow means convex (mounded) wafer, negative means concave (dished).
                    var planeCenterZ = plane.Center.Z - (plane.Normal.X * (centerBowPointData.XPosition - plane.Center.X) + plane.Normal.Y * (centerBowPointData.YPosition - plane.Center.Y)) / plane.Normal.Z;
                    resData.Bow = new Length(planeCenterZ - centerBowPointData.AirGap.Value, centerBowPointData.AirGap.Unit);
                    resData.QualityScore = planeAirGaps.Count > 0 ? minQualityScore : 0d;
                    resData.State = planeAirGaps.Count >= 3 ? measureSettings.GetBowMeasureState(resData.Bow) : MeasureState.NotMeasured;
                }

                return resData;
            }
            catch (Exception ex)
            {
                resData.State = MeasureState.NotMeasured;
                resData.Message = ex.Message;
                return resData;
            }
        }

        protected override MeasurePointDataResultBase Process(BowSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        protected override MeasurePointDataResultBase SubProcess(BowSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            var resData = new BowPointData();

            try
            {
                resData = MeasureAirGap(measureSettings, measureContext.MeasurePoint.Position.ToXYZTopZBottomPosition(new WaferReferential()));
            }
            catch (Exception ex)
            {
                resData.State = MeasureState.Error;
                resData.Message = ex.Message;
                resData.QualityScore = 0d;
            }

            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
            return resData;
        }

        public override void FinalizeMetroResult(MeasureResultBase measureResultBase, MeasureSettingsBase measureSettingsBase)
        {
            base.FinalizeMetroResult(measureResultBase, measureSettingsBase);
            var bowResult = measureResultBase as BowResult;

            // Loop on all BowPointResult
            foreach (var bowPointResult in bowResult.Points.Cast<BowPointResult>().ToList())
            {
                if (bowPointResult.Datas != null && bowPointResult.Datas.Count > 0)
                {
                    // We remove all BowPointData item from Datas collection in order to keep only BowTotalPointData items
                    bowPointResult.Datas.RemoveAll(d => d is BowPointData);
                }
            }

            // Finally, we remove potential Points without Datas
            bowResult.Points.RemoveAll(p => p.Datas == null || p.Datas.Count == 0);
        }

        private bool IsPointClosestToZeroZeroThanCurrentCenter(double xPoint, double yPoint, double xCurrentCenter, double yCurrentCenter)
        {
            return Math.Sqrt(Math.Pow(xPoint, 2) + Math.Pow(yPoint, 2)) < Math.Sqrt(Math.Pow(xCurrentCenter, 2) + Math.Pow(yCurrentCenter, 2));
        }

        private BowPointData MeasureAirGap(BowSettings measureSettings, XYZTopZBottomPosition position = null)
        {
            var initialContext = new ContextsList(
                new ObjectiveContext
                {
                    ObjectiveId = (measureSettings.ProbeSettings as SingleLiseSettings)?.ProbeObjectiveContext.ObjectiveId
                }
            );

            var airGapInput = new AirGapLiseInput(measureSettings.ProbeSettings.ProbeId, (measureSettings.ProbeSettings as SingleLiseSettings).LiseGain, _measureConfig.NbAveragingLise)
            {
                InitialContext = initialContext
            };
            bool shouldEnsureUnclampedAirGapMeasure = HardwareUtils.ManageWaferUnclampingBeforeAirGapMeasure(HardwareManager,
                measureSettings.WaferCharacteristic.Diameter,
                position,
                _measureConfig.ReleaseWaferTimeoutMilliseconds);

            var airGapFlow = new LiseAirGapFlow(airGapInput);
            var airGapResult = airGapFlow.Execute();

            if (shouldEnsureUnclampedAirGapMeasure)
            {
                HardwareManager.ClampHandler.ClampWafer(measureSettings.WaferCharacteristic.Diameter);
            }

            if (airGapResult.Status.State != FlowState.Success)
            {
                throw new Exception($"Air Gap Error {airGapResult.Status.Message}");
            }

            return new BowPointData
            {
                AirGap = airGapResult.AirGap,
                XPosition = position.X,
                YPosition = position.Y,
                QualityScore = airGapResult.Quality
            };
        }

        #region IMeasureDCProvider

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            var dcPointsMeasureData = new List<DCPointMeasureData>();
            var bowPointResult = measurePointResult as BowPointResult;
            if (bowPointResult is null || bowPointResult.BowTotalPointDatas is null)
            {
                return null;
            }

            // Repeta is disabled in Production, so we can access directly to the first element of the list
            var bowData = bowPointResult.BowTotalPointDatas.Count > 0 ? bowPointResult.BowTotalPointDatas[0] : null;
            var dcPointMeasureData = new DCPointMeasureData
            {
                CoordinateX = bowPointResult.XPosition.Millimeters().Micrometers,
                CoordinateY = bowPointResult.YPosition.Millimeters().Micrometers,
                PointMeasuresData = new List<DCData>(),
                DieColumnIndex = dieCol ?? 0,
                DieRowIndex = dieRow ?? 0,
                SiteId = siteId
            };

            var isMeasured = bowData != null ? bowData.State != MeasureState.NotMeasured : false;
            var dcBow = new DCDataDouble()
            { 
                Name = "BOW", 
                Value = isMeasured ? bowData.Bow.Micrometers : double.NaN,
                Unit = "um",
                IsMeasured = isMeasured
            };
            dcPointMeasureData.PointMeasuresData.Add(dcBow);
            dcPointsMeasureData.Add(dcPointMeasureData);

            return dcPointsMeasureData;
        }

        public List<DCData> GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var waferStatistics = new List<DCData>();
            var bowResult = measureResult as BowResult;
            if (bowResult is null || bowResult.Points is null)
            {
                return null;
            }

            var measurePointResult = bowResult.Points.Last();
            var bowPointResult = measurePointResult as BowPointResult;
            if (bowPointResult is null || bowPointResult.BowTotalPointDatas is null)
            {
                return null;
            }

            var bowData = bowPointResult.BowTotalPointDatas.Count > 0 ? bowPointResult.BowTotalPointDatas[0] : null;

            var isMeasured = bowData != null ? bowData.State != MeasureState.NotMeasured : false;
            var dcBow = new DCDataDouble()
            {
                Name = "BOW Wafer Average",
                Value = isMeasured ? bowData.Bow.Micrometers : double.NaN,
                Unit = "um",
                IsMeasured = isMeasured
            };
            waferStatistics.Add(dcBow);
            return waferStatistics;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        #endregion IMeasureDCProvider
    }
}
