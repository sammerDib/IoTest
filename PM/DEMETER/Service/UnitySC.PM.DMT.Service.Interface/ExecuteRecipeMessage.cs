using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.DMT.Service.Interface
{
    public class ExecuteRecipeMessage
    {
        public ExecuteRecipeMessage(Guid actorRecipeID, string dataflowID, ResultType dataType, Guid dapToken)
        {
            ActorRecipeID = actorRecipeID;
            DataflowID = dataflowID;
            DataType = dataType;
            DapToken = dapToken;
        }

        public Guid ActorRecipeID { get; }
        public string DataflowID { get; }
        public ResultType DataType { get; }
        public Guid DapToken { get; }
    }
}
