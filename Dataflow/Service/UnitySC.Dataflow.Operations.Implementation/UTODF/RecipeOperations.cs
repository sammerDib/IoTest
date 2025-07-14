using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using UnitySC.DataAccess.Dto;
using UnitySC.Dataflow.Operations.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.Tools;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Dataflow.Operations.Implementation
{
    public class RecipeOperations : IRecipeOperations
    {
        private ILogger _logger;
        private IDFManager _dfManager;

        private List<DataflowRecipeInfo> _recipeList = new List<DataflowRecipeInfo>();
        public List<DataflowRecipeInfo> RecipeList { get => _recipeList; set => _recipeList = value; }

        public RecipeOperations()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<RecipeOperations>>();
        }

        public void Init()
        {
            _dfManager = ClassLocator.Default.GetInstance<IDFManager>();
        }

        private void AddRecipe(DataflowRecipeInfo recipe)
        {
            List<DataflowRecipeInfo> newRecipes = new List<DataflowRecipeInfo>();
            newRecipes.Add(recipe);
            AddRecipes(newRecipes);
        }

        public void AddRecipes(List<DataflowRecipeInfo> recipesList)
        {
            foreach (DataflowRecipeInfo currRecipe in recipesList)
            {
                if (_recipeList.FindIndex(r => (r.IdGuid == currRecipe.IdGuid)) < 0)
                {
                    _recipeList.Add(currRecipe);
                }
            }
            _logger.Debug("Recipes: " + string.Join(",", _recipeList.Select<DataflowRecipeInfo, string>(r => r.Name)));
        }

        private int DeleteRecipes(String recipe)
        {
            List<String> recipes = new List<String>();
            recipes.Add(recipe);
            return DeleteRecipes(recipe);
        }

        public void DeleteRecipes(List<DataflowRecipeInfo> recipesList)
        {
            List<DataflowRecipeInfo> recipesDeletedToSent = new List<DataflowRecipeInfo>();
            foreach (DataflowRecipeInfo currRecipe in recipesList)
            {
                if (_recipeList.Find(r => (r.IdGuid == currRecipe.IdGuid)) != null)
                {
                    _recipeList.Remove(currRecipe);
                }
            }
            _logger.Debug("Recipes deleted: " + string.Join(",", _recipeList.Select<DataflowRecipeInfo, String>(r => r.Name)));
        }

        public void SelectRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            If_DFInMaintenance_Throw("SelectRecipe failed. Dataflow is in maintenance");

            _dfManager.SelectRecipe(dfRecipeInfo);

            If_DFInMaintenance_Throw("SelectRecipe failed. Dataflow is in maintenance");
        }

        public void AbortRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            If_DFInMaintenance_Throw("AbortRecipe failed. Dataflow is in maintenance");

            _dfManager.AbortRecipe(dfRecipeInfo);

            If_DFInMaintenance_Throw("AbortRecipe failed. Dataflow is in maintenance");
        }

        public void ReInitialize()
        {
            _dfManager.ReInitialize();

            If_DFInMaintenance_Throw("ReInitialize failed. Dataflow is in maintenance");
        }

        public void DFRecipeListUpdated(List<ActorType> actors, int toolkey)
        {
            _logger.Debug("UpdateTCRecipeList - Recipes list changed");

            // Get all recipes
            List<DataflowRecipeInfo> allRecipesList = _dfManager.GetAllRecipes(actors, toolkey);

            // Search all new recipes not in RecipeList
            List<DataflowRecipeInfo> newRecipesList = allRecipesList.FindAll(r => !RecipeList.Exists(currRecipe => currRecipe.IdGuid == r.IdGuid));
            List<DataflowRecipeInfo> deletedRecipesList = RecipeList.FindAll(currRecipe => !allRecipesList.Exists(r => currRecipe.IdGuid == r.IdGuid));

            if (newRecipesList != null && newRecipesList.Count != 0)
            {
                // Send recipes to TC and update TRecipeList to know all recipes known by TC
                AddRecipes(newRecipesList);
            }
            if (deletedRecipesList != null && deletedRecipesList.Count != 0)
            {
                // Send recipes to TC and update TRecipeList to know all recipes known by TC
                DeleteRecipes(deletedRecipesList);
            }
        }

        public List<DataflowRecipeInfo> GetAllRecipes(List<ActorType> actors, int toolkey)
        {
            If_DFInMaintenance_Throw("GetAllRecipes failed. Dataflow is in maintenance");

            List<DataflowRecipeInfo> list = _dfManager.GetAllRecipes(actors, toolkey);

            If_DFInMaintenance_Throw("GetAllRecipes failed. Dataflow is in maintenance");

            return list;
        }

        public UTOJobProgram StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid)
        {
            If_DFInMaintenance_Throw("StartRecipeDF failed. Dataflow is in maintenance");

            UTOJobProgram utoJP = _dfManager.StartRecipeDF(dfRecipeInfo, processJobID, wafersGuid);

            If_DFInMaintenance_Throw("StartRecipeDF failed. Dataflow is in maintenance");

            return utoJP;
        }
        public UTOJobProgram GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo)
        {
            If_DFInMaintenance_Throw("GetUTOJobProgramForARecipeDF failed. Dataflow is in maintenance");

            UTOJobProgram utoJP = _dfManager.GetUTOJobProgramForARecipeDF(dfRecipeInfo);

            If_DFInMaintenance_Throw("GetUTOJobProgramForARecipeDF failed. Dataflow is in maintenance");

            return utoJP;
        }

        public void StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {
            If_DFInMaintenance_Throw("StartJob_Material failed. Dataflow is in maintenance");

            _dfManager.StartJob_Material(dfRecipeInfo, wafer);

            If_DFInMaintenance_Throw("StartJob_Material failed. Dataflow is in maintenance");
        }

        private void If_DFInMaintenance_Throw(string msgError)
        {
            if (_dfManager.State == TC_DataflowStatus.Maintenance) 
                throw new Exception(msgError);
        }

        public bool IfAllDataflowActorsAreTerminated(Material wafer)
        {
            return _dfManager.IfAllDataflowActorsAreTerminated(wafer);
        }
        public bool IfAllDataflowPMActorsAreTerminated(Material wafer)
        {
            return _dfManager.IfAllDataflowPMActorsAreTerminated(wafer);
        }

        public DataflowRecipeInfo GetAssociatedDataflowFullInfo(Material wafer)
        {
            return _dfManager.GetAssociatedDataflowFullInfo(wafer);
        }

        public void UpdateDFRecipeInstanceStatus(DataflowRecipeInfo dfRecipeInfo, Material wafer, DataflowRecipeStatus status)
        {
            If_DFInMaintenance_Throw("UpdateDFRecipeInstanceStatus failed. Dataflow is in maintenance");

            _dfManager.UpdateDFRecipeInstanceStatus(dfRecipeInfo, wafer, status);

            If_DFInMaintenance_Throw("UpdateDFRecipeInstanceStatus failed. Dataflow is in maintenance");
        }

        public List<string> GetJobIdsByDFRecipe(DataflowRecipeInfo dfRecipeInfo)
        {
            If_DFInMaintenance_Throw("GetJobIdsByDFRecipe failed. Dataflow is in maintenance");
            
            List<string> jobs = _dfManager.GetJobIdsByDFRecipe(dfRecipeInfo);

            If_DFInMaintenance_Throw("GetJobIdsByDFRecipe failed. Dataflow is in maintenance");

            return jobs;
        }

        public List<JobRecipeInfo> GetRecipesToBeInterrupted(string jobId)
        {
            If_DFInMaintenance_Throw("GetRecipesToBeInterrupted failed. Dataflow is in maintenance");

            List<JobRecipeInfo> jobinfos = _dfManager.GetRecipesToBeInterrupted(jobId);

            If_DFInMaintenance_Throw("GetRecipesToBeInterrupted failed. Dataflow is in maintenance");

            return jobinfos;
        }

        public void UpdateDataflowActorStatusForJobId(string jobId, ActorRecipeStatus status)
        {
            _dfManager.UpdateDataflowActorStatusForJobId(jobId, status);
        }
    }
}
