using System.Drawing;

using UnitySC.Shared.Format.Helper;

namespace UnitySC.Shared.Format.ASO
{
    public class AsoDefect : RectpxItem
    {
        private readonly ClusterReport _defect;
        public ClusterReport Defect { get => _defect; }

        public int DefectId { get => _defect.ClusterNumber; }
        public string DefectCategory { get => _defect.UserLabel; }
        public Color Color { get => _defect.Color; }

        public AsoDefect(ClusterReport defect, RectangleF rcpx)
          : base(rcpx)
        {
            _defect = defect;
        }
    }
}