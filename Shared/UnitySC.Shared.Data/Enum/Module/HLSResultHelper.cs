namespace UnitySC.Shared.Data.Enum.Module
{
    public static class HLSResultHelper
    {
        // [Specific Module result (8 bits)] ---- cf ResultType
        // ==> [HLS Direction (2 bits)][HLS result (6 bits)]
        public const int HLSResultSize = 6;
        public const int HLSResultShift = PMEnumHelper.ResultSpecificShift;
        public const int HLSDirectionSize = 2;
        public const int HLSDirectionShift = HLSResultShift + HLSResultSize;

        public static HLSDirection GetDirection(this ResultType resultType)
        {
            return (HLSDirection)PMEnumHelper.ApplyMask(resultType, HLSDirectionShift, HLSDirectionSize);
        }
        public static int GetHLSAcquisitionTypeId(this ResultType resultType)
        {
            return PMEnumHelper.ApplyMask(resultType, HLSResultShift, HLSResultSize) >> HLSResultShift;
        }

    }
}
