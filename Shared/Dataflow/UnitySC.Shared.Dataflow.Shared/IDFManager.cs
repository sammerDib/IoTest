using System;
using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.Dataflow.Shared
{
    public interface IDFManager
    {
        TC_DataflowStatus State { get; }
        void SelectRecipe(DataflowRecipeInfo dfRecipeInfo);

        void AbortRecipe(DataflowRecipeInfo dfRecipeInfo);

        void ReInitialize();

        List<DataflowRecipeInfo> GetAllRecipes(List<ActorType> actors, int toolId);

        UTOJobProgram StartRecipeDF(DataflowRecipeInfo dfRecipeInfo, string processJobID, List<Guid> wafersGuid);

        void StartJob_Material(DataflowRecipeInfo dfRecipeInfo, Data.Material wafer);

        bool IfAllDataflowActorsAreTerminated(Data.Material wafer);
        bool IfAllDataflowPMActorsAreTerminated(Data.Material wafer);

        DataflowRecipeInfo GetAssociatedDataflowFullInfo(Data.Material wafer);

        bool CanStartPMRecipe(Identity identity, Data.Material wafer);

        void RecipeExecutionComplete(Identity identity, Data.Material wafer, Guid? recipeKey, string results, RecipeTerminationState status);

        void UpdateDFRecipeInstanceStatus(DataflowRecipeInfo dfRecipeInfo, Data.Material wafer, DataflowRecipeStatus status);

        void UpdatePMActorStatusByWafer(Identity identity, Data.Material wafer, ActorRecipeStatus status);

        DataflowRecipeInfo GetDataflowRecipeInfo(Identity identity, string processJobID, Guid materialGuid);

        Guid? RetrieveTheAssociatedPMRecipeKey(Identity identity, Data.Material wafer);

        DataflowActorRecipe GetPPRecipeByMaterialAndBySide(Identity identity, Data.Material wafer, Side waferSide);

        void RecipeStarted(Identity identity, Data.Material wafer);

        List<string> GetJobIdsByDFRecipe(DataflowRecipeInfo dfRecipeInfo);

        List<JobRecipeInfo> GetRecipesToBeInterrupted(string jobId);

        void UpdateDataflowActorStatusForJobId(string jobId, ActorRecipeStatus status);
        void Init();

        Identity Identity { get; }

        UTOJobProgram GetUTOJobProgramForARecipeDF(DataflowRecipeInfo dfRecipeInfo);

    
    }
}
