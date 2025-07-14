using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager
{
    public partial class AbstractDataFlowManager
    {
        protected abstract void InternalSimulateStartRecipe(MaterialRecipe materialRecipe, string processJobId, Tempomat tempomat);
        protected abstract void InternalSimulateAbortRecipe(string jobId, Tempomat tempomat);
        protected abstract void InternalSimulateStartJobOnMaterial(DataAccess.Dto.DataflowRecipeInfo recipe, Material.Wafer wafer, Tempomat tempomat);
        protected abstract void InternalSimulateGetAvailableRecipes(Tempomat tempomat);
    }
}