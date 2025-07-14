using System.ServiceModel;

using UnitySC.PM.DMT.Service.Interface.Recipe;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface.RecipeService
{
    public interface IDMTRecipeServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecipeProgress(RecipeStatus status);

        [OperationContract(IsOneWay = true)]
        void ResultGenerated(string name, Side side, string path);
    }
}
