using System;
using System.ServiceModel;

using UnitySC.PM.AGS.Service.Implementation.Proxy;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.Shared;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using SensorID = UnitySC.PM.AGS.Data.Enum.SensorID;

namespace UnitySC.PM.AGS.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ArgosRecipeService : BaseService, IRecipeService
    {
        private DBRecipeServiceProxy _dbRecipeService;
        private Mapper _mapper = ClassLocator.Default.GetInstance<Mapper>();
        private PMConfiguration _pmConfiguration;

        public ArgosRecipeService(ILogger<ArgosRecipeService> logger) : base(logger, ExceptionType.RecipeException)
        {
            _dbRecipeService = ClassLocator.Default.GetInstance<DBRecipeServiceProxy>();
            _pmConfiguration = ClassLocator.Default.GetInstance<PMConfiguration>();
        }

        public Response<ArgosRecipe> GetRecipe(Guid recipeKey, bool useArchived = false)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                var dbrecipe = _dbRecipeService.GetLastRecipe(recipeKey, /*includeRecipeFileInfos*/ false, useArchived);
                var recipe = ConvertRecipe(dbrecipe);
                CheckRecipe(recipe);
                return recipe;
            });
        }

        private ArgosRecipe ConvertRecipe(DataAccess.Dto.Recipe dbrecipe)
        {
            if (dbrecipe == null)
                return null;
            var argosRecipe = _mapper.AutoMap.Map<ArgosRecipe>(dbrecipe);

            return argosRecipe;
        }

        private void CheckRecipe(ArgosRecipe recipe)
        {
            //TODO Check if the recipe is valid (all necessary values are presnet and coherent)
        }

        // TODO(JPR): refine default recipe creation
        public Response<ArgosRecipe> CreateDefaultRecipe(string name, int stepID, int userID)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                _logger.Debug("CreateDefaultRecipe");
                var recipe = new ArgosRecipe();

                recipe.Name = name;
                recipe.ActorType = ActorType.Argos;
                recipe.StepId = stepID;
                recipe.UserId = userID;
                recipe.Created = DateTime.Now;
                recipe.CreatorChamberId = _pmConfiguration.ChamberId;

                recipe.Frequency_Hz = 25000; // FIXME(JPR): How to determine default frequency value
                recipe.StartAngle_deg = 0;

                recipe.SensorRecipes.Add(SensorID.Top, SensorRecipe.Default());
                recipe.SensorRecipes.Add(SensorID.TopBevel, SensorRecipe.Default());
                recipe.SensorRecipes.Add(SensorID.Apex, SensorRecipe.Default());
                recipe.SensorRecipes.Add(SensorID.BottomBevel, SensorRecipe.Default());
                recipe.SensorRecipes.Add(SensorID.Bottom, SensorRecipe.Default());

                return recipe;
            });
        }

        public Response<VoidResult> SaveRecipe(ArgosRecipe recipe)
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                _dbRecipeService.SetRecipe(_mapper.AutoMap.Map<DataAccess.Dto.Recipe>(recipe), true);
            });
        }
    }
}
