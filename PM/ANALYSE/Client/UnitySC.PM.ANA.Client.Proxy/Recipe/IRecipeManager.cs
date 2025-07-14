using System;
using System.IO.Packaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.Shared.Data;

namespace UnitySC.PM.ANA.Client.Proxy.Recipe
{
    public interface IRecipeManager
    {
        bool SetEditedRecipe(Guid recipeKey, bool isNewRecipe, bool forceReload = false);

        bool SaveRecipe();

        bool CanClose();

        void EndRecipeEdition();

        void ExportRecipe(Guid recipeKey, string folderPath);

        RecipeInfo ImportRecipe(int stepId, int userId, string fileName);

        event EventHandler OnEndRecipeEdition;

        ANARecipeVM EditedRecipe { get; set; }
    }
}
