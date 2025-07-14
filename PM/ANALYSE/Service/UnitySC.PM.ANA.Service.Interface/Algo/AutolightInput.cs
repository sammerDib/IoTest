using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class AutolightInput : IANAInputFlow
    {
        public AutolightInput()
        {
        }

        public AutolightInput(string cameraId, string lightId, double exposure, ScanRangeWithStep lightPower)
        {
            CameraId = cameraId;
            LightId = lightId;
            ExposureTimeMs = exposure;
            LightPower = lightPower;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The camera ID is missing.");
            }

            if (LightId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The light ID is missing.");
            }

            if (double.IsNaN(ExposureTimeMs))
            {
                validity.IsValid = false;
                validity.Message.Add($"The exposure time is missing.");
            }

            if (LightPower is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The light power range is missing.");
            }
            else
            {
                validity.ComposeWith(LightPower.CheckInputValidity());
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public string LightId { get; set; }

        [DataMember]
        public double ExposureTimeMs { get; set; }

        [DataMember]
        public ScanRangeWithStep LightPower { get; set; }
    }
}
