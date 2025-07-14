using System.IO;

namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.Recipes
{
    public class RecipeDownloadResult
    {
        #region Properties

        public string RecipeName { get; }

        public bool Success { get; }

        public string ErrorMessage { get; }

        public MemoryStream Recipe { get; }

        #endregion

        #region Constructor

        public RecipeDownloadResult(
            string recipeName,
            bool success,
            string errorMessage,
            MemoryStream recipe)
        {
            RecipeName = recipeName;
            Success = success;
            ErrorMessage = errorMessage;
            Recipe = recipe;
        }

        #endregion
    }
}
