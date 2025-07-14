using System.Threading;

using ADCEngine;

namespace HazeLSModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class AppendHazeResultModule : ModuleBase
    {
        //=================================================================
        // 
        //=================================================================
        public AppendHazeResultModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Interlocked.Increment(ref nbObjectsIn);

            ProcessChildren(obj);
        }

    }
}
