using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Proxy;
using UnitySC.PM.ANA.EP.Shared;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.AutofocusV2;
using UnitySC.PM.ANA.Service.Core.Context;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Core.VSI;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;


namespace UnitySC.PM.ANA.Service.Measure.Topography
{
    public class MeasureTopography : MeasureBase<TopographySettings, TopographyPointResult>, IMeasureDCProvider
    {
        public override MeasureType MeasureType => MeasureType.Topography;

        private readonly AnaHardwareManager _hardwareManager;
        private readonly MeasureTopoConfiguration _measureConfig;
        private TopographySettings _topographySettings;
        private MeasureContext _measureContext;
        private CancellationToken _cancelToken;
        private XYZTopZBottomPosition _startPosition;
        private XYZTopZBottomPosition _stopPosition;
        private int _stepCount;
        private readonly Length _stepSize;

        public MeasureTopography() : base(ClassLocator.Default.GetInstance<ILogger<MeasureTopography>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureTopoConfiguration>().SingleOrDefault();
            _stepSize = _measureConfig.VSIStepSize;
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var topographySettings = measureSettings as TopographySettings;
            var metroResult = new TopographyResult();

            metroResult.Settings.ExternalProcessingOutputs = new List<ExternalProcessingOutput>();
            if (topographySettings.PostProcessingSettings != null && topographySettings.PostProcessingSettings.IsEnabled)
            {
                foreach (var output in topographySettings.PostProcessingSettings.Outputs.Where(x => x.IsUsed))
                {
                    var resultOutput = new ExternalProcessingOutput();
                    resultOutput.Name = output.Name;
                    resultOutput.OutputTarget = output.Target;
                    resultOutput.OutputTolerance = new Tolerance() { Unit = ToleranceUnit.AbsoluteValue, Value = output.Tolerance };
                    metroResult.Settings.ExternalProcessingOutputs.Add(resultOutput);
                }
            }

            return metroResult;
        }

        public override List<string> GetLightIds()
        {
            // We need a white light, and the main light is supposed to be white.
            return HardwareManager.Lights.Where(x => x.Value.IsMainLight).Select(x => x.Value.DeviceID).ToList();
        }

