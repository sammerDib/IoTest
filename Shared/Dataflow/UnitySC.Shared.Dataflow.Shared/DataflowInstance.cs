using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class DataflowInstance
    {
        private Material _wafer;
        public Material Wafer { get => _wafer; set => _wafer = value; }

        private List<DataflowActorValues> _dataflowActorValues = new List<DataflowActorValues>();
        public List<DataflowActorValues> DataflowActorValues { get => _dataflowActorValues; }

        private DataflowRecipeStatus _status = DataflowRecipeStatus.Available;
        public DataflowRecipeStatus Status { get => _status; set => _status = value; }

        public DataflowInstance(DataflowRecipe dataflowRecipe)
        {
            _dataflowActorValues = new List<DataflowActorValues>(
                dataflowRecipe.Actors.Values.Select(a =>
                    new DataflowActorValues()
                    {
                        DataflowActor = a,
                        DataflowActorStatus = ActorRecipeStatus.Available,
                        DataflowActorRecipe = a.DataflowActorRecipe,
                        InputValues = a.Inputs.Select(i => new InputOutputValue(i)).ToList(),
                        OutputValues = a.Outputs.Select(i => new InputOutputValue(i)).ToList()
                    }
                ));
        }

        public void ChangePMActorStatus(ActorType actorType, ActorRecipeStatus actorStatus)
        {
            if (actorType.GetCatgory() == ActorCategory.ProcessModule)
            {
                DataflowActorValues dataflowActorValues = _dataflowActorValues.FirstOrDefault(a => a.DataflowActor.ActorType == actorType);

                if (DataflowActorValues != null)
                {
                    dataflowActorValues.DataflowActorStatus = actorStatus;
                }
            }
        }

        public void ChangePPActorStatus(ActorType actorType, DataflowActorRecipe recipe, ActorRecipeStatus actorStatus)
        {
            if (actorType.GetCatgory() == ActorCategory.PostProcessing)
            {
                DataflowActorValues dataflowActorValues = _dataflowActorValues.FirstOrDefault(a => (a.DataflowActor.ActorType == actorType) && (a.DataflowActorRecipe == recipe));

                if (DataflowActorValues != null)
                {
                    dataflowActorValues.DataflowActorStatus = actorStatus;
                }
            }
        }

        public ActorRecipeStatus RetrieveActorStatus(ActorType actorType)
        {
            DataflowActorValues dataflowActorValues = _dataflowActorValues.FirstOrDefault(a => a.DataflowActor.ActorType == actorType);

            if (DataflowActorValues != null)
            {
                return dataflowActorValues.DataflowActorStatus;
            }
            return ActorRecipeStatus.Unknow;
        }

        public void AssignIdentity(Identity identity)
        {
            DataflowActorValues dataflowActorValues = _dataflowActorValues.FirstOrDefault(a => a.DataflowActor.ActorType == identity.ActorType);

            if (DataflowActorValues != null)
            {
                dataflowActorValues.DataflowActor.DataflowActorManager.Identity = identity;
            }
        }
    }
}
