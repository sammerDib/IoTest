namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.Recipes
{
    public class RecipeUploadResult
    {
        #region Properties

        public string RecipeName { get; }

        public bool Success { get; }

        public string ErrorMessage { get; }

        #endregion

        #region Constructor

        public RecipeUploadResult(string recipeName, bool success, string errorMessage)
        {
            RecipeName = recipeName;
            Success = success;
            ErrorMessage = errorMessage;
        }

        #endregion
    }
}
