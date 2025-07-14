using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using LibProcessing;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.MilCharacterization
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    // On a trois types de caractéristiques:
    // - les carac des clusters
    // - les carac des blobs
    // - les features MIL
    // L'utilisateur choisit les carac cluster, on les convertit en carac blob
    // puis en feature MIL.
    ///////////////////////////////////////////////////////////////////////
    public class MilCharacterizationModule : ModuleBase, ICharacterizationModule
    {
        public List<Characteristic> SupportedCharacteristics { get { return clusterToBlobCharacteristicDico.Keys.ToList(); } }
        public List<Characteristic> AvailableCharacteristics
        {
            get
            {
                paramCharacterization.ClusterCharacteristicList.SortByList(SupportedCharacteristics);
                return paramCharacterization.ClusterCharacteristicList;
            }
        }

        private List<Characteristic> blobCharacteristics = new List<Characteristic>();

        ///<summary> Association Carac Cluster => Carac Blob </summary>
        private static Dictionary<Characteristic, int> milFeaturesDico = new Dictionary<Characteristic, int>();
        ///<summary> Association Carac Blob => Feature MIL </summary>
        private static MultiDictionary<Characteristic, Characteristic> clusterToBlobCharacteristicDico = new MultiDictionary<Characteristic, Characteristic>();

        private static ProcessingClassMil _processClass = new ProcessingClassMil();

        //=================================================================
        // Paramètres du XML
        //=================================================================
        [ExportableParameter(false)]
        public readonly MilCharacterizationParameter paramCharacterization;


        //=================================================================
        // Constructeur
        //=================================================================
        static MilCharacterizationModule()
        {
            InitMilFeaturesDico();
            InitClusterToBlobCharacteristicDico();
        }

        public MilCharacterizationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramCharacterization = new MilCharacterizationParameter(this, "Characterization");
            ModuleProperty = eModuleProperty.Stage;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            //-------------------------------------------------------------
            // Caracs toujours calculées
            //-------------------------------------------------------------
            List<Characteristic> clusterCharacteristicList = new List<Characteristic>(paramCharacterization.ClusterCharacteristicList);
            clusterCharacteristicList.TryAdd(ClusterCharacteristics.AbsolutePosition);
            clusterCharacteristicList.TryAdd(ClusterCharacteristics.Area);
            clusterCharacteristicList.TryAdd(ClusterCharacteristics.RealDiameter);
            clusterCharacteristicList.TryAdd(ClusterCharacteristics.RealWidth);
            clusterCharacteristicList.TryAdd(ClusterCharacteristics.RealHeight);

            //-------------------------------------------------------------
            // Conversion carac cluster -> carac blob
            //-------------------------------------------------------------
            foreach (Characteristic carac in clusterCharacteristicList)
            {
                List<Characteristic> list = clusterToBlobCharacteristicDico[carac];
                blobCharacteristics.UnionWith(list);
            }
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Cluster cluster = (Cluster)obj;
            logDebug("Caracterization " + cluster);
            Interlocked.Increment(ref nbObjectsIn);

            CreateBlobs(cluster);
            if (cluster.blobList.Count != 0)
            {
                ComputeClusterCharacteristics(cluster);
                ProcessChildren(obj);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected List<Blob> CreateBlobs(Cluster cluster)
        {
            return CreateBlobs(cluster, this, blobCharacteristics);
        }

        public static List<Blob> CreateBlobs(Cluster cluster, ModuleBase module, List<Characteristic> blobCharacteristics)
        {
            List<Blob> blobList = new List<Blob>();

            using (MilBlobFeatureList blobFeatureList = new MilBlobFeatureList())
            using (MilBlobResult blobResult = new MilBlobResult())
            {
                MilImage milImage = cluster.ResultProcessingImage.GetMilImage();
                MilImage milOriginalImage = cluster.OriginalProcessingImage.GetMilImage();

                //--------------------------------------------------------------
                // Pas de Binarisation de l'image, c'est déjà une image binaire
                //--------------------------------------------------------------
                // _processClass.Binarisation(cluster.ResultProcessingImage, 1);

                //--------------------------------------------------------------
                // Calcul des blobs
                //--------------------------------------------------------------
                // Features a calculer: conversion carac blob => feature MIL
                blobFeatureList.Alloc();
                blobFeatureList.SelectFeature(MIL.M_BOX);
                foreach (Characteristic carac in blobCharacteristics)
                    blobFeatureList.SelectFeature(milFeaturesDico[carac]);

                // Calcul des features pour tous les blobs
                blobResult.Alloc();
                blobResult.Calculate(milImage, milOriginalImage, blobFeatureList);

                //--------------------------------------------------------------
                // Récuperation des caractéristiques X/Y
                //--------------------------------------------------------------
                MIL_INT nbBlobs = blobResult.Number;
                module.logDebug("cluster: " + cluster + "   nb_blobs_found: " + nbBlobs);
                if (nbBlobs == 0)
                    return blobList;

                // Get blob areas
                double[] lfDataBlob_Left = blobResult.GetResult(MIL.M_BOX_X_MIN);
                double[] lfDataBlob_Right = blobResult.GetResult(MIL.M_BOX_X_MAX);
                double[] lfDataBlob_Top = blobResult.GetResult(MIL.M_BOX_Y_MIN);
                double[] lfDataBlob_Bottom = blobResult.GetResult(MIL.M_BOX_Y_MAX);
                double[] lfDataBlob_Area = blobResult.GetResult(MIL.M_AREA);

                //--------------------------------------------------------------
                // Création des blobs
                //--------------------------------------------------------------
                for (int i = 0; i < nbBlobs; i++)
                {
                    // Allocation
                    //...........
                    LayerBase layer = cluster.Layer;
                    Blob blob = new Blob(i, cluster);
                    blobList.Add(blob);

                    // Calcul du rectangle
                    //.....................
                    blob.pixelRect.X = cluster.imageRect.X + (int)lfDataBlob_Left[i];
                    blob.pixelRect.Y = cluster.imageRect.Y + (int)lfDataBlob_Top[i];
                    blob.pixelRect.Width = (int)(lfDataBlob_Right[i] - lfDataBlob_Left[i]) + 1;
                    blob.pixelRect.Height = (int)(lfDataBlob_Bottom[i] - lfDataBlob_Top[i]) + 1;

                    blob.pixelArea = (int)lfDataBlob_Area[i];

                    if (cluster.Layer is DieLayer)
                    {
                        blob.micronQuad = ((DieLayer)(cluster.Layer)).PixelToMicron(blob.pixelRect);
                    }
                    else
                    {
                        blob.micronQuad = cluster.Layer.Matrix.pixelToMicron(blob.pixelRect);
                    }
                    if (cluster.Layer.Matrix is RectangularMatrix)
                    {
                        RectangularMatrix mat = cluster.Layer.Matrix as RectangularMatrix;
                        double pixAreaSize = (mat.PixelSize.Width * mat.PixelSize.Height);
                        blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.pixelArea * pixAreaSize);
                    }
                    else if (cluster.Layer.Matrix is AffineMatrix)
                    {
                        AffineMatrix mat = cluster.Layer.Matrix as AffineMatrix;
                        double pixAreaSize = (mat.PixelSize.Width * mat.PixelSize.Height);
                        blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.pixelArea * pixAreaSize);
                    }
                    else if (cluster.Layer.Matrix is EyeEdgeMatrix)
                    {
                        EyeEdgeMatrix mat = cluster.Layer.Matrix as EyeEdgeMatrix;
                        double pixAreaSize = (mat.PixelSize.Width * mat.PixelSize.Height);
                        blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.pixelArea * pixAreaSize);
                    }
                    else
                    {
                        blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.micronQuad.SurroundingRectangle.Area());
                        blob.characteristics.Add(ClusterCharacteristics.RealWidth, (double)blob.micronQuad.SurroundingRectangle.Width);
                        blob.characteristics.Add(ClusterCharacteristics.RealHeight, (double)blob.micronQuad.SurroundingRectangle.Height);
                    }
                }

                //--------------------------------------------------------------
                // Récupération des autres caractéristiques
                //--------------------------------------------------------------
                foreach (Characteristic carac in blobCharacteristics)
                {
                    // Récupération des valeurs associées à la carac
                    int milFeature = milFeaturesDico[carac];
                    double[] lfDataBlob = blobResult.GetResult(milFeature);

                    // Ajout d'un offset pour convetir X/Y du repère cluster vers le repère image
                    double offset = 0;
                    if (milFeature == MIL.M_BOX_X_MIN || milFeature == MIL.M_BOX_X_MAX)
                        offset = cluster.imageRect.X;
                    else if (milFeature == MIL.M_BOX_Y_MIN || milFeature == MIL.M_BOX_Y_MAX)
                        offset = cluster.imageRect.Y;

                    // Mise à jour des blobs
                    for (int i = 0; i < nbBlobs; i++)
                    {
                        lfDataBlob[i] += offset;
                        module.logDebug("blob " + blobList[i] + "  " + carac + " = " + lfDataBlob[i]);
                        blobList[i].characteristics.Add(carac, lfDataBlob[i]);
                    }
                }
            }

            cluster.blobList.AddRange(blobList);

            return blobList;
        }


        //=================================================================
        // Calcul des caracteristiques du cluster en "sommant" les 
        // caracteristiques des blobs.
        //=================================================================
        protected void ComputeClusterCharacteristics(Cluster cluster)
        {
            var ClusterStatTypeList = new List<MIL_INT>();

            //-------------------------------------------------------------
            // Calcul du rectangle englobant
            //-------------------------------------------------------------
            int left = (int)CharacteristicMinimum(cluster, BlobCharacteristics.BOX_X_MIN);
            int top = (int)CharacteristicMinimum(cluster, BlobCharacteristics.BOX_Y_MIN);
            int right = (int)CharacteristicMaximum(cluster, BlobCharacteristics.BOX_X_MAX);
            int bottom = (int)CharacteristicMaximum(cluster, BlobCharacteristics.BOX_Y_MAX);
            Rectangle pixelSurroundingRect = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
            QuadF micronQuad = cluster.Layer.Matrix.pixelToMicron(pixelSurroundingRect);
            RectangleF micronSurroundingRect = micronQuad.SurroundingRectangle;

            double area = CharacteristicSum(cluster, ClusterCharacteristics.Area);

            //-------------------------------------------------------------
            // Calcul des caractérisques à partir des blobs
            //-------------------------------------------------------------
            foreach (Characteristic carac in paramCharacterization.ClusterCharacteristicList)
            {
                //--------------------
                if (carac == ClusterCharacteristics.Area)
                {
                    cluster.characteristics[carac] = area;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Barycenter)
                {
                    double lfCompactness = CharacteristicAverage(cluster, ClusterCharacteristics.Compactness);
                    double lfElongation = CharacteristicAverage(cluster, ClusterCharacteristics.Elongation);

                    cluster.characteristics[carac] = lfCompactness / lfElongation;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.SymetricDispersion)
                {
                    if (pixelSurroundingRect.Width > pixelSurroundingRect.Height)
                        cluster.characteristics[carac] = (double)pixelSurroundingRect.Height / pixelSurroundingRect.Width;
                    else
                        cluster.characteristics[carac] = (double)pixelSurroundingRect.Width / pixelSurroundingRect.Height;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.FillingValue)
                {
                    double surroundRectanglesArea = CharacteristicSurroundingRectanglesArea(cluster);
                    if (surroundRectanglesArea == 0)    // no blob, so area==0
                        surroundRectanglesArea = 1;

                    cluster.characteristics[carac] = area / surroundRectanglesArea;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Breadth)
                {
                    cluster.characteristics[carac] = CharacteristicAverage(cluster, ClusterCharacteristics.Breadth);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Compactness)
                {
                    cluster.characteristics[carac] = CharacteristicAverage(cluster, ClusterCharacteristics.Compactness);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.ConvexPerimeter)
                {
                    cluster.characteristics[carac] = CharacteristicSum(cluster, ClusterCharacteristics.ConvexPerimeter);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Elongation)
                {
                    cluster.characteristics[carac] = CharacteristicSum(cluster, ClusterCharacteristics.Elongation);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.EulerNumber)
                {
                    cluster.characteristics[carac] = CharacteristicSum(cluster, ClusterCharacteristics.EulerNumber);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Perimeter)
                {
                    cluster.characteristics[carac] = CharacteristicSum(cluster, ClusterCharacteristics.Perimeter);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Roughness)
                {
                    cluster.characteristics[carac] = CharacteristicAverage(cluster, ClusterCharacteristics.Roughness);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.Length)
                {
                    cluster.characteristics[carac] = CharacteristicSum(cluster, ClusterCharacteristics.Length);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.AxisPrincipalAngle)
                {
                    cluster.characteristics[carac] = CharacteristicAverage(cluster, ClusterCharacteristics.AxisPrincipalAngle);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.AxisSecondaryAngle)
                {
                    cluster.characteristics[carac] = CharacteristicAverage(cluster, ClusterCharacteristics.AxisSecondaryAngle);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.RadialPosition)
                {
                    PointF center = micronSurroundingRect.Middle();
                    cluster.characteristics[carac] = center.Radius();
                }
                //--------------------
                else if (carac == ClusterCharacteristics.AbsolutePosition)
                {
                    cluster.characteristics[carac] = micronSurroundingRect;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.SurroundingRectangleArea)
                {
                    cluster.characteristics[carac] = micronSurroundingRect.Area();
                }
                //--------------------
                else if (carac == ClusterCharacteristics.RealDiameter)
                {
                    double diameter = Math.Sqrt(Math.Pow(micronSurroundingRect.Width, 2) + Math.Pow(micronSurroundingRect.Height, 2));
                    cluster.characteristics[carac] = diameter;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.RealHeight)
                {
                    cluster.characteristics[carac] = (double)micronSurroundingRect.Height;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.RealWidth)
                {
                    cluster.characteristics[carac] = (double)micronSurroundingRect.Width;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.BlobAverageGreyLevel)
                {
                    double sumLevel = CharacteristicSum(cluster, ClusterCharacteristics.SumLevel);
                    cluster.characteristics[carac] = sumLevel / area;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.BlobMaxGreyLevel)
                {
                    cluster.characteristics[carac] = CharacteristicMaximum(cluster, ClusterCharacteristics.BlobMaxGreyLevel);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.BlobMinGreyLevel)
                {
                    cluster.characteristics[carac] = CharacteristicMinimum(cluster, ClusterCharacteristics.BlobMinGreyLevel);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.BlobStandardDev)
                {
                    cluster.characteristics[carac] = CharacteristicAverage(cluster, ClusterCharacteristics.BlobStandardDev);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.BlobCount)
                {
                    cluster.characteristics[carac] = (double)cluster.blobList.Count();
                }
                //--------------------
                else if (carac == ClusterCharacteristics.RatioVertical)
                {
                    cluster.characteristics[carac] = (double)pixelSurroundingRect.Height / pixelSurroundingRect.Width;
                }
                //--------------------
                else if (carac == ClusterCharacteristics.SumLevel)
                {
                    cluster.characteristics[carac] = CharacteristicSum(cluster, ClusterCharacteristics.SumLevel);
                }
                //--------------------
                else if (carac == ClusterCharacteristics.AnglePosition)
                {
                    PointF center = micronSurroundingRect.Middle();
                    cluster.characteristics[carac] = center.Angle();
                }
                //--------------------
                else if (carac == ClusterCharacteristics.ClusterAverageGreyLevel)
                {
                    ClusterStatTypeList.Add(MIL.M_STAT_MEAN);
                }
                else if (carac == ClusterCharacteristics.ClusterMinGreyLevel)
                {
                    ClusterStatTypeList.Add(MIL.M_STAT_MIN);
                }
                else if (carac == ClusterCharacteristics.ClusterMaxGreyLevel)
                {
                    ClusterStatTypeList.Add(MIL.M_STAT_MAX);
                }
                else if (carac == ClusterCharacteristics.ClusterStandardDev)
                {
                    ClusterStatTypeList.Add(MIL.M_STAT_STANDARD_DEVIATION);
                }
                //--------------------
                else
                {
                    throw new ApplicationException("unknown cluster characteristic");
                }
            }

            //-------------------------------------------------------------
            // Caractérisques calculées sur le Cluster
            //-------------------------------------------------------------
            if (ClusterStatTypeList.Count != 0)
                ComputeClusterStat(cluster, ClusterStatTypeList);
        }

        //=================================================================
        // Moyenne d'une caractéristique pour l'ensemble des blobs 
        // composant le cluster
        //=================================================================
        private double CharacteristicAverage(Cluster cluster, Characteristic carac)
        {
            double result = 0;
            int iCounter = 0;

            foreach (Blob blob in cluster.blobList)
            {
                result += (double)blob.characteristics[carac];
                iCounter++;
            }

            if (iCounter != 0)
                result = result / iCounter;

            return result;
        }

        //=================================================================
        // Somme d'une caractéristique pour l'ensemble des blobs composant le cluster
        //=================================================================
        private double CharacteristicSum(Cluster cluster, Characteristic carac)
        {
            double result = 0;

            foreach (Blob blob in cluster.blobList)
            {
                result += (double)blob.characteristics[carac];
            }

            return result;
        }

        //=================================================================
        // Maximum d'une caractéristique pour l'ensemble des blobs composant le cluster
        //=================================================================
        private double CharacteristicMaximum(Cluster cluster, Characteristic carac)
        {
            double max = Double.MinValue;

            foreach (Blob blob in cluster.blobList)
            {
                if (max < (double)blob.characteristics[carac])
                    max = (double)blob.characteristics[carac];
            }

            return max;
        }

        //=================================================================
        // Minimum d'une caractéristique pour l'ensemble des blobs composant le cluster
        //=================================================================
        private double CharacteristicMinimum(Cluster cluster, Characteristic carac)
        {
            double min = Double.MaxValue;

            foreach (Blob blob in cluster.blobList)
            {
                if (min > (double)blob.characteristics[carac])
                    min = (double)blob.characteristics[carac];
            }

            return min;
        }

        //=================================================================
        // Ou booléen sur les caractéristiques des blobs
        //=================================================================
        private bool CharacteristicBooleanOr(Cluster cluster, Characteristic carac)
        {
            foreach (Blob blob in cluster.blobList)
            {
                if ((double)blob.characteristics[carac] == 1)
                    return true;
            }

            return false;
        }

        //=================================================================
        // Somme des aires des rectangles englobants. 
        //=================================================================
        private double CharacteristicSurroundingRectanglesArea(Cluster cluster)
        {
            double result = 0;

            foreach (Blob blob in cluster.blobList)
                result = result + blob.pixelRect.Area();

            return result;
        }

        //=================================================================
        // 
        //=================================================================
        private void ComputeClusterStat(Cluster cluster, List<MIL_INT> StatTypes)
        {
            using (var stat = new MilImageResult())
            {
                MilImage milImage = cluster.OriginalProcessingImage.GetMilImage();
                stat.AllocResult(milImage.OwnerSystem, MIL.M_DEFAULT, MIL.M_STATISTICS_RESULT);

                stat.Stat(milImage, StatTypes);

                if (StatTypes.Contains(MIL.M_STAT_MIN))
                    cluster.characteristics[ClusterCharacteristics.ClusterMinGreyLevel] = stat.GetResult(MIL.M_STAT_MIN);
                if (StatTypes.Contains(MIL.M_STAT_MAX))
                    cluster.characteristics[ClusterCharacteristics.ClusterMaxGreyLevel] = stat.GetResult(MIL.M_STAT_MAX);
                if (StatTypes.Contains(MIL.M_STAT_MEAN))
                    cluster.characteristics[ClusterCharacteristics.ClusterAverageGreyLevel] = stat.GetResult(MIL.M_STAT_MEAN);
                if (StatTypes.Contains(MIL.M_STAT_STANDARD_DEVIATION))
                    cluster.characteristics[ClusterCharacteristics.ClusterStandardDev] = stat.GetResult(MIL.M_STAT_STANDARD_DEVIATION);
            }
        }

        //=================================================================
        // Conversion une carac cluster en carac blob
        //=================================================================
        private static bool InitClusterToBlobCharacteristicDico()
        {
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Area, ClusterCharacteristics.Area);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Barycenter, ClusterCharacteristics.Compactness, ClusterCharacteristics.Elongation);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.SymetricDispersion);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.FillingValue);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Breadth, ClusterCharacteristics.Breadth);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Compactness, ClusterCharacteristics.Compactness);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.ConvexPerimeter, ClusterCharacteristics.ConvexPerimeter);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Elongation, ClusterCharacteristics.Elongation);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Perimeter, ClusterCharacteristics.Perimeter);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.EulerNumber, ClusterCharacteristics.EulerNumber);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Roughness, ClusterCharacteristics.Roughness);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.Length, ClusterCharacteristics.Length);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.AxisPrincipalAngle, ClusterCharacteristics.AxisPrincipalAngle);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.AxisSecondaryAngle, ClusterCharacteristics.AxisSecondaryAngle);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.RadialPosition);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.AbsolutePosition,
                                                BlobCharacteristics.BOX_X_MIN, BlobCharacteristics.BOX_Y_MIN, BlobCharacteristics.BOX_X_MAX, BlobCharacteristics.BOX_Y_MAX);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.SurroundingRectangleArea);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.RealDiameter);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.RealHeight);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.RealWidth);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.BlobMinGreyLevel, ClusterCharacteristics.BlobMinGreyLevel);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.ClusterMinGreyLevel);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.BlobMaxGreyLevel, ClusterCharacteristics.BlobMaxGreyLevel);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.ClusterMaxGreyLevel);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.BlobAverageGreyLevel, ClusterCharacteristics.SumLevel, ClusterCharacteristics.Area);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.ClusterAverageGreyLevel);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.BlobStandardDev, ClusterCharacteristics.BlobStandardDev);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.ClusterStandardDev);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.BlobCount);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.RatioVertical);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.SumLevel, ClusterCharacteristics.SumLevel);
            clusterToBlobCharacteristicDico.Add(ClusterCharacteristics.AnglePosition);

            return true;
        }

        //=================================================================
        // Conversion d'une carac en feature MIL
        //=================================================================
        private static bool InitMilFeaturesDico()
        {
            Characteristic.Init();

            // Toutes les carac ne sont pas supportées sur les blobs
            milFeaturesDico.Add(ClusterCharacteristics.Area, MIL.M_AREA);
            //milFeaturesDico.Add(ClusterCharacteristics.Barycenter, MIL.M_COMPACTNESS, MIL.M_ELONGATION);
            //milFeaturesDico.Add(ClusterCharacteristics.SymetricDispersion);
            //milFeaturesDico.Add(ClusterCharacteristics.FillingValue);
            milFeaturesDico.Add(ClusterCharacteristics.Breadth, MIL.M_BREADTH);
            milFeaturesDico.Add(ClusterCharacteristics.Compactness, MIL.M_COMPACTNESS);
            milFeaturesDico.Add(ClusterCharacteristics.ConvexPerimeter, MIL.M_CONVEX_PERIMETER);
            milFeaturesDico.Add(ClusterCharacteristics.Elongation, MIL.M_ELONGATION);
            milFeaturesDico.Add(ClusterCharacteristics.Perimeter, MIL.M_PERIMETER);
            milFeaturesDico.Add(ClusterCharacteristics.EulerNumber, MIL.M_EULER_NUMBER);
            milFeaturesDico.Add(ClusterCharacteristics.Roughness, MIL.M_ROUGHNESS);
            milFeaturesDico.Add(ClusterCharacteristics.Length, MIL.M_LENGTH);
            milFeaturesDico.Add(ClusterCharacteristics.AxisPrincipalAngle, MIL.M_AXIS_PRINCIPAL_ANGLE);
            milFeaturesDico.Add(ClusterCharacteristics.AxisSecondaryAngle, MIL.M_AXIS_SECONDARY_ANGLE);
            //milFeaturesDico.Add(ClusterCharacteristics.RadialPosition);
            //milFeaturesDico.Add(ClusterCharacteristics.AbsolutePosition);
            //milFeaturesDico.Add(ClusterCharacteristics.SurroundingRectangleArea);
            //milFeaturesDico.Add(ClusterCharacteristics.RealDiameter);
            //milFeaturesDico.Add(ClusterCharacteristics.RealHeight);
            //milFeaturesDico.Add(ClusterCharacteristics.RealWidth);
            milFeaturesDico.Add(ClusterCharacteristics.BlobAverageGreyLevel, MIL.M_MEAN_PIXEL);
            milFeaturesDico.Add(ClusterCharacteristics.BlobMaxGreyLevel, MIL.M_MAX_PIXEL);
            milFeaturesDico.Add(ClusterCharacteristics.BlobMinGreyLevel, MIL.M_MIN_PIXEL);
            milFeaturesDico.Add(ClusterCharacteristics.BlobStandardDev, MIL.M_SIGMA_PIXEL);
            //milFeaturesDico.Add(ClusterCharacteristics.BlobCount);
            //milFeaturesDico.Add(ClusterCharacteristics.RatioVertical);
            milFeaturesDico.Add(ClusterCharacteristics.SumLevel, MIL.M_SUM_PIXEL);
            //milFeaturesDico.Add(ClusterCharacteristics.AnglePosition);

            milFeaturesDico.Add(BlobCharacteristics.BOX_X_MIN, MIL.M_BOX_X_MIN);
            milFeaturesDico.Add(BlobCharacteristics.BOX_X_MAX, MIL.M_BOX_X_MAX);
            milFeaturesDico.Add(BlobCharacteristics.BOX_Y_MIN, MIL.M_BOX_Y_MIN);
            milFeaturesDico.Add(BlobCharacteristics.BOX_Y_MAX, MIL.M_BOX_Y_MAX);
            milFeaturesDico.Add(BlobCharacteristics.CENTER_OF_GRAVITY_X, MIL.M_CENTER_OF_GRAVITY_X);
            milFeaturesDico.Add(BlobCharacteristics.CENTER_OF_GRAVITY_Y, MIL.M_CENTER_OF_GRAVITY_Y);
            milFeaturesDico.Add(BlobCharacteristics.REAL_PX_AREA, MIL.M_AREA);

            return true;
        }

    }
}
