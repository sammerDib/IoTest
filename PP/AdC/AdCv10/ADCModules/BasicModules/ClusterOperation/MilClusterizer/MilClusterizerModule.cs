
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace BasicModules.MilClusterizer
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class MilClusterizerModule : ClusterizerModuleBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly ConditionalIntParameter paramMinimumArea;
        public readonly ConditionalIntParameter paramMaximumArea;
        public readonly EnumParameter<eClusterizationAlgorithm> paramClusterizationAlgorithm;


        //=================================================================
        // Autres membres
        //=================================================================
        private int _nbBlobs;

        //=================================================================
        // Constructeur
        //=================================================================
        public MilClusterizerModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramMinimumArea = new ConditionalIntParameter(this, "MinimumArea");
            paramMinimumArea.Value = 0;
            paramMinimumArea.IsUsed = true;

            paramMaximumArea = new ConditionalIntParameter(this, "MaximumArea");
            paramMaximumArea.Value = 10000;
            paramMaximumArea.IsUsed = true;

            paramClusterizationAlgorithm = new EnumParameter<eClusterizationAlgorithm>(this, "ClusterizationAlgorithm");
            paramClusterizationAlgorithm.ValueChanged +=
                (algo) =>
                {
                    paramClusterizationStep.IsEnabled = (algo == eClusterizationAlgorithm.SurroundingRectangle);
                };
            paramClusterizationStep.IsEnabled = (paramClusterizationAlgorithm == eClusterizationAlgorithm.SurroundingRectangle);
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _nbBlobs = 0;
        }

        //=================================================================
        // 
        //=================================================================
        public override string Validate()
        {
            string error = base.Validate();
            if (error != null)
                return error;

            if (paramMinimumArea.IsUsed && paramMaximumArea.IsUsed && paramMinimumArea > paramMaximumArea)
                return "inconsistant min/max area";

            return null;
        }

        //=================================================================
        // 
        //=================================================================

        // Malheureusement, il y a un problème dans MIL quand on fait beaucoup
        // d'allocation en parallèle. Donc ça va plus vite si on traite les 
        // images avec un seul thread.
        // Sniff!
        private object mutex = new object();

        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            lock (mutex)
            {
                if (State != eModuleState.Aborting)
                    ProcessSingleThreaded(parent, obj);
            }
        }

        public void ProcessSingleThreaded(ModuleBase parent, ObjectBase obj)
        {
            logDebug("clusterization of image " + obj);
            Interlocked.Increment(ref nbObjectsIn);
            ImageBase image = (ImageBase)obj;
            StoreRenderingObject(image);

            // Découpage des blobs
            //....................
            using (MilBlobResult blobResult = new MilBlobResult())
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                List<Cluster> clusterList = ClusterizeBlobs(image, blobResult);
                stopWatch.Stop();
                logDebug("ClusterizeBlobs " + clusterList.Count() + " blobs  " + stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                if (clusterList.IsNullOrEmpty())
                    return;

                // Algo de regroupement des clusters
                //..................................
                if (paramClusterizationAlgorithm == eClusterizationAlgorithm.SurroundingRectangle)
                {
                    stopWatch.Restart();
                    clusterList = GroupBlobsBySurroundingRectangles(image, clusterList);
                    QueueClusters(image, clusterList);
                    stopWatch.Stop();
                    logDebug("GroupBlobsBySurroundingRectangles " + clusterList.Count() + " blobs  " + stopWatch.Elapsed.ToString(@"hh\:mm\:ss\.fff"));
                }
                else if (paramClusterizationAlgorithm == eClusterizationAlgorithm.Blob)
                {
                    CopyBlobVignette(clusterList, image, blobResult);
                }
                else
                {
                    throw new ApplicationException("unknown clusterization algorithm: " + paramClusterizationAlgorithm.Value);
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected List<Cluster> ClusterizeBlobs(ImageBase image, MilBlobResult blobResult)
        {
            List<Cluster> clusterList = new List<Cluster>();

            if (paramDefectCountLimit.IsUsed && _nbBlobs > paramDefectCountLimit)
            {
                Recipe.PartialAnalysis = true;
                return clusterList;
            }

            using (  MilBlobFeatureList blobFeatureList = new MilBlobFeatureList())
            {
                MilImage milImage = image.ResultProcessingImage.GetMilImage();

                //--------------------------------------------------------------
                // Calcul des blobs
                //--------------------------------------------------------------
                // Features a calculer
                blobFeatureList.Alloc();
                blobFeatureList.SelectFeature(MIL.M_BOX);

                blobResult.Alloc();

                //--------------------------------------------------------------
                // Binarisation de l'image si image 32 bits car les blob ne fonctionne que sur des image 1,8 ou 16
                //--------------------------------------------------------------                
                if (milImage.SizeBit == 32)
                {
                    MilImage binImage = new MilImage();
                    binImage.Alloc2d(milImage.SizeX, milImage.SizeY, 8 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC);
                    MilImage.Binarize(milImage, binImage, MIL.M_GREATER_OR_EQUAL, 1, MIL.M_NULL);

                    image.ResultProcessingImage.SetMilImage(binImage);

                    milImage = image.ResultProcessingImage.GetMilImage();
                }

                // Calcul des features pour tous les blobs                    
                blobResult.Calculate(milImage, null, blobFeatureList);

                //--------------------------------------------------------------
                // Filtrage des blobs
                //--------------------------------------------------------------

                // Exclude blobs whose area is too small/too large.
                if (paramMinimumArea.IsUsed)
                    blobResult.Select(MIL.M_DELETE, MIL.M_AREA, MIL.M_LESS, paramMinimumArea, MIL.M_NULL);
                if (paramMaximumArea.IsUsed)
                    blobResult.Select(MIL.M_DELETE, MIL.M_AREA, MIL.M_GREATER, paramMaximumArea, MIL.M_NULL);
                blobResult.Control(MIL.M_SAVE_RUNS, MIL.M_ENABLE);

                //--------------------------------------------------------------
                // Copie dans l'image d'origine
                //--------------------------------------------------------------
                milImage.Clear();
                blobResult.Fill(milImage, MIL.M_INCLUDED_BLOBS, 255);

                //--------------------------------------------------------------
                // Recuperation des caracteristiques
                //--------------------------------------------------------------
                MIL_INT nbBlobs = blobResult.Number;
                log("image: " + image + "   nb_blobs_found: " + nbBlobs);
                if (nbBlobs == 0)
                    return clusterList;

                // Get blob areas
                double[] lfDataBlob_Left = blobResult.GetResult(MIL.M_BOX_X_MIN);
                double[] lfDataBlob_Right = blobResult.GetResult(MIL.M_BOX_X_MAX);
                double[] lfDataBlob_Top = blobResult.GetResult(MIL.M_BOX_Y_MIN);
                double[] lfDataBlob_Bottom = blobResult.GetResult(MIL.M_BOX_Y_MAX);

                //--------------------------------------------------------------
                // Création des clusters
                //--------------------------------------------------------------
                for (int i = 0; i < nbBlobs; i++)
                {
                    CheckMemoryLimit();
                    if (State == eModuleState.Aborting)
                        break;

                    Interlocked.Increment(ref _nbBlobs);
                    if (paramDefectCountLimit.IsUsed && _nbBlobs > paramDefectCountLimit)
                        break;

                    // Allocation
                    //...........
                    int num = CreateClusterNumber(image, i);
                    Cluster cluster = new Cluster(num, image.Layer);
                    clusterList.Add(cluster);

                    if (image is FullImage)
                    {
                        // Calcul du rectangle
                        //.....................
                        int left = image.imageRect.Left + (int)lfDataBlob_Left[i];
                        int top = image.imageRect.Top + (int)lfDataBlob_Top[i];
                        int right = image.imageRect.Left + (int)lfDataBlob_Right[i];
                        int bottom = image.imageRect.Top + (int)lfDataBlob_Bottom[i];
                        cluster.pixelRect = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
                        // Les coordonnées microns seront calculées après le regroupement des blobs
                    }

                    // Info spécifiques
                    //.................
                    if (image is DieImage)
                    {
                        int left = image.imageRect.Left + (int)lfDataBlob_Left[i];
                        int top = image.imageRect.Top + (int)lfDataBlob_Top[i];
                        int right = image.imageRect.Left + (int)lfDataBlob_Right[i];
                        int bottom = image.imageRect.Top + (int)lfDataBlob_Bottom[i];
                        //int left = (int)lfDataBlob_Left[i];
                        //int top =  (int)lfDataBlob_Top[i];
                        //int right = (int)lfDataBlob_Right[i];
                        //int bottom = (int)lfDataBlob_Bottom[i];
                        cluster.pixelRect = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);
                        DieImage dieimg = (DieImage)image;
                        cluster.DieIndexX = dieimg.DieIndexX;
                        cluster.DieIndexY = dieimg.DieIndexY;
                        cluster.DieOffsetImage = dieimg.imageRect.TopLeft();
                    }

                    if (image is MosaicImage)
                    {
                        // Calcul du rectangle
                        //.....................
                        int left = image.imageRect.Left + (int)lfDataBlob_Left[i];
                        int top = image.imageRect.Top + (int)lfDataBlob_Top[i];
                        int right = image.imageRect.Left + (int)lfDataBlob_Right[i];
                        int bottom = image.imageRect.Top + (int)lfDataBlob_Bottom[i];
                        cluster.pixelRect = Rectangle.FromLTRB(left, top, right + 1, bottom + 1);

                        MosaicImage mosaicimg = (MosaicImage)image;
                        cluster.Line = mosaicimg.Line;
                        cluster.Column = mosaicimg.Column;
                    }

                    // NB: On ne crée pas de liste de blobs
                    //.....................................
                }
            }

            return clusterList;
        }

        //=================================================================
        // 
        //=================================================================
        protected void CopyBlobVignette(List<Cluster> clusterList, ImageBase image, MilBlobResult blobResult)
        {
            using (MilGraphicsContext milGC = new MilGraphicsContext())
            {
                milGC.Alloc(image.OriginalProcessingImage.GetMilImage().OwnerSystem);
                milGC.Color = 255;

                double[] labels = blobResult.GetResult(MIL.M_LABEL_VALUE);

                for (int i = 0; i < clusterList.Count; i++)
                {
                    Cluster cluster = clusterList[i];
                    if (State != eModuleState.Aborting)
                    {
                        cluster.micronQuad = image.Layer.Matrix.pixelToMicron(cluster.pixelRect);
                        AllocateVignette(image, cluster);
                        CopyBlobVignette(image, blobResult, milGC, (long)labels[i], cluster);
                        outputQueue.Enqueue(cluster);
                    }
                    cluster.DelRef();
                }
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void CopyBlobVignette(ImageBase image, MilBlobResult blobResult, MilGraphicsContext milGC, long label, Cluster cluster)
        {
            // Position du cluster dans l'image
            //.................................
            Rectangle vignetteRect = cluster.imageRect.NegativeOffset(image.imageRect.TopLeft());

            // Copie de la vignette Originale (niveaux de gris)
            //.................................................
            MilImage clusterImage = cluster.OriginalProcessingImage.GetMilImage();
            MilImage sourceImage = image.OriginalProcessingImage.GetMilImage();
            MilImage.CopyColor2d(sourceImage, clusterImage,    //src, dest
                 MIL.M_ALL_BAND, vignetteRect.X, vignetteRect.Y,   // src
                 MIL.M_ALL_BAND, 0, 0,   // dest
                 vignetteRect.Width, vignetteRect.Height
                 );

            // Copie de la vignette Résultat (Noir et Blanc)
            //..............................................
            //vignetteRect = cluster.pixelRect.NegativeOffset(image.imageRect.TopLeft());

            clusterImage = cluster.ResultProcessingImage.GetMilImage();

            MIL.MgraControl(milGC, MIL.M_DRAW_OFFSET_X, vignetteRect.X);
            MIL.MgraControl(milGC, MIL.M_DRAW_OFFSET_Y, vignetteRect.Y);
            clusterImage.Clear();
            blobResult.Draw(milGC, clusterImage, MIL.M_DRAW_BLOBS, label);

            // Indique que l'image de travail est l'image originale
            //.....................................................
            cluster.CurrentIsOriginal = true;
        }


    }
}
