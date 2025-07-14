using System.Collections.Generic;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace BasicModules.Sizing
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    internal class SizingWithBlobCalculationModule : MilCharacterization.MilCharacterizationModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly SizingParameter paramSizing;

        //=================================================================
        // Constructeur
        //=================================================================
        public SizingWithBlobCalculationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramSizing = new SizingParameter(this, "Sizing");
            _parameterList = new List<ParameterBase>() { paramSizing };
            _exportableParameterList = new List<ParameterBase>();
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug(obj.ToString());
            Cluster cluster = (Cluster)obj;
            Interlocked.Increment(ref nbObjectsIn);

            cluster.blobList.Clear();
            CreateBlobs(cluster);
            if (cluster.blobList.Count != 0)
            {
                ComputeClusterCharacteristics(cluster);

                SizingModule.Measure(cluster, paramSizing);
                double defectMaxSize = (double)cluster.characteristics[SizingCharacteristics.DefectMaxSize];
                double totalDefectSize = (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize];
                logDebug(cluster.ToString() + " DefectMaxSize: " + defectMaxSize + " TotalDefectSize: " + totalDefectSize);

                ProcessChildren(obj);
            }
        }


    }
}
