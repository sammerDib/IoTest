using System;
using System.ServiceModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Interface.Recipe
{
    [ServiceContract(CallbackContract = typeof(IANARecipeServiceCallback))]
    public interface IANARecipeService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        [PreserveReferences]

        Response<ANARecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0);

        [OperationContract]
        [PreserveReferences]
        Response<ANARecipe> GetRecipeFromKey(Guid recipeKey);

        /// <summary>
        /// Add or update recipe in DataBase
        /// </summary>
        [OperationContract]
        [PreserveReferences]
        Response<int> SaveRecipe(ANARecipe recipe, bool incrementVersion, int userId);

        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> StartRecipe(ANARecipe recipe, int nbRuns = 1);

        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> StopRecipe();

        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> PauseRecipe();

        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> ResumeRecipe();

        [OperationContract]
        [PreserveReferences]
        Response<TimeSpan> GetEstimatedTime(ANARecipe recipe, int nbRuns = 1);

        [OperationContract]
        Response<VoidResult> SubscribeToRecipeChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToRecipeChanges();

        [OperationContract]
        Response<VoidResult> SaveCurrentResultInProductionDatabase(string lotName);
    }
}
