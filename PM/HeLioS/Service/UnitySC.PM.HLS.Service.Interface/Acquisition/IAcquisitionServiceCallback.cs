
using System.ServiceModel;

namespace UnitySC.PM.HLS.Service.Interface.Acquisition
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
