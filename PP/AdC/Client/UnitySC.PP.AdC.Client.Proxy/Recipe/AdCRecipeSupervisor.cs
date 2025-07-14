using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PP.ADC.Service.Interface.Recipe;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Client.Proxy.Recipe
{
    /// <summary>
    /// Proxy to supervise the distant recipe service
    /// </summary>
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ADCRecipeSupervisor
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private ServiceInvoker<IADCRecipeService> _recipeService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ADCRecipeSupervisor(ILogger<ADCRecipeSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            // Recipe service
            _recipeService = new ServiceInvoker<IADCRecipeService>("ADCRecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IADCRecipeService>>(), messenger);
            _logger = logger;
            _messenger = messenger;
        }

        public ADCRecipe CreateRecipe(string name = null, int stepId = -1, int userId = 0)
        {
            return _recipeService.Invoke(s => s.CreateRecipe(name, stepId, userId));
        }


        public ADCRecipe GetRecipe(Guid recipeKey, bool takeArchivedRecipes = false)
        {
            return _recipeService.Invoke(s => s.GetRecipeFromKey(recipeKey, takeArchivedRecipes));
        }


        public void Test()
        {
            _recipeService.Invoke(s => s.Test());
        }

        public void SaveRecipe(ADCRecipe recipe)
        {
            _recipeService.Invoke(s => s.SaveRecipe(recipe));
        }
    }
}
