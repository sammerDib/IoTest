namespace UnitySC.Shared.Format.Base
{
    public class ResultDataStats
    {
        public long DBResultItemId { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public int UnitType { get; set; }

        public ResultDataStats()
        {
            DBResultItemId = -1;
            Type = -1;
            Name = string.Empty;
            Value = 0.0;
            UnitType = -1;
        }

        public ResultDataStats(long dBResid, int nType, string sName, double dValue, int nUnitType)
        {
            DBResultItemId = dBResid;
            Type = nType;
            Name = sName;
            Value = dValue;
            UnitType = nUnitType;
        }

        public override string ToString()
        {
            return $"[{Type}] {Name} = {Value} | {DBResultItemId} | {UnitType}";
        }
    }
}