        public override MeasurePointDataResultBase ExecutePostProcessing(MeasureSettingsBase measureSettings, MeasureContext measureContext, MeasurePointDataResultBase result)
        {
            try
            {
                var topographySettings = measureSettings as TopographySettings;

                if (result.State == MeasureState.NotMeasured)
                {
                    // VSI acquition have failed - no 3da has been produced nothing could be sent to external post processing
                    var topoDataNotMeasured = result as TopographyPointData;
                    if(topoDataNotMeasured.ExternalProcessingResults == null)
                        topoDataNotMeasured.ExternalProcessingResults = new List<ExternalProcessingResult>();

                    foreach (var outputMeasure in topographySettings.PostProcessingSettings.Outputs)
                    {
                        if ((outputMeasure.IsUsed))
                        {
                            var epResult = new ExternalProcessingResult();
                            epResult.Name = outputMeasure.Name;
                            epResult.Value = double.NaN;
                            epResult.Unit = outputMeasure.Unit;
                            epResult.State = MeasureState.NotMeasured;
                            topoDataNotMeasured.ExternalProcessingResults.Add(epResult);
                        }
                    }
                    return topoDataNotMeasured;
                }

                var topographyData = result as TopographyPointData;
                if (topographySettings.PostProcessingSettingsIsEnabled && !FlowsAreSimulated)
                {
                    topographyData.ExternalProcessingResults = new List<ExternalProcessingResult>();
                    var mountainsSupervisor = ClassLocator.Default.GetInstance<MountainsSupervisor>();
                    var mountainsExecutionParameter = new MountainsExecutionParameters();
                    mountainsExecutionParameter.PrintPDF = topographySettings.PostProcessingSettings.PdfIsSaved;
                    mountainsExecutionParameter.PointData = new PointData();
                    mountainsExecutionParameter.PointData.YCoordinate = measureContext.MeasurePoint.Position.Y;
                    mountainsExecutionParameter.PointData.XCoordinate = measureContext.MeasurePoint.Position.X;
                    mountainsExecutionParameter.PointData.PointNumber = measureContext.MeasurePoint.Id;
                    mountainsExecutionParameter.ResultFolderPath = Path.Combine(measureContext.ResultFoldersPath.RecipeFolderPath, measureContext.ResultFoldersPath.ExternalFileFolderName);
                    mountainsExecutionParameter.ResultFileName = Path.GetFileNameWithoutExtension(topographyData.ResultImageFileName);
                    var studiable = new ServiceImage();
                    string studiableFilePath = Path.Combine(measureContext.ResultFoldersPath.RecipeFolderPath, topographyData.ResultImageFileName);
                    studiable.Data = File.ReadAllBytes(studiableFilePath);
                    studiable.Type = ServiceImage.ImageType._3DA;
                    var mountainsResults = mountainsSupervisor.Execute(mountainsExecutionParameter, topographySettings.PostProcessingSettings.Template, studiable);

                    if (mountainsExecutionParameter.PrintPDF)
                    {
                        string pdfFileName = string.Concat(mountainsExecutionParameter.ResultFileName, ".pdf");
                        topographyData.ReportFileName = Path.Combine(measureContext.ResultFoldersPath.ExternalFileFolderName, pdfFileName);
                    }

                    foreach (var mountainRes in mountainsResults.Result)
                    {
                        var outputMeasure = topographySettings.PostProcessingSettings.Outputs.SingleOrDefault(x => x.Key == mountainRes.Description && x.IsUsed);
                        if (outputMeasure != null)
                        {
                            var mountainsValue = mountainRes.DoubleValue.Value;
                            var moutainResUnit = mountainRes.Unit;
                            switch (moutainResUnit.ToLower()) // handle "um" & "UM"
                            {
                                //"pm" : find out if unity map return some pm
                                case "um":
                                    moutainResUnit = "µm";
                                    break;
                                default: break;
                            }
                            if (moutainResUnit != outputMeasure.Unit)
                            {
                                LengthUnit resUnit, outputUnit;
                                bool resUnitFound = Length.TryGetLengthUnit(moutainResUnit, out resUnit);
                                bool outputUnitFound = Length.TryGetLengthUnit(outputMeasure.Unit, out outputUnit);
                                if (resUnitFound && outputUnitFound)
                                {
                                    Length resLength = new Length(mountainRes.DoubleValue.Value, resUnit);
                                    mountainsValue = resLength.ToUnit(outputUnit).Value;
                                }
                            }
                            var epResult = new ExternalProcessingResult();
                            epResult.Name = outputMeasure.Name;
                            epResult.Value = outputMeasure.Correction.ApplyCorrection(mountainsValue);
                            epResult.Unit = outputMeasure.Unit;
                            var tolerance = new Tolerance(outputMeasure.Tolerance, ToleranceUnit.AbsoluteValue);
                            bool inTolerance = tolerance.IsInTolerance(epResult.Value, outputMeasure.Target);
                            epResult.State = inTolerance ? MeasureState.Success : MeasureState.Error;
                            topographyData.ExternalProcessingResults.Add(epResult);
                        }
                    }

                    // Add the not measured ExternalProcessingResults
                    foreach (var outputMeasure in topographySettings.PostProcessingSettings.Outputs)
                    {
                        if ((outputMeasure.IsUsed) && (!topographyData.ExternalProcessingResults.Any(ep => ep.Name == outputMeasure.Name)))
                        {
                            var epResult = new ExternalProcessingResult();
                            epResult.Name = outputMeasure.Name;
                            epResult.Value = double.NaN;
                            epResult.Unit = outputMeasure.Unit;
                            epResult.State = MeasureState.NotMeasured;
                            topographyData.ExternalProcessingResults.Add(epResult);
                        }
                    }

                    // Set the global state of the measure
                    if (topographyData.ExternalProcessingResults.Any(ep => ep.State == MeasureState.NotMeasured))
                    {
                        if (topographyData.ExternalProcessingResults.Any(ep => ep.State == MeasureState.Success || ep.State == MeasureState.Error))
                        {
                            topographyData.State = MeasureState.Partial;
                            topographyData.Message = "PostProcessing partially failed";
                        }
                        else
                        {
                            topographyData.State = MeasureState.NotMeasured;
                            topographyData.Message = "PostProcessing failed";
                        }
                    }

                    return result;
                }
                else
                {
                    return base.ExecutePostProcessing(measureSettings, measureContext, result);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "PostProcessing");
                result.State = MeasureState.NotMeasured;
                result.Message = "PostProcessing error " + ex.Message;
                return result;
            }
        }

