using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Proxy
{
    public class DbRecipeServiceProxy
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;

        public DbRecipeServiceProxy()
        {
            _dbRecipeService = new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), null, ClassLocator.Default.GetInstance<ModuleConfiguration>().DataAccessAddress);
        }

        public Chamber GetChamberFromKeys(int toolKey, int chamberKey)
        {
            return _dbRecipeService.Invoke(s => s.GetChamberFromKeys(toolKey, chamberKey));
        }

        public Recipe GetLastRecipe(Guid recipeKey, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            return _dbRecipeService.Invoke(s => s.GetLastRecipe(recipeKey, includeRecipeFileInfos, takeArchivedRecipes));
        }

        public Recipe GetLastRecipeWithProductAndStep(Guid recipeKey)
        {
            return _dbRecipeService.Invoke(s => s.GetLastRecipeWithProductAndStep(recipeKey));
        }

        public int GetRecipeId(Guid key, int version)
        {
            return _dbRecipeService.Invoke(s => s.GetRecipeId(key, version));
        }

        public List<RecipeInfo> GetRecipeList(ActorType actorType, int stepId, int chamberKey, int toolKey)
        {
            return _dbRecipeService.Invoke(s => s.GetRecipeList(actorType, stepId, chamberKey, toolKey, false, false));
        }

        public List<TCPMRecipe> GetTCRecipeList(ActorType actorType, int toolKey)
        {
            return _dbRecipeService.Invoke(s => s.GetTCPMRecipeList(actorType, toolKey));
        }

        public Recipe GetRecipeWithTC(string tcPMRecipeName)
        {
            return _dbRecipeService.Invoke(s => s.GetPMRecipeWithTC(tcPMRecipeName));
        }

        public int SetRecipe(Recipe recipe, bool incrementVersion)
        {
            return _dbRecipeService.Invoke(s => s.SetRecipe(recipe, incrementVersion));
        }

        public ExternalFileBase GetExternalFile(string fileNameKey, Guid recipeKey)
        {
            return _dbRecipeService.Invoke(s => s.GetExternalFile(fileNameKey, recipeKey));
        }

        public List<Recipe> GetADCRecipes(string recipeName = null, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            return _dbRecipeService.Invoke(s => s.GetADCRecipes(recipeName, includeRecipeFileInfos, takeArchivedRecipes));
        }
    }
}
