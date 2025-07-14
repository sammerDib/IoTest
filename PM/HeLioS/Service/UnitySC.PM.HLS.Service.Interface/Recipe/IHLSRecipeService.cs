using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;

namespace UnitySC.PM.HLS.Service.Interface.Recipe
{
    public delegate void RecipeAddedEventHandler(object sender, HLSRecipe recipe);

    [ServiceContract(CallbackContract = typeof(IHLSRecipeServiceCallback))]
    public interface IHLSRecipeService
    {
        /// <summary>
        /// Event to indicate Recipe added
        /// </summary>
        event RecipeAddedEventHandler RecipeAdded;

        [OperationContract]
        Response<VoidResult> Test();

        [OperationContract]
        Response<List<RecipeInfo>> GetRecipeList(bool takeArchivedRecipes = false);

        [OperationContract]
        Response<List<TCPMRecipe>> GetTCRecipeList();

        [OperationContract]
        Response<HLSRecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0);

        [OperationContract]
        Response<HLSRecipe> GetRecipeFromKey(Guid recipeKey, bool takeArchivedRecipes = false);

        /// <summary>
        /// Ajoute ou met à jour une recette dans la base de données
        /// </summary>
        [OperationContract]
        Response<VoidResult> SaveRecipe(HLSRecipe recipe);
    }
}