        protected override MeasurePointDataResultBase Process(TopographySettings measureSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            _topographySettings = measureSettings;
            _measureContext = measureContext;
            _cancelToken = cancelToken;

            var zFocusPositionLand = ExecuteLandedAutofocusCamera();
            ComputeStartStopPosition(zFocusPositionLand);
            ComputeStepCount();

            // PatternRec here was supposed to correct X/Y movement cause by Land/unLand.
            // It seems that simply restore position as it's done in ExecuteLandedAutofocusCamera
            // function works as well as patternRec for this case.
            //ExecutePatternRec(measureContext);

            var vsiResult = ExecuteVSI();
            var resData = CreateTopographyPointData(vsiResult);

            return resData;
        }

        protected override MeasureToolsBase GetMeasureToolsInternal(TopographySettings measureSettings)
        {
            TopographyMeasureTools tools = new TopographyMeasureTools();
            tools.CompatibleObjectives = HardwareManager.GetObjectiveConfigs()
                .Where(x => x.ObjType == Interface.ObjectiveConfig.ObjectiveType.INT)
                .Select(x => x.DeviceID).ToList();
            tools.PostProcessingIsAvailable = ClassLocator.Default.GetInstance<ExternalProcessingConfiguration>().Mountains != null;
            return tools;
        }

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(TopographySettings measureSettings, Exception ex)
        {
            var data = new TopographyPointData()
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message,
                ExternalProcessingResults = new List<ExternalProcessingResult>()
            };

            foreach (var outputMeasure in measureSettings.PostProcessingSettings.Outputs)
            {
                if ((outputMeasure.IsUsed))
                {
                    var epResult = new ExternalProcessingResult();
                    epResult.Name = outputMeasure.Name;
                    epResult.Value = double.NaN;
                    epResult.Unit = outputMeasure.Unit;
                    epResult.State = MeasureState.NotMeasured;
                    data.ExternalProcessingResults.Add(epResult);
                }
            }
            return data;
        }

        private double ExecuteLandedAutofocusCamera()
        {
            if (_topographySettings?.AutoFocusSettings?.ImageAutoFocusContext == null)
            {
                return double.NaN;
            }

            var afSettings = _topographySettings.AutoFocusSettings;

            // We can't simply change objective of afSettings.ImageAutoFocusContext or,
            // because it's a reference, we will definitly change objective use for the measure's autofocus.
            var contextManager = ClassLocator.Default.GetInstance<IContextManager>();
            var afImageContext = contextManager.GetCurrent<TopImageAcquisitionContext>();
            afImageContext.Lights = afSettings.ImageAutoFocusContext.Lights;
            afImageContext.TopObjectiveContext.ObjectiveId = _topographySettings.ObjectiveId;

            var afCameraInput = new AFCameraInput(afImageContext, afSettings.CameraId, afSettings.CameraScanRange, afSettings.CameraScanRangeConfigured);
            afCameraInput.UseCurrentZPosition = afSettings.UseCurrentZPosition;

            // We apply given autofocus context before call AFV2CameraFlow with a null context.
            // Then, we allow autofocus with landed stage.
            var contextApplier = ClassLocator.Default.GetInstance<ContextApplier<AFCameraInput>>();
            contextApplier.ApplyInitialContext(afCameraInput);
            afCameraInput.InitialContext = null;

            var xyPositionSaved = _hardwareManager.Axes.GetPos().ToXYPosition();
            try
            {
                _hardwareManager.Axes.Land();

                var afV2CameraFlow = FlowsAreSimulated ? new AFV2CameraFlowDummy(afCameraInput) : new AFV2CameraFlow(afCameraInput);
                afV2CameraFlow.CancellationToken = _cancelToken;

                var afRes = afV2CameraFlow.Execute();
                if (afRes?.Status?.State == FlowState.Success)
                {
                    return afRes.ZPosition;
                }
            }
            finally
            {
                _hardwareManager.Axes.StopLanding();
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, xyPositionSaved);
            }

