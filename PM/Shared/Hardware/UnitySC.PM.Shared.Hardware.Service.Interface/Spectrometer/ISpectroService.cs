using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer
{
    [ServiceContract(CallbackContract = typeof(ISpectroServiceCallback))]
    public interface ISpectroService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<SpectroSignal> DoMeasure(SpectrometerParamBase param);

        [OperationContract]
        Response<VoidResult> StartContinuousAcquisition(SpectrometerParamBase param);

        [OperationContract]
        Response<VoidResult> StopContinuousAcquisition();
    }
}
