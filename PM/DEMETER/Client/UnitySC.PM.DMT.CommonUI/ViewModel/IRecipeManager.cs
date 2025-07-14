using System;

using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.Shared.Data;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public interface IRecipeManager
    {
        bool SetEditedRecipe(Guid recipeKey, bool isNewRecipe, bool forceReload=false);

        RecipeInfo SaveRecipe();

        void ExportRecipe(Guid recipeKey, string folderPath);

        RecipeInfo ImportRecipe(int stepId, int userId, string fileName);

        bool CanClose();

        void EndRecipeEdition();

        event EventHandler OnEndRecipeEdition;
    }
}
