using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using ADCEngine;

using AdcTools;

using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;


namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Cluster, 
    /// NB: Les blobs ne dérivent pas d'ObjectBase, ils sont toujours 
    /// attachés à un Cluster.
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public class Cluster : ImageBase
    {
        public static readonly string DefectClassNone = "none";

        /// <summary>
        /// La position du cluster dans l'image wafer globale.
        /// NB: C'est different du imageRect car la vignette a été agrandie.
        /// </summary>
        [Category("Cluster"), Browsable(true), ExpandableObject]
        public Rectangle pixelRect { get; set; }

        /// <summary>
        /// La position du cluster dans le Wafer, c'est-à-dire le pixelRect en coordonnées microns
        /// </summary>
        [Category("Cluster"), Browsable(true), ExpandableObject]
        public QuadF micronQuad { get; set; }

        [Category("Cluster"), Browsable(true)]
        public bool IsDummy { get; set; } = false;

        //=================================================================
        // Proprietes
        //=================================================================
        public List<Blob> blobList = new List<Blob>();

        protected int _index;
        virtual public int Index
        {
            get { return _index; }
            set
            {
                _index = value;
                Name = "CLU-" + _index;
            }
        }

        public CustomExceptionDictionary<Characteristic, object> characteristics = new CustomExceptionDictionary<Characteristic, object>();

        /// <summary>
        /// Liste des classes défauts auquelles appartient le cluster.
        /// Attention, l'ordre est important.
        /// </summary>
        public List<string> defectClassList = new List<string>();

        /// <summary>
        /// La première classe de défaut du cluster.
        /// </summary>
        public string DefectClass
        {
            get
            {
                if (defectClassList.IsEmpty())
                    return DefectClassNone;
                else
                    return defectClassList[0];
            }
        }

        // Pour les images Die
        [Category("Die"), Browsable(true)]
        public int DieIndexX { get; set; }

        [Category("Die"), Browsable(true)]
        public int DieIndexY { get; set; }

        [Category("Die"), Browsable(true), ExpandableObject]
        public Point DieOffsetImage { get; set; }

        // Pour les images Mosaic
        [Category("Mosaic"), Browsable(true)]
        public int Line { get; set; }
        [Category("Mosaic"), Browsable(true)]
        public int Column { get; set; }

        //=================================================================
        // Constructeur
        //=================================================================
        public Cluster(int index, LayerBase layer)
            : base(layer)
        {
            Layer = layer;
            Index = index;
        }

        //=================================================================
        // Proprietées Browsables
        //=================================================================
        [Category("Characteristics"), Browsable(true), ExpandableObject]
        public DictionaryPropertyGridAdapter Characteristics { get { return new DictionaryPropertyGridAdapter(characteristics); } }
    }
}
