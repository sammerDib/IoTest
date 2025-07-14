using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    // PM (UTO.API) Serveur -> UTO Client
    [ServiceContract]
    public interface IStatusVariableServiceCB
    {
        [OperationContract(Name = "SVSetMessage_List", IsOneWay = true)]
        void SVSetMessage(List<StatusVariable> statusVariables);
    }
}
