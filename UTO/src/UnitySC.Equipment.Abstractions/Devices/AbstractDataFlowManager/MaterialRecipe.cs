using System.Collections.Generic;

using UnitySC.DataAccess.Dto;
using UnitySC.Equipment.Abstractions.Material;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager
{
    public class MaterialRecipe
    {
        #region Properties

        public DataflowRecipeInfo Recipe { get; }

        public List<Wafer> Wafers { get; }

        #endregion

        #region Constructor

        public MaterialRecipe(DataflowRecipeInfo recipe, List<Wafer> wafers)
        {
            Recipe = recipe;
            Wafers = new List<Wafer>(wafers);
        }

        #endregion
    }
}
