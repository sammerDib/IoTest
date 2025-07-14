using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto
{
    public partial class ResultItem
    {
        [DataMember]
        public ResultType ResultTypeEnum
        {
            get
            {
                return (ResultType)ResultType;
            }
            set
            {
                ResultType = (int)value;
            }
        }

        [DataMember]
        public string ResPath { get; set; }

        [DataMember]
        public string ResThumbnailPath { get; set; }

        public ResultFormat ResultFormat => ResultTypeEnum.GetResultFormat();
    }
}
