using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter
{
    [ServiceContract]
    public interface IOpticalPowermeterServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(PowerIlluminationFlow flow, string state);

        [OperationContract(IsOneWay = true)]
        void PowerChangedCallback(PowerIlluminationFlow flow, double value, double powerCal_mW, double rfactor);

        [OperationContract(IsOneWay = true)]
        void MaximumPowerChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void MinimumPowerChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void WavelengthChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void BeamDiameterChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void WavelengthRangeChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void IdentifierChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract(IsOneWay = true)]
        void CustomChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract]
        void AvailableWavelengthsCallback(List<string> wavelengths);

        [OperationContract(IsOneWay = true)]
        void CurrentChangedCallback(PowerIlluminationFlow flow, double current_mA);

        [OperationContract(IsOneWay = true)]
        void DarkAdjustStateChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract(IsOneWay = true)]
        void DarkOffsetChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void ResponsivityChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void SensorTypeChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract(IsOneWay = true)]
        void SensorAttenuationChangedCallback(PowerIlluminationFlow flow, uint value);
    }
}
