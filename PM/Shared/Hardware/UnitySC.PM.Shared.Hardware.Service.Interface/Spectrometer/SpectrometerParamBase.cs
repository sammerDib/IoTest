using System.Runtime.Serialization;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer
{
    [DataContract]
    public class SpectrometerParamBase
    {
        [DataMember]
        public double IntegrationTime_ms = 0;

        [DataMember]
        public int NbAverage = 1;

        public SpectrometerParamBase()
        { }

        public SpectrometerParamBase(double integratintime_ms, int nbAverage)
        {
            IntegrationTime_ms = integratintime_ms;
            NbAverage = nbAverage;
        }
    }
}
