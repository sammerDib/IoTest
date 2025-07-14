using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Controller
{
    [ServiceContract]
    public interface IControllerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateStatusOfIosCallback(List<DataAttribute> dataAttributes);

    }
}
