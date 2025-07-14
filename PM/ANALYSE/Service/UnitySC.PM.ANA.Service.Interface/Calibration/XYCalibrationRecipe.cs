using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.Shared.Format.Metro;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class XYCalibrationRecipe
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public WaferMapResult WaferMap { get; set; }

        [DataMember]
        public XYCalibrationSettings Settings { get; set; }
    }
}
