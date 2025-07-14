using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Core.Recipe;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, MaxItemsInObjectGraph = 2147483646)]
    public class ANARecipeService : DuplexServiceBase<IANARecipeServiceCallback>, IANARecipeService
    {
        private DbRecipeServiceProxy _dbRecipeService;
        private Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();
        private PMConfiguration _pmConfiguration;
        private readonly IANARecipeExecutionManager _recipeManager;
        private int _pmChamberDBID = -1;
        private int _pmToolDBID = -1;

        public ANARecipeService(ILogger logger) : base(logger, ExceptionType.RecipeException)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            _pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
            _recipeManager = ClassLocator.Default.GetInstance<IANARecipeExecutionManager>();

            var messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Register<RecipeProgress>(this, (r, m) => RecipeProgressChanged(m));
            messenger.Register<MeasurePointResultMessage>(this, (r, m) => MeasureResultChanged(m));
            messenger.Register<RecipeStartedMessage>(this, (r, m) => RecipeStartedChanged(m));

        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
            });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
            });
        }

        public Response<ANARecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0)
        {

            if (_pmChamberDBID == -1)
            {
                try
                {
                    var chamber = _dbRecipeService.GetChamberFromKeys(_pmConfiguration.ToolKey, _pmConfiguration.ChamberKey);
                    _pmChamberDBID = chamber.Id;
                    _pmToolDBID = chamber.ToolId;
                    _logger.Debug($"[ToolKey, ChamberKey] = [{_pmConfiguration.ToolKey},{_pmConfiguration.ChamberKey}] => [ToolId, ChamberId] = [{_pmChamberDBID},{_pmToolDBID}]");

                }
                catch {
                    _logger.Warning($"could not get chamber id from [ToolKey,ChamberKey] = [{_pmConfiguration.ToolKey},{_pmConfiguration.ChamberKey}]");
                }
            }

            return InvokeDataResponse(messagesContainer =>
            {
                _logger.Debug("CreateRecipe");
                var recipe = new ANARecipe();
                recipe.Name = name;
                recipe.ActorType = ActorType.ANALYSE;
                recipe.Key = Guid.NewGuid();
                recipe.StepId = stepId;
                recipe.UserId = userId;
                recipe.Created = DateTime.Now;
                recipe.CreatorChamberId = _pmChamberDBID;
                recipe.Measures = new List<MeasureSettingsBase>();
                recipe.Points = new List<MeasurePoint>();
                return recipe;
            });
        }

        public Response<ANARecipe> GetRecipeFromKey(Guid recipeKey)
        {
            return InvokeDataResponse(() =>
            {
                var dbrecipe = _dbRecipeService.GetLastRecipeWithProductAndStep(recipeKey);
                var recipe = _recipeManager.Convert_RecipeToAnaRecipe(dbrecipe);
                return recipe;
            });
        }

        public Response<int> SaveRecipe(ANARecipe anaRecipe, bool incrementVersion, int userId)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                var dbrecipe = _recipeManager.Convert_AnaRecipeToRecipe(anaRecipe);

                dbrecipe.AddOutput(ResultType.ANALYSE_Thickness);
                return _dbRecipeService.SetRecipe(dbrecipe, incrementVersion);
            });
        }

        public ANARecipe GetRecipeWithTC(string recipeName)
        {
            var dbrecipe = _dbRecipeService.GetRecipeWithTC(recipeName);
            return _recipeManager.Convert_RecipeToAnaRecipe(dbrecipe);
        }

        public Response<TimeSpan> GetEstimatedTime(ANARecipe recipe, int nbRuns = 1)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                return _recipeManager.GetEstimatedExecutionTime(recipe, nbRuns);
            });
        }

        public Response<VoidResult> SubscribeToRecipeChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Subscribed to recipe service change");
                Subscribe();
            });
        }

        public Response<VoidResult> UnsubscribeToRecipeChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("UnSubscribed to recipe service change");
                Unsubscribe();
            });
        }

        public Response<VoidResult> StartRecipe(ANARecipe recipe, int nbRuns = 1)
        {
            return InvokeVoidResponse(_ =>
            {
                Task.Run(() =>
                {
                    try
                    {
 
                        //Just for test for integration with automation
                        /* RemoteProductionInfo automationInfo = new RemoteProductionInfo();
                         automationInfo.LotName = "LotTestRunRecipev2";
                         automationInfo.SlotId = 1;
                         automationInfo.JobName = "JobTestRunRecipev2";
                         automationInfo.TCRecipeName = "TCRecipeNameTestv2";
                         automationInfo.WaferBaseName = "WaferTest";
                         var result = _recipeManager.Execute(recipe, automationInfo);*/

                        var result = _recipeManager.Execute(recipe, null, nbRuns);
                        InvokeCallback(callback => callback.RecipeFinished(result?.Values?.ToList()));

                    }
                    catch(Exception ex)
                    {
                        RecipeInfo recipeInfo = recipe?.GetBaseRecipeInfo();
                        RecipeProgressChanged(new RecipeProgress() { Message = "Error during recipe execution: "+ ex.Message, RecipeProgressState = RecipeProgressState.Error, RunningRecipeInfo = recipeInfo});
                        _logger.Error(ex, "Error during recipe execution");
                    }
                });
            });
        }

        public Response<VoidResult> StopRecipe()
        {
            return InvokeVoidResponse(_ =>
            {
                _recipeManager.StopRecipe();
            });
        }

        public Response<VoidResult> PauseRecipe()
        {
            return InvokeVoidResponse(_ =>
            {
                _recipeManager.PauseRecipe();
            });
        }

        public Response<VoidResult> ResumeRecipe()
        {
            return InvokeVoidResponse(_ =>
            {
                _recipeManager.ResumeRecipe();
            });
        }

        private void HandleProgress(RecipeProgress progress)
        {
            InvokeCallback(callback => callback.RecipeProgressChanged(progress));
         }

        private void RecipeProgressChanged(RecipeProgress progress)
        {
            InvokeCallback(callback => callback.RecipeProgressChanged(progress));
         }


        private void MeasureResultChanged(MeasurePointResultMessage measureResult)
        {
            InvokeCallback(callback => callback.MeasureResultChanged(measureResult.ResultMeasure, measureResult.ResultFolderPath, measureResult.ResultDieIndex));
        }


        private void RecipeStartedChanged(RecipeStartedMessage startedRecipe)
        {
            InvokeCallback(callback => callback.RecipeStarted(startedRecipe.Recipe));
        }
       
        private void HandleMeasureResult(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex)
        {
            InvokeCallback(callback => callback.MeasureResultChanged(res, resultFolderPath, dieIndex));
        }

        public Response<VoidResult> SaveCurrentResultInProductionDatabase(string lotName)
        {
            return InvokeVoidResponse(_ =>
            {
                _recipeManager.SaveCurrentResultInDatabase(lotName);
            });
        }
    }
}
