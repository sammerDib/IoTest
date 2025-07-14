using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Core.Thickness;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Warp;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosCppWrapper;

namespace UnitySC.PM.ANA.Service.Measure.Thickness
{
    public class MeasureThickness : MeasureBase<ThicknessSettings, ThicknessPointResult>, IMeasureDCProvider
    {
        private LiseThicknessMeasurementFlow _liseFlowBeingExecuted;
        private DualLiseThicknessMeasurementFlow _dualLiseFlowBeingExecuted;
        private MeasureToolsForLayers _measureToolsForLayers;
        private readonly MeasureThicknessConfiguration _measureConfig;

        public MeasureThickness() : base(ClassLocator.Default.GetInstance<ILogger<MeasureThickness>>())
        {
            _measureToolsForLayers = new MeasureToolsForLayers(HardwareManager);
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureThicknessConfiguration>().SingleOrDefault();
            if (_measureConfig is null)
            {
                throw new Exception("Thickness measure configuration is missing");
            }
        }

        public override MeasureType MeasureType => MeasureType.Thickness;

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var thicknessSettings = measureSettings as ThicknessSettings;
            if (thicknessSettings is null)
            {
                throw new Exception("Given thickness settings is null, cannot create metro result");
            }

            var unifiedLayers = new UnifiedLayers(thicknessSettings);
            thicknessSettings.PhysicalLayers = unifiedLayers.PhysicalLayers;
            thicknessSettings.LayersToMeasure = unifiedLayers.LayersToMeasure;
            var thicknessLayers = new List<ThicknessLengthSettings>();
            var totalThicknessLayer = thicknessSettings.LayersToMeasure.FirstOrDefault(layer => layer.IsWaferTotalThickness);
            foreach (var rcpSettings in thicknessSettings.PhysicalLayers)
            {
                thicknessLayers.Add(new ThicknessLengthSettings()
                {
                    Name = rcpSettings.Name,
                    Target = rcpSettings.Thickness,
                    Tolerance = GetTolerance(rcpSettings, thicknessSettings.LayersToMeasure),
                    IsMeasured = MustLayerBeMeasured(rcpSettings, thicknessSettings.LayersToMeasure),
                    LayerColor = rcpSettings.LayerColor
                });
            }

            var metroResult = new ThicknessResult();
            metroResult.Settings.ThicknessLayers = thicknessLayers;
            metroResult.Settings.HasWarpMeasure = thicknessSettings.HasWarpMeasure;
            metroResult.Settings.WarpTargetMax = thicknessSettings.WarpTargetMax;
            metroResult.Settings.HasWaferThicknesss = totalThicknessLayer != null;
            metroResult.Settings.TotalTolerance = totalThicknessLayer?.ThicknessTolerance != null ? totalThicknessLayer.ThicknessTolerance : new LengthTolerance(5, LengthToleranceUnit.Micrometer);
            metroResult.Settings.TotalTarget = thicknessSettings.PhysicalLayers.Sum(layer => layer.Thickness.Micrometers).Micrometers();
            return metroResult;
        }

