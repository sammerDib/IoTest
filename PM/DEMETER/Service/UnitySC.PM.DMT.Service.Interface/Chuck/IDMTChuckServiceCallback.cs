using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;

namespace UnitySC.PM.DMT.Service.Interface.Chuck
{
    [ServiceContract]
    public interface IDMTChuckServiceCallback : IUSPChuckServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void TagChangedCallback(string tag);
    }
}
