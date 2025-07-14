using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Interface
{
    [ServiceContract]

    public interface IDAP
    {
        [OperationContract]
        Response<Guid> GetWriteToken();

        [OperationContract]
        Response<List<Guid>> GetReadToken(Guid dapWriteToken, int count);


        [OperationContract]
        Response<bool> SendData(Guid dapWriteToken, DAPData data);

        [OperationContract]
        Response<DAPData> GetData(Guid dapReadToken);

    }
}
