using UnitySC.Shared.Data;

namespace UnitySC.Shared.TC.PM.Operations.Interface
{
    public interface IANAHandling : IHandling
    {
        void PMClampMaterial(Material material);
        void PMUnclampMaterial(Material material);
    }
}
