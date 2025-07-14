using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.PM.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IUTOPMServiceCB))]
    public interface IUTOPMService : IAlarmService, ICommonEventService,
                                    IEquipmentConstantService,
                                    IMaterialService,
                                    IStatusVariableService
    {

        [OperationContract]
        Response<VoidResult> SubscribeToChanges();


        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        int GetNbClientsConnected();

        [OperationContract]
        Response<bool> AreYouThere();


    }
}
