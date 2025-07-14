using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.CalibFlow;
using UnitySC.PM.ANA.Service.Core.Calibration;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Format.XYCalibration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.PM.ANA.Service.Core.CalibFlow.ProbeSpotPosition;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.Tools.Collection;
using UnitySC.PM.ANA.Service.Core.Shared;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class CalibrationService : DuplexServiceBase<ICalibrationServiceCallback>, ICalibrationService
    {
        private AnaHardwareManager _anaHardwareManager;
        private CalibrationManager _calibrationManager;
        private IGlobalStatusServer _globalStatusServer;
        private MeasuresConfiguration _measuresConfiguration;

        private ObjectiveCalibrationFlow _objectiveCalibrationFlow;
        private IFlowTask _objectiveCalibrationFlowTask;
        private const string XYCalibrationRecipeFolder = "XYRecipes";
        private ANARecipe _calibrationRecipe;
        private XYCalibrationProgressCallback _currentProgressCallback;

        private LiseHFIntegrationTimeCalibrationFlow _liseHFIntegrationTimeCalibrationFlow;
        private IFlowTask _liseHFIntegrationTimeCalibrationFlowTask;

        private LiseHFSpotCalibrationFlow _liseHFSpotCalibrationFlow;
        private IFlowTask _liseHFSpotCalibrationFlowTask;

        internal bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;

        private ANAChuckConfig ChuckConfig
        {
            get
            {
                if (_anaHardwareManager.Chuck.Configuration is ANAChuckConfig anaChuckConfig)
                    return anaChuckConfig;
                else
                    throw new Exception("Configuration from chuck is wrong type");
            }
        }

        private delegate void ProgressCallbackFunction(string progress, ProgressType progressType);

        private delegate ProgressCallbackFunction ProgressCallbackGetter(ICalibrationServiceCallback callback);

        public CalibrationService(ILogger logger, AnaHardwareManager anaHardwareManager, CalibrationManager calibrationManager, IGlobalStatusServer globalStatusServer) : base(logger, ExceptionType.CalibrationException)
        {
            _anaHardwareManager = anaHardwareManager;
            _calibrationManager = calibrationManager;
            _globalStatusServer = globalStatusServer;
            _measuresConfiguration = ClassLocator.Default.GetInstance<MeasuresConfiguration>();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<RecipeProgress>(this, (r, m) => RecipeProgressChanged(m));
        }

        public Response<IEnumerable<ICalibrationData>> GetCalibrations()
        {
            return InvokeDataResponse(() =>
            {
                return _calibrationManager.Calibrations;
            });
        }

        public Response<List<OpticalReferenceDefinition>> GetReferences()
        {
            return InvokeDataResponse(() =>
            {
                try
                {
                    return ChuckConfig.ReferencesList;
                }
                catch (Exception Ex)
                {
                    _logger.Error("Calibration cannot GetReferences from configuration", Ex);
                    return null;
                }
            });
        }

        public Response<VoidResult> SubscribeToCalibrationChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Subscribed to calibration service change");
                Subscribe();
            });
        }

        public Response<VoidResult> UnsubscribeToCalibrationChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("UnSubscribed to calibration service change");
                Unsubscribe();
            });
        }

        public Response<VoidResult> SaveCalibration(ICalibrationData calibrationData)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information($"{calibrationData.User} save calibration {calibrationData.GetType()}");
                _calibrationManager.UpdateCalibration(calibrationData, UpdateMode.Persistent);
                if (calibrationData is LiseHFCalibrationData liseHfCalibrationData)
                {
                    ExportLiseHFIntegrationTimeCSVSignals(liseHfCalibrationData);
                    HardwareUtils.ResetProbeCalibrationManager(_anaHardwareManager, "ProbeLiseHF"); 
                }
            });
        }

        #region Objective Calibration
        public Response<VoidResult> StartObjectiveCalibration(ObjectiveCalibrationInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information($"Start objective calibration for {input.ObjectiveId}");
                if (FlowsAreSimulated)
                    _objectiveCalibrationFlow = new ObjectiveCalibrationFlowDummy(input);
                else
                    _objectiveCalibrationFlow = new ObjectiveCalibrationFlow(input);

                _objectiveCalibrationFlowTask = new FlowTask<ObjectiveCalibrationInput, ObjectiveCalibrationResult, ObjectiveCalibrationConfiguration>(_objectiveCalibrationFlow);
                _objectiveCalibrationFlow.StatusChanged += ObjectiveCalibrationFlow_StatusChanged;
                Task.Run(() => _objectiveCalibrationFlowTask.Start());
            });
        }

        public Response<VoidResult> CancelObjectiveCalibration()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _objectiveCalibrationFlowTask?.Cancel();
            });
        }

        private void ObjectiveCalibrationFlow_StatusChanged(FlowStatus status, ObjectiveCalibrationResult statusData)
        {
            InvokeCallback(callback => callback.ObjectiveCalibrationChanged(statusData));
            if (status.State == FlowState.Error || status.State == FlowState.Success)
                _objectiveCalibrationFlow.StatusChanged -= ObjectiveCalibrationFlow_StatusChanged;
        }

        public Response<List<ObjectiveToCalibrate>> GetObjectivesToCalibrate()
        {
            return InvokeDataResponse(messageContainer =>
            {
                var objectives = new List<ObjectiveToCalibrate>();
                foreach (var camera in _anaHardwareManager.Cameras.Values)
                {
                    foreach (var objective in _anaHardwareManager.ObjectivesSelectors[camera.Config.ObjectivesSelectorID].Config.Objectives)
                    {
                        var objectiveToCalibrate = new ObjectiveToCalibrate();
                        objectiveToCalibrate.Position = camera.Config.ModulePosition;
                        objectiveToCalibrate.CameraId = camera.DeviceID;
                        objectiveToCalibrate.DeviceId = objective.DeviceID;
                        objectiveToCalibrate.ObjectiveName = objective.Name;
                        objectiveToCalibrate.Magnification = objective.Magnification;
                        objectiveToCalibrate.ObjType = objective.ObjType;
                        objectiveToCalibrate.IsMain = objective.IsMainObjective;
                        objectives.Add(objectiveToCalibrate);
                    }
                }
                return objectives;
            });
        }
        #endregion

        #region XY Calibration
        public Response<VoidResult> StartXYCalibration(XYCalibrationInput input, string userName)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Task.Factory.StartNew(() =>
                {
                    // Remove existing calibration to make sure we compute the new calibration from raw
                    var previousCalib = _calibrationManager.PopCalibrationData<XYCalibrationData>();

                    // Try-finally to make sure that the previous calibration is set back even if an error occurs.
                    XYCalibrationData result = null;
                    try
                    {
                        ProgressCallbackGetter callbackGetter = calibrationService => calibrationService.XYCalibrationProgress;
                        WaferReferentialSettings preAlignment = ComputePreAlignment(input, callbackGetter);

                        var progressCallback = new XYCalibrationProgressCallback(callbackGetter, PreAlignmentInfosToPrepend(preAlignment));
                        result = ComputeCalibrationData<XYCalibrationData>(input, progressCallback, CalibrationType.Complete, preAlignment);
                        result.User = userName;
                        result.Input = input;
                    }
                    catch (Exception ex)
                    {
                        InvokeCallback(x => x.XYCalibrationProgress(ex.Message, ProgressType.Error));
                    }
                    finally
                    {
                        if (!(previousCalib is null))
                            _calibrationManager.UpdateCalibration(previousCalib);
                    }

                    if (!(result is null))
                        InvokeCallback(x => x.XYCalibrationChanged(result));
                });
            });
        }

        public Response<VoidResult> StopXYCalibration()
        {
            return InvokeVoidResponse(_ =>
            {
                var recipeManager = ClassLocator.Default.GetInstance<IANARecipeExecutionManager>();
                recipeManager.StopRecipe();

                var referencialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
                referencialManager.DeleteSettings(ReferentialTag.Wafer);
            });
        }

        public static string PreAlignmentInfosToPrepend(WaferReferentialSettings preAlignment)
        {
            const string format = "G4";

            var referencialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            var sb = new StringBuilder();

            var currentWaferReferential = referencialManager.GetSettings(ReferentialTag.Wafer) as WaferReferentialSettings;
            sb.AppendLine($"BWA + Alignment marks: ShiftX={currentWaferReferential.ShiftX.ToMostRepresentativeUnit().ToString(format)}, ShiftY={currentWaferReferential.ShiftY.ToMostRepresentativeUnit().ToString(format)}, Angle={currentWaferReferential.WaferAngle.ToString(format)}");
            sb.AppendLine($"Pre-alignment: ShiftX={preAlignment.ShiftX.ToMostRepresentativeUnit().ToString(format)}, ShiftY={preAlignment.ShiftY.ToMostRepresentativeUnit().ToString(format)}, Angle={preAlignment.WaferAngle.ToString(format)}\nCalibration :");
            return sb.ToString();
        }

        public Response<VoidResult> StartXYCalibrationTest(XYCalibrationData xYCalibrationData)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                Task.Factory.StartNew(() =>
                {
                    var previousCalib = _calibrationManager.GetXYCalibrationData();

                    XYCalibrationTest result = null;
                    // Try-finally to make sure that the previous calibration is set back even if an error occurs.
                    try
                    {
                        _calibrationManager.UpdateCalibration(xYCalibrationData);
                        ProgressCallbackGetter callbackGetter = calibrationService => calibrationService.XYCalibrationTestProgress;

                        xYCalibrationData.Input.CalibFlag |= CalibrationFlag.Test; //This is a Test
                        WaferReferentialSettings preAlignment = ComputePreAlignment(xYCalibrationData.Input, callbackGetter);
                        var progressCallback = new XYCalibrationProgressCallback(callbackGetter, PreAlignmentInfosToPrepend(preAlignment));

                        result = ComputeCalibrationData<XYCalibrationTest>(xYCalibrationData.Input, progressCallback, CalibrationType.Complete, preAlignment);
                    }
                    catch (Exception ex)
                    {
                        InvokeCallback(x => x.XYCalibrationTestProgress(ex.Message, ProgressType.Error));
                    }
                    finally
                    {
                        xYCalibrationData.Input.CalibFlag &= ~CalibrationFlag.Test; //This was a Test
                        if (!(previousCalib is null))
                            _calibrationManager.UpdateCalibration(previousCalib);
                        else
                            _calibrationManager.PopCalibrationData<XYCalibrationData>();
                    }

                    if (!(result is null))
                        InvokeCallback(x => x.XYCalibrationTestChanged(result));
                });
            });
        }

        private ANARecipe ReadCalibrationRecipe(XYCalibrationInput input, CalibrationType calibrationType)
        {
            string xyCalibrationFolder = Path.Combine(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath, XYCalibrationRecipeFolder);
            if (!Directory.Exists(xyCalibrationFolder))
            {
                Directory.CreateDirectory(xyCalibrationFolder);
            }
            var calibrationRecipe = XML.Deserialize<ANARecipe>(Path.Combine(xyCalibrationFolder, input.RecipeName + ".anarcp"));
            calibrationRecipe.Name = input.RecipeName;
            var externalFiles = SubObjectFinder.GetAllSubObjectOfTypeT<ExternalFileBase>(calibrationRecipe);
            calibrationRecipe.Step = CreateAchenseeWaferCharacteristic();

            // Update recipe with external files content.
            foreach (var externalFile in externalFiles.Values)
            {
                string externalFileName = Path.Combine(xyCalibrationFolder, externalFile.FileNameKey);
                externalFile.LoadFromFile(externalFileName);
            }

            XYCalibrationSettings calibrationSettings = (calibrationRecipe.Measures[0] as XYCalibrationSettings);
            if (!input.UseAutoFocus)
                calibrationSettings.AutoFocusSettings = null;

            var xyCalibrationConfig = _measuresConfiguration?.Measures.OfType<MeasureXYCalibrationConfiguration>().SingleOrDefault();
            if (calibrationType == CalibrationType.PreAlignment)
                calibrationRecipe.Dies = DieIndexSelector.SelectDiesOnCentralCross(calibrationRecipe.WaferMap.WaferMapData, xyCalibrationConfig.PreAlignmentDiesPeriodicityFromCenter, xyCalibrationConfig.PreAlignmentNbDiesPerBranch);
            else
                calibrationRecipe.Dies = DieIndexSelector.SelectDiesOnGrid(calibrationRecipe.WaferMap.WaferMapData, input.EveryNbDie, input.ScanDirection);

            return calibrationRecipe;
        }

        // TODO : since we store only the recipe, we don't have informations about product (i.e. wafer).
        // Then we have to create a new one here.
        // For now, we only use the Achensee wafer for calibration.
        private Step CreateAchenseeWaferCharacteristic()
        {
            var step = new Step();
            step.Product = new Product();
            step.Product.WaferCategory = new WaferCategory();
            step.Product.WaferCategory.DimentionalCharacteristic = new WaferDimensionalCharacteristic
            {
                WaferShape = WaferShape.Notch,
                Diameter = 300.Millimeters(),
                Category = "1.15",
                Notch = new NotchDimentionalCharacteristic
                {
                    Depth = 1.Millimeters(),
                    Angle = 0.Degrees(),
                    DepthPositiveTolerance = 0.25.Millimeters(),
                    AngleNegativeTolerance = 1.Degrees(),
                    AnglePositiveTolerance = 5.Degrees()
                },
            };
            return step;
        }

        private WaferReferentialSettings ComputePreAlignment(XYCalibrationInput input, ProgressCallbackGetter progressCallbackGetter)
        {
            var progressCallback = new XYCalibrationProgressCallback(progressCallbackGetter, "BWA, Alignment Marks and Pre-alignment: ");
            WaferReferentialSettings initialShift = null;
            input.CalibFlag |= CalibrationFlag.PreAlign; // this is a prealignment
            if (input.InitialShiftCenterX != 0.Millimeters() || input.InitialShiftCenterY != 0.Millimeters())
            {
                initialShift = new WaferReferentialSettings()
                {
                    ShiftX = input.InitialShiftCenterX,
                    ShiftY = input.InitialShiftCenterY
                };
            }
            var preAlignmentCalibration = ComputeCalibrationData<XYCalibrationData>(input, progressCallback, CalibrationType.PreAlignment, initialShift);
            input.CalibFlag &= ~CalibrationFlag.PreAlign; // this was a prealignment
            return ExtractAlignmentSettings(preAlignmentCalibration);
        }

        private static WaferReferentialSettings ExtractAlignmentSettings(XYCalibrationData calibration)
        {
            return new WaferReferentialSettings()
            {
                ShiftX = calibration.ShiftX,
                ShiftY = calibration.ShiftY,
                WaferAngle = calibration.ShiftAngle
            };
        }

        private static void TransferAlignmentShiftsToCalibration(WaferReferentialSettings alignment, XYCalibrationData calibration)
        {
            if (alignment is null)
                return;

            calibration.ShiftX += alignment.ShiftX;
            calibration.ShiftY += alignment.ShiftY;
            calibration.ShiftAngle += alignment.WaferAngle;
        }


        private void RecipeProgressChanged(RecipeProgress progress)
        {
            if (_currentProgressCallback is null)
                return;
            XYCalibrationSettings calibrationMeasure = _calibrationRecipe.Measures.OfType<XYCalibrationSettings>().FirstOrDefault();
            int initialNbPoints = calibrationMeasure.MeasurePoints.Count * (_calibrationRecipe.WaferHasDies ? _calibrationRecipe.Dies.Count : 1);
           
            int completion = 100 - (100 * progress.NbRemainingPoints) / initialNbPoints;
            InvokeCallback(x => _currentProgressCallback.ProgressCallbackGetter(x)($"{_currentProgressCallback.ToPrepend} progress {completion} % ({progress.NbRemainingPoints} remaining)", ProgressType.Information));
         
        }

        private TXYCalibrationData ComputeCalibrationData<TXYCalibrationData>(XYCalibrationInput input, XYCalibrationProgressCallback progressCallback, CalibrationType calibrationType, WaferReferentialSettings preAlignment = null)
            where TXYCalibrationData : XYCalibrationData, new()
        {
            _calibrationRecipe = ReadCalibrationRecipe(input, calibrationType);
            var referencialManager = ClassLocator.Default.GetInstance<IReferentialManager>();
            referencialManager.EnableReferentialConverter(ReferentialTag.Wafer, ReferentialTag.Stage);
            referencialManager.EnableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Wafer);

            if (!(preAlignment is null))
            {
                // Add preAlignment result to current wafer referential computed by the recipe alignment (Autofocus + BWA + MarkAlign).
                var currentWaferReferential = referencialManager.GetSettings(ReferentialTag.Wafer) as WaferReferentialSettings;
                preAlignment.ShiftX += currentWaferReferential.ShiftX;
                preAlignment.ShiftY += currentWaferReferential.ShiftY;
                preAlignment.WaferAngle += currentWaferReferential.WaferAngle;

                referencialManager.SetSettings(preAlignment);

                _calibrationRecipe.Execution.Alignment.RunMarkAlignment = false;
                _calibrationRecipe.Execution.Alignment.RunAutoFocus = false;
                _calibrationRecipe.Execution.Alignment.RunBwa = false;
            }
            else
            {
                referencialManager.DeleteSettings(ReferentialTag.Wafer);
            }

            bool IsACalibTest = input.CalibFlag.HasFlag(CalibrationFlag.Test);
            if (IsACalibTest)
            {
                referencialManager.EnableReferentialConverter(ReferentialTag.Motor, ReferentialTag.Stage);
                referencialManager.EnableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Motor);
            }

            XYCalibrationSettings calibrationSettings = (_calibrationRecipe.Measures[0] as XYCalibrationSettings);
            calibrationSettings.CalibFlag = input.CalibFlag;
            var waferCalibrationDiameter = _calibrationRecipe.Step.Product.WaferCategory.DimentionalCharacteristic.Diameter;

            var recipeExecutionManager = ClassLocator.Default.GetInstance<IANARecipeExecutionManager>();

            Dictionary<string, MetroResult> res;
            try
            {
                _currentProgressCallback = progressCallback;
                res = recipeExecutionManager.Execute(_calibrationRecipe);
                _currentProgressCallback = null;
            }
            catch (Exception e)
            {
                referencialManager.DeleteSettings(ReferentialTag.Wafer);

                var msg = "Error in " + _calibrationRecipe.Name + " recipe execution";

                _logger.Error($"{msg} : {e.Message}");
                throw new Exception(msg);
            }

            var calibrationResult = res.First().Value.MeasureResult as XYCalibrationResult;
            var result = XYCalibration.ComputeCalibration<TXYCalibrationData>(calibrationResult, waferCalibrationDiameter, input.CalibFlag.HasFlag(CalibrationFlag.PreAlign));

            if (IsACalibTest)
            {
                referencialManager.DisableReferentialConverter(ReferentialTag.Motor, ReferentialTag.Stage);
                referencialManager.DisableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Motor);
            }

            if (!(preAlignment is null))
            {
                referencialManager.DisableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Wafer);
                referencialManager.DisableReferentialConverter(ReferentialTag.Wafer, ReferentialTag.Stage);
                referencialManager.DeleteSettings(ReferentialTag.Wafer);
                TransferAlignmentShiftsToCalibration(preAlignment, result);
            }

            if (!(_calibrationRecipe.WaferMap is null))
                referencialManager.DeleteSettings(ReferentialTag.Die);

            return result;
        }

        public Response<List<XYCalibrationRecipe>> GetXYCalibrationRecipes()
        {
            return InvokeDataResponse(messageContainer =>
            {
                var recipes = new List<XYCalibrationRecipe>();
                string xyCalibrationFolder = Path.Combine(ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().CalibrationFolderPath, XYCalibrationRecipeFolder);
                if (!Directory.Exists(xyCalibrationFolder))
                {
                    Directory.CreateDirectory(xyCalibrationFolder);
                }

                var recipeFiles = Directory.GetFiles(xyCalibrationFolder, "*.anarcp");
                foreach (var recipeFile in recipeFiles)
                {
                    try
                    {
                        var calibrationRecipe = XML.Deserialize<ANARecipe>(Path.Combine(xyCalibrationFolder, recipeFile));

                        if (calibrationRecipe.WaferHasDies)
                        {
                            recipes.Add(new XYCalibrationRecipe()
                            {
                                Name = Path.GetFileNameWithoutExtension(recipeFile),
                                WaferMap = calibrationRecipe.WaferMap.WaferMapData,
                                Settings = (calibrationRecipe.Measures[0] as XYCalibrationSettings)
                            });
                        }
                        else
                        {
                            _logger.Warning($"The calibration recipe must contains dies {recipeFile}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error in calibration recipe {recipeFile}", ex);
                    }
                }
                return recipes;
            });
        }

        private enum CalibrationType
        {
            PreAlignment,
            Complete,
        }

        private class XYCalibrationProgressCallback
        {
            public XYCalibrationProgressCallback(ProgressCallbackGetter progressCallbackGetter, string toPrepend)
            {
                ProgressCallbackGetter = progressCallbackGetter;
                ToPrepend = toPrepend;
            }

            public ProgressCallbackGetter ProgressCallbackGetter { get; set; }

            public string ToPrepend { get; set; }
        }
        #endregion

        #region LiseHF

        #region Integration Time Calibration
        public Response<VoidResult> StartLiseHFIntegrationTimeCalibration(LiseHFIntegrationTimeCalibrationInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information($"Start LiseHF IntegrationTime calibration");

                _liseHFIntegrationTimeCalibrationFlow = FlowsAreSimulated
                    ? new LiseHFIntegrationTimeCalibrationFlowDummy(input)
                    : new LiseHFIntegrationTimeCalibrationFlow(input);

                _liseHFIntegrationTimeCalibrationFlowTask = new FlowTask<LiseHFIntegrationTimeCalibrationInput, LiseHFIntegrationTimeCalibrationResults, LiseHFIntegrationTimeCalibrationConfiguration>(_liseHFIntegrationTimeCalibrationFlow);
                _liseHFIntegrationTimeCalibrationFlow.StatusChanged += LiseHFIntegrationTimeCalibrationFlow_StatusChanged;
                Task.Run(() => _liseHFIntegrationTimeCalibrationFlowTask.Start());
            });
        }

        public Response<VoidResult> StopLiseHFIntegrationTimeCalibration()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _liseHFIntegrationTimeCalibrationFlowTask?.Cancel();
            });
        }

        private void LiseHFIntegrationTimeCalibrationFlow_StatusChanged(FlowStatus status, LiseHFIntegrationTimeCalibrationResults calibrationData)
        {
            InvokeCallback(callback => callback.LiseHFRefCalibrationChanged(calibrationData));
            if (status.State == FlowState.Error || status.State == FlowState.Success || status.State == FlowState.Canceled)
                _liseHFIntegrationTimeCalibrationFlow.StatusChanged -= LiseHFIntegrationTimeCalibrationFlow_StatusChanged;
        }
        #endregion

        #region Spot Calibration
        public Response<VoidResult> StartLiseHFSpotCalibration(LiseHFSpotCalibrationInput input)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information($"Start LiseHF Spot calibration");
                _liseHFSpotCalibrationFlow = FlowsAreSimulated
                   ? new LiseHFSpotCalibrationFlowDummy(input)
                   : new LiseHFSpotCalibrationFlow(input);

                _liseHFSpotCalibrationFlowTask = new FlowTask<LiseHFSpotCalibrationInput, LiseHFSpotCalibrationResults, LiseHFSpotCalibrationConfiguration>(_liseHFSpotCalibrationFlow);
                _liseHFSpotCalibrationFlow.StatusChanged += LiseHFSpotCalibrationFlow_StatusChanged;
                Task.Run(() => _liseHFSpotCalibrationFlowTask.Start());
            });
        }

        public Response<VoidResult> StopLiseHFSpotCalibration()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _liseHFSpotCalibrationFlowTask?.Cancel();
            });

        }

        private void LiseHFSpotCalibrationFlow_StatusChanged(FlowStatus status, LiseHFSpotCalibrationResults spotCalibrationData)
        {
            InvokeCallback(callback => callback.LiseHFSpotCalibrationChanged(spotCalibrationData));
            if (status.State == FlowState.Error || status.State == FlowState.Success || status.State == FlowState.Canceled)
                _liseHFSpotCalibrationFlow.StatusChanged -= LiseHFSpotCalibrationFlow_StatusChanged;
        }
        #endregion

        private void ExportLiseHFIntegrationTimeCSVSignals(LiseHFCalibrationData calibrationData)
        {
            foreach (var liseHfObjectiveIntegrationTimeCalibration in calibrationData.IntegrationTimes)
            {
                if (liseHfObjectiveIntegrationTimeCalibration.StandardSignal != null &&
                    liseHfObjectiveIntegrationTimeCalibration.LowIllumSignal != null &&
                    liseHfObjectiveIntegrationTimeCalibration.StandardSignal.Count == liseHfObjectiveIntegrationTimeCalibration.LowIllumSignal.Count)
                {
                    string filename = $"LiseHF_IT_{liseHfObjectiveIntegrationTimeCalibration.ObjectiveDeviceId}_{liseHfObjectiveIntegrationTimeCalibration.Date.ToString("yyyyMMdd_HHmmss")}";
                    var sbCSV = new CSVStringBuilder();
                    sbCSV.AppendLine((liseHfObjectiveIntegrationTimeCalibration.WaveSignal != null) ? "Index" : "WL",
                        $"STD ({liseHfObjectiveIntegrationTimeCalibration.StandardFilterIntegrationTime_ms} ms | {liseHfObjectiveIntegrationTimeCalibration.StandardFilterBaseCount} cnt)",
                        $"LOW ({liseHfObjectiveIntegrationTimeCalibration.LowIllumFilterIntegrationTime_ms} ms | {liseHfObjectiveIntegrationTimeCalibration.LowIllumFilterBaseCount} cnt)");
                    for (int i = 0; i < liseHfObjectiveIntegrationTimeCalibration.StandardSignal.Count; i++)
                    {
                        sbCSV.AppendLine(((liseHfObjectiveIntegrationTimeCalibration.WaveSignal != null) ? i.ToString() : liseHfObjectiveIntegrationTimeCalibration.WaveSignal[i].ToString()),
                            liseHfObjectiveIntegrationTimeCalibration.StandardSignal[i].ToString(),
                            liseHfObjectiveIntegrationTimeCalibration.LowIllumSignal[i].ToString());
                    }

                    _calibrationManager.WriteLiseHFCalibCSV(filename, sbCSV.ToString());
                }
            }
        }

        #endregion

    }
}
