using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

using static UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck.SubstrateSlotConfig;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Axes
{
    [ServiceContract(CallbackContract = typeof(IAxesServiceCallback))]
    [ServiceKnownType(typeof(ACSAxisConfig))]
    [ServiceKnownType(typeof(ACSControllerConfig))]
    [ServiceKnownType(typeof(PIE709ControllerConfig))]
    [ServiceKnownType(typeof(XYPosition))]
    [ServiceKnownType(typeof(XTPosition))]
    [ServiceKnownType(typeof(XPosition))]
    [ServiceKnownType(typeof(YPosition))]
    [ServiceKnownType(typeof(ZPosition))]
    [ServiceKnownType(typeof(ZTopPosition))]
    [ServiceKnownType(typeof(ZBottomPosition))]
    [ServiceKnownType(typeof(ZPiezoPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomPosition))]
    [ServiceKnownType(typeof(AnaPosition))]
    [ServiceKnownType(typeof(XYZPosition))]
    [ServiceKnownType(typeof(XYZTopZBottomMove))]
    [ServiceKnownType(typeof(DieReferential))]
    [ServiceKnownType(typeof(MotorReferential))]
    [ServiceKnownType(typeof(WaferReferential))]
    [ServiceKnownType(typeof(StageReferential))]
    [ServiceKnownType(typeof(TMAPAxesConfig))]
    [ServiceKnownType(typeof(NSTAxesConfig))]
    [ServiceKnownType(typeof(PSDAxesConfig))]
    [ServiceKnownType(typeof(LSAxesConfig))]
    [ServiceKnownType(typeof(LiseHfAxesConfig))]
    [ServiceKnownType(typeof(PhotoLumAxesConfig))]
    [ServiceKnownType(typeof(PiezoAxisConfig))]
    [ServiceKnownType(typeof(RelianceAxisConfig))]
    [ServiceKnownType(typeof(PhytronAxisConfig))]
    [ServiceKnownType(typeof(ThorlabsSliderAxisConfig))]
    [ServiceKnownType(typeof(OwisAxisConfig))]
    [ServiceKnownType(typeof(AerotechAxisConfig))]
    [ServiceKnownType(typeof(ParallaxAxisConfig))]
    [ServiceKnownType(typeof(IoAxisConfig))]
    [ServiceKnownType(typeof(CNCAxisConfig))]
    public interface IAxesService
    {
        [OperationContract]
        Response<VoidResult> SubscribeToAxesChanges();

        [OperationContract]
        Response<VoidResult> UnsubscribeToAxesChanges();

        [OperationContract]
        Response<AxesConfig> GetAxesConfiguration();

        [OperationContract]
        Response<bool> GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom);

        [OperationContract]
        Response<bool> GotoPosition(PositionBase position, AxisSpeed speed);

        [OperationContract]
        Response<bool> MoveIncremental(IncrementalMoveBase move, AxisSpeed speed);

        [OperationContract]
        Response<bool> GotoSpecificPosition(SpecificPositions positionRefId, Length waferDiameter, AxisSpeed speed);

        [OperationContract]
        Response<bool> GoToHome(AxisSpeed speed);

        [OperationContract]
        Response<bool> GoToPark(Length waferDiameter, AxisSpeed speed);

        [OperationContract]
        Response<bool> GoToChuckCenter(Length waferDiameter, AxisSpeed speed);

        [OperationContract]
        Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed);

        [OperationContract]
        Response<bool> WaitMotionEnd(int timeout, bool waitStabilization = true);

        [OperationContract]
        Response<PositionBase> GetChuckCenterPosition(Length waferDiameter);

        [OperationContract]
        Response<PositionBase> GetCurrentPosition();

        [OperationContract]
        Response<AxesState> GetCurrentState();

        [OperationContract]
        Response<bool> StopAllMoves();

        [OperationContract]
        Response<bool> StopLanding();

        [OperationContract]
        Response<bool> Land();

        [OperationContract]
        Response<bool> ResetAxis();

        [OperationContract]
        Response<bool> AcknowledgeResetAxis();

        [OperationContract]
        Task<Response<VoidResult>> ResetZTopFocus();

        [OperationContract]
        Task<Response<VoidResult>> ResetZBottomFocus();
    }
}
