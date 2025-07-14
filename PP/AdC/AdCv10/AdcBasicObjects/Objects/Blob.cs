using System;
using System.Drawing;

using AdcTools;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Blob.
    /// NB: Les blobs ne dérivent pas d'ObjectBase, ils sont toujours 
    /// attachés à un Cluster.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class Blob : ICloneable
    {
        //=================================================================
        // Proprietes
        //=================================================================
        public Cluster ParentCluster { get; protected set; }
        public string Name { get; protected set; }

        /// <summary>
        /// Index unique du blob.
        /// Les algos de clusterisation se débrouille ppour créer des IDs
        /// uniques et répétables (i.e. indépendants de l'ordre de traitement des images).
        /// </summary>
        public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                Name = "BLOB-" + ParentCluster.Index + "-" + _index;
            }
        }
        private int _index;

        /// <summary> Coordonnées du blob en microns </summary>
        public QuadF micronQuad;
        /// <summary> Coordonnées du blob en pixels </summary>
        public Rectangle pixelRect;

        public int pixelArea;

        public CustomExceptionDictionary<Characteristic, object> characteristics = new CustomExceptionDictionary<Characteristic, object>();

        //=================================================================
        // 
        //=================================================================
        public Blob(int index, Cluster parentCluster)
        {
            ParentCluster = parentCluster;
            Index = index;
        }

        //=================================================================
        // 
        //=================================================================
        public override string ToString()
        {
            return Name;
        }

        //=================================================================
        // 
        //=================================================================
        public object Clone()
        {
            Blob clone = (Blob)MemberwiseClone();
            clone.characteristics = new CustomExceptionDictionary<Characteristic, object>(characteristics);
            return clone;
        }
    }
}
