using System;
using System.ServiceModel;

using UnitySC.PM.AGS.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Interface.RecipeService
{
    [ServiceContract]
    public interface IRecipeService
    {
        [OperationContract]
        Response<ArgosRecipe> GetRecipe(Guid recipeKey, bool useArchived = false);

        [OperationContract]
        Response<VoidResult> SaveRecipe(ArgosRecipe recipe);
    }
}
