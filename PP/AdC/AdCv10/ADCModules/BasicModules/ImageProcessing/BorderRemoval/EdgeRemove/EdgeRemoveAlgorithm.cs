//#define DRAW_MAP
// Crée une map dans une autre image. La map 
// contient les zones dedans/dehors/sur le bord.
#if DRAW_MAP
#warning DRAW_MAP
#endif

//#define DRAW_DEBUG
using System.Drawing;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // 
    ///////////////////////////////////////////////////////////////////////
    public class EdgeRemoveAlgorithm : RemoveAlgorithmBase
    {
        //=================================================================
        // Paramètres
        //=================================================================
        public double Margin;

        private WaferBase _wafer;

        //=================================================================
        // 
        //=================================================================
        public override void PerformRemoval(ImageBase image)
        {
            _wafer = image.Layer.Wafer;
            base.PerformRemoval(image);
        }

        //=================================================================
        // 
        //=================================================================
        protected override eCompare IsQuadInside(QuadF micronQuad)
        {
            return _wafer.IsQuadInside(micronQuad, Margin);
        }

        protected override bool IsPointInside(PointF micronPoint)
        {
            return _wafer.IsPointInside(micronPoint, Margin);
        }

    }
}
