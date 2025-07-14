using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.API.Service.Interface
{
    [ServiceContract(CallbackContract = typeof(IUTODFServiceCB))]
    public interface IUTODFService :  IAlarmService, 
                                    ICommonEventService,
                                    IEquipmentConstantService,                                    
                                    IRecipeDFService,
                                    IStatusVariableService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        void Init();
    }
}
