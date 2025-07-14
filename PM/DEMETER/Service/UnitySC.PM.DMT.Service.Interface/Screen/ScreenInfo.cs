using System.Runtime.Serialization;

namespace UnitySC.PM.DMT.Hardware.Service.Interface.Screen
{
    [DataContract]
    public class ScreenInfo
    {
        [DataMember]
        public string Model;

        [DataMember]
        public string SerialNumber;

        [DataMember]
        public string Version;

        [DataMember]
        public int Width;

        [DataMember]
        public int Height;

        [DataMember]
        public double PixelPitchHorizontal;

        [DataMember]
        public double PixelPitchVertical;

        [DataMember]
        public double ScreenWhiteDisplayTimeSec;// unit in "second"
    }
}
