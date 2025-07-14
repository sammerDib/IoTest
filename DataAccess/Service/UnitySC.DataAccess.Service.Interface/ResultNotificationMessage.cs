using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;

namespace UnitySC.DataAccess.Service.Interface
{
    public delegate void ResultAddedEventHandler(ResultNotificationMessage msg);

    public delegate void ResultAcqAddedEventHandler(ResultAcqNotificationMessage msg);

    public delegate void StateUpdateEventHandler(ResultStateNotificationMessage msg);

    public class ResultNotificationMessage
    {
        public int JobID { get; set; }
        public UnitySC.DataAccess.Dto.ModelDto.LocalDto.WaferResultData WaferResultData { get; set; }

        public int ActorType { get => WaferResultData.ResultItem.Result.ActorType; }
        public int SlotID { get => WaferResultData.ResultItem.Result.WaferResult.SlotId; }
        public ResultItem ResultItem { get => WaferResultData.ResultItem; }
    }

    public class ResultStatsNotificationMessage
    {
        public int JobID { get; set; }
        public UnitySC.DataAccess.Dto.ModelDto.LocalDto.WaferResultData WaferResultData { get; set; }

        public int ActorType { get => WaferResultData.ResultItem.Result.ActorType; }
        public int SlotID { get => WaferResultData.ResultItem.Result.WaferResult.SlotId; }
        public ResultItem ResultItem { get => WaferResultData.ResultItem; }
    }

    public class ResultStateNotificationMessage
    {
        public long ResultID { get; set; }

        public int State { get; set; }
        public int InternalState { get; set; }
        public bool IsAcquisitionResult { get; set; }
    }

    public class ResultAcqNotificationMessage
    {
        public int JobID { get; set; }
        public ResultAcqItem ResultAcqItem { get; set; }

        public int ActorType { get => ResultAcqItem.ResultAcq.ActorType; }
        public int SlotID { get => ResultAcqItem.ResultAcq.WaferResult.SlotId; }
    }

}
