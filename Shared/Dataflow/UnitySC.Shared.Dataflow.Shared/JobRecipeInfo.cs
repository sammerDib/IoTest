using System;
using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.Dataflow.Shared
{
    [DataContract]
    public class JobRecipeInfo
    {
        public JobRecipeInfo()
        {

        }
        public JobRecipeInfo(Identity identity, Guid? recipeKey, Data.Material wafer)
        {
            Identity = identity;
            RecipeKey = recipeKey;
            Wafer = wafer;
        }

        [DataMember]
        public Identity Identity { get; set; }

        [DataMember]
        public Guid? RecipeKey { get; set; }

        [DataMember]
        public Data.Material Wafer { get; set; }

        [DataMember]
        public DataflowRecipeInfo DataflowRecipeInfo { get; set; }

        public override string ToString()
        {
            return string.Format("[JobRecipeInfo] identity={0} RecipeKey={1} Wafer={2}", Identity.ToString(), RecipeKey, Wafer.ToString());
        }

    }
}
