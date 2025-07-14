using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.ClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class ClusterDispatcherModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        [ExportableParameter(false)]
        public readonly ClusterDispatcherParameter paramBranches;

        //=================================================================
        // 
        //=================================================================
        public ClusterDispatcherModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
            paramBranches = new ClusterDispatcherParameter(this, "Branches");
            ModuleProperty = eModuleProperty.Stage;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Interlocked.Increment(ref nbObjectsIn);
            logDebug("Process " + obj);

            Cluster cluster = (Cluster)obj;
            if (cluster.DefectClass == null || cluster.DefectClass == "")
                throw new ApplicationException("unclassified cluster:" + cluster);

            //-------------------------------------------------------------
            // Cherche la bonne branche
            //-------------------------------------------------------------
            DispatcherDefectClass defectClass = paramBranches.DefectClassList.Find(b => b.DefectClass == cluster.DefectClass);
            if (defectClass == null)
                throw new ApplicationException("can't find branch for cluster:" + cluster + " defect:" + cluster.DefectClass);

            int branchIndex = defectClass.BranchIndex;
            if (branchIndex < 0 || branchIndex >= Children.Count)
                throw new ApplicationException("invalid branch index:" + branchIndex + " defect:" + defectClass.DefectClass + " cluster:" + cluster);

            //-------------------------------------------------------------
            // Route le cluster
            //-------------------------------------------------------------
            ModuleBase child = Children[branchIndex];
            Interlocked.Increment(ref nbObjectsOut);
            ProcessChild(child, obj);
        }

    }
}
