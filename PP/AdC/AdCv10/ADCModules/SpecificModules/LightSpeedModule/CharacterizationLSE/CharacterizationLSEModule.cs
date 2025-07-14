using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using BasicModules;

namespace SpecificModules.LightSpeedModule
{
    ///////////////////////////////////////////////////////////////////////
    // a Module
    ///////////////////////////////////////////////////////////////////////
    public class CharacterizationLSEModule : ModuleBase, ICharacterizationModule
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly ConditionalDoubleParameter paramFactor;
        public readonly ConditionalDoubleParameter paramOffset;

        //=================================================================
        // Characteristics
        //=================================================================
        /// <summary> Caractéristiques nécressaires pour ce module </summary>
        private List<Characteristic> neededBlobCharacteristics = new List<Characteristic>();

        private List<Characteristic> _availableCharacteristics;
        public List<Characteristic> AvailableCharacteristics
        {
            get
            {
                if (_availableCharacteristics == null)
                {
                    _availableCharacteristics = new List<Characteristic>();
                    _availableCharacteristics.Add(ClusterPSLCharacteristics.IsSaturatedCharacteristic);
                    _availableCharacteristics.Add(ClusterCharacteristics.PSLMaxValue);
                }
                return _availableCharacteristics;
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public CharacterizationLSEModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
            ModuleProperty = eModuleProperty.Stage;

            neededBlobCharacteristics.Add(ClusterCharacteristics.BlobMaxGreyLevel);
            neededBlobCharacteristics.Add(ClusterCharacteristics.Area);

            paramFactor = new ConditionalDoubleParameter(this, "Factor");
            paramFactor.Value = 1;
            paramOffset = new ConditionalDoubleParameter(this, "Offset");
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            Cluster cluster = (Cluster)obj;

            // Le cluster contient-il déjà les caractéristiques dont on a besoins pour nos calculs ?
            //......................................................................................
            bool bIsCharacterized = true;
            foreach (var carac in neededBlobCharacteristics)
                bIsCharacterized = bIsCharacterized && cluster.characteristics.ContainsKey(carac);

            // Non => on recalcule les clusters
            if (!bIsCharacterized)
            {
                cluster.blobList.Clear();
                BasicModules.MilCharacterization.MilCharacterizationModule.CreateBlobs(cluster, this, neededBlobCharacteristics);
            }

            // Calculs des caractéristiques LightSpeed
            //........................................
            if (cluster.blobList.Count != 0)
            {
                ComputeCharacteristics(cluster);
                ProcessChildren(obj);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void ComputeCharacteristics(Cluster cluster)
        {
            // Seuil de saturation
            double threshold = double.Parse(cluster.Layer.MetaData[AcquisitionAdcExchange.LayerMetaData.lseMax_nm]);
            double psl_saturated_µ = threshold / 1000;


            if (paramFactor.IsUsed)
                threshold *= paramFactor.Value;
            if (paramOffset.IsUsed)
                threshold += paramOffset.Value;

            threshold = (int)threshold; // on arrondit car l'image ne contient pas de flottant


            double maxPSL = 0;
            bool clusterIsSaturated = false;

            foreach (Blob blob in cluster.blobList)
            {
                double psl = (double)blob.characteristics[ClusterCharacteristics.BlobMaxGreyLevel];
                maxPSL = Math.Max(maxPSL, psl);
                bool isStaturated = psl >= threshold;
                blob.characteristics[ClusterPSLCharacteristics.IsSaturatedCharacteristic] = isStaturated;
                //totalArea += (double)blob.characteristics[ClusterCharacteristics.Area];

                double size;
                if (isStaturated)
                {
                    clusterIsSaturated = true;
                }
                else
                {
                    size = psl;
                }
            }

            cluster.characteristics[ClusterPSLCharacteristics.IsSaturatedCharacteristic] = clusterIsSaturated;
            cluster.characteristics[ClusterCharacteristics.PSLValue] = maxPSL / 1000;
            cluster.characteristics[ClusterCharacteristics.PSLMaxValue] = maxPSL / 1000;
            if (clusterIsSaturated)
            {

                foreach (Blob bl in cluster.blobList)
                {
                    // chaque blobl prend sa valeur saturé * Nb pixel
                    bl.characteristics[BlobCharacteristics.pslµsize] = psl_saturated_µ * bl.pixelArea;

                }
            }
            else
            {

                foreach (Blob bl in cluster.blobList)
                {
                    // chaque blob prend la vlauer PSL
                    bl.characteristics[BlobCharacteristics.pslµsize] = (double)bl.characteristics[ClusterCharacteristics.BlobMaxGreyLevel] / 1000;// µm
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            List<ModuleBase> modules = FindAncestors(m => m is ICharacterizationModule);
            if (modules.Count() > 0)
            {
                bool found = false;
                foreach (ICharacterizationModule mod in modules)
                {
                    found = mod.AvailableCharacteristics.Contains(ClusterCharacteristics.BlobMaxGreyLevel);
                }

                if (!found)
                    return "Parent characterization modules must calculate \"BlobMaxGreyLevel\".";
            }

            return base.Validate();
        }
    }
}
