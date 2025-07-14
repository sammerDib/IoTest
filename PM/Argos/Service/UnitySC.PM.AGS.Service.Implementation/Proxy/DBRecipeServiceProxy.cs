using System;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Implementation.Proxy
{
    public class DBRecipeServiceProxy
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;


        public DBRecipeServiceProxy()
        {
            _dbRecipeService = new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), null);
        }
        
        public Recipe GetLastRecipe(Guid recipeKey, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            return _dbRecipeService.Invoke(s => s.GetLastRecipe(recipeKey, includeRecipeFileInfos, takeArchivedRecipes));
        }

        public void SetRecipe(Recipe recipe, bool incrementVersion)
        {
            _dbRecipeService.Invoke(s => s.SetRecipe(recipe, incrementVersion));
        }
    }
}
