using System.ServiceModel;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract]
    public interface IFeedbackLoopServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PowerChangedCallback(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor);

        [OperationContract(IsOneWay = true)]
        void WavelengthChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void BeamDiameterChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void PowerLaserChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void InterlockStatusChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void LaserTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void PsuTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void ShutterIrisPositionChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void AttenuationPositionChangedCallback(double value);
    }
}
