using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.DMT.Service.Interface.Calibration
{
    [Serializable]
    [DataContract]
    public class ExposureMatchingInputs
    {
        private Dictionary<Side, ExposureMatchingGoldenValues> _goldenValuesBySide;

        [XmlElement(ElementName = "GoldenValues")]
        [DataMember]
        public List<ExposureMatchingGoldenValues> GoldenValues { get; set; }

        [XmlIgnore]
        public Dictionary<Side, ExposureMatchingGoldenValues> GoldenValuesBySide
        {
            get
            {
                if ((_goldenValuesBySide.IsNullOrEmpty() && !GoldenValues.IsNullOrEmpty()) ||
                    _goldenValuesBySide.Count != GoldenValues.Count)
                {
                    _goldenValuesBySide = GoldenValues.ToDictionary(goldenValues => goldenValues.Side,
                        goldenValues => goldenValues);
                }

                return _goldenValuesBySide;
            }
        }

        [DataMember]
        // Camera exposure time in seconds
        // TODO THP: Refactor using Timespans instead of doubles
        public double AcquisitionExposureTimeMs { get; set; }
    }
}
