using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Format.Metro.XYCalibration;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Format.Metro.NanoTopo;
using UnitySC.Shared.Format.Metro.Thickness;
using UnitySC.Shared.Format.Metro.Topography;
using UnitySC.Shared.Format.Metro.Step;
using UnitySC.Shared.Format.Metro.EdgeTrim;
using UnitySC.Shared.Format.Metro.Trench;
using UnitySC.Shared.Format.Metro.Pillar;
using UnitySC.Shared.Format.Metro.PeriodicStruct;
using UnitySC.Shared.Format.Metro.Bow;
using UnitySC.Shared.Format.Metro.Warp;

namespace UnitySC.Shared.Format.Metro
{
    [Serializable]
    [XmlInclude(typeof(TSVPointData))]
    [KnownType(typeof(TSVPointData))]
    [XmlInclude(typeof(NanoTopoPointData))]
    [KnownType(typeof(NanoTopoPointData))]
    [XmlInclude(typeof(ThicknessPointData))]
    [KnownType(typeof(ThicknessPointData))]
    [XmlInclude(typeof(TopographyPointData))]
    [KnownType(typeof(TopographyPointData))]
    [XmlInclude(typeof(TrenchPointData))]
    [KnownType(typeof(TrenchPointData))]
    [XmlInclude(typeof(PillarPointData))]
    [KnownType(typeof(PillarPointData))]
    [XmlInclude(typeof(PeriodicStructPointData))]
    [KnownType(typeof(PeriodicStructPointData))]
    [XmlInclude(typeof(BowPointData))]
    [KnownType(typeof(BowPointData))]
    [XmlInclude(typeof(BowTotalPointData))]
    [KnownType(typeof(BowTotalPointData))]
    [XmlInclude(typeof(WarpPointData))]
    [KnownType(typeof(WarpPointData))]
    [XmlInclude(typeof(WarpTotalPointData))]
    [KnownType(typeof(WarpTotalPointData))]
    [XmlInclude(typeof(XYCalibrationPointData))]
    [KnownType(typeof(XYCalibrationPointData))]
    [XmlInclude(typeof(StepPointData))]
    [KnownType(typeof(StepPointData))]
    [XmlInclude(typeof(EdgeTrimPointData))]
    [KnownType(typeof(EdgeTrimPointData))]
    [XmlInclude(typeof(TrenchPointData))]
    [KnownType(typeof(TrenchPointData))]
    [DataContract]
    public abstract class MeasurePointDataResultBase
    {
        [XmlAttribute("Id")]
        [DataMember]
        public int IndexRepeta { get; set; }

        [XmlAttribute("State")]
        [DataMember]
        public MeasureState State { get; set; }

        [XmlIgnore]
        [DataMember]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [XmlAttribute("Message")]
        [DataMember]
        public string Message { get; set; }

        [XmlAttribute("QScore")]
        [DataMember]
        public double QualityScore { get; set; }

        public override string ToString()
        {
            return $"Id: {IndexRepeta} State: {State} QScore:{QualityScore}";
        }

        public virtual void NewIterInPath(int NewIter)
        {
            // in case of external files, override this method to handle any Iteration Number 
        }
    }

    static public class MeasurePointDataResultBaseHelper
    {
        static public void FormatNewIterPath(ref string path, int newIter)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var splits = path.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length >= 2)
                {
                    path = $"{splits[0]}\\RunIter{newIter:00}\\{splits[1]}";
                }
                else
                {
                    path = $"RunIter{newIter:00}\\" + path;
                }
            }
        }
        static public string FormatNewIterPathCopy(string path, int newIter)
        {
            String newPath = string.Copy(path);
            FormatNewIterPath(ref newPath, newIter);
            return newPath;
        }
    }
}
