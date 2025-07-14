using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Interface
{
    /// <summary>
    /// Service pour accéder aux recettes
    /// </summary>
    [ServiceContract]
    public interface IDbRecipeService
    {
        [OperationContract]
        [PreserveReferences]
        Response<Chamber> GetChamberFromKeys(int toolKey, int chamberKey);

        #region Recipe

        [Obsolete]
        [OperationContract]
        Response<List<RecipeInfo>> GetRecipeList_Obsolete(ActorType actorType, int stepId, int chamberId, bool takeArchivedRecipes = false, bool takeTemplateRecipes = false);

        [OperationContract]
        Response<List<RecipeInfo>> GetRecipeList(ActorType actorType, int stepId, int chamberKey, int toolKey, bool takeArchivedRecipes = false, bool takeTemplateRecipes = false);

        [OperationContract]
        Response<List<TCPMRecipe>> GetTCPMRecipeList(ActorType actorType, int toolKey);

        [OperationContract]
        Response<List<RecipeInfo>> GetCompatibleRecipes(Guid? parentRecipeKey, int stepId, int toolKey);

        [OperationContract(Name = "GetRecipeByName")]
        [PreserveReferences]
        Response<Recipe> GetRecipe(ActorType actorType, int stepId, string recipeName, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false);

        [OperationContract(Name = "GetRecipeById")]
        [PreserveReferences]
        Response<Recipe> GetRecipe(int id, bool includeRecipeFileInfos = false);

        [OperationContract(Name = "GetPMRecipeWithTC")]
        [PreserveReferences]
        Response<Recipe> GetPMRecipeWithTC(string tcPMRecipeName);

        [OperationContract(Name = "GetDataflowRecipeWithTC")]
        [PreserveReferences]
        Response<DataflowRecipeComponent> GetDataflowWithTC(string tcRecipeName);

        [OperationContract]
        Response<RecipeInfo> GetRecipeInfo(int id);

        [OperationContract]
        Response<int> GetRecipeId(Guid key, int version);

        [OperationContract]
        Response<int> GetRecipeVersion(ActorType actorType, int stepId, string recipeName, bool takeArchivedRecipes = false);

        /// <summary>
        /// Retourne le status Archivée/Disponible pour toutes les versions d'une recette.
        /// </summary>
        [OperationContract]
        Response<List<bool>> GetArchivedStatus(ActorType actorType, int recipeId);

        [OperationContract]
        [PreserveReferences]
        Response<int> SetRecipe(Recipe recipe, bool incrementVersion = true);

        [OperationContract]
        Response<int> CloneRecipe(Guid key, string newName, int userId);

        [OperationContract]
        Response<VoidResult> UpdateArchivedRecipes(Dictionary<int, bool> recipeIdArchiveState, int userId);

        [OperationContract(Name = "GetLastRecipeByKey")]
        [PreserveReferences]
        Response<Recipe> GetLastRecipe(Guid key, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false);

        [OperationContract(Name = "GetLastRecipeByName")]
        [PreserveReferences]
        Response<Recipe> GetLastRecipe(ActorType actorType, int stepId, string name, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false);

        [OperationContract]
        [PreserveReferences]
        Response<Recipe> GetLastRecipeWithProductAndStep(Guid key);

        [OperationContract]
        Response<VoidResult> ChangeRecipeSharedState(Guid key, int userId, bool isShared);

        [OperationContract]
        Response<VoidResult> ChangeRecipeValidateState(Guid key, int userId, bool isValidated);

        [OperationContract]
        Response<VoidResult> ArchiveAllVersionOfRecipe(Guid key, int userId);

        [OperationContract]
        Response<VoidResult> RestoreAllVersionOfRecipe(Guid key, int userId);

        [OperationContract]
        Response<int> SetExternalFile(ExternalFileBase externalFile, int recipeId, int userId);

        [OperationContract]
        Response<ExternalFileBase> GetExternalFile(string fileNameKey, Guid recipeKey);

        [OperationContract]
        Response<List<ExternalFileBase>> GetExternalFiles(Guid recipeKey);

        // pour ADC Configuration a voir si on garde plus tard...
        [OperationContract]
        Response<List<Recipe>> GetADCRecipes(string recipeName, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false);

        #endregion Recipe

        #region Dataflow

        [OperationContract]
        Response<int> SetDataflow(DataflowRecipeComponent dataflowRecipe, int userId, int stepId, int toolKey, bool incrementVersion = true);

        [OperationContract]
        Response<DataflowRecipeComponent> GetDataflow(int dataflowId);

        [OperationContract(Name = "GetLastDataflowByName")]
        Response<DataflowRecipeComponent> GetLastDataflow(string name, int stepId);

        [OperationContract(Name = "GetLastDataflowByKey")]
        Response<DataflowRecipeComponent> GetLastDataflow(Guid key, bool takeArchivedDataflow = false);

        [OperationContract]
        Response<bool> IsDataflowNameAlreadyUsedByOtherRecipe(string dataflowName, Guid key, bool takeArchivedDataflow = false);

        [OperationContract]
        Response<List<DataflowInfo>> GetDataflowInfos(int stepId, int toolKey, bool takeArchivedDataflow = false);

        [OperationContract]
        Response<List<DataflowRecipeInfo>> GetAllDataflow(List<ActorType> actors, int toolKey, bool takeArchivedDataflow = false);

        [OperationContract]
        Response<VoidResult> ArchiveAllVersionOfDataflow(Guid key, int userId);

        [OperationContract]
        Response<VoidResult> RestoreAllVersionOfDataflow(Guid key, int userId);

        [OperationContract]
        Response<VoidResult> ChangeDataflowSharedState(Guid key, int userId, bool isShared);

        #endregion Dataflow

        [OperationContract]
        Response<bool> IsConnectionAvailable();

        [OperationContract]
        Response<bool> CheckDatabaseVersion();

        
    }
}
