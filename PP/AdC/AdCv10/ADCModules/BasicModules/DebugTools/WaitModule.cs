using System.Threading;

using ADCEngine;

namespace BasicModules.Debug
{
    public class WaitModule : ModuleBase
    {
        public readonly IntParameter ParamWaitMilliSeconde;

        public WaitModule(IModuleFactory factory, int id, Recipe recipe) : base(factory, id, recipe)
        {
            ParamWaitMilliSeconde = new IntParameter(this, "WaitMilliSeconde");
            ParamWaitMilliSeconde.Value = 1000;
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            Thread.Sleep(ParamWaitMilliSeconde);
            ProcessChildren(obj);
        }
    }
}
