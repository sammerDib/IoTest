using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Controls;

using AdcBasicObjects;
using AdcBasicObjects.Rendering;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

namespace AdvancedModules.ClusterOperation.Stitching
{
    public class StitchingModule : QueueModuleBase<Stitch>
    {
        // Internal data
        //..............
        private MergeGrid<Stitch> mergeGrid;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramStitchDistance;
        private int nbCandidates;

        //=================================================================
        // Constructeur
        //=================================================================
        public StitchingModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramStitchDistance = new IntParameter(this, "StitchDistance");
            paramStitchDistance.Value = 5;
        }

        //=================================================================
        // Rendering
        //=================================================================
        private ImageRenderingViewModel _renderingVM;
        private ImageRenderingView _renderingView;
        public override UserControl RenderingUI
        {
            get
            {
                if (_renderingVM == null)
                    _renderingVM = new ImageRenderingViewModel(this);
                if (_renderingView == null)
                {
                    _renderingView = new ImageRenderingView();
                    _renderingView.DataContext = _renderingVM;
                }

                return _renderingView;
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            //-------------------------------------------------------------
            // Création de la grille
            //-------------------------------------------------------------

            // Récupération de la Layer
            //.........................
            List<ModuleBase> ancestors = FindAncestors(m => m is IDataLoader);
            if (ancestors.Count == 0)
                throw new ApplicationException("Can't find DataLoader");
            if (ancestors.Count > 1)
                throw new ApplicationException("Stitching more than one layer");
            IDataLoader dataloader = (IDataLoader)ancestors[0];

            MosaicLayer layer = dataloader.Layer as MosaicLayer;
            if (layer == null)
                throw new ApplicationException("Stitching a layer that is not a mosaic");

            // Création de la grille
            //......................
            Rectangle rect = new Rectangle(new Point(0, 0), layer.FullImageSize);
            mergeGrid = new MergeGrid<Stitch>(MergeStitch, GetStitchRectangle);
            mergeGrid.Init(rect, step: 1000);
            mergeGrid.mergeNeighbourDistance = paramStitchDistance;

            //-------------------------------------------------------------
            // Autres inits
            //-------------------------------------------------------------
            nbCandidates = 0;
            InitOutputQueue();
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Cluster cluster = (Cluster)obj;

            //-------------------------------------------------------------
            // Sanity check
            //-------------------------------------------------------------
            var res = cluster.Layer.Wafer.IsQuadInside(cluster.micronQuad);
            if (res == eCompare.QuadIsOutside)
                throw new ApplicationException("cluster is outside of wafer");

            //-------------------------------------------------------------
            // Processing du cluster
            //-------------------------------------------------------------
            if (IsClusterOnMosaicBorder(cluster))
            {
                // Le cluster est candidat pour le stitching
                //..........................................
                using (Stitch stitch = new Stitch(cluster))
                {

                    lock (mergeGrid)
                    {
                        nbCandidates++;
                        mergeGrid.Merge(stitch);
                        stitch.AddRef();
                    }
                }
            }
            else
            {
                // Pas besoin de stitcher
                //.......................
                ProcessChildren(cluster);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            Scheduler.StartSingleTask("ProcessStitching", () => ProcessStitching());
            base.OnStopping(oldState);
        }

        //=================================================================
        //
        //=================================================================
        public override void Abort()
        {
            logDebug("abort");
            // On abort pas la queue maintenant car les parents peuvent encore pousser des données.
            // La queue sera fermée quand les parents seront stoppés.
            SetState(eModuleState.Aborting);
        }

        //=================================================================
        //
        //=================================================================
        public override void GetStats(out int nbIn, out int nbOut)
        {
            nbIn = nbObjectsIn;
            nbOut = nbObjectsOut;
        }

        //=================================================================
        // 
        //=================================================================
        private bool IsClusterOnMosaicBorder(Cluster cluster)
        {
            MosaicLayer layer = (MosaicLayer)cluster.Layer;

            Rectangle pixelRect = cluster.pixelRect;
            Rectangle mosaicRect = pixelRect.SnapToGrid(layer.MosaicImageSize.Width, layer.MosaicImageSize.Height);

            bool border = false;
            border = border || ((pixelRect.X - mosaicRect.X) <= paramStitchDistance);
            border = border || ((pixelRect.Y - mosaicRect.Y) <= paramStitchDistance);
            border = border || ((mosaicRect.Right - pixelRect.Right) <= paramStitchDistance);
            border = border || ((mosaicRect.Bottom - pixelRect.Bottom) <= paramStitchDistance);

            return border;
        }

        //=================================================================
        // Delegate pour la MergeGrid
        //=================================================================
        private bool MergeStitch(Stitch stitch1, Stitch stitch2)
        {
            bool compatible = CanStitch(stitch1, stitch2);
            if (compatible)
                MergeCompatibleStitches(stitch1, stitch2);

            return compatible;
        }

        //=================================================================
        // Delegate pour la MergeGrid
        //=================================================================
        private Rectangle GetStitchRectangle(Stitch stitch)
        {
            return stitch.surroundingPixelRect;
        }

        ///=================================================================
        ///<summary>
        /// Est-ce que les stitch peuvent être fusionnés ?
        ///</summary>
        ///=================================================================
        private bool CanStitch(Stitch stitch1, Stitch stitch2)
        {
            bool compatible = false;
            foreach (Cluster cl1 in stitch1.clusterList)
            {
                MosaicLayer layer = (MosaicLayer)cl1.Layer;
                MosaicLayer.LC lc1 = layer.GetLineColum(cl1.pixelRect.TopLeft());


                // Comme on traite les clusters à peu près dans l'ordre où ils
                // sont créés, il est plus probable que l'intersection se fasse
                // avec un cluster de la fin de la liste.
                //..............................................................
                for (int i = stitch2.clusterList.Count - 1; i >= 0; i--)
                {
                    Cluster cl2 = stitch2.clusterList[i];

                    // Vérifie que les clusters ne proviennent pas de la même tesselle
                    //................................................................
                    MosaicLayer.LC lc2 = layer.GetLineColum(cl2.pixelRect.TopLeft());
                    if (lc1.l == lc2.l && lc1.c == lc2.c)
                        continue;

                    // Est-ce que les clusters se "touchent" ?
                    //........................................
                    RectangleF r1 = cl1.pixelRect;
                    r1.Inflate(paramStitchDistance, paramStitchDistance);
                    RectangleF r2 = cl2.pixelRect;
                    // Ne pas augmenter r2
                    compatible = r1.IntersectsWith(r2);
                    if (compatible)
                        return true;
                }
            }

            return false;
        }

        ///=================================================================<summary>
        /// Fusionne la liste des clusters des stitch
        ///</summary>=================================================================
        private void MergeCompatibleStitches(Stitch stitch1, Stitch stitch2)
        {
            // Mise à jour des rectangles
            //...........................
            stitch1.surroundingImageRect = Rectangle.Union(stitch1.surroundingImageRect, stitch2.surroundingImageRect);
            stitch1.surroundingPixelRect = Rectangle.Union(stitch1.surroundingPixelRect, stitch2.surroundingPixelRect);

            // Optimisation: On exchange les listes de clusters pour 
            // pour les merger plus rapidement
            //......................................................
            List<Cluster> list1 = stitch1.clusterList;
            List<Cluster> list2 = stitch2.clusterList;
            if (list1.Count < list2.Count)
            {
                stitch1.clusterList = list2;
                stitch2.clusterList = list1;
                stitch1.firstCluster = stitch2.firstCluster;
            }

            // Ajout des clusters dans la liste
            //.................................
            foreach (Cluster cl2 in stitch2.clusterList)
            {
                cl2.AddRef();
                stitch1.clusterList.Add(cl2);
                if (cl2.Index < stitch1.firstCluster.Index)
                    stitch1.firstCluster = cl2;
            }

            stitch2.DelRef();
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessStitching()
        {
            int nb_stitches = nbCandidates - mergeGrid.Count;
            logDebug("Starting Process Stitching, nb_clusters: " + nbObjectsIn + "  nb_clusters_on_borders: " + nbCandidates + "  stitchs: " + nb_stitches);

            //-------------------------------------------------------------
            // Debug
            //-------------------------------------------------------------
            // DumpStitches();

            //-------------------------------------------------------------
            // On met les Stitch dans la queue
            //-------------------------------------------------------------
            foreach (Stitch stitch in mergeGrid.GetAll())
            {
                if (State != eModuleState.Aborting)
                {
                    outputQueue.Enqueue(stitch);
                }
                stitch.DelRef();
            }

            //-------------------------------------------------------------
            // Puis on arrête la queue
            //-------------------------------------------------------------
            if (State == eModuleState.Aborting)
                outputQueue.AbortQueue();

            logDebug("end");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void ProcessQueueElement(Stitch stitch)
        {
            if (State != eModuleState.Aborting)
            {
                Cluster cluster = StitchVignettes(stitch);
                ProcessChildren(cluster);
            }
        }

        //=================================================================
        // Stitching des vignettes des clusters
        // - on met à jour le 1er cluster,
        // - on supprime les autres clusters.
        //=================================================================
        protected Cluster StitchVignettes(Stitch stitch)
        {
            Cluster cl1 = stitch.firstCluster;

            if (stitch.clusterList.Count == 1)  // un seul cluster, pas besoin de stitcher
                return cl1;

            //-------------------------------------------------------------
            // Création de la vignette Résult
            //-------------------------------------------------------------
            Rectangle imageRect = stitch.surroundingImageRect;
            int imgtype = cl1.ResultProcessingImage.GetMilImage().Type;
            int imgattr = cl1.ResultProcessingImage.GetMilImage().Attribute;

            // Allocation
            //...........
            MilImage milResultImage;
            using (milResultImage = new MilImage())
            {
                milResultImage.Alloc2d(imageRect.Width, imageRect.Height, imgtype, imgattr);
                milResultImage.Clear();

                // Copie des images des clusters
                //..............................
                foreach (Cluster cl in stitch.clusterList)
                {
                    // Position du cluster dans la nouvelle vignette
                    int x = cl.imageRect.X - imageRect.X;
                    int y = cl.imageRect.Y - imageRect.Y;

                    MilImage.CopyClip(cl.ResultProcessingImage.GetMilImage(), milResultImage, x, y);
                }

                // Mise à jour du cluster résultat
                //................................
                cl1.ResultProcessingImage.SetMilImage(milResultImage);
            }

            //-------------------------------------------------------------
            // Création de la vignette Originale
            //-------------------------------------------------------------
            using (MilImage milOrigninalImage = new MilImage())
            {
                milOrigninalImage.Alloc2d(imageRect.Width, imageRect.Height, imgtype, imgattr);

                ImageLayerBase layer = (ImageLayerBase)cl1.Layer;
                layer.CopyImageDataTo(milOrigninalImage, imageRect);

                cl1.OriginalProcessingImage.SetMilImage(milOrigninalImage);
                cl1.CurrentIsOriginal = true;
            }

            //-------------------------------------------------------------
            // Copie des autres informations
            //-------------------------------------------------------------
            if (cl1.characteristics.Count != 0)
                throw new ApplicationException("Stitching must be done before Characterization");

            cl1.imageRect = stitch.surroundingImageRect;
            cl1.pixelRect = stitch.surroundingPixelRect;
            cl1.micronQuad = cl1.Layer.Matrix.pixelToMicron(imageRect);

            return cl1;
        }

        //=================================================================
        // 
        //=================================================================
        private void DumpStitches()
        {
            using (StreamWriter stream = new StreamWriter(@"c:\temp\stitches.log"))
            {
                List<Stitch> stitchlist = mergeGrid.GetAll();
                foreach (Stitch stitch in stitchlist)
                    stitch.clusterList.Sort((cl1, cl2) => cl1.Index - cl2.Index);

                stitchlist.Sort((st1, st2) => st1.clusterList[0].Index - st2.clusterList[0].Index);

                foreach (Stitch stitch in stitchlist)
                {
                    foreach (Cluster cl in stitch.clusterList)
                        stream.Write(cl + " ");
                    stream.WriteLine();
                }
            }
        }


    }
}
