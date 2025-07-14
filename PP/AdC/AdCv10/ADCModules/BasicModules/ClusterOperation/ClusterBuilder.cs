using AdcBasicObjects;

using ADCEngine;

namespace BasicModules
{
    ///////////////////////////////////////////////////////////////////////
    // Element de la queue de MilClusterizer
    // C'est un cluster incomplet (sans vignette), avec les infos 
    // nécessaires pour le construire
    ///////////////////////////////////////////////////////////////////////
    public class ClusterBuilder : ObjectBase
    {
        public Cluster cluster;
        public ImageBase image;

        public ClusterBuilder(Cluster cluster, ImageBase image)
        {
            this.cluster = cluster;
            cluster.AddRef();

            this.image = image;
            image.AddRef();
        }

        protected override void Dispose(bool disposing)
        {
            if (cluster != null)
            {
                cluster.DelRef();
                cluster = null;
            }

            if (image != null)
            {
                image.DelRef();
                image = null;
            }

            base.Dispose(disposing);
        }
    }

}
