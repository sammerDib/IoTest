using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using UnitySC.Shared.LibMIL;

namespace AdvancedModules.CmcNamespace
{
    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class CmcModule : QueueModuleBase<CMC>
    {
        // Internal data
        //..............
        private MergeGrid<CMC> mergeGrid;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly IntParameter paramClusterizationStep;
        public readonly CmcParameter paramClusterizationBranches;

        //=================================================================
        // Constructeur
        //=================================================================
        public CmcModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            paramClusterizationStep = new IntParameter(this, "ClusterizationStep");
            paramClusterizationBranches = new CmcParameter(this, "Branches");

            paramClusterizationStep.Value = 5;
            ModuleProperty = eModuleProperty.Stage;
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

            // Comme le wafer peut-être tourné, il faut un grille plus grande que le wafer.
            float diagonal = (float)Wafer.SurroundingRectangle.ToRectangle().Diagonal();
            float xy = -diagonal / 2;
            RectangleF rectf = new RectangleF(xy, xy, diagonal, diagonal);

            mergeGrid = new MergeGrid<CMC>(MergeCMC, GetCMCRectangle);
            mergeGrid.Init(rectf.ToRectangle(), step: 2000);
            mergeGrid.mergeNeighbourDistance = paramClusterizationStep;

            //-------------------------------------------------------------
            // Autres inits
            //-------------------------------------------------------------
            InitOutputQueue();
        }


        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("from Id = " + parent.Id + " - cluster " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            Cluster cluster = (Cluster)obj;

            // Sanity check
            //.............
            var res = cluster.Layer.Wafer.IsQuadInside(cluster.micronQuad);
            if (res == eCompare.QuadIsOutside)
                throw new ApplicationException("cluster is outside of wafer");

            // Création d'un CMC correspondant au cluster
            //...........................................
            int branch = Parents.IndexOf(parent);
            using (CMC cmc = new CMC(cluster, branch))
            {
                lock (mergeGrid)
                {
                    mergeGrid.Merge(cmc);
                    cmc.AddRef();
                }
            }
        }

        //=================================================================
        // Delegate pour la MergeGrid
        //=================================================================
        protected bool MergeCMC(CMC cmc1, CMC cmc2)
        {
            bool compatible = AreCMCsCompatible(cmc1, cmc2);
            if (compatible)
                MergeCompatibileCMCs(cmc1, cmc2);
            return compatible;
        }

        ///=================================================================
        ///<summary>
        /// Est-ce que les cmc peuvent être fusionnés ?
        ///</summary>
        ///=================================================================
        protected bool AreCMCsCompatible(CMC cmc1, CMC cmc2)
        {
            //-------------------------------------------------------------
            // on ne merge pas si deux clusters de la même couche et intra-clusterisation pas activée
            //-------------------------------------------------------------
            foreach (CMC.CmcBranch cmcbranch1 in cmc1.cmcBranchList.Values)
            {
                if (cmc2.cmcBranchList.ContainsKey(cmcbranch1.branchIndex))
                {
                    if (!paramClusterizationBranches.IntraClusterization[cmcbranch1.branchIndex])
                        return false;
                }
            }

            //-------------------------------------------------------------
            // Comparaison des rectangles des clusters entre les layers
            //-------------------------------------------------------------
            foreach (CMC.CmcBranch cmcbranch1 in cmc1.cmcBranchList.Values)
            {
                foreach (CMC.CmcBranch cmcbranch2 in cmc2.cmcBranchList.Values)
                {
                    if (cmcbranch1.branchIndex == cmcbranch2.branchIndex)
                        continue;

                    RectangleF r1 = cmcbranch1.pixelRect;
                    r1.Inflate(paramClusterizationStep, paramClusterizationStep);
                    RectangleF r2 = cmcbranch2.pixelRect;
                    // Ne pas augmenter r2
                    bool compatible = r1.IntersectsWith(r2);
                    if (compatible)
                        return true;
                }
            }

            return false;
        }

        //=================================================================
        // Merge des CMCs
        //=================================================================
        protected void MergeCompatibileCMCs(CMC cmc1, CMC cmc2)
        {
            //-------------------------------------------------------------
            // Mise à jour du rectangle
            //-------------------------------------------------------------
            cmc1.micronRect = RectangleExtension.Union(cmc1.micronRect, cmc2.micronRect);

            //-------------------------------------------------------------
            // Merge des CmcBranch
            //-------------------------------------------------------------
            foreach (CMC.CmcBranch cmcbranch2 in cmc2.cmcBranchList.Values.ToList())
            {
                CMC.CmcBranch cmcbranch1;
                bool found = cmc1.cmcBranchList.TryGetValue(cmcbranch2.branchIndex, out cmcbranch1);
                if (found)
                {
                    // Mise à jour des rectangles
                    //...........................
                    cmcbranch1.imageRect = RectangleExtension.Union(cmcbranch1.imageRect, cmcbranch2.imageRect);
                    cmcbranch1.pixelRect = RectangleExtension.Union(cmcbranch1.pixelRect, cmcbranch2.pixelRect);

                    // Echange des listes pour éviter de copier de grosses listes
                    //...........................................................
                    List<Cluster> list1 = cmcbranch1.clusterList;
                    List<Cluster> list2 = cmcbranch2.clusterList;
                    if (list1.Count > list2.Count)
                    {
                        cmcbranch1.clusterList = list2;
                        cmcbranch2.clusterList = list1;

                        list1 = cmcbranch1.clusterList;
                        list2 = cmcbranch2.clusterList;
                    }

                    // Merge des listes
                    //.................
                    list1.AddRange(list2);
                    list2.Clear();
                }
                else
                {
                    cmc1.cmcBranchList.Add(cmcbranch2.branchIndex, cmcbranch2);
                    cmc2.cmcBranchList.Remove(cmcbranch2.branchIndex);
                }
            }

            //-------------------------------------------------------------
            // Destruction du CMC mergé
            //-------------------------------------------------------------
            // NB il ne reste plus que des banches vides dans le CMC2.
            foreach (CMC.CmcBranch branch2 in cmc2.cmcBranchList.Values)
            {
                if (branch2.clusterList.Count != 0)
                    throw new ApplicationException("Branches are not empty");
            }

            cmc2.DelRef();
        }

