

using System.ServiceModel;

namespace UnitySC.PM.ANA.Service.Interface.Positionning
{
    public interface IPositionningServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void ConversionDone(CompositePosition position);
    }
}
