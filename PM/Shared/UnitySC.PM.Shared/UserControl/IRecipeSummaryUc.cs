using System;
using System.ComponentModel.Composition;

namespace UnitySC.PM.Shared.UC
{
    /// <summary>
    /// Readonly user control for the recipe summary
    /// </summary>
    [InheritedExport(typeof(IRecipeSummaryUc))] // Export all classes inherited this interface (MEF)
    public interface IRecipeSummaryUc : IPmUc
    {
        void LoadRecipe(Guid key);

        void Refresh();
    }
}
