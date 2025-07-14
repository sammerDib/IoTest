namespace UnitySC.DataAccess.ResultScanner.Interface
{
    public delegate void StateChangeEventHandler(ResultScannerStateNotificationMessage msg);

    public delegate void StatisticsChangeEventHandler(long resultID);

    public class ResultScannerStateNotificationMessage
    {
        public long ResultID { get; set; } // a voir si necéssaire

        public int State { get; set; }
        public int InternalState { get; set; }
    }
}
