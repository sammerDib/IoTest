using System;
using System.ComponentModel.Composition;
using UnitySC.Shared.Data;


namespace UnitySC.PM.Shared.UC
{
    [InheritedExport(typeof(IRecipeRunLiveViewUc))] // Export all classes inherited this interface (MEF)
    public interface IRecipeRunLiveViewUc : IPmUc
    {
         void Display();

        void Hide();
    }
}
