using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    /// <summary>
    /// Measure settings are global to the measure, and do not change between each measure point.
    /// They give some common characteristics of the measure (like the list of measure points,
    /// the number of times the measure should be repeated). Local settings like the current measure
    /// point where the measure is applied, or the current die, are not meant to be stored in the
    /// settings but passed in the <see cref="MeasureContext"/>.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ThicknessSettings))]
    [KnownType(typeof(TSVSettings))]
    [KnownType(typeof(NanoTopoSettings))]
    [KnownType(typeof(TopographySettings))]
    [KnownType(typeof(XYCalibrationSettings))]
    [KnownType(typeof(BowSettings))]
    [KnownType(typeof(WarpSettings))]
    [KnownType(typeof(StepSettings))]
    [KnownType(typeof(EdgeTrimSettings))]
    [KnownType(typeof(TrenchSettings))]
    [XmlInclude(typeof(ThicknessSettings))]
    [XmlInclude(typeof(TSVSettings))]
    [XmlInclude(typeof(NanoTopoSettings))]
    [XmlInclude(typeof(TopographySettings))]
    [XmlInclude(typeof(XYCalibrationSettings))]
    [XmlInclude(typeof(BowSettings))]
    [XmlInclude(typeof(WarpSettings))]
    [XmlInclude(typeof(StepSettings))]
    [XmlInclude(typeof(EdgeTrimSettings))]
    [XmlInclude(typeof(TrenchSettings))]
    public abstract class MeasureSettingsBase
    {
        [DataMember]
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlIgnore]
        public abstract MeasureType MeasureType { get; }

        [XmlIgnore]
        public virtual bool MeasureStartAtMeasurePoint => false;

        [XmlIgnore]
        public virtual bool PostProcessingSettingsIsEnabled => false;

        [XmlIgnore]
        [DataMember]
        public virtual bool IsMeasureWithSubMeasurePoints { get; set; } = false;

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsConfigured { get; set; }

        [DataMember]
        public List<int> MeasurePoints { get; set; } = new List<int>();

        [DataMember]
        public virtual List<int> SubMeasurePoints { get; set; } = new List<int>();

        [DataMember]
        public int NbOfRepeat { get; set; }

        public MeasureSettingsBase Clone()
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Seek(0, SeekOrigin.Begin);
                return (MeasureSettingsBase)formatter.Deserialize(ms);
            }
        }

    }
}
