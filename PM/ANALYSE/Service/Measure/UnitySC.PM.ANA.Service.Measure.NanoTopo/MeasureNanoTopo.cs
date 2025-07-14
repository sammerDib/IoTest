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
using UnitySC.PM.ANA.Service.Core.PSI;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Image;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Measure.NanoTopo
{
    public class MeasureNanoTopo : MeasureBase<NanoTopoSettings, NanoTopoPointResult>, IMeasureDCProvider
    {
        public override MeasureType MeasureType => MeasureType.NanoTopo;

        private readonly AnaHardwareManager _hardwareManager;
        private readonly MeasureNanoTopoConfiguration _measureConfig;
        private CancellationToken _cancelToken;
        private NanoTopoSettings _nanoTopoSettings;

        public MeasureNanoTopo() : base(ClassLocator.Default.GetInstance<ILogger<MeasureNanoTopo>>())
        {
            _measureConfig = MeasuresConfiguration?.Measures.OfType<MeasureNanoTopoConfiguration>().SingleOrDefault();
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
            if (_measureConfig is null)
                throw new Exception("NanoTopo measure configuration is missing");
        }

        public override MeasureResultBase CreateMetroMeasureResult(MeasureSettingsBase measureSettings)
        {
            var nanoTopoSettings = measureSettings as NanoTopoSettings;
            var metroResult = new NanoTopoResult();
            metroResult.Settings.ExternalProcessingOutputs = new List<ExternalProcessingOutput>();
            if (nanoTopoSettings.PostProcessingSettings != null && nanoTopoSettings.PostProcessingSettings.IsEnabled)
            {
                foreach (var output in nanoTopoSettings.PostProcessingSettings.Outputs.Where(x => x.IsUsed))
                {
                    var resultOutput = new ExternalProcessingOutput();
                    resultOutput.Name = output.Name;
                    resultOutput.OutputTarget = output.Target;
                    resultOutput.OutputTolerance = new Tolerance() { Unit = ToleranceUnit.AbsoluteValue, Value = output.Tolerance };
                    metroResult.Settings.ExternalProcessingOutputs.Add(resultOutput);
                }
            }
            metroResult.Settings.StepHeightTarget = nanoTopoSettings.StepHeightTarget;
            metroResult.Settings.StepHeightTolerance = nanoTopoSettings.StepHeightTolerance;
            metroResult.Settings.RoughnessTarget = nanoTopoSettings.RoughnessTarget;
            metroResult.Settings.RoughnessTolerance = nanoTopoSettings.RoughnessTolerance;

            return metroResult;
        }

        public override List<string> GetLightIds()
        {
            return HardwareManager.Lights.Where(x => x.Value.Config.Wavelength != null
            && x.Value.Config.Wavelength <= _measureConfig.MaxCompatibleLightWavelength
            && x.Value.Config.Wavelength >= _measureConfig.MinCompatibleLightWavelength).Select(x => x.Value.DeviceID).ToList();
        }

        public override MeasurePointDataResultBase ExecutePostProcessing(MeasureSettingsBase measureSettings, MeasureContext measureContext, MeasurePointDataResultBase result)
        {
            try
            {
                var nanoTopoSettings = measureSettings as NanoTopoSettings;

                if (result.State == MeasureState.NotMeasured)
                {
                    // PSI acquition have failed - no 3da has been produced nothing could be sent to external post processing
                    var nanotopoDataNotMeasured = result as NanoTopoPointData;
                    if (nanotopoDataNotMeasured.ExternalProcessingResults == null)
                        nanotopoDataNotMeasured.ExternalProcessingResults = new List<ExternalProcessingResult>();

                    foreach (var outputMeasure in nanoTopoSettings.PostProcessingSettings.Outputs)
                    {
                        if ((outputMeasure.IsUsed))
                        {
                            var epResult = new ExternalProcessingResult();
                            epResult.Name = outputMeasure.Name;
                            epResult.Value = double.NaN;
                            epResult.Unit = outputMeasure.Unit;
                            epResult.State = MeasureState.NotMeasured;
                            nanotopoDataNotMeasured.ExternalProcessingResults.Add(epResult);
                        }
                    }
                    return nanotopoDataNotMeasured;
                }

                var nanoData = result as NanoTopoPointData;
                if (nanoTopoSettings.PostProcessingSettingsIsEnabled && !FlowsAreSimulated)
                {
                    nanoData.ExternalProcessingResults = new List<ExternalProcessingResult>();
                    var mountainsSupervisor = ClassLocator.Default.GetInstance<MountainsSupervisor>();
                    var mountainsExecutionParameter = new MountainsExecutionParameters();
                    mountainsExecutionParameter.PrintPDF = nanoTopoSettings.PostProcessingSettings.PdfIsSaved;
                    mountainsExecutionParameter.PointData = new PointData();
                    mountainsExecutionParameter.PointData.YCoordinate = measureContext.MeasurePoint.Position.Y;
                    mountainsExecutionParameter.PointData.XCoordinate = measureContext.MeasurePoint.Position.X;
                    mountainsExecutionParameter.PointData.PointNumber = measureContext.MeasurePoint.Id;
                    mountainsExecutionParameter.ResultFolderPath = Path.Combine(measureContext.ResultFoldersPath.RecipeFolderPath, measureContext.ResultFoldersPath.ExternalFileFolderName);
                    mountainsExecutionParameter.ResultFileName = Path.GetFileNameWithoutExtension(nanoData.ResultImageFileName);

                    var studiable = new ServiceImage();
                    string studiableFilePath = Path.Combine(measureContext.ResultFoldersPath.RecipeFolderPath, nanoData.ResultImageFileName);
                    studiable.Data = File.ReadAllBytes(studiableFilePath);
                    studiable.Type = ServiceImage.ImageType._3DA;
                    var mountainsResults = mountainsSupervisor.Execute(mountainsExecutionParameter, nanoTopoSettings.PostProcessingSettings.Template, studiable);

                    if (mountainsExecutionParameter.PrintPDF)
                    {
                        string pdfFileName = string.Concat(mountainsExecutionParameter.ResultFileName, ".pdf");
                        nanoData.ReportFileName = Path.Combine(measureContext.ResultFoldersPath.ExternalFileFolderName, pdfFileName);
                    }

                    foreach (var mountainRes in mountainsResults.Result)
                    {
                        var outputMeasure = nanoTopoSettings.PostProcessingSettings.Outputs.SingleOrDefault(x => x.Key == mountainRes.Description && x.IsUsed);
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
                            var epresult = new ExternalProcessingResult();
                            epresult.Name = outputMeasure.Name;
                            epresult.Value = mountainsValue;
                            epresult.Unit = outputMeasure.Unit;
                            var tolerance = new Tolerance(outputMeasure.Tolerance, ToleranceUnit.AbsoluteValue);
                            bool inTolerance = tolerance.IsInTolerance(epresult.Value, outputMeasure.Target);
                            epresult.State = inTolerance ? MeasureState.Success : MeasureState.Error;
                            nanoData.ExternalProcessingResults.Add(epresult);
                        }
                    }

                    // Add the not measured ExternalProcessingResults
                    foreach (var outputMeasure in nanoTopoSettings.PostProcessingSettings.Outputs)
                    {
                        if ((outputMeasure.IsUsed) && (!nanoData.ExternalProcessingResults.Any(ep => ep.Name == outputMeasure.Name)))
                        {
                            var epResult = new ExternalProcessingResult();
                            epResult.Name = outputMeasure.Name;
                            epResult.Value = double.NaN;
                            epResult.Unit = outputMeasure.Unit;
                            epResult.State = MeasureState.NotMeasured;
                            nanoData.ExternalProcessingResults.Add(epResult);
                        }
                    }

                    // Set the global state of the measure
                    if (nanoData.ExternalProcessingResults.Any(ep => ep.State == MeasureState.NotMeasured))
                    {
                        if (nanoData.ExternalProcessingResults.Any(ep => ep.State == MeasureState.Success || ep.State == MeasureState.Error))
                        {
                            nanoData.State = MeasureState.Partial;
                            nanoData.Message = "PostProcessing partially failed";
                        }
                        else
                        {
                            nanoData.State = MeasureState.NotMeasured;
                            nanoData.Message = "PostProcessing failed";
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

        protected override MeasurePointDataResultBase CreateNotMeasuredPointData(NanoTopoSettings measureSettings, Exception ex)
        {
            var data = new NanoTopoPointData
            {
                State = MeasureState.NotMeasured,
                Message = ex.Message,
                ExternalProcessingResults = new List<ExternalProcessingResult>(),
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

        protected override MeasureToolsBase GetMeasureToolsInternal(NanoTopoSettings measureSettings)
        {
            NanoTopoMeasureTools tools = new NanoTopoMeasureTools();
            tools.CompatibleObjectives = HardwareManager.GetObjectiveConfigs()
                .Where(x => x.ObjType == Interface.ObjectiveConfig.ObjectiveType.INT)
                .Select(x => x.DeviceID).ToList();

            tools.OrderedAlgoNames = _measureConfig.Algos.OrderBy(x => x.Name).Select(x => x.Name).ToList();
            tools.PostProcessingIsAvailable = ClassLocator.Default.GetInstance<ExternalProcessingConfiguration>().Mountains != null;
            return tools;
        }

        protected override MeasurePointDataResultBase Process(NanoTopoSettings nanoTopoSettings, MeasureContext measureContext, CancellationToken cancelToken)
        {
            _cancelToken = cancelToken;
            _nanoTopoSettings = nanoTopoSettings;
            ExecuteLandedAutofocusCamera();

            var psiResult = ExecutePSI();
            var resData = CreateNanoTopoPointData(nanoTopoSettings, measureContext, psiResult);
            return resData;
        }

        private void ExecuteLandedAutofocusCamera()
        {
            if (_nanoTopoSettings?.AutoFocusSettings?.ImageAutoFocusContext == null)
            {
                return;
            }

            var afSettings = _nanoTopoSettings.AutoFocusSettings;

            // We can't simply change objective of afSettings.ImageAutoFocusContext or,
            // because it's a reference, we will definitly change objective use for the measure's autofocus.
            var contextManager = ClassLocator.Default.GetInstance<IContextManager>();
            var afImageContext = contextManager.GetCurrent<TopImageAcquisitionContext>();
            afImageContext.Lights = afSettings.ImageAutoFocusContext.Lights;
            afImageContext.TopObjectiveContext.ObjectiveId = _nanoTopoSettings.ObjectiveId;

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

                afV2CameraFlow.Execute();
            }
            finally
            {
                _hardwareManager.Axes.StopLanding();
                HardwareUtils.MoveAxesTo(_hardwareManager.Axes, xyPositionSaved);
            }
        }

        private PSIResult ExecutePSI()
        {
            var input = CreatePSIInput();

            PSIFlow psiFlow = FlowsAreSimulated ? new PSIFlowDummy(input) : new PSIFlow(input);
            psiFlow.CancellationToken = _cancelToken;

            psiFlow.StatusChanged += PsiFlow_StatusChanged;
            var psiResult = psiFlow.Execute();
            psiFlow.StatusChanged -= PsiFlow_StatusChanged;

            return psiResult;
        }

        private static NanoTopoPointData CreateNanoTopoPointData(NanoTopoSettings nanoTopoSettings, MeasureContext measureContext, PSIResult psiResult)
        {
            var resData = new NanoTopoPointData();
            resData.IndexRepeta = (measureContext as MeasureContextRepeat)?.RepeatIndex ?? 0;
            resData.ExternalProcessingResults = new List<ExternalProcessingResult>();
            if (psiResult.Status.State == FlowState.Success)
            {
                resData.State = MeasureState.Success;
                resData.QualityScore = psiResult.QualityScore;
                resData.StepHeight = (nanoTopoSettings.StepHeightTarget != null) ? psiResult.StepHeight : null;
                resData.Roughness = (nanoTopoSettings.RoughnessTarget != null) ? psiResult.Roughness : null;
                resData.ResultImageFileName = SaveServiceImage(psiResult.NanoTopographyImage, "NanoTopo.3da", measureContext);
            }
            else
            {
                resData.State = MeasureState.NotMeasured;
                resData.Message = psiResult.Status.Message;
            }

            return resData;
        }

        private void PsiFlow_StatusChanged(FlowStatus status, PSIResult _)
        {
            NotifyMeasureProgressChanged(new MeasurePointProgress() { Message = $"[NanoTopo] ${status.State} - ${status.Message}" });
        }

        private PSIInput CreatePSIInput()
        {
            var algo = _measureConfig.Algos.FirstOrDefault(x => x.Name == _nanoTopoSettings.AlgoName);
            if (algo is null)
                throw new InvalidOperationException($"{_nanoTopoSettings.AlgoName} Algo defined in measure settings doesn't exist in MeasureNanoTopoConfiguration");

            var light = HardwareManager.Lights.FirstOrDefault(x => x.Value.DeviceID == _nanoTopoSettings.LightId);
            if (light.Value is null)
                throw new InvalidOperationException($" Light {_nanoTopoSettings.LightId} is not defined in hardware configuration");

            var wavelength = light.Value.Config?.Wavelength;
            if (wavelength is null)
                throw new InvalidOperationException($"Wavelength for light {_nanoTopoSettings.LightId} is not defined in hardware configuration");

            var acquisition = _measureConfig.Acquisitions.FirstOrDefault(x => x.Resolution == _nanoTopoSettings.Resolution);
            if (acquisition is null)
                throw new InvalidOperationException($"{_nanoTopoSettings.Resolution} acquisition resolution defined in measure settings doesn't exist in MeasureNanoTopoConfiguration");

            Length step = algo.FactorBetweenWavelengthAndStepSize * wavelength;
            var psiInitialContext = _nanoTopoSettings.MeasureContext;

            var input = new PSIInput(
                psiInitialContext,
                _nanoTopoSettings.ObjectiveId,
                _nanoTopoSettings.CameraId,
                step,
                algo.StepCount,
                acquisition.ImagesPerStep,
                _nanoTopoSettings.ROI,
                algo.PhaseCalculation,
                algo.PhaseUnwrapping,
                wavelength);

            return input;
        }

        #region IMeasureDCProvider

        public List<DCPointMeasureData> GetDCResultBase(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            return null;
        }

        public List<DCPointMeasureData> GetDCResult(MeasurePointResult measurePointResult, MeasureSettingsBase measureSettings, int siteId, int? dieRow = null, int? dieCol = null)
        {
            var dcPointsMeasureData = new List<DCPointMeasureData>();

            if (!(measurePointResult is NanoTopoPointResult nanoTopoPointResult))
            {
                return null;
            }

            var nanoTopoPointResultDataList = nanoTopoPointResult.Datas?.OfType<NanoTopoPointData>().ToList();
            if ((nanoTopoPointResultDataList is null) || (nanoTopoPointResultDataList.Count == 0))
            {
                dcPointsMeasureData.Add(GetPointMeasureData(nanoTopoPointResult, null, siteId, dieRow, dieCol));
            }
            else
            {
                dcPointsMeasureData.AddRange(nanoTopoPointResultDataList.Select(nanoTopoData => GetPointMeasureData(nanoTopoPointResult, nanoTopoData, siteId, dieRow, dieCol)));
            }

            return dcPointsMeasureData;
        }

        private DCPointMeasureData GetPointMeasureData(NanoTopoPointResult nanoTopoPointResult, NanoTopoPointData nanoTopoData, int siteId, int? dieRow = null, int? dieCol = null)
        {
            var dcPointMeasureData = new DCPointMeasureData
            {
                CoordinateX = nanoTopoPointResult.WaferRelativeXPosition.Millimeters().Micrometers,
                CoordinateY = nanoTopoPointResult.WaferRelativeYPosition.Millimeters().Micrometers,
                PointMeasuresData = new List<DCData>(),
                DieColumnIndex = dieCol ?? 0,
                DieRowIndex = dieRow ?? 0,
                SiteId = siteId
            };

            if (nanoTopoPointResult.Datas is null)
            {
                return dcPointMeasureData;
            }

            foreach (var externalProcessingResult in nanoTopoData.ExternalProcessingResults)
            {
                var dcExtProc = new DCDataDouble()
                { 
                    Name = externalProcessingResult.Name, 
                    IsMeasured = externalProcessingResult.State != MeasureState.NotMeasured,
                    Value = externalProcessingResult.Value,
                    Unit = externalProcessingResult.Unit.Replace('µ', 'u')
                };
                dcPointMeasureData.PointMeasuresData.Add(dcExtProc);
            }

            return dcPointMeasureData;
        }

        public List<DCDieStatistics> GetDCDiesStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            if (!(measureResult is NanoTopoResult nanoTopoResult))
            {
                return null;
            }

            if ((nanoTopoResult.Dies is null) || (nanoTopoResult.Dies.Count == 0))
            {
                return null;
            }

            var allDiesStatistics = new List<DCDieStatistics>();
            foreach (var die in nanoTopoResult.Dies)
            {
                var dieStatistics = new DCDieStatistics()
                { 
                    RowIndex = die.RowIndex, 
                    ColumnIndex = die.ColumnIndex, 
                    DieStatistics = new List<DCData>()
                };
                var dieStatsExternalProcessing = new Dictionary<string, DieStatExternalProcessing>();

                foreach (var pointResult in die.Points.OfType<NanoTopoPointResult>().ToList())
                {
                    foreach (var externProcStats in pointResult.ExternalProcessingStats)
                    {
                        if (!dieStatsExternalProcessing.ContainsKey(externProcStats.Key))
                        {
                            dieStatsExternalProcessing.Add(externProcStats.Key, new DieStatExternalProcessing());
                            dieStatsExternalProcessing[externProcStats.Key].Unit = externProcStats.Value.Unit;
                        }
                        if ((externProcStats.Value?.Mean != null) && (!double.IsNaN(externProcStats.Value.Mean)))
                        {
                            dieStatsExternalProcessing[externProcStats.Key].Total += externProcStats.Value.Mean;
                            dieStatsExternalProcessing[externProcStats.Key].NbItems++;
                        }
                    }
                }

                foreach (var dieStatsExtProc in dieStatsExternalProcessing)
                {
                    var dcMean = new DCDataDouble()
                    { 
                        Name = $"{dieStatsExtProc.Key} Die Average",
                        IsMeasured = dieStatsExtProc.Value.NbItems > 0,
                        Value = dieStatsExtProc.Value.Mean,
                        Unit = dieStatsExtProc.Value.Unit.Replace('µ', 'u')
                    };
                    dieStatistics.DieStatistics.Add(dcMean);
                }

                allDiesStatistics.Add(dieStatistics);
            }

            return allDiesStatistics;
        }

        List<DCData> IMeasureDCProvider.GetDCWaferStatistics(MeasureResultBase measureResult, MeasureSettingsBase measureSettings)
        {
            var waferStatistics = new List<DCData>();

            if (measureResult is NanoTopoResult nanoTopoResult)
            {
                foreach (var outputStat in nanoTopoResult.ExternalOutputStats)
                {
                    var dcMeanOutput = new DCDataDouble()
                    { 
                        Name = outputStat.Key + " Wafer Average",
                        IsMeasured = (!(outputStat.Value?.Mean is null)) && (!double.IsNaN(outputStat.Value.Mean)),
                        Value = outputStat.Value?.Mean ?? double.NaN,
                        Unit = outputStat.Value.Unit.Replace('µ', 'u')
                    };
                    waferStatistics.Add(dcMeanOutput);
                }
            }

            return waferStatistics;
        }

        #endregion IMeasureDCProvider
    }
}
