using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.OpticalPowermeter
{
    [CallbackBehaviorAttribute(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class OpticalPowermeterSupervisor : IOpticalPowermeterServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private IMessenger _messenger;
        private DuplexServiceInvoker<IOpticalPowermeterService> _opticalPowermeterService;
        private IDialogOwnerService _dialogService;

        /// <summary>
        /// Constructor
        /// </summary>
        public OpticalPowermeterSupervisor(ILogger<OpticalPowermeterSupervisor> logger, IMessenger messenger, IDialogOwnerService dialogService)
        {
            _instanceContext = new InstanceContext(this);
            _opticalPowermeterService = new DuplexServiceInvoker<IOpticalPowermeterService>(_instanceContext, "OpticalPowermeterService", ClassLocator.Default.GetInstance<SerilogLogger<IOpticalPowermeterService>>(), messenger, s => s.SubscribeToChanges());
            _logger = logger;
            _messenger = messenger;
            _dialogService = dialogService;
        }        

        public Response<VoidResult> Connect(PowerIlluminationFlow flow)
        {
            return _opticalPowermeterService.TryInvokeAndGetMessages(s => s.Connect(flow));
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _opticalPowermeterService.TryInvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _opticalPowermeterService.TryInvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> TriggerUpdateEvent(PowerIlluminationFlow flow)
        {
            return _opticalPowermeterService.TryInvokeAndGetMessages(s => s.TriggerUpdateEvent(flow));
        }

        public Response<VoidResult> CustomCommand(PowerIlluminationFlow flow, string customCmd)
        {
            return _opticalPowermeterService.TryInvokeAndGetMessages(s => s.CustomCommand(flow, customCmd));
        }

        void IOpticalPowermeterServiceCallback.StateChangedCallback(PowerIlluminationFlow flow, string state)
        {
            throw new NotImplementedException();
        }

        void IOpticalPowermeterServiceCallback.PowerChangedCallback(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor)
        {
            _messenger.Send(new PowerChangedMessage() { Flow = flow, Power = power, PowerCal_mW = powerCal_mW, RFactor = rfactor });
        }

        void IOpticalPowermeterServiceCallback.MaximumPowerChangedCallback(PowerIlluminationFlow flow, double power)
        {
            _messenger.Send(new MaxPowerChangedMessage() { Flow = flow, MaximumPower = power });
        }

        void IOpticalPowermeterServiceCallback.MinimumPowerChangedCallback(PowerIlluminationFlow flow, double power)
        {
            _messenger.Send(new MinPowerChangedMessage() { Flow = flow, MinimumPower = power });
        }

        void IOpticalPowermeterServiceCallback.WavelengthChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new WavelengthChangedMessage() { Flow = flow, Wavelength = value });
        }

        void IOpticalPowermeterServiceCallback.BeamDiameterChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new BeamDiameterChangedMessage() { Flow = flow, BeamDiameter = value });
        }

        void IOpticalPowermeterServiceCallback.WavelengthRangeChangedCallback(PowerIlluminationFlow flow, double value)
        {
            _messenger.Send(new WavelengthRangeChangedMessage() { Flow = flow, WavelengthRange = value });
        }

        void IOpticalPowermeterServiceCallback.IdentifierChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new IdentifierChangedMessage() { Flow = flow, Identifier = value });
        }

        void IOpticalPowermeterServiceCallback.CustomChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new CustomChangedMessage() { Flow = flow, Custom = value });
        }

        void IOpticalPowermeterServiceCallback.CurrentChangedCallback(PowerIlluminationFlow flow, double current_mA)
        {
            _messenger.Send(new CurrentChangedMessage() { Flow = flow, Current_mA = current_mA });
        }

        void IOpticalPowermeterServiceCallback.AvailableWavelengthsCallback(List<string> wavelengths)
        {
            _logger.Information("AvailableWavelengthsCallback");
            _messenger.Send(new AvailableWavelengthsChangedMessage() { Wavelengths = wavelengths });
        }

        void IOpticalPowermeterServiceCallback.DarkAdjustStateChangedCallback(UnitySC.Shared.Data.Enum.PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new DarkAdjustStateChangedMessage() { Flow = flow, DarkAdjustState = value });
        }

        void IOpticalPowermeterServiceCallback.DarkOffsetChangedCallback(PowerIlluminationFlow flow, double value)
        {
            _messenger.Send(new DarkOffsetChangedMessage() { Flow = flow, DarkOffset_mW = value });
        }

        void IOpticalPowermeterServiceCallback.ResponsivityChangedCallback(PowerIlluminationFlow flow, double value)
        {
            _messenger.Send(new ResponsivityChangedMessage() { Flow = flow, Responsivity = value });
        }

        void IOpticalPowermeterServiceCallback.SensorTypeChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new SensorTypeChangedMessage() { Flow = flow, SensorType = value });
        }

        void IOpticalPowermeterServiceCallback.SensorAttenuationChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new SensorAttenuationChangedMessage() { Flow = flow, SensorAttenuation = value });
        }
    }
}
