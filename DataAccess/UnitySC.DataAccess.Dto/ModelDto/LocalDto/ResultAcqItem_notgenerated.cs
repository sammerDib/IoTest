using System.Runtime.Serialization;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.DataAccess.Dto
{
    public partial class ResultAcqItem
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
        //public string ResPath { get { return InPreRegisterAcqHelper.FullAcqFilePath(ResultAcq.PathName, FileName); }  }

        [DataMember]
        public string ResThumbnailPath { get; set; }
        //public string ResThumbnailPath { get { return InPreRegisterAcqHelper.FullAcqFilePathThumbnail(ResultAcq.PathName, FileName); } }
    }
}
