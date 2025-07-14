using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract]
    public interface IRotatorsKitCalibrationServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PowermeterPowerChangedCallback(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor);

        [OperationContract(IsOneWay = true)]
        void PowermeterCurrentChangedCallback(PowerIlluminationFlow flow, double current_mA);

        [OperationContract(IsOneWay = true)]
        void PowermeterIdentifierChangedCallback(PowerIlluminationFlow flow, string value);        

        [OperationContract(IsOneWay = true)]
        void PowermeterSensorTypeChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract(IsOneWay = true)]
        void PowermeterSensorAttenuationChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void PowermeterWavelengthChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void PowermeterRangeChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract(IsOneWay = true)]
        void PowermeterBeamDiameterChangedCallback(PowerIlluminationFlow flow, uint value);

        [OperationContract(IsOneWay = true)]
        void PowermeterDarkAdjustStateChangedCallback(PowerIlluminationFlow flow, string value);

        [OperationContract(IsOneWay = true)]
        void PowermeterDarkOffsetChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void PowermeterResponsivityChangedCallback(PowerIlluminationFlow flow, double value);

        [OperationContract(IsOneWay = true)]
        void PowermeterRFactorsCalibChangedCallback(PowerIlluminationFlow flow, double rfactorS, double rfactorP);
        

        [OperationContract(IsOneWay = true)]
        void PowerLaserChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void InterlockStatusChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void LaserTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void PsuTemperatureChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void AttenuationPositionChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void PolarisationPositionChangedCallback(double value);

        [OperationContract(IsOneWay = true)]
        void PolarisationCalibConfigChangedCallback(double polarAngleHsS, double polarAngleHsP, double polarAngleHtS, double polarAngleHtP);

        [OperationContract(IsOneWay = true)]
        void ShutterIrisPositionChangedCallback(string value);

        [OperationContract(IsOneWay = true)]
        void MppcStateSignalsChangedCallback(MppcCollector collector, MppcStateModule value);

        [OperationContract(IsOneWay = true)]
        void MppcOutputVoltageChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void MppcOutputCurrentChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void MppcSensorTemperatureChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void UpdateDataAttributesCallback(List<DataAttribute> dataAttributes);
    }
}
