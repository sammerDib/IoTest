using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    [ServiceContract]
    public interface IChamberServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateDataAttributesCallback(List<DataAttribute> dataAttributes);
    }
}