using System.Threading;

using ADCEngine;

namespace BasicModules.Append
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class AppendModule : ModuleBase
    {
        //=================================================================
        // 
        //=================================================================
        public AppendModule(IModuleFactory facotory, int id, Recipe recipe)
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
