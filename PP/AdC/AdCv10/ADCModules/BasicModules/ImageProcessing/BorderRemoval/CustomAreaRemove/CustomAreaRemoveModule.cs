//#define DRAW_MAP
// Crée une map dans une autre image. La map 
// contient les zones dedans/dehors/sur le bord.
#if DRAW_MAP
#warning DRAW_MAP
#endif

using System.Drawing;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class CustomAreaRemoveModule : ImageModuleBase
    {
        private const int _step = 40;   // minimum size to divide and recurse

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramOffsetX;
        public readonly DoubleParameter paramOffsetY;
        public readonly DoubleParameter paramWidth;
        public readonly DoubleParameter paramHeight;
        public readonly BoolParameter paramDebug;

        //=================================================================
        // Constructeur
        //=================================================================
        public CustomAreaRemoveModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramOffsetX = new DoubleParameter(this, "OffsetX");
            paramOffsetY = new DoubleParameter(this, "OffsetY");
            paramWidth = new DoubleParameter(this, "Width", min: 0);
            paramHeight = new DoubleParameter(this, "Height", min: 0);
            paramDebug = new BoolParameter(this, "Debug");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process" + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;

            CustomAreaRemoveAlgorithm algo = new CustomAreaRemoveAlgorithm();
            algo.MicronRect = new RectangleF((float)paramOffsetX.Value, (float)paramOffsetY.Value, (float)paramWidth.Value, (float)paramHeight.Value);
            algo.RemoveInterior = true;
            algo.Debug = paramDebug.Value;

#if DRAW_MAP
            algo.DrawDebugMap = true;
#endif

            algo.PerformRemoval(image);

            ProcessChildren(obj);
        }

        //=================================================================
        // 
        //=================================================================
#if DRAW_MAP
        public override void Stop(ModuleBase parent)
        {
            EdgeRemoveAlgorithm.map.Save(PathString.GetTempPath() / "EdgeRemove.tif");
            EdgeRemoveAlgorithm.map.DelRef();

            base.Stop(parent);
        }
#endif

    }
}
