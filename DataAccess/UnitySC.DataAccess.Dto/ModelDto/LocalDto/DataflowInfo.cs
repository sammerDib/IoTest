using System.Runtime.Serialization;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class DataflowInfo
    {
        //TODO DATAFLOW : je doute que ça marche si la database n' a pas été changé (RT).        
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int StepId { get; set; }

        [DataMember]
        public string Comment { get; set; }
    }
}
