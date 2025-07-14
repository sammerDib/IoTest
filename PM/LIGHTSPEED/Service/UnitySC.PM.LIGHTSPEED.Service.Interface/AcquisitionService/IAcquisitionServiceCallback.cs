using System.ServiceModel;

namespace UnitySC.PM.LIGHTSPEED.Service.Interface
{
    [ServiceContract]
    public interface IAcquisitionServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StatusChanged(int test);
    }
}
