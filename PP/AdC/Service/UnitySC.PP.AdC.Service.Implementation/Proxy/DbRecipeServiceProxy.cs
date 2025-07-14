using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Implementation.Proxy
{
    public class DbRecipeServiceProxy
    {
        private ServiceInvoker<IDbRecipeService> _dbRecipeService;

        public DbRecipeServiceProxy()
        {
            _dbRecipeService = new ServiceInvoker<IDbRecipeService>("RecipeService", ClassLocator.Default.GetInstance<SerilogLogger<IDbRecipeService>>(), null);
        }

        public Recipe GetLastRecipe(Guid recipeKey, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            return _dbRecipeService.Invoke(s => s.GetLastRecipe(recipeKey, includeRecipeFileInfos, takeArchivedRecipes));
        }

        public List<TCPMRecipe> GetTCRecipeList(int toolId)
        {
            return _dbRecipeService.Invoke(s => s.GetTCPMRecipeList(ActorType.ANALYSE, toolId));
        }

        public Recipe GetRecipeWithTC(string tcPMRecipeName)
        {
            return _dbRecipeService.Invoke(s => s.GetPMRecipeWithTC(tcPMRecipeName));
        }

        public void SetRecipe(Recipe recipe, bool incrementVersion)
        {
            _dbRecipeService.Invoke(s => s.SetRecipe(recipe, incrementVersion));
        }
    }
}
