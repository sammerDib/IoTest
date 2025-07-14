using System;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.Shared.TC.PM.Operations.Interface.UTOOperations
{
    public interface IPMDFOperations
    {
        void Init();
        bool CanStartPMRecipe(Identity identity, Material wafer);
        void RecipeExecutionComplete(Identity identity, Material wafer, Guid? recipeKey, string results, RecipeTerminationState status);
        void RecipeStarted(Identity identity, Material wafer);
        DataflowRecipeInfo GetDataflowRecipeInfo(Identity identity, Material wafer);
        Guid? RetrieveTheAssociatedPMRecipeKey(Identity identity, Material wafer);
        DataflowActorRecipe GetADCRecipeForSide(Identity identity, Material wafer, Side waferSide);
        void NotifyDataCollectionChanged(Identity identity, ModuleDataCollection pmDataCollection);
    }
}
