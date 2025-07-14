using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Flow.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.Calibration
{
    [DataContract]
    public class ObjectiveCalibrationResult : IFlowResult
    {
        public ObjectiveCalibrationResult()
        {
            AutoFocus = new AutofocusParameters();
            AutoFocus.Lise = new LiseAutofocusParameters();
            Image = new ImageParameters();
        }

        [DataMember]
        public FlowStatus Status { get; set; }

        [DataMember]
        public string ObjectiveId { get; set; }

        [DataMember]
        public double Magnification { get; set; }

        [DataMember]
        public AutofocusParameters AutoFocus { get; set; }

        [DataMember]
        public ImageParameters Image { get; set; }

        [DataMember]
        public Length OpticalReferenceElevationFromStandardWafer { get; set; }
    }
}
