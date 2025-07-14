using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace AdcRobotExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: Classe de base des VIDs
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    [KnownType(typeof(VidEdge))]
    [KnownType(typeof(VidBowWarpMeasure))]
    [KnownType(typeof(VidMeasure2D))]
    [KnownType(typeof(VidMeasure3D))]
    [KnownType(typeof(VidDefect))]
    [KnownType(typeof(VidApc))]
    [KnownType(typeof(VidHaze))]
    [KnownType(typeof(VidBF2DDieCollection))]
    [KnownType(typeof(VidBF3DDieCollection))]
    [XmlInclude(typeof(VidEdge))]
    [XmlInclude(typeof(VidBowWarpMeasure))]
    [XmlInclude(typeof(VidMeasure2D))]
    [XmlInclude(typeof(VidMeasure3D))]
    [XmlInclude(typeof(VidDefect))]
    [XmlInclude(typeof(VidApc))]
    [XmlInclude(typeof(VidHaze))]
    [XmlInclude(typeof(VidBF2DDieCollection))]
    [XmlInclude(typeof(VidBF3DDieCollection))]

    public class VidBase
    {
        [DataMember] public int VidNumber;
        [DataMember] public string VidLabel;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID contenant une mesure Edge
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidEdge : VidBase
    {
        [DataMember] public string Name;
        [DataMember] public double Measure;
        [DataMember] public string UnitValue;
        [DataMember] public int Xcoordinate;
        [DataMember] public int Ycoordinate;
        [DataMember] public int RadiusValue;
        [DataMember] public int MeasureNumber;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID contenant une mesure 2DMetro
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidMeasure2D : VidBase
    {
        [DataMember] public double Measure;
        [DataMember] public string UnitValue;
    }
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID contenant une mesure 3DMetro
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidMeasure3D : VidBase
    {
        [DataMember] public double Measure;
        [DataMember] public string UnitValue;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID contenant une mesure Bow/Warp
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidBowWarpMeasure : VidBase
    {
        [DataMember] public double Measure;
        [DataMember] public string UnitValue;
        [DataMember] public int Xcoordinate;
        [DataMember] public int Ycoordinate;
        [DataMember] public int MeasureNumber;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID décrivant des défauts
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidDefect : VidBase
    {
        [DataMember] public double[] DefectSizePerBin = new double[4];
        [DataMember] public double[] DefectCountPerBin = new double[4];
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID pour l'APC
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidApc : VidBase
    {
        [DataMember] public List<VidApcModule> Modules = new List<VidApcModule>();
    }

    [DataContract]
    public class VidApcModule
    {
        [DataMember] public int ActorTypeId;
        [DataMember] public Dictionary<string, double> Dictionary;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID pour l'haze
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidHaze : VidBase
    {
        // OPI : Ceci EST LA bonne version
        [DataMember] public double Measure;
        [DataMember] public string UnitValue;
        [DataMember] public int Xcoordinate;
        [DataMember] public int Ycoordinate;
        [DataMember] public int MeasureNumber;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID pour la metro 2D
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidBF2DDieCollection : VidBase
    {
        public List<VidBF2DDieCollectionModule> lsdieCollection;
    }

    [DataContract]
    public class VidBF2DDieCollectionModule
    {
        [DataMember] public int dieCol;
        [DataMember] public int dieRow;
        [DataMember] public string status;
        [DataMember] public int totBumpFail;
        [DataMember] public int bumpFailMissing;
        [DataMember] public int bumpFailDiameter;
        [DataMember] public int bumFailOffset;
        [DataMember] public string averageDiameterStatus;
        [DataMember] public double bumpDiameterMin;
        [DataMember] public double bumpDiameterMax;
        [DataMember] public double bumpDiameterStdDv;
        [DataMember] public double bumpOffsetMin;
        [DataMember] public double bumpOffsetMax;
        [DataMember] public double bumpOffsetAverage;
        [DataMember] public double bumpOffsetStdDv;
        [DataMember] public double averageDiameter;
    }

    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID pour la metro 2D
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidBF3DDieCollection : VidBase
    {
        public List<VidBF3DDieCollectionModule> lsdieCollection;
    }
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// ADC->Robot: VID pour la metro 2D
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class VidBF3DDieCollectionModule
    {
        [DataMember] public int dieCol;
        [DataMember] public int dieRow;
        [DataMember] public string status;
        [DataMember] public int totBumpFail;
        [DataMember] public int bumpFailMissing;
        [DataMember] public int bumpFaiHeight;
        [DataMember] public string averageHeightStatus;
        [DataMember] public string coplanarityStatus;
        [DataMember] public string subtrateCoplanarityStatus;
        [DataMember] public double bumpHeightMin;
        [DataMember] public double bumpHeightMax;
        [DataMember] public double bumpHeightStdDv;
        [DataMember] public double bumpHeightMean;
        [DataMember] public double coplanarityValue;
        [DataMember] public double SubstrateCoplanarityValue;
    }
}
