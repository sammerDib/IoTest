using UnitySC.Shared.Dataflow.Shared;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs
{
    public class ProcessModuleRecipeEventArgs : System.EventArgs
    {
        #region Properties

        public DriveableProcessModule.DriveableProcessModule ProcessModule { get; }

        public string RecipeName { get; }

        public RecipeTerminationState RecipeTerminationState { get; }

        #endregion

        #region Constructor

        public ProcessModuleRecipeEventArgs(DriveableProcessModule.DriveableProcessModule processModule, string recipeName, RecipeTerminationState recipeTerminationState = RecipeTerminationState.unknown)
        {
            ProcessModule = processModule;
            RecipeName = recipeName;
            RecipeTerminationState = recipeTerminationState;
        }

        #endregion
    }
}
