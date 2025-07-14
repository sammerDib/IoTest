using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Flows.AutoExposure;
using UnitySC.PM.DMT.Service.Flows.Deflectometry;
using UnitySC.PM.DMT.Service.Flows.Dummy;
using UnitySC.PM.DMT.Service.Flows.FlowTask;
using UnitySC.PM.DMT.Service.Flows.SaveImage;
using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Execution.Measure;
using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.AutoExposure;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.DMT.Service.Interface.Fringe;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Measure.Configuration;
using UnitySC.PM.DMT.Service.Interface.Measure.Outputs;
using UnitySC.PM.DMT.Service.Interface.Proxy;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.DMT.Service.Interface.Screen;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Data.Ada;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Composer;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.MonitorTasks;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

using CorrectorFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.CorrectorInput,
    UnitySC.PM.DMT.Service.Interface.Flow.CorrectorResult,
    UnitySC.PM.DMT.Service.Interface.Flow.CorrectorConfiguration>;
using SaveImageFlowTask = UnitySC.PM.Shared.Flow.Implementation.FlowTask<
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageInput,
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageResult,
    UnitySC.PM.DMT.Service.Interface.Flow.SaveImageConfiguration>;

namespace UnitySC.PM.DMT.Service.Implementation.Execution
{
    public class RecipeExecution
    {
        private static int UniqueID;
        
        private const string MttReportFolderName = "MTT";
        private const string FlowExecutionErrorKey = "FlowError";

        private readonly DMTCameraManager _cameraManager;
        private readonly DbRegisterAcquisitionServiceProxy _dbRegisterAcqService;
        private readonly DbToolServiceProxy _dbToolService;
        private readonly DMTHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly object _mutex = new object();
        private readonly PMConfiguration _pmConfiguration;
        private readonly MeasuresConfiguration _measuresConfiguration;
        private readonly FlowsConfiguration _flowsConfiguration;
        private readonly MonitorTaskTimer _monitorTaskTimer;

        private readonly TemplateComposer _acqPathComposer;
        private readonly TemplateComposer _acqFileNameComposer;
        private readonly TemplateComposer _adaFolderComposer;
        private readonly TemplateComposer _adaFileNameComposer;

        private readonly IDMTScreenService _screenService;
        private readonly Length _chuckProcessPos = new Length(2, LengthUnit.Millimeter); // 2 = Process position

        private Dictionary<Side, Dictionary<CurvatureImageType, ImageData>> _curvatureMapMasksBySide;
        private Dictionary<Side, Dictionary<FringesDisplacement, PSDResult>> _darkSourcesBySide;
        private List<MeasureExecutionBase> _execList;
        private bool _isFirstWafer = true;
        private Chamber _pmChamber;
        private Dictionary<Side, Dictionary<CurvatureImageType, ImageData>> _rawCurvatureMapsBySide;
        private DateTime _recipeExecutionDateTime;
        private readonly ILogger<RecipeExecution> _logger;
        private CancellationTokenSource _executionCancellationTokenSource;
        private int _totalAcquisitionSteps;
        private int _currentAcquisitionStep;
        private int _currentComputationStep;
        private int _totalComputationSteps;
        private readonly bool _applyCorrectors;

        public AdaWriter AdaWriterBS;
        public readonly object AdaWriterBSLock;
        public AdaWriter AdaWriterFS;
        public readonly object AdaWriterFSLock;

        public DMTRecipe Recipe { get; private set; }
        public bool RecipeRequiresComputation;
        public bool IsRecipeRunning;
        private bool _areFlowsSimulated;

        public RecipeExecution(
            PMConfiguration pmConfiguration, DMTHardwareManager hardwareManager, IDMTScreenService screenService,
            DbToolServiceProxy dbToolService, CalibrationManager calibrationManager,
            DbRegisterAcquisitionServiceProxy dbRegisterAcqService, DMTCameraManager cameraManager,
            FlowsConfiguration flowsConfiguration, MeasuresConfiguration measuresConfiguration, IPMServiceConfigurationManager serviceConfiguration, Lazy<MonitorTaskTimer> monitorTaskTimer, ILogger<RecipeExecution> logger)
        {
            _pmConfiguration = pmConfiguration;
            _hardwareManager = hardwareManager;
            _screenService = screenService;
            _dbToolService = dbToolService;
            _dbRegisterAcqService = dbRegisterAcqService;
            _cameraManager = cameraManager;
            _calibrationManager = calibrationManager;
            _executionCancellationTokenSource = new CancellationTokenSource();
            AdaWriterFSLock = new object();
            AdaWriterBSLock = new object();
            _logger = logger;
            _measuresConfiguration = measuresConfiguration;
            _areFlowsSimulated = serviceConfiguration.FlowsAreSimulated;

            _flowsConfiguration = flowsConfiguration;
            _applyCorrectors = flowsConfiguration.Flows.OfType<CorrectorConfiguration>().FirstOrDefault()?.AreApplied ?? false;

            _acqPathComposer = new TemplateComposer(_pmConfiguration.OutputAcqPathTemplate, ResultPathParams.Empty);
            _acqFileNameComposer = new TemplateComposer(_pmConfiguration.OutputAcqFileNameTemplate, ResultPathParams.Empty);
            _adaFolderComposer = new TemplateComposer(_pmConfiguration.OutputAdaFolder, ResultPathParams.Empty);
            _adaFileNameComposer = new TemplateComposer(_pmConfiguration.OutputAdaFileNameTemplate, ResultPathParams.Empty);
            _monitorTaskTimer = _pmConfiguration.MonitorTaskTimerIsEnable ? monitorTaskTimer.Value : null;
        }

        public PathString OutputPath { get; private set; }
        public PathString FileBasePathFS { get; private set; }
        public PathString FileBasePathBS { get; private set; }
        public RemoteProductionInfo RemoteProductionInfo { get; private set; }

        public string AcqDestFolder { get; private set; }

        public static bool Aborting { get; private set; }

        public string DataflowID { get; private set; }

        public Chamber PmChamber
        {
            get
            {
                if (_pmChamber == null)
                {
                    _pmChamber =
                        _dbToolService.GetChamber(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey);
                }

                return _pmChamber;
            }
        }

        public event EventHandler<RecipeStatus> ProgressChanged;

        public event EventHandler<DMTResultGeneratedEventArgs> ResultGenerated;

        private int ComputeNumberOfAcquisitionSteps()
        {
            return _execList.Sum(mExec => mExec.ComputeNumberOfAcquisitionSteps());
        }

        private int ComputeNumberOfComputationSteps()
        {
            int steps = _execList.Sum(mExec => mExec.ComputeNumberOfComputationSteps());
            if (!_applyCorrectors)
            {
                steps -= _execList.OfType<BrightFieldMeasureExecution>().Sum(bfExec => bfExec.ComputeNumberOfComputationSteps());
            }

            return steps;
        }


