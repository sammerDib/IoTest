using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

namespace AdvancedModules.ClusterOperation.Sieve
{
    internal class SieveModule : ImageModuleBase
    {
        public readonly SieveParameter paramSieve;
        public SieveModule(IModuleFactory factory, int id, Recipe recipe) :
            base(factory, id, recipe)
        {
            paramSieve = new SieveParameter(this, "Sieve");
        }

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Cluster cluster = (Cluster)obj;

            // Test if a filter is applied to the cluster defect class 
            var filterClass = paramSieve.SieveClasses.Values.Any(scl => scl.DefectLabel == cluster.DefectClass && !scl.ApplyFilter);
            if (filterClass) { return; }

            // Pass current object to the next module 
            ProcessChildren(obj);
        }

    }
}
