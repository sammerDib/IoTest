
using UnitySC.Shared.Data;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IMaterialOperationsCB
    {
        void MoveToLoadingUnloadingPosition_msc(MaterialTypeInfo materialTypeInfo);

        void OnTransferMaterialStarted_msc();

        void OnTransferMaterialFinished_msc(string failedReason);

        void OnMaterialChanged_msc(Material material);

        void StartRecipeRequest();

        void AbortRecipeRequest();
    }
}
