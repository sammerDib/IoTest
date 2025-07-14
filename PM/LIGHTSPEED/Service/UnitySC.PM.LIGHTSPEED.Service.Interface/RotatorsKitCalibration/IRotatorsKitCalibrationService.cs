using System.ServiceModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IRotatorsKitCalibrationServiceCallback))]
    public interface IRotatorsKitCalibrationService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> PowerOn();

        [OperationContract]
        Response<VoidResult> PowerOff();

        [OperationContract]
        Response<VoidResult> SetPower(int power);

        [OperationContract]
        Response<VoidResult> SetCurrent(int current);

        [OperationContract]
        Response<VoidResult> MoveAbsPosition(BeamShaperFlow beamShaperFlow, double position);

        [OperationContract]
        Response<VoidResult> HomePosition(BeamShaperFlow beamShaperFlow);

        [OperationContract]
        Response<VoidResult> OpenShutterCommand();

        [OperationContract]
        Response<VoidResult> CloseShutterCommand();

        [OperationContract]
        Response<VoidResult> InitializeUpdate();

        [OperationContract]
        Response<VoidResult> AttenuationRefresh();

        [OperationContract]
        Response<VoidResult> MppcPowerOn(MppcCollector collector, bool powerOn);

        [OperationContract]
        Response<VoidResult> MppcSetOutputVoltage(MppcCollector collector, double voltage);

        [OperationContract]
        Response<VoidResult> MppcManageRelay(MppcCollector collector, bool relayActivated);

        [OperationContract]
        Response<VoidResult> SetIoValue(string identifier, string name, bool value);

        [OperationContract]
        Response<VoidResult> GetIoValue(string identifier, string name);

        [OperationContract]
        Response<VoidResult> PowermeterEnableAutoRange(PowerIlluminationFlow flow, bool activate);        

        [OperationContract]
        Response<VoidResult> PowermeterRangesVariation(PowerIlluminationFlow flow, string range);

        [OperationContract]
        Response<VoidResult> PowermeterStartDarkAdjust(PowerIlluminationFlow flow);

        [OperationContract]
        Response<VoidResult> PowermeterCancelDarkAdjust(PowerIlluminationFlow flow);

        [OperationContract]
        Response<VoidResult> PowermeterEditResponsivity(PowerIlluminationFlow flow, double responsivity_mA_W);

        [OperationContract]
        Response<VoidResult> PowermeterEditRFactorsCalib(PowerIlluminationFlow flow, double rfactorS, double rfactorP);        
    }
}
