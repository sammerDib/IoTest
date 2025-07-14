using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Controller
{
    [ServiceContract(CallbackContract = typeof(IControllerServiceCallback))]
    public interface IControllerService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<List<string>> GetControllersIds();

        [OperationContract]
        Response<ControllerConfig> GetControllerById(string deviceId);


        [OperationContract]
        Response<bool> GetDigitalIoState(string deviceId, string ioId);

        [OperationContract]
        Response<double> GetAnalogIoValue(string deviceId, string ioId);

        [OperationContract]
        Response<VoidResult> SetDigitalIoState(string deviceId, string ioId, bool value);

        [OperationContract]
        Response<VoidResult> SetAnalogIoValue(string deviceId, string ioId, double value);

        [OperationContract]
        Response<List<IOControllerConfig>> GetControllersIOs();

        [OperationContract]
        Response<VoidResult> StartIoRefreshTask();

        [OperationContract]
        Response<VoidResult> StopIoRefreshTask();
    }
}
