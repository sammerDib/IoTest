using System.Collections.Generic;

using UnitySC.DataAccess.Dto;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Recipes
{
    public class DataFlowStep
    {
        #region Properties

        public string Name { get; }

        public List<DataflowRecipeInfo> Recipes { get; }

        #endregion

        #region Constructor

        public DataFlowStep(string name, List<DataflowRecipeInfo> recipes)
        {
            Name = name;
            Recipes = new List<DataflowRecipeInfo>(recipes);
        }

        #endregion
    }
}
