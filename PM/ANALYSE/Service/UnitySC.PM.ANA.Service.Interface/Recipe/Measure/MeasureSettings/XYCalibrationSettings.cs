using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings
{
    public class XYCalibrationSettings : MeasureSettingsBase, IAutoFocusMeasureSettings
    {
        [XmlIgnore]
        public override MeasureType MeasureType => MeasureType.XYCalibration;

        [XmlIgnore]
        public override bool MeasureStartAtMeasurePoint => true;

        [DataMember]
        public Length WaferCalibrationDiameter { get; set; } = 300.Millimeters();

        [DataMember]
        public AutoFocusSettings AutoFocusSettings { get; set; }

        [DataMember]
        public PatternRecognitionData PatternRecognitionData { get; set; }

        [XmlIgnore]
        [DataMember]
        public CalibrationFlag CalibFlag { get; set; } = CalibrationFlag.Calib;
    }
}
