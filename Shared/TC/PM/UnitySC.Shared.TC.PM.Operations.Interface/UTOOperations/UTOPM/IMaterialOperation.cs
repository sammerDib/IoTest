using UnitySC.Shared.Data;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IMaterialOperations
    {
        void Init();

        void LoadMaterial(Material wafer);

        void PostTransfer();

        bool PrepareForTransfer(TransferType transferType, MaterialTypeInfo materialTypeInfo);

        void StartTransfer();
        void StartRecipe();
        void AbortRecipe();
        Material UnloadMaterial();

        void ResetTransferInProgress();

        bool PMInitialization();
    }
}