        private bool IsCalibrationNeeded(MeasureSettingsBase measureSettingsBase)
        {
            if (measureSettingsBase is ThicknessSettings thicknessSettings) 
            {
                if (thicknessSettings.HasWarpMeasure)
                    return true;

                if (thicknessSettings.LayersToMeasure != null)
                {
                    foreach (var layerToMasure in thicknessSettings.LayersToMeasure)
                    {
                        if (layerToMasure.ProbeSettings is DualLiseSettings || layerToMasure.ProbeSettings is LiseHFSettings)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override bool PrepareExecution(MeasureSettingsBase measureSettings, MeasureContext measureContext, CancellationToken cancelToken = default)
        {
            // TODO When thickeness measurement will be available with lise hf probe we have to handle how to perform calibration

            //if (IsCalibrationNeeded(measureSettings))
            //{
            //    switch ((measureSettings as ThicknessSettings).ProbeSettings) //  Need to check layers to measure probes
            //    {
            //        case LiseHFSettings liseHFsettings:
            //            {
            //               var inputParams = LiseHFInputParamsFactory.FromLiseHFSettings(liseHFsettings);

            //                var calibrationManager = ClassLocator.Default.GetInstance<AnaHardwareManager>().Probes[liseHFsettings.ProbeId].CalibrationManager;
            //                if (calibrationManager.GetCalibration(true, liseHFsettings.ProbeId, inputParams, null) == null)
            //                {
            //                    Logger.Error($"Failed to get calibration for probe {liseHFsettings?.ProbeId} - [{inputParams.ObjectiveId}] {(inputParams.IsLowIlluminationPower ? "LowIllum" : string.Empty)}");
            //                    return false;
            //                }
            //            }
            //            break;

            //        default:
            //            return true; // noting to do all ok

            //    }
            //}

            return true; // noting to do all ok
        }

        public override void MeasureTerminatedInRecipe(MeasureSettingsBase measureSettingsBase)
        {
            if (measureSettingsBase is ThicknessSettings thicknessSettings && !(thicknessSettings.LayersToMeasure is null))
            {
                var uniqueProbIds = thicknessSettings.LayersToMeasure.Select(layer => layer.ProbeSettings.ProbeId).Distinct();
                foreach (var probeId in uniqueProbIds)
                {
                    var probe = HardwareManager.Probes[probeId];
                    probe.CalibrationManager?.MeasureExecutionTerminated();
                }
            }
        }

        public override bool CanZAxisMove(MeasureSettingsBase measureSettingsBase)
        {
            if (measureSettingsBase is ThicknessSettings thicknessSettings)
            {
                if (thicknessSettings.HasWarpMeasure)
                {
                    return false;
                }

                foreach (var layerToMasure in thicknessSettings.LayersToMeasure)
                {
                    if (layerToMasure.ProbeSettings is DualLiseSettings)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void ApplyMeasureCorrection(MeasureResultBase measureResult, MeasureSettingsBase measureSettingsBase)
        {
            if (IsCalibrationNeeded(measureSettingsBase))
            {
                if (measureResult.Dies != null)
                {
                    foreach (var die in measureResult.Dies)
                    {
                        foreach (var point in die.Points)
                        {
                            foreach (var repeatPoint in point.Datas)
                            {
                                if (point.State == MeasureState.NotMeasured)
                                    continue;

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
                else
                {
                    foreach (var point in measureResult.Points)
                    {
                        foreach (var repeatPoint in point.Datas)
                        {
                            if (point.State == MeasureState.NotMeasured)
                                continue;

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
        }

        public override void FinalizeMetroResult(MeasureResultBase measureResultBase, MeasureSettingsBase measureSettingsBase)
        {
            base.FinalizeMetroResult(measureResultBase, measureSettingsBase);
            var thicknessResult = measureResultBase as ThicknessResult;

            if (!thicknessResult.Settings.HasWarpMeasure)
            {
                return;
            }

            var pointsForPlane = new List<ThicknessPointForPlane>();
            foreach (var point in thicknessResult.Points)
            {
                // Don't take only points that were not measured
                if (point.State == MeasureState.NotMeasured)
                {
                    continue;
                }

                var iPointData = point.Datas != null ? point.Datas.Count - 1 : -1;
                Length distanceToWaferMiddle = null;
                ThicknessPointData pointData = null;
                if (iPointData >= 0)
                {
                    pointData = point.Datas[iPointData] as ThicknessPointData;
                    // Compute called here in order to fill TotalThickness before using it
                    pointData?.ComputeTotalThickness(thicknessResult.Settings);
                    if (pointData?.TotalThickness != null)
                    {
                        if (pointData.AirGapUp != null)
                        {
                            distanceToWaferMiddle = pointData?.AirGapUp + pointData?.TotalThickness / 2;
                        }
                        else
                        {
                            distanceToWaferMiddle = pointData?.AirGapDown + pointData?.TotalThickness / 2;
                        }
                    }
                }

                if (distanceToWaferMiddle != null)
                {
                    var point3d = new Point3d(point.XPosition, point.YPosition, distanceToWaferMiddle.Millimeters);
                    pointsForPlane.Add(new ThicknessPointForPlane(point3d, pointData));
                }
                else
                {
                    point.State = MeasureState.Error;
                    point.Message = "Warp cannot be computed";
                }
            }

            Plane plane = PlaneDetector.FindLeastSquarePlane(pointsForPlane.Select(p => p.PointForPlane).ToArray());
            if (plane is null)
            {
                thicknessResult.WarpWaferResults.Add(null);
            }
            else
            {
                // Compute RPD for each measure point
                Length minRPD = double.PositiveInfinity.Micrometers();
                Length maxRPD = double.NegativeInfinity.Micrometers();
                foreach (var point in pointsForPlane)
                {
                    Length rpd = plane.DistanceTo(point.PointForPlane).Millimeters().ToUnit(LengthUnit.Micrometer);
                    point.PointData.WarpResult = new WarpPointData() { RPD = rpd };

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
                Length warp = maxRPD - minRPD;
                thicknessResult.WarpWaferResults.Add(warp);
            }
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(ThicknessSettings measureSettings, Exception ex)
        {
            var data = new ThicknessPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message,
                ThicknessLayerResults = new List<ThicknessLengthResult>()
            };

            if (measureSettings?.LayersToMeasure != null)
            {
                foreach (var layerToMeasure in measureSettings.LayersToMeasure)
                {
                    if (layerToMeasure != null)
                    {
                        if (layerToMeasure.IsWaferTotalThickness)
                        {
                            data.WaferThicknessResult = new ThicknessLengthResult
                            {
                                Name = layerToMeasure.Name,
                                Length = null,
                                State = MeasureState.NotMeasured,
                            };
                        }
                        else
                        {
                            data.ThicknessLayerResults.Add(new ThicknessLengthResult
                            {
                                Name = layerToMeasure.Name,
                                Length = null,
                                State = MeasureState.NotMeasured,
                            });
                        }
                    }
                }
            }

            return data;
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(ThicknessSettings measureSettings)
        {
            return _measureToolsForLayers.FindMeasureTools(measureSettings);
        }

        protected override MeasurePointDataResultBase Process(ThicknessSettings thicknessSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            Logger.Information("Start Thickness");

            var totalQuality = new List<double>();
            var resData = new ThicknessPointData
            {
                IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0,
                ThicknessLayerResults = new List<ThicknessLengthResult>()
            };

            var unifiedLayers = new UnifiedLayers(thicknessSettings);
            thicknessSettings.PhysicalLayers = unifiedLayers.PhysicalLayers;
            thicknessSettings.LayersToMeasure = unifiedLayers.LayersToMeasure;
            foreach (var layersToMeasure in unifiedLayers.LayersByProbeSettings)
            {
                var thicknessResult = MeasureLayerThickness(thicknessSettings, measureContext, cancelToken, layersToMeasure, resData);
                if (thicknessResult == null)
                {
                    return resData;
                }

                AddQuality(totalQuality, thicknessResult);
                FillResult(layersToMeasure, resData, thicknessResult, unifiedLayers.OffsetByLayerToMeasureName);
            }

            resData.QualityScore = totalQuality.Any() ? totalQuality.Average() : 0;
            return resData;
        }

        #region warp

        private class ThicknessPointForPlane
        {
            public ThicknessPointForPlane(Point3d pointForPlane, ThicknessPointData pointData)
            {
                PointForPlane = pointForPlane;
                PointData = pointData;
            }

            public Point3d PointForPlane { get; }
            public ThicknessPointData PointData { get; }
        }

        #endregion warp

        private static void AddQuality(List<double> totalQuality, ILiseMeasureResult thicknessResult)
        {
            if (thicknessResult != null)
            {
                totalQuality.AddRange(from layer in thicknessResult.LayersThickness
                    where layer.IsMandatory
                    select layer.Quality);
            }
        }

        private void FillResult(KeyValuePair<ProbeSettings, List<Layer>> layersToMeasure, ThicknessPointData resData, ILiseMeasureResult thicknessResult, Dictionary<string, Length> offsetByLayerToMeasureName)
        {
            var probe = HardwareUtils.GetProbeLiseFromID(HardwareManager, layersToMeasure.Key.ProbeId);
            var layerTotalThickness = layersToMeasure.Value.Find(layer => layer.IsWaferTotalThickness);
            if (IsIndependentLayerTotalThickness(layersToMeasure))
            {
                FillResultData(resData, thicknessResult, offsetByLayerToMeasureName, probe.Configuration, layerTotalThickness);
            }
            else
            {
                FillResultData(resData, thicknessResult, offsetByLayerToMeasureName, probe.Configuration);
                if (layerTotalThickness != null)
                {
                    if (layersToMeasure.Key.Equals(layerTotalThickness.ProbeSettings))
                    {
                        FillResultData(resData, thicknessResult, offsetByLayerToMeasureName, probe.Configuration, layerTotalThickness);
                    }
                }
            }
        }

        private ILiseMeasureResult MeasureLayerThickness(ThicknessSettings thicknessSettings, MeasureContext measureContext, CancellationToken cancelToken, KeyValuePair<ProbeSettings, List<Layer>> layersToMeasure, ThicknessPointData resData)
        {
            var probe = HardwareUtils.GetProbeLiseFromID(HardwareManager, layersToMeasure.Key.ProbeId);
            if (probe.Configuration is ProbeDualLiseConfig)
            {
                return ExecuteDualLiseThickness(thicknessSettings, measureContext, cancelToken, probe, layersToMeasure);
            }

            return ExecuteSingleLiseThickness(thicknessSettings, cancelToken, probe, layersToMeasure);
        }

        private ILiseMeasureResult ExecuteSingleLiseThickness(ThicknessSettings thicknessSettings, CancellationToken cancelToken, IProbeLise probe, KeyValuePair<ProbeSettings, List<Layer>> layersToMeasure)
        {
            ILiseMeasureResult thicknessResult;
            var config = probe.Configuration as ProbeLiseConfig;
            var settings = layersToMeasure.Key as SingleLiseSettings;

            CheckingThicknessLayers(thicknessSettings, config);

            var simpleLiseInput = new MeasureLiseInput(new ThicknessLiseInput(config.DeviceID, GetProbeSample(thicknessSettings.PhysicalLayers, layersToMeasure.Value, config), settings.LiseGain, _measureConfig.NbAveragingLise));
            simpleLiseInput.InitialContext = settings.ProbeObjectiveContext;
            thicknessResult = ExecuteLiseThicknessFlow(simpleLiseInput, cancelToken);

            if (config.ModulePosition == ModulePositions.Down)
            {
                thicknessResult.LayersThickness.Reverse();
            }

            return thicknessResult;
        }

        private ILiseMeasureResult ExecuteDualLiseThickness(ThicknessSettings thicknessSettings, MeasureContext measureContext,
            CancellationToken cancelToken, IProbeLise probe, KeyValuePair<ProbeSettings, List<Layer>> layersToMeasure)
        {
            var config = probe.Configuration as ProbeDualLiseConfig;
            var settings = layersToMeasure.Key as DualLiseSettings;
            var layers = layersToMeasure.Value;

            int unknownLayerIndex = FindUnknownLayerIndex(thicknessSettings, config);
            var layersUp = thicknessSettings.PhysicalLayers.GetRange(0, unknownLayerIndex);
            var layersDown = thicknessSettings.PhysicalLayers.GetRange(unknownLayerIndex + 1, thicknessSettings.PhysicalLayers.Count - layersUp.Count - 1);
            var unkonwnLayer = thicknessSettings.PhysicalLayers.ElementAt(unknownLayerIndex);

            var liseUpInput = new MeasureLiseInput(new ThicknessLiseInput(config.ProbeUpID, GetProbeSample(layersUp, layers, config), settings.LiseUp.LiseGain, _measureConfig.NbAveragingLise));
            var liseDownInput = new MeasureLiseInput(new ThicknessLiseInput(config.ProbeDownID, GetProbeSample(layersDown, layers, config), settings.LiseDown.LiseGain, _measureConfig.NbAveragingLise));
            var unknownLayerSample = new ProbeSampleLayer()
            {
                Name = unkonwnLayer.Name,
                Thickness = unkonwnLayer.Thickness,
                IsMandatory = true,
                RefractionIndex = (double)unkonwnLayer.RefractiveIndex,
                Tolerance = GetTolerance(unkonwnLayer, layersToMeasure.Value)
            };

            var probeLiseCalibResult = probe.CalibrationManager.GetCalibration(true, config.DeviceID,
                null, measureContext.MeasurePoint.Position, measureContext.DieIndex) as ProbeDualLiseCalibResult;

            var dualLiseInput = new MeasureDualLiseInput(config.DeviceID, liseUpInput, liseDownInput, unknownLayerSample, probeLiseCalibResult);
            dualLiseInput.InitialContext = settings.LiseUp.ProbeObjectiveContext;
            return ExecuteDualLiseThicknessFlow(dualLiseInput, cancelToken);
        }

        private static int FindUnknownLayerIndex(ThicknessSettings thicknessSettings, ProbeDualLiseConfig config)
        {
            int unknownLayerIndex = 0;
            bool layerWithUnknownRIIsFound = thicknessSettings.PhysicalLayers.Any(layer => layer.RefractiveIndex.IsNullOrNaN());

            bool layerWithSmallerRIIFound = thicknessSettings.PhysicalLayers.Any(layer => (layer.Thickness.Micrometers / layer.RefractiveIndex) < config.ThicknessThresholdInTheAir.Micrometers);

            if (layerWithUnknownRIIsFound)
            {
                unknownLayerIndex = thicknessSettings.PhysicalLayers.FindIndex(layer => layer.RefractiveIndex.IsNullOrNaN());
            }
            if (layerWithSmallerRIIFound)
            {
                unknownLayerIndex = thicknessSettings.PhysicalLayers.FindIndex(layer => (layer.Thickness.Micrometers / layer.RefractiveIndex) < 30);
            }

            return unknownLayerIndex;
        }

        private static void CheckingThicknessLayers(ThicknessSettings thicknessSettings, ProbeLiseConfig config)
        {
            for (int i = 0; i < thicknessSettings.PhysicalLayers.Count;)
            {
                var physicalLayers = thicknessSettings.PhysicalLayers;

                if ((physicalLayers[i].Thickness.Micrometers / physicalLayers[i].RefractiveIndex) <= config.ThicknessThresholdInTheAir.Micrometers)
                {
                    int nearestLayerIndex;
                    if (i > 0)
                    {
                        nearestLayerIndex = i - 1;
                    }
                    else
                    {
                        nearestLayerIndex = i + 1;
                    }

                    var totalThickness = physicalLayers[i].Thickness + physicalLayers[nearestLayerIndex].Thickness;

                    string tooThinPhysicalLayerName = physicalLayers[i].Name;
                    RemoveOfTheThinLayerForPhysicalLayers(physicalLayers, tooThinPhysicalLayerName);

                    string principalPhysicalLayerName = physicalLayers[nearestLayerIndex].Name;
                    UpdatePhysicalList(physicalLayers, principalPhysicalLayerName, totalThickness);
                }
                else
                {
                    i++;
                }
            }
        }

        private static void UpdatePhysicalList(List<LayerSettings> physicalLayers, string principalPhysicalLayerName, Length totalThickness)
        {
            int indexPhysicalLayerPrincipal = physicalLayers.FindIndex(x => x.Name == principalPhysicalLayerName);
            physicalLayers[indexPhysicalLayerPrincipal].Name = principalPhysicalLayerName;
            physicalLayers[indexPhysicalLayerPrincipal].Thickness = totalThickness;
        }

        private static void RemoveOfTheThinLayerForPhysicalLayers(List<LayerSettings> physicalLayers, string tooThinPhysicalLayerName)
        {
            int indexsPhysicalLayerTooThinToBeDeleted = physicalLayers.FindIndex(x => x.Name == tooThinPhysicalLayerName);
            physicalLayers.RemoveAt(indexsPhysicalLayerTooThinToBeDeleted);
        }

        private void ApplyMeasurePointCorrection(MeasurePointDataResultBase measurePointDataResult, MeasureSettingsBase measureSettingsBase)
        {
            var thicknessSettings = measureSettingsBase as ThicknessSettings;

            if (thicknessSettings is null)
                return;
            if (!(thicknessSettings.LayersToMeasure is null))
            {
                foreach (var layerToMeasure in thicknessSettings.LayersToMeasure)
                {
                    var probeCalibrationManager = HardwareManager.Probes[layerToMeasure.ProbeSettings.ProbeId].CalibrationManager;
                    if (probeCalibrationManager is null)
                        continue;
                    probeCalibrationManager.CorrectMeasurePoint(measurePointDataResult);
                }
            }

            if (thicknessSettings.HasWarpMeasure)
            {
                // The warp is alwayays measured with he dual lise
                var dualLiseProbe = HardwareManager.Probes.FirstOrDefault(probe => probe.Value.Configuration is ProbeDualLiseConfig).Value;
                if (!(dualLiseProbe is null))
                {
                    var probeCalibrationManager = dualLiseProbe.CalibrationManager;
                    if (!(probeCalibrationManager is null))
                        probeCalibrationManager.CorrectMeasurePoint(measurePointDataResult);
                }
            }
        }

        private static bool IsIndependentLayerTotalThickness(KeyValuePair<ProbeSettings, List<Layer>> layersToMeasure)
        {
            return layersToMeasure.Value.Count == 1 && layersToMeasure.Value[0].IsWaferTotalThickness;
        }

        private static void FillResultData(ThicknessPointData resData, ILiseMeasureResult thicknessResult, Dictionary<string, Length> offsetByLayerToMeasureName, IProbeConfig probeConfig, Layer waferTotalThickness = null)
        {
            if (thicknessResult.Status.State != FlowState.Success)
            {
                resData.State = MeasureState.NotMeasured;
                resData.Message = thicknessResult.Status.Message;
                return;
            }

            if (!(waferTotalThickness is null))
            {
                FillWaferTotalThickness(resData, thicknessResult, offsetByLayerToMeasureName, waferTotalThickness);
            }
            else
            {
                FillLayerThickness(resData, thicknessResult, offsetByLayerToMeasureName);
            }

            if (thicknessResult is MeasureLiseResult simpleLiseResult)
            {
                if (probeConfig.ModulePosition == ModulePositions.Up)
                    resData.AirGapUp = simpleLiseResult.AirGap;
                else
                    resData.AirGapDown = simpleLiseResult.AirGap;
            }
            else if (thicknessResult is MeasureDualLiseResult dualLiseResult)
            {
                resData.AirGapUp = dualLiseResult.AirGapUp;
                resData.AirGapDown = dualLiseResult.AirGapDown;
            }
            resData.Timestamp = thicknessResult.Timestamp;
        }

        private static void FillLayerThickness(ThicknessPointData resData, ILiseMeasureResult thicknessResult, Dictionary<string, Length> offsetByLayerToMeasureName)
        {
            foreach (var layer in thicknessResult.LayersThickness)
            {
                if (!layer.IsMandatory)
                {
                    continue;
                }

                if (!offsetByLayerToMeasureName.TryGetValue(layer.Name, out var offset)) offset = 0.Micrometers();
                var lengthResult = new ThicknessLengthResult
                {
                    Name = layer.Name,
                    Length = layer.Thickness + offset,
                    State = MeasureState.Success,
                };
                resData.ThicknessLayerResults.Add(lengthResult);
            }
        }

        private static void FillWaferTotalThickness(ThicknessPointData resData, ILiseMeasureResult thicknessResult, Dictionary<string, Length> offsetByLayerToMeasureName, Layer waferTotalThickness)
        {
            var totalThickness = 0.Micrometers();
            var totalOffset = 0.Micrometers();
            foreach (var layer in thicknessResult.LayersThickness)
            {
                foreach (string layerName in from string idOffset in offsetByLayerToMeasureName.Keys
                         where idOffset == layer.Name
                         select idOffset)
                {
                    offsetByLayerToMeasureName.TryGetValue(layerName, out var offset);
                    totalOffset += offset;
                }

                totalThickness += layer.Thickness;
            }

            totalThickness += totalOffset;
            resData.WaferThicknessResult = new ThicknessLengthResult
            {
                Name = waferTotalThickness.Name,
                Length = totalThickness,
                State = MeasureState.Success,
            };
        }

        private void GetProbeSampleCalibration(out double x, out double y, out ProbeSample probeCalibration)
        {
            var chuckDefinition = HardwareManager.Chuck.Configuration as ANAChuckConfig;
            var list = chuckDefinition.ReferencesList;
            Length refThickness = 0.Micrometers();
            Length refTolerence = 0.Micrometers();
            double refRefractionIndex = 0.0;
            x = 0.0;
            y = 0.0;
            string name = "";
            //TODO Which reference shim should be used?? Information given by the user?? 100, 750, 1500, ou ref cam?
            //TODO Don't leave it hard
            foreach (var reference in list)
            {
                if (reference.ReferenceName == "REF 750UM-sori")
                {
                    name = reference.ReferenceName;
                    refThickness = reference.RefThickness;
                    refRefractionIndex = reference.RefRefrIndex;
                    refTolerence = reference.RefTolerance;
                    x = reference.PositionX.Millimeters;
                    y = reference.PositionY.Millimeters;
                    break;
                }
            }

            var probeSampleLayerCalibration = new ProbeSampleLayer()
            {
                Name = name,
                Thickness = refThickness,
                RefractionIndex = refRefractionIndex,
                Tolerance = new LengthTolerance(refTolerence.Micrometers, LengthToleranceUnit.Micrometer),
                IsMandatory = true,
            };
            var probeSampleList = new List<ProbeSampleLayer>() { probeSampleLayerCalibration };
            probeCalibration = new ProbeSample()
            {
                Name = "",
                Info = "",
                Layers = probeSampleList,
            };
        }

        private void LiseThicknessFlow_StatusChanged(FlowStatus status, ILiseMeasureResult _)
        {
            NotifyMeasureProgressChanged(new MeasurePointProgress() { Message = $"[Thickness] ${status.State} - ${status.Message}" });
            if ((status.State == FlowState.Error) || (status.State == FlowState.Success) || (status.State == FlowState.Canceled))
                _liseFlowBeingExecuted.StatusChanged -= LiseThicknessFlow_StatusChanged;
        }

        private void DualLiseThicknessFlow_StatusChanged(FlowStatus status, ILiseMeasureResult _)
        {
            NotifyMeasureProgressChanged(new MeasurePointProgress() { Message = $"[Thickness] ${status.State} - ${status.Message}" });
            if ((status.State == FlowState.Error) || (status.State == FlowState.Success) || (status.State == FlowState.Canceled))
                _dualLiseFlowBeingExecuted.StatusChanged -= DualLiseThicknessFlow_StatusChanged;
        }

        private bool MustLayerBeMeasured(LayerSettings layer, List<Layer> layersToMeasures)
        {
            foreach (var layerToMeasure in layersToMeasures)
            {
                //In the case of a lonely wafer thickness we want all physicalLayers to be measured
                if (layerToMeasure.IsWaferTotalThickness)
                {
                    if (layersToMeasures.Count == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (layer.Name == layerToMeasure.PhysicalLayers[0].Name)
                {
                    return true;
                }
            }
            return false;
        }

        private ProbeSample GetProbeSample(List<LayerSettings> layers, List<Layer> layersToMeasure, IProbeConfig probeConfig)
        {
            var probeSampleLayersList = new List<ProbeSampleLayer>();
            foreach (var physicalLayer in layers)
            {
                bool isMeasure = MustLayerBeMeasured(physicalLayer, layersToMeasure);

                var tolerance = GetTolerance(physicalLayer, layersToMeasure);
                foreach (var layerToMeasure in layersToMeasure)
                {
                    if (layerToMeasure.IsWaferTotalThickness)
                    {
                        tolerance = layerToMeasure.ThicknessTolerance;
                    }
                }
                var thickness = physicalLayer.Thickness;

                var probeSampleLayer = new ProbeSampleLayer()
                {
                    Name = physicalLayer.Name,
                    Thickness = thickness,
                    IsMandatory = isMeasure,
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

        private LengthTolerance GetTolerance(LayerSettings physicalLayer, List<Layer> layersToMeasure)
        {
            var defaultTolerance = new LengthTolerance(50, LengthToleranceUnit.Micrometer);
            foreach (var layerToMeasure in layersToMeasure)
            {
                if (physicalLayer.Name == layerToMeasure.Name)
                {
                    return layerToMeasure.ThicknessTolerance;
                }
                if (layerToMeasure.PhysicalLayers.Count <= 0)
                {
                    return layerToMeasure.ThicknessTolerance;
                }
                if (physicalLayer.Name == layerToMeasure.PhysicalLayers[0].Name)
                {
                    return layerToMeasure.ThicknessTolerance;
                }
            }
            return defaultTolerance;
        }

        private MeasureLiseResult ExecuteLiseThicknessFlow(MeasureLiseInput input, CancellationToken cancelToken)
        {
            _liseFlowBeingExecuted = FlowsAreSimulated ? new LiseThicknessMeasurementFlowDummy(input) : new LiseThicknessMeasurementFlow(input);
            _liseFlowBeingExecuted.CancellationToken = cancelToken;
            _liseFlowBeingExecuted.StatusChanged += LiseThicknessFlow_StatusChanged;

            return _liseFlowBeingExecuted.Execute();
        }

        private MeasureDualLiseResult ExecuteDualLiseThicknessFlow(MeasureDualLiseInput input, CancellationToken cancelToken)
        {
            _dualLiseFlowBeingExecuted = FlowsAreSimulated ? new DualLiseThicknessMeasurementFlowDummy(input) : new DualLiseThicknessMeasurementFlow(input);
            _dualLiseFlowBeingExecuted.CancellationToken = cancelToken;
            _dualLiseFlowBeingExecuted.StatusChanged += DualLiseThicknessFlow_StatusChanged;

            return _dualLiseFlowBeingExecuted.Execute();
        }

        #region IMeasureDCProvider

        private class DieLayerThickness
        {
            public int LayerIndex { get; set; }
            public double Total { get; set; }
            public int Count { get; set; }

            public DieLayerThickness(int layerIndex = 0)
            {
                LayerIndex = layerIndex;
                Total = 0d;
                Count = 0;
            }

            public double GetMean()
            {
                return Count > 0 ? Total / Count : double.NaN;
            }
        }

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            var dcPointsMeasureData = new List<DCPointMeasureData>();
            var thicknessPointResult = measurePointResult as ThicknessPointResult;
            if (thicknessPointResult == null || thicknessPointResult.Datas == null)
            {
                return null;
            }

            var thicknessPointResultDataList = thicknessPointResult.Datas?.OfType<ThicknessPointData>().ToList();
            if ((thicknessPointResultDataList is null) || (thicknessPointResultDataList.Count == 0))
            {
                dcPointsMeasureData.Add(GetPointMeasureData(thicknessPointResult, null, siteId, dieRow, dieCol));
            }
            else
            {
                dcPointsMeasureData.AddRange(thicknessPointResultDataList.Select(thicknessData => GetPointMeasureData(thicknessPointResult, thicknessData, siteId, dieRow, dieCol)));
            }

            return dcPointsMeasureData;
        }

        private DCPointMeasureData GetPointMeasureData(ThicknessPointResult measurePointResult, ThicknessPointData thicknessData, int siteId, int? dieRow = null, int? dieCol = null)
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

            int iLayerMeasured = 1;
            thicknessData.ThicknessLayerResults.ForEach(thicknessLayerResult =>
            {
                var dcThickness = new DCDataDoubleWithDescription()
                { 
                    Name = $"Thickness {iLayerMeasured++}",
                    IsMeasured = thicknessData != null && (thicknessData.State != MeasureState.NotMeasured),
                    Value = thicknessLayerResult.Length?.Micrometers ?? double.NaN,
                    Unit = "um",
                    Description = thicknessLayerResult.Name
                };
                dcPointMeasureData.PointMeasuresData.Add(dcThickness);
            });

            if (thicknessData.WaferThicknessResult != null)
            {
                var dcTotalThickness = new DCDataDoubleWithDescription()
                {
                    Name = "Total Thickness",
                    IsMeasured = thicknessData.WaferThicknessResult.State != MeasureState.NotMeasured,
                    Value = thicknessData.WaferThicknessResult.Length.Micrometers,
                    Unit = "um",
                    Description = "Total Thickness"
                };
                dcPointMeasureData.PointMeasuresData.Add(dcTotalThickness);
            }

            return dcPointMeasureData;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var allDiesStatistics = new List<DCDieStatistics>();
            var thicknessResult = measureResult as ThicknessResult;
            if (thicknessResult is null || (thicknessResult.Dies is null) || (thicknessResult.Dies.Count == 0))
            {
                return null;
            }
            
            var thicknessSettings = measureSettings as ThicknessSettings;
            bool waferThicknessIsMeasured = thicknessSettings.LayersToMeasure?.Any(layer => layer.IsWaferTotalThickness) ?? false;

            foreach (var die in thicknessResult.Dies)
            {
                var dieStatistics = new DCDieStatistics() { RowIndex = die.RowIndex, ColumnIndex = die.ColumnIndex, DieStatistics = new List<DCData>() };
                var waferLayerThickness = new DieLayerThickness();
                var dieLayersThicknesses = new Dictionary<string, DieLayerThickness>();

                PrepareDieLayerThicknessStatistics(waferThicknessIsMeasured, die, waferLayerThickness, dieLayersThicknesses);
                ComputeDieLayerThicknessStatistics(waferThicknessIsMeasured, dieStatistics, waferLayerThickness, dieLayersThicknesses);

                allDiesStatistics.Add(dieStatistics);
            }

            return allDiesStatistics;
        }

        private static void PrepareDieLayerThicknessStatistics(bool waferThicknessIsMeasured, MeasureDieResult die, DieLayerThickness waferLayerThickness, Dictionary<string, DieLayerThickness> dieLayerThickness)
        {
            // Browse each point
            foreach (var point in die.Points.OfType<ThicknessPointResult>())
            {
                // Wafer thickness
                if (waferThicknessIsMeasured && point.WaferThicknessStat != null && point.WaferThicknessStat.Mean != null)
                {
                    waferLayerThickness.Total += point.WaferThicknessStat.Mean.Micrometers;
                    waferLayerThickness.Count++;
                }

                // Browse each layer stat to calculate total thickness
                int iLayer = 1;
                foreach (var thicknessLayerStat in point.ThicknessLayerStats)
                {
                    if (thicknessLayerStat.Value != null && thicknessLayerStat.Value.Mean != null)
                    {
                        if (!dieLayerThickness.ContainsKey(thicknessLayerStat.Key))
                        {
                            dieLayerThickness.Add(thicknessLayerStat.Key, new DieLayerThickness(iLayer++));
                        }

                        dieLayerThickness[thicknessLayerStat.Key].Total += thicknessLayerStat.Value.Mean.Micrometers;
                        dieLayerThickness[thicknessLayerStat.Key].Count++;
                    }
                }
            }
        }

        private static void ComputeDieLayerThicknessStatistics(bool waferThicknessIsMeasured, DCDieStatistics dieStatistics, DieLayerThickness waferLayerThickness, Dictionary<string, DieLayerThickness> dieLayersThicknesses)
        {
            foreach (var layerThickness in dieLayersThicknesses)
            {
                var dcMeanThickness = new DCDataDoubleWithDescription()
                {
                    Name = $"Thickness {layerThickness.Value.LayerIndex} Die Average",
                    IsMeasured = layerThickness.Value.Count > 0,
                    Value = layerThickness.Value.GetMean(),
                    Unit = "um",
                    Description = layerThickness.Key
                };
                dieStatistics.DieStatistics.Add(dcMeanThickness);
            }

            if (waferThicknessIsMeasured)
            {
                var dcMeanTotalThickness = new DCDataDoubleWithDescription()
                {
                    Name = "Total Thickness Die Average",
                    IsMeasured = waferLayerThickness.Count > 0,
                    Value = waferLayerThickness.GetMean(),
                    Unit = "um",
                    Description = "Total Thickness"
                };
                dieStatistics.DieStatistics.Add(dcMeanTotalThickness);
            }
        }

        public List<DCData> GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var waferStatistics = new List<DCData>();

            var thicknessResult = measureResult as ThicknessResult;
            if (thicknessResult is null)
            {
                return null;
            }

            int iLayerMeasured = 1;
            foreach (var thicknessLayerStat in thicknessResult.ThicknessLayerStats)
            {
                var dcMeanThickness = new DCDataDoubleWithDescription()
                {
                    Name = $"Thickness {iLayerMeasured++} Wafer Average",
                    IsMeasured = thicknessLayerStat.Value.State != MeasureState.NotMeasured,
                    Value = thicknessLayerStat.Value?.Mean?.Micrometers ?? double.NaN,
                    Unit = "um",
                    Description = thicknessLayerStat.Key
                };
                waferStatistics.Add(dcMeanThickness);
            }

            if (thicknessResult.WaferThicknessStat != null)
            {
                var dcMeanTotalThickness = new DCDataDoubleWithDescription()
                {
                    Name = "Total Thickness Wafer Average",
                    IsMeasured = thicknessResult.WaferThicknessStat.State != MeasureState.NotMeasured,
                    Value = thicknessResult.WaferThicknessStat?.Mean?.Micrometers ?? double.NaN,
                    Unit = "um",
                    Description = "Total Thickness"
                };
                waferStatistics.Add(dcMeanTotalThickness);
            }

            return waferStatistics;
        }

        #endregion IMeasureDCProvider
    }
}
