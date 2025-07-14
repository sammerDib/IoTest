using System.ServiceModel;

using UnitySC.Shared.Data.FDC;

namespace UnitySC.Shared.FDC.Interface
{
    [ServiceContract]
    public interface IFDCServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void UpdateFDCDataCallback(FDCData fdcData);
    }
}
