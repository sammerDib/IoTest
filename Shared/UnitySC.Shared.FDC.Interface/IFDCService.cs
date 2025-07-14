using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.FDC.Interface
{
    [ServiceContract(CallbackContract = typeof(IFDCServiceCallback))]
    public interface IFDCService
    {

        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<List<FDCItemConfig>> GetFDCsConfig();

        [OperationContract]
        Response<VoidResult> SetFDCsConfig(List<FDCItemConfig> fdcItemsConfig);

        [OperationContract]
        Response<VoidResult> ResetFDC(string fdcName);

        [OperationContract]
        Response<VoidResult> SetInitialCountdownValue(string fdcName, double initialCountdownValue);

        [OperationContract]
        Response<FDCData> GetFDC(string fdcName);

    }
}
