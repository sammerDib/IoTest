using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules;

using LibProcessing;

namespace DefectPatternLearning
{
    ///////////////////////////////////////////////////////////////////////
    // a Module
    ///////////////////////////////////////////////////////////////////////
    public class DefectPatternLearningModule : ClusterizerModuleBase, ICharacterizationModule
    {
        public enum eClusterizationAlgorithmEx
        {
            [Description("By Pattern")]
            Blob,
            [Description("By Surrounding Rectangle")]
            SurroundingRectangle,
            [Description("By Surrounding Rectangle, separating each class of defect")]
            SurroundingRectangleSeparatingClasses,
        }

        /// <summary> L'objet de la library de Defect Learning </summary>
        private DefectLearning.Business.DefectLearning DefectLearning;
        private List<string> DefectLearningClassList;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly FileParameter paramClassifierP1File;
        public readonly FileParameter paramClassifierP2File;
        public readonly DoubleParameter paramGridStep;
        public readonly DoubleParameter paramThreshold;
        public readonly DoubleParameter paramLocality;
        public readonly EnumParameter<eClusterizationAlgorithmEx> paramClusterizationAlgorithm;

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
                    _availableCharacteristics.Add(DefectPatternLearningFactory.ClassCharacteristic);
                    _availableCharacteristics.Add(DefectPatternLearningFactory.ConfidenceP1Characteristic);
                    _availableCharacteristics.Add(DefectPatternLearningFactory.ConfidenceP2Characteristic);
                }
                return _availableCharacteristics;
            }
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public DefectPatternLearningModule(IModuleFactory facotory, int id, Recipe recipe)
            : base(facotory, id, recipe)
        {
            paramClassifierP1File = new FileParameter(this, "ClassifierP1", "Classifiers (*.psc)|*.psc");
            paramClassifierP2File = new FileParameter(this, "ClassifierP2", "Classifiers (*.pcc)|*.pcc");
            paramGridStep = new DoubleParameter(this, "GridStep");
            paramThreshold = new DoubleParameter(this, "Threshold");
            paramLocality = new DoubleParameter(this, "Locality");
            paramGridStep.Value = paramThreshold.Value = paramLocality.Value = 0.5;

            paramClusterizationAlgorithm = new EnumParameter<eClusterizationAlgorithmEx>(this, "ClusterizationAlgorithm");
            paramClusterizationAlgorithm.ValueChanged +=
                (algo) =>
                {
                    paramClusterizationStep.IsEnabled = (algo != eClusterizationAlgorithmEx.Blob);
                };
            paramClusterizationStep.IsEnabled = (paramClusterizationAlgorithm != eClusterizationAlgorithmEx.Blob);

        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            //-------------------------------------------------------------
            // Init de base
            //-------------------------------------------------------------
            base.OnInit();

            //-------------------------------------------------------------
            // Chargement des classifieurs
            //-------------------------------------------------------------
            DefectLearning = new DefectLearning.Business.DefectLearning();
            DefectLearning.LoadClassifierP1(paramClassifierP1File.FullFilePath);
            DefectLearning.LoadClassifierP2(paramClassifierP2File.FullFilePath);

            DefectLearning.Data.SearchParameter searchParameter = new DefectLearning.Data.SearchParameter();
            searchParameter.GridStep = paramGridStep;
            searchParameter.Threshold = paramThreshold;
            searchParameter.Locality = paramLocality;
            searchParameter.MaxRes = paramDefectCountLimit.IsUsed ? paramDefectCountLimit.Value : 1000000;
            DefectLearning.Init(searchParameter);

            DefectLearningClassList = DefectLearning.GetClassList();
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("DefectPatternLearning " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            ImageBase image = (ImageBase)obj;

            // Détection / Classification
            //...........................
            List<Cluster> clusters = FindDefectPatterns(image);
            log("image: " + image + "   nb_patterns_found: " + clusters.Count());

            // Regroupement des clusters
            //..........................
            switch (paramClusterizationAlgorithm.Value)
            {
                case eClusterizationAlgorithmEx.Blob:
                    break;
                case eClusterizationAlgorithmEx.SurroundingRectangle:
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    clusters = GroupBlobsBySurroundingRectangles(image, clusters);
                    stopWatch.Stop();
                    logDebug("GroupBlobsBySurroundingRectangles " + clusters.Count() + " blobs  " + stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                    break;
                default:
                    throw new ApplicationException("unknown algorithm: " + paramClusterizationAlgorithm.Value);
            }

            ComputeClustersCaracteristics(clusters);

            // Envoi des clusters dans la queue
            //.................................
            StoreRenderingObject(image);
            QueueClusters(image, clusters);
        }

        //=================================================================
        // 
        //=================================================================
        private List<Cluster> FindDefectPatterns(ImageBase image)
        {
            List<Cluster> clusters = new List<Cluster>();

            //-------------------------------------------------------------
            // Détection / Classification
            //-------------------------------------------------------------
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            CSharpImage csharpImage = image.CurrentProcessingImage.GetCSharpImage();
            DefectLearning.Data.ImageDataInput dataInput = new DefectLearning.Data.ImageDataInput();
            dataInput.width = csharpImage.width;
            dataInput.height = csharpImage.height;
            dataInput.pitch = csharpImage.pitch;
            dataInput.depth = csharpImage.depth;
            dataInput.ptr = csharpImage.ptr;
            dataInput.PixelFormat = PixelFormat.Format8bppIndexed;

            List<DefectLearning.Data.ExchangeData> defectsFound = null;
            DefectLearning.ProcessClassification(dataInput, 0, ref defectsFound);   // Détection + Classification
            stopWatch.Stop();
            logDebug("DefectPatternLearning " + defectsFound.Count() + " defect patterns  " + stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));

            int featureWindowWidth, featureWindowHeight;
            DefectLearning.GetFeatureWindowSize(out featureWindowWidth, out featureWindowHeight);
            //NB: il manque les info des position du centre, on suppose qu'il est au milieu de la feature window

            //-------------------------------------------------------------
            // Création des clusters
            //-------------------------------------------------------------
            LayerBase layer = image.Layer;
            for (int i = 0; i < defectsFound.Count; i++)
            {
                CheckMemoryLimit();

                if (State == eModuleState.Aborting)
                    break;

                DefectLearning.Data.ExchangeData ed = defectsFound[i];

                int num = CreateClusterNumber(image, i);
                using (Cluster cluster = new Cluster(num, layer))
                {
                    // Calcul des coordonnées
                    //.......................
                    int left = ed.Position_X_px - featureWindowWidth / 2;
                    int right = left + featureWindowWidth;
                    int top = ed.Position_Y_px - featureWindowHeight / 2;
                    int bottom = top + featureWindowHeight;

                    cluster.pixelRect = Rectangle.FromLTRB(left, top, right, bottom);
                    // Les coordonnées microns seront calculées plus tard

                    // Info spécifiques
                    //.................
                    if (image is DieImage)
                    {
                        DieImage dieimg = (DieImage)image;
                        cluster.DieIndexX = dieimg.DieIndexX;
                        cluster.DieIndexY = dieimg.DieIndexY;
                        cluster.DieOffsetImage = dieimg.imageRect.TopLeft();
                    }

                    if (image is MosaicImage)
                    {
                        MosaicImage mosaicimg = (MosaicImage)image;
                        cluster.Line = mosaicimg.Line;
                        cluster.Column = mosaicimg.Column;
                    }

                    // Blob et Caractéristiques
                    //.........................
                    Blob blob = new Blob(num, cluster);
                    blob.pixelRect = cluster.pixelRect;

                    blob.characteristics[DefectPatternLearningFactory.ClassCharacteristic] = ed.ClassName;
                    blob.characteristics[DefectPatternLearningFactory.ConfidenceP1Characteristic] = ed.ConfidenceNoteP1;
                    blob.characteristics[DefectPatternLearningFactory.ConfidenceP2Characteristic] = ed.ConfidenceNoteP2;

                    cluster.blobList.Add(blob);

                    // Ajout du cluster dans la liste
                    //...............................
                    cluster.AddRef();
                    clusters.Add(cluster);
                }
            }

            return clusters;
        }

        //=================================================================
        // 
        //=================================================================
        protected override int GetNbClasses()
        {
            if (paramClusterizationAlgorithm.Value == eClusterizationAlgorithmEx.SurroundingRectangleSeparatingClasses)
                return DefectLearningClassList.Count;
            else
                return 1;
        }

        protected override int GetClusterClass(Cluster cluster)
        {
            if (paramClusterizationAlgorithm.Value == eClusterizationAlgorithmEx.SurroundingRectangleSeparatingClasses)
            {
                string _class = (string)cluster.blobList[0].characteristics[DefectPatternLearningFactory.ClassCharacteristic];
                return DefectLearningClassList.IndexOf(_class);
            }
            else
            {
                return 0;
            }
        }

        //=================================================================
        //
        //=================================================================
        private void ComputeClustersCaracteristics(IEnumerable<Cluster> clusters)
        {
            //-------------------------------------------------------------
            // Calcul des confiances
            //-------------------------------------------------------------
            foreach (Cluster cluster in clusters)
            {
                double confidenceP1 = 0, confidenceP2 = 0;
                int[] NbBlobPerClass = new int[DefectLearningClassList.Count];

                foreach (Blob blob in cluster.blobList)
                {
                    confidenceP1 += (double)blob.characteristics[DefectPatternLearningFactory.ConfidenceP1Characteristic];
                    confidenceP2 += (double)blob.characteristics[DefectPatternLearningFactory.ConfidenceP2Characteristic];
                }

                confidenceP1 /= cluster.blobList.Count;
                confidenceP2 /= cluster.blobList.Count;
                cluster.characteristics[DefectPatternLearningFactory.ConfidenceP1Characteristic] = confidenceP1;
                cluster.characteristics[DefectPatternLearningFactory.ConfidenceP2Characteristic] = confidenceP2;
            }

            //-------------------------------------------------------------
            // Choix de la classe
            //-------------------------------------------------------------
            foreach (Cluster cluster in clusters)
            {
                string _class;
                if (paramClusterizationAlgorithm.Value != eClusterizationAlgorithmEx.SurroundingRectangleSeparatingClasses)
                {
                    _class = (string)cluster.blobList[0].characteristics[DefectPatternLearningFactory.ClassCharacteristic];
                    cluster.defectClassList.Add(_class);
                    cluster.characteristics[DefectPatternLearningFactory.ClassCharacteristic] = _class;
                }
                else
                {
                    int[] NbBlobPerClass = new int[DefectLearningClassList.Count];
                    int indexOfMax = 0;

                    foreach (Blob blob in cluster.blobList)
                    {
                        _class = (string)blob.characteristics[DefectPatternLearningFactory.ClassCharacteristic];
                        int idx = DefectLearningClassList.IndexOf(_class);
                        NbBlobPerClass[idx]++;

                        if (NbBlobPerClass[idx] > NbBlobPerClass[indexOfMax])
                            indexOfMax = idx;
                    }

                    _class = DefectLearningClassList[indexOfMax];
                    cluster.defectClassList.Add(_class);
                    cluster.characteristics[DefectPatternLearningFactory.ClassCharacteristic] = _class;
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            // Libération des resources
            //.........................
            DefectLearning.Dispose();

            // Stop
            //.....
            base.OnStopping(oldState);
        }


    }
}
