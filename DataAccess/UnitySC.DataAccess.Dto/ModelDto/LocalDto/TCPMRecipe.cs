using System.Runtime.Serialization;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class TCPMRecipe
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Author { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}