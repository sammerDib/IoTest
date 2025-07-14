using System;
using System.Runtime.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe
{
    [DataContract]
    public class ANARecipeWithExecContext
    {
        [DataMember]
        public ANARecipe Recipe { get; set; }

        [DataMember]
        public string JobId { get; set; }

        [DataMember]
        public string DFRecipeName { get; set; }

        [DataMember]
        public DateTime PMStartRecipeTime { get; set; }
    }
}
