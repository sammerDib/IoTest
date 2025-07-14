using System;

using UnitySC.Shared.Data;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public interface IRecipeManager
    {
        bool SetEditedRecipe(Guid recipeKey, bool isNewRecipe, bool forceReload = false);

        bool SaveRecipe();

        bool CanClose();
        
        void ExportRecipe(Guid recipeKey, string folderPath);

        RecipeInfo ImportRecipe(int stepId, int userId, string fileName);
        
        EMERecipeVM EditedRecipe { get; set; }
    }
}
