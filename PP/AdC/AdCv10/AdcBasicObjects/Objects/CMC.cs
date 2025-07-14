using System.Collections.Generic;
using System.Drawing;

using ADCEngine;

using AdcTools;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Cluster Multi-Couche.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class CMC : ObjectBase
    {
        ///=================================================================<summary>
        /// Regroupe les clusters d'une branche du CMC
        ///</summary>=================================================================
        public class CmcBranch
        {
            public List<Cluster> clusterList = new List<Cluster>();
            public int branchIndex;
            public Rectangle pixelRect;
            public Rectangle imageRect;

            public CmcBranch(Cluster cluster, int branchIndex)
            {
                cluster.AddRef();
                clusterList.Add(cluster);

                pixelRect = cluster.pixelRect;
                imageRect = cluster.imageRect;
                this.branchIndex = branchIndex;
            }
        }

        //=================================================================
        // Data
        //=================================================================
        public Dictionary<int, CmcBranch> cmcBranchList = new Dictionary<int, CmcBranch>();
        public Rectangle micronRect = new Rectangle();

        //=================================================================
        // Constructeur
        //=================================================================
        public CMC(Cluster cluster, int branch)
        {
            CmcBranch cmclayer = new CmcBranch(cluster, branch);
            cmcBranchList[branch] = cmclayer;

            micronRect = cluster.micronQuad.SurroundingRectangle.ToRectangle();

            Name = "CMC-" + cluster.Index;
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            foreach (CmcBranch cmclayer in cmcBranchList.Values)
            {
                foreach (Cluster cluster in cmclayer.clusterList)
                    cluster.DelRef();
                cmclayer.clusterList.Clear();
            }

            cmcBranchList.Clear();

            base.Dispose(disposing);
        }

        //=================================================================
        // 
        //=================================================================
        public IEnumerable<Cluster> GetClusters()
        {
            foreach (CmcBranch cmcbranch in cmcBranchList.Values)
            {
                foreach (Cluster cl in cmcbranch.clusterList)
                    yield return cl;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public Cluster GetFirstClusterFromBranch(int branch)
        {
            CmcBranch cmcbranch;
            bool found = cmcBranchList.TryGetValue(branch, out cmcbranch);
            if (!found)
                return null;
            else
                return cmcbranch.clusterList[0];
        }

        //=================================================================
        // 
        //=================================================================
        public IEnumerable<Cluster> GetClustersFromLayer(string layername)
        {
            foreach (CmcBranch cmcbranch in cmcBranchList.Values)
            {
                foreach (Cluster cluster in cmcbranch.clusterList)
                {
                    if (cluster.Layer.name == layername)
                        yield return cluster;
                }
            }
        }
    }
}
