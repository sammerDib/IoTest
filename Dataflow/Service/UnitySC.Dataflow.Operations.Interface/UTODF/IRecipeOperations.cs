using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;

using Material = UnitySC.Shared.Data.Material;


namespace UnitySC.Dataflow.Operations.Interface
{
    public interface IRecipeOperations
    {
        List<DataflowRecipeInfo> RecipeList { get; }

        void AbortRecipe(DataflowRecipeInfo dfRecipeInfo);

        void ReInitialize();

        List<DataflowRecipeInfo> GetAllRecipes(List<ActorType> actors, int toolkey);

        void SelectRecipe(DataflowRecipeInfo dfRecipeInfo);

        UTOJobProgram StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid);

        void StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Material wafer);

        void DeleteRecipes(List<DataflowRecipeInfo> recipelist);

        void AddRecipes(List<DataflowRecipeInfo> recipelist);

        bool IfAllDataflowActorsAreTerminated(Material wafer);
        bool IfAllDataflowPMActorsAreTerminated(Material wafer);

        DataflowRecipeInfo GetAssociatedDataflowFullInfo(Material wafer);

        void UpdateDFRecipeInstanceStatus(DataflowRecipeInfo dfRecipeInfo, Material wafer, DataflowRecipeStatus status);

        List<string> GetJobIdsByDFRecipe(DataflowRecipeInfo dfRecipeInfo);

        List<JobRecipeInfo> GetRecipesToBeInterrupted(string jobId);

        void UpdateDataflowActorStatusForJobId(string jobId, ActorRecipeStatus status);

        void Init();
        UTOJobProgram GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo);
    }
}
