using System.Collections.Generic;
using System.Runtime.Serialization;

using UnitySC.Shared.Image;

namespace UnitySC.PM.DMT.Service.Interface
{

    public enum DeadPixelsCalibrationStatus
    {
        Success,
        Failure
    }


    [DataContract]
    public class ServiceImageWithDeadPixels
    {

        [DataMember]
        public int NumberOfDeadPixelsFound;

        [DataMember]
        public int MaximumDeadPixelThreshold;

        [DataMember]
        public DeadPixelsCalibrationStatus CalibrationStatus;

        [DataMember]
        public DeadPixelTypes DeadPixelType;

        /// <summary>
        /// Liste des pixels morts
        /// </summary>
        [DataMember]
        public List<DeadPixel> DeadPixels;

        /// <summary>
        /// Image
        /// </summary>
        [DataMember]
        public ServiceImage Image;


    }
}
