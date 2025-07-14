using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Interface.Recipe
{
    [ServiceContract]
    public interface IANARecipeServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        [PreserveReferences]
        void RecipeStarted(ANARecipeWithExecContext startedRecipe );

        [OperationContract(IsOneWay = true)]
        void RecipeProgressChanged(RecipeProgress recipeProgress);
        
        [OperationContract(IsOneWay = true)]
        void MeasureResultChanged(MeasurePointResult res, string resultFolderPath, DieIndex dieIndex);

        [OperationContract(IsOneWay = true)]
        void RecipeFinished(List<MetroResult> results);
    }
}
