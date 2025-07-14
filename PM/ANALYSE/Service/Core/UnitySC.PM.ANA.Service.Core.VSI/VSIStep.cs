using UnitySC.Shared.Image;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.VSI
{
    public class VSIStep
    {
        public int Number { get; set; }
        public Length StartPosition { get; set; }
        public Length EndPosition { get; set; }
        public ServiceImage Image { get; set; }
    }
}
