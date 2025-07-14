using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data;

namespace UnitySC.PP.ADC.Service.Interface.Recipe
{

    [DataContract(Namespace = "")]
    public class ADCRecipe : RecipeInfo
    {



        [XmlIgnore]
        public ADCEngine.Recipe ADCEngineRecipe { get; set; }


        [DataMember]

        public string ADCEngineRecipeXml { get; set; }


    }
}
