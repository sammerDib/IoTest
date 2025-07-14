namespace UnitySC.ToolControl.ProcessModules.Devices.ToolControlManager.Recipes
{
    public class DeleteRecipeResult
    {
        #region Properties

        public bool Success { get; }

        public string ErrorMessage { get; }

        #endregion

        #region Constructor

        public DeleteRecipeResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        #endregion
    }
}
