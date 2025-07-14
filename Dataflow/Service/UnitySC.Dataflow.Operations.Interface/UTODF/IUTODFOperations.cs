using UnitySC.Dataflow.Operations.Interface.UTODF;
using UnitySC.Shared.TC.Shared.Operations.Interface;

namespace UnitySC.Dataflow.Operations.Interface
{
    public interface IUTODFOperations : IUTOBaseOperations
    {
        IRecipeOperations RecipeOperations { get; }
        IDFStatusVariableOperations SVOperations { get; }
    }
}
