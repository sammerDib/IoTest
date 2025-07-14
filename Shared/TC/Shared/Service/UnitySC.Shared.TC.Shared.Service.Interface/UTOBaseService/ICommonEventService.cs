using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.Shared.Service.Interface
{
    // UTO Client -> PM (UTO.API) Serveur
    [ServiceContract]
    public interface ICommonEventService
    {
        [OperationContract]
        Response<List<CommonEvent>> GetAll(); // A la connexion
    }
}
