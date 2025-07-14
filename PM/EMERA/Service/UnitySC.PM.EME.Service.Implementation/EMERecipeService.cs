using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.EME.Hardware;
using UnitySC.PM.EME.Service.Core.Recipe;
using UnitySC.PM.EME.Service.Core.Shared;
using UnitySC.PM.EME.Service.Implementation.Extensions;
using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Proxy;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Recipe = UnitySC.DataAccess.Dto.Recipe;

namespace UnitySC.PM.EME.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EMERecipeService : DuplexServiceBase<IEMERecipeServiceCallback>, IEMERecipeService
    {
        private DbRecipeServiceProxy _dbRecipeService;
        private readonly PMConfiguration _pmConfiguration;
        private readonly Mapper _mapper;
        private readonly IMessenger _messenger;
        private int _pmChamberDBID = -1;
        private int _pmToolDBID = -1;

        private bool _isCycling;
        private RecipeOrchestrator _orchestrator;

        public EMERecipeService(PMConfiguration pmConfiguration, Mapper mapper, IMessenger messenger, ILogger logger) : base(logger, ExceptionType.RecipeException)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            _pmConfiguration = pmConfiguration;
            _mapper = mapper;
            _messenger = messenger;
            messenger.Register<RecipeExecutionMessage>(this, OnRecipeExecutionMessage);
        }

        private void OnRecipeExecutionMessage(object recipient, RecipeExecutionMessage message)
        {
            InvokeCallback(i => i.ExecutionStatusUpdated(message));
        }

        public Response<EMERecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0)
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
                catch
                {
                    _logger.Warning($"could not get chamber id from [ToolKey,ChamberKey] = [{_pmConfiguration.ToolKey},{_pmConfiguration.ChamberKey}]");
                }
            }
            return InvokeDataResponse(_ =>
            {
                _logger.Debug("CreateRecipe");
                var recipe = new EMERecipe();
                recipe.Name = name;
                recipe.ActorType = ActorType.EMERA;
                recipe.Key = Guid.NewGuid();
                recipe.StepId = stepId;
                recipe.UserId = userId;
                recipe.Created = DateTime.Now;
                recipe.CreatorChamberId = _pmChamberDBID;
                recipe.Acquisitions = new List<Acquisition>();
                return recipe;
            });
        }

        public Response<EMERecipe> GetRecipeFromKey(Guid recipeKey)
        {
            return InvokeDataResponse(() =>
            {
                var dbrecipe = _dbRecipeService.GetLastRecipeWithProductAndStep(recipeKey);
                var recipe = Convert_RecipeToEmeRecipe(dbrecipe);
                return recipe;
            });
        }
        public Response<int> SaveRecipe(EMERecipe emeRecipe, bool incrementVersion, int userId)
        {
            return InvokeDataResponse(_ =>
            {
                var hardwareManager = ClassLocator.Default.GetInstance<EmeHardwareManager>();
                var dbrecipe = Convert_EmeRecipeToRecipe(emeRecipe);
                var outputs = emeRecipe.Acquisitions
                               .Select(acquisition =>
                                       {
                                           try
                                           {
                                               return acquisition.GetOutputType(hardwareManager);
                                           }
                                           catch (Exception ex)
                                           {
                                               _logger.Error(ex.Message, $"Error in GetOutputType for Acquisition = {acquisition.Name}");
                                               return ResultType.NotDefined;
                                           }
                                       })
                               .ToList();
                if (!outputs.Any())
                {
                    outputs.Add(ResultType.NotDefined);
                }
                foreach (var output in outputs)
                {
                    dbrecipe.AddOutput(output);
                }
                return _dbRecipeService.SetRecipe(dbrecipe, incrementVersion);
            });
        }

        public Response<VoidResult> StartRecipe(EMERecipe recipe, string customSavePath)
        {
            return InvokeVoidResponse(_ =>
            {
                _orchestrator = ClassLocator.Default.GetInstance<RecipeOrchestrator>();
                Task.Run(() =>
                {
                    var builder = ClassLocator.Default.GetInstance<RecipeAdapterBuilder>();
                    var adaptedRecipe = builder.ValidateAndBuild(recipe, customSavePath);
                    _orchestrator.Start(adaptedRecipe);
                });
            });
        }

        public Response<VoidResult> StopRecipe()
            => InvokeVoidResponse(_ => _orchestrator?.Cancel());

        public Response<VoidResult> PauseRecipe()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> ResumeRecipe()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartCycling(EMERecipe recipe, string customSavePath)
        {
            return InvokeVoidResponse(_ =>
            {
                _isCycling = true;
                Task.Run(() => StartCycle(recipe, customSavePath));
            });
        }

        private void StartCycle(EMERecipe recipe, string customSavePath)
        {
            if (_orchestrator == null)
                _orchestrator = ClassLocator.Default.GetInstance<RecipeOrchestrator>();

            int iteration = 0;
            while (_isCycling)
            {
                _logger.Information($"Recipe Cycle iteration {iteration}");
                var builder = ClassLocator.Default.GetInstance<RecipeAdapterBuilder>();
                var adaptedRecipe = builder.ValidateAndBuild(recipe, customSavePath);
                _orchestrator.Start(adaptedRecipe);
                iteration++;
            }
        }

        public Response<VoidResult> StopCycling()
        {
            return InvokeVoidResponse(_ =>
            {
                _orchestrator?.Cancel();
                _isCycling = false;
            });
        }

        public Response<VoidResult> SubscribeToRecipeChanges()
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("Subscribed to recipe service change");
                Subscribe();
            });
        }

        public Response<VoidResult> UnsubscribeToRecipeChanges()
        {
            return InvokeVoidResponse(_ =>
            {
                _logger.Information("UnSubscribed to recipe service change");
                Unsubscribe();
            });
        }

        private EMERecipe Convert_RecipeToEmeRecipe(Recipe dbrecipe)
        {
            if (dbrecipe == null)
                return null;

            var recipe = _mapper.AutoMap.Map<EMERecipe>(dbrecipe);
            return recipe;
        }

        private Recipe Convert_EmeRecipeToRecipe(EMERecipe emeRecipe)
        {
            if (emeRecipe == null)
                return null;

            var recipe = _mapper.AutoMap.Map<Recipe>(emeRecipe);
            return recipe;
        }
    }
}
