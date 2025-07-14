using System.Collections.Generic;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.PSI
{
    public class PSIStep
    {
        public int Number { get; set; }
        public Length StartPosition { get; set; }
        public Length EndPosition { get; set; }
        public List<USPImage> Images { get; set; } // images are captured at the started position
    }
}
