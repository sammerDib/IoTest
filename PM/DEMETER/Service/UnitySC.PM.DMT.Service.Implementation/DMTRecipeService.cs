using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.PM.DMT.Hardware.Manager;
using UnitySC.PM.DMT.Service.Implementation.Calibration;
using UnitySC.PM.DMT.Service.Implementation.Camera;
using UnitySC.PM.DMT.Service.Implementation.Execution;
using UnitySC.PM.DMT.Service.Implementation.Fringes;
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
using UnitySC.PM.DMT.Shared;
using UnitySC.PM.Shared;
using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.PM.DMT.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false,
                        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class DMTRecipeService : DuplexServiceBase<IDMTRecipeServiceCallback>, IDMTRecipeService
    {
        private const string RecipeFolderName = "Recipes";

        private readonly AlgorithmManager _algorithmManager;

        private readonly DbRecipeServiceProxy _dbRecipeService;
        private readonly IFlowsConfiguration _flowsConfiguration;
        private readonly FringeManager _fringeManager;
        private readonly DMTHardwareManager _hardwareManager;
        private readonly CalibrationManager _calibrationManager;
        private readonly object _lock = new object();
        private readonly Mapper _mapper;
        private readonly PMConfiguration _pmConfiguration;
        private readonly IDMTScreenService _screenService;
        private readonly DMTCameraManager _cameraManager;
        private readonly MeasuresConfiguration _measuresConfiguration;

        private double _exposureFactorForMatchingBS = double.NaN;

        private double _exposureFactorForMatchingFS = double.NaN;
        private readonly RecipeExecution _recipeExecution;

        public DMTRecipeService(DMTCameraManager cameraManager, DbRecipeServiceProxy dbRecipeService, PMConfiguration pmConfiguration,
            AlgorithmManager algorithmManager, FringeManager fringeManager, DMTHardwareManager hardwareManager, CalibrationManager calibrationManger,
            Mapper mapper, RecipeExecution recipeExecution, IFlowsConfiguration flowsConfiguration, ILogger<DMTRecipeService> logger,
            IMessenger messenger, MeasuresConfiguration measuresConfiguration) :
            base(logger, ExceptionType.RecipeException)
        {
            if (!Directory.Exists(RecipePath))
                Directory.CreateDirectory(RecipePath);
            _dbRecipeService = dbRecipeService;
            _pmConfiguration = pmConfiguration;
            _algorithmManager = algorithmManager;
            _flowsConfiguration = flowsConfiguration;
            _cameraManager = cameraManager;
            _fringeManager = fringeManager;
            _hardwareManager = hardwareManager;
            _calibrationManager = calibrationManger;
            _measuresConfiguration = measuresConfiguration;

            _mapper = mapper;
            _recipeExecution = recipeExecution;
            _recipeExecution.ProgressChanged += ReportProgress;
            _recipeExecution.ResultGenerated += ResultGenerated;
        }

        private string RecipePath => Path.Combine(Directory.GetCurrentDirectory(), RecipeFolderName);

        public double ExposureFactorForMatchingFS
        {
            get
            {
                if (_exposureFactorForMatchingFS is double.NaN)
                {
                    _exposureFactorForMatchingFS = GetExposureFactorMatching(Side.Front);
                }

                return _exposureFactorForMatchingFS;
            }
        }

        public double ExposureFactorForMatchingBS
        {
            get
            {
                if (_exposureFactorForMatchingBS is double.NaN)
                {
                    _exposureFactorForMatchingBS = GetExposureFactorMatching(Side.Back);
                }

                return _exposureFactorForMatchingBS;
            }
        }

        public event ReportProgressEventHandler Progress;

        public event RecipeAddedEventHandler RecipeAdded;

        Response<VoidResult> IDMTRecipeService.Subscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Subscribed to Recipe status change");
                    Subscribe();
                }
            });
        }

        Response<VoidResult> IDMTRecipeService.Unsubscribe()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                lock (_lock)
                {
                    _logger.Information("Unsubscribed to Recipe status change");
                    Unsubscribe();
                }
            });
        }

        Response<VoidResult> IDMTRecipeService.Test()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _logger.Information("Test");
            });
        }

        public Response<DMTRecipe> GetRecipeFromKey(Guid recipeKey, bool takeArchivedRecipes)
        {
            return InvokeDataResponse(messages =>
            {
                var dbrecipe =
                    _dbRecipeService.GetLastRecipe(recipeKey, /*includeRecipeFileInfos*/ false, takeArchivedRecipes);

                var recipe = PrepareRecipe(dbrecipe, messages);
                return recipe;
            });
        }

        public Response<DMTRecipe> GetLastRecipeWithProductAndStep(Guid recipeKey)
        {
            return InvokeDataResponse(messages =>
            {
                var dbRecipe = _dbRecipeService.GetLastRecipeWithProductAndStep(recipeKey);

                return PrepareRecipe(dbRecipe, messages);
            });
        }

        /// <summary>
        ///     Create new recipe
        /// </summary>
        Response<DMTRecipe> IDMTRecipeService.CreateRecipe(string name, int stepId, int userId)
        {
            return InvokeDataResponse(messagesContainer => InternalCreateRecipe(name, stepId, userId));
        }

        /// <summary>
        ///     Ajoute ou met à jour une recette dans la base de données
        /// </summary>
        Response<VoidResult> IDMTRecipeService.SaveRecipe(DMTRecipe recipe)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                InternalSaveRecipe(recipe);
            });
        }

        public Response<List<RecipeInfo>> GetRecipeList(int stepId, bool takeArchivedRecipes)
        {
            return InvokeDataResponse(() =>
            {
                int chamberKey = _pmConfiguration.ChamberKey;
                int toolKey = _pmConfiguration.ToolKey;
                return _dbRecipeService.GetRecipeList(ActorType.DEMETER, stepId, chamberKey, toolKey);
            });
        }

        public Response<List<TCPMRecipe>> GetTCRecipeList()
        {
            return InvokeDataResponse(messagesContainer =>
            {
                int toolKey = _pmConfiguration.ToolKey;
                return _dbRecipeService.GetTCRecipeList(ActorType.DEMETER, toolKey);
            });
        }

        /// <summary>
        ///     Démarre une recette qui s'exécutera en tâche de fond
        /// </summary>
        public async Task<Response<VoidResult>> StartRecipeAsync(
            DMTRecipe recipe, string acqDestFolder, bool overwriteOutput,
            string dataflowID)
        {
            return await InvokeVoidResponseAsync(async messageContainer =>
            {
                await Task.Run(() =>
                {
                     _recipeExecution.StartRecipe(recipe, acqDestFolder, overwriteOutput, dataflowID);
                });
            });
        }

        public void ReportProgress(object sender, RecipeStatus status)
        {
            OnProgress(status);

            InvokeCallback(x => x.RecipeProgress(status));
        }

        public void ResultGenerated(object sender, DMTResultGeneratedEventArgs args)
        {
            InvokeCallback(x => x.ResultGenerated(args.Name, args.WaferSide, args.Path));
        }

        public Response<VoidResult> Abort()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _recipeExecution.AbortExecution();
            });
        }

        public async Task<Response<Dictionary<CurvatureImageType, ServiceImage>>> BaseCurvatureDarkDynamicsAcquisition(
            DeflectometryMeasure measure, Length waferDiameter, bool isDarkRequired)
        {
            return await InvokeDataResponseAsync(async () =>
                                                     await _recipeExecution
                                                         .BaseCurvatureDynamicsAcquisitionAsync(measure, waferDiameter,
                                                    isDarkRequired));
        }

        public async Task<Response<Dictionary<CurvatureImageType, ServiceImage>>> RecalculateCurvatureDynamics(
            DeflectometryMeasure measure)
        {
            return await InvokeDataResponseAsync(async messagesContainer => await _recipeExecution.RecalculateCurvatureDynamicsAsync(measure));
        }

        public async Task<Response<ServiceImage>> RecalculateDarkDynamics(DeflectometryMeasure measure)
        {
            return await InvokeDataResponseAsync(async messagesContainer => await _recipeExecution.RecalculateDarkDynamicsAsync(measure));
        }

        public Response<VoidResult> DisposeCurvatureDarkDynamicsAdjustmentMeasureExecution()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
            });
        }

        Response<RemoteProductionInfo> IDMTRecipeService.GetDefaultRemoteProductionInfo()
        {
            var material = new Material()
            {
                SubstrateID = _pmConfiguration.DefaultWaferName,
                LotID = _pmConfiguration.DefaultLotName,
                ProcessJobID = _pmConfiguration.DefaultJobName,
                JobPosition = JobPosition.FirstAndLast,
                SlotID = _pmConfiguration.DefaultSlotId,
                DeviceID= _pmConfiguration.DefaultWaferName
            };
            return InvokeDataResponse(messageContainer => new RemoteProductionInfo()
            {
                ProcessedMaterial = material,
                DFRecipeName = "NoRecipe"
            });
        }

        public Response<DMTRecipe> ImportRecipe(DMTRecipe recipe, int stepId, int userId)
        {
            return InvokeDataResponse(messages =>
            {
                CheckMeasureSideCompatibility(recipe, messages);
                
                CheckPerspectiveCalibrationCompatibility(recipe, messages);

                CheckNotAvailableMeasures(recipe, messages);
                
                CheckBrightFieldMeasuresCompatibility(recipe, messages);
                
                CheckDeflectometryMeasuresCompatibility(recipe, messages);

                if (messages.Any(m => m.Level == MessageLevel.Error))
                {
                    return null;
                }
                var newRecipe = InternalCreateRecipe(recipe.Name, stepId, userId);
                recipe.StepId = newRecipe.StepId;
                recipe.UserId = newRecipe.UserId;
                recipe.Created = newRecipe.Created;
                recipe.ActorType = newRecipe.ActorType;
                recipe.CreatorChamberId = newRecipe.CreatorChamberId;
                recipe.Name = newRecipe.Name;
                recipe.Key = newRecipe.Key;
                recipe.FileVersion = DMTRecipe.CurrentFileVersion;
                
                AddMeasuresForMissingSideIfNeeded(recipe, newRecipe);
                
                InternalSaveRecipe(recipe);
                
                return recipe;
            });
        }

        public Response<DMTRecipe> GetRecipeForExport(Guid recipeKey)
        {
            return InvokeDataResponse(messages =>
            {
                var dbRecipe = _dbRecipeService.GetLastRecipe(recipeKey);
                var recipe = PrepareRecipe(dbRecipe, messages);
                AdjustExposureTimesForToolMatchingBeforeSaving(recipe);
                return recipe;
            });
        }

        private void CheckNotAvailableMeasures(DMTRecipe recipe, List<Message> messages)
        {
            if (recipe.Measures.Any(m =>
                    m.IsEnabled && !_measuresConfiguration.GetMeasureConfiguration(m.MeasureType).IsAvailable))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.NotAvailableMeasure.ToString()));
            }
        }

        private void AddMeasuresForMissingSideIfNeeded(DMTRecipe recipe, DMTRecipe newRecipte)
        {
            bool recipeHasFrontMeasures = recipe.Measures.Any(m => m.Side == Side.Front);
            bool recipeHasBackMeasures = recipe.Measures.Any(m => m.Side == Side.Back);
            if (!recipeHasFrontMeasures)
            {
                recipe.Measures.AddRange(newRecipte.Measures.Where(m => m.Side == Side.Front));
            }

            if (!recipeHasBackMeasures)
            {
                recipe.Measures.AddRange(newRecipte.Measures.Where(m => m.Side == Side.Back));
            }
        }

        private DMTRecipe InternalCreateRecipe(string name, int stepId, int userId)
        {
            int pmChamberDBID = -1;
            int pmToolDBID = -1;
            try
            {
                var chamber =
                    _dbRecipeService.GetChamberFromKeys(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey);
                pmChamberDBID = chamber.Id;
                pmToolDBID = chamber.ToolId;
                _logger.Debug(
                    $"[ToolKey, ChamberKey] = [{_pmConfiguration.ToolKey},{_pmConfiguration.ChamberKey}] => [ToolId, ChamberId] = [{pmChamberDBID},{pmToolDBID}]");
            }
            catch
            {
                _logger.Warning(
                    $"could not get chamber id from [ToolKey,ChamberKey] = [{_pmConfiguration.ToolKey},{_pmConfiguration.ChamberKey}]");
            }

            _logger.Debug("CreateRecipe");
            var recipe = new DMTRecipe();

            recipe.Name = name;
            recipe.ActorType = ActorType.DEMETER;
            recipe.Key = Guid.NewGuid();
            recipe.StepId = stepId;
            recipe.UserId = userId;
            recipe.Created = DateTime.Now;
            recipe.CreatorChamberId = pmChamberDBID;

            recipe.Measures = new List<MeasureBase>();

            var measuresConfiguration = ClassLocator.Default.GetInstance<MeasuresConfiguration>();
            var defaultDeflectometryOutputs = GetDeafultDeflectometryOutputs();
            
            foreach (Side side in new [] {Side.Front, Side.Back})
            {
                bool sideAvailable = side == Side.Front ? _hardwareManager.IsFrontSideAvailable() : _hardwareManager.IsBackSideAvailable();
                if (sideAvailable)
                {
                    bool areMeasuresEnabled = side == Side.Front;
                    recipe.SetIsPerspectiveCalibrationUsed(side, IsPerspectiveCalibrationAvailable(side));
                    AddBrightFieldMeasureForSideIfAvailable(measuresConfiguration, recipe, side, areMeasuresEnabled);
                    AddDeflectometryMeasureForSideIfAvailable(measuresConfiguration, recipe, defaultDeflectometryOutputs, side, areMeasuresEnabled);
                    AddHighAngleDarkFieldMeasureForSideIfAvailable(measuresConfiguration, recipe, side, false);

                    if (side == Side.Back)
                    {
                        AddBacklightMeasureIfAvailable(measuresConfiguration, recipe);        
                    }
                }
            }
            
            // Initialise all exposure settings of the measures
            foreach (var measure in recipe.Measures)
            {
                InitAutoExposureSettings(measure);
            }
            
            return recipe;
        }

        private DeflectometryOutput GetDeafultDeflectometryOutputs()
        {
            var standardOutputs = DeflectometryOutput.Curvature | DeflectometryOutput.LowAngleDarkField;
            
            return _measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>().AvailableOutputs & standardOutputs;
        }

        private void AddBacklightMeasureIfAvailable(MeasuresConfiguration measuresConfiguration, DMTRecipe recipe)
        {
            if (measuresConfiguration.GetConfiguration<BackLightMeasureConfiguration>().IsAvailable)
            {
                recipe.Measures.Add(new BackLightMeasure
                {
                    IsEnabled = false,
                    ROI = CreateROI(Side.Front),
                    Side = Side.Back,
                    ExposureTimeMs = _measuresConfiguration.GetConfiguration<BackLightMeasureConfiguration>().ExposureTimeMs
                });
            }
        }

        private void AddBrightFieldMeasureForSideIfAvailable(MeasuresConfiguration measuresConfiguration,
            DMTRecipe recipe, Side side, bool isMeasureEnabled)
        {
            if (measuresConfiguration.GetConfiguration<BrightFieldMeasureConfiguration>().IsAvailable)
            {
                recipe.Measures.Add(new BrightFieldMeasure
                {
                    IsEnabled = isMeasureEnabled,
                    ROI = CreateROI(side),
                    Side = side
                });
            }
        }
        
        private void AddDeflectometryMeasureForSideIfAvailable(MeasuresConfiguration measuresConfiguration,
            DMTRecipe recipe, DeflectometryOutput deflectometryOutputs, Side side, bool isMeasureEnabled)
        {
            if (measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>().IsAvailable)
            {
                recipe.Measures.Add(new DeflectometryMeasure
                {
                    IsEnabled = isMeasureEnabled,
                    ROI = CreateROI(side),
                    Side = side,
                    AutoExposureTimeTrigger =
                        AutoExposureTimeTrigger.OnFirstWaferOfLot,
                    Fringe =
                        _fringeManager.Fringes.First(f => f.FringeType ==
                                                          FringeType.Standard),
                    Outputs = deflectometryOutputs,
                    AvailableOutputs = GetAvailableOutputs()
                });
            }
        }
        
        private void AddHighAngleDarkFieldMeasureForSideIfAvailable(MeasuresConfiguration measuresConfiguration,
            DMTRecipe recipe, Side side, bool isMeasureEnabled)
        {
            if (measuresConfiguration.GetConfiguration<HighAngleDarkFieldMeasureConfiguration>().IsAvailable)
            {
                recipe.Measures.Add(new HighAngleDarkFieldMeasure
                {
                    IsEnabled = isMeasureEnabled,
                    ROI = CreateROI(side),
                    Side = side
                });
            }
        }

        private void InternalSaveRecipe(DMTRecipe recipe)
        {
            if (recipe.Measures.IsNullOrEmpty())
            {
                throw new Exception(
                    "Cannot create a recipe without measures, please check measures configuration file");
            }

            AdjustExposureTimesForToolMatchingBeforeSaving(recipe);
            var dbrecipe = _mapper.AutoMap.Map<Recipe>(recipe);
            foreach (var measure in recipe.Measures.Where(m => m.IsEnabled))
            {
                foreach (var output in measure.GetOutputTypes())
                {
                    dbrecipe.AddOutput(output);
                }
            }

            _dbRecipeService.SetRecipe(dbrecipe, true);
        }

        private void CheckDeflectometryMeasuresCompatibility(DMTRecipe recipe, List<Message> messages)
        {
            CheckDeflectometryMeasuresOutputs(recipe, messages);

            if (recipe.Measures.OfType<DeflectometryMeasure>().Any(m =>
                    m.Fringe.FringeType == FringeType.Standard &&
                    !_fringeManager.Fringes.Exists(fringe => fringe.Period == m.Fringe.Period)))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.NotCompatibleDeflectometryStandardFringe.ToString()));
            }
            
            if (recipe.Measures.OfType<DeflectometryMeasure>().Any(m =>
                    m.Fringe.FringeType == FringeType.Multi &&
                    (!_fringeManager.Fringes.Exists(fringe => fringe.Period == m.Fringe.Period) ||
                    !DMTDeflectometryMeasureHelper.IsMultiPeriodFringeAvailable(m.Fringe.Periods,
                        _hardwareManager.ScreensBySide[m.Side].Width, _hardwareManager.ScreensBySide[m.Side].Height))))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.NotCompatibleDeflectometryMultiFringe.ToString()));
            }
        }

        private void CheckDeflectometryMeasuresOutputs(DMTRecipe recipe, List<Message> messages)
        {
            if (recipe.Measures.OfType<DeflectometryMeasure>().Any(m => !AreAllDeflectometryOutputsCompatible(m)))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.NotCompatibleDeflectometryOutputs.ToString()));
            }
        }

        private void CheckBrightFieldMeasuresCompatibility(DMTRecipe recipe, List<Message> messages)
        {
            if (recipe.Measures.OfType<BrightFieldMeasure>().Any(m => !_fringeManager.Colors.Contains(m.Color)))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.BrightFieldColorNotCompatible.ToString()));
            }

            if (recipe.Measures.OfType<BrightFieldMeasure>().Any(m =>
                    m.AutoExposureTimeTrigger == AutoExposureTimeTrigger.Never && m.ApplyUniformityCorrection))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.BrightFieldApplyUniformity.ToString()));
            }
        }

        private void CheckMeasureSideCompatibility(DMTRecipe recipe, List<Message> messages)
        {
            if
                ((!_hardwareManager.IsBackSideAvailable() &&
                  recipe.Measures.Any(m => m.IsEnabled && m.Side == Side.Back)) ||
                 (!_hardwareManager.IsFrontSideAvailable() &&
                  recipe.Measures.Any(m => m.IsEnabled && m.Side == Side.Front)))
            {
                messages.Add(new Message(MessageLevel.Error, RecipeCheckError.SideIncompatibility.ToString()));
            }
        }

        /// <summary>
        ///     Called to signal to subscribers that recipe progress changed
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProgress(RecipeStatus e)
        {
            var eh = Progress;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        public override void Shutdown()
        {
            if (_recipeExecution != null)
            {
                _recipeExecution.ProgressChanged -= ReportProgress;
                _recipeExecution.ResultGenerated -= ResultGenerated;
            }

            base.Shutdown();
        }

        /// <summary>
        ///     Called to signal to subscribers that recipe added
        /// </summary>
        /// <param DMTRecipe="r"></param>
        protected virtual void OnRecipeAdded(DMTRecipe r)
        {
            var eh = RecipeAdded;
            if (eh != null)
            {
                eh(this, r);
            }
        }

        public Response<DMTRecipe> GetRecipeWithTC(string recipeName)
        {
            return InvokeDataResponse(messages =>
            {
                var dbrecipe = _dbRecipeService.GetRecipeWithTC(recipeName);
                return PrepareRecipe(dbrecipe, messages);
            });
        }

        private DMTRecipe PrepareRecipe(Recipe dbrecipe, List<Message> messages)
        {
            if (dbrecipe == null)
                return null;
            var dmtRecipe = _mapper.AutoMap.Map<DMTRecipe>(dbrecipe);
            
            CheckMeasureSideCompatibility(dmtRecipe, messages);
            
            CheckNotAvailableMeasures(dmtRecipe, messages);
            
            CheckPerspectiveCalibrationCompatibility(dmtRecipe, messages);
            
            CheckBrightFieldMeasuresCompatibility(dmtRecipe, messages);

            // We check that the outputs used used in the recipe for the defectometry are available on this machine
            CheckDeflectometryMeasuresOutputs(dmtRecipe, messages);
            
            foreach (var deflectometryMeasure in dmtRecipe.Measures.OfType<DeflectometryMeasure>())
            {
                deflectometryMeasure.AvailableOutputs = GetAvailableOutputs();
            }

            AdjustExposureTimesForToolMatchingAfterLoading(dmtRecipe);

            return messages.Any(m => m.Level == MessageLevel.Error) ? null : dmtRecipe;
        }

        private bool AreAllDeflectometryOutputsCompatible(DeflectometryMeasure deflectometryMeasure)
        {
            // DeflectometryOutput enum is a flag enum, casting to uint to ensure correct bitwise operations:
            uint measureOutputs = (uint)deflectometryMeasure.Outputs;
            uint availableOutputs = (uint)GetAvailableOutputs();
            return (measureOutputs & availableOutputs) == measureOutputs;
        }

        private void CheckPerspectiveCalibrationCompatibility(DMTRecipe dmtRecipe, List<Message> messages)
        {            
            if (dmtRecipe.IsFSPerspectiveCalibrationUsed && !IsPerspectiveCalibrationAvailable(Side.Front))
            {
                messages.Add(new Message(MessageLevel.Error,
                    RecipeCheckError.NotCompatiblePerspectiveCalibration.ToString(),
                    "Front side perspective calibration is required by recipe but not available"));
            }            
            if (dmtRecipe.IsBSPerspectiveCalibrationUsed && !IsPerspectiveCalibrationAvailable(Side.Back))
            {
                messages.Add(new Message(MessageLevel.Error,
                    RecipeCheckError.NotCompatiblePerspectiveCalibration.ToString(),
                    "Back side perspective calibration is required by recipe but not available"));
            }           
        }

        private DeflectometryOutput GetAvailableOutputs()
        {
            return _measuresConfiguration.GetConfiguration<DeflectometryMeasureConfiguration>().AvailableOutputs;
        }

        private bool IsPerspectiveCalibrationAvailable(Side side)
        {
            var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
            return !(calibration is null);
        }

        // Initialise the Auto Exposure settings of the measure based on the settings from the algorithm settings (AlgorithmsConfiguration.xml)
        private void InitAutoExposureSettings(MeasureBase measure)
        {
            var autoExposureConfigconfig =
                _flowsConfiguration.Flows.OfType<AutoExposureConfiguration>().FirstOrDefault()?
                    .DefaultAutoExposureSetting.FirstOrDefault(defaultConfig =>
                        defaultConfig.WaferSide == measure.Side && defaultConfig.Measure == measure.MeasureType);
            if (!(autoExposureConfigconfig is null))
            {
                measure.AutoExposureTargetSaturation = autoExposureConfigconfig.TargetSaturation;
                measure.AutoExposureSaturationTolerance = autoExposureConfigconfig.SaturationTolerance;
                measure.AutoExposureInitialExposureTimeMs = autoExposureConfigconfig.InitialExposureTimeMs;
                measure.AutoExposureRatioSaturated = autoExposureConfigconfig.RatioSaturated;
            }
        }

        private ROI CreateROI(Side side)
        {
            if (!_hardwareManager.CamerasBySide.ContainsKey(side))
                return null;

            var roi = new ROI();
            var calibration = _calibrationManager.GetPerspectiveCalibrationForSide(side);
            if (calibration is null)
            {
                double x = _hardwareManager.CamerasBySide[side].Width / 4;
                double y = _hardwareManager.CamerasBySide[side].Height / 4;
                var topLeft = new Point(-x, y);
                var bottomRight = new Point(x, -y);
                roi.RoiType = RoiType.Rectangular;
                roi.Rect = new Rect(topLeft, bottomRight);
                roi.WaferRadius = Math.Min(2 * x, 2 * y);
            }
            else
            {
                roi.RoiType = RoiType.WholeWafer;
                roi.WaferRadius = calibration.WaferRadius_um;
                roi.Rect = RoiHelper.CreateRectInsideWholeWafer(roi.WaferRadius,
                    _flowsConfiguration.Flows.OfType<AutoExposureConfiguration>().First().DefaultEdgeExclusionLength
                        .Micrometers);
            }

            return roi;
        }

        private double GetExposureFactorMatching(Side side)
        {
            try
            {
                string cameraSerialNumber = _hardwareManager.CamerasBySide[side].SerialNumber;
                var exposureCalibration = _calibrationManager.GetExposureCalibrationBySide(side);
                if (exposureCalibration == null)
                {
                    _logger.Error($"Exposure Matching coefficient for side {side} not found." +
                        $" Exposure time will not be corrected.");
                    return 1;
                }

                if (exposureCalibration.CameraSerialNumber != cameraSerialNumber)
                {
                    _logger.Error($"Exposure Matching coefficient for camera with serial number {cameraSerialNumber} not found." +
                        $" Exposure time will not be corrected.");
                    return 1;
                }

                return exposureCalibration.ExposureCorrectionCoef;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void AdjustExposureTimesForToolMatchingAfterLoading(DMTRecipe dmtRecipe)
        {
            foreach (var measure in dmtRecipe.Measures)
            {
                if (measure.AutoExposureTimeTrigger == AutoExposureTimeTrigger.Never)
                {
                    double ExposureFactorForMatching = measure.Side == Side.Front
                        ? ExposureFactorForMatchingFS
                        : ExposureFactorForMatchingBS;
                    if (ExposureFactorForMatching == 0)
                        throw new Exception("NotAvailableExposureMatchingCalibration");
                    measure.ExposureTimeMs *= ExposureFactorForMatching;
                }
            }
        }

        private void AdjustExposureTimesForToolMatchingBeforeSaving(DMTRecipe recipe)
        {
            foreach (var measure in recipe.Measures)
            {
                if (measure.AutoExposureTimeTrigger == AutoExposureTimeTrigger.Never)
                {
                    double ExposureFactorForMatching = measure.Side == Side.Front
                        ? ExposureFactorForMatchingFS
                        : ExposureFactorForMatchingBS;
                    if (ExposureFactorForMatching == 0)
                        throw new Exception("NotAvailableExposureMatchingCalibration");
                    measure.ExposureTimeMs /= ExposureFactorForMatching;
                }
            }
        }
    }
}
