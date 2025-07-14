using System;
using System.Collections.Generic;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface.Chamber;
using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Client.Proxy.Chamber
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ChamberSupervisor : IDMTChamberService, IDMTChamberServiceCallback
    {
        private readonly ILogger _logger;
        private IMessenger _messenger;
        private readonly DuplexServiceInvoker<IDMTChamberService> _dmtChamberService;

        private ChamberVM _chamberVM;

        public ChamberSupervisor(ILogger<ChamberSupervisor> logger, ILogger<IDMTChamberService> serviceLogger, IMessenger messenger, ActorType? actorType)
        {
            var instanceContext = new InstanceContext(this);

            var endPoint = "ChamberService";
            if (actorType != null)
                endPoint = actorType + endPoint;
            _dmtChamberService = new DuplexServiceInvoker<IDMTChamberService>(instanceContext, endPoint, serviceLogger, messenger,
                s => s.SubscribeToChanges(), ClientConfiguration.GetServiceAddress(actorType));

            _logger = logger;
            _messenger = messenger;

        }

        public ChamberVM ChamberVM
        {
            get
            {
                if (_chamberVM == null)
                {
                    _chamberVM = new ChamberVM(this);
                }
                return _chamberVM;
            }
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _dmtChamberService.InvokeAndGetMessages(s => s.SubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chamber subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _dmtChamberService.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Chamber unsubscribe error");
            }

            return resp;
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            return _dmtChamberService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent());
        }

        public Response<DMTChamberConfig> GetChamberConfiguration()
        {
            return _dmtChamberService.TryInvokeAndGetMessages(s => s.GetChamberConfiguration());
        }

        public void UpdateIsInMaintenanceCallback(bool value)
        {
            _messenger.Send(new IsInMaintenanceChangedMessage() { IsInMaintenance = value });
        }

        public void UpdateArmNotExtendedCallback(bool value)
        {
            _messenger.Send(new ArmNotExtendedChangedMessage() { ArmNotExtended = value });
        }

        public void UpdateEfemSlitDoorOpenPositionCallback(bool value)
        {
            _messenger.Send(new EfemSlitDoorOpenPositionChangedMessage() { EfemSlitDoorOpenPosition = value });
        }

        public void UpdateIsReadyToLoadUnloadCallback(bool value)
        {
            _messenger.Send(new IsReadyToLoadUnloadChangedMessage() { IsReadyToLoadUnload = value });
        }

        public void UpdateInterlocksCallback(InterlockMessage interlock)
        {
            _messenger.Send(interlock);
        }

        public void UpdateSlitDoorPositionCallback(SlitDoorPosition position)
        {
            _messenger.Send(new SlitDoorPositionChangedMessage() { SlitDoorPosition = position });
        }

        public void UpdateSlitDoorOpenPositionCallback(bool position)
        {
            _messenger.Send(new SlitDoorOpenPositionChangedMessage() { SlitDoorOpenPosition = position });
        }

        public void UpdateSlitDoorClosePositionCallback(bool position)
        {
            _messenger.Send(new SlitDoorClosePositionChangedMessage() { SlitDoorClosePosition = position });
        }

        public void UpdateCdaPneumaticValveCallback(bool valveIsOpened)
        {
            _messenger.Send(new CdaPneumaticValveChangedMessage() { ValveIsOpened = valveIsOpened });
        }

        public Response<VoidResult> OpenSlitDoor()
        {
            return _dmtChamberService.TryInvokeAndGetMessages(s => s.OpenSlitDoor());
        }

        public Response<VoidResult> CloseSlitDoor()
        {
            return _dmtChamberService.TryInvokeAndGetMessages(s => s.CloseSlitDoor());
        }

        public Response<VoidResult> OpenCdaPneumaticValve()
        {
            return _dmtChamberService.TryInvokeAndGetMessages(s => s.OpenCdaPneumaticValve());
        }

        public Response<VoidResult> CloseCdaPneumaticValve()
        {
            return _dmtChamberService.TryInvokeAndGetMessages(s => s.CloseCdaPneumaticValve());
        }
    }
}
