using System.Collections.Generic;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Recipes
{
    public class DataFlowProduct
    {
        #region Properties

        public string Name { get; }

        public List<DataFlowStep> Steps { get; }

        #endregion

        #region Constructor

        public DataFlowProduct(string name, List<DataFlowStep> steps)
        {
            Name = name;
            Steps = new List<DataFlowStep>(steps);
        }

        #endregion
    }
}