        //=================================================================
        // Delegate pour la MergeGrid
        //=================================================================
        protected Rectangle GetCMCRectangle(CMC cmc)
        {
            return cmc.micronRect;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            log("Starting Process Cmc");
            Task task = Scheduler.StartSingleTask("ProcessCmc", () =>
            {
                ProcessCmc();
                base.OnStopping(oldState);
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessCmc()
        {
            foreach (CMC cmc in mergeGrid.GetAll())
            {
                if (State != eModuleState.Aborting)
                {
                    logDebug("process cmc " + cmc);

                    outputQueue.Enqueue(cmc);
                }
                cmc.DelRef();
            }

            if (State == eModuleState.Aborting)
                outputQueue.AbortQueue();
            else
                outputQueue.CloseQueue();

            logDebug("end");
        }

        //=================================================================
        // 
        //=================================================================
        protected override void ProcessQueueElement(CMC cmc)
        {
            if (State != eModuleState.Aborting)
            {
                MergeClusters(cmc);
                foreach (Cluster cluster in cmc.GetClusters())
                {
                    cluster.characteristics.Add(CmcFactory.NbLayers, (double)cmc.cmcBranchList.Count);
                    cluster.characteristics.Add(CmcFactory.LayerIsMeasured, true);
                }
                ProcessChildren(cmc);
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void MergeClusters(CMC cmc)
        {
            foreach (CMC.CmcBranch cmcbranch in cmc.cmcBranchList.Values)
            {
                if (cmcbranch.clusterList.Count == 1)
                    break;

                MergeClustersInBranch(cmcbranch);
            }
        }

        //=================================================================
        // Merge des clusters d'une branche: 
        // - on met à jour le 1er cluster de la couche en "additionnant"
        // tous les clusters,
        // - on supprime les autres clusters.
        //=================================================================
        protected void MergeClustersInBranch(CMC.CmcBranch cmcbranch)
        {
            //-------------------------------------------------------------
            // Sanity Check
            //-------------------------------------------------------------
            List<Cluster> list = cmcbranch.clusterList;
            ImageLayerBase layer = (ImageLayerBase)list[0].Layer;

            foreach (Cluster cl in list)
            {
                if (cl.Layer != layer)
                    throw new ApplicationException("Layers are not compatible");
            }

            //-------------------------------------------------------------
            // Création d'un nouveau cluster
            //-------------------------------------------------------------
            Cluster result_cluster = new Cluster(-1, layer);

            //-------------------------------------------------------------
            // Création de la vignette Résult
            //-------------------------------------------------------------
            Cluster cl1 = list[0];

            Rectangle imageRect = cmcbranch.imageRect;
            Rectangle pixelRect = cmcbranch.pixelRect;

            // Allocation
            //...........
            MilImage milResultImage;
            using (milResultImage = new MilImage())
                result_cluster.ResultProcessingImage.SetMilImage(milResultImage);

            int imgtype = cl1.ResultProcessingImage.GetMilImage().Type;
            int imgattr = cl1.ResultProcessingImage.GetMilImage().Attribute;
            milResultImage.Alloc2d(imageRect.Width, imageRect.Height, imgtype, imgattr);
            milResultImage.Clear();

            // Copie des images des clusters
            //..............................
            foreach (Cluster cl in list)
            {
                // Position du cluster dans la nouvelle vignette
                Point pos = PointSizeExtension.Subtract(imageRect.TopLeft(), imageRect.TopLeft()).ToPoint();

                MilImage.CopyClip(cl.ResultProcessingImage.GetMilImage(), milResultImage, pos.X, pos.Y);
            }

            //-------------------------------------------------------------
            // Création de la vignette Originale
            //-------------------------------------------------------------
            MilImage milOrigninalImage;
            using (milOrigninalImage = new MilImage())
                result_cluster.OriginalProcessingImage.SetMilImage(milOrigninalImage);

            milOrigninalImage.Alloc2d(imageRect.Width, imageRect.Height, imgtype, imgattr);
            layer.CopyImageDataTo(milOrigninalImage, imageRect);
            cl1.CurrentIsOriginal = true;

            //-------------------------------------------------------------
            // Copie des autres données
            //-------------------------------------------------------------
            cl1.imageRect = imageRect;
            cl1.pixelRect = pixelRect;
            cl1.micronQuad = cl1.Layer.Matrix.pixelToMicron(imageRect);

            // Copie des blobs
            foreach (Cluster cl in list)
            {
                cl1.Index = Math.Min(cl1.Index, cl.Index);
                cl1.defectClassList.AddRange(cl.defectClassList);
                cl1.blobList.AddRange(cl.blobList);
            }

            //-------------------------------------------------------------
            // On ne garde que le premier cluster
            //-------------------------------------------------------------
            foreach (Cluster cl in list)
                cl.DelRef();
            list.Clear();

            list.Add(result_cluster);
        }


    }
}
