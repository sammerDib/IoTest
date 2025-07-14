using System;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class PMActor : DataflowActorManager
    {
        //public IRecipeDFServiceCB PMDataflowManagerCallback { get; set; }

        public PMActor(string id, ActorType actorType) : base(id, actorType)
        {
        }

        public PMStatus Status { get; set; } = PMStatus.Available;

        public override bool IsReadyToStart()
        {
            //Status = PMStatus.RecipeExecuting;

            //return _iWorkflowManagerPM.IsReady();

            return true;
        }


        public override void Reserve()
        {
            Status = PMStatus.IsReserved;
        }

        public override string ToString()
        {
            return string.Format("[PM Actor] ID={0} ActorType={1} State={2}", ID, ActorType, Status);
        }

        public override bool IsAvailable()
        {
            return Status == PMStatus.Available;
        }

        public override bool IsExecuting()
        {
            return Status == PMStatus.RecipeExecuting;
        }

        public override void PrepareStartOfRecipe(string recipeName, Material wafer)
        {
            Status = PMStatus.RecipeExecuting;
        }

        public override void DataAvailable(string actorID, Material wafer, Guid dapToken)
        {
        }

        public override void Ended(string actorID, Material wafer)
        {
            Status = PMStatus.Available;
        }
    }
}
