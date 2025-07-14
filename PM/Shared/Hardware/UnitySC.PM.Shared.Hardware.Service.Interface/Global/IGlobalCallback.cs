using System.Collections.Generic;
using System.ServiceModel;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Global
{
    [ServiceContract]
    public interface IGlobalCallback
    {
        [OperationContract(IsOneWay = true)]
        void StatusChanged(List<GlobalDevice> devices);
    }
}