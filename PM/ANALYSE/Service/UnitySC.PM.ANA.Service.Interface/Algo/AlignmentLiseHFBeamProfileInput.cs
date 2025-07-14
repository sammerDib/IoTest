using System.Collections.Generic;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    public class AlignmentLiseHFBeamProfileInput : IANAInputFlow
    {
        public List<char> Image { get; set; }
        public double PixelSizeX { get; set; }
        public double PixelSizeY { get; set; }
        public double MaxIntensity { get; set; }

        public InputValidity CheckInputValidity()
        {
            throw new System.NotImplementedException();
        }

        public ANAContextBase InitialContext { get; set; }
    }
}
