using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base.Acquisition;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Delegates;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Client.Proxy.Recipe
{
    /// <summary>
    /// Proxy to supervise the distant recipe service
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EMERecipeSupervisor : IEMERecipeSupervisor
    {
        private InstanceContext _instanceContext;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly DuplexServiceInvoker<IEMERecipeService> _recipeService;
        public delegate void RecipeSaveProgressChanged(string message);
        private IUserSupervisor _userSupervisor;
        public delegate void RecipeFinishedHandler(List<FullImageResult> results);
        public event RecipeSaveProgressChanged RecipeSaveProgressChangedEvent;
        public event RecipeLoadProgressChanged RecipeLoadProgressChangedEvent;
        public event RecipeFinishedHandler RecipeFinishedChangedEvent;

        public EMERecipeSupervisor(ILogger<EMERecipeSupervisor> logger, IMessenger messenger, IUserSupervisor userSupervisor)
        {
            _instanceContext = new InstanceContext(this);
            _logger = logger;
            _messenger = messenger;
            _userSupervisor = userSupervisor;
            // EME Recipe service
            _recipeService = new DuplexServiceInvoker<IEMERecipeService>(_instanceContext, "EMERecipeService",
                ClassLocator.Default.GetInstance<SerilogLogger<IEMERecipeService>>(), messenger,
                x => x.SubscribeToRecipeChanges(), ClientConfiguration.GetServiceAddress(ActorType.EMERA));
        }
        public EMERecipe GetRecipe(Guid recipeKey)
        {
            RecipeSaveProgressChangedEvent?.Invoke("Start get recipe");
            var emeRecipe = _recipeService.Invoke(s => s.GetRecipeFromKey(recipeKey));
            return emeRecipe;
        }
        public void SaveRecipe(EMERecipe recipe, bool incrementVersion = true)
        {
            _logger.Debug("Save recipe");
            RecipeSaveProgressChangedEvent?.Invoke("Start save recipe");
            // Save recipe
            var recipeId = _recipeService.Invoke(s => s.SaveRecipe(recipe, incrementVersion, _userSupervisor.CurrentUser.Id));
        }

        public Response<EMERecipe> CreateRecipe(string name, int stepId, int userId)
        {
            return _recipeService.InvokeAndGetMessages(s => s.CreateRecipe(name, stepId, userId));
        }

        public Response<EMERecipe> GetRecipeFromKey(Guid recipeKey)
        {
            return _recipeService.InvokeAndGetMessages(s => s.GetRecipeFromKey(recipeKey));
        }

        public Response<int> SaveRecipe(EMERecipe recipe, bool incrementVersion, int userId)
        {
            return _recipeService.InvokeAndGetMessages(s => s.SaveRecipe(recipe, incrementVersion, userId));
        }

        public Response<VoidResult> StartRecipe(EMERecipe recipe, string customSavePath)
        {
            return _recipeService.InvokeAndGetMessages(s => s.StartRecipe(recipe, customSavePath));
        }

        public Response<VoidResult> StopRecipe()
        {
            return _recipeService.InvokeAndGetMessages(s => s.StopRecipe());
        }

        public Response<VoidResult> PauseRecipe()
        {
            return _recipeService.InvokeAndGetMessages(s => s.PauseRecipe());
        }

        public Response<VoidResult> ResumeRecipe()
        {
            return _recipeService.InvokeAndGetMessages(s => s.ResumeRecipe());
        }

        public Response<VoidResult> StartCycling(EMERecipe recipe, string customSavePath)
        {
            return _recipeService.InvokeAndGetMessages(s => s.StartCycling(recipe, customSavePath));
        }

        public Response<VoidResult> StopCycling()
        {
            return _recipeService.InvokeAndGetMessages(s => s.StopCycling());
        }

        public Response<VoidResult> SubscribeToRecipeChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _recipeService.InvokeAndGetMessages(s => s.SubscribeToRecipeChanges());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Recipe subscribe error");
            }
            return resp;
        }

        public Response<VoidResult> UnsubscribeToRecipeChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _recipeService.InvokeAndGetMessages(s => s.UnsubscribeToRecipeChanges());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Recipe unsubscribe error");
            }
            return resp;
        }

        public void ExecutionStatusUpdated(RecipeExecutionMessage message)
        {
            _messenger.Send(message);
        }
    }
}
