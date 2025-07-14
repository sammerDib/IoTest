using System.ServiceModel;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [ServiceContract(CallbackContract = typeof(IMotionAxesServiceCallback))]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(XTPosition))]
    [ServiceKnownType(typeof(XPosition))]
    [ServiceKnownType(typeof(YPosition))]
    [ServiceKnownType(typeof(ZPosition))]
    [ServiceKnownType(typeof(TPosition))]
    [ServiceKnownType(typeof(LinearPosition))]
    [ServiceKnownType(typeof(RotationPosition))]
    [ServiceKnownType(typeof(UnknownPosition))]
    [ServiceKnownType(typeof(ZTopPosition))]
    [ServiceKnownType(typeof(ZBottomPosition))]
    [ServiceKnownType(typeof(ZPiezoPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomMove))]
    [ServiceKnownType(typeof(DieReferential))]
    [ServiceKnownType(typeof(MotorReferential))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(StageReferential))]
    [ServiceKnownType(typeof(LiseHfAxesConfig))]
    [ServiceKnownType(typeof(PhotoLumAxesConfig))]
    [ServiceKnownType(typeof(ThorlabsSliderAxisConfig))]
    [ServiceKnownType(typeof(OwisAxisConfig))]
    [ServiceKnownType(typeof(AerotechAxisConfig))]
    [ServiceKnownType(typeof(ParallaxAxisConfig))]
    [ServiceKnownType(typeof(IoAxisConfig))]
    [ServiceKnownType(typeof(CNCAxisConfig))]
    public interface IMotionAxesService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToAxesChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToAxesChanges();

        [OperationContract]
        Response<AxesConfig> GetAxesConfiguration();

        [OperationContract]
        Response<bool> Move(params PMAxisMove[] moves);

        [OperationContract]
        Response<bool> RelativeMove(params PMAxisMove[] moves);

        [OperationContract]
        Response<bool> GoToHome(AxisSpeed speed);

        [OperationContract]
        Response<bool> WaitMotionEnd(int timeout);

        [OperationContract]
        Response<PositionBase> GetCurrentPosition();

        [OperationContract]
        Response<AxesState> GetCurrentState();

        [OperationContract]
        Response<bool> StopAllMotion();

        [OperationContract]
        Response<VoidResult> TriggerUpdateEvent();
    }
}
