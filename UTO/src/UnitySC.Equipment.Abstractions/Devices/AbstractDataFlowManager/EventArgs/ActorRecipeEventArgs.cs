using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs
{
    public class ActorRecipeEventArgs : System.EventArgs
    {
        #region Properties

        public ActorType Actor { get; }

        public string RecipeName { get; }

        public RecipeTerminationState RecipeTerminationState { get; }
        #endregion

        #region Constructor

        public ActorRecipeEventArgs(ActorType actor, string recipeName, RecipeTerminationState recipeTerminationState = RecipeTerminationState.unknown)
        {
            Actor = actor;
            RecipeName = recipeName;
            RecipeTerminationState = recipeTerminationState;
        }

        #endregion
    }
}
