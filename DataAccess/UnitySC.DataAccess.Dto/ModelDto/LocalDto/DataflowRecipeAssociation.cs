using System.Runtime.Serialization;

using UnitySC.DataAccess.Dto.ModelDto.Enum;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class DataflowRecipeAssociation
    {
        //TODO DATAFLOW : je doute que ça marche si la database n' a pas été changé (RT).
        public DataflowRecipeAssociation()
        {
        }

        public DataflowRecipeAssociation(AssociationRecipeType type, DataflowRecipeComponent component)
        {
            Type = type;
            Component = component;
        }

        [DataMember]
        public AssociationRecipeType Type { get; set; }

        [DataMember]
        public DataflowRecipeComponent Component { get; set; }
    }
}
