using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    // UTO Client -> PM (UTO.API) Serveur
    [ServiceContract]
    public interface IStatusVariableService
    {
        [OperationContract]
        Response<List<StatusVariable>> SVGetAllRequest();

        [OperationContract]
        Response<List<StatusVariable>> SVGetRequest(List<int> id);


        [OperationContract]
        Response<VoidResult> RequestAllFDCsUpdate();
    }
}
