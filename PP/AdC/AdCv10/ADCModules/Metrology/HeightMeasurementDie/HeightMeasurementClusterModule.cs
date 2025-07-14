using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
//using System.Windows.Controls;
using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules;

using UnitySC.Shared.LibMIL;

using Matrox.MatroxImagingLibrary;

namespace HeightMeasurementDieModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HeightMeasurementClusterModule : QueueModuleBase<Cluster3DHMBuilder>, ICharacterizationModule
    {

        private List<Characteristic> supportedCharacteristics = new List<Characteristic>();
        public List<Characteristic> AvailableCharacteristics { get { return supportedCharacteristics; } }


        //=================================================================
        // Paramètres du XML
        //=================================================================

        //=================================================================
        // Constructeur
        //=================================================================
        public HeightMeasurementClusterModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            // characteristic always compute
            supportedCharacteristics.TryAdd(Cluster3DCharacteristics.Height);
            supportedCharacteristics.TryAdd(ClusterCharacteristics.AbsolutePosition);

            ModuleProperty = eModuleProperty.Stage;
        }


        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            InitOutputQueue();
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            if (State == eModuleState.Aborting)
                return;

            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            if (!(obj is Cluster3DDieHM))
                throw new ApplicationException("Object received is not a Cluster 3D Die HM");

            Cluster3DDieHM cluster3DDie = (Cluster3DDieHM)obj;
            MilImage dieImageProcessing = cluster3DDie.CurrentProcessingImage.GetMilImage();
            int nImgType = dieImageProcessing.Type;
            int nImgAttrib = dieImageProcessing.Attribute;

            int nDieIndexX = cluster3DDie.DieIndexX;
            int nDieIndexY = cluster3DDie.DieIndexY;
            Point DieOffsetImage = cluster3DDie.DieOffsetImage;

            int nBbBlobMeasures = cluster3DDie.blobList.Count;
            int ClusterDieIndex = cluster3DDie.Index;
            // Création d'un index unique -- du moins on va essayer ...
            int idx = ClusterDieIndex * nBbBlobMeasures;
            foreach (Blob blobmeasure in cluster3DDie.blobList)
            {
                if (State == eModuleState.Aborting)
                    break;

                Cluster3DHM clusterHM = new Cluster3DHM(idx, cluster3DDie.Layer);
                // Calcul du rectangle englobant ---> already computed in HMDieExecutor
                clusterHM.pixelRect = new Rectangle(blobmeasure.pixelRect.Location, blobmeasure.pixelRect.Size);
                clusterHM.micronQuad = new QuadF(blobmeasure.micronQuad);
                clusterHM.DieIndexX = nDieIndexX;
                clusterHM.DieIndexY = nDieIndexY;
                clusterHM.DieOffsetImage = new Point(DieOffsetImage.X, DieOffsetImage.Y);

                // Add Die Characteristics from Die cluster
                foreach (KeyValuePair<Characteristic, object> kvp in cluster3DDie.characteristics)
                    clusterHM.characteristics.Add(kvp.Key, kvp.Value);

                // add current blob mleasure cluster characteristics
                clusterHM.characteristics[ClusterCharacteristics.AbsolutePosition] = clusterHM.micronQuad;
                clusterHM.characteristics[Cluster3DCharacteristics.Height] = (double)blobmeasure.characteristics[Blob3DCharacteristics.HeightMicron];
                clusterHM.characteristics[Blob3DCharacteristics.SubstrateHeightMicron] = (double)blobmeasure.characteristics[Blob3DCharacteristics.SubstrateHeightMicron];

                // make the blob for coherence
                Blob hmblob = new Blob(blobmeasure.Index, clusterHM);
                hmblob.pixelRect = new Rectangle(blobmeasure.pixelRect.Location, blobmeasure.pixelRect.Size);
                hmblob.micronQuad = new QuadF(blobmeasure.micronQuad);
                hmblob.characteristics.Add(BlobCharacteristics.MicronArea, hmblob.micronQuad.SurroundingRectangle.Area());
                hmblob.characteristics.Add(ClusterCharacteristics.RealWidth, (double)hmblob.micronQuad.SurroundingRectangle.Width);
                hmblob.characteristics.Add(ClusterCharacteristics.RealHeight, (double)hmblob.micronQuad.SurroundingRectangle.Height);

                foreach (KeyValuePair<Characteristic, object> kvp in blobmeasure.characteristics)
                    hmblob.characteristics.Add(kvp.Key, kvp.Value);
                clusterHM.blobList.Add(hmblob);

                AllocateVignette(nImgType, nImgAttrib, clusterHM);
                using (Cluster3DHMBuilder builder = new Cluster3DHMBuilder(clusterHM, cluster3DDie))
                    outputQueue.Enqueue(builder);
                clusterHM.DelRef();

                // on incremente notre index ...
                idx++;
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void AllocateVignette(int imgtype, int imgattr, Cluster3DHM cluster)
        {
            // Taille de la vignette
            //......................
            // int oversize = 2;

            cluster.imageRect = cluster.pixelRect;
            // cluster.imageRect.Inflate(oversize, oversize);
            // cluster.imageRect.Intersect(image.imageRect);

            // Allocation
            //...........
            MilImage milImage = cluster.OriginalProcessingImage.GetMilImage();
            milImage.Alloc2d(cluster.imageRect.Width, cluster.imageRect.Height, imgtype, imgattr);

            milImage = cluster.ResultProcessingImage.GetMilImage();
            milImage.Alloc2d(cluster.imageRect.Width, cluster.imageRect.Height, imgtype, imgattr);
        }

        //=================================================================
        // 
        //=================================================================
        protected void CopyVignette(Cluster3DDieHM diecluster, Cluster3DHM cluster)
        {
            // Position du cluster dans l'image
            //.................................
            Rectangle vignetteRect = cluster.imageRect.NegativeOffset(diecluster.imageRect.TopLeft());

            // Copie des vignettes
            //....................
            MilImage clusterImage = cluster.OriginalProcessingImage.GetMilImage();
            MilImage sourceImage = diecluster.OriginalProcessingImage.GetMilImage();
            MilImage.CopyColor2d(sourceImage, clusterImage,    //src, dest
                 MIL.M_ALL_BAND, vignetteRect.X, vignetteRect.Y,   // src
                 MIL.M_ALL_BAND, 0, 0,   // dest
                 vignetteRect.Width, vignetteRect.Height
                 );

            clusterImage = cluster.ResultProcessingImage.GetMilImage();
            sourceImage = diecluster.ResultProcessingImage.GetMilImage();
            MilImage.CopyColor2d(sourceImage, clusterImage,    //src, dest
                 MIL.M_ALL_BAND, vignetteRect.X, vignetteRect.Y,   // src
                 MIL.M_ALL_BAND, 0, 0,   // dest
                 vignetteRect.Width, vignetteRect.Height
                 );

            // Indique que l'image de travail est l'image originale
            //.....................................................
            cluster.CurrentIsOriginal = true;
        }


        //=================================================================
        // 
        //=================================================================
        protected override void ProcessQueueElement(Cluster3DHMBuilder builder)
        {
            if (State != eModuleState.Aborting)
            {
                CopyVignette(builder.dieclusterimage, builder.cluster);
                ProcessChildren(builder.cluster);
            }
        }
    }
}
