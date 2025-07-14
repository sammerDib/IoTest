using System.ServiceModel;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [ServiceContract]
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(XTPosition))]
    [ServiceKnownType(typeof(ZPiezoPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomMove))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(MotorReferential))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(StageReferential))]
    [ServiceKnownType(typeof(DieReferential))]
    public interface IAxesServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PositionChangedCallback(PositionBase position);

        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(AxesState state);

        [OperationContract(IsOneWay = true)]
        void EndMoveCallback(bool targetReached);
    }
}
