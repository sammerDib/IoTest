using System.Runtime.Serialization;

namespace UnitySC.DataAccess.Dto.ModelDto.LocalDto
{
    [DataContract]
    public class WaferResultPath
    {
        [DataMember]
        public string FolderPath { get; set; }

        [DataMember]
        public string FileName { get; set; }
    }
}