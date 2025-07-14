using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data.FDC;

namespace UnitySC.DataAccess.Service.Interface
{
    [ServiceContract]
    public interface ISendFdcServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void SendFDCs(List<FDCData> fdcsToSend);


    }
}
