
using System;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Interface.Recipe
{
    [ServiceContract(CallbackContract = typeof(IADCRecipeServiceCallback))]
    public interface IADCRecipeService
    {
        [OperationContract]
        Response<VoidResult> Test();

        [OperationContract]
        Response<ADCRecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0);

        [OperationContract]
        Response<ADCRecipe> GetRecipeFromKey(Guid recipeKey, bool takeArchivedRecipes = false);

        /// <summary>
        /// Ajoute ou met à jour une recette dans la base de données
        /// </summary>
        [OperationContract]
        Response<VoidResult> SaveRecipe(ADCRecipe recipe);
    }
}
