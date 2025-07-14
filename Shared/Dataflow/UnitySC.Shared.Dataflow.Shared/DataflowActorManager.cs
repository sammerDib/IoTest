using System;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.Dataflow.Shared
{
    public abstract class DataflowActorManager
    {
        public string ID { get; set; }
        public ActorType ActorType { get; set; }

        public DataflowActorManager(string id, ActorType actorType)
        {
            ID = id;
            ActorType = actorType;
        }
        public Identity Identity { get; set; }

        /// <summary>
        /// Check that everything is ready to start
        /// for example, all input data is available
        /// </summary>
        /// <returns></returns>
        public abstract bool IsReadyToStart();

        public abstract bool IsAvailable();

        public abstract bool IsExecuting();

        public abstract void PrepareStartOfRecipe(string recipeName, Material wafer);

        public abstract void DataAvailable(string actorID, Material wafer, Guid dapToken);

        public abstract void Ended(string actorID, Material wafer);

        public abstract void Reserve();
    }
}
