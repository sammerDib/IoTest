using System.ServiceModel;

using UnitySC.PM.AGS.Data;
using UnitySC.PM.AGS.Service.Interface.Flow;
using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Data;
using UnitySC.PM.Shared.Data.ProcessingImage;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Interface.AcquisitionService
{
    public delegate void ReportProgressEventHandler(object sender, RecipeStatus recipeStatus);

    [ServiceContract(Namespace = "", CallbackContract = typeof(IAcquisitionServiceCallback))]
    public interface IAcquisitionService
    {
        /// <summary>
        /// Event to indicate Recipe Progress changed
        /// </summary>
        event ReportProgressEventHandler Progress;

        [OperationContract]
        Response<VoidResult> Subscribe();

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<USPImageMil> GetSingleImage(string camera);

        [OperationContract]
        Response<PmWaferInfo> GetDefaultWaferInfo();

        [OperationContract]
        Response<AcquisitionResult> StartAcquisition(ArgosRecipe recipe, PmWaferInfo waferInfo, bool overwriteOutput);

        [OperationContract]
        Response<ArgosRecipe> StartAutoSetting(string screenId);

        [OperationContract]
        Response<VoidResult> AbortAcquisition();
    }
}
