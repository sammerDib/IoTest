using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Axes
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class MotionAxesSupervisor : IMotionAxesService, IMotionAxesServiceCallback, IMotionAxesServiceCallbackProxy
    {
        private readonly ILogger _logger;
        private readonly IDialogOwnerService _dialogService;

        private readonly DuplexServiceInvoker<IMotionAxesService> _motionAxesService;

        private MotionAxesVM _motionAxesVM;

        private IMessenger _messenger;

        public MotionAxesSupervisor(ILogger<MotionAxesSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;

            var instanceContext = new InstanceContext(this);
            _motionAxesService = new DuplexServiceInvoker<IMotionAxesService>(instanceContext, "HARDWAREMotionAxesService", ClassLocator.Default.GetInstance<SerilogLogger<IMotionAxesService>>(), messenger, s => s.SubscribeToAxesChanges(), ClientConfiguration.GetServiceAddress(UnitySC.Shared.Data.Enum.ActorType.HardwareControl));
        }

        public MotionAxesVM MotionAxesVM
        {
            get
            {
                if (_motionAxesVM == null)
                {
                    _motionAxesVM = new MotionAxesVM(this, _dialogService, _logger);
                }
                return _motionAxesVM;
            }
        }

        public Response<VoidResult> SubscribeToAxesChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _motionAxesService.TryInvokeAndGetMessages(s => s.SubscribeToAxesChanges());
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
                resp = _motionAxesService.TryInvokeAndGetMessages(s => s.UnsubscribeToAxesChanges());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Motion axes unsubscribe error");
            }

            return resp;
        }

        public Response<bool> Move(params PMAxisMove[] moves)
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.Move(moves));
        }

        public Response<bool> RelativeMove(params PMAxisMove[] moves)
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.RelativeMove(moves));
        }

        public Response<bool> GoToHome(AxisSpeed speed = 0)
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.GoToHome(speed));
        }

        public Response<AxesConfig> GetAxesConfiguration()
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.GetAxesConfiguration());
        }

        public Response<PositionBase> GetCurrentPosition()
        {
            var curPosition = _motionAxesService.TryInvokeAndGetMessages(s => s.GetCurrentPosition());
            // We update the position
            if (_motionAxesVM != null && curPosition.Result is PositionBase position)
            {
                _motionAxesVM.UpdatePosition(curPosition.Result);
            }
            return curPosition;
        }

        public Response<AxesState> GetCurrentState()
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.GetCurrentState());
        }

        public Response<bool> StopAllMotion()
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.StopAllMotion());
        }

        public Response<bool> WaitMotionEnd(int timeout)
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.WaitMotionEnd(timeout));
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _motionAxesService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        void IMotionAxesServiceCallback.DeviceStateChangedCallback(DeviceState deviceState)
        {
            _messenger.Send(new DeviceStateChangedMessage() { State = deviceState });
        }

        void IMotionAxesServiceCallback.PositionChangedCallback(PositionBase newPosition)
        {
            if (_motionAxesVM != null && newPosition is PositionBase position)
            {
                _motionAxesVM.UpdatePosition(position);
            }
            else
            {
                _logger.Warning($"Received position is not a PositionBase");
            }
        }

        void IMotionAxesServiceCallback.StateChangedCallback(AxesState state)
        {
        }

        void IMotionAxesServiceCallback.EndMoveCallback(bool targetReached)
        {
        }

        public void PositionChanged(PositionBase newPosition)
        {
            if (_motionAxesVM != null && newPosition is PositionBase position)
            {
                _motionAxesVM.UpdatePosition(position);
            }
            else
            {
                _logger.Warning($"Received position is not a PositionBase");
            }
        }

        public void StateChanged(AxesState state)
        {
        }

        public void EndMove(bool targetReached)
        {
        }
    }
}