            return double.NaN;
        }

        // cf. comment above, in process() function
        /*private void ExecutePatternRec()
        {
            if (!(_measureContext.MeasurePoint.PatternRec is null))
            {
                var patternRec = _measureContext.MeasurePoint.PatternRec;
                PatternRecInput patternRecInput = new PatternRecInput(patternRec);

                patternRecInput.InitialContext = patternRec.Context;

                PatternRecFlow patternRecFlow = FlowsAreSimulated ? new PatternRecFlowDummy(patternRecInput) : new PatternRecFlow(patternRecInput);
                patternRecFlow.CancellationToken = _cancelToken;
                var patternRecRes = patternRecFlow.Execute();

                if (patternRecRes.Status.State == FlowState.Success)
                {
                    // Apply patternRec shift found
                    var patternRecShift = new XYZTopZBottomMove(patternRecRes.ShiftX.Millimeters, patternRecRes.ShiftY.Millimeters, 0, 0);
                    HardwareUtils.MoveIncremental(HardwareManager.Axes, patternRecShift);
                }
            }
        }*/

        private void ComputeStartStopPosition(double zStartPosition)
        {
            if (!double.IsNaN(zStartPosition))
            {
                _startPosition = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, zStartPosition, double.NaN);
            }
            else
            {
                _startPosition = new XYZTopZBottomPosition(new WaferReferential(), double.NaN, double.NaN, _measureContext.MeasurePoint.Position.ZTop, double.NaN);
            }
            _stopPosition = (XYZTopZBottomPosition)_startPosition.Clone();

            var surfaceInFocus = _topographySettings.SurfacesInFocus;
            var HeightVariation = _topographySettings.HeightVariation;
            var ScanMargin = _topographySettings.ScanMargin;

            if (surfaceInFocus == SurfacesInFocus.Top)
            {
                _startPosition.ZTop -= HeightVariation.Millimeters + ScanMargin.Millimeters + _measureConfig.VSIMarginConstant.Millimeters;
                _stopPosition.ZTop += ScanMargin.Millimeters + _measureConfig.VSIMarginConstant.Millimeters;
            }
            else if (surfaceInFocus == SurfacesInFocus.Bottom)
            {
                _startPosition.ZTop -= ScanMargin.Millimeters + _measureConfig.VSIMarginConstant.Millimeters;
                _stopPosition.ZTop += HeightVariation.Millimeters + ScanMargin.Millimeters + _measureConfig.VSIMarginConstant.Millimeters;
            }
            else if (surfaceInFocus == SurfacesInFocus.Unknown)
            {
                _startPosition.ZTop -= HeightVariation.Millimeters + ScanMargin.Millimeters + _measureConfig.VSIMarginConstant.Millimeters;
                _stopPosition.ZTop += HeightVariation.Millimeters + ScanMargin.Millimeters + _measureConfig.VSIMarginConstant.Millimeters;
            }

            CheckScanRangeAmplitude();
        }

        private void ComputeStepCount()
        {
            _stepCount = (int)(Math.Abs(_stopPosition.ZTop - _startPosition.ZTop) / _stepSize.Millimeters);
        }

        private VSIResult ExecuteVSI()
        {
            var input = new VSIInput()
            {
                CameraId = _topographySettings.CameraId,
                ObjectiveId = _topographySettings.ObjectiveId,
                StartPosition = _startPosition,
                StepSize = _stepSize,
                StepCount = _stepCount,
                ROI = _topographySettings.ROI,
                InitialContext = _topographySettings.MeasureContext,
            };

            VSIFlow vsiFlow = FlowsAreSimulated ? new VSIFlowDummy(input) : new VSIFlow(input);
            vsiFlow.CancellationToken = _cancelToken;

            vsiFlow.StatusChanged += VsiFlow_StatusChanged;
            var vsiResult = vsiFlow.Execute();
            vsiFlow.StatusChanged -= VsiFlow_StatusChanged;

            return vsiResult;
        }

        private TopographyPointData CreateTopographyPointData(VSIResult vsiResult)
        {
            var resData = new TopographyPointData();
            resData.IndexRepeta = (_measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
            resData.ExternalProcessingResults = new List<ExternalProcessingResult>();
            if (vsiResult.Status.State == FlowState.Success)
            {
                resData.State = MeasureState.Success;
                resData.QualityScore = vsiResult.QualityScore;
                resData.ResultImageFileName = SaveServiceImage(vsiResult.TopographyImage, "Topo.3da", _measureContext);
            }
            else
            {
                resData.State = MeasureState.NotMeasured;
                resData.Message = vsiResult.Status.Message;
            }

            return resData;
        }

        private void VsiFlow_StatusChanged(FlowStatus status, VSIResult _)
        {
            NotifyMeasureProgressChanged(new MeasurePointProgress() { Message = $"[Topo] ${status.State} - ${status.Message}" });
        }

        private void CheckScanRangeAmplitude()
        {
            if (!FlowsAreSimulated)
            {
                var piezoController = _hardwareManager.GetPiezoController(_topographySettings.ObjectiveId);
                var piezoAxis = piezoController?.AxesList.First(axis => axis.AxisID == _hardwareManager.GetPiezoAxisID(_topographySettings.ObjectiveId));
                var maxPiezoAmplitude = Math.Abs(piezoAxis.AxisConfiguration.PositionMax.Millimeters - piezoAxis.AxisConfiguration.PositionMin.Millimeters);
                var scanRangeAmplitude = Math.Abs(_startPosition.ZTop - _stopPosition.ZTop);

                if (scanRangeAmplitude > maxPiezoAmplitude)
                {
                    throw new Exception($"VSI scan range is too large : {scanRangeAmplitude}µm > {maxPiezoAmplitude}µm");
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
            var topographyPointResult = measurePointResult as TopographyPointResult;

            if (topographyPointResult is null)
                return null;
            var topographyPointResultDataList = topographyPointResult.TopographyDatas;
            if ((topographyPointResultDataList is null) || (topographyPointResultDataList.Count == 0))
            {
                dcPointsMeasureData.Add(GetPointMeasureData(topographyPointResult, null, siteId, dieRow, dieCol));
            }
            else
            {
                dcPointsMeasureData.AddRange(topographyPointResultDataList.Select(topographyData => GetPointMeasureData(topographyPointResult, topographyData, siteId, dieRow, dieCol)));
            }
            return dcPointsMeasureData;
        }

        private DCPointMeasureData GetPointMeasureData(TopographyPointResult measurePointResult, TopographyPointData topographyData, int siteId, int? dieRow = null, int? dieCol = null)
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

            if (measurePointResult.TopographyDatas is null)
            {
                return dcPointMeasureData;
            }

            foreach (var externalProcessingResult in topographyData.ExternalProcessingResults)
            {
                var dcExtProc = new DCDataDouble() { Name = externalProcessingResult.Name, IsMeasured = (externalProcessingResult.State != MeasureState.NotMeasured), Value = externalProcessingResult.Value, Unit = externalProcessingResult.Unit.Replace('µ', 'u') };
                dcPointMeasureData.PointMeasuresData.Add(dcExtProc);
            }

            return dcPointMeasureData;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var topographyResult = measureResult as TopographyResult;
            if (topographyResult is null)
                return null;

            if ((topographyResult.Dies is null) || (topographyResult.Dies.Count == 0))
                return null;
            var allDiesStatistics = new List<DCDieStatistics>();
            foreach (var die in topographyResult.Dies)
            {
                var dieStatistics = new DCDieStatistics() { RowIndex = die.RowIndex, ColumnIndex = die.ColumnIndex, DieStatistics = new List<DCData>() };
                Dictionary<string, DieStatExternalProcessing> dieStatsExternalProcessing = new Dictionary<string, DieStatExternalProcessing>();

                foreach (TopographyPointResult pointResult in die.Points.OfType<TopographyPointResult>().ToList())
                {
                    foreach (var externProcStats in pointResult.ExternalProcessingStats)
                    {
                        if (!dieStatsExternalProcessing.ContainsKey(externProcStats.Key))
                        {
                            dieStatsExternalProcessing.Add(externProcStats.Key, new DieStatExternalProcessing());
                            dieStatsExternalProcessing[externProcStats.Key].Unit = externProcStats.Value.Unit;
                        }
                        if ((externProcStats.Value?.Mean != null) && (!Double.IsNaN(externProcStats.Value.Mean)))
                        {
                            dieStatsExternalProcessing[externProcStats.Key].Total += externProcStats.Value.Mean;
                            dieStatsExternalProcessing[externProcStats.Key].NbItems++;
                        }
                    }
                }

                foreach (var dieStatsExtProc in dieStatsExternalProcessing)
                {
                    var dcMean = new DCDataDouble() { Name = $"{dieStatsExtProc.Key} Die Average", IsMeasured = (dieStatsExtProc.Value.NbItems > 0), Value = dieStatsExtProc.Value.Mean, Unit = dieStatsExtProc.Value.Unit.Replace('µ', 'u') };
                    dieStatistics.DieStatistics.Add(dcMean);
                }

                allDiesStatistics.Add(dieStatistics);
            }
            return allDiesStatistics;
        }

        List<DCData> IMeasureDCProvider.GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var waferStatistics = new List<DCData>();

            var topographyResult = measureResult as TopographyResult;
            if (topographyResult is null)
                return waferStatistics;

            foreach (var outputStat in topographyResult.ExternalOutputStats)
            {
                var dcMeanOutput = new DCDataDouble() { Name = outputStat.Key + " Wafer Average", IsMeasured = (!(outputStat.Value?.Mean is null)) && (!Double.IsNaN(outputStat.Value.Mean)), Value = outputStat.Value?.Mean ?? double.NaN, Unit = outputStat.Value.Unit.Replace('µ', 'u') };
                waferStatistics.Add(dcMeanOutput);
            }
            return waferStatistics;
        }

        #endregion IMeasureDCProvider
    }
}
