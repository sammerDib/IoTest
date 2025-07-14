using System.ServiceModel;

using UnitySC.Shared.Data.Enum;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Mppc
{
    [ServiceContract]
    public interface IMppcServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(MppcCollector collector, string state);

        [OperationContract(IsOneWay = true)]
        void MonitorInfoStatusChangedCallback(MppcCollector collector, string value);

        [OperationContract(IsOneWay = true)]
        void OutputCurrentChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void OutputVoltageChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void OutputVoltageSettingChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void HighVoltageStatusChangedCallback(MppcCollector collector, string value);

        [OperationContract(IsOneWay = true)]
        void StateSignalsChangedCallback(MppcCollector collector, MppcStateModule value);

        [OperationContract(IsOneWay = true)]
        void TemperatureChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void SensorTemperatureChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void PowerFctReadChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void TempCorrectionFactorReadChangedCallback(MppcCollector collector, double value);

        [OperationContract(IsOneWay = true)]
        void FirmwareChangedCallback(MppcCollector collector, string value);

        [OperationContract(IsOneWay = true)]
        void IdentifierChangedCallback(MppcCollector collector, string value);
    }
}
