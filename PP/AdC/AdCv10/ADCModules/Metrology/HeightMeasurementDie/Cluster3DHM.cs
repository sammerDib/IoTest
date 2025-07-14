using AdcBasicObjects;

using ADCEngine;

namespace HeightMeasurementDieModule
{

    public class Cluster3DHM : Cluster
    {
        override public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                Name = "CLU3Dhm-" + _index;
            }
        }

        public Cluster3DHM(int index, LayerBase layer)
            : base(index, layer)
        {

        }
    }


    public class Cluster3DHMBuilder : ObjectBase
    {
        public Cluster3DHM cluster;
        public Cluster3DDieHM dieclusterimage;

        public Cluster3DHMBuilder(Cluster3DHM p_cluster, Cluster3DDieHM p_dieclusterimage)
        {
            cluster = p_cluster;
            cluster.AddRef();

            dieclusterimage = p_dieclusterimage;
            dieclusterimage.AddRef();
        }

        protected override void Dispose(bool disposing)
        {
            if (cluster != null)
            {
                cluster.DelRef();
                cluster = null;
            }

            if (dieclusterimage != null)
            {
                dieclusterimage.DelRef();
                dieclusterimage = null;
            }

            base.Dispose(disposing);
        }
    }
}
