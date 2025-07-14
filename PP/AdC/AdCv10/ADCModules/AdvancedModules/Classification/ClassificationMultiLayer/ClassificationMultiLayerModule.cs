using System;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

namespace AdvancedModules.ClassificationMultiLayer
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class ClassificationMultiLayerModule : ModuleBase
    {
        public static readonly int MeasuredBranchAutomatic = -1;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        [ExportableParameter(false)]
        public readonly ClassificationMultiLayerParameter paramClassification;

        //=================================================================
        // Constructeur
        //=================================================================
        public ClassificationMultiLayerModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramClassification = new ClassificationMultiLayerParameter(this, "Classification");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("ClassificationMultiLayer " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            CMC cmc = (CMC)obj;

            MultiLayerDefectClass defectClass = Classify(cmc);
            if (defectClass != null)
            {
                Cluster mostSignificantCluster = SelectLayer(cmc, defectClass);
                ProcessChildren(mostSignificantCluster);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected MultiLayerDefectClass Classify(CMC cmc)
        {
            foreach (MultiLayerDefectClass defectClass in paramClassification.DefectClassList)
            {
                bool bMatch = true;
                bool bOneBranchMatch = false;   // Au moins une branche correspond
                int nb_branches_tested = 0;

                for (int branch = 0; branch < defectClass.DefectBranchList.Count; branch++)
                {
                    Cluster cluster = cmc.GetFirstClusterFromBranch(branch);
                    switch (defectClass.DefectBranchList[branch])
                    {
                        case DefectTestType.DefectClassNotUsed:
                            break;
                        case DefectTestType.DoNotTest:
                            bOneBranchMatch = bOneBranchMatch || ((cluster != null) && (cluster.defectClassList.Contains(defectClass.DefectLabel)));
                            break;
                        case DefectTestType.DefectMustBePresent:
                            bMatch = bMatch && (cluster != null);
                            bMatch = bMatch && (cluster.defectClassList.Contains(defectClass.DefectLabel));
                            nb_branches_tested++;
                            break;
                        case DefectTestType.DefectMustNotBePresent:
                            bool defect_not_present = (cluster == null) || !cluster.defectClassList.Contains(defectClass.DefectLabel);
                            bMatch = bMatch && defect_not_present;
                            nb_branches_tested++;
                            break;
                        default:
                            throw new ApplicationException("unknown DefectTestType: " + defectClass.DefectBranchList[branch]);
                    }

                    if (!bMatch)
                        break;
                }

                if (nb_branches_tested == 0)
                    bMatch = bOneBranchMatch;

                if (bMatch)
                {
                    logDebug(cmc.ToString() + " classified as " + defectClass.DefectLabel);
                    foreach (Cluster cluster in cmc.GetClusters())
                        cluster.defectClassList.Insert(0, defectClass.DefectLabel);

                    return defectClass;
                }
            }

            return null;
        }

        //=================================================================
        // 
        //=================================================================
        protected Cluster SelectLayer(CMC cmc, MultiLayerDefectClass defectClass)
        {
            Cluster mostSignificantCluster;

            if (defectClass.MeasuredBranch == MeasuredBranchAutomatic)
                mostSignificantCluster = SelectLayerFromCharacteristic(cmc, defectClass);
            else
                mostSignificantCluster = cmc.GetFirstClusterFromBranch(defectClass.MeasuredBranch);

            return mostSignificantCluster;
        }

        //=================================================================
        // 
        //=================================================================
        protected Cluster SelectLayerFromCharacteristic(CMC cmc, MultiLayerDefectClass defectClass)
        {
            Cluster mostSignificantCluster = null;

            double max = double.NegativeInfinity;
            foreach (Cluster cluster in cmc.GetClusters())
            {
                if (cluster.defectClassList.Contains(defectClass.DefectLabel))
                {
                    double val = (double)cluster.characteristics[defectClass.CharacteristicForAutomaticLayer];
                    if (val > max)
                    {
                        max = val;
                        mostSignificantCluster = cluster;
                    }
                }
            }

            if (mostSignificantCluster == null)
                throw new ApplicationException("No MostSignificantCluster");
            return mostSignificantCluster;
        }

    }
}
