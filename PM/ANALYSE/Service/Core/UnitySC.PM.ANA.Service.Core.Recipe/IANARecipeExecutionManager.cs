using System;
using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Metro;

using static UnitySC.PM.ANA.Service.Core.Recipe.ANARecipeExecutionManager;

namespace UnitySC.PM.ANA.Service.Core.Recipe
{
    public interface IANARecipeExecutionManager
    {
        /// <summary>
        /// Execute recipe
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="automationInfo"> If null the result is not saved in database</param>
        /// <returns>Key => MeasureName, Value => Result </returns>
        Dictionary<string, MetroResult> Execute(ANARecipe recipe, RemoteProductionInfo automationInfo = null, int nbRuns = 1);

        TimeSpan GetEstimatedExecutionTime(ANARecipe recipe, int nbRuns);

        void StopRecipe();

        void PauseRecipe();

        void ResumeRecipe();

        void SaveCurrentResultInDatabase(string lotName);

        ANARecipe Convert_RecipeToAnaRecipe(DataAccess.Dto.Recipe dbrecipe);

        DataAccess.Dto.Recipe Convert_AnaRecipeToRecipe(ANARecipe anaRecipe);

        void NotifyRecipeExecutionFailed();
    }
}
