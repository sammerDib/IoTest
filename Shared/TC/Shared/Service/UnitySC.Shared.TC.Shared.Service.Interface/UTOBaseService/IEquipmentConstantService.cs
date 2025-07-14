using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    [ServiceContract]
    public interface IEquipmentConstantService
    {
        // Callback pour se faire appeler
        [OperationContract]
        Response<List<EquipmentConstant>> ECGetAllRequest(); // a la connexion

        [OperationContract]
        Response<List<EquipmentConstant>> ECGetRequest(List<int> id);

        [OperationContract]
        Response<bool> ECSetRequest(EquipmentConstant ecid);
    }
}
