using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.PM.Shared.Hardware.AttenuationFilter;
using UnitySC.PM.Shared.Hardware.Common;
using UnitySC.PM.Shared.Hardware.Laser;
using UnitySC.PM.Shared.Hardware.PolarisationFilter;
using UnitySC.PM.Shared.Hardware.Shutter;
using UnitySC.PM.Shared.Hardware.Mppc;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.PM.Shared.Hardware.Chamber;
using System.Collections.Generic;
using UnitySC.Shared.Data;
using System.Collections.ObjectModel;
using System;

namespace UnitySC.PM.LIGHTSPEED.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class LSRotatorsKitCalibrationService : DuplexServiceBase<IRotatorsKitCalibrationServiceCallback>, IRotatorsKitCalibrationService
    {
        private HardwareManager _hardwareManager;

        public string PowermeterId { get; set; }

        public LSRotatorsKitCalibrationService(ILogger logger, HardwareManager hardwareManager) : base(logger, ExceptionType.HardwareException)
        {
            _hardwareManager = hardwareManager;
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();

            messenger.Register<Shared.Hardware.OpticalPowermeter.PowerMessage>(this, (m) => { UpdatePowermeterPower(m.Flow, m.Power, m.PowerCal_mW, m.RFactor); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.CurrentMessage>(this, (m) => { UpdatePowermeterCurrent(m.Flow, m.Current_mA); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.IdentifierMessage>(this, (m) => { UpdatePowermeterIdentifier(m.Flow, m.Identifier); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.WavelengthMessage>(this, (m) => { UpdatePowermeterWavelength(m.Flow, m.Wavelength); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.RangesMessage>(this, (m) => { UpdatePowermeterRanges(m.Flow, m.PowermeterRange); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.BeamDiameterMessage>(this, (m) => { UpdatePowermeterBeamDiameter(m.Flow, m.BeamDiameter); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.DarkAdjustStateMessage>(this, (m) => { UpdatePowermeterDarkAdjustState(m.Flow, m.DarkAdjustState); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.DarkOffsetMessage>(this, (m) => { UpdatePowermeterDarkOffset(m.Flow, m.DarkOffset); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.ResponsivityMessage>(this, (m) => { UpdatePowermeterResponsivity(m.Flow, m.Responsivity); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.SensorTypeMessage>(this, (m) => { UpdatePowermeterSensorType(m.Flow, m.SensorType); });            
            messenger.Register<Shared.Hardware.OpticalPowermeter.SensorAttenuationMessage>(this, (m) => { UpdatePowermeterSensorAttenuation(m.Flow, m.SensorAttenuation); });
            messenger.Register<Shared.Hardware.OpticalPowermeter.RFactorsCalibMessage>(this, (m) => { UpdatePowermeterRFactorsCalib(m.Flow, m.RFactorS, m.RFactorP); });

            messenger.Register<Shared.Hardware.Laser.LaserQuantum.PowerMessage>(this, (m) => { UpdatePowerLaser(m.Power); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.InterlockStatusMessage>(this, (m) => { UpdateInterlockStatus(m.InterlockStatus); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.LaserTemperatureMessage>(this, (m) => { UpdateLaserTemperature(m.LaserTemperature); });
            messenger.Register<Shared.Hardware.Laser.LaserQuantum.PsuTemperatureMessage>(this, (m) => { UpdatePsuTemperature(m.PsuTemperature); });
            messenger.Register<AttenuationPositionMessage>(this, (m) => { UpdateAttenuationPosition(m.Position); });
            messenger.Register<PolarisationPositionMessage>(this, (m) => { UpdatePolarisationPosition(m.Position); });
            messenger.Register<PolarisationAngleCalibMessage>(this, (m) => { UpdatePolarisationAngleCalib(m.PolarAngleHsS, m.PolarAngleHsP, m.PolarAngleHtS, m.PolarAngleHtP); });
            messenger.Register<ShutterIrisPositionMessage>(this, (m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
            messenger.Register<StateSignalsMessage>(this, (m) => { UpdateMppcStateSignals(m.Collector, m.StateSignals); });
            messenger.Register<OutputVoltageMessage>(this, (m) => { UpdateMppcOutputVoltage(m.Collector, m.OutputVoltage); });
            messenger.Register<OutputCurrentMessage>(this, (m) => { UpdateMppcOutputCurrent(m.Collector, m.OutputCurrent); });
            messenger.Register<SensorTemperatureMessage>(this, (m) => { UpdateMppcSensorTemperature(m.Collector, m.SensorTemperature); });

            messenger.Register<DataAttributesChamberMessage>(this, (m) => { UpdateDataAttributes(m.DataAttributes); });
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to RotatorsKitCalibration change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to RotatorsKitCalibration change"));
            });
        }

        public Response<VoidResult> PowerOn()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("PowerOn");
                    _hardwareManager.Laser.PowerOn();
                    messageContainer.Add(new Message(MessageLevel.Information, "PowerOn"));
                }
            });
        }

        public Response<VoidResult> PowerOff()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("PowerOff");
                    _hardwareManager.Laser.PowerOff();
                    messageContainer.Add(new Message(MessageLevel.Information, "PowerOff"));
                }
            });
        }

        public Response<VoidResult> SetPower(int power)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("SetPower");
                    _hardwareManager.Laser.SetPower(power);
                    messageContainer.Add(new Message(MessageLevel.Information, "SetPower"));
                }
            });
        }

        public Response<VoidResult> SetCurrent(int current)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Laser != null)
                {
                    _logger.Debug("SetCurrent");
                    _hardwareManager.Laser.SetCurrent(current);
                    messageContainer.Add(new Message(MessageLevel.Information, "SetCurrent"));
                }
            });
        }

        public Response<VoidResult> MoveAbsPosition(BeamShaperFlow beamShaperFlow, double position)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("MoveAbsPosition " + "[" + position.ToString() + "]");
                if (beamShaperFlow == BeamShaperFlow.Attenuation)
                {
                    if (_hardwareManager.AttenuationFilter != null)
                        _hardwareManager.AttenuationFilter.MoveAbsPosition(position);
                }
                else
                {
                    if (_hardwareManager.OpticalPowermeters != null)
                        _hardwareManager.PolarisationFilter.MoveAbsPosition(position);
                }

                messageContainer.Add(new Message(MessageLevel.Information, "MoveAbsPosition " + "[" + position.ToString() + "]"));
            });
        }

        public Response<VoidResult> HomePosition(BeamShaperFlow beamShaperFlow)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("HomePosition");
                if (beamShaperFlow == BeamShaperFlow.Attenuation)
                {
                    if (_hardwareManager.AttenuationFilter != null)
                        _hardwareManager.AttenuationFilter.HomePosition();
                }
                else
                {
                    if (_hardwareManager.OpticalPowermeters != null)
                        _hardwareManager.PolarisationFilter.HomePosition();
                }
                messageContainer.Add(new Message(MessageLevel.Information, "HomePosition"));
            });
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Shutter != null)
                {
                    _logger.Information("OpenShutterCommand");
                    _hardwareManager.Shutter.OpenShutter();
                    messageContainer.Add(new Message(MessageLevel.Information, "OpenShutterCommand"));
                }
            });
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Shutter != null)
                {
                    _logger.Information("CloseShutterCommand");
                    _hardwareManager.Shutter.CloseShutter();
                    messageContainer.Add(new Message(MessageLevel.Information, "CloseShutterCommand"));
                }
            });
        }

        public Response<VoidResult> InitializeUpdate()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Debug("InitializeUpdate");
                if (_hardwareManager.Chamber != null)
                    _hardwareManager.Chamber.InitializeUpdate();
                if (_hardwareManager.Laser != null)
                    _hardwareManager.Laser.InitializeUpdate();
                if (_hardwareManager.AttenuationFilter != null)
                    _hardwareManager.AttenuationFilter.InitializeUpdate();
                if (_hardwareManager.Shutter != null)
                    _hardwareManager.Shutter.InitializeUpdate();

                if (_hardwareManager.Mppcs.Count != 0)
                {
                    foreach (var mppc in _hardwareManager.Mppcs)
                    {
                        if (mppc.Key.ToString() == MppcCollector.WIDE.ToString())
                            _hardwareManager.Mppcs[MppcCollector.WIDE.ToString()].InitializeUpdate();
                        else
                            _hardwareManager.Mppcs[MppcCollector.NARROW.ToString()].InitializeUpdate();
                    }
                }

                if (_hardwareManager.PolarisationFilter != null)
                    _hardwareManager.PolarisationFilter.InitializeUpdate();

                if (_hardwareManager.OpticalPowermeters.Count != 0)
                {
                    foreach (var powermeter in _hardwareManager.OpticalPowermeters)
                    {
                        _hardwareManager.OpticalPowermeters[powermeter.Key].InitializeUpdate();
                    }
                    messageContainer.Add(new Message(MessageLevel.Information, "InitializeUpdate"));
                }
            });
        }

        public Response<VoidResult> AttenuationRefresh()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.AttenuationFilter != null)
                    _hardwareManager.AttenuationFilter.InitializeUpdate();
            });
        }

        public Response<VoidResult> MppcPowerOn(MppcCollector collector, bool powerOn)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Mppcs[collector.ToString()] != null)
                {
                    if (powerOn)
                        _hardwareManager.Mppcs[collector.ToString()].OutputVoltageOn(); 
                    else
                        _hardwareManager.Mppcs[collector.ToString()].OutputVoltageOff();
                }
            });
        }

        public Response<VoidResult> MppcSetOutputVoltage(MppcCollector collector, double voltage)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                if (_hardwareManager.Mppcs[collector.ToString()] != null)
                {
                    _hardwareManager.Mppcs[collector.ToString()].SetOutputVoltage(voltage);
                }
            });
        }

        public Response<VoidResult> MppcManageRelay(MppcCollector collector, bool relayActivated)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.Mppcs[collector.ToString()].ManageRelays(relayActivated);
            });
        }

        public Response<VoidResult> SetIoValue(string identifier, string name, bool value)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Set IO identifier " + identifier + " - named " + name + "[" + value.ToString() + "]");
                _hardwareManager.Chamber.SetIoValue(identifier, name, value);
                messageContainer.Add(new Message(MessageLevel.Information, "Set IO identifier " + identifier + " - named " + name + "[" + value.ToString() + "]"));
            });
        }

        public Response<VoidResult> GetIoValue(string identifier, string name)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _logger.Information("Get IO identifier " + identifier + " - named " + name);
                _hardwareManager.Chamber.GetIoValue(identifier, name);
                messageContainer.Add(new Message(MessageLevel.Information, "Get IO identifier " + identifier + " - named " + name));
            });
        }

        public Response<VoidResult> PowermeterEnableAutoRange(PowerIlluminationFlow flow, bool activate)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.OpticalPowermeters[flow.ToString()].EnableAutoRange(activate);
            });
        }

        public Response<VoidResult> PowermeterRangesVariation(PowerIlluminationFlow flow, string range)
        {
            return InvokeVoidResponse(messageContainer =>
{
                _hardwareManager.OpticalPowermeters[flow.ToString()].RangesVariation(range);
            });
        }

        public Response<VoidResult> PowermeterStartDarkAdjust(PowerIlluminationFlow flow)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.OpticalPowermeters[flow.ToString()].StartDarkAdjust();
            });
        }

        public Response<VoidResult> PowermeterCancelDarkAdjust(PowerIlluminationFlow flow)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.OpticalPowermeters[flow.ToString()].CancelDarkAdjust();
            });
        }

        public Response<VoidResult> PowermeterEditResponsivity(PowerIlluminationFlow flow, double responsivity_mA_W)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                _hardwareManager.OpticalPowermeters[flow.ToString()].EditResponsivity(responsivity_mA_W);
            });
        }

        public Response<VoidResult> PowermeterEditRFactorsCalib(PowerIlluminationFlow flow, double rfactorS, double rfactorP)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                //_hardwareManager.OpticalPowermeters[flow.ToString()].EditResponsivity(responsivity_mA_W);
            });
        }

        public void UpdatePowermeterPower(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor)
        {
            InvokeCallback(i => i.PowermeterPowerChangedCallback(flow, power, powerCal_mW, rfactor));
        }

        public void UpdatePowermeterCurrent(PowerIlluminationFlow flow, double current_mA)
        {
            InvokeCallback(i => i.PowermeterCurrentChangedCallback(flow, current_mA));
        }

        public void UpdatePowermeterIdentifier(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.PowermeterIdentifierChangedCallback(flow, value));
        }        

        public void UpdatePowermeterSensorType(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.PowermeterSensorTypeChangedCallback(flow, value));
        }

        public void UpdatePowermeterSensorAttenuation(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.PowermeterSensorAttenuationChangedCallback(flow, value));
        }

        public void UpdatePowermeterWavelength(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.PowermeterWavelengthChangedCallback(flow, value));
        }

        public void UpdatePowermeterRanges(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.PowermeterRangeChangedCallback(flow, value));
        }
        
        public void UpdatePowermeterBeamDiameter(PowerIlluminationFlow flow, uint value)
        {
            InvokeCallback(i => i.PowermeterBeamDiameterChangedCallback(flow, value));
        }

        public void UpdatePowermeterDarkAdjustState(PowerIlluminationFlow flow, string value)
        {
            InvokeCallback(i => i.PowermeterDarkAdjustStateChangedCallback(flow, value));
        }

        public void UpdatePowermeterDarkOffset(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.PowermeterDarkOffsetChangedCallback(flow, value));
        }

        public void UpdatePowermeterResponsivity(PowerIlluminationFlow flow, double value)
        {
            InvokeCallback(i => i.PowermeterResponsivityChangedCallback(flow, value));
        }

        public void UpdatePowermeterRFactorsCalib(PowerIlluminationFlow flow, double rfactorS, double rfactorP)
        {
            InvokeCallback(i => i.PowermeterRFactorsCalibChangedCallback(flow, rfactorS, rfactorP));
        }

        public void UpdatePowerLaser(double value)
        {
            InvokeCallback(i => i.PowerLaserChangedCallback(value));
        }

        public void UpdateInterlockStatus(string value)
        {
            InvokeCallback(i => i.InterlockStatusChangedCallback(value));
        }

        private void UpdateLaserTemperature(double value)
        {
            InvokeCallback(i => i.LaserTemperatureChangedCallback(value));
        }

        private void UpdatePsuTemperature(double value)
        {
            InvokeCallback(i => i.PsuTemperatureChangedCallback(value));
        }

        public void UpdateAttenuationPosition(double position)
        {
            InvokeCallback(i => i.AttenuationPositionChangedCallback(position));
        }

        public void UpdatePolarisationPosition(double position)
        {
            InvokeCallback(i => i.PolarisationPositionChangedCallback(position));
        }

        public void UpdatePolarisationAngleCalib(double polarAngleHsS, double polarAngleHsP, double polarAngleHtS, double polarAngleHtP)
        {
            InvokeCallback(i => i.PolarisationCalibConfigChangedCallback(polarAngleHsS, polarAngleHsP, polarAngleHtS, polarAngleHtP));
        }        

        public void UpdateShutterIrisPosition(string value)
        {
            InvokeCallback(i => i.ShutterIrisPositionChangedCallback(value));
        }

        public void UpdateMppcStateSignals(MppcCollector collector, MppcStateModule value)
        {
            InvokeCallback(i => i.MppcStateSignalsChangedCallback(collector, value));
        }

        public void UpdateMppcOutputVoltage(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.MppcOutputVoltageChangedCallback(collector, value));
        }

        public void UpdateMppcOutputCurrent(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.MppcOutputCurrentChangedCallback(collector, value));
        }

        public void UpdateMppcSensorTemperature(MppcCollector collector, double value)
        {
            InvokeCallback(i => i.MppcSensorTemperatureChangedCallback(collector, value));
        }        

        public void UpdateDataAttributes(List<DataAttribute> values)
        {
            InvokeCallback(i => i.UpdateDataAttributesCallback(values));
        }        
    }
}
