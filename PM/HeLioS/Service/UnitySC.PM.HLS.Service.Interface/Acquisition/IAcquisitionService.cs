using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceModel;
using UnitySC.Shared.Tools.Service;
using UnitySC.PM.Shared.Data;
using UnitySC.PM.Shared.Hardware.Service.Interface;

using UnitySC.PM.HLS.Service.Interface.Recipe;

namespace UnitySC.PM.HLS.Service.Interface.Acquisition
{

    //public delegate void ReportProgressEventHandler(object sender, RecipeStatus recipeStatus);

    [ServiceContract(Namespace = "", CallbackContract = typeof(IAcquisitionServiceCallback))]
    public interface IAcquisitionService
    {
        /// <summary>
        /// Event to indicate Recipe Progress changed
        /// </summary>
        //event ReportProgressEventHandler Progress;

        [OperationContract]
        Response<VoidResult> Subscribe();

        [OperationContract]
        Response<VoidResult> Unsubscribe();

        [OperationContract]
        Response<SupportedFeatures> GetSupportedFeatures();

        [OperationContract]
        Response<VoidResult> StartRecipe(HLSRecipe recipe, PmWaferInfo waferInfo, bool overwriteOutput);

        [OperationContract]
        Response<VoidResult> Abort();

    }
}
