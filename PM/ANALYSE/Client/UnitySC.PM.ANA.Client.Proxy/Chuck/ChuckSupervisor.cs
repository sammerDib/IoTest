using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Configuration;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.Proxy.Chuck
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class ChuckSupervisor : IChuckService, IChuckServiceCallback
    {
        private readonly ILogger _logger;

        private readonly DuplexServiceInvoker<IChuckService> _chuckService;

        private ChuckVM _chuck;

        private readonly IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ChuckSupervisor(ILogger<ChuckSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            var instanceContext = new InstanceContext(this);
            // Probe service
            _chuckService = new DuplexServiceInvoker<IChuckService>(instanceContext, "ANALYSEChuckService", ClassLocator.Default.GetInstance<SerilogLogger<IChuckService>>(), messenger, s => s.SubscribeToChuckChanges(), ClientConfiguration.GetServiceAddress(ActorType.ANALYSE));
            _chuckService.TryInvokeAndGetMessages(s => s.GetCurrentState());
            _logger = logger;
            _dialogService = dialogService;
        }

        public Response<ChuckState> GetCurrentState()
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.GetCurrentState());
        }

        public ChuckVM ChuckVM
        {
            get
            {
                if (_chuck == null)
                {
                    _chuck = new ChuckVM(this, _dialogService, _logger);
                    _chuck.Init();
                }
                return _chuck;
            }
        }

        public Response<VoidResult> SubscribeToChuckChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _chuckService.TryInvokeAndGetMessages(s => s.SubscribeToChuckChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Axes subscribe error");
            }

            return resp;
        }

        public Response<VoidResult> UnsubscribeToChuckChanges()
        {
            var resp = new Response<VoidResult>();

            try
            {
                resp = _chuckService.TryInvokeAndGetMessages(s => s.UnsubscribeToChuckChanges());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Axes unsubscribe error");
            }

            return resp;
        }

        public void StateChangedCallback(ChuckState state)
        {
            //Console.WriteLine("State Changed CallBack");
            if (_chuck == null)
                return;
            _chuck.UpdateStatus(state);
        }

        public Response<ChuckBaseConfig> GetChuckConfiguration()
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.GetChuckConfiguration());
        }

        public Response<bool> ClampWafer(WaferDimensionalCharacteristic wafer)
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.ClampWafer(wafer));
        }

        public Response<bool> ReleaseWafer(WaferDimensionalCharacteristic wafer)
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.ReleaseWafer(wafer));
        }

        public async Task<Response<VoidResult>> ResetAirbearing()
        {
            var task = _chuckService.InvokeAndGetMessagesAsync(s => s.ResetAirbearing());
            await task;
            return task.Result;
        }

        public async Task<Response<VoidResult>> ResetWaferStage()
        {
            var task = _chuckService.InvokeAndGetMessagesAsync(s => s.ResetWaferStage());
            await task;
            return task.Result;
        }

        public Response<Dictionary<string, float>> GetSensorValues()
        {
            return _chuckService.TryInvokeAndGetMessages(s => s.GetSensorValues());
        }

        public Response<VoidResult> TriggerUpdateEvent()
        {
            throw new NotImplementedException();
        }

        public Response<MaterialPresence> GetWaferPresence()
        {
            throw new NotImplementedException();
        }

        public void ChuckWaferPresenceChangedCallback(MaterialPresence waferPresence)
        {
            throw new NotImplementedException();
        }
    }
}
