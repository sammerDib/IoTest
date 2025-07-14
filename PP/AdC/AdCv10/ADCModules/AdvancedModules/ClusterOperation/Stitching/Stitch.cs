using System.Collections.Generic;
using System.Drawing;

using AdcBasicObjects;

using ADCEngine;


namespace AdvancedModules.ClusterOperation.Stitching
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Stitch contenant les clusters à stitcher.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class Stitch : ObjectBase
    {
        //=================================================================
        // Paramètres du XML
        //=================================================================
        public List<Cluster> clusterList = new List<Cluster>();
        ///<summary> Rectangle englobant qui inclus la marge de stitching </summary>
        public Rectangle surroundingPixelRect;
        ///<summary> Rectangle de la vignette, en pixels </summary>
        public Rectangle surroundingImageRect;
        ///<summary> Cluster avec l'index le plus petit </summary>
        public Cluster firstCluster;

        //=================================================================
        // Constructeur
        //=================================================================
        public Stitch(Cluster cluster)
        {
            clusterList.Add(cluster);
            cluster.AddRef();
            firstCluster = cluster;

            surroundingPixelRect = cluster.pixelRect;
            surroundingImageRect = cluster.imageRect;
        }

        //=================================================================
        // Dispose
        //=================================================================
        protected override void Dispose(bool disposing)
        {
            foreach (Cluster cl in clusterList)
                cl.DelRef();
            clusterList.Clear();

            base.Dispose(disposing);
        }


    }
}
