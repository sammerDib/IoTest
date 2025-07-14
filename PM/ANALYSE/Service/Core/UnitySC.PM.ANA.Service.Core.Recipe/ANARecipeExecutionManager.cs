using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.AlignmentMarks;
using UnitySC.PM.ANA.Service.Core.Autofocus;
using UnitySC.PM.ANA.Service.Core.BareWaferAlignment;
using UnitySC.PM.ANA.Service.Core.Dummy;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Alignment;
using UnitySC.PM.ANA.Service.Interface.Recipe.AlignmentMarks;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Measure.Configuration;
using UnitySC.PM.ANA.Service.Measure.Loader;
using UnitySC.PM.ANA.Service.Measure.Shared;
using UnitySC.PM.Shared;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Proxy;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Base.Export;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    // This class is dedicated to the execution of recipes.
    // Warning: only one recipe can be executed at a time.
    public class ANARecipeExecutionManager : IANARecipeExecutionManager
    {
        private const string MttReportFolderName = "MTT";
        private const string RecipesResultFolderName = "RunResults";

        private readonly ILogger _logger;
        private readonly AnaHardwareManager _hardwareManager;
        private readonly IReferentialManager _referentialManager;
        private readonly PMConfiguration _pmConfiguration;
        private readonly Mapper _mapper;
        private readonly MeasuresConfiguration _measuresConfiguration;
        private readonly MeasureLoader _measureLoader;
        private readonly DbRecipeServiceProxy _dbRecipeServiceProxy;
        private readonly DbRegisterResultServiceProxy _dbRegisterResultServiceProxy;
        private readonly DbToolServiceProxy _dbToolServiceProxy;

        private RemoteProductionInfo _automationInfo;
        private bool _useResultDatabase => _automationInfo != null;

        private bool FlowsAreSimulated => ClassLocator.Default.GetInstance<IPMServiceConfigurationManager>().FlowsAreSimulated;
        private CancellationTokenSource _recipeCancellationTokenSource;
        private ANARecipe _lastRecipe;
        private ANARecipe _runningRecipe;
        private bool _isRecipePaused;
        private int _nbMeasuredPoints = 0;
        private int _nbPointsToMeasure = 0;
        private List<RecipeMeasure> _recipeMeasuresToExecute = null;
        private ResultState _lastResultStateError = ResultState.Error;


        private DateTime _recipeExecutionStartTime;
        private readonly ANARecipeExecutionManagerFDCProvider _executionManagerFDCProvider;

        /// <summary>
        /// key => measureName
        /// </summary>
        private Dictionary<string, MetroResult> _currentResults;

        /// <summary>
        /// key => measureName
        /// </summary>
        private Dictionary<string, ResultFoldersPath> _resultsFolderPaths;

        private readonly MetroDisplay _resultDisplay = new MetroDisplay();

        public delegate void ProgressingEventHandler(RecipeProgress progress);

        public event ProgressingEventHandler Progressing;

        public delegate void MeasureResultEventHandler(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex);

        public event MeasureResultEventHandler MeasureResult;

        private IMessenger _messenger;

        public ANARecipeExecutionManager(ILogger logger, AnaHardwareManager hardwareManager,
            IReferentialManager referentialManager, PMConfiguration pmConfiguration)
        {
            _logger = logger;
            _hardwareManager = hardwareManager;
            _referentialManager = referentialManager;
            _pmConfiguration = pmConfiguration;
            _mapper = ClassLocator.Default.GetInstance<Mapper>();
            _measureLoader = new MeasureLoader(ClassLocator.Default.GetInstance<ILogger<MeasureLoader>>());
            _measuresConfiguration = ClassLocator.Default.GetInstance<MeasuresConfiguration>();
            _dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            _dbRegisterResultServiceProxy = ClassLocator.Default.GetInstance<DbRegisterResultServiceProxy>();
            _dbToolServiceProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();
            _executionManagerFDCProvider = ClassLocator.Default.GetInstance<ANARecipeExecutionManagerFDCProvider>();
        }

        #region Recipe Execution

        /// <summary>
        /// Run a recipe with all its measurements
        /// </summary>
        /// <param name="recipe">Recipe that will be ran</param>
        /// <param name="automationInfo"></param>
        /// <param name="nbRuns">if nbRuns > 1, it means that Repeta is Used</param>
        /// <exception cref="InvalidOperationException">When another recipe is running (only one recipe can be executed at a time)</exception>
        /// <exception cref="Exception">When given recipe has no step</exception>
        public Dictionary<string, MetroResult> Execute(ANARecipe recipe, RemoteProductionInfo automationInfo = null, int nbRuns = 1)
        {
            ThrowIfARecipeIsAlreadyRunning();

            if (recipe.Points.IsEmpty())
            {
                _runningRecipe = recipe;
                NotifyRecipeExecutionSucceeded();
                _logger.Warning("No point found in given recipe");
                var emptyResult = new Dictionary<string, MetroResult>();
                return emptyResult;
            }

            var recipeWithExecContext = new ANARecipeWithExecContext()
            {
                Recipe = recipe
            };

            if (automationInfo != null)
            {
                recipeWithExecContext.JobId = automationInfo.ProcessedMaterial.ProcessJobID;
                recipeWithExecContext.DFRecipeName = automationInfo.DFRecipeName;
                recipeWithExecContext.PMStartRecipeTime = automationInfo.ModuleStartRecipeTime;
            }

            _messenger.Send(new RecipeStartedMessage() { Recipe = recipeWithExecContext });

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(recipe.Name);
            }

            try
            {
                InitializeRecipeVariables(recipe, automationInfo, nbRuns);
                InitializeMetroResults();
                
                InitializeProbeCalibration(); // fait un reset des calibration -- to do handle calibration LiseHF qui ne doivent pas être reset

                ProcessRecipePreparation();

                PrepareMeasureExecution();

                ProcessRecipeMeasures(nbRuns);

                SaveAndSendResults_WithSuccess(automationInfo);
                // TODO When the measure srategy optimizes the route, the results must be sorted in order to be in the same order as selected by the user
                //if ((recipe.Execution.Strategy == MeasurementStrategy.PerMeasurementTypeOptimized) || (recipe.Execution.Strategy == MeasurementStrategy.PerPointoptimized))
                //    SortResultsInUserOrder(_currentResults, _runningRecipe);
                NotifyRecipeExecutionSucceeded();

                return _currentResults;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed Recipe : {ex.Message}");
                SaveAndSendResults_WithErrors(automationInfo);
                if (!_recipeCancellationTokenSource.IsCancellationRequested)                
                    NotifyRecipeExecutionFailed();
                
                return _currentResults;
            }
            finally
            {
                if (_recipeCancellationTokenSource.IsCancellationRequested)
                {
                    _logger.Verbose("OnRecipeStopped");
                    NotifyRecipeStopped();
                }
                _lastRecipe = _runningRecipe;
                _runningRecipe = null;

                if (_recipeMeasuresToExecute != null)
                {
                    _recipeMeasuresToExecute.Clear();
                    _recipeMeasuresToExecute = null;
                }

                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    EndMtt(recipe.Name);
                    FlushMtt(recipe.Name);
                }
                _recipeCancellationTokenSource.Dispose();
            }
        }

        private void ThrowIfARecipeIsAlreadyRunning()
        {
            if (_runningRecipe != null)
            {
                NotifyRecipeExecutionFailed();
                throw new InvalidOperationException(
                    "Cannot execute multiple recipes simultaneously, one recipe is already running");
            }
        }

        private void InitializeRecipeVariables(ANARecipe recipe, RemoteProductionInfo automationInfo, int nbRuns)
        {
            _lastResultStateError = ResultState.Error;
            _recipeCancellationTokenSource = new CancellationTokenSource();
            _executionManagerFDCProvider.StartRecipeTimer();
            _recipeExecutionStartTime = DateTime.Now;
            _automationInfo = automationInfo;
            _runningRecipe = recipe;
            _nbMeasuredPoints = 0;
            _nbPointsToMeasure = GetNbPointsToMeasure(recipe, nbRuns);
        }

        private void InitializeMetroResults()
        {
            List<RecipeMeasure> recipeMeasures = GetRecipeMeasuresToExecute();
            var dies = _runningRecipe.WaferHasDies ? _runningRecipe.Dies : null;
            InitMetroResults(recipeMeasures, dies);

            _recipeMeasuresToExecute = recipeMeasures; // to avoid 2nd call of GetRecipeMeasuresToExecute
        }

        private void InitializeProbeCalibration()
        {
            foreach (var probe in _hardwareManager.Probes)
            {
                var probeBase = (probe.Value as IProbe);
                if (probeBase.CalibrationManager != null)
                {
                    probeBase.CalibrationManager.RecipeExecutionStarted();
                    probeBase.CalibrationManager.CancellationToken = _recipeCancellationTokenSource.Token;
                }
            }
        }

        private void PrepareMeasureExecution()
        {
            if (!_recipeCancellationTokenSource.IsCancellationRequested)
            {
                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    StartMtt(_runningRecipe.Name + "PrepMeasureExecution");
                }
                List<RecipeMeasure> recipeMeasures = _recipeMeasuresToExecute; // avoid a second call to GetRecipeMeasuresToExecute

                var origindummypt = new MeasurePoint(-1, 0.0, 0.0, false);
                var diedummy = new DieIndex(0, 0);

                foreach (var recipe in recipeMeasures)
                {
                    if (_recipeCancellationTokenSource.IsCancellationRequested)
                        break;

                    // all recipe here are active (sort done in GetRecipeMeasuresToExecute)
                    var currentResultFoldersPath = _resultsFolderPaths[recipe.Settings.Name];
                    var measureContext = new MeasureContext(origindummypt, diedummy, currentResultFoldersPath);
                    if (!recipe.Measure.PrepareExecution(recipe.Settings, measureContext, _recipeCancellationTokenSource.Token))
                    {
                        recipe.Settings.IsActive = false; // to do check si on desactive la measure ou si on mets la recette en erreur et on aura des resulat partiel
                        _logger.Error($"Error in Measure [{recipe.Settings.Name}] Preparation - Measure is disabled");

                        // Add a NotMeasure point due to disabled measurement
                        var metroResult = _currentResults[recipe.Settings.Name];
                        metroResult.MeasureResult.Dies = null;
                        metroResult.MeasureResult.DiesMap = null;
                        metroResult.MeasureResult.Points = new List<MeasurePointResult>(){
                            recipe.Measure.CreateNotMeasuredEmptyResult($"Measure has been disabled after Prepare Execution failure")
                        };

                    }
                }

                //We remove disabled measure from prepare and update _recipeMeasuresToExecute
                var activeMeasureList = recipeMeasures.Where(recipe => recipe.Settings.IsActive).ToList();
                _recipeMeasuresToExecute = activeMeasureList;

                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    EndMtt(_runningRecipe.Name + "PrepMeasureExecution");
                }
            }
        }

        private void ProcessRecipeMeasures(int nbRuns)
        {
            if (!_recipeCancellationTokenSource.IsCancellationRequested)
            {
                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    StartMtt(_runningRecipe.Name + "Execution");
                }

                List<RecipeMeasure> recipeMeasures = _recipeMeasuresToExecute; // avoid a second call to GetRecipeMeasuresToExecute

                var sorter = new ANARecipeMeasurePointsSorter();

                // TODO : we should handle the optimizeDiesRoute, optimizePointsRoute, routePerDie booleans on client side and
                // these parameters should be saved in recipe file.
                bool optimizeDiesRoute;
                bool optimizePointsRoute;
                bool routePerDie;
                if (recipeMeasures.Count == 1 && recipeMeasures[0].Settings.MeasureType == MeasureType.XYCalibration)
                {
                    optimizeDiesRoute = false;
                    optimizePointsRoute = false;
                    routePerDie = true;
                }
                else
                {
                    switch (_runningRecipe.Execution.Strategy)
                    {
                        case MeasurementStrategy.PerMeasurementType:
                        case MeasurementStrategy.PerPoint:
                            optimizeDiesRoute = false;
                            optimizePointsRoute = false;
                            routePerDie = true;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }

                var sortedPoints = sorter.SortRecipeMeasurePoints(_runningRecipe, recipeMeasures, optimizeDiesRoute, optimizePointsRoute, routePerDie);

                PerformMeasuresAndFinalizeMetroResults(recipeMeasures, sortedPoints, nbRuns);

                if (!_recipeCancellationTokenSource.IsCancellationRequested)
                {
                    _lastResultStateError = ResultState.Ok; // no error anymore
                }

                if (_pmConfiguration.MonitorTaskTimerIsEnable)
                {
                    EndMtt(_runningRecipe.Name + "Execution");
                }

                // Measurement has been done - clean our list
                recipeMeasures.Clear();
                _recipeMeasuresToExecute = null;
            }
        }

        private List<RecipeMeasure> GetRecipeMeasuresToExecute()
        {
            var measuresToExecute = _runningRecipe.Measures.Where(m => m.IsActive).ToList();
            var recipeMeasures = new List<RecipeMeasure>();
            foreach (var measureSettings in measuresToExecute)
            {
                IMeasure measure = _measureLoader.GetMeasure(measureSettings.MeasureType);
                if (measure is null)
                {
                    _logger.Error($"Cannot get the measure {measureSettings.MeasureType}: it is not loaded.");
                    continue;
                }

                var measurePointsIDs = new List<int>();
                if (measureSettings.SubMeasurePoints != null && measureSettings.SubMeasurePoints.Count > 0)
                {
                    measurePointsIDs.Add(measureSettings.SubMeasurePoints[0]);
                }
                else
                {
                    measurePointsIDs.AddRange(measureSettings.MeasurePoints);
                }
                recipeMeasures.Add(new RecipeMeasure
                {
                    Measure = measure,
                    Settings = measureSettings,
                    MeasurePointIds = new HashSet<int>(measurePointsIDs)
                });
            }
            return recipeMeasures;
        }

        private void PerformMeasuresAndFinalizeMetroResults(List<RecipeMeasure> recipeMeasures, List<RecipeSortedPoints> recipeSortedPoints, int nbRuns)
        {
            var lastRecipeMeasures = GetLastRecipeMeasures(recipeSortedPoints);

            recipeMeasures.ForEach(rm => rm.Settings.NbOfRepeat = nbRuns);

            for (int iRepeatIndex = 0; iRepeatIndex < nbRuns; iRepeatIndex++)
            {
                PerformMeasuresAndFinalizeMetroResultsForOneRepeat(recipeMeasures, recipeSortedPoints, lastRecipeMeasures, nbRuns, iRepeatIndex);

                if (_recipeCancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }
            }
        }

        private void PerformMeasuresAndFinalizeMetroResultsForOneRepeat(List<RecipeMeasure> recipeMeasures, List<RecipeSortedPoints> recipeSortedPoints, Dictionary<string, RecipeSortedPoints> lastRecipeMeasures, int nbRuns, int iRepeatIndex)
        {
            for (int i = 0; i < recipeSortedPoints.Count; i++)
            {
                var sortedPoint = recipeSortedPoints[i];
                var measurePoint = _runningRecipe.Points.Find(p => p.Id == sortedPoint.PointId);
                var recipeMeasure = recipeMeasures.Find(m => m.Settings.Name == sortedPoint.MeasureName);

                var die = sortedPoint.DieIndex;
                if (_isRecipePaused)
                {
                    NotifyRecipePaused();
                    // We wait for the resume or the cancel
                    while (_isRecipePaused && (!_recipeCancellationTokenSource.IsCancellationRequested))
                    {
                        Thread.Sleep(200);
                    }
                }

                if (_recipeCancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                if (!recipeMeasure.Settings.IsMeasureWithSubMeasurePoints)
                {
                    NotifyMeasuringProgress(new MeasurePointInfo
                    {
                        MeasureName = recipeMeasure.Settings.Name,
                        Die = die,
                        PointDataIndex = measurePoint.Id,
                        NbOfRepeat = nbRuns,
                        RepeatIndex = iRepeatIndex + 1
                    });
                    _nbMeasuredPoints++;
                }

                var subMeasurePoints = _runningRecipe.Points.Where(p => recipeMeasure.Settings.SubMeasurePoints.Contains(p.Id)).ToList();
                MeasureOnPoint(sortedPoint.DieIndex, measurePoint, recipeMeasure, subMeasurePoints, iRepeatIndex, nbRuns);

                if (lastRecipeMeasures.TryGetValue(sortedPoint.MeasureName, out var lastMeasure) && lastMeasure.Equals(sortedPoint))
                {
                    recipeMeasure.Measure.MeasureTerminatedInRecipe(recipeMeasure.Settings);
                }
            }

            if (_recipeCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            FinalizeMetroResults(recipeMeasures);
        }

        private void MeasureOnPoint(DieIndex die, MeasurePoint point, RecipeMeasure recipeMeasure, List<MeasurePoint> subMeasurePoints, int repeatIndex, int nbRuns)
        {
            var currentResultFoldersPath = _resultsFolderPaths[recipeMeasure.Settings.Name];
            var measureContext = new MeasureContext(point, die, currentResultFoldersPath);

            if (subMeasurePoints != null && subMeasurePoints.Count > 0)
            {
                var subResults = new List<MeasurePointResult>();
                foreach(var subMeasurePoint in subMeasurePoints)
                {
                    var subMeasureContext = new MeasureContext(subMeasurePoint, die, currentResultFoldersPath);
                    if (repeatIndex != 0)
                    {
                        subMeasureContext = subMeasureContext.ConvertToMeasureContextRepeat(repeatIndex);
                    }

                    NotifySubMeasureStarted(new MeasurePointInfo
                    {
                        MeasureName = recipeMeasure.Settings.Name + " point",
                        Die = null,
                        PointDataIndex = subMeasurePoint.Id,
                        Position = subMeasurePoint.Position.ToXYPosition(new WaferReferential()),
                        NbOfRepeat = nbRuns,
                        RepeatIndex = repeatIndex + 1
                    });
                    _nbMeasuredPoints++;

                    var subResult = recipeMeasure.Measure.ExecuteSubMeasure(recipeMeasure.Settings, subMeasureContext, _recipeCancellationTokenSource.Token);
                    SubMeasureFinished(subResult);

                    subResults.Add(subResult);
                    AddMetroResult(subResult, subMeasureContext, recipeMeasure.Settings.Name);
                }

                NotifyComputeMeasureFromSubMeasures(new MeasurePointInfo
                {
                    MeasureName = recipeMeasure.Settings.Name,
                    Die = die,
                    PointDataIndex = 0,
                    Position = new XYPosition(new WaferReferential(), 0, 0),
                    NbOfRepeat = nbRuns,
                    RepeatIndex = repeatIndex + 1
                });
                _nbMeasuredPoints++;
                if (repeatIndex != 0)
                {
                    measureContext = measureContext.ConvertToMeasureContextRepeat(repeatIndex);
                }
                var result = recipeMeasure.Measure.ComputeMeasureFromSubMeasures(recipeMeasure.Settings, measureContext, subResults, _recipeCancellationTokenSource.Token);
                AddMetroResult(result, measureContext, recipeMeasure.Settings.Name);
                OnOnePointMeasured(result, currentResultFoldersPath.RecipeFolderPath, die);
            }
            else
            {
                if (repeatIndex != 0)
                {
                    measureContext = measureContext.ConvertToMeasureContextRepeat(repeatIndex);
                }
                var result = recipeMeasure.Measure.Execute(recipeMeasure.Settings, measureContext, _recipeCancellationTokenSource.Token);
                AddMetroResult(result, measureContext, recipeMeasure.Settings.Name);
                OnOnePointMeasured(result, currentResultFoldersPath.RecipeFolderPath, die);
            }
        }

        private Dictionary<string, RecipeSortedPoints> GetLastRecipeMeasures(List<RecipeSortedPoints> recipeSortedPoints)
        {
            var lastMeasures = new Dictionary<string, RecipeSortedPoints>();
            foreach (var point in recipeSortedPoints)
            {
                lastMeasures[point.MeasureName] = point;
            }
            return lastMeasures;
        }

        #endregion Recipe Execution

        #region UsedByRecipeService

        public void SaveCurrentResultInDatabase(string lotName)
        {
            DatabaseIds databaseIds = GetDatabaseIds(_lastRecipe);
            string recipeName = new PathString(_lastRecipe.Name).RemoveInvalidFilePathCharacters("_");
            SaveCurrentResultInDatabase(recipeName, lotName, recipeName, _lastRecipe.Step.Product.Name, 1, databaseIds);
        }

        public ANARecipe Convert_RecipeToAnaRecipe(DataAccess.Dto.Recipe dbrecipe)
        {
            if (dbrecipe == null)
            {
                return null;
            }
            var recipe = _mapper.AutoMap.Map<ANARecipe>(dbrecipe);

            Type targetTypeT = typeof(MeasureBase<,>);
            var typesFound = GetTypesFromAssemblies(targetTypeT);

            var version = new Version(GetSettingVersionFromXML(dbrecipe.XmlContent));
            foreach (var measureSettings in recipe.Measures)
            {
                foreach (var typeFound in typesFound)
                {
                    var methodInfo = typeFound.GetMethod("ConvertSetting", BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo != null)
                    {
                        var parameters = methodInfo.GetParameters();
                        if (parameters.Length == 3 &&
                            parameters[0].ParameterType == typeof(MeasureSettingsBase) &&
                            parameters[1].ParameterType == typeof(Version) &&
                            parameters[2].ParameterType == typeof(Version))
                        {
                            methodInfo.Invoke(null, new object[] { measureSettings, version, new Version(ANARecipe.CurrentFileVersion) });
                        }
                    }
                }
            }
            return recipe;
        }

        private List<Type> GetTypesFromAssemblies(Type targetType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("UnitySC.PM.ANA.Service.Measure")).ToList();
            var typesFound = types.SelectMany(assembly => assembly.GetTypes())
                          .Where(t => t.IsClass && !t.IsAbstract &&
                                      t.BaseType != null && t.BaseType.IsGenericType &&
                                      t.BaseType.GetGenericTypeDefinition() == targetType
                          ).ToList();
            return typesFound;

        }

        private string GetSettingVersionFromXML(string xmlContent)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            string res = "0";
            XmlNode rootNode = xmlDoc.DocumentElement;
            if (rootNode != null)
            {
                XmlAttribute fileVersionAttribute = rootNode.Attributes["FileVersion"];
                if (fileVersionAttribute != null)
                {
                    res = fileVersionAttribute.Value;
                }
            }
            return res;
        }

        public DataAccess.Dto.Recipe Convert_AnaRecipeToRecipe(ANARecipe anaRecipe)
        {
            if (anaRecipe == null)
            {
                return null;
            }

            var recipe = _mapper.AutoMap.Map<DataAccess.Dto.Recipe>(anaRecipe);
            return recipe;
        }

        public TimeSpan GetEstimatedExecutionTime(ANARecipe recipe, int nbRuns)
        {
            double estimatedTimeInMs = 0;
            foreach (var measureSettings in recipe.Measures.Where(m => m.IsActive))
            {
                var measure = _measureLoader.GetMeasure(measureSettings.MeasureType);
                // Measures with sub measures display an extra point during recipe run which contains measure global results
                var nbPoints = measureSettings.MeasurePoints.Count + (measureSettings.IsMeasureWithSubMeasurePoints ? 1 : 0);
                if (!(measure is null))
                {
                    double measureEstimatedTimeInMs = measure.GetEstimatedTime(measureSettings).TotalMilliseconds * nbPoints;

                    // Measures with sub measures don't rely on dies
                    if (recipe.WaferHasDies && !measureSettings.IsMeasureWithSubMeasurePoints)
                    {
                        measureEstimatedTimeInMs *= recipe.Dies.Count;
                    }
                    if (nbRuns > 1)
                    {
                        measureEstimatedTimeInMs *= nbRuns;
                    }
                    estimatedTimeInMs += measureEstimatedTimeInMs;
                }
            }

            return TimeSpan.FromMilliseconds(estimatedTimeInMs);
        }

        #endregion UsedByRecipeService

        #region Recipe Preparation

        private void ProcessRecipePreparation()
        {
            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                StartMtt(_runningRecipe.Name + "Preparation");
            }

            var parameters = _runningRecipe.Execution?.Alignment;
            var settings = _runningRecipe.Alignment;
            if (parameters is null || settings is null)
            {
                _lastResultStateError = ResultState.PreparationError;
                throw new Exception("Recipe could not be execute without Alignment settings");
            }

            // waferReferentialSettings will by fed by Autofocus/BWA/MarkAlignment,
            // Please do not change function order, each one need the previous version with updates.
            var waferReferentialSettings = CreateWaferReferential(parameters);

            var lightIntensity = settings.AutoLight.LightIntensity;
            SetMainLightIntensity(lightIntensity);

            FindFocusPositionAndUpdateWaferReferential(ref waferReferentialSettings);

            // calculate IN PRODUCTION  the main camera pixel mean of the actual scene in the current context
            _executionManagerFDCProvider.CreateFDCCameraMeanPixel();

            if (parameters.RunBwa)
            {
                RunBwaAndUpdateWaferReferential(ref waferReferentialSettings);
            }

            if (parameters.RunMarkAlignment)
            {
                RunMarkAlignmentAndUpdateWaferReferential(ref waferReferentialSettings);
            }

            if (_runningRecipe.WaferMap != null)
            {
                ApplyDieReferential();
            }

            if (_pmConfiguration.MonitorTaskTimerIsEnable)
            {
                EndMtt(_runningRecipe.Name + "Preparation");
            }
        }

        private WaferReferentialSettings CreateWaferReferential(AlignmentParameters parameters)
        {
            var waferReferentialSettings = new WaferReferentialSettings();
            if (parameters.RunBwa || parameters.RunMarkAlignment)
            {
                _referentialManager.DeleteSettings(ReferentialTag.Wafer);
            }
            else
            {
                waferReferentialSettings = _referentialManager.GetSettings(ReferentialTag.Wafer) as WaferReferentialSettings ?? waferReferentialSettings;
            }

            return waferReferentialSettings;
        }

        private void SetMainLightIntensity(double intensity)
        {
            var mainLight = _hardwareManager.GetMainLight();
            mainLight.SetIntensity(intensity);
        }

        private void FindFocusPositionAndUpdateWaferReferential(ref WaferReferentialSettings waferReferentialSettings)
        {
            var parameters = _runningRecipe.Execution.Alignment;
            var settings = _runningRecipe.Alignment;
            var newZTop = settings.AutoFocusLise.ZTopFocus;
            var objectiveId = settings.AutoFocusLise.LiseObjectiveContext?.ObjectiveId ?? _hardwareManager.GetMainObjectiveOfMainCamera().DeviceID;

            if (parameters.RunAutoFocus || parameters.RunMarkAlignment || parameters.RunBwa)
            {
                // Select objective
                var objectiveConfig = _hardwareManager.GetObjectiveConfigs().Find(_ => _.DeviceID == objectiveId);
                var objectiveSelector = _hardwareManager.GetObjectiveSelectorOfObjective(objectiveId);
                objectiveSelector.SetObjective(objectiveConfig);
            }

            if (parameters.RunAutoFocus && !settings.AutoFocusLise.ZIsDefinedByUser)
            {
                NotifyAlignmentProgressing(RecipeProgressState.AutoFocusInProgress, "Auto focus in progress");

                var afResult = ApplyAutoFocus(settings.AutoFocusLise);
                newZTop = afResult.ZPosition.Millimeters();
            }
            GoToZTopFocusPosition(newZTop);

            waferReferentialSettings.ZTopFocus = newZTop;
            waferReferentialSettings.ObjectiveIdForTopFocus = objectiveId;
            _referentialManager.SetSettings(waferReferentialSettings);
        }

        private void GoToZTopFocusPosition(Length zTop)
        {
            if (_recipeCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            var newPos = new XYZTopZBottomPosition(new StageReferential(), double.NaN, double.NaN, zTop.Millimeters, double.NaN);
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, newPos, AxisSpeed.Normal);
        }

        private void RunBwaAndUpdateWaferReferential(ref WaferReferentialSettings waferReferentialSettings)
        {
            if (_recipeCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            NotifyAlignmentProgressing(RecipeProgressState.EdgeAlignmentInProgress, "Bare wafer alignment in progress");

            var waferCharacteristics = _runningRecipe.Step?.Product.WaferCategory.DimentionalCharacteristic;
            var result = AlignBareWafer(waferCharacteristics, _runningRecipe.Alignment.BareWaferAlignment);
            if (result.Status.State != FlowState.Success)
            {
                _lastResultStateError = ResultState.AlignmentError;
                throw new Exception($"Bare wafer alignment failed");
            }
            // add shift from zero 
            waferReferentialSettings.ShiftX = ((BareWaferAlignmentResult)result).ShiftStageX;
            waferReferentialSettings.ShiftY = ((BareWaferAlignmentResult)result).ShiftStageY;
            waferReferentialSettings.WaferAngle = ((BareWaferAlignmentResult)result).Angle;
            _referentialManager.SetSettings(waferReferentialSettings);
        }

        private void RunMarkAlignmentAndUpdateWaferReferential(ref WaferReferentialSettings waferReferentialSettings)
        {
            if (_recipeCancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            NotifyAlignmentProgressing(RecipeProgressState.MarkAlignmentInProgress, "Alignment Marks in progress");

            var result = AlignMarks(_runningRecipe.AlignmentMarks);
            if (result.Status.State != FlowState.Success)
            {
                _lastResultStateError = ResultState.WaferAlignmentError;
                throw new Exception($"AlignMarks failed");
            }
            //  need to add shift to actual wafer ref shift
            waferReferentialSettings.ShiftX += result.ShiftX;
            waferReferentialSettings.ShiftY += result.ShiftY;
            waferReferentialSettings.WaferAngle += result.RotationAngle;
            _referentialManager.SetSettings(waferReferentialSettings);
        }

        private void ApplyDieReferential()
        {
            var _waferMap = _runningRecipe.WaferMap.WaferMapData;
            _referentialManager.SetSettings(new DieReferentialSettings(_waferMap.RotationAngle, _waferMap.DieDimensions, _waferMap.DieGridTopLeft, _waferMap.DiesPresence));
        }

        private AFLiseResult ApplyAutoFocus(AutoFocusLiseParameters autoFocusLiseParameters)
        {
            // Global autofocus is run on wafer center
            var zTopFocus = autoFocusLiseParameters.ZTopFocus.Millimeters;
            var centerPosition = new XYZTopZBottomPosition(new StageReferential(), 0, 0, zTopFocus, double.NaN);
            var probeLiseUpId = _hardwareManager.GetProbeLiseUp().DeviceID;
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, centerPosition);

            AFLiseInput input = new AFLiseInput(probeLiseUpId);
            if (autoFocusLiseParameters.LiseParametersAreDefinedByUser)
            {
                input.Gain = autoFocusLiseParameters.LiseGain;
                input.ZPosScanRange = new ScanRange(
                    autoFocusLiseParameters.ZMin.Millimeters,
                    autoFocusLiseParameters.ZMax.Millimeters
                );
            }

            var afLiseFlow = FlowsAreSimulated ? new AFLiseFlowDummy(input) : new AFLiseFlow(input);
            afLiseFlow.CancellationToken = _recipeCancellationTokenSource.Token;

            var result = afLiseFlow.Execute();
            if (result.Status.State != FlowState.Success)
            {
                _lastResultStateError = ResultState.PreparationError;
                throw new Exception($"AutoFocus failed");
            }

            return result;
        }

        private BareWaferAlignmentChangeInfo AlignBareWafer(WaferDimensionalCharacteristic waferCharacteristics, BareWaferAlignmentParameters parameters)
        {
            var mainCameraId = _hardwareManager.GetMainCamera().DeviceID;
            var input = new BareWaferAlignmentInput()
            {
                Wafer = waferCharacteristics,
                CameraId = mainCameraId,
                InitialContext = parameters.ObjectiveContext
            };

            if (parameters.CustomImagePositions.Any())
            {
                
                // Edge Position should be expressed in stage reférentials due to chuck center position that can be not equal to zero 
                var topEdge = parameters.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Top);
                if (!(topEdge is null))
                {
                    var manualPosition = new XYPosition(new StageReferential(), topEdge.X.Millimeters, topEdge.Y.Millimeters);
                    input.ImageTop = new ServiceImageWithPosition(null, manualPosition);
                }

                var rightEdge = parameters.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Right);
                if (!(rightEdge is null))
                {
                    var manualPosition = new XYPosition(new StageReferential(), rightEdge.X.Millimeters, rightEdge.Y.Millimeters);
                    input.ImageRight = new ServiceImageWithPosition(null, manualPosition);
                }

                var bottomEdge = parameters.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Bottom);
                if (!(bottomEdge is null))
                {
                    var manualPosition = new XYPosition(new StageReferential(), bottomEdge.X.Millimeters, bottomEdge.Y.Millimeters);
                    input.ImageBottom = new ServiceImageWithPosition(null, manualPosition);
                }

                var leftEdge = parameters.CustomImagePositions.FirstOrDefault(cip => cip.EdgePosition == Service.Interface.Algo.WaferEdgePositions.Left);
                if (!(leftEdge is null))
                {
                    var manualPosition = new XYPosition(new StageReferential(), leftEdge.X.Millimeters, leftEdge.Y.Millimeters);
                    input.ImageLeft = new ServiceImageWithPosition(null, manualPosition);
                }
            }

            var bwaFlow = FlowsAreSimulated ? new BareWaferAlignmentFlowDummy(input) : new BareWaferAlignmentFlow(input);
            bwaFlow.CancellationToken = _recipeCancellationTokenSource.Token;

            var result = bwaFlow.Execute();
            if (result.Status.State != FlowState.Success)
            {
                _lastResultStateError = ResultState.WaferAlignmentError;
                throw new Exception("BWA failed.");
            }

            return result;
        }

        private AlignmentMarksResult AlignMarks(AlignmentMarksSettings settings)
        {
            var input = new AlignmentMarksInput()
            {
                AutoFocusSettings = settings.AutoFocus,
                Site1Images = settings.AlignmentMarksSite1,
                Site2Images = settings.AlignmentMarksSite2,
                InitialContext = settings.ObjectiveContext
            };

            var alignmentMarksFlow = FlowsAreSimulated ? new AlignmentMarksFlowDummy(input) : new AlignmentMarksFlow(input);
            alignmentMarksFlow.CancellationToken = _recipeCancellationTokenSource.Token;

            var result = alignmentMarksFlow.Execute();
            if (result.Status.State != FlowState.Success)
            {
                _lastResultStateError = ResultState.AlignmentError;
                throw new Exception("Mark alignment failed.");
            }

            return result;
        }

        #endregion Recipe Preparation

        #region Monitor task timer

        private void StartMtt(string name)
        {
            var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
            mtt.FreshStart();
            mtt.Tag_Start(name);
        }

        private void EndMtt(string name)
        {
            var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
            mtt.Tag_End(name);
        }

        private void FlushMtt(string recipeName)
        {
            try
            {
                var mtt = ClassLocator.Default.GetInstance<UnitySC.Shared.Tools.MonitorTasks.MonitorTaskTimer>();
                mtt.Stop();

                string recipeMttFolder = Path.Combine(_logger.LogDirectory, MttReportFolderName, new PathString(recipeName).RemoveInvalidFilePathCharacters("_"));
                string mttFilePath = Path.Combine(recipeMttFolder, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".mtt");
                Directory.CreateDirectory(recipeMttFolder);

                mtt.SaveMonitorCSV(mttFilePath);
                mtt.Clear();
            }
            catch (Exception ex)
            {
                _logger.Error("Error in monitorTaskTimer reporting : " + ex.Message);
            }
        }

        #endregion Monitor task timer

        #region Results

        private sealed class DatabaseIds
        {
            public DatabaseIds(int chamberId, int recipeId, int productId, int toolId)
            {
                ChamberId = chamberId;
                RecipeId = recipeId;
                ProductId = productId;
                ToolId = toolId;
            }

            public int ChamberId { get; }
            public int RecipeId { get; }
            public int ProductId { get; }
            public int ToolId { get; }
        }

        private void SaveAndSendResults_WithSuccess(RemoteProductionInfo automationInfo)
        {
            if (_measuresConfiguration.LocalBackupOfAllResults || _useResultDatabase)
            {
                foreach (var key in _currentResults.Keys)
                {
                    bool statsDone = false;
                    string fileName;
                    if (_useResultDatabase)
                    {
                        _logger.Verbose("Use result database ResultFileName = {0} ResType = {1}", _resultsFolderPaths[key].PreRegisterResult.ResultFileName, _currentResults[key].ResType.GetExt());
                        fileName = $"{_resultsFolderPaths[key].PreRegisterResult.ResultFileName}.{_currentResults[key].ResType.GetExt()}";
                    }
                    else
                    {
                        _logger.Verbose("NOT Use result database ExternalFileFolderName = {0} ResType = {1}", _resultsFolderPaths[key].ExternalFileFolderName, _currentResults[key].ResType.GetExt());
                        fileName = $"Result{_resultsFolderPaths[key].ExternalFileFolderName}.{_currentResults[key].ResType.GetExt()}";
                    }

                    string filePath = Path.Combine(@"..\", _resultsFolderPaths[key].RecipeFolderPath, fileName);
                    _logger.Debug("filePath = {0}", filePath);
                    _currentResults[key].Serialize(filePath);

                    if (_useResultDatabase)
                    {
                        UpdateDatabaseMeasureResultState(key, ResultState.Ok);
                    }

                    if (_pmConfiguration.AutoExtractResultCSVIsEnable)
                    {
                        _currentResults[key].ResFilePath = filePath;
                        if (!statsDone)
                        {
                            _currentResults[key].MeasureResult.FillNonSerializedProperties(true, true);
                            statsDone = true;
                        }

                        _logger.Verbose($"Auto Export CSV : {_currentResults[key].ResFilePath}_Data.csv");
                        var exportQuery = new ExportQuery
                        {
                            FilePath = Path.GetDirectoryName(filePath),
                            AdditionalExports = new List<string>() { MetroExportResult.CsvExport }
                        };
                        _resultDisplay.ExportResult.Export(exportQuery, _currentResults[key]);
                    }

                    if (automationInfo != null)
                    {
                        // DVID will be sent need to perform stats if not already done
                        if (!statsDone)
                        {
                            _currentResults[key].MeasureResult.FillNonSerializedProperties(true, true);
                            statsDone = true;
                        }
                    }
                }
                if (_useResultDatabase)
                {
                    SendDVIDs();
                }
            }

            _executionManagerFDCProvider.StopRecipeTimer();
            _executionManagerFDCProvider.CreateFDCAvgExecutionTime();
        }

        private void SortResultsInUserOrder(Dictionary<string, MetroResult> currentResults, ANARecipe runningRecipe)
        {
            foreach (var currentResult in _currentResults)
            {
                if (currentResult.Value.MeasureResult.Points != null)
                {
                    currentResult.Value.MeasureResult.Points.Sort((x, y) => x.SiteId.CompareTo(y.SiteId));
                }

                if (currentResult.Value.MeasureResult.Dies != null)
                {
                    // Sort the dies
                    if ((_runningRecipe.Dies != null) && _currentResults.Count > 0)
                        currentResult.Value.MeasureResult.Dies.Sort((x, y) => CompareDies(x, y));
                    foreach (var dieResult in currentResult.Value.MeasureResult.Dies)
                    {
                        dieResult.Points.Sort((x, y) => x.SiteId.CompareTo(y.SiteId));
                    }
                }

            }
        }

        private int CompareDies(MeasureDieResult mdr1, MeasureDieResult mdr2)
        {
            int mdr1RowIndex = DieIndexConverter.ConvertRowFromDieReference(mdr1.RowIndex, _currentResults.First().Value.MeasureResult.DiesMap.DieReferenceRowIndex);
            int mdr1ColumnIndex = DieIndexConverter.ConvertColumnToDieReference(mdr1.ColumnIndex, _currentResults.First().Value.MeasureResult.DiesMap.DieReferenceColumnIndex);
            int mdr2RowIndex = DieIndexConverter.ConvertRowFromDieReference(mdr2.RowIndex, _currentResults.First().Value.MeasureResult.DiesMap.DieReferenceRowIndex);
            int mdr2ColumnIndex = DieIndexConverter.ConvertColumnToDieReference(mdr2.ColumnIndex, _currentResults.First().Value.MeasureResult.DiesMap.DieReferenceColumnIndex);

            var indexMdr1 = _runningRecipe.Dies.FindIndex(x => x.Column == mdr1ColumnIndex && x.Row == mdr1RowIndex);
            var indexMdr2 = _runningRecipe.Dies.FindIndex(x => x.Column == mdr2ColumnIndex && x.Row == mdr2RowIndex);
            return indexMdr1.CompareTo(indexMdr2);
        }

        private void SaveAndSendResults_WithErrors(RemoteProductionInfo automationInfo)
        {
            if (_currentResults != null && _useResultDatabase)
            {
                foreach (var key in _currentResults.Keys)
                {
                    if (!_resultsFolderPaths.ContainsKey(key))
                    {
                        // pre registering should have failed we did not have a path to copy this result
                        _logger.Warning($"Skip Save Result [{key}] due to error = {_lastResultStateError} since Path is unknown");
                        continue;
                    }

                    if (_currentResults[key].MeasureResult.State > GlobalState.Error)
                    {
                        // Some points exist so save some result anyway
                        string fileName;
                        _logger.Verbose("Use result database ResultFileName = {0} ResType = {1}", _resultsFolderPaths[key].PreRegisterResult.ResultFileName, _currentResults[key].ResType.GetExt());
                        fileName = $"{_resultsFolderPaths[key].PreRegisterResult.ResultFileName}.{_currentResults[key].ResType.GetExt()}";
                        string filePath = Path.Combine(@"..\", _resultsFolderPaths[key].RecipeFolderPath, fileName);
                        _logger.Debug("filePath = {0}", filePath);
                        _currentResults[key].Serialize(filePath);

                        UpdateDatabaseMeasureResultState(key, ResultState.Partial);
                    }
                    else
                    {
                        UpdateDatabaseMeasureResultState(key, _lastResultStateError);
                    }
                }
            }
        }

        private void UpdateDatabaseMeasureResultState(string measureNamekey, ResultState resultState)
        {
            if (_resultsFolderPaths[measureNamekey].PreRegisterResult?.InternalDBResItemId > 0)
            {
                _logger.Debug($"UpdateResultState InternalDBResItemId  = {_resultsFolderPaths[measureNamekey].PreRegisterResult.InternalDBResItemId} ResultState = {resultState}");
                _dbRegisterResultServiceProxy.UpdateResultState(_resultsFolderPaths[measureNamekey].PreRegisterResult.InternalDBResItemId, resultState);
            }
        }

        private void SaveCurrentResultInDatabase(string jobName, string lotName, string tCRecipeName, string waferBaseName, int slotId, DatabaseIds databaseIds)
        {
            _logger.Debug("SaveCurrentResultInDatabase");

            Dictionary<ResultType, int> resultTypeIndex = new Dictionary<ResultType, int>();
            OutPreRegister outPreRegister = null;
            foreach (var res in _currentResults)
            {
                if (resultTypeIndex.ContainsKey(res.Value.ResType))
                {
                    resultTypeIndex[res.Value.ResType]++;
                }
                else
                {
                    resultTypeIndex[res.Value.ResType] = 0;
                }
                InPreRegister inPreregister;
                if (outPreRegister == null)
                {
                    inPreregister = new InPreRegister(res.Value.ResType, jobName, lotName, tCRecipeName, databaseIds.ToolId, databaseIds.ChamberId, databaseIds.ProductId, databaseIds.RecipeId, waferBaseName, slotId, (byte)resultTypeIndex[res.Value.ResType], DataAccess.Dto.ModelDto.Enum.ResultFilterTag.Engineering);
                }
                else
                {
                    inPreregister = new InPreRegister(res.Value.ResType, outPreRegister.InternalDBResId, (byte)resultTypeIndex[res.Value.ResType], DataAccess.Dto.ModelDto.Enum.ResultFilterTag.Engineering);
                }
                outPreRegister = _dbRegisterResultServiceProxy.PreRegisterResult(inPreregister);

                if (outPreRegister == null)
                {
                    throw new Exception("Impossible to save current Result in database");
                }

                if (!Directory.Exists(outPreRegister.ResultPathRoot))
                    Directory.CreateDirectory(outPreRegister.ResultPathRoot);

                // Serialize result
                if (outPreRegister.RunIter != 0)
                {
                    res.Value.MeasureResult.UpdateIteration(outPreRegister.RunIter);
                }
                res.Value.Serialize(Path.Combine(outPreRegister.ResultPathRoot, $"{outPreRegister.ResultFileName}.{res.Value.ResType.GetExt()}"));

                // copy external files
                if (outPreRegister.RunIter == 0)
                {
                    DirectoryTools.CopyDirectory(Path.Combine(_resultsFolderPaths[res.Key].RecipeFolderPath, _resultsFolderPaths[res.Key].ExternalFileFolderName), Path.Combine(outPreRegister.ResultPathRoot, _resultsFolderPaths[res.Key].ExternalFileFolderName), true);
                }
                else
                {
                    string RunIterPath = $"{Path.Combine(outPreRegister.ResultPathRoot, _resultsFolderPaths[res.Key].ExternalFileFolderName)}\\RunIter{outPreRegister.RunIter:00}";
                    DirectoryTools.CopyDirectory(Path.Combine(_resultsFolderPaths[res.Key].RecipeFolderPath, _resultsFolderPaths[res.Key].ExternalFileFolderName), RunIterPath, true);
                }

                // Change result state
                _dbRegisterResultServiceProxy.UpdateResultState(outPreRegister.InternalDBResItemId, _lastResultStateError);
            }
        }

        private void AddMetroResult(MeasurePointResult measurePointResult, MeasureContext measureContext, string measureName)
        {
            var metroResult = _currentResults[measureName];
            var isRepeta = (measureContext is MeasureContextRepeat);

            if (measureContext.DieIndex != null)
            {
                // Convert index in result referential
                int rowIndex = DieIndexConverter.ConvertRowToDieReference(measureContext.DieIndex.Row, metroResult.MeasureResult.DiesMap.DieReferenceRowIndex);
                int columnIndex = DieIndexConverter.ConvertColumnToDieReference(measureContext.DieIndex.Column, metroResult.MeasureResult.DiesMap.DieReferenceColumnIndex);
                var currentDie = metroResult.MeasureResult.Dies.SingleOrDefault(x => x.RowIndex == rowIndex && x.ColumnIndex == columnIndex);
                if (currentDie == null)
                {
                    throw new InvalidOperationException($"Error during add result in die. Unknow die in result row:{currentDie.RowIndex} column:{currentDie.ColumnIndex}");
                }

                if (isRepeta)
                {
                    SearchPointAndAddDataAfter(currentDie.Points, measurePointResult);
                }
                else
                {
                    currentDie.Points.Add(measurePointResult);
                }
            }
            else
            {
                if (isRepeta)
                {
                    SearchPointAndAddDataAfter(metroResult.MeasureResult.Points, measurePointResult);
                }
                else
                {
                    metroResult.MeasureResult.Points.Add(measurePointResult);
                }
            }

            // If cancellation requested, global result could be incomplete
            if (_recipeCancellationTokenSource.IsCancellationRequested)
            {
                metroResult.MeasureResult.State = GlobalState.Partial;
            }
            else
            {
                metroResult.MeasureResult.State = GlobalState.Success;
            }
        }

        private void SearchPointAndAddDataAfter(List<MeasurePointResult> measurePointResults, MeasurePointResult measurePointResult)
        {
            int iMeasurePointResult = measurePointResults.FindIndex(p => p.SiteId == measurePointResult.SiteId);
            if (iMeasurePointResult == -1)
            {
                measurePointResults.Add(measurePointResult);
            }
            else
            {
                measurePointResults[iMeasurePointResult].Datas.AddRange(measurePointResult.Datas);
            }
        }

        private void InitMetroResults(List<RecipeMeasure> recipeMeasures, List<DieIndex> dies)
        {
            // Parent Result PrimaryKey ID in Database Table Result 
            long parentMeasure_DBId = -1;

            _currentResults = new Dictionary<string, MetroResult>();
            _resultsFolderPaths = new Dictionary<string, ResultFoldersPath>();
            WaferMap diesMap = null;
            if (dies != null)
            {
                diesMap = CreateMetroDiesMap();
            }

            var recipeDate = DateTime.Now;
            RemoteProductionResultInfo automResultInfo = null;
            if (_useResultDatabase)
            {
                automResultInfo = new RemoteProductionResultInfo(_automationInfo);
            }

            Dictionary<MeasureType, int> measureTypeIndex = new Dictionary<MeasureType, int>();

            // Create metro result
            foreach (var mesure in recipeMeasures)
            {
                if ((!(mesure.Settings.IsActive)) || (!(mesure.Settings.IsConfigured)))
                {
                    continue;
                }

                string resultFolderName = mesure.Settings.MeasureType.ToString();
                if (measureTypeIndex.ContainsKey(mesure.Settings.MeasureType))
                {
                    measureTypeIndex[mesure.Settings.MeasureType]++;
                    resultFolderName += measureTypeIndex[mesure.Settings.MeasureType].ToString();
                }
                else
                {
                    measureTypeIndex[mesure.Settings.MeasureType] = 0;
                }

                if (_useResultDatabase) // use database result folder
                {
                    OutPreRegister outPreRegister;
                    if (parentMeasure_DBId == -1)
                    {
                        outPreRegister = _dbRegisterResultServiceProxy.PreRegisterResult(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey, _runningRecipe, _automationInfo,
                                                                        mesure.Settings.MeasureType.GetResultType(), (byte)measureTypeIndex[mesure.Settings.MeasureType]);
                        if (outPreRegister == null)
                        {
                            _lastResultStateError = ResultState.DatabaseError;
                            throw new Exception($"Impossible to initialize current Result <{mesure.Settings.Name}> in database - Check Dataaccess / DataBase Connection status");
                        }

                        parentMeasure_DBId = outPreRegister.InternalDBResId;
                    }
                    else
                    {
                        outPreRegister = _dbRegisterResultServiceProxy.PreRegisterResult_SameParent(parentMeasure_DBId, _automationInfo.ModuleStartRecipeTime,
                                                                         mesure.Settings.MeasureType.GetResultType(), (byte)measureTypeIndex[mesure.Settings.MeasureType]);
                        if (outPreRegister == null)
                        {
                            _lastResultStateError = ResultState.DatabaseError;
                            throw new Exception($"Impossible to initialize current Result <{mesure.Settings.Name}> in database from its parents - Check Dataaccess / DataBase Connection status");
                        }
                    }

                    if (!Directory.Exists(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName)))
                    {
                        Directory.CreateDirectory(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName));
                    }

                    if (outPreRegister.InternalDBResId != parentMeasure_DBId)
                    {
                        _logger.Warning($"outPreRegister.InternalDBResId differs from Parent measure DBid for <{mesure.Settings.Name}> - should be the same");  // warning message to say that something weird happens in databas probable that initial table or the way we handle result and resulitme has changed otherwise somebody has broken registering ;)
                    }

                    var resultFoldersPath = new ResultFoldersPath();
                    resultFoldersPath.PreRegisterResult = outPreRegister;
                    resultFoldersPath.RecipeFolderPath = outPreRegister.ResultPathRoot;
                    resultFoldersPath.ExternalFileFolderName = resultFolderName;
                    if (outPreRegister.RunIter != 0)
                    {
                        resultFolderName = Path.Combine(resultFolderName, $"RunIter{outPreRegister.RunIter:00}");
                        if (!Directory.Exists(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName)))
                        {
                            Directory.CreateDirectory(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName));
                        }
                        resultFoldersPath.ExternalFileFolderName = resultFolderName;
                    }
                    resultFoldersPath.ExternalFilePrefix = $"S{_automationInfo.ProcessedMaterial.SlotID:00}_";
                    _resultsFolderPaths.Add(mesure.Settings.Name, resultFoldersPath);

                    _logger.Debug($"Preregister done : Measure={mesure.Settings.Name}");
                    _logger.Verbose($"Preregister done : path={outPreRegister.ResultPathRoot}");
                    _logger.Verbose($"Preregister done : filename={outPreRegister.ResultFileName}");
                }
                else // local result folder
                {
                    _resultsFolderPaths.Add(mesure.Settings.Name, CreateResultFolderPath(resultFolderName, recipeDate));
                }

                var metroResult = new MetroResult(mesure.Settings.MeasureType.GetResultType());
                metroResult.ResFilePath = Path.Combine(_resultsFolderPaths[mesure.Settings.Name].RecipeFolderPath, $"InMemory.{mesure.Settings.MeasureType.GetResultType().GetExt()}");
                metroResult.MeasureResult = mesure.Measure.CreateMetroMeasureResult(mesure.Settings);
                metroResult.MeasureResult.Name = mesure.Settings.Name;
                metroResult.MeasureResult.AutomationInfo = automResultInfo;
                metroResult.MeasureResult.Wafer = _runningRecipe.Step?.Product.WaferCategory.DimentionalCharacteristic;

                if (!(metroResult.MeasureResult.Wafer is null) && metroResult.MeasureResult.Wafer.DiameterTolerance is null)
                {
                    metroResult.MeasureResult.Wafer.DiameterTolerance = 0.Micrometers();
                }

                if (!RecipeHelper.IsMeasureInWaferReferential(mesure, _runningRecipe.Points))
                {
                    metroResult.MeasureResult.DiesMap = diesMap;
                }

                // Create dies
                if (dies != null && !RecipeHelper.IsMeasureInWaferReferential(mesure, _runningRecipe.Points))
                {
                    metroResult.MeasureResult.Dies = new List<MeasureDieResult>();
                    foreach (var die in dies)
                    {
                        var dieResult = mesure.Measure.CreateMetroDieResult();
                        dieResult.RowIndex = DieIndexConverter.ConvertRowToDieReference(die.Row, diesMap.DieReferenceRowIndex);
                        dieResult.ColumnIndex = DieIndexConverter.ConvertColumnToDieReference(die.Column, diesMap.DieReferenceColumnIndex);
                        metroResult.MeasureResult.Dies.Add(dieResult);
                    }
                }
                else
                {
                    metroResult.MeasureResult.Points = new List<MeasurePointResult>();
                }

                _currentResults.Add(mesure.Settings.Name, metroResult);
            }
        }

        [Obsolete]
        private void InitMetroResults_Old(List<RecipeMeasure> recipeMeasures, List<DieIndex> dies)
        {
            // Create it before loop to avoid multiple calls to DB
            DatabaseIds databaseIds = _useResultDatabase ? GetDatabaseIds(_runningRecipe) : null;

            _currentResults = new Dictionary<string, MetroResult>();
            _resultsFolderPaths = new Dictionary<string, ResultFoldersPath>();
            WaferMap diesMap = null;
            if (dies != null)
            {
                diesMap = CreateMetroDiesMap();
            }

            var recipeDate = DateTime.Now;
            RemoteProductionResultInfo automResultInfo = null;
            if (_useResultDatabase)
            {
                automResultInfo = new RemoteProductionResultInfo()
                {
                    DFRecipeName = _automationInfo.DFRecipeName,
                    PMRecipeName = _automationInfo.ModuleRecipeName,
                    ProcessJobID = _automationInfo.ProcessedMaterial.ProcessJobID,
                    LotID = _automationInfo.ProcessedMaterial.LotID,
                    CarrierID = _automationInfo.ProcessedMaterial.CarrierID,
                    SlotID = _automationInfo.ProcessedMaterial.SlotID,
                    StartRecipeTime = _automationInfo.ModuleStartRecipeTime
                };
            }

            Dictionary<MeasureType, int> measureTypeIndex = new Dictionary<MeasureType, int>();

            // Create metro result
            foreach (var mesure in recipeMeasures)
            {
                if ((!(mesure.Settings.IsActive)) || (!(mesure.Settings.IsConfigured)))
                {
                    continue;
                }

                string resultFolderName = mesure.Settings.MeasureType.ToString();
                if (measureTypeIndex.ContainsKey(mesure.Settings.MeasureType))
                {
                    measureTypeIndex[mesure.Settings.MeasureType]++;
                    resultFolderName += measureTypeIndex[mesure.Settings.MeasureType].ToString();
                }
                else
                {
                    measureTypeIndex[mesure.Settings.MeasureType] = 0;
                }

                if (_useResultDatabase) // use database result folder
                {
                    var inPreregister = new InPreRegister(mesure.Settings.MeasureType.GetResultType(), _automationInfo.ProcessedMaterial.ProcessJobID, _automationInfo.ProcessedMaterial.LotID, _automationInfo.DFRecipeName, databaseIds.ToolId, databaseIds.ChamberId, databaseIds.ProductId, databaseIds.RecipeId, _automationInfo.ProcessedMaterial.WaferBaseName, _automationInfo.ProcessedMaterial.SlotID, (byte)measureTypeIndex[mesure.Settings.MeasureType]);
                    if (_automationInfo.ModuleStartRecipeTime > DateTime.MinValue)
                    {
                        inPreregister.DateTimeRun = _automationInfo.ModuleStartRecipeTime;
                    }
                    var outPreRegister = _dbRegisterResultServiceProxy.PreRegisterResult(inPreregister);
                    if (outPreRegister == null)
                    {
                        _lastResultStateError = ResultState.DatabaseError;
                        throw new Exception("Impossible to initialize current Result in database");
                    }

                    if (!Directory.Exists(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName)))
                    {
                        Directory.CreateDirectory(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName));
                    }
                    var resultFoldersPath = new ResultFoldersPath();
                    resultFoldersPath.PreRegisterResult = outPreRegister;
                    resultFoldersPath.RecipeFolderPath = outPreRegister.ResultPathRoot;
                    resultFoldersPath.ExternalFileFolderName = resultFolderName;
                    if (outPreRegister.RunIter != 0)
                    {
                        resultFolderName = Path.Combine(resultFolderName, $"RunIter{outPreRegister.RunIter:00}");
                        if (!Directory.Exists(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName)))
                        {
                            Directory.CreateDirectory(Path.Combine(outPreRegister.ResultPathRoot, resultFolderName));
                        }
                        resultFoldersPath.ExternalFileFolderName = resultFolderName;
                    }
                    resultFoldersPath.ExternalFilePrefix = $"S{_automationInfo.ProcessedMaterial.SlotID:00}_";
                    _resultsFolderPaths.Add(mesure.Settings.Name, resultFoldersPath);

                    _logger.Debug($"Preregister done : path={outPreRegister.ResultPathRoot}");
                    _logger.Debug($"Preregister done : filename={outPreRegister.ResultFileName}");
                    _logger.Debug($"Preregister done : dateruntime ={inPreregister.DateTimeRun}");
                    _logger.Debug($"Preregister done : jobname={inPreregister.JobName}");
                    _logger.Debug($"Preregister done : jobid={inPreregister.JobId}");
                }
                else // local result folder
                {
                    _resultsFolderPaths.Add(mesure.Settings.Name, CreateResultFolderPath(resultFolderName, recipeDate));
                }

                var metroResult = new MetroResult(mesure.Settings.MeasureType.GetResultType());
                metroResult.ResFilePath = Path.Combine(_resultsFolderPaths[mesure.Settings.Name].RecipeFolderPath, $"InMemory.{mesure.Settings.MeasureType.GetResultType().GetExt()}");
                metroResult.MeasureResult = mesure.Measure.CreateMetroMeasureResult(mesure.Settings);
                metroResult.MeasureResult.Name = mesure.Settings.Name;
                metroResult.MeasureResult.AutomationInfo = automResultInfo;
                metroResult.MeasureResult.Wafer = _runningRecipe.Step?.Product.WaferCategory.DimentionalCharacteristic;

                if (!(metroResult.MeasureResult.Wafer is null) && metroResult.MeasureResult.Wafer.DiameterTolerance is null)
                {
                    metroResult.MeasureResult.Wafer.DiameterTolerance = 0.Micrometers();
                }

                if (!RecipeHelper.IsMeasureInWaferReferential(mesure, _runningRecipe.Points))
                {
                    metroResult.MeasureResult.DiesMap = diesMap;
                }

                // Create dies
                if (dies != null && !RecipeHelper.IsMeasureInWaferReferential(mesure, _runningRecipe.Points))
                {
                    metroResult.MeasureResult.Dies = new List<MeasureDieResult>();
                    foreach (var die in dies)
                    {
                        var dieResult = mesure.Measure.CreateMetroDieResult();
                        dieResult.RowIndex = DieIndexConverter.ConvertRowToDieReference(die.Row, diesMap.DieReferenceRowIndex);
                        dieResult.ColumnIndex = DieIndexConverter.ConvertColumnToDieReference(die.Column, diesMap.DieReferenceColumnIndex);
                        metroResult.MeasureResult.Dies.Add(dieResult);
                    }
                }
                else
                {
                    metroResult.MeasureResult.Points = new List<MeasurePointResult>();
                }

                _currentResults.Add(mesure.Settings.Name, metroResult);
            }
        }

        private WaferMap CreateMetroDiesMap()
        {
            var metroWaferMap = new WaferMap();
            var anaWaferMap = _runningRecipe.WaferMap.WaferMapData;
            metroWaferMap.RotationAngle = anaWaferMap.RotationAngle;
            metroWaferMap.DieSizeWidth = anaWaferMap.DieDimensions.DieWidth;
            metroWaferMap.DieSizeHeight = anaWaferMap.DieDimensions.DieHeight;
            metroWaferMap.DiePitchWidth = metroWaferMap.DieSizeWidth + anaWaferMap.DieDimensions.StreetWidth;
            metroWaferMap.DiePitchHeight = metroWaferMap.DieSizeHeight + anaWaferMap.DieDimensions.StreetHeight;
            metroWaferMap.DieGridTopLeftXPosition = new Length(anaWaferMap.DieGridTopLeft.X, LengthUnit.Millimeter);
            metroWaferMap.DieGridTopLeftYPosition = new Length(anaWaferMap.DieGridTopLeft.Y, LengthUnit.Millimeter);
            metroWaferMap.DieReferenceColumnIndex = anaWaferMap.DieReference.Column;
            metroWaferMap.DieReferenceRowIndex = anaWaferMap.DieReference.Row;
            metroWaferMap.SetDiesPresences(anaWaferMap.DiesPresence.Values);
            return metroWaferMap;
        }

        private ResultFoldersPath CreateResultFolderPath(string externalFileFolderName, DateTime recipeDate)
        {
            try
            {
                var foldersResult = new ResultFoldersPath();
                foldersResult.ExternalFileFolderName = externalFileFolderName;
                string recipeResultFolderPath = Path.Combine(_pmConfiguration.LocalCacheResultFolderPath, RecipesResultFolderName);
                var recipeName = new PathString(_runningRecipe.Name).RemoveInvalidFilePathCharacters("_");
                foldersResult.RecipeFolderPath = Path.Combine(recipeResultFolderPath, recipeName + recipeDate.ToString("yyMMdd-HHmmss-ff"));
                Directory.CreateDirectory(Path.Combine(foldersResult.RecipeFolderPath, foldersResult.ExternalFileFolderName));
                return foldersResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during create recipe result folder path");
                throw new Exception("Error during create recipe result folder path", ex);
            }
        }

        private DatabaseIds GetDatabaseIds(ANARecipe recipe)
        {
            if (recipe.Step == null)
            {
                _lastResultStateError = ResultState.Error;
                throw new Exception("Step undefined in given recipe");
            }

            var chamber = _dbToolServiceProxy.GetChamber(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey);

            return new DatabaseIds(
                chamberId: chamber.Id,
                recipeId: _dbRecipeServiceProxy.GetRecipeId(recipe.Key, recipe.Version),
                productId: recipe.Step.ProductId,
                toolId: chamber.ToolId
            );
        }

        private void FinalizeMetroResults(List<RecipeMeasure> recipeMeasures)
        {
            foreach (var measure in recipeMeasures)
            {
                measure.Measure.ApplyMeasureCorrection(_currentResults[measure.Settings.Name].MeasureResult, measure.Settings);
                measure.Measure.FinalizeMetroResult(_currentResults[measure.Settings.Name].MeasureResult, measure.Settings);
            }
        }

        #endregion Results

        #region DVIDs

        private void SendDVIDs()
        {
            try
            {
                _logger.Debug("Generation of the DVIDs");
                // TODO retrieve the ProcessJobStartTime
                ANADataCollection dataCollection = new ANADataCollection()
                {
                    SlotID = _automationInfo.ProcessedMaterial.SlotID,
                    LoadportID = _automationInfo.ProcessedMaterial.LoadportID,
                    LotID = _automationInfo.ProcessedMaterial.LotID,
                    ControlJobID = _automationInfo.ProcessedMaterial.ControlJobID,
                    ProcessJobID = _automationInfo.ProcessedMaterial.ProcessJobID,
                    ProcessStartTime = _recipeExecutionStartTime,
                    ProcessEndTime = DateTime.Now,
                    CarrierID = _automationInfo.ProcessedMaterial.CarrierID,
                    SubstrateID = _automationInfo.ProcessedMaterial.SubstrateID,
                    AcquiredID = _automationInfo.ProcessedMaterial.AcquiredID,
                    RecipeID = _automationInfo.ModuleRecipeName
                };

                // Generate DVID Data
                dataCollection.WaferMeasureData = GenerateMeasureDVIDs(_currentResults);
                dataCollection.WaferStatistics = GenerateWaferStatsDVIDs(_currentResults);
                dataCollection.AllDiesStatistics = GenerateDiesStatsDVIDs(_currentResults);

                var chamber = ClassLocator.Default.GetInstance<DbToolServiceProxy>().GetChamber(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey);
                var dfSupervisor = ClassLocator.Default.GetInstance<DFSupervisor>();

                var identity = new Identity(chamber.ToolId, _pmConfiguration.Actor, chamber.Id);
                _logger.Debug("Send the DVIDs");
                dfSupervisor.NotifyDataCollectionChanged(identity, dataCollection);
            }
            catch (Exception)
            {
                _logger.Error("Failed to send the DVIDs");
            }
        }

        private DVIDWaferStatistics GenerateWaferStatsDVIDs(Dictionary<string, MetroResult> currentResults)
        {
            DVIDWaferStatistics dvidWaferStatistics = new DVIDWaferStatistics() { Name = "Global wafer statistics", WaferStatistics = new List<DCWaferStatisticsForMeasure>() };

            foreach (var currentResult in currentResults)
            {
                DCWaferStatisticsForMeasure curWaferStatsticsForMeasure = new DCWaferStatisticsForMeasure();
                curWaferStatsticsForMeasure.MeasureName = currentResult.Key;
                var measureSettings = _runningRecipe.Measures.Find(m => m.Name == currentResult.Value.MeasureResult.Name);
                var measure = _measureLoader.GetMeasure(measureSettings.MeasureType);
                if (measure is IMeasureDCProvider measureDCProvider)
                {
                    curWaferStatsticsForMeasure.WaferStatisticsForMeasure = measureDCProvider.GetDCWaferStatistics(currentResult.Value.MeasureResult, measureSettings);
                    dvidWaferStatistics.WaferStatistics.Add(curWaferStatsticsForMeasure);
                }
            }
            return dvidWaferStatistics;
        }

        private DVIDAllDiesStatistics GenerateDiesStatsDVIDs(Dictionary<string, MetroResult> currentResults)
        {
            DVIDAllDiesStatistics dvidDiesStatistics = new DVIDAllDiesStatistics() { Name = "Dies Statistics", DiesStatistics = new List<DCDiesStatisticsForMeasure>() };

            foreach (var currentResult in currentResults)
            {
                DCDiesStatisticsForMeasure curDiesStatsticsForMeasure = new DCDiesStatisticsForMeasure();
                curDiesStatsticsForMeasure.MeasureName = currentResult.Key;
                var measureSettings = _runningRecipe.Measures.Find(m => m.Name == currentResult.Value.MeasureResult.Name);
                var measure = _measureLoader.GetMeasure(measureSettings.MeasureType);
                if (measure is IMeasureDCProvider measureDCProvider)
                {
                    curDiesStatsticsForMeasure.DiesStatisticsForMeasure = measureDCProvider.GetDCDiesStatistics(currentResult.Value.MeasureResult, measureSettings);
                    dvidDiesStatistics.DiesStatistics.Add(curDiesStatsticsForMeasure);
                }
            }
            return dvidDiesStatistics;
        }

        private DVIDWaferMeasureData GenerateMeasureDVIDs(Dictionary<string, MetroResult> currentResults)
        {
            DVIDWaferMeasureData dvidWaferMeasureData = new DVIDWaferMeasureData() { Name = "Points measures", WaferMeasuresData = new List<DCPointMeasureDataForMeasure>() };

            foreach (var currentResult in currentResults)
            {
                DCPointMeasureDataForMeasure curPointMeasureDataForMeasure;
                curPointMeasureDataForMeasure = dvidWaferMeasureData.WaferMeasuresData.FirstOrDefault(a => a.MeasureName == currentResult.Key);
                if (curPointMeasureDataForMeasure is null)
                {
                    curPointMeasureDataForMeasure = new DCPointMeasureDataForMeasure() { MeasureName = currentResult.Key };
                    dvidWaferMeasureData.WaferMeasuresData.Add(curPointMeasureDataForMeasure);
                    curPointMeasureDataForMeasure.WaferMeasuresDataForMeasure = new List<DCPointMeasureData>();
                }

                var measureSettings = _runningRecipe.Measures.Find(m => m.Name == currentResult.Value.MeasureResult.Name);
                var measure = _measureLoader.GetMeasure(measureSettings.MeasureType);
                var measureDCProvider = measure as IMeasureDCProvider;
                if (measureDCProvider is null)
                {
                    continue;
                }
                if (currentResult.Value.MeasureResult.Dies is null)
                {
                    var measureResultsData = measureDCProvider.GetDCResultBase(currentResult.Value.MeasureResult, measureSettings);
                    if (measureResultsData != null)
                    {
                        curPointMeasureDataForMeasure.WaferMeasuresDataForMeasure.AddRange(measureResultsData);
                    }
                    foreach (var pointResult in currentResult.Value.MeasureResult.Points)
                    {
                        var pointResultsData = measureDCProvider.GetDCResult(pointResult, measureSettings, pointResult.SiteId);
                        if (pointResultsData != null)
                        {
                            curPointMeasureDataForMeasure.WaferMeasuresDataForMeasure.AddRange(pointResultsData);
                        }
                    }
                }
                else
                {
                    foreach (var dieResult in currentResult.Value.MeasureResult.Dies)
                    {
                        foreach (var pointResult in dieResult.Points)
                        {
                            var pointResultsData = measureDCProvider.GetDCResult(pointResult, measureSettings, pointResult.SiteId, dieResult.RowIndex, dieResult.ColumnIndex);
                            if (pointResultsData != null)
                            {
                                curPointMeasureDataForMeasure.WaferMeasuresDataForMeasure.AddRange(pointResultsData);
                            }
                        }
                    }
                }
            }

            return dvidWaferMeasureData;
        }

        #endregion DVIDs

        #region Recipe Events

        public void StopRecipe()
        {
            if (_recipeCancellationTokenSource is null)
            {
                return;
            }

            if (_lastResultStateError != ResultState.Ok)
            {
                _lastResultStateError = ResultState.AbortError;
            }

            // We should stop the current measure or the current alignment step
            _recipeCancellationTokenSource.Cancel();
        }

        public void PauseRecipe()
        {
            _isRecipePaused = true;
        }

        public void ResumeRecipe()
        {
            _isRecipePaused = false;
        }

        private void OnOnePointMeasured(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex)
        {
            _messenger.Send(new MeasurePointResultMessage() { ResultMeasure = res, ResultFolderPath = resultFolderPath, ResultDieIndex = dieIndex });
        }

        private void NotifyAlignmentProgressing(RecipeProgressState progressState, string message)
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = message,
                RecipeProgressState = progressState,
                PointMeasureStarted = null,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            }) ;
        }

        private void NotifyMeasuringProgress(MeasurePointInfo inProgressMeasure)
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Measure started on point {inProgressMeasure.PointDataIndex}" + ((inProgressMeasure.Die is null) ? " " : $" die row: {inProgressMeasure.Die.Row} column: {inProgressMeasure.Die.Column}"),
                RecipeProgressState = RecipeProgressState.Measuring,
                PointMeasureStarted = inProgressMeasure,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        private void NotifyComputeMeasureFromSubMeasures(MeasurePointInfo inProgressMeasure)
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Measure started on point {inProgressMeasure.PointDataIndex}" + ((inProgressMeasure.Die is null) ? " " : $" die row: {inProgressMeasure.Die.Row} column: {inProgressMeasure.Die.Column}"),
                RecipeProgressState = RecipeProgressState.ComputeMeasureFromSubMeasures,
                PointMeasureStarted = inProgressMeasure,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        private void NotifyRecipeStopped()
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Recipe stopped",
                RecipeProgressState = RecipeProgressState.Canceled,
                PointMeasureStarted = null,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        private void NotifyRecipePaused()
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Recipe paused",
                RecipeProgressState = RecipeProgressState.InPause,
                PointMeasureStarted = null,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        private void NotifyRecipeExecutionSucceeded()
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Measure terminated successfully",
                RecipeProgressState = RecipeProgressState.Success,
                PointMeasureStarted = null,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        public void NotifyRecipeExecutionFailed()
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Measure terminated with error",
                RecipeProgressState = RecipeProgressState.Error,
                PointMeasureStarted = null,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        private void NotifySubMeasureStarted(MeasurePointInfo subMeasurePointInfo)
        {
            _messenger.Send(new RecipeProgress
            {
                NbRemainingPoints = _nbPointsToMeasure - _nbMeasuredPoints,
                RemainingTime = GetEstimatedExecutionTimeToFinish(_runningRecipe, _nbPointsToMeasure - _nbMeasuredPoints),
                Message = $"Sub Measure started on point {subMeasurePointInfo.PointDataIndex}" + ((subMeasurePointInfo.Die is null) ? " " : $" die row: {subMeasurePointInfo.Die.Row} column: {subMeasurePointInfo.Die.Column}"),
                RecipeProgressState = RecipeProgressState.SubMeasuring,
                PointMeasureStarted = subMeasurePointInfo,
                ResultFolderPath = CurrentRecipeFolderPath(),
                RunningRecipeInfo = _runningRecipe.GetBaseRecipeInfo(),
                RemoteInfo = _automationInfo
            });
        }

        private void SubMeasureFinished(MeasurePointResult subMeasurePointResult)
        {
            _messenger.Send(new MeasurePointResultMessage()
            {
                ResultMeasure = subMeasurePointResult
            });
        }

        private int GetNbPointsToMeasure(ANARecipe recipe, int nbRuns)
        {
            var nbPoints = 0;
            // We first count only active measures which can rely on dies
            foreach (var measure in recipe.Measures.Where(m => m.IsActive && !m.IsMeasureWithSubMeasurePoints))
            {
                nbPoints += measure.MeasurePoints.Count;
            }

            if (recipe.WaferHasDies)
            {
                nbPoints *= recipe.Dies.Count;
            }

            // Measures with sub measures don't rely on dies, that's why we count their points after the multiplication by the number of dies
            foreach (var measure in recipe.Measures.Where(m => m.IsActive && m.IsMeasureWithSubMeasurePoints))
            {
                // Measures with sub measures display an extra point during recipe run which contains measure global results
                nbPoints += measure.MeasurePoints.Count + 1;
            }

            // Finally we multiply by the number of repetas
            if (nbRuns > 1)
            {
                nbPoints *= nbRuns;
            }

            return nbPoints;
        }

        // TODO we must find a way to express the current progress in order to estimate the remaining time
        private TimeSpan GetEstimatedExecutionTimeToFinish(ANARecipe recipe, int nbRemainingPoints)
        {
            // TODO estimate really the acquisition time
            int estimatedTimeinSeconds = nbRemainingPoints * 5;
            return new TimeSpan(0, 0, estimatedTimeinSeconds);
        }

        private string CurrentRecipeFolderPath()
        {
            var resultFoldersPath = _resultsFolderPaths?.Values.FirstOrDefault();
            if (resultFoldersPath != null)
            {
                return resultFoldersPath.RecipeFolderPath;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion Recipe Events
    }
}
