using System.ServiceModel;

using UnitySC.PM.DMT.Service.Interface.Recipe;

namespace UnitySC.PM.DMT.Service.Interface
{
    public interface IDMTAlgorithmsServiceCallback
    {

        [OperationContract(IsOneWay = true)]
        void ReportProgress(RecipeStatus status);
    }
}
