using System;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Data;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Dataflow.PM.Service.Interface
{
    // DF Serveur -> PM Client
    [ServiceContract]
    public interface IPMDFServiceCB
    {
        [OperationContract(IsOneWay = true)]
        void StartRecipeExecution(Identity identity, Guid? recipeKey, DataflowRecipeInfo dfRecipeInfo, Data.Material material);

        [OperationContract(IsOneWay = true)]
        void AbortRecipeExecution(Identity identity);

        [OperationContract(IsOneWay = true)]
        void AreYouThere();

        bool AskAreYouThere();

        [OperationContract(IsOneWay = true)]
        void OnErrorAcknowledged(Identity identity, ErrorID error);
        [OperationContract(IsOneWay = true)]
        void OnErrorReset(Identity identity, ErrorID error);
        [OperationContract(IsOneWay = true)]
        void SetPMInCriticalErrorState(Identity identity, ErrorID error);
        [OperationContract(IsOneWay = true)]
        void RequestAllFDCsUpdate();
    }
}
