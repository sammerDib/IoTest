using System.Drawing;

using AdcTools;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public class CustomAreaRemoveAlgorithm : RemoveAlgorithmBase
    {
        //=================================================================
        // Paramètres
        //=================================================================
        public RectangleF MicronRect;

        //=================================================================
        // 
        //=================================================================
        protected override eCompare IsQuadInside(QuadF micronQuad)
        {
            return micronQuad.IsInside(MicronRect);
        }

        protected override bool IsPointInside(PointF micronPoint)
        {
            return MicronRect.Contains(micronPoint);
        }

    }
}
