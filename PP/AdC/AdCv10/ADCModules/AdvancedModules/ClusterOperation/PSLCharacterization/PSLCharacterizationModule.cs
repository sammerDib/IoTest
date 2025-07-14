using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Drawing;
using System.Windows.Controls;
using Matrox.MatroxImagingLibrary;
using ADCEngine;
using AdcTools;
using libMIL;
using LibProcessing;
using AdcBasicObjects;
using MergeContext.Context;

namespace BasicModules.PSLCharacterization
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    // On a trois types de caractéristiques:
    // - les carac des clusters
    // - les carac des blobs
    // - les features MIL
    // L'utilisateur choisit les carac cluster, on les convertit en carac blob
    // puis en feature MIL.
    // Ici on converti les carac en applicant la LUT PSL sur les blobs Average Grey Level
    // 
    // Ce module peut être lancer seul ou aprés un milcharaterisation classique
    //          si des carac classique MIl n'ont pas été calculé avant on les calculs, sinon on les utilises
    //
    ///////////////////////////////////////////////////////////////////////
    class PSLCharacterizationModule : ModuleBase, ICharacterizationModule
    {
        private List<Characteristic> _availableCharacteristics;
        public List<Characteristic> AvailableCharacteristics
        {
            get
            {
                if (_availableCharacteristics == null)
                {
                    _availableCharacteristics = new List<Characteristic>();
                    _availableCharacteristics.Add(ClusterCharacteristics.PSLValue);
                    _availableCharacteristics.Add(ClusterCharacteristics.PSLMaxValue);
                    _availableCharacteristics.Add(ClusterCharacteristics.BlobMaxGreyLevel);
                }
                return _availableCharacteristics;
            }
        }

        ///<summary> Association Carac Cluster => Carac Blob </summary>
        private static ProcessingClassMil _processClass = new ProcessingClassMil();

        private LookupTable _lutpsl = null;
        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Constructeur
        //=================================================================
        public PSLCharacterizationModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            Cluster cluster = (Cluster)obj;
            logDebug("Caracterization PSL " + cluster);
            Interlocked.Increment(ref nbObjectsIn);

            _lutpsl = cluster.Layer.GetContextMachine<LookupTable>(ConfigurationType.LutPsl.ToString());

            bool bHasBeenPreviouslyCharacterized = cluster.characteristics.ContainsKey(ClusterCharacteristics.BlobMaxGreyLevel);
            // Le cluster contient t'il déjà des élément caratérisées dont aurai besoins pour nos calculs de Psl
            if (!bHasBeenPreviouslyCharacterized)
            {
                // on calcule les blobs
                CreateBlobs(cluster);
            }

            // Les blobs Max Grey Levels ont été calculés on applique la LUT       
            if (cluster.blobList.Count != 0)
            {
                ComputeClusterCharacteristics_PSL(cluster, bHasBeenPreviouslyCharacterized);
                ProcessChildren(obj);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected List<Blob> CreateBlobs(Cluster cluster)
        {
            List<Blob> blobList = new List<Blob>();

            using (MilBlobFeatureList blobFeatureList = new MilBlobFeatureList())
            using (MilBlobResult blobResult = new MilBlobResult())
            {
                MilImage milImage = cluster.ResultProcessingImage.GetMilImage();
                MilImage milOriginalImage = cluster.OriginalProcessingImage.GetMilImage();

                //--------------------------------------------------------------
                // Calcul des blobs
                //--------------------------------------------------------------
                // Features a calculer: conversion carac blob => feature MIL
                blobFeatureList.Alloc();
                blobFeatureList.SelectFeature(MIL.M_BOX);
                blobFeatureList.SelectFeature(MIL.M_MAX_PIXEL);

                // Calcul des features pour tous les blobs
                blobResult.Alloc();
                blobResult.Calculate(milImage, milOriginalImage, blobFeatureList);

                //--------------------------------------------------------------
                // Récuperation des caractéristiques X/Y
                //--------------------------------------------------------------
                MIL_INT nbBlobs = blobResult.Number;
                logDebug("cluster: " + cluster + "   nb_blobs_found: " + nbBlobs);
                if (nbBlobs == 0)
                    return blobList;

                // Get blob info
                double[] lfDataBlob_Left = blobResult.GetResult(MIL.M_BOX_X_MIN);
                double[] lfDataBlob_Right = blobResult.GetResult(MIL.M_BOX_X_MAX);
                double[] lfDataBlob_Top = blobResult.GetResult(MIL.M_BOX_Y_MIN);
                double[] lfDataBlob_Bottom = blobResult.GetResult(MIL.M_BOX_Y_MAX);
                double[] lfDataBlob_MAXGL = blobResult.GetResult(MIL.M_MAX_PIXEL);

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

                    blob.micronQuad = cluster.Layer.Matrix.pixelToMicron(blob.pixelRect);

                    blob.characteristics.Add(ClusterCharacteristics.BlobMaxGreyLevel, lfDataBlob_MAXGL[i]);
                    blob.characteristics.Add(BlobCharacteristics.MicronArea, blob.micronQuad.SurroundingRectangle.Area());
                    blob.characteristics.Add(ClusterCharacteristics.RealWidth, (double)blob.micronQuad.SurroundingRectangle.Width);
                    blob.characteristics.Add(ClusterCharacteristics.RealHeight, (double)blob.micronQuad.SurroundingRectangle.Height);
                }
            }

            cluster.blobList.AddRange(blobList);

            return blobList;
        }


        //=================================================================
        // Calcul des caracteristiques PSL
        //=================================================================
        protected void ComputeClusterCharacteristics_PSL(Cluster cluster, bool p_bHasBeenPreviouslyCharacterized)
        {
            //-------------------------------------------------------------
            // Calcul des caractérisques PSL à partir des blobs
            //-------------------------------------------------------------

            double clusterMaxGL = Double.MinValue;
            double clusterMaxPSL = Double.NegativeInfinity;
            double clusterSumPSL = 0.0;
            bool bCharacMaxGL = !p_bHasBeenPreviouslyCharacterized;
            foreach (Blob blob in cluster.blobList)
            {
                double dValMaxGL = (double)blob.characteristics[ClusterCharacteristics.BlobMaxGreyLevel];
                if (bCharacMaxGL)
                {
                    if (clusterMaxGL < dValMaxGL)
                        clusterMaxGL = dValMaxGL;
                }

                double dValPSL = (double)_lutpsl.LookupValues[(int)dValMaxGL].Value;
                blob.characteristics[BlobCharacteristics.PSLValue] = dValPSL;               // nm
                blob.characteristics[ClusterCharacteristics.RealWidth] = dValPSL / 1000;    // µm
                blob.characteristics[ClusterCharacteristics.RealHeight] = dValPSL / 1000;   // µm

                if (clusterMaxPSL < dValPSL)
                    clusterMaxPSL = dValPSL;
                clusterSumPSL += dValPSL;
            }

            if (bCharacMaxGL)
            {
                cluster.characteristics[ClusterCharacteristics.BlobMaxGreyLevel] = clusterMaxGL;
            }
            cluster.characteristics[ClusterCharacteristics.PSLValue] = clusterSumPSL;
            cluster.characteristics[ClusterCharacteristics.PSLMaxValue] = clusterMaxPSL;
        }
    }
}
