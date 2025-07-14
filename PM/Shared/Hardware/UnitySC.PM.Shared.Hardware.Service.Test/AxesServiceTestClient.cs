using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.Service.Test
{
    /// <summary>
    /// Implementation of a WCF client for the IStageService server testing
    ///
    /// Override needed methods in tests
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class AxesServiceTestClient : IAxesService, IAxesServiceCallback
    {
        protected DuplexServiceInvoker<IAxesService> _remoteService;
        private readonly ILogger<IAxesService> _logger;
        private readonly IMessenger _messenger;

        public AxesServiceTestClient()
        {
            var instanceContext = new InstanceContext(this);
            _logger = new SerilogLogger<IAxesService>();
            _messenger = new WeakReferenceMessenger();
            _remoteService = new DuplexServiceInvoker<IAxesService>(
                instanceContext,
                "AxesService",
                _logger,
                _messenger);
        }

        public Response<bool> ClampWafer(List<string> valvesToUseList)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> DisconnectAxes()
        {
            throw new NotImplementedException();
        }

        public void EndMoveCallback(bool targetReached)
        {
            throw new NotImplementedException();
        }

        public Response<int> GetChuckPosition()
        {
            throw new NotImplementedException();
        }

        public Response<PositionBase> GetCurrentPosition()
        {
            throw new NotImplementedException();
        }

        public Response<int> GetAxesPosition()
        {
            throw new NotImplementedException();
        }

        public Response<bool> GoToHome(AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<bool> GoToPark(Length waferDiameter, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<bool> GoToChuckCenter(Length waferDiameter, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<PositionBase> GetChuckCenterPosition(Length waferDiameter)
        {
            throw new NotImplementedException();
        }

        public Response<bool> GotoPosition(PositionBase position, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<bool> Land()
        {
            throw new NotImplementedException();
        }

        public void PositionChangedCallback(PositionBase position)
        {
            throw new NotImplementedException();
        }

        public Response<bool> ReleaseWafer(List<string> valvesToUseList)
        {
            throw new NotImplementedException();
        }

        public Response<bool> StopAllMoves()
        {
            throw new NotImplementedException();
        }

        public Response<bool> StopLanding()
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _remoteService.TryInvokeAndGetMessages(s => s.SubscribeToAxesChanges());
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
                resp = _remoteService.TryInvokeAndGetMessages(s => s.UnsubscribeToAxesChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Axes unsubscribe error");
            }

            return resp;
        }

        public Response<bool> WaitMotionEnd(int timeout, bool waitStabilization)
        {
            throw new NotImplementedException();
        }

        public Response<bool> MoveIncremental(IncrementalMoveBase move, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }

        public Response<VoidResult> SendCommandToAxes(string commandToApply)
        {
            throw new NotImplementedException();
        }

        public Response<AxesConfig> GetAxesConfiguration()
        {
            throw new NotImplementedException();
        }

        public Response<bool> GotoPointCustomSpeedAccel(AxisMove moveX, AxisMove moveY, AxisMove moveZTop, AxisMove moveZBottom)
        {
            throw new NotImplementedException();
        }

        public Response<AxesState> GetCurrentState()
        {
            throw new NotImplementedException();
        }

        public void StateChangedCallback(AxesState state)
        {
            throw new NotImplementedException();
        }

        public Response<bool> ResetAxis()
        {
            throw new NotImplementedException();
        }

        public Response<bool> AcknowledgeResetAxis()
        {
            throw new NotImplementedException();
        }

        public Task<Response<VoidResult>> ResetZTopFocus()
        {
            throw new NotImplementedException();
        }

        public Task<Response<VoidResult>> ResetZBottomFocus()
        {
            throw new NotImplementedException();
        }

        public Response<bool> GotoSpecificPosition(SpecificPositions positionRefId, Length waferDiameter, AxisSpeed speed)
        {
            throw new NotImplementedException();
        }


    }
}
