using System;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Proxy.Axes
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class AxesSupervisor : IAxesService, IAxesServiceCallback
    {
        private readonly ILogger _logger;

        private readonly DuplexServiceInvoker<IAxesService> _axesService;

        private AxesVM _axesVM;

        private ChuckSupervisor _chuckSupervisor;
        private readonly IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public AxesSupervisor(ILogger<AxesSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            var instanceContext = new InstanceContext(this);
            // Probe service
            _axesService = new DuplexServiceInvoker<IAxesService>(instanceContext, "ANALYSEAxesService", ClassLocator.Default.GetInstance<SerilogLogger<IAxesService>>(), messenger, s => s.SubscribeToAxesChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.ANALYSE));
            _logger = logger;
            _dialogService = dialogService;
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();

            _logger.Debug($"AxesSupervisor AxeVM init Started");
            var axesvm = new AxesVM(this, _dialogService, _logger);
            axesvm.Init();
            _axesVM = axesvm; 
            _logger.Debug($"AxesSupervisor AxeVM initialized");
        }

        public Response<bool> GotoPosition(PositionBase position, AxisSpeed speed)
        {
            if (AxesVM == null)
                return new Response<bool>() { Result = false };
            if (AxesVM.IsSpeedLimitedWhenUnclamped && !_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                speed = AxisSpeed.Slow;
            return _axesService.TryInvokeAndGetMessages(s => s.GotoPosition(position, speed));
        }

        public Response<bool> GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GotoPointCustomSpeedAccel(moveX, moveY, moveZTop, moveZBottom));
        }

        public Response<bool> MoveIncremental(IncrementalMoveBase move, AxisSpeed speed)
        {
            if (AxesVM == null)
                return new Response<bool>() { Result = false };
            if (AxesVM.IsSpeedLimitedWhenUnclamped && !_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                speed = AxisSpeed.Slow;
            return _axesService.TryInvokeAndGetMessages(s => s.MoveIncremental(move, speed));
        }

        public Response<bool> StopAllMoves()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.StopAllMoves());
        }

        public Response<bool> WaitMotionEnd(int timeout, bool waitStabilization = true)
        {
            return _axesService.TryInvokeAndGetMessages(s => s.WaitMotionEnd(timeout, waitStabilization));
        }

        public Response<PositionBase> GetCurrentPosition()
        {
            var curPosition = _axesService.TryInvokeAndGetMessages(s => s.GetCurrentPosition());
            // We update the position
            if (AxesVM != null && curPosition.Result is AnaPosition position)
            {
                var positionOnWafer = ServiceLocator.ReferentialSupervisor.ConvertTo(position, ReferentialTag.Wafer)?.Result;
                if (positionOnWafer != null)
                {
                    AxesVM.UpdatePosition(positionOnWafer as AnaPosition);
                }
            }
            return curPosition;
        }

        public Response<AxesState> GetCurrentState()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GetCurrentState());
        }

        public Response<bool> StopLanding()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.StopLanding());
        }

        public Response<bool> Land()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.Land());
        }

        public AxesVM AxesVM => _axesVM;

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _axesService.TryInvokeAndGetMessages(s => s.SubscribeToAxesChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Axes subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnsubscribeToAxesChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _axesService.TryInvokeAndGetMessages(s => s.UnsubscribeToAxesChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Axes unsubscribe error");
            }

            return resp;
        }

        public void PositionChangedCallback(PositionBase newPosition)
        {
            if (AxesVM == null)
                return;

            if (newPosition is AnaPosition position)
            {
                AxesVM.UpdatePosition(position);
            }
            else
            {
                _logger.Warning($"Received position is not a AnaPosition");
            }
        }

        public void StateChangedCallback(AxesState state)
        {
            if (AxesVM == null)
                return;
            AxesVM.UpdateStatus(state);
        }

        // This function is virtual for the unit test. Do not remove it
        public virtual void EndMoveCallback(bool targetReached)
        {
            _logger?.Verbose($"EndMoveCallback - Target reached: {targetReached}");
        }

        public Response<AxesConfig> GetAxesConfiguration()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GetAxesConfiguration());
        }

        public Response<bool> GotoSpecificPosition(SpecificPositions positionRefId, Length waferDiameter, AxisSpeed speed)
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GotoSpecificPosition(positionRefId, waferDiameter, speed));
        }

        public Response<bool> GoToHome(AxisSpeed speed)
        {
            if (AxesVM == null)
                return new Response<bool>() { Result = false };
            if (AxesVM.IsSpeedLimitedWhenUnclamped && !_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                speed = AxisSpeed.Slow;
            return _axesService.TryInvokeAndGetMessages(s => s.GoToHome(speed));
        }

        public Response<bool> GoToPark(Length waferDiameter, AxisSpeed speed)
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GoToPark(waferDiameter, speed));
        }

        public Response<bool> GoToChuckCenter(Length waferDiameter, AxisSpeed speed)
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GoToChuckCenter(waferDiameter, speed));
        }

        public Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed)
        {
            if (AxesVM == null)
                return new Response<bool>() { Result = false };
            if (AxesVM.IsSpeedLimitedWhenUnclamped && !_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                speed = AxisSpeed.Slow;
            return _axesService.TryInvokeAndGetMessages(s => s.GoToManualLoad(waferDiameter, speed));
        }

        public Response<PositionBase> GetChuckCenterPosition(Length waferDiameter)
        {
            return _axesService.TryInvokeAndGetMessages(s => s.GetChuckCenterPosition(waferDiameter));
        }

        public Response<int> GetAxesPosition()
        {
            throw new NotImplementedException();
        }

        public Response<int> GetChuckPosition()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> DisconnectAxes()
        {
            throw new NotImplementedException();
        }

        public XYZTopZBottomPosition GetXYZTopZBottomPosition()
        {
            return GetCurrentPosition()?.Result as XYZTopZBottomPosition;
        }

        public Response<bool> ResetAxis()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.ResetAxis());
        }

        public Response<bool> AcknowledgeResetAxis()
        {
            return _axesService.TryInvokeAndGetMessages(s => s.AcknowledgeResetAxis());
        }

        public async Task<Response<VoidResult>> ResetZTopFocus()
        {
            var task = _axesService.TryInvokeAndGetMessagesAsync(s => s.ResetZTopFocus());
            await task;
            return task.Result;
        }

        public async Task<Response<VoidResult>> ResetZBottomFocus()
        {
            var task = _axesService.TryInvokeAndGetMessagesAsync(s => s.ResetZBottomFocus());
            await task;
            return task.Result;
        }
    }
}
