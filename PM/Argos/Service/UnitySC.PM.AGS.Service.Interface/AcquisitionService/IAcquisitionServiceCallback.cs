using System.ServiceModel;

namespace UnitySC.PM.AGS.Service.Interface.AcquisitionService
{
    [ServiceContract]
    public interface IAcquisitionServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void RecipeProgress(RecipeStatus status);

        [OperationContract(IsOneWay = true)]
        void ResultGenerated(string name, string side, string path);
    }
}
