using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Warp
{
    public class MeasureWarp : MeasureBase<WarpSettings, WarpPointResult>, IMeasureDCProvider
    {
        public override MeasureType MeasureType => MeasureType.Warp;

        private MeasureWarpConfiguration _measureConfig;

        // When we need an action on the clamp during the measure, we should run it last because
        // clamping/unclamping can have an effect on the wafer position.
        public override bool WaferUnclampedMeasure => HardwareUtils.ShouldEnsureUnclampedAirGapMeasure(HardwareManager);

        public MeasureWarp() : base(ClassLocator.Default.GetInstance<ILogger<MeasureWarp>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureWarpConfiguration>().SingleOrDefault();
            if (_measureConfig is null)
            {
                throw new Exception("Warp measure configuration is missing");
            }
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var warpSettings = measureSettings as WarpSettings;
            if (warpSettings is null)
            {
                throw new Exception("Given warp settings is null, cannot create metro result");
            }

            var metroResult = new WarpResult();
            metroResult.Settings.IsSurfaceWarp = warpSettings.IsSurfaceWarp;
            metroResult.Settings.WarpMax = warpSettings.WarpMax;
            return metroResult;
        }

        private bool IsCalibrationNeeded(MeasureSettingsBase measureSettingsBase)
        {
            var warpSettings = measureSettingsBase as WarpSettings;
            return warpSettings.ProbeSettings is DualLiseSettings || warpSettings.ProbeSettings is LiseHFSettings;
        }

        public override bool PrepareExecution(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default)
        {

            if (IsCalibrationNeeded(measureSettings))
            {
                switch ((measureSettings as WarpSettings).ProbeSettings)
                {
                    case LiseHFSettings liseHFsettings:
                        {
                            var inputParams = LiseHFInputParamsFactory.FromLiseHFSettings(liseHFsettings);

                            var calibrationManager = ClassLocator.Default.GetInstance<AnaHardwareManager>().Probes[liseHFsettings.ProbeId].CalibrationManager;
                            if (calibrationManager.GetCalibration(true, liseHFsettings.ProbeId, inputParams, null) == null)
                            {
                                Logger.Error($"Failed to get calibration for probe {liseHFsettings?.ProbeId} - [{inputParams.ObjectiveId}] {(inputParams.IsLowIlluminationPower ? "LowIllum" : string.Empty)}");
                                return false;
                            }
                        }
                        break;

                    default:
                        return true; // noting to do all ok

                }
            }

            return true; // noting to do all ok
        }

        public override void MeasureTerminatedInRecipe(MeasureSettingsBase measureSettingsBase)
        {
            if (measureSettingsBase is WarpSettings warpSettings && !(warpSettings.ProbeSettings is null))
            {
                var probe = HardwareManager.Probes[warpSettings.ProbeSettings.ProbeId];
                probe.CalibrationManager?.MeasureExecutionTerminated();
            }
        }

        public override bool CanZAxisMove(MeasureSettingsBase measureSettingsBase)
        {
            return false;
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(WarpSettings measureSettings, Exception ex)
        {
            var data = new WarpTotalPointData();
            data.State = MeasureState.NotMeasured;
            data.Message = ex.Message;
            return data;
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(WarpSettings measureSettings)
        {
            var measureTools = new WarpMeasureTools();
            Length distanceFromCenter = measureSettings.WaferCharacteristic.Diameter / 2 - _measureConfig.DefaultReferencePlanePointsDistanceFromWaferEdge;
            measureTools.DefaultReferencePlanePositions = ReferencePlanePositionsHelper.GenerateDefaultReferencePlanePositions(_measureConfig.DefaultReferencePlanePointsAngularPositions, distanceFromCenter);
            if (ReferencePlanePositionsHelper.AreDefaultReferencePlanePositionsUnreachable(HardwareManager, measureSettings.ProbeSettings.ProbeId))
            {
                ReferencePlanePositionsHelper.RotateUnreachableDefaultReferencePlanePositions(measureTools.DefaultReferencePlanePositions, _measureConfig.ReferencePlanePointsRotationWhenDefaultUnreachable);
            }
            measureTools.CompatibleProbes = GetCompatibleProbes(measureSettings);
            return measureTools;
        }

        private List<ProbeMaterialBase> GetCompatibleProbes(WarpSettings measureSettings)
        {
            ProbeWithObjectivesMaterial probeUp = MeasureToolsHelper.GetProbeLiseWithObjectives(HardwareManager, ModulePositions.Up);
            ProbeWithObjectivesMaterial probeDown = MeasureToolsHelper.GetProbeLiseWithObjectives(HardwareManager, ModulePositions.Down);
            DualProbeWithObjectivesMaterial dualProbe = MeasureToolsHelper.GetDualLiseProbe(HardwareManager, probeUp, probeDown);

            var compatibleProbes = new List<ProbeMaterialBase>();

            if (!measureSettings.IsSurfaceWarp)
            {
                compatibleProbes.Add(dualProbe);
            }

            if (measureSettings.IsSurfaceWarp || measureSettings.IsWaferTransparent)
            {
                compatibleProbes.Add(probeUp);
            }

            return compatibleProbes;
        }

        protected override MeasurePointDataResultBase ComputeMeasureFromSubMeasures(WarpSettings measureSettings, MeasureContext measureContext, List<MeasurePointResult> subResults, CancellationToken cancelToken)
        {
            var resData = new WarpTotalPointData();
            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;

            try
            {
                var pointsForPlane = new List<WarpPointForPlane>();
                Length minTT = double.PositiveInfinity.Micrometers();
                Length maxTT = double.NegativeInfinity.Micrometers();
                double minQualityScore = 1d;
                foreach (var warpPointResult in subResults.Cast<WarpPointResult>())
                {
                    var warpPointData = warpPointResult.Datas[0] as WarpPointData;
                    if (warpPointData != null && warpPointData.State != MeasureState.NotMeasured)
                    {
                        var pointForPlane = GetPointForPlane(warpPointData, measureSettings.IsSurfaceWarp);
                        if (pointForPlane != null)
                        {
                            pointsForPlane.Add(pointForPlane);
                            if (minQualityScore > warpPointData.QualityScore)
                            {
                                minQualityScore = warpPointData.QualityScore;
                            }
                        }
                        else
                        {
                            warpPointData.State = MeasureState.Error;
                        }

                        if (!measureSettings.IsSurfaceWarp && warpPointData.TotalThickness != null)
                        {
                            if (warpPointData.TotalThickness < minTT)
                            {
                                minTT = warpPointData.TotalThickness;
                            }

                            if (warpPointData.TotalThickness > maxTT)
                            {
                                maxTT = warpPointData.TotalThickness;
                            }
                        }
                    }
                }

                if (pointsForPlane.Count >= 9)
                {
                    resData.Warp = ComputeWarpAndRPD(pointsForPlane);
                    resData.QualityScore = minQualityScore;
                    resData.State = measureSettings.GetWarpMeasureState(resData.Warp);
                }
                else
                {
                    resData.QualityScore = 0d;
                    resData.State = MeasureState.NotMeasured;
                    resData.Message = "Warp was not calculated since at least 9 measured points are necessary for its calculation";
                }

                if (!measureSettings.IsSurfaceWarp && maxTT != double.NegativeInfinity.Micrometers() && minTT != double.PositiveInfinity.Micrometers())
                {
                    resData.TTV = maxTT - minTT;
                }

                return resData;
            }
            catch (Exception ex)
            {
                return resData;
            }
        }

        protected override MeasurePointDataResultBase Process(WarpSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        protected override MeasurePointDataResultBase SubProcess(WarpSettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            var resData = new WarpPointData();

            try
            {
                resData = ComputeWarpPointData(measureSettings, measureContext.MeasurePoint);
            }
            catch (Exception ex)
            {
                resData.State = MeasureState.NotMeasured;
                resData.Message = ex.Message;
                resData.QualityScore = 0d;
            }

            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
            return resData;
        }

        private WarpPointData ComputeWarpPointData(WarpSettings measureSettings, MeasurePoint measurePoint)
        {
            var warpPointData = MeasureAirGapsAndThickness(measureSettings, measurePoint);
            warpPointData.IsSurfaceWarp = measureSettings.IsSurfaceWarp;
            if (!measureSettings.IsSurfaceWarp && warpPointData.TotalThickness != null)
            {
                Length distanceToWaferMiddle = null;
                if (warpPointData.AirGapUp != null)
                {
                    distanceToWaferMiddle = new Length(warpPointData.AirGapUp.Micrometers + warpPointData.TotalThickness.Micrometers / 2, LengthUnit.Micrometer);
                }
                else if (warpPointData.AirGapDown != null)
                {
                    distanceToWaferMiddle = new Length(warpPointData.AirGapDown.Micrometers + warpPointData.TotalThickness.Micrometers / 2, LengthUnit.Micrometer);
                }
                warpPointData.ZMedian = distanceToWaferMiddle;
            }

            return warpPointData;
        }

        private WarpPointForPlane GetPointForPlane(WarpPointData warpPointData, bool isSurfaceWarp)
        {
            WarpPointForPlane pointForPlane = null;
            if (isSurfaceWarp)
            {
                var point3d = new Point3d(warpPointData.XPosition, warpPointData.YPosition, warpPointData.AirGapUp.Millimeters);
                pointForPlane = new WarpPointForPlane(point3d, warpPointData);
            }
            else if (warpPointData.ZMedian != null)
            {
                var point3d = new Point3d(warpPointData.XPosition, warpPointData.YPosition, warpPointData.ZMedian.Millimeters);
                pointForPlane = new WarpPointForPlane(point3d, warpPointData);
            }

            return pointForPlane;
        }

        /// <summary>
        /// For each point, compute and update RDP
        /// Then compute and return Warp
        /// </summary>
        /// <param name="pointsForPlane"></param>
        /// <returns>Warp if it is possible to calculate</returns>
        private Length ComputeWarpAndRPD(List<WarpPointForPlane> pointsForPlane)
        {
            Length warp = null;
            Plane plane = PlaneDetector.FindLeastSquarePlane(pointsForPlane.Select(p => p.PointForPlane).ToArray());
            if (plane != null)
            {
                // Compute RPD for each point
                Length minRPD = double.PositiveInfinity.Micrometers();
                Length maxRPD = double.NegativeInfinity.Micrometers();
                foreach (var point in pointsForPlane)
                {
                    Length rpd = plane.DistanceTo(point.PointForPlane).Millimeters().ToUnit(LengthUnit.Micrometer);
                    point.PointData.RPD = rpd;

                    if (rpd < minRPD)
                    {
                        minRPD = rpd;
                    }

                    if (rpd > maxRPD)
                    {
                        maxRPD = rpd;
                    }
                }

                // Compute warp
                warp = maxRPD - minRPD;
            }

            return warp;
        }

        public override void FinalizeMetroResult(MeasureResultBase measureResultBase, MeasureSettingsBase measureSettingsBase)
        {
            base.FinalizeMetroResult(measureResultBase, measureSettingsBase);
            var warpResult = measureResultBase as WarpResult;
            // Loop on all WarpPointResult
            foreach(var warpPointResult in warpResult.Points.Cast<WarpPointResult>().ToList())
            {
                if (warpPointResult.Datas != null && warpPointResult.Datas.Count > 0)
                {
                    // Take all datas that are WarpTotalPointData and use it to fill the WarpResult WarpWaferResults & TTVWaferResults lists
                    foreach (var warpTotalPointData in warpPointResult.Datas.Where(d => d is WarpTotalPointData)?.Cast<WarpTotalPointData>())
                    {
                        warpResult.WarpWaferResults.Add(warpTotalPointData.Warp);
                        warpResult.TTVWaferResults.Add(warpTotalPointData.TTV);
                    }

                    // Then we remove all WarpTotalPointData item from Datas collection in order to keep only WarpPointData items
                    warpPointResult.Datas.RemoveAll(d => d is WarpTotalPointData);
                }
            }

            // Finally, we remove potential Points without Datas
            warpResult.Points.RemoveAll(p => p.Datas == null || p.Datas.Count == 0);
        }

        public override void ApplyMeasureCorrection(MeasureResultBase measureResult, MeasureSettingsBase measureSettingsBase)
        {
            if (IsCalibrationNeeded(measureSettingsBase))
            {
                foreach (var point in measureResult.Points)
                {
                    foreach (var repeatPoint in point.Datas)
                    {
                        if (point.State == MeasureState.NotMeasured)
                        {
                            continue;
                        }

                        try
                        {
                            ApplyMeasurePointCorrection(repeatPoint, measureSettingsBase);
                        }
                        catch (Exception ex)
                        {
                            repeatPoint.State = MeasureState.NotMeasured;
                            repeatPoint.Message = ex.Message;
                            repeatPoint.QualityScore = 0;
                        }
                    }
                }
            }
        }

        private static int FindUnknownLayerIndex(WarpSettings warpSettings, ProbeDualLiseConfig config)
        {
            int unknownLayerIndex = 0;
            bool layerWithUnknownRIIsFound = warpSettings.PhysicalLayers.Any(layer => layer.RefractiveIndex.IsNullOrNaN());

            bool layerWithSmallerRIIFound = warpSettings.PhysicalLayers.Any(layer => (layer.Thickness.Micrometers / layer.RefractiveIndex) < config.ThicknessThresholdInTheAir.Micrometers);

            if (layerWithUnknownRIIsFound)
            {
                unknownLayerIndex = warpSettings.PhysicalLayers.FindIndex(layer => layer.RefractiveIndex.IsNullOrNaN());
            }
            if (layerWithSmallerRIIFound)
            {
                unknownLayerIndex = warpSettings.PhysicalLayers.FindIndex(layer => (layer.Thickness.Micrometers / layer.RefractiveIndex) < 30);
            }

            return unknownLayerIndex;
        }

        private WarpPointData MeasureAirGapsAndThicknessWithDualLise(WarpSettings measureSettings, MeasurePoint measurePoint)
        {
            var dualLiseSettings = measureSettings.ProbeSettings as DualLiseSettings;
            var probe = HardwareUtils.GetProbeLiseFromID(HardwareManager, dualLiseSettings.ProbeId);
            var config = probe.Configuration as ProbeDualLiseConfig;
            var probeLiseCalibResult = probe.CalibrationManager.GetCalibration(true, config.DeviceID, null, measurePoint.Position, null) as ProbeDualLiseCalibResult;

            var position = measurePoint.Position.ToXYZTopZBottomPosition(new WaferReferential());
            bool shouldEnsureUnclampedAirGapMeasure = HardwareUtils.ManageWaferUnclampingBeforeAirGapMeasure(HardwareManager, 
                measureSettings.WaferCharacteristic.Diameter, 
                position, 
                _measureConfig.ReleaseWaferTimeoutMilliseconds);

            int unknownLayerIndex = FindUnknownLayerIndex(measureSettings, config);
            var layersUp = measureSettings.PhysicalLayers.GetRange(0, unknownLayerIndex);
            var layersDown = measureSettings.PhysicalLayers.GetRange(unknownLayerIndex + 1, measureSettings.PhysicalLayers.Count - layersUp.Count - 1);
            var unkonwnLayer = measureSettings.PhysicalLayers.ElementAt(unknownLayerIndex);

            var liseUpInput = new MeasureLiseInput(new ThicknessLiseInput(config.ProbeUpID, GetProbeSample(layersUp, measureSettings.TotalThicknessTolerance, config), dualLiseSettings.LiseUp.LiseGain, _measureConfig.NbAveragingLise));
            var liseDownInput = new MeasureLiseInput(new ThicknessLiseInput(config.ProbeDownID, GetProbeSample(layersDown, measureSettings.TotalThicknessTolerance, config), dualLiseSettings.LiseDown.LiseGain, _measureConfig.NbAveragingLise));
            var unknownLayerSample = new ProbeSampleLayer()
            {
                Name = unkonwnLayer.Name,
                Thickness = unkonwnLayer.Thickness,
                IsMandatory = true,
                RefractionIndex = (double)unkonwnLayer.RefractiveIndex,
                Tolerance = measureSettings.TotalThicknessTolerance
            };

            var dualLiseInput = new MeasureDualLiseInput(config.DeviceID, liseUpInput, liseDownInput, unknownLayerSample, probeLiseCalibResult);
            var airGapsAndThicknessFlow = FlowsAreSimulated ? new DualLiseThicknessMeasurementFlowDummy(dualLiseInput) : new DualLiseThicknessMeasurementFlow(dualLiseInput);
            var airGapsAndThicknessResult = airGapsAndThicknessFlow.Execute();

            if (shouldEnsureUnclampedAirGapMeasure)
            {
                HardwareManager.ClampHandler.ClampWafer(measureSettings.WaferCharacteristic.Diameter);
            }

            if (airGapsAndThicknessResult.Status.State != FlowState.Success)
            {
                throw new Exception($"Dual Lise Air Gap Error {airGapsAndThicknessResult.Status.Message}");
            }

            var totalThickness = new Length(0d, LengthUnit.Micrometer);
            airGapsAndThicknessResult.LayersThickness.ForEach(layer => totalThickness += layer.Thickness);
            var warpPointData = new WarpPointData
            {
                TotalThickness = totalThickness,
                AirGapUp = airGapsAndThicknessResult.AirGapUp,
                AirGapDown = airGapsAndThicknessResult.AirGapDown,
                XPosition = position.X,
                YPosition = position.Y,
                QualityScore = airGapsAndThicknessResult.Quality
            };

            if (measureSettings.TheoricalWaferThickness != null && measureSettings.TheoricalWaferThickness.Value != 0)
            {
                var numerator = Math.Max(warpPointData.TotalThickness.Micrometers, measureSettings.TheoricalWaferThickness.Micrometers);
                var denominator = Math.Min(warpPointData.TotalThickness.Micrometers, measureSettings.TheoricalWaferThickness.Micrometers);
                if (numerator / denominator > _measureConfig.DualLiseTotalThicknessValidityFactor)
                {
                    var micrometerSymbol = Length.GetUnitSymbol(LengthUnit.Micrometer);
                    warpPointData.State = MeasureState.NotMeasured;
                    warpPointData.Message = String.Format(
                        "Total thickness measured ({0:f3}" + " " + micrometerSymbol + ") is inconsistent according to theorical layer thickness ({1:f3}" + " " + micrometerSymbol + "). Measure point is probably unsuitable for warp measurement",
                        warpPointData.TotalThickness.Micrometers,
                        measureSettings.TheoricalWaferThickness.Micrometers);
                    warpPointData.TotalThickness = null;
                }
            }

            return warpPointData;
        }

        private WarpPointData MeasureAirGapsAndThicknessWithTopLise(WarpSettings measureSettings, MeasurePoint measurePoint)
        {
            var singleLiseSettings = measureSettings.ProbeSettings as SingleLiseSettings;
            var probe = HardwareUtils.GetProbeLiseFromID(HardwareManager, singleLiseSettings.ProbeId);
            var config = probe.Configuration as ProbeLiseConfig;

            var position = measurePoint.Position.ToXYZTopZBottomPosition(new WaferReferential());
            bool shouldEnsureUnclampedAirGapMeasure = HardwareUtils.ManageWaferUnclampingBeforeAirGapMeasure(HardwareManager,
                measureSettings.WaferCharacteristic.Diameter,
                position,
                _measureConfig.ReleaseWaferTimeoutMilliseconds);

            var probeSample = measureSettings.IsSurfaceWarp ? GetProbeSample(new List<LayerSettings>(), null, config) : GetProbeSample(measureSettings.PhysicalLayers, measureSettings.TotalThicknessTolerance, config);
            var liseUpInput = new MeasureLiseInput(new ThicknessLiseInput(config.DeviceID, probeSample, singleLiseSettings.LiseGain, _measureConfig.NbAveragingLise));
            var airGapsAndThicknessFlow = FlowsAreSimulated ? new LiseThicknessMeasurementFlowDummy(liseUpInput) : new LiseThicknessMeasurementFlow(liseUpInput);
            var airGapsAndThicknessResult = airGapsAndThicknessFlow.Execute();

            if (shouldEnsureUnclampedAirGapMeasure)
            {
                HardwareManager.ClampHandler.ClampWafer(measureSettings.WaferCharacteristic.Diameter);
            }

            if (airGapsAndThicknessResult.Status.State != FlowState.Success)
            {
                throw new Exception($"Lise Air Gap Error {airGapsAndThicknessResult.Status.Message}");
            }

            var totalThickness = new Length(0d, LengthUnit.Micrometer);
            airGapsAndThicknessResult.LayersThickness.ForEach(layer => totalThickness += layer.Thickness);
            return new WarpPointData
            {
                TotalThickness = measureSettings.IsSurfaceWarp ? null : totalThickness,
                AirGapUp = airGapsAndThicknessResult.AirGap,
                XPosition = position.X,
                YPosition = position.Y,
                QualityScore = airGapsAndThicknessResult.Quality
            };
        }

        private WarpPointData MeasureAirGapsAndThickness(WarpSettings measureSettings, MeasurePoint measurePoint)
        {
            if (measureSettings.ProbeSettings is DualLiseSettings dualLiseSettings)
            {
                return MeasureAirGapsAndThicknessWithDualLise(measureSettings, measurePoint);
            }

            return MeasureAirGapsAndThicknessWithTopLise(measureSettings, measurePoint);
        }

        private ProbeSample GetProbeSample(List<LayerSettings> layers, LengthTolerance tolerance, IProbeConfig probeConfig)
        {
            var probeSampleLayersList = new List<ProbeSampleLayer>();
            foreach (var physicalLayer in layers)
            {
                var probeSampleLayer = new ProbeSampleLayer()
                {
                    Name = physicalLayer.Name,
                    Thickness = physicalLayer.Thickness,
                    IsMandatory = true,
                    RefractionIndex = (double)physicalLayer.RefractiveIndex,
                    Tolerance = tolerance
                };
                probeSampleLayersList.Add(probeSampleLayer);
            }
            if (probeConfig.ModulePosition == ModulePositions.Down)
            {
                probeSampleLayersList.Reverse();
            }

            var probeSample = new ProbeSample()
            {
                // TODO Name and Info not needed? See if it can be removed?
                Name = "",
                Layers = probeSampleLayersList,
                Info = ""
            };

            return probeSample;
        }

        private class WarpPointForPlane
        {
            public WarpPointForPlane(Point3d pointForPlane, WarpPointData pointData)
            {
                PointForPlane = pointForPlane;
                PointData = pointData;
            }

            public Point3d PointForPlane { get; }
            public WarpPointData PointData { get; }
        }

        private void ApplyMeasurePointCorrection(MeasurePointDataResultBase measurePointDataResult, MeasureSettingsBase measureSettingsBase)
        {
            var warpSettings = measureSettingsBase as WarpSettings;

            if (warpSettings is null)
            {
                return;
            }

            var probeCalibrationManager = HardwareManager.Probes[warpSettings.ProbeSettings.ProbeId].CalibrationManager;
            if (!(probeCalibrationManager is null))
            {
                probeCalibrationManager.CorrectMeasurePoint(measurePointDataResult as WarpPointData);
            }
        }

        #region IMeasureDCProvider

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var dcPointsMeasureData = new List<DCPointMeasureData>();
            var warpResult = measureResult as WarpResult;
            if (warpResult is null || warpResult.WarpWaferResults is null)
            {
                return null;
            }

            // Get point in (0, 0) coordinates if it exists
            var measurePointZero = warpResult.Points.FirstOrDefault(p => p.XPosition == 0d && p.YPosition == 0d);

            var dcPointMeasureData = new DCPointMeasureData
            {
                CoordinateX = measurePointZero != null ? measurePointZero.XPosition : 0d,
                CoordinateY = measurePointZero != null ? measurePointZero.YPosition : 0d,
                PointMeasuresData = new List<DCData>(),
                DieColumnIndex = 0,
                DieRowIndex = 0,
                SiteId = measurePointZero != null ? measurePointZero.SiteId : 0
            };

            // Repeta is disabled in Production, so we can access directly to the first element of the list
            bool isWarpMeasured = warpResult.WarpWaferResults.Count > 0 && warpResult.WarpWaferResults[0] != null;
            var dcWarp = new DCDataDouble()
            {
                Name = "WARP",
                Value = isWarpMeasured ? warpResult.WarpWaferResults[0].Micrometers : double.NaN,
                Unit = "um",
                IsMeasured = isWarpMeasured
            };
            dcPointMeasureData.PointMeasuresData.Add(dcWarp);

            // Repeta is disabled in Production, so we can access directly to the first element of the list
            bool isTTVMeasured = warpResult.TTVWaferResults.Count > 0 && warpResult.TTVWaferResults[0] != null;
            var dcTTV = new DCDataDouble()
            {
                Name = "TTV",
                Value = isTTVMeasured ? warpResult.TTVWaferResults[0].Micrometers : double.NaN,
                Unit = "um",
                IsMeasured = isTTVMeasured
            };
            dcPointMeasureData.PointMeasuresData.Add(dcTTV);

            dcPointsMeasureData.Add(dcPointMeasureData);

            return dcPointsMeasureData;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            return null;
        }

        public List<DCData> GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            // Same as for GetDCResultBase -- need to add Average
            var waferStatistics = new List<DCData>();
            var warpResult = measureResult as WarpResult;
            if (warpResult is null || warpResult.WarpWaferResults is null)
            {
                return null;
            }

            // Repeta is disabled in Production, so we can access directly to the first element of the list
            bool isWarpMeasured = warpResult.WarpWaferResults.Count > 0 && warpResult.WarpWaferResults[0] != null;
            var dcWarp = new DCDataDouble()
            {
                Name = "WARP Wafer Average",
                Value = isWarpMeasured ? warpResult.WarpWaferResults[0].Micrometers : double.NaN,
                Unit = "um",
                IsMeasured = isWarpMeasured
            };
            waferStatistics.Add(dcWarp);

            // Repeta is disabled in Production, so we can access directly to the first element of the list
            bool isTTVMeasured = warpResult.TTVWaferResults.Count > 0 && warpResult.TTVWaferResults[0] != null;
            var dcTTV = new DCDataDouble()
            {
                Name = "TTV Wafer Average",
                Value = isTTVMeasured ? warpResult.TTVWaferResults[0].Micrometers : double.NaN,
                Unit = "um",
                IsMeasured = isTTVMeasured
            };
            waferStatistics.Add(dcTTV);

            return waferStatistics;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        #endregion
    }
}
