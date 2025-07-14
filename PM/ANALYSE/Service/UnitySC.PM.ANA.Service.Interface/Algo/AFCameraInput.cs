using System.Runtime.Serialization;
using System.Windows;
using System.Xml.Serialization;

using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Interface;

namespace UnitySC.PM.ANA.Service.Interface.Algo
{
    [DataContract]
    [XmlInclude(typeof(ANAContextBase))]
    public class AFCameraInput : IANAInputFlow
    {
        public AFCameraInput()
        {
        }

        public AFCameraInput(string cameraId, ScanRangeType rangeType, ScanRangeWithStep scanRange = null) : this(null, cameraId, rangeType, scanRange)
        {
        }

        public AFCameraInput(ANAContextBase context, string cameraId, ScanRangeType rangeType, ScanRangeWithStep scanRange = null)
        {
            InitialContext = context;
            CameraId = cameraId;
            RangeType = rangeType;
            ScanRangeConfigured = scanRange;
        }

        public InputValidity CheckInputValidity()
        {
            var validity = new InputValidity(true);

            if (CameraId is null)
            {
                validity.IsValid = false;
                validity.Message.Add($"The camera ID is missing.");
            }

            if (RangeType == ScanRangeType.Configured)
            {
                if (ScanRangeConfigured is null)
                {
                    validity.IsValid = false;
                    validity.Message.Add($"The scan range configured must be provided when range type  is set to 'Configured'.");
                }
                else
                {
                    validity.ComposeWith(ScanRangeConfigured.CheckInputValidity());
                }
            }

            return validity;
        }

        [DataMember]
        public ANAContextBase InitialContext { get; set; }

        /// <summary>
        /// True if the scan is centered on the current z position False if the scan is centered on the values defined
        /// in configuration.
        /// </summary>
        [DataMember]
        public bool UseCurrentZPosition { get; set; }

        [DataMember]
        public ScanRangeWithStep ScanRangeConfigured { get; set; }

        [DataMember]
        public ScanRangeType RangeType { get; set; }

        [DataMember]
        public string CameraId { get; set; }

        [DataMember]
        public Rect Aoi { get; set; } = new Rect(0,0,0,0);

        [DataMember]
        public double CameraFramerate { get; set; }

        [DataMember]
        public double CameraImageExposureTime { get; set; }
    }
}
