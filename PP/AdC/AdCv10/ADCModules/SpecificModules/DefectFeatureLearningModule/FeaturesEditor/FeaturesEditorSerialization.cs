using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

using AdcTools;

namespace DefectFeatureLearning.FeaturesEditor
{
    ///////////////////////////////////////////////////////////////////////
    // Classes qui sont sérialisées pour créer le fichier ASE
    ///////////////////////////////////////////////////////////////////////
    [Serializable]
    public class Feature
    {
        [XmlAttribute] public string Name;
        [XmlAttribute] public double Value;
    }

    [Serializable]
    public class Layer
    {
        [XmlAttribute] public string Name;
        [XmlAttribute] public string OriginalImage;
        [XmlAttribute] public string BinaryImage;
        [XmlAttribute] public int ClusterNumber;
        public List<Feature> Features = new List<Feature>();
    }

    [Serializable]
    public class Defect
    {
        [XmlAttribute] public string Class;
        public List<Layer> Layers = new List<Layer>();
        public RectangleF MicronRect;
    }

    [Serializable]
    public class Classification : Serializable
    {
        public WaferDataBase WaferData;
        public List<Defect> Defects = new List<Defect>();
    }

    [Serializable]
    public class WaferInfo
    {
        [XmlAttribute] public string Key;
        [XmlAttribute] public string Value;
    }

    [Serializable]
    [XmlInclude(typeof(NotchWaferData))]
    [XmlInclude(typeof(FlatWaferData))]
    [XmlInclude(typeof(RectangularWaferData))]
    public abstract class WaferDataBase
    {
        public List<WaferInfo> WaferInfos;
    }

    [Serializable]
    public class NotchWaferData : WaferDataBase
    {
        public double Diameter;
    }

    [Serializable]
    public class FlatWaferData : WaferDataBase
    {
        public double FlatVerticalX;
        public double FlatHorizontalY;
    }

    [Serializable]
    public class RectangularWaferData : WaferDataBase
    {
        public double Width;
        public double Height;
    }
}
