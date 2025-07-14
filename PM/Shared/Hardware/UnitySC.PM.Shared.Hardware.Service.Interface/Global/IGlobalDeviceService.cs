using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Global
{
    [ServiceContract(CallbackContract = typeof(IGlobalCallback))]
    public interface IGlobalDeviceService
    {
        [OperationContract]
        Response<List<GlobalDevice>> GetDevices();

        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();
    }
}