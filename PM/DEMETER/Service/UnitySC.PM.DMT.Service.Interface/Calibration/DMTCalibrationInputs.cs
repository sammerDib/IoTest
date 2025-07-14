using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    public class DMTCalibrationInputs
    {

        private Dictionary<Side, ExposureMatchingInputs> _exposureMatchingInputsBySide;
        
        private Dictionary<Side, EllipseMaskInput> _ellipseMaskInputsBySide;
        
        [XmlElement("ExposureMatchingInputs")]
        public ExposureMatchingInputs ExposureMatchingInputs { get ; set; }

        public BlackDeadPixelExposureInputs BlackDeadPixelExposureInputs { get; set; }

        [XmlElement("EllipseMask")]
        public List<EllipseMaskInput> EllipseMaskInputs { get; set; }

        [XmlIgnore]
        public Dictionary<Side, EllipseMaskInput> EllipseMaskInputsBySide
        {
            get
            {
                if (_ellipseMaskInputsBySide.IsNullOrEmpty() && !EllipseMaskInputs.IsNullOrEmpty())
                {
                    _ellipseMaskInputsBySide = EllipseMaskInputs.ToDictionary(k => k.Side, v => v);
                }
                return _ellipseMaskInputsBySide;
            }
        }

        public int AlignmentScreenVerticalLineThicknessInPixels { get; set; }

        public GlobalTopoCalibrationWaferDefinition GlobalTopoCalibrationWaferDefinition { get; set; }

        public FresnelCoefficients FresnelCoefficients { get; set; }
    }
}
