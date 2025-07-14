using System;
using System.ServiceModel;

using UnitySC.Shared.Tools.Service;

namespace UnitySC.PP.ADC.Service.Interface
{

    [ServiceContract(CallbackContract = typeof(IADCServiceCallback))]
    public interface IADCService
    {
        [OperationContract]
        Response<VoidResult> Test();

        [OperationContract]
        Response<int> ExecuteRecipe(string recipeFileNam, string adaFile);

        [OperationContract]
        Response<VoidResult> StartPPRecipe(string actorRecipeName, string actorId, string dataFlowId);

        [OperationContract]
        Response<bool> DataAvailable(string actorID, string dataFlowId, Guid dapToken);

        [OperationContract]
        Response<bool> RecipeEnded(string actorID, string dataFlowId);

    }
}
