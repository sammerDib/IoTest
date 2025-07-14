using System.Runtime.Serialization;

using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.Data
{
    [DataContract(Namespace = "")]
    public class PmRecipe : RecipeInfo, IPmRecipe
    {
        [DataMember]
        public string Content { get; set; }

    }
}
