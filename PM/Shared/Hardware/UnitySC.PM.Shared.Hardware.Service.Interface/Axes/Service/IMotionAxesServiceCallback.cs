using System.ServiceModel;

using UnitySC.PM.Shared.Referentials.Interface;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [ServiceContract]
    [ServiceKnownType(typeof(XPosition))]
    [ServiceKnownType(typeof(YPosition))]
    [ServiceKnownType(typeof(TPosition))]
    [ServiceKnownType(typeof(LinearPosition))]
    [ServiceKnownType(typeof(RotationPosition))]
    [ServiceKnownType(typeof(UnknownPosition))]
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(XTPosition))]
    [ServiceKnownType(typeof(ZPiezoPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomMove))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(MotorReferential))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(StageReferential))]
    [ServiceKnownType(typeof(DieReferential))]
    public interface IMotionAxesServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void PositionChangedCallback(PositionBase position);

        [OperationContract(IsOneWay = true)]
        void StateChangedCallback(AxesState state);

        [OperationContract(IsOneWay = true)]
        void DeviceStateChangedCallback(DeviceState deviceState);

        [OperationContract(IsOneWay = true)]
        void EndMoveCallback(bool targetReached);
    }
}
