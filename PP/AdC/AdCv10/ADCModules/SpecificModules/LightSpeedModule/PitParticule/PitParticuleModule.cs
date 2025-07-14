using System.Collections.Generic;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

namespace SpecificModules.LightSpeedModule
{
    ///////////////////////////////////////////////////////////////////////
    // a Module
    ///////////////////////////////////////////////////////////////////////
    public class PitParticuleModule : ModuleBase, ICharacterizationModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        private int branch1 = 0;
        private int branch2 = 1;
        private Characteristic carac = ClusterCharacteristics.BlobMaxGreyLevel;

        //=================================================================
        // Characteristics
        //=================================================================
        private List<Characteristic> _availableCharacteristics;
        List<Characteristic> ICharacterizationModule.AvailableCharacteristics
        {
            get
            {
                if (_availableCharacteristics == null)
                {
                    _availableCharacteristics = new List<Characteristic>();
                    _availableCharacteristics.Add(PitParticuleFactory.RatioCharacteristic);
                }
                return _availableCharacteristics;
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public PitParticuleModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            CMC cmc = (CMC)obj;

            double ratio = double.NaN;
            Cluster cluster1 = cmc.GetFirstClusterFromBranch(branch1);
            Cluster cluster2 = cmc.GetFirstClusterFromBranch(branch2);
            if (cluster1 != null && cluster2 != null)
            {
                double value1 = (double)cluster1.characteristics[carac];
                bool saturated1 = (bool)(cluster1.characteristics[ClusterPSLCharacteristics.IsSaturatedCharacteristic]);
                if (saturated1)
                    value1 = double.PositiveInfinity;
                double value2 = (double)cluster2.characteristics[carac];
                bool saturated2 = (bool)(cluster2.characteristics[ClusterPSLCharacteristics.IsSaturatedCharacteristic]);
                if ((bool)(cluster2.characteristics[ClusterPSLCharacteristics.IsSaturatedCharacteristic]))
                    value2 = double.PositiveInfinity;
                if (saturated1 && saturated2)
                    ratio = 1;
                else
                    ratio = value1 / value2;
            }
            else if (cluster1 != null && cluster2 == null)
            {
                ratio = double.PositiveInfinity;
            }
            else if (cluster1 == null && cluster2 != null)
            {
                ratio = 0;
            }
            else // if (cluster1 == null && cluster2 == null)
            {
                ratio = double.NaN;
            }

            foreach (Cluster cluster in cmc.GetClusters())
                cluster.characteristics[PitParticuleFactory.RatioCharacteristic] = ratio;

            ProcessChildren(obj);
        }

    }
}
