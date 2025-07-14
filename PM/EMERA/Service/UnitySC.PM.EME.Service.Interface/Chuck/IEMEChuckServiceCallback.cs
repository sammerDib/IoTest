using System.ServiceModel;

using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;

namespace UnitySC.PM.EME.Service.Interface.Chuck
{
    [ServiceContract]
    public interface IEMEChuckServiceCallback : IUSPChuckServiceCallback
    {
    }
}
