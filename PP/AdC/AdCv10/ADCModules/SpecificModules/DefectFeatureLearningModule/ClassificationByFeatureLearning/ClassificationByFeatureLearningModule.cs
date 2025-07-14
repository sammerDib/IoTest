using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules;

using UnitySC.Shared.Tools;

namespace DefectFeatureLearning.ClassificationByFeatureLearning
{
    internal class ClassificationByFeatureLearningModule : ModuleBase, IClassifierModule
    {
        public readonly FileParameter paramClassifierFile;
        public readonly ConditionalDoubleParameter ConfidenceMinValueFilterParameter;
        public readonly LayerSelectionParameter paramLayerSelection;
        public Classification Classification;

        List<string> IClassifierModule.DefectClassLabelList
        {
            get
            {
                if (Classification == null)
                    return new List<string>();
                else
                    return Classification.Defects.Select(c => c.ClassName).ToList();
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public ClassificationByFeatureLearningModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramClassifierFile = new FileParameter(this, "Classifier", "Classifiers (*.adclrn)|*.adclrn");
            paramClassifierFile.ValueChanged += ParamClassifierFile_ValueChanged;
            paramLayerSelection = new LayerSelectionParameter(this, "LayerSelection");
            ConfidenceMinValueFilterParameter = new ConditionalDoubleParameter(this, "ConfidenceMinValueFilter", 0.01, 1);
            ConfidenceMinValueFilterParameter.IsUsed = false;
            ConfidenceMinValueFilterParameter.Value = 0.6;
        }

        private void ParamClassifierFile_ValueChanged(ExternalRecipeFile t)
        {
            string path = paramClassifierFile.FullFilePath;
            if (path != null)
            {
                try
                {
                    Classification = XML.Deserialize<Classification>(path);
                }
                catch (Exception ex)
                {
                    Classification = null;
                    ExceptionMessageBox.Show("Failed to load Classification file " + path, ex);
                }
            }
            else
            {
                Classification = null;
            }
            paramLayerSelection.Synchronize();
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            //-------------------------------------------------------------
            // 
            //-------------------------------------------------------------
            Classification = XML.Deserialize<Classification>(paramClassifierFile.FullFilePath);

            foreach (Defect d in Classification.Defects)
            {
                foreach (Box b in d.Boxes)
                {
                    foreach (Feature f in b.Features)
                        f.Characteristic = Characteristic.Parse(f.Name);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            if (obj is Cluster)
            {
                Cluster cluster = (Cluster)obj;
                ClassifyCluster(cluster);
                ProcessChildren(obj);
            }
            else if (obj is CMC)
            {
                CMC cmc = (CMC)obj;
                ClassifyCMC(cmc);
                SelectAndProcessCluster(cmc);
            }
            else
            {
                throw new ApplicationException("unknown object type: " + obj);
            }
        }

        private void SelectAndProcessCluster(CMC cmc)
        {
            IEnumerable<Cluster> clusters = cmc.GetClusters();
            string defectClass = clusters.First().DefectClass;

            LayerSelector layerSelect = paramLayerSelection.DefectClasses.SingleOrDefault(x => x.DefectLabel == defectClass);


            if (layerSelect != null)
            {
                double max = double.NegativeInfinity;
                Cluster mostSignificantCluster = null;
                foreach (var cluster in clusters)
                {
                    double val = (double)cluster.characteristics[layerSelect.Characteristic];
                    if (val > max)
                    {
                        max = val;
                        mostSignificantCluster = cluster;
                    }
                }

                if (mostSignificantCluster == null)
                    throw new ApplicationException("No MostSignificantCluster");

                ProcessChildren(mostSignificantCluster);
            }
        }

        //=================================================================
        // 
        //=================================================================
        private void ClassifyCluster(Cluster cluster)
        {
            foreach (Defect c in Classification.Defects)
            {
                bool match = IsClusterOfClass(cluster, c);
                if (match)
                {
                    cluster.defectClassList.Add(c.ClassName);
                    break;
                }
            }
        }

        //=================================================================
        /// <summary> Teste si un cluster fait partie d'une classe </summary>
        //=================================================================
        private bool IsClusterOfClass(Cluster cluster, Defect cdefectClass)
        {
            foreach (Box b in cdefectClass.Boxes)
            {
                bool match = true;
                foreach (Feature f in b.Features)
                {
                    double v = (double)cluster.characteristics[f.Characteristic];
                    if (v < f.LowerBound || f.UpperBound < v)
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return true;
            }

            return false;
        }

        //=================================================================
        // 
        //=================================================================
        private void ClassifyCMC(CMC cmc)
        {
            foreach (Defect c in Classification.Defects)
            {
                bool match = IsCMCOfClass(cmc, c);
                if (match)
                {
                    foreach (Cluster cluster in cmc.GetClusters())
                        cluster.defectClassList.Add(c.ClassName);
                }
            }
        }

        //=================================================================
        /// <summary> Teste si un cluster fait partie d'une classe </summary>
        //=================================================================
        private bool IsCMCOfClass(CMC cmc, Defect cdefectClass)
        {
            foreach (Box b in cdefectClass.Boxes)
            {
                bool match = IsCMCInBox(cmc, b);
                if (match)
                    return true;
            }

            return false;
        }

        private bool IsCMCInBox(CMC cmc, Box box)
        {
            if (ConfidenceMinValueFilterParameter.IsUsed && box.Probability < ConfidenceMinValueFilterParameter)
                return false;

            int nbLayers = cmc.GetClusters().Count();
            foreach (Feature f in box.Features)
            {
                int nbclusters = 0;
                foreach (Cluster cluster in cmc.GetClustersFromLayer(f.Layer))
                {
                    nbclusters++;
                    double v;

                    object charactValue = cluster.characteristics[f.Characteristic];

                    if (f.Characteristic.Type == typeof(bool))
                        v = Convert.ToDouble((bool)charactValue);
                    else
                        v = (double)charactValue;

                    if (v < f.LowerBound || f.UpperBound < v)
                        return false;
                }

                // Les valeurs des characteristiques sont à 0 pour un layer non présente
                if (nbclusters == 0)
                {
                    double v = 0;
                    if (v < f.LowerBound || f.UpperBound < v)
                        return false;
                }
            }

            return true;
        }
    }
}
