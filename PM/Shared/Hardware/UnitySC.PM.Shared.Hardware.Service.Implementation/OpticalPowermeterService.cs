using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using UnitySC.PM.Shared.Hardware.OpticalPowermeter;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Service.Interface.OpticalPowermeter;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;
using System.Windows.Markup;

namespace UnitySC.PM.Shared.Hardware.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class OpticalPowermeterService : DuplexServiceBase<IOpticalPowermeterServiceCallback>, IOpticalPowermeterService
    {
        private HardwareManager _hardwareManager;

        public string PowermeterId { get; set; }

        public OpticalPowermeterService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<StateMessage>(this, (r, m) => { UpdateState(m.Flow, m.State); });
            messenger.Register<PowerMessage>(this, (r, m) => { UpdatePower(m.Flow, m.Power, m.PowerCal_mW, m.RFactor); });
            messenger.Register<MaxPowerMessage>(this, (r, m) => { UpdateMaxPower(m.Flow, m.MaximumPower); });
            messenger.Register<MinPowerMessage>(this, (r, m) => { UpdateMinPower(m.Flow, m.MinimumPower); });
            messenger.Register<WavelengthMessage>(this, (r, m) => { UpdateWavelength(m.Flow, m.Wavelength); });
            messenger.Register<BeamDiameterMessage>(this, (r, m) => { UpdateBeamDiameter(m.Flow, m.BeamDiameter); });
            messenger.Register<WavelengthRangeMessage>(this, (r, m) => { UpdateWavelengthRange(m.Flow, m.WavelengthRange); });
            messenger.Register<IdentifierMessage>(this, (r, m) => { UpdateIdentifier(m.Flow, m.Identifier); });
            messenger.Register<CustomMessage>(this, (r, m) => { UpdateCustom(m.Flow, m.Custom); });
            messenger.Register<AvailableWavelengthsMessage>(this, (r, m) => { UpdateAvailableWavelengths(m.Wavelengths); });

            messenger.Register<DarkAdjustStateMessage>(this, (r, m) => { UpdateDarkAdjustState(m.Flow, m.DarkAdjustState); });
            messenger.Register<DarkOffsetMessage>(this, (r, m) => { UpdateDarkOffset(m.Flow, m.DarkOffset); });
            messenger.Register<ResponsivityMessage>(this, (r, m) => { UpdateResponsivity(m.Flow, m.Responsivity); });
            messenger.Register<SensorTypeMessage>(this, (r, m) => { UpdateSensorType(m.Flow, m.SensorType); });            
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to OpticalPowermeter change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to OpticalPowermeter change"));
            });
        }        

        public Response<VoidResult> Connect(PowerIlluminationFlow flow)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.OpticalPowermeters[flow.ToString()] != null)
                {
                    _logger.Information("OpticalPowermeter communication Connect");                    
                    _hardwareManager.OpticalPowermeters[flow.ToString()].Connect();
                    messageContainer.Add(new Message(MessageLevel.Information, "Hardware status " + _hardwareManager.OpticalPowermeters[flow.ToString()].State.Status.ToString()));
                }
            });
        }

        public Response<VoidResult> TriggerUpdateEvent(PowerIlluminationFlow flow)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("OpticalPowermeter initialize update");
                _hardwareManager.OpticalPowermeters[flow.ToString()].TriggerUpdateEvent();
                messageContainer.Add(new Message(MessageLevel.Information, "OpticalPowermeter initialize update"));
            });
        }
        
        public Response<VoidResult> CustomCommand(PowerIlluminationFlow flow, string custom)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("CustomCommand " + custom);
                _hardwareManager.OpticalPowermeters[flow.ToString()].CustomCommand(custom);
                messageContainer.Add(new Message(MessageLevel.Information, "CustomCommand " + custom));
            });
        }

        public void UpdateState(PowerIlluminationFlow flow, string state)
        {
            InvokeCallback(i => i.StateChangedCallback(flow, state));
        }

        public void UpdatePower(PowerIlluminationFlow flow, double value, double powerCal_mW, double rfactor)
        {
            InvokeCallback(i => i.PowerChangedCallback(flow, value, powerCal_mW, rfactor));
        }

        public void UpdateMaxPower(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.MaximumPowerChangedCallback(flow, value));
        }

        public void UpdateMinPower(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.MinimumPowerChangedCallback(flow, value));
        }

        public void UpdateWavelength(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.WavelengthChangedCallback(flow, value));
        }

        public void UpdateBeamDiameter(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.BeamDiameterChangedCallback(flow, value));
        }

        public void UpdateWavelengthRange(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.WavelengthRangeChangedCallback(flow, value));
        }

        public void UpdateIdentifier(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.IdentifierChangedCallback(flow, value));
        }

        public void UpdateCustom(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.CustomChangedCallback(flow, value));
        }

        public void UpdateAvailableWavelengths(List<string> wavelengths)
        {
            InvokeCallback(i => i.AvailableWavelengthsCallback(wavelengths));
        }

        public void UpdateDarkAdjustState(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.DarkAdjustStateChangedCallback(flow, value));
        }

        public void UpdateDarkOffset(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.DarkOffsetChangedCallback(flow, value));
        }

        public void UpdateResponsivity(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.ResponsivityChangedCallback(flow, value));
        }

        public void UpdateSensorType(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.SensorTypeChangedCallback(flow, value));
        }

        public void UpdateSensorAttenuation(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.SensorAttenuationChangedCallback(flow, value));
        }
    }
}
