using System.Drawing;

using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format._001
{
    public class KlarfDefect : RectpxItem
    {
        private readonly PrmDefect _defect;
        public PrmDefect Defect { get => _defect; }

        public int RoughBinKey { get; private set; }

        public KlarfDefect(PrmDefect defect, RectangleF rcpx)
            : base(rcpx)
        {
            _defect = defect;
            RoughBinKey = (int)_defect.Get("ROUGHBINNUMBER");
        }
    }
}