using System.Drawing;
using System.Xml.XPath;

using ADCEngine;

using UnitySC.Shared.LibMIL;

using UnitySC.Shared.Data.Enum;

namespace AdcBasicObjects
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Classe de base pour les Layers contenant des images
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    public abstract class ImageLayerBase : LayerBase
    {
        //public int ModuleID;
        //public int ChannelID;
        //public int ImageID;
        //public int? ChamberID;

        public ResultType ResultType;
        public int ChamberKey;
        public int ToolKey;


        public bool KeepImageData;

        /// <summary> Ajoute une image dans le stockage de la layer </summary>
        public abstract void AddImage(ImageBase image);
        /// <summary> Copy une zone de la layer dans l'image de destination </summary>
        public abstract void CopyImageDataTo(MilImage destImage, Rectangle pixelRect);
        ///<summary> Libération des images </summary>
        public abstract void FreeImages();
    }
}
