using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Shared;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Delegates;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Client.Proxy.Recipe
{
    /// <summary>
    /// Proxy to supervise the distant recipe service
    /// </summary>
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ANARecipeSupervisor : IANARecipeServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IANARecipeService> _recipeService;

        public delegate void RecipeProgressChangedHandler(RecipeProgress recipeProgress);

        public event RecipeProgressChangedHandler RecipeProgressChangedEvent;

        public delegate void MeasureResultChangedHandler(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex);

        public event MeasureResultChangedHandler MeasureResultChangedEvent;

        public delegate void RecipeSaveProgressChanged(string message);

        public event RecipeSaveProgressChanged RecipeSaveProgressChangedEvent;

        public event RecipeLoadProgressChanged RecipeLoadProgressChangedEvent;

        public delegate void RecipeFinishedHandler(List<MetroResult> results);

        public event RecipeFinishedHandler RecipeFinishedChangedEvent;

        public delegate void RecipeStartedHandler(ANARecipeWithExecContext startedRecipe);

        public event RecipeStartedHandler RecipeStartedChangedEvent;

        private ServiceInvoker<IDbRecipeService> _dbRecipeService;
        private IUserSupervisor _userSupervisor;

        /// <summary>
        /// Constructor
        /// </summary>
        public ANARecipeSupervisor(ILogger<ANARecipeSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            // ANA Recipe service
            _recipeService = new DuplexServiceInvoker<IANARecipeService>(_instanceContext, "ANALYSERecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IANARecipeService>>(), messenger, x => x.SubscribeToRecipeChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
            _logger = logger;
            _messenger = messenger;
            // DataAcces service
            _dbRecipeService = new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), _messenger, ClientConfiguration.GetDataAccessAddress());
            _userSupervisor = ClassLocator.Default.GetInstance<IUserSupervisor>();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _recipeService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error($"PM subscribe error: {ex.Message}");
                throw;
            }
            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _recipeService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PM UnSubscribe error");
            }
            return resp;
        }

        public ANARecipe CreateRecipe(string name = null, int stepId = -1, int userId = 0)
        {
            return _recipeService.Invoke(s => s.CreateRecipe(name, stepId, userId));
        }

        public ANARecipe GetRecipe(Guid recipeKey, bool loadExternalFiles)
        {
            RecipeSaveProgressChangedEvent?.Invoke("Start get recipe");
            var anaRecipe = _recipeService.Invoke(s => s.GetRecipeFromKey(recipeKey));
            if (loadExternalFiles)
            {
                ANARecipeHelper.UpdateRecipeWithExternalFiles(anaRecipe, RecipeLoadProgressChangedEvent);
            }
            return anaRecipe;
        }

        public void StartRecipe(ANARecipe recipe, int nbRuns = 1)
        {
            _recipeService.Invoke(s => s.StartRecipe(recipe, nbRuns));
        }

        public void PauseRecipe()
        {
            _recipeService.Invoke(s => s.PauseRecipe());
        }

        public void ResumeRecipe()
        {
            _recipeService.Invoke(s => s.ResumeRecipe());
        }

        public void StopRecipe()
        {
            _recipeService.Invoke(s => s.StopRecipe());
        }

        public TimeSpan GetEstimatedTime(ANARecipe recipe, int nbRuns = 1)
        {
            return _recipeService.Invoke(s => s.GetEstimatedTime(recipe, nbRuns));
        }

        public void SaveRecipe(ANARecipe recipe, bool incrementVersion = true)
        {
            _logger.Debug("Save recipe");
            RecipeSaveProgressChangedEvent?.Invoke("Start save recipe");
            var externalFiles = SubObjectFinder.GetAllSubObjectOfTypeT<ExternalFileBase>(recipe);
            // update external File key
            foreach (var externalFile in externalFiles)
            {
                externalFile.Value.FileNameKey = string.Concat(externalFile.Key, externalFile.Value.FileExtension);
            }

            // Save recipe
            var recipeId = _recipeService.Invoke(s => s.SaveRecipe(recipe, incrementVersion, _userSupervisor.CurrentUser.Id));

            // Save external files
            SaveExternalFiles(externalFiles.Select(x => x.Value), recipeId);
        }

        private void SaveExternalFiles(IEnumerable<ExternalFileBase> files, int recipeId)
        {
            int nbExternalFiles = files.Count();
            int currentExternalFiles = 0;
            foreach (var externalFile in files)
            {
                currentExternalFiles++;
                _logger.Debug($"Save extenal file {externalFile.FileNameKey} ({currentExternalFiles}/{nbExternalFiles})");
                RecipeSaveProgressChangedEvent?.Invoke($"Save extenal file {currentExternalFiles}/{nbExternalFiles}");
                var externalFileId = _dbRecipeService.Invoke(x => x.SetExternalFile(externalFile, recipeId, _userSupervisor.CurrentUser.Id));
            }
        }
        public void RecipeProgressChanged(RecipeProgress recipeProgress)
        {
            RecipeProgressChangedEvent?.Invoke(recipeProgress);
        }

        public void MeasureResultChanged(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex)
        {
            MeasureResultChangedEvent?.Invoke(res, resultFolderPath, dieIndex);
        }

        public void SaveCurrentResultInProductionDatabase(string lotName)
        {
            _recipeService.Invoke(s => s.SaveCurrentResultInProductionDatabase(lotName));
        }

        public void RecipeFinished(List<MetroResult> results)
        {
            RecipeFinishedChangedEvent?.Invoke(results);
        }

        public void RecipeStarted(ANARecipeWithExecContext startedRecipe)
        {
            RecipeStartedChangedEvent?.Invoke(startedRecipe);
        }
    }
}
