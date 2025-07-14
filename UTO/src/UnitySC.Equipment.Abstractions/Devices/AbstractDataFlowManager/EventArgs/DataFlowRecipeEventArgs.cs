using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Dataflow.Shared;

namespace UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager.EventArgs
{
    public class DataFlowRecipeEventArgs : System.EventArgs
    {
        #region Properties

        public DataflowRecipeInfo Recipe { get; }
        public string ProcessJobId { get; }
        public string SubstrateId { get; }
        public DataflowRecipeStatus Status { get; }

        #endregion

        #region Constructor

        public DataFlowRecipeEventArgs(DataflowRecipeInfo recipe, string processJobId = "", string substrateId = "", DataflowRecipeStatus status = DataflowRecipeStatus.Unknow)
        {
            Recipe = recipe;
            ProcessJobId = processJobId;
            SubstrateId = substrateId;
            Status = status;
        }

        #endregion
    }
}
