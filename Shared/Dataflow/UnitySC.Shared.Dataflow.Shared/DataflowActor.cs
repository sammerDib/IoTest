using System;
using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.Shared.Dataflow.Shared
{
    public class DataflowActor
    {
        public DataflowActor(string id, ActorType actorType)
        {
            ID = id;
            ActorType = actorType;
        }

        public string ID { get; set; }
        public ActorType ActorType { get; set; }

        /// <summary>
        /// lists of child actors defined in the WF recipe
        /// </summary>
        public List<DataflowActor> ChildActors { get; set; } = new List<DataflowActor>();


        public DataflowActorManager DataflowActorManager { get; set; }

        /// <summary>
        /// actor's recipe, link save in WF recipe (KeyForAllVersion)
        /// </summary>
        public DataflowActorRecipe DataflowActorRecipe { get; set; }

        public List<InputOutputDataType> Inputs { get; set; }

        public List<InputOutputDataType> Outputs { get; set; }
    }

    public class DataflowActorRecipe : IComparable<DataflowActorRecipe>
    {
        public Guid KeyForAllVersion { get; set; }
        public string Name { get; set; }

        public int CompareTo(DataflowActorRecipe other)
        {
            if (other == null)
                return 1;

            int guidComparison = KeyForAllVersion.CompareTo(other.KeyForAllVersion);
            if (guidComparison != 0)
                return guidComparison;
            return 0;
        }

        public static bool operator ==(DataflowActorRecipe dar1, DataflowActorRecipe dar2)
        {
            if (ReferenceEquals(dar1, null) && ReferenceEquals(dar2, null))
                return true;

            if (ReferenceEquals(dar1, null) || ReferenceEquals(dar2, null))
                return false;

            return dar1.KeyForAllVersion == dar2.KeyForAllVersion;
        }
        public static bool operator !=(DataflowActorRecipe dar1, DataflowActorRecipe dar2)
        {
            return !(dar1 == dar2);
        }
    }

    public class DataflowActorValues
    {
        public ActorRecipeStatus DataflowActorStatus { get; set; }

        public DataflowActor DataflowActor { get; set; }

        public DataflowActorRecipe DataflowActorRecipe { get; set; }

        public List<InputOutputValue> InputValues { get; set; }

        public List<InputOutputValue> OutputValues { get; set; }
    }
}
