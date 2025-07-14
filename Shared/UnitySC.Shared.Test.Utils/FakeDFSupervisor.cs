using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.Test.Tools
{
    public class FakeDFSupervisor : IPMDFService, IPMDFServiceCB
    {
        public void AbortRecipeExecution(Shared.TC.Shared.Data.Identity identity)
        {
            throw new NotImplementedException();
        }

        public void AreYouThere()
        {
            throw new NotImplementedException();
        }

        public bool AskAreYouThere()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> NotifyDataCollectionChanged(Shared.TC.Shared.Data.Identity identity, ModuleDataCollection dataCollection)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> NotifyError(Shared.TC.Shared.Data.Identity identity, Message errorMessage)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> NotifyFDCCollectionChanged(Shared.TC.Shared.Data.Identity identity, List<FDCData> dataCollection)
        {
            throw new NotImplementedException();
        }

        public void OnErrorAcknowledged(Shared.TC.Shared.Data.Identity identity, ErrorID error)
        {
            throw new NotImplementedException();
        }

        public void OnErrorReset(Shared.TC.Shared.Data.Identity identity, ErrorID error)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> RecipeAcquisitionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> RecipeExecutionComplete(Shared.TC.Shared.Data.Identity identity, Material material, Guid? recipeKey, string results, Dataflow.Shared.RecipeTerminationState status)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> RecipeStarted(Shared.TC.Shared.Data.Identity identity, Material material)
        {
            throw new NotImplementedException();
        }

        public void RequestAllFDCsUpdate()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SendAda(Shared.TC.Shared.Data.Identity identity, Material material, string adaContent, string adafullPathFileName)
        {
            throw new NotImplementedException();
        }

        public void SetPMInCriticalErrorState(Shared.TC.Shared.Data.Identity identity, ErrorID error)
        {
            throw new NotImplementedException();
        }

        public void StartRecipeExecution(Shared.TC.Shared.Data.Identity identity, Guid? recipeKey, DataAccess.Dto.DataflowRecipeInfo dfRecipeInfo, Material material)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> StartRecipeRequest(Shared.TC.Shared.Data.Identity identity, Material material)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            throw new NotImplementedException();
        }
    }
}
