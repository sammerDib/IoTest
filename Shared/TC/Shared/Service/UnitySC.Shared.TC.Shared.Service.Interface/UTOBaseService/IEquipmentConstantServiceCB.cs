using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    // PM (UTO.API) Serveur -> UTO Client
    [ServiceContract]
    public interface IEquipmentConstantServiceCB
    {
        [OperationContract]
        void SetECValues(List<EquipmentConstant> equipmentConstants);
    }
}
