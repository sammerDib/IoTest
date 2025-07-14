//#define DRAW_MAP
// Crée une map dans une autre image. La map 
// contient les zones dedans/dehors/sur le bord.
#if DRAW_MAP
#warning DRAW_MAP
#endif

//#define DRAW_DEBUG
// Dessine les zones "removées" dans l'image
#if DRAW_DEBUG
#warning DRAW_DEBUG
#endif

using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.BorderRemoval
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class EdgeRemoveModule : ImageModuleBase
    {
        private const int _step = 40;   // minimum size to divide and recurse

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly DoubleParameter paramMargin;
        public readonly BoolParameter paramRemoveInterior;
        public readonly BoolParameter paramDebug;


        //=================================================================
        // Constructeur
        //=================================================================
        public EdgeRemoveModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMargin = new DoubleParameter(this, "Margin");
            paramRemoveInterior = new BoolParameter(this, "RemoveInterior");
            paramDebug = new BoolParameter(this, "Debug");

            paramMargin.Value = 3000;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("EdgeRemove " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            ImageBase image = (ImageBase)obj;

            EdgeRemoveAlgorithm algo = new EdgeRemoveAlgorithm();
            algo.Margin = paramMargin;
            algo.RemoveInterior = paramRemoveInterior;
            algo.Debug = paramDebug;

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
