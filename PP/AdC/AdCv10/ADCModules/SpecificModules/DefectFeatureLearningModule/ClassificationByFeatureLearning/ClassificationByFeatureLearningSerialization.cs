using System.Collections.Generic;
using System.Xml.Serialization;

using AdcBasicObjects;

namespace DefectFeatureLearning.ClassificationByFeatureLearning
{
    ///////////////////////////////////////////////////////////////////////
    // Classes sérialisables pour lire le fichier de classif
    ///////////////////////////////////////////////////////////////////////
    public class Feature
    {
        [XmlAttribute] public string Name;
        [XmlAttribute] public string Layer;
        [XmlAttribute] public double LowerBound = double.NegativeInfinity;
        [XmlAttribute] public double UpperBound = double.PositiveInfinity;
        [XmlIgnore] public Characteristic Characteristic;
    }

    public class Box
    {
        [XmlAttribute] public int Id;
        [XmlAttribute] public double Probability;
        public List<Feature> Features;
    }

    public class Defect
    {
        [XmlAttribute] public string ClassName;
        public List<Box> Boxes;
    }

    public class Classification
    {
        public List<Defect> Defects;
    }
}
