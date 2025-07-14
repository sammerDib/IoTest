using System.ServiceModel;

using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IUTODFServiceCB))]
    public interface IUTODFService : IAlarmService,
                                    ICommonEventService,
                                    IEquipmentConstantService,
                                    IRecipeDFService,
                                    IStatusVariableService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<bool> AreYouThere();


    }
}
