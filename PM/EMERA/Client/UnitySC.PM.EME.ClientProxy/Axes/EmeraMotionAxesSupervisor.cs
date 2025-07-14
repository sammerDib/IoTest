using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Proxy.Axes
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class EmeraMotionAxesSupervisor : IEmeraMotionAxesService, IMotionAxesServiceCallback, IMotionAxesServiceCallbackProxy
    {
        private readonly ILogger _logger;
        private readonly DuplexServiceInvoker<IEmeraMotionAxesService> _motionAxesService;
        private readonly IMessenger _messenger;

        public EmeraMotionAxesSupervisor(ILogger<IEmeraMotionAxesService> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;

            var instanceContext = new InstanceContext(this);
            var serviceAddress = ClientConfiguration.GetServiceAddress(ActorType.HardwareControl);
            _motionAxesService = new DuplexServiceInvoker<IEmeraMotionAxesService>(instanceContext, "EMERAMotionAxesService", logger, messenger, s => s.SubscribeToAxesChanges(), serviceAddress);
        }

        #region IEmeraMotionAxesService
        public Response<AxesConfig> GetAxesConfiguration()
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.GetAxesConfiguration());
        }

        public Response<PositionBase> GetCurrentPosition()
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.GetCurrentPosition());
        }

        public Response<AxesState> GetCurrentState()
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.GetCurrentState());
        }

        public Response<bool> GoToHome(AxisSpeed speed)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.GoToHome(speed));
        }

        public Response<VoidResult> GoToPosition(XYZPosition targetPosition, AxisSpeed speed)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.GoToPosition(targetPosition, speed));
        }

        public Response<bool> Move(params PMAxisMove[] moves)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.Move(moves));
        }

        public Response<bool> RelativeMove(params PMAxisMove[] moves)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.RelativeMove(moves));
        }

        public Response<bool> StopAllMotion()
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.StopAllMotion());
        }

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _motionAxesService.InvokeAndGetMessages(s => s.SubscribeToAxesChanges());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Motion axes subscribe error");
            }
            return resp;
        }

        public Response<VoidResult> UnsubscribeToAxesChanges()
        {
            var resp = new Response<VoidResult>();
            try
            {
                resp = _motionAxesService.InvokeAndGetMessages(s => s.UnsubscribeToAxesChanges());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Motion axes unsubscribe error");
            }
            return resp;
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<bool> WaitMotionEnd(int timeout)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.WaitMotionEnd(timeout));
        }

        public Response<bool> GoToEfemLoad(Length waferDiameter, AxisSpeed speed)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.GoToEfemLoad(waferDiameter, speed));
        }

        public Response<bool> GoToManualLoad(Length waferDiameter, AxisSpeed speed)
        {
            return _motionAxesService.InvokeAndGetMessages(s => s.GoToManualLoad(waferDiameter,speed));
        }    
        #endregion

        #region IMotionAxesServiceCallback

        void IMotionAxesServiceCallback.DeviceStateChangedCallback(DeviceState deviceState)
        {
            _messenger.Send(new DeviceStateChangedMessage() { State = deviceState });
        }

        void IMotionAxesServiceCallback.EndMoveCallback(bool targetReached)
        {
        }

        void IMotionAxesServiceCallback.PositionChangedCallback(PositionBase newPosition)
        {
            if (newPosition != null)
            {
                _messenger.Send(newPosition);
            }           
        }

        void IMotionAxesServiceCallback.StateChangedCallback(AxesState state)
        {
            _messenger.Send(state);
        }

        #endregion

        #region IMotionAxesServiceCallbackProxy        
        public void EndMove(bool targetReached)
        {
        }

        public void PositionChanged(PositionBase newPosition)
        {
            if (newPosition != null)
            {
                _messenger.Send(newPosition);
            }           
        }

        public void StateChanged(AxesState state)
        {
        }
        #endregion
    }
}
