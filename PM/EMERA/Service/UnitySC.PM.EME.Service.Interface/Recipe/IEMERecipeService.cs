using System;
using System.ServiceModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.EME.Service.Interface.Recipe
{
    [ServiceContract(CallbackContract = typeof(IEMERecipeServiceCallback))]
    public interface IEMERecipeService
    {       
        [OperationContract]
        [PreserveReferences]
        Response<EMERecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0);

        [OperationContract]
        [PreserveReferences]
        Response<EMERecipe> GetRecipeFromKey(Guid recipeKey);

        /// <summary>
        /// Add or update recipe in DataBase
        /// </summary>
        [OperationContract]
        [PreserveReferences]
        Response<int> SaveRecipe(EMERecipe recipe, bool incrementVersion, int userId);

        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> StartRecipe(EMERecipe recipe, string customSavePath);

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
        Response<VoidResult> SubscribeToRecipeChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToRecipeChanges();

        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> StartCycling(EMERecipe recipe, string customSavePath);

        [OperationContract]
        Response<VoidResult> StopCycling();
    }
}
