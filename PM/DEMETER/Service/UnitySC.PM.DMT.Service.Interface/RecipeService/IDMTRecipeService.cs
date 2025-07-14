using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Service.Interface
{
    public delegate void RecipeAddedEventHandler(object sender, DMTRecipe recipe);

    public delegate void ReportProgressEventHandler(object sender, RecipeStatus recipeStatus);

    [ServiceContract(CallbackContract = typeof(IDMTRecipeServiceCallback))]
    public interface IDMTRecipeService
    {
        /// <summary>
        ///     Event to indicate Recipe added
        /// </summary>
        event RecipeAddedEventHandler RecipeAdded;

        event ReportProgressEventHandler Progress;

        [OperationContract]
        Response<VoidResult> Subscribe();

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<VoidResult> Test();

        [OperationContract]
        [PreserveReferences]
        Response<List<RecipeInfo>> GetRecipeList(int stepId, bool takeArchivedRecipes = false);

        [OperationContract]
        Response<List<TCPMRecipe>> GetTCRecipeList();

        [OperationContract]
        [PreserveReferences]
        Response<DMTRecipe> CreateRecipe(string name = null, int stepId = -1, int userId = 0);

        //[OperationContract]
        //Response<DMTRecipe> GetRecipeWithTC(string name);

        [OperationContract]
        [PreserveReferences]
        Response<DMTRecipe> GetRecipeFromKey(Guid recipeKey, bool takeArchivedRecipes = false);

        [OperationContract]
        [PreserveReferences]
        Response<DMTRecipe> GetLastRecipeWithProductAndStep(Guid recipeKey);

        /// <summary>
        ///     Ajoute ou met à jour une recette dans la base de données
        /// </summary>
        [OperationContract]
        [PreserveReferences]
        Response<VoidResult> SaveRecipe(DMTRecipe recipe);
        
        [OperationContract]
        [PreserveReferences]
        Response<DMTRecipe> ImportRecipe(DMTRecipe recipe, int stepId, int userId);

        [OperationContract]
        [PreserveReferences]
        Response<DMTRecipe> GetRecipeForExport(Guid recipeKey);

        [OperationContract]
        [PreserveReferences]
        Task<Response<VoidResult>> StartRecipeAsync(
            DMTRecipe recipe, string acqdestFolder, bool overwriteOutput,
            string dataflowID);

        [OperationContract]
        Response<VoidResult> Abort();

        [OperationContract]
        Task<Response<Dictionary<CurvatureImageType, ServiceImage>>> BaseCurvatureDarkDynamicsAcquisition(
            DeflectometryMeasure measure, Length waferDiameter, bool isDarkRequired);

        [OperationContract]
        Task<Response<Dictionary<CurvatureImageType, ServiceImage>>> RecalculateCurvatureDynamics(
            DeflectometryMeasure measure);

        [OperationContract]
        Task<Response<ServiceImage>> RecalculateDarkDynamics(DeflectometryMeasure measure);

        [OperationContract]
        Response<VoidResult> DisposeCurvatureDarkDynamicsAdjustmentMeasureExecution();

        [OperationContract]
        Response<RemoteProductionInfo> GetDefaultRemoteProductionInfo();

        void ReportProgress(object sender, RecipeStatus status);

        void ResultGenerated(object sender, DMTResultGeneratedEventArgs args);
    }
}
