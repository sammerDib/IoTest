using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace AdvancedModules.MultiLayerClusterDispatcher
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class MultiLayerClusterDispatcherModule : ModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        [ExportableParameter(false)]
        public readonly MultiLayerClusterDispatcherParameter paramBranches;

        //=================================================================
        // 
        //=================================================================
        public MultiLayerClusterDispatcherModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
            paramBranches = new MultiLayerClusterDispatcherParameter(this, "Branches");
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
            DispatcherDefectClass defectClass = paramBranches.DefectClassList.Find(b => b.DefectLabel == cluster.DefectClass && b.DefectLayer == cluster.Layer.name);
            if (defectClass == null)
                throw new ApplicationException("can't find branch for cluster:" + cluster + " defect:" + cluster.DefectClass);

            int branchIndex = defectClass.BranchIndex;
            if (branchIndex >= Children.Count)
                throw new ApplicationException("invalid branch index:" + branchIndex + " defect:" + defectClass.DefectLabel + " cluster:" + cluster);
            if (branchIndex < 0)
                return; //on ignore le cluster

            //-------------------------------------------------------------
            // Route le cluster
            //-------------------------------------------------------------
            ModuleBase child = Children[branchIndex];
            Interlocked.Increment(ref nbObjectsOut);
            ProcessChild(child, obj);
        }

    }
}
