using UnitySC.PM.WOTAN.Common;

namespace UnitySC.PM.WOTAN.Processing
{
    public interface IBareWaferAligner
    {
        AlignResult Align(Wafer wafer);
    }
}