        // Used in engineering mode
        public void StartRecipe(DMTRecipe recipe, string acqDestFolder, bool overwriteOutput, string dataflowID)
        {
            if (acqDestFolder == null)
                AcqDestFolder = string.Empty;
            else
                AcqDestFolder = Path.Combine(acqDestFolder, DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            StartRecipe(recipe, remoteProductionInfo:null, overwriteOutput, dataflowID);
        }


        /// <summary>
        ///     Démarre une recette qui s'exécutera en tâche de fond
        /// </summary>
        public void StartRecipe(DMTRecipe recipe, RemoteProductionInfo remoteProductionInfo, bool overwriteOutput, string dataflowID, bool productionMode = true)
        {
            _executionCancellationTokenSource = new CancellationTokenSource();
            try
            {
                if (!(_monitorTaskTimer is null))
                {
                    StartMtt(recipe.Name);
                }
                var moveChuckTask = MoveChuckToProcessPosition();
                Recipe = recipe;
                RemoteProductionInfo = remoteProductionInfo;
                DataflowID = dataflowID;
                AdaWriterFS = null;
                AdaWriterBS = null;
                _currentAcquisitionStep = 0;
                _totalAcquisitionSteps = 0;
                _currentComputationStep = 0;
                _totalComputationSteps = 0;
                _recipeExecutionDateTime = DateTime.Now;
                SendFirstRecipeState();

                if (RemoteProductionInfo != null)
                {
                    var jobPos = RemoteProductionInfo.ProcessedMaterial.JobPosition;
                    _isFirstWafer = jobPos == JobPosition.First || jobPos == JobPosition.FirstAndLast;
                }
                else
                {
                    _isFirstWafer = true;
                }

                // On lance l'exécution en tâche de fond
                _logger.Information("Starting recipe " + recipe.Name + " wafer:" + RemoteProductionInfo?.ProcessedMaterial?.WaferBaseName);

                PrepareRecipeExecution(overwriteOutput, productionMode);
                moveChuckTask.Wait();
            }
            catch (Exception ex)
            {
                _logger.Error("Creation failed:" + ex);
                ReportProgress(new RecipeStatus
                {
                    State = DMTRecipeState.Failed,
                    Message = $"Creation Error: {ex.Message}"
                });

                return;
            }

            Start(ExecuteRecipe);
        }

        /// <summary>
        ///     Prepares the recipe execution
        /// </summary>
        private void PrepareRecipeExecution(bool overwriteOutput, bool productionMode = true)
        {
            if (!(_monitorTaskTimer is null))
            {
                StartMtt("Recipe preparation");
            }
            string outputAcqServer = _pmConfiguration.OutputAcqServer;
            _execList = CreateMeasureExecutionList(productionMode);

            ResultPathParams prm = null;
            if (RemoteProductionInfo is null)
            {
                OutputPath = AcqDestFolder;
                FileBasePathFS = Enum.GetName(typeof(Side), Side.Front) + @"\Wafer";
            }
            else
            {
                prm = new ResultPathParams
                {
                    ToolName = PmChamber.Tool.Name,
                    ToolId = PmChamber.Tool.Id,
                    ToolKey = PmChamber.Tool.ToolKey,
                    ChamberName = PmChamber.Name,
                    ChamberId = PmChamber.Id,
                    ChamberKey = PmChamber.ChamberKey,
                    JobName = RemoteProductionInfo.ProcessedMaterial.ProcessJobID,
                    //JobId = // database ID not know here
                    LotName = RemoteProductionInfo.ProcessedMaterial.LotID,
                    RecipeName = RemoteProductionInfo.DFRecipeName,
                    StartProcessDate = _recipeExecutionDateTime,
                    Slot = RemoteProductionInfo.ProcessedMaterial.SlotID,
                    // RunIter = // not known yet
                    WaferName = RemoteProductionInfo.ProcessedMaterial.WaferBaseName,
                    // ResultType = // not known here and generic for multiple acquisition result use that for
                    // Index = res.Idx,
                    ProductName = Recipe.Step.Product.Name,
                    StepName = Recipe.Step.Name
                };

                string outputTemplatePathFS = GenerateOutputFilePath(prm, Side.Front);
                OutputPath = Path.Combine(outputAcqServer, outputTemplatePathFS);
                FileBasePathFS = GenerateOutputFileName(prm, Side.Front);
            }
            try
            {
                CreateOutputDirectory(OutputPath, overwriteOutput, FileBasePathFS.Directory);
                OnResultGenerated(new DMTResultGeneratedEventArgs("folder", Side.Front, OutputPath / FileBasePathFS.Directory));
            }
            catch (Exception)
            {
                _logger.Debug($"Failed to create the OutputPath: {OutputPath / FileBasePathFS.Directory}");
            }

            if (HasBackSideMeasure())
            {
                if (RemoteProductionInfo is null)
                {
                    FileBasePathBS = Enum.GetName(typeof(Side), Side.Back) + @"\Wafer";
                }
                else
                {
                    // prm ResultType must be NotDefined here, or "back" folder will not be created
                    FileBasePathBS = GenerateOutputFileName(prm, Side.Back);
                }
                try
                {
                    CreateOutputDirectory(OutputPath, overwriteOutput, FileBasePathBS.Directory);
                    OnResultGenerated(new DMTResultGeneratedEventArgs("folder", Side.Back, OutputPath / FileBasePathBS.Directory));
                }
                catch (Exception)
                {
                    _logger.Debug($"Failed to create the OutputPath: {OutputPath / FileBasePathBS.Directory}");
                }
            }

            RecipeRequiresComputation |= _execList.Exists(mExec => mExec is DeflectometryMeasureExecution);

            if (Recipe.AreAcquisitionsSavedInDatabase && productionMode)
            {
                long previousInternalDbResId = -1;
                foreach (var measureExecution in _execList)
                {
                    if (previousInternalDbResId == -1)
                    {
                        previousInternalDbResId = measureExecution.PreRegisterFirstAcquisitionResult();
                    }
                    else
                    {
                        measureExecution.PreRegisterAcquisitionResultsWithParent(previousInternalDbResId);
                    }
                }
            }

            if (!(_monitorTaskTimer is null))
            {
                EndMtt("Recipe preparation");
            }
        }

        public void AbortExecution()
        {
            _executionCancellationTokenSource.Cancel();
        }

        private Task MoveChuckToProcessPosition()
        {
            return Task.Run(() =>
            {
                if (!IsChuckInProcessPosition())
                {
                    var move = new PMAxisMove("Linear", _chuckProcessPos);
                    _hardwareManager.MotionAxes.Move(move);
                    _hardwareManager.MotionAxes.WaitMotionEnd(5000); // Wait motion end for MotionAxes is not reliable!!!!!
                    SpinWait.SpinUntil(IsChuckInProcessPosition);
                }
            });
        }

        private bool IsChuckInProcessPosition()
        {
            try
            {
                if (_hardwareManager.MotionAxes != null)
                {
                    var pos = (XTPosition)_hardwareManager.MotionAxes.GetPosition();
                    return pos.X.Millimeters() == _chuckProcessPos;
                }

                // Tools without MotionAxes are considered to be in process position
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GenerateOutput(ResultPathParams prm, Side side, TemplateComposer composer)
        {
            bool needRevertResultType = false;
            if (prm.ResultType == ResultType.NotDefined)
            {
                // needed to handle Side since we did not use resultType here
                needRevertResultType = true;
                prm.ResultType = side == Side.Front ? ResultType.DMT_CurvatureX_Front : ResultType.DMT_CurvatureX_Back;
            }

            string output = composer.ComposeWith(prm);

            if (needRevertResultType)
                prm.ResultType = ResultType.NotDefined;

            return output;
        }

        private string GenerateOutputFilePath(ResultPathParams prm, Side side)
        {
            return GenerateOutput(prm, side, _acqPathComposer);
        }

        private string GenerateOutputFileName(ResultPathParams prm, Side side)
        {
            return GenerateOutput(prm, side, _acqFileNameComposer);
        }

        /// <summary>
        ///     Thread qui enchaîne les acquisitions pour les différentes mesures
        /// </summary>
        private async Task ExecuteRecipe()
        {
            Thread.CurrentThread.Name = "RecipeExecution";
            _logger.Debug("Starting recipe thread");

            // ADA file FS and one BS if needed
            AdaWriterFS = CreateAda(Side.Front, AcqDestFolder);

            if (HasBackSideMeasure())
                AdaWriterBS = CreateAda(Side.Back, AcqDestFolder);

            var chrono = new Stopwatch();
            chrono.Start();
            if (_isFirstWafer)
                MeasureExecutionWithAutoExposure.ResetCache();
            try
            {
                foreach (var screen in _hardwareManager.Screens.Values)
                {
                    screen.Clear();
                }

                var correctorFlowTaskTuples = new List<(Side, DMTSingleAcquisitionFlowTask, DMTCorrectorFlowTask)>(2);
                var recipeSingleAcquisitionFlowTasks =
                    GetRecipeSingleAcquisitionFlowTasksAndAddNecessaryCorrectorFlowTasks(correctorFlowTaskTuples);

                var recipeDeflectometryAcquisitionFlowTasks = _execList.OfType<DeflectometryMeasureExecution>()
                    .Select(mExec => mExec.GetAcquisitionTask())
                    .ToList();
                
                IFlowTask firstTask;
                IFlowTask lastComputationTask = null;
                var imageSavingTasks =
                    new List<SaveImageFlowTask>(recipeSingleAcquisitionFlowTasks.Count +
                                                8 * recipeDeflectometryAcquisitionFlowTasks.Count);

                if (!(_monitorTaskTimer is null))
                {
                    StartMtt(Recipe.Name + " execution");
                }
                // Create single acquisition tasks (for Bright-field, High angle dark-field and Backlight measure executions)
                var lastAcquisitionTask = recipeSingleAcquisitionFlowTasks
                    .Aggregate<DMTSingleAcquisitionFlowTask, DMTSingleAcquisitionFlowTask>(null,
                        (previousTask, currentTask) =>
                        {
                            if (previousTask is null)
                            {
                                currentTask.Start(_executionCancellationTokenSource, AutoExposureFlowOnStatusChanged,
                                    AcquireImageFlowOnStatusChanged, AcquisitionSaveImageFlowOnStatusChanged,
                                    OnResultGenerated);
                                firstTask = currentTask.FirstTask;
                                imageSavingTasks.Add(currentTask.SaveImageTask);
                                return currentTask;
                            }

                            var result = previousTask.ContinueWith(currentTask, AutoExposureFlowOnStatusChanged,
                                AcquireImageFlowOnStatusChanged,
                                AcquisitionSaveImageFlowOnStatusChanged, OnResultGenerated);
                            imageSavingTasks.Add(result.SaveImageTask);
                            return result;
                        });
                
                // Create deflectometry acquisition tasks
                var lastDeflectometryAcquisitionFlowTask =
                    recipeDeflectometryAcquisitionFlowTasks
                        .Aggregate<DMTDeflectometryAcquisitionFlowTask, DMTDeflectometryAcquisitionFlowTask>(null,
                            (previousFlowTask, currentFlowTask) =>
                            {
                                if (!(previousFlowTask is null))
                                {
                                    return previousFlowTask.ContinueWith(currentFlowTask,
                                        AutoExposureFlowOnStatusChanged,
                                        AcquirePhaseImageFlowOnStatusChanged);
                                }

                                if (!recipeSingleAcquisitionFlowTasks.IsNullOrEmpty())
                                {
                                    recipeSingleAcquisitionFlowTasks.Last()
                                        .ContinueWith(currentFlowTask,
                                            AutoExposureFlowOnStatusChanged,
                                            AcquirePhaseImageFlowOnStatusChanged);
                                }
                                else
                                {
                                    currentFlowTask.Start(_executionCancellationTokenSource,
                                        AutoExposureFlowOnStatusChanged,
                                        AcquirePhaseImageFlowOnStatusChanged);
                                }

                                return currentFlowTask;
                            });

                // Add corrector computations if necessary
                var recipeDeflectometryComputationFlowTasks = _execList.OfType<DeflectometryMeasureExecution>()
                    .Select(mExec =>
                    {
                        var calculationTask = mExec.GetCalculationTask();
                        var dfMeasureOutputs = mExec.Measure.Outputs;
                        calculationTask.AddComputePhaseMapHandler(ComputePhaseMapFlowOnStatusChanged);
                        if (dfMeasureOutputs.HasFlag(DeflectometryOutput.Curvature))
                        {
                            calculationTask.AddCurvatureMapsHandlers(ComputeRawCurvatureMapFlowOnStatusChanged, AdjustCurvatureDynamicsForRawCurvatureMapFlowOnStatusChanged);
                        }
                        if (dfMeasureOutputs.HasFlag(DeflectometryOutput.LowAngleDarkField))
                        {
                            calculationTask.AddComputeLowAngleDarkFieldHandlers(ComputeDarkImageFlowOnStatusChanged);
                        }
                        if (dfMeasureOutputs.HasFlag(DeflectometryOutput.NanoTopo))
                        {
                            calculationTask.AddNanoTopoHandlers(ComputeNanoTopoFlowOnStatusChanged);
                        }
                        if (calculationTask.SaveImageTasks.IsNullOrEmpty())
                        {
                            calculationTask.AddSaveImageHandler(ComputationSaveImageFlowOnStatusChanged);
                        }

                        return calculationTask;
                    })
                    .ToList();
                var adaTasks = new List<Task>(2);
                foreach (var (correctorSide, dmtAcquisitionFlowTask, dmtCorrectorFlowTask) in correctorFlowTaskTuples)
                {
                    lastComputationTask = dmtAcquisitionFlowTask.ContinueWith(dmtCorrectorFlowTask, lastComputationTask).ComputationTask;
                    object adaLock = correctorSide == Side.Front ? AdaWriterFSLock : AdaWriterBSLock;
                    var adaWriter = correctorSide == Side.Front ? AdaWriterFS : AdaWriterBS;
                    adaTasks.Add(dmtCorrectorFlowTask.ComputationTask.ContinueWith(computationFlowTask =>
                    {
                        var correctorFlowTask = (Task<CorrectorResult>)computationFlowTask;
                        if (!(correctorFlowTask.Result.WaferAngle is null))
                        {
                            lock (adaLock)
                            {
                                adaWriter.WriteInfoWafer("WaferXShiftum",
                                    $"{correctorFlowTask.Result.WaferXShift.Micrometers}");
                                adaWriter.WriteInfoWafer("WaferYShiftum",
                                    $"{correctorFlowTask.Result.WaferYShift.Micrometers}");
                                adaWriter.WriteInfoWafer("WaferAngleDegrees",
                                    $"{correctorFlowTask.Result.WaferAngle.Degrees}");
                            }
                        }
                    }));
                    recipeDeflectometryComputationFlowTasks.FirstOrDefault(flowTask =>
                        flowTask.Side == correctorSide)?.SetCorrectorResultForPhaseMapFlows(dmtCorrectorFlowTask);
                }

                // Add deflectometry computation tasks if necessary
                Task<SaveMaskResult> lastSaveMaskTask = null;
                recipeDeflectometryComputationFlowTasks.ForEach(computationFlowTask =>
                {
                    lastComputationTask =
                        computationFlowTask
                            .CreateAndChainComputationContinuationTasks(OnResultGenerated, lastComputationTask);
                    imageSavingTasks.AddRange(computationFlowTask.SaveImageTasks);
                    lastSaveMaskTask = (Task<SaveMaskResult>)computationFlowTask.SaveMaskFlowTask;
                });

                if (!(lastDeflectometryAcquisitionFlowTask is null))
                {
                    lastDeflectometryAcquisitionFlowTask.LastAcquisitionTask.ContinueWith(previousAcquisitionTask =>
                    {
                        if (previousAcquisitionTask.Result.Status.State == FlowState.Success)
                        {
                            ReportProgress(new RecipeStatus
                            {
                                State = DMTRecipeState.AcquisitionComplete,
                                Step = DMTRecipeExecutionStep.Acquisition,
                                Message = $"All acquisitions have been done for recipe {Recipe.Name}"
                            });
                        }
                    });
                }
                else
                {
                    lastAcquisitionTask.LastAcquisitionTask.ContinueWith(previousAcquisitionTask =>
                    {
                        if (previousAcquisitionTask.Result.Status.State == FlowState.Success)
                        {
                            ReportProgress(new RecipeStatus
                            {
                                State = DMTRecipeState.AcquisitionComplete,
                                Step = DMTRecipeExecutionStep.Acquisition,
                                Message = $"All acquisitions have been done for recipe {Recipe.Name}"
                            });
                        }
                    });
                }

                var allTasks =
                    new List<Task>(imageSavingTasks.Count + 6) { (Task)lastAcquisitionTask.LastAcquisitionTask };
                allTasks.AddRange(imageSavingTasks.Select(task => (Task)task));
                allTasks.AddRange(adaTasks);
                if (!(lastComputationTask is null))
                {
                    allTasks.Add(lastComputationTask.ToTask());
                }

                if (!(lastDeflectometryAcquisitionFlowTask is null))
                {
                    allTasks.Add((Task)lastDeflectometryAcquisitionFlowTask.LastAcquisitionTask);
                }

                if (!(lastSaveMaskTask is null))
                {
                    allTasks.Add(lastSaveMaskTask);
                }


                await Task.WhenAll(allTasks.Distinct().Where(task => !(task is null))).ContinueWith(whenAllTask =>
                {
                    if (allTasks.Any(task => !IsFlowTaskState(task, FlowState.Success)))
                    {
                        var exception = new TaskCanceledException();
                        if (allTasks.Any(task => IsFlowTaskState(task, FlowState.Error)))
                        {
                            exception.Data.Add(FlowExecutionErrorKey, null);
                        }
                        throw exception;
                    }
                });
                

                foreach (var screen in _hardwareManager.Screens.Values)
                    screen.Clear();

                if (!_executionCancellationTokenSource.IsCancellationRequested)
                {
                    AdaWriterFS.Close();

                    var dfSupervisor = ClassLocator.Default.GetInstance<DFSupervisor>();
                    var identity = new Identity(_pmConfiguration.ToolKey, ActorType.DEMETER,
                        _pmConfiguration.ChamberKey);
                    // Send Ada to dataflow in production mode
                    if (RemoteProductionInfo != null)
                    {                        
                        string adaFSContent = File.ReadAllText(AdaWriterFS.FileName);
                        dfSupervisor.SendAda(identity,
                            RemoteProductionInfo.ProcessedMaterial,
                            adaFSContent, AdaWriterFS.FileName);
                    }

                    var dapTokenFS = SendDataToDAP(AdaWriterFS.FileName);

                    if (!(AdaWriterBS is null))
                    {
                        AdaWriterBS.Close();
                        // Send Ada to dataflow in production mode
                        if (RemoteProductionInfo != null)
                        {
                            var adaFullPathFileName = AdaWriterBS.FileName;
                            string adaBSContent = File.ReadAllText(AdaWriterBS.FileName);
                            dfSupervisor.SendAda(identity,
                                RemoteProductionInfo.ProcessedMaterial,
                                adaBSContent, adaFullPathFileName);
                        }

                        var dapTokenBS = SendDataToDAP(AdaWriterBS.FileName);
                    }
                }

                chrono.Stop();

                _logger.Debug($"Recipe executed in {chrono.ElapsedMilliseconds}ms");
                _logger.Information("------------------------------------------------------------------");

                ReportProgress(new RecipeStatus
                {
                    Message = "Recipe execution complete",
                    State = DMTRecipeState.ExecutionComplete,
                    Step = DMTRecipeExecutionStep.Computation,
                });
            }
            catch (TaskCanceledException e)
            {
                var message = e.Data.Contains(FlowExecutionErrorKey) ? "Recipe execution failed" : "Recipe execution aborted";
                var state = e.Data.Contains(FlowExecutionErrorKey) ? DMTRecipeState.Failed : DMTRecipeState.Aborted;
                ReportProgress(new RecipeStatus
                {
                    Message = message,
                    State = state,
                    Step = DMTRecipeExecutionStep.Acquisition
                });
                ReportProgress(new RecipeStatus
                {
                    Message = message,
                    State = state,
                    Step = DMTRecipeExecutionStep.Computation,
                });
            }
            finally
            {
                foreach (var screen in _hardwareManager.Screens.Values)
                {
                    screen.Clear();
                }

                foreach (var exec in _execList)
                    exec.Dispose();

                _execList.Clear();
                _executionCancellationTokenSource.Dispose();
                if (!(_monitorTaskTimer is null))
                {
                    EndMtt(Recipe.Name + " execution");
                    EndMtt(Recipe.Name);
                    FlushMtt(Recipe.Name);
                }
            }
        }

        private static bool IsFlowTaskState(Task task, FlowState state)
        {
            switch (task)
            {
                case Task<AcquireOneImageResult> acqOneImageTask:
                    return acqOneImageTask.Result.Status.State == state;
                case Task<AcquirePhaseImagesForPeriodAndDirectionResult> acqPhaseImagesForPeriodAndDirectionTask:
                    return acqPhaseImagesForPeriodAndDirectionTask.Result.Status.State == state;
                case Task<SaveImageResult> saveImageTask:
                    return saveImageTask.Result.Status.State == state;
                case Task<CorrectorResult> correctorTask:
                    return correctorTask.Result.Status.State == state;
                case Task<AdjustCurvatureDynamicsForRawCurvatureMapResult> adjustCurvatureDynamicsForRawCurvatureMapTask:
                    return adjustCurvatureDynamicsForRawCurvatureMapTask.Result.Status.State == state;
                case Task<ComputeLowAngleDarkFieldImageResult> computeLowAngleDarkFieldImageTask:
                    return computeLowAngleDarkFieldImageTask.Result.Status.State == state;
                case Task<ComputeNanoTopoResult> computeNanoTopoTask:
                    return computeNanoTopoTask.Result.Status.State == state;
                case Task<ComputeRawCurvatureMapForPeriodAndDirectionResult> computeRawCurvatureMapForPeriodAndDirectionTask:
                    return computeRawCurvatureMapForPeriodAndDirectionTask.Result.Status.State == state;
                case Task<ComputeUnwrappedPhaseMapForDirectionResult> computeUnwrappedPhaseMapForDirectionTask:
                    return computeUnwrappedPhaseMapForDirectionTask.Result.Status.State == state;
                case Task<SaveMaskResult> saveMaskTask:
                    return saveMaskTask.Result.Status.State == state;
                default:
                    var taskStatus = state == FlowState.Error ? TaskStatus.Faulted : TaskStatus.RanToCompletion;
                    return task.Status == taskStatus;
            }
        }

        private List<DMTSingleAcquisitionFlowTask> GetRecipeSingleAcquisitionFlowTasksAndAddNecessaryCorrectorFlowTasks(List<(Side, DMTSingleAcquisitionFlowTask, DMTCorrectorFlowTask)> correctorFlowTaskTuples)
        {
            return _execList
                .Where(mExec =>
                    mExec is BrightFieldMeasureExecution ||
                    mExec is BackLightMeasureExecution ||
                    mExec is HighAngleDarkFieldMeasureExecution)
                .Select(mExec =>
                {
                    switch (mExec)
                    {
                        case BrightFieldMeasureExecution bfMExec:
                            var acqFlowTask = bfMExec.GetDMTAcquisitionFlowTask();
                            if (_applyCorrectors)
                            {
                                correctorFlowTaskTuples.Add((bfMExec.Measure.Side, acqFlowTask, bfMExec.GetDMTCalculationFlowTask()));                                                                       
                            }
                            return acqFlowTask;

                        case BackLightMeasureExecution blMExec:
                            return blMExec.GetDMTAcquisitionFlowTask();

                        case HighAngleDarkFieldMeasureExecution olMExec:
                            return olMExec
                                .GetDMTAcquisitionFlowTask();

                        default:
                            return null;
                    }
                })
                .ToList();
        }

        private bool HasBackSideMeasure()
        {
            return _execList.Exists(exec => exec.Measure.IsEnabled && exec.Measure.Side == Side.Back);
        }

        private void SendFirstRecipeState()
        {
            // CurrentStep will be incremented : the first CurrentStep will be 0
            _currentAcquisitionStep = -1;
            var recipeStatusAcquisition = new RecipeStatus
            {
                State = DMTRecipeState.Preparing,
                Message = "Preparing recipe execution",
            };
            ReportProgress(recipeStatusAcquisition);
        }

        private static void SetRecipeStatusState(FlowStatus status, RecipeStatus recipeStatus)
        {
            switch (status.State)
            {
                case FlowState.Canceled:
                    recipeStatus.State = DMTRecipeState.Aborted;
                    break;

                case FlowState.Error:
                    recipeStatus.State = DMTRecipeState.Failed;
                    break;

                case FlowState.InProgress:
                case FlowState.Success:
                    recipeStatus.State = DMTRecipeState.Executing;
                    break;
            }
        }

        private void ReportAcquisitionFlowProgress(FlowStatus status)
        {
            if (_executionCancellationTokenSource is null || _executionCancellationTokenSource.IsCancellationRequested || status.Message.IsNullOrEmpty())
            {
                return;
            }

            var recipeStatus = new RecipeStatus { Message = status.Message, Step = DMTRecipeExecutionStep.Acquisition };
            SetRecipeStatusState(status, recipeStatus);

            ReportProgress(recipeStatus);
        }

        private void ReportComputationFlowProgress(FlowStatus status)
        {
            if (_executionCancellationTokenSource is null || _executionCancellationTokenSource.IsCancellationRequested || status.Message.IsNullOrEmpty())
            {
                return;
            }

            var recipeStatus = new RecipeStatus { Message = status.Message, Step = DMTRecipeExecutionStep.Computation };
            SetRecipeStatusState(status, recipeStatus);

            ReportProgress(recipeStatus);
        }

        private void AcquireImageFlowOnStatusChanged(FlowStatus status, AcquireOneImageResult statusdata)
        {
            ReportAcquisitionFlowProgress(status);
        }

        private void AutoExposureFlowOnStatusChanged(FlowStatus status, AutoExposureResult statusdata)
        {
            var recipeStatus = new AutoExposureStatus
            {
                ExposureTimeMs = statusdata.ExposureTimeMs,
                Side = statusdata.WaferSide,
                Message = status.Message
            };
            SetRecipeStatusState(status, recipeStatus);

            ReportProgress(recipeStatus);
        }

        private void AcquirePhaseImageFlowOnStatusChanged(
            FlowStatus status, AcquirePhaseImagesForPeriodAndDirectionResult statusData)
        {
            ReportAcquisitionFlowProgress(status);
        }

        private void ComputePhaseMapFlowOnStatusChanged(
            FlowStatus status, ComputePhaseMapAndMaskForPeriodAndDirectionResult statusData)
        {
            ReportComputationFlowProgress(status);
        }

        private void ComputeRawCurvatureMapFlowOnStatusChanged(
            FlowStatus status, ComputeRawCurvatureMapForPeriodAndDirectionResult statusData)
        {
            ReportComputationFlowProgress(status);
        }

        private void AdjustCurvatureDynamicsForRawCurvatureMapFlowOnStatusChanged(
            FlowStatus status, AdjustCurvatureDynamicsForRawCurvatureMapResult statusData)
        {
            ReportComputationFlowProgress(status);
        }

        private void ComputeDarkImageFlowOnStatusChanged(FlowStatus status, ComputeLowAngleDarkFieldImageResult statusData)
        {
            ReportComputationFlowProgress(status);
        }

        private void ComputeNanoTopoFlowOnStatusChanged(FlowStatus status, ComputeNanoTopoResult statusData)
        {
            ReportComputationFlowProgress(status);
        }

        private void AcquisitionSaveImageFlowOnStatusChanged(FlowStatus status, SaveImageResult statusData)
        {
            // TODO what should we report for a saveImage flow?
        }

        private void ComputationSaveImageFlowOnStatusChanged(FlowStatus status, SaveImageResult statusData)
        {
            // TODO what should we report for a saveImage flow?
        }

        private Guid? SendDataToDAP(string adaFilePath)
        {
            try
            {
                if (DataflowID is null)
                    return null;
                var dapProxy = ClassLocator.Default.GetInstance<DapProxy>();
                var dapWriteToken = dapProxy.GetWriteToken().Result;
                var dapData = new DAPData
                {
                    Token = Guid.NewGuid(),
                    Data = adaFilePath
                };
                dapProxy.SendData(dapWriteToken, dapData);
                return dapWriteToken;
            }
            catch (Exception)
            {
                _logger.Debug("Dataflow service is not started");
            }

            return null;
        }

        private void CreateOutputDirectory(PathString outputPath, bool overwrite, PathString subfolders)
        {
            var path = outputPath;
            if (subfolders != null)
            {
                path = Path.Combine(path, subfolders);
            }

            if (!overwrite && Directory.Exists(path) && (Directory.GetFiles(path).Count() != 0 ||
                                                               Directory.GetDirectories(path).Count() != 0))
            {
                throw new IOException("Destination directory is not empty");
            }

            if (Directory.Exists(path) || File.Exists(path))
            {
                //ShellFileOperation.Delete(path, silent: true);

                // There is a bug in the Directory.Delete so if there is an exception the first time we retry
                try
                {
                    Directory.Delete(path, true);
                    Thread.Sleep(20);
                }
                catch (IOException)
                {
                    Thread.Sleep(20);
                    Directory.Delete(path, true);
                }

                // We wait until the folder is realy deleted
                // TimeOut of 20 s

                int timeOut = 1000;
                int timeOutStep = 0;
                while (Directory.Exists(path) && timeOutStep < timeOut)
                {
                    Thread.Sleep(50);
                    timeOutStep++;
                }

                if (timeOutStep == timeOut)
                    throw new Exception("Failed to clean the destination directory");
            }

            Directory.CreateDirectory(path);
        }

        private List<MeasureExecutionBase> CreateMeasureExecutionList(bool productionMode = true)
        {
            var measureExecutionList = new List<MeasureExecutionBase>(8);
            // Correctors can only be computed on bright-field images, so we need to add bright-field measures in case they are not already here
            if (_applyCorrectors)
            {
                RecipeRequiresComputation = true;
                AddBrightFieldMeasuresForCorrectorsIfNecessary(measureExecutionList);
            }

            measureExecutionList.AddRange(Recipe.Measures.Where(m => m.IsEnabled)
                                                .OrderBy(m => GetMeasureOrderKey(m))
                                                .Select(m => MeasureExecutionBase.Create(m, _logger, this, productionMode,
                                                         PmChamber.ToolId, PmChamber.Id, Recipe.Key,
                                                         Recipe.Version, Recipe.Step.Product.Id, _pmConfiguration,
                                                         _screenService,
                                                         _cameraManager, _calibrationManager, _dbRegisterAcqService, _measuresConfiguration, _flowsConfiguration)));
            measureExecutionList[measureExecutionList.Count - 1].IsLastMeasureExecution = true;
            return measureExecutionList;
        }

        private void AddBrightFieldMeasuresForCorrectorsIfNecessary(List<MeasureExecutionBase> measureExecutionList)
        {
            bool recipeHasFrontSideMeasures =
                Recipe.Measures.Exists(measure => measure.IsEnabled && measure.Side == Side.Front);
            bool recipeHasBackSideMeasures =
                Recipe.Measures.Exists(measure => measure.IsEnabled && measure.Side == Side.Back);
            if (recipeHasFrontSideMeasures)
            {
                bool hasBrightFieldMeasureForFrontSide = Recipe.Measures.Exists(measure =>
                                                                                 measure.IsEnabled &&
                                                                                 measure is BrightFieldMeasure &&
                                                                                 measure.Side == Side.Front);
                if (!hasBrightFieldMeasureForFrontSide)
                {
                    AddBrightfieldMeasureExecutionForCorrectorsToMeasureExecutionList(measureExecutionList, Side.Front);
                }
            }

            if (recipeHasBackSideMeasures)
            {
                bool hasBrightFieldMeasureForBackSide = Recipe.Measures.Exists(measure =>
                                                                                measure.IsEnabled &&
                                                                                measure is BrightFieldMeasure &&
                                                                                measure.Side == Side.Back);
                if (!hasBrightFieldMeasureForBackSide)
                {
                    AddBrightfieldMeasureExecutionForCorrectorsToMeasureExecutionList(measureExecutionList, Side.Back);
                }
            }
        }

        private void AddBrightfieldMeasureExecutionForCorrectorsToMeasureExecutionList(
            List<MeasureExecutionBase> measureExecutionList, Side side)
        {
            var brightFieldMeasure = new BrightFieldMeasure
            {
                Side = side,
                AutoExposureInitialExposureTimeMs = 150,
                AutoExposureTimeTrigger = AutoExposureTimeTrigger.OnFirstWaferOfLot
            };
            measureExecutionList.Add(new BrightFieldMeasureExecution(brightFieldMeasure, _logger, this, false,
                PmChamber.Tool.Id, PmChamber.Id, Recipe.Key, Recipe.Version, Recipe.Step.Product.Id, _pmConfiguration,
                _cameraManager, _dbRegisterAcqService,
                _measuresConfiguration.GetConfiguration<BrightFieldMeasureConfiguration>(),
                _flowsConfiguration.GetConfiguration<AutoExposureConfiguration>())
            {
                IsMeasureUsedForCorrector = true
            });
        }

        private int GetMeasureOrderKey(MeasureBase m)
        {
            int key = 0;
            switch (m)
            {
                case DeflectometryMeasure dfMeasure:
                    key += 30;
                    if (dfMeasure.Side == Side.Back)
                    {
                        key += 1;
                    }

                    break;

                default:
                    if (m.Side == Side.Back)
                    {
                        key += 10;
                    }

                    if (m is HighAngleDarkFieldMeasure)
                    {
                        key += 1;
                    }

                    break;
            }

            return key;
        }

        private AdaWriter CreateAda(Side side, string destFolderWhenNoMaterial)
        {
            string outputAdaFolder;
            string adaFileName;

            if (RemoteProductionInfo is null)
            {
                outputAdaFolder = Path.Combine(destFolderWhenNoMaterial, side.ToString());
                adaFileName = Recipe.Name;
            }
            else
            {
                var prm = new ResultPathParams
                {
                    ToolName = PmChamber.Tool.Name,
                    ToolId = PmChamber.Tool.Id,
                    ToolKey = PmChamber.Tool.ToolKey,
                    ChamberName = PmChamber.Name,
                    ChamberId = PmChamber.Id,
                    ChamberKey = PmChamber.ChamberKey,
                    JobName = RemoteProductionInfo.ProcessedMaterial.ProcessJobID,
                    //JobId = // database ID not know here
                    LotName = RemoteProductionInfo.ProcessedMaterial.LotID,
                    RecipeName = RemoteProductionInfo.DFRecipeName,
                    StartProcessDate = _recipeExecutionDateTime,
                    Slot = RemoteProductionInfo.ProcessedMaterial.SlotID,
                    // RunIter = // not known yet
                    WaferName = RemoteProductionInfo.ProcessedMaterial.WaferBaseName,
                    ResultType = (side == Side.Front ? ResultType.DMT_CurvatureX_Front : ResultType.DMT_CurvatureX_Back), // only for Side and actor Type
                                                                                                                          // Index = res.Idx,
                    ProductName = Recipe.Step.Product.Name,
                    StepName = Recipe.Step.Name
                };
                adaFileName = _adaFileNameComposer.ComposeWith(prm);
                outputAdaFolder = _adaFolderComposer.ComposeWith(prm);
            }
            // We create the output directory for the Ada if needed
            try
            {
                Directory.CreateDirectory(outputAdaFolder);
            }
            catch (Exception)
            {
                _logger.Debug($"Failed to create the Ada Output Path: {outputAdaFolder}");
            }

            var material = RemoteProductionInfo?.ProcessedMaterial;
            string adaFilePath = Path.Combine(outputAdaFolder, adaFileName + ".ada");

            var adaWriter = new AdaWriter(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey, adaFilePath);
            adaWriter.WriteInfoWafer("StartProcess", DateTime.Now.ToString("dd-MM-yyyy  HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture));

            adaWriter.WriteInfoWafer("ADCOutputDataFilePath", string.Empty); // Obsolete Only if adc is not connected to databse which cannot be done in USP
            adaWriter.WriteInfoWafer("SummaryFile", string.Empty);
            adaWriter.WriteInfoWafer("Side", side);
            adaWriter.WriteInfoWafer("CarrierStatus", (material != null) ? ((int)material.JobPosition).ToString() : JobPosition.FirstAndLast.ToString());
            adaWriter.WriteInfoWafer("WaferID", material?.WaferBaseName ?? string.Empty);
            adaWriter.WriteInfoWafer("SlotID", (material != null) ? material?.SlotID.ToString() : string.Empty);
            adaWriter.WriteInfoWafer("LoadPortID", (material != null) ? material?.LoadportID.ToString() : string.Empty);
            // voir si on affiche pas directement le step name de la recette ou un mix Product/Step
            adaWriter.WriteInfoWafer("StepID", Recipe.StepId.HasValue ? Recipe.StepId.Value.ToString() : string.Empty);
            adaWriter.WriteInfoWafer("DeviceID", material?.DeviceID ?? string.Empty);
            adaWriter.WriteInfoWafer("JobID", material?.ProcessJobID ?? string.Empty);
            adaWriter.WriteInfoWafer("LotID", material?.LotID ?? string.Empty);
            adaWriter.WriteInfoWafer("ToolRecipe", RemoteProductionInfo?.DFRecipeName ?? string.Empty); // Dataflow recipe name
            if (!(material is null))
            {
                adaWriter.WriteInfoWafer("BaseName", material.SubstrateID);
            }
            
            adaWriter.WriteInfoWafer("ADCRecipeFileName", string.Empty);

            adaWriter.WriteInfoWafer("WaferType", Recipe.Step.Product.WaferCategory.Name);
            adaWriter.WriteInfoWafer("CorrectorsEnabled", _applyCorrectors ? 1 : 0);

            return adaWriter;
        }

        public void OnResultGenerated(DMTResultGeneratedEventArgs e)
        {
            ResultGenerated?.Invoke(this, e);
        }

        public async Task<Dictionary<CurvatureImageType, ServiceImage>> BaseCurvatureDynamicsAcquisitionAsync(
            DeflectometryMeasure measure, Length waferDiameter, bool isDarkRequired = false)
        {
            if (_rawCurvatureMapsBySide is null)
            {
                InitializeCurvatureDynamicsAdjustmentFields();
            }

            var fringeManager = ClassLocator.Default.GetInstance<IFringeManager>();
            var cancellationTokenSource = new CancellationTokenSource();
            var acquisitionFlowTask =
                CreateAcquisitionFlowTask(measure, fringeManager, cancellationTokenSource, _cameraManager);
            acquisitionFlowTask.Start(cancellationTokenSource, null, null);
            var cpmXFlow = CreateComputePhaseMapFlow(measure, waferDiameter, FringesDisplacement.X);
            var cpmYFlow = CreateComputePhaseMapFlow(measure, waferDiameter, FringesDisplacement.Y);
            var cpmFlowsList = new List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow>
                               {
                                   cpmXFlow,
                                   cpmYFlow
                               };
            var crcmXFlow = CreateComputeRawCurvatureMapFlow(measure, FringesDisplacement.X);
            var crcmYFlow = CreateComputeRawCurvatureMapFlow(measure, FringesDisplacement.Y);
            var acmXFlow = CreateAdjustCurvatureDynamicsForCurvatureMapFlow(measure, FringesDisplacement.X);
            var acmYFlow = CreateAdjustCurvatureDynamicsForCurvatureMapFlow(measure, FringesDisplacement.Y);
            var acmByCrcmFlowsDictionary =
                new Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow,
                    AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                {
                    { crcmXFlow, acmXFlow },
                    { crcmYFlow, acmYFlow }
                };
            var acmFlowsDictionary =
                new Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow>
                {
                    { acmXFlow, null },
                    { acmYFlow, null }
                };
            var (computationFlowTask, darkFlow) = CreateComputationTask(measure, isDarkRequired, acquisitionFlowTask,
                                                                        cpmFlowsList, acmByCrcmFlowsDictionary,
                                                                        acmFlowsDictionary);

            var lastComputationTask = computationFlowTask.CreateAndChainComputationContinuationTasks(args => {});
            await Task.WhenAll(lastComputationTask.ToTask(), (Task)acquisitionFlowTask.LastAcquisitionTask);

            _rawCurvatureMapsBySide[measure.Side][CurvatureImageType.CurvatureMapX] =
                crcmXFlow.Result.RawCurvatureMap;
            _rawCurvatureMapsBySide[measure.Side][CurvatureImageType.CurvatureMapY] =
                crcmYFlow.Result.RawCurvatureMap;
            _curvatureMapMasksBySide[measure.Side][CurvatureImageType.CurvatureMapX] = crcmXFlow.Result.Mask;
            _curvatureMapMasksBySide[measure.Side][CurvatureImageType.CurvatureMapY] = crcmYFlow.Result.Mask;
            _darkSourcesBySide[measure.Side][FringesDisplacement.X] = cpmXFlow.Result.PsdResult;
            _darkSourcesBySide[measure.Side][FringesDisplacement.Y] = cpmYFlow.Result.PsdResult;
            return CreateResultingServiceImageDictionary(isDarkRequired, acmXFlow, acmYFlow, darkFlow);
        }

        public async Task<Dictionary<CurvatureImageType, ServiceImage>> RecalculateCurvatureDynamicsAsync(
            DeflectometryMeasure measure)
        {
            if (_rawCurvatureMapsBySide?[measure.Side][CurvatureImageType.CurvatureMapX] is null)
            {
                throw new Exception("Base acquisition hasn't been carried out, cannot recalculate curvature dynamics");
            }

            var acmXFlow = CreateAdjustCurvatureDynamicsFlow(measure, CurvatureImageType.CurvatureMapX);
            var xFlowTask =
                new FlowTask<AdjustCurvatureDynamicsForRawCurvatureMapInput,
                    AdjustCurvatureDynamicsForRawCurvatureMapResult,
                    AdjustCurvatureDynamicsForRawCurvatureMapConfiguration>(acmXFlow);
            xFlowTask.Start();

            var acmYFlow = CreateAdjustCurvatureDynamicsFlow(measure, CurvatureImageType.CurvatureMapY);
            var yFlowTask = xFlowTask.ContinueWith(acmYFlow);
            await yFlowTask;
            Dictionary<CurvatureImageType, ServiceImage> results;
            if (xFlowTask.Result.Status.State != FlowState.Success)
            {
                throw new Exception($"Adjust curvature dynamics X error : {xFlowTask.Result.Status.Message}");
            }
            if (yFlowTask.Result.Status.State != FlowState.Success)
            {
                throw new Exception($"Adjust curvature dynamics Y error : {yFlowTask.Result.Status.Message}");
            }

            using (var ccmXMil = xFlowTask.Result.CurvatureMap.ConvertToUSPImageMil())
            {
                using (var ccmYMil = yFlowTask.Result.CurvatureMap.ConvertToUSPImageMil())
                {
                    results = new Dictionary<CurvatureImageType, ServiceImage>
                              {
                                  { CurvatureImageType.CurvatureMapX, ccmXMil.ToServiceImage() },
                                  { CurvatureImageType.CurvatureMapY, ccmYMil.ToServiceImage() }
                              };
                }
            }

            return results;
        }

        public async Task<ServiceImage> RecalculateDarkDynamicsAsync(DeflectometryMeasure measure)
        {
            if (_darkSourcesBySide?[measure.Side][FringesDisplacement.X] is null)
            {
                throw new Exception("Base acquisition hasn't been carried out, cannot recalculate dark dynamics");
            }

            var darkFieldInput = new ComputeLowAngleDarkFieldImageInput(_darkSourcesBySide[measure.Side][FringesDisplacement.X],
                                                      _darkSourcesBySide[measure.Side][FringesDisplacement.Y],
                                                      measure.Fringe, measure.Fringe.Period, measure.Side,
                                                      (float)measure.DarkDynamic);
            var darkFlow = _areFlowsSimulated ? new ComputeLowAngleDarkFieldImageFlowDummy(darkFieldInput)
                                             : new ComputeLowAngleDarkFieldImageFlow(darkFieldInput);
            var darkFlowTask =
                new FlowTask<ComputeLowAngleDarkFieldImageInput, ComputeLowAngleDarkFieldImageResult,
                    ComputeLowAngleDarkFieldImageConfiguration>(darkFlow);
            darkFlowTask.Start();
            await (Task)darkFlowTask;
            ServiceImage resultImage;
            using (var darkMil = darkFlow.Result.DarkImage.ConvertToUSPImageMil())
            {
                resultImage = darkMil.ToServiceImage();
            }

            return resultImage;
        }

        private AdjustCurvatureDynamicsForRawCurvatureMapFlow CreateAdjustCurvatureDynamicsFlow(
            DeflectometryMeasure measure, CurvatureImageType imageType)
        {
            var acmXInput = new AdjustCurvatureDynamicsForRawCurvatureMapInput
            {
                DynamicsCoefficient = (float)measure.CurvatureDynamic,
                Fringe = measure.Fringe,
                FringesDisplacementDirection = FringesDisplacement.X,
                Period = measure.Fringe.Period,
                Mask = _curvatureMapMasksBySide[measure.Side][imageType],
                RawCurvatureMap = _rawCurvatureMapsBySide[measure.Side][imageType]
            };
            var acmXFlow = _areFlowsSimulated ? new AdjustCurvatureDynamicsForRawCurvatureMapFlowDummy(acmXInput)
                                             : new AdjustCurvatureDynamicsForRawCurvatureMapFlow(acmXInput);
            return acmXFlow;
        }

        private static Dictionary<CurvatureImageType, ServiceImage> CreateResultingServiceImageDictionary(
            bool isDarkRequired, AdjustCurvatureDynamicsForRawCurvatureMapFlow acmXFlow,
            AdjustCurvatureDynamicsForRawCurvatureMapFlow acmYFlow, ComputeLowAngleDarkFieldImageFlow lowAngleDarkFieldFlow)
        {
            var result = new Dictionary<CurvatureImageType, ServiceImage>(isDarkRequired ? 3 : 2);
            using (var ccmXMil = acmXFlow.Result.CurvatureMap.ConvertToUSPImageMil())
            {
                using (var ccmYMil = acmYFlow.Result.CurvatureMap.ConvertToUSPImageMil())
                {
                    result[CurvatureImageType.CurvatureMapX] = ccmXMil.ToServiceImage();
                    result[CurvatureImageType.CurvatureMapY] = ccmYMil.ToServiceImage();
                }
            }

            if (!isDarkRequired)
            {
                return result;
            }

            using (var darkMil = lowAngleDarkFieldFlow.Result.DarkImage.ConvertToUSPImageMil())
            {
                result[CurvatureImageType.Dark] = darkMil.ToServiceImage();
            }

            return result;
        }

        private (DMTDeflectometryCalculationFlowTask, ComputeLowAngleDarkFieldImageFlow) CreateComputationTask(
            DeflectometryMeasure measure, bool isDarkRequired, DMTDeflectometryAcquisitionFlowTask acquisitionFlowTask,
            List<ComputePhaseMapAndMaskForPeriodAndDirectionFlow> cpmFlowsList,
            Dictionary<ComputeRawCurvatureMapForPeriodAndDirectionFlow, AdjustCurvatureDynamicsForRawCurvatureMapFlow>
                acmByCrcmFlowsDictionary,
            Dictionary<AdjustCurvatureDynamicsForRawCurvatureMapFlow, SaveImageFlow> acmFlowsDictionary)
        {
            var computationFlowTask = new DMTDeflectometryCalculationFlowTask(acquisitionFlowTask, cpmFlowsList)
                .AddCurvatureMapFlows(acmByCrcmFlowsDictionary, acmFlowsDictionary);
            ComputeLowAngleDarkFieldImageFlow lowAngleDarkFieldFlow = null;
            if (isDarkRequired)
            {
                var darkInput =
                    new ComputeLowAngleDarkFieldImageInput(measure);
                lowAngleDarkFieldFlow = _areFlowsSimulated ? new ComputeLowAngleDarkFieldImageFlowDummy(darkInput)
                                             : new ComputeLowAngleDarkFieldImageFlow(darkInput);
                computationFlowTask.AddLowAngleDarkFieldFlows(lowAngleDarkFieldFlow, null);
            }

            return (computationFlowTask, lowAngleDarkFieldFlow);
        }

        private AdjustCurvatureDynamicsForRawCurvatureMapFlow CreateAdjustCurvatureDynamicsForCurvatureMapFlow(
            DeflectometryMeasure measure, FringesDisplacement direction)
        {
            var acmXInput = new AdjustCurvatureDynamicsForRawCurvatureMapInput
            {
                DynamicsCoefficient = (float)measure.CurvatureDynamic,
                Fringe = measure.Fringe,
                FringesDisplacementDirection = direction,
                Period = measure.Fringe.Period,
                CurvatureDynamicsCalibrationCoefficient =
                        (float)(_calibrationManager.GetCurvatureDynamicsCalibrationBySide(measure.Side)?.DynamicsCoefficient ?? 0f)
            };
            var acmXFlow = _areFlowsSimulated ? new AdjustCurvatureDynamicsForRawCurvatureMapFlowDummy(acmXInput)
                                             : new AdjustCurvatureDynamicsForRawCurvatureMapFlow(acmXInput);
            return acmXFlow;
        }

        private ComputeRawCurvatureMapForPeriodAndDirectionFlow CreateComputeRawCurvatureMapFlow(
            DeflectometryMeasure measure, FringesDisplacement direction)
        {
            var crcmXInput =
                new ComputeRawCurvatureMapForPeriodAndDirectionInput(measure, measure.Fringe.Period, direction);
            var crcmXFlow = _areFlowsSimulated ? new ComputeRawCurvatureMapForPeriodAndDirectionFlowDummy(crcmXInput)
                                              : new ComputeRawCurvatureMapForPeriodAndDirectionFlow(crcmXInput);

            return crcmXFlow;
        }

        private ComputePhaseMapAndMaskForPeriodAndDirectionFlow CreateComputePhaseMapFlow(
            DeflectometryMeasure measure, Length waferDiameter, FringesDisplacement direction)
        {
            var cpmInput =
                new ComputePhaseMapAndMaskForPeriodAndDirectionInput(measure, measure.Fringe.Period, direction)
                {
                    WaferDiameter = waferDiameter,
                    PerspectiveCalibration = _calibrationManager.GetPerspectiveCalibrationForSide(measure.Side)
                };
            var cpmFlow = _areFlowsSimulated ? new ComputePhaseMapAndMaskForPeriodAndDirectionFlowDummy(cpmInput)
                                             : new ComputePhaseMapAndMaskForPeriodAndDirectionFlow(cpmInput);
            return cpmFlow;
        }

        private DMTDeflectometryAcquisitionFlowTask CreateAcquisitionFlowTask(
            DeflectometryMeasure measure, IFringeManager fringeManager, CancellationTokenSource cancellationTokenSource,
            IDMTInternalCameraMethods internalCameraMethods)
        {
            var fringeImageForAutoExposure =
                fringeManager.GetFringeImageDict(measure.Side, measure.Fringe)[FringesDisplacement.X]
                    [measure.Fringe.Period][0];
            var aeInput = new AutoExposureInput(measure, fringeImageForAutoExposure);
            var aeFlow = _areFlowsSimulated ? new AutoExposureFlowDummy(aeInput, _hardwareManager, internalCameraMethods)
                                           : new AutoExposureFlow(aeInput, _hardwareManager, internalCameraMethods);
            var apiXInput =
                new AcquirePhaseImagesForPeriodAndDirectionInput(measure, measure.Fringe.Period,
                                                                 FringesDisplacement.X);
            var apiYInput =
                new AcquirePhaseImagesForPeriodAndDirectionInput(measure, measure.Fringe.Period,
                                                                 FringesDisplacement.Y);
            var apiXFlow = _areFlowsSimulated ?
                new AcquirePhaseImagesForPeriodAndDirectionFlowDummy(apiXInput, _hardwareManager,
                                                                internalCameraMethods,
                                                                fringeManager)
              : new AcquirePhaseImagesForPeriodAndDirectionFlow(apiXInput, _hardwareManager,
                                                                internalCameraMethods,
                                                                fringeManager);
            var apiYFlow = _areFlowsSimulated ?
                new AcquirePhaseImagesForPeriodAndDirectionFlowDummy(apiXInput, _hardwareManager,
                                                                internalCameraMethods,
                                                                fringeManager)
              : new AcquirePhaseImagesForPeriodAndDirectionFlow(apiYInput, _hardwareManager,
                                                                internalCameraMethods,
                                                                fringeManager);
            var apiFlowsList = new List<AcquirePhaseImagesForPeriodAndDirectionFlow>
                               {
                                   apiXFlow,
                                   apiYFlow
                               };
            var acquisitionFlowTask = new DMTDeflectometryAcquisitionFlowTask(aeFlow, apiFlowsList);
            return acquisitionFlowTask;
        }

        private void InitializeCurvatureDynamicsAdjustmentFields()
        {
            _rawCurvatureMapsBySide = new Dictionary<Side, Dictionary<CurvatureImageType, ImageData>>(2);
            _curvatureMapMasksBySide = new Dictionary<Side, Dictionary<CurvatureImageType, ImageData>>(2);
            _darkSourcesBySide = new Dictionary<Side, Dictionary<FringesDisplacement, PSDResult>>(2);
            foreach (Side side in Enum.GetValues(typeof(Side)))
            {
                _rawCurvatureMapsBySide[side] = new Dictionary<CurvatureImageType, ImageData>
                                                {
                                                    { CurvatureImageType.CurvatureMapX, null },
                                                    { CurvatureImageType.CurvatureMapY, null }
                                                };
                _curvatureMapMasksBySide[side] = new Dictionary<CurvatureImageType, ImageData>
                                                 {
                                                     { CurvatureImageType.CurvatureMapX, null },
                                                     { CurvatureImageType.CurvatureMapY, null }
                                                 };
                _darkSourcesBySide[side] = new Dictionary<FringesDisplacement, PSDResult>
                                           {
                                               { FringesDisplacement.X, null },
                                               { FringesDisplacement.Y, null }
                                           };
            }
        }

        internal PathString GetOutputFileName(Side side, string label)
        {
            return (side == Side.Front ? FileBasePathFS : FileBasePathBS) + $"_{label}.tif";
        }

        private void Start(Func<Task> action)
        {
            lock (_mutex)
            {
                if (IsRecipeRunning)
                {
                    throw new ApplicationException("Another recipe is already running");
                }

                IsRecipeRunning = true;
                _totalAcquisitionSteps = ComputeNumberOfAcquisitionSteps();
                _totalComputationSteps = ComputeNumberOfComputationSteps();

                Task.Run(async () =>
                {
                    try
                    {
                        ReportProgress(new RecipeStatus
                        {
                            Message = "Starting",
                            State = DMTRecipeState.Executing
                        });

                        await action.Invoke();
                        var state = _executionCancellationTokenSource.IsCancellationRequested ? DMTRecipeState.Aborted : DMTRecipeState.ExecutionComplete;
                        _logger.Information("Execution status: " + state);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Execution failed: " + ex);
                        string errorMessage = ex.InnerException?.Message != null && ex.InnerException.Message != string.Empty
                            ? ex.InnerException.Message
                            : ex.Message;
                        ReportProgress(new RecipeStatus
                        {
                            Message = "Error: " + errorMessage,
                            State = DMTRecipeState.Failed
                        });
                    }
                    finally
                    {
                        IsRecipeRunning = false;
                        _executionCancellationTokenSource.Dispose();
                        _executionCancellationTokenSource = null;
                    }
                });
            }
        }

        private void OnProgressChanged(RecipeStatus status)
        {
            ProgressChanged?.Invoke(this, status);
        }

        public void ReportProgress(RecipeStatus status)
        {
            switch (status.State)
            {
                case DMTRecipeState.Executing:
                    switch (status.Step)
                    {
                        case DMTRecipeExecutionStep.Acquisition:
                            _currentAcquisitionStep++;
                            break;

                        case DMTRecipeExecutionStep.Computation:
                            _currentComputationStep++;
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                case DMTRecipeState.AcquisitionComplete:
                    _currentAcquisitionStep = _totalAcquisitionSteps;
                    break;

                case DMTRecipeState.ExecutionComplete:
                    _currentAcquisitionStep = _totalAcquisitionSteps;
                    _currentComputationStep = _totalComputationSteps;
                    break;

                case DMTRecipeState.Preparing:
                case DMTRecipeState.Aborted:
                case DMTRecipeState.Failed:
                    // Nothing to do
                    break;

                default:
                    throw new NotImplementedException();
            }

            status.CurrentRemoteProductionInfo = RemoteProductionInfo;
            status.CurrentStep = status.Step == DMTRecipeExecutionStep.Acquisition ? _currentAcquisitionStep : _currentComputationStep;
            status.TotalSteps = status.Step == DMTRecipeExecutionStep.Acquisition ? _totalAcquisitionSteps : _totalComputationSteps;
            status.RecipeKey = Recipe.Key;

            string msg = $"step {_currentAcquisitionStep}: {status.Message}";
            if (status.State == DMTRecipeState.Failed)
                _logger.Error(msg);
            else
                _logger.Information(msg);

            OnProgressChanged(status);
        }

        private void StartMtt(string name)
        {
            _monitorTaskTimer.FreshStart();
            _monitorTaskTimer.Tag_Start(name);
        }

        private void EndMtt(string name)
        {
            _monitorTaskTimer.Tag_End(name);
        }

        private void FlushMtt(string name)
        {
            try
            {
                _monitorTaskTimer.Stop();
                
                string recipeMttFolder = Path.Combine(_logger.LogDirectory, MttReportFolderName, new PathString(Recipe.Name).RemoveInvalidFilePathCharacters("_"));
                string mttFilePath = Path.Combine(recipeMttFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".mtt");
                Directory.CreateDirectory(recipeMttFolder);

                _monitorTaskTimer.SaveMonitorCSV(mttFilePath);
                _monitorTaskTimer.Clear();
            }
            catch (Exception e)
            {
                _logger.Error("Error in monitorTaskTimer reporting : " + e.Message);
            }
        }
    }
}
