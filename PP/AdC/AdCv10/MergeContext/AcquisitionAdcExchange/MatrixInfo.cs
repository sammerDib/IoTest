using System;
using System.Runtime.Serialization;

namespace AcquisitionAdcExchange
{
    ///////////////////////////////////////////////////////////////////////
    /// <summary>
    /// Info sur les conditions d'acquisition
    /// </summary>
    ///////////////////////////////////////////////////////////////////////
    [DataContract]
    public class MatrixInfo
    {
        [DataMember] public double PixelWidth = Double.NaN;      // μm/pixel
        [DataMember] public double PixelHeight = Double.NaN;
        [DataMember] public double WaferCenterX = Double.NaN;   // pixels
        [DataMember] public double WaferCenterY = Double.NaN;
        [DataMember] public bool WaferPositionCorrected;
        [DataMember] public double AlignerAngleRadian = Double.NaN;
        // Pour les matrices edge:
        [DataMember] public double AcquisitionStartAngle = Double.NaN;
        [DataMember] public double SensorRadiusPosition = Double.NaN;     //μm
        [DataMember] public double WaferPositionOnChuckX = Double.NaN;    //µm
        [DataMember] public double WaferPositionOnChuckY = Double.NaN;
        [DataMember] public double SensorVerticalAngle = Double.NaN;
        [DataMember] public int NotchY = -1;          // position du Notch dans l'image, en pixels
        [DataMember] public int ChuckOriginY = -1;    // origine du chuck dans l'image, en pixels

        public double AlignerAngleDegree
        {
            get { return AlignerAngleRadian * 180 / Math.PI; }
            set { AlignerAngleRadian = value / 180 * Math.PI; }
        }

        public double AcquisitionStartAngleDegree
        {
            get { return AcquisitionStartAngle * 180 / Math.PI; }
            set { AcquisitionStartAngle = value / 180 * Math.PI; }
        }

        public double SensorVeritcalAngleDegree
        {
            get { return SensorVerticalAngle * 180 / Math.PI; }
            set { SensorVerticalAngle = value / 180 * Math.PI; }
        }

    }
}
