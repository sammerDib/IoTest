using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using UnitySC.Shared.Data;


namespace UnitySC.PM.Shared.UC
{
    [InheritedExport(typeof(IRecipeEditorUc))] // Export all classes inherited this interface (MEF)
    public interface IRecipeEditorUc : IPmUc
    {
        void LoadRecipe(Guid key);

        Task ExportRecipeAsync(Guid key);

        Task<RecipeInfo> ImportRecipeAsync(int stepId, int userId);

        event EventHandler ExitEditor;

        RecipeInfo CreateNewRecipe(string name, int stepId, int userId);

        bool CanClose();
    }
}
