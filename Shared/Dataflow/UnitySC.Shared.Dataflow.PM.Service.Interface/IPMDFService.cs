using System;
using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Dataflow.PM.Service.Interface
{
    // PM Client -> DF Serveur
    [ServiceContract(CallbackContract = typeof(IPMDFServiceCB))]
    [ServiceKnownType(typeof(ANADataCollection))]
    public interface IPMDFService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToChanges();

        [OperationContract]
        Response<VoidResult> UnSubscribeToChanges();

        [OperationContract]
        Response<VoidResult> StartRecipeRequest(Identity identity, Material material);

        [OperationContract]
        Response<VoidResult> RecipeStarted(Identity identity, Material material);

        [OperationContract]
        Response<VoidResult> RecipeExecutionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState status);

        [OperationContract]
        Response<VoidResult> RecipeAcquisitionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState status);

        [OperationContract]
        Response<VoidResult> SendAda(Identity identity, Material material, string adaContent, String adaFullPathFileName);

        [OperationContract]
        Response<VoidResult> NotifyError(Identity identity, Message errorMessage);

        [OperationContract]
        Response<VoidResult> NotifyDataCollectionChanged(Identity identity, ModuleDataCollection dataCollection);
        [OperationContract]
        Response<VoidResult> NotifyFDCCollectionChanged(Identity identity, List<FDCData> dataCollection);
    }
}
