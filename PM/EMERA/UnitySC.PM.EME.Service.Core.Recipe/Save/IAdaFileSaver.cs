using System.Collections.Generic;

namespace UnitySC.PM.EME.Service.Core.Recipe.Save
{
    public interface IAdaFileSaver
    {
        void GenerateFile(RecipeAdapter recipe, List<IAcquisitionImageResult> acquisitionResults);
    }
}
