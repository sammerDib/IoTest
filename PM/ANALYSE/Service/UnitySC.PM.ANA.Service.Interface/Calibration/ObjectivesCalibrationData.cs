using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [DataContract]
    public class ObjectivesCalibrationData : ICalibrationData
    {
        public ObjectivesCalibrationData()
        {
            Calibrations = new List<ObjectiveCalibration>();
        }

        [DataMember]
        public DateTime CreationDate { get; set; }


        [DataMember]
        public List<ObjectiveCalibration> Calibrations { get; set; }

        [DataMember]
        public string User { get; set; }

        public string Information => string.Format($"Created by {User} on {CreationDate}");
    }

    [DataContract]
    public class ObjectiveCalibration
    {
        [DataMember]
        public string DeviceId { get; set; }

        [DataMember]
        public string ObjectiveName { get; set; }

        [DataMember]
        public double Magnification { get; set; }

        [DataMember]
        public AutofocusParameters AutoFocus { get; set; }

        [DataMember]
        public ImageParameters Image { get; set; }

        [DataMember]
        public Length ZOffsetWithMainObjective { get; set; }

        [DataMember]
        public Length OpticalReferenceElevationFromStandardWafer { get; set; }
    }

    [DataContract]
    public class AutofocusParameters
    {
        [DataMember]
        public LiseAutofocusParameters Lise { get; set; }

        [DataMember]
        public Length ZFocusPosition { get; set; }
    }

    [DataContract]
    public class LiseAutofocusParameters
    {
        [DataMember]
        public double MinGain { get; set; }

        [DataMember]
        public double MaxGain { get; set; }

        [DataMember]
        public Length ZStartPosition { get; set; }

        [DataMember]
        public Length AirGap { get; set; }
    }

    [DataContract]
    public class ImageParameters
    {
        [DataMember]
        public XYPosition CentricitiesRefPosition { get; set; }

        [DataMember]
        public Length PixelSizeX { get; set; }

        [DataMember]
        public Length PixelSizeY { get; set; }

        [DataMember]
        public Length XOffset { get; set; }

        [DataMember]
        public Length YOffset { get; set; }
    }
}
