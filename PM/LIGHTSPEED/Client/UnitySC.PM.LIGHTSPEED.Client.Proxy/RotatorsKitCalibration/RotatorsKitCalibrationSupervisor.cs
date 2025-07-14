using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using GalaSoft.MvvmLight.Messaging;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.LIGHTSPEED.Client.Proxy.RotatorsKitCalibration
{
    public class RotatorsKitCalibrationSupervisor : IRotatorsKitCalibrationService, IRotatorsKitCalibrationServiceCallback
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private DuplexServiceInvoker<IRotatorsKitCalibrationService> _rotatorsKitCalibrationService;
        private IMessenger _messenger;

        public RotatorsKitCalibrationSupervisor(ILogger<RotatorsKitCalibrationSupervisor> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _instanceContext = new InstanceContext(this);
            _rotatorsKitCalibrationService = new DuplexServiceInvoker<IRotatorsKitCalibrationService>(_instanceContext, "RotatorsKitCalibrationService", ClassLocator.Default.GetInstance<ILogger<IRotatorsKitCalibrationService>>(), _messenger);

            SubscribeToChanges();
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.SubscribeToChanges());
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.UnSubscribeToChanges());
        }

        public Response<VoidResult> PowerOn()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowerOn());
        }

        public Response<VoidResult> PowerOff()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowerOff());
        }

        public Response<VoidResult> SetPower(int power)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.SetPower(power));
        }

        public Response<VoidResult> SetCurrent(int current)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.SetCurrent(current));
        }

        public Response<VoidResult> MoveAbsPosition(BeamShaperFlow beamShaperFlow, double position)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.MoveAbsPosition(beamShaperFlow, position));
        }

        public Response<VoidResult> HomePosition(BeamShaperFlow beamShaperFlow)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.HomePosition(beamShaperFlow));
        }

        public Response<VoidResult> OpenShutterCommand()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.OpenShutterCommand());
        }

        public Response<VoidResult> CloseShutterCommand()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.CloseShutterCommand());
        }

        public Response<VoidResult> InitializeUpdate()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.InitializeUpdate());
        }

        public Response<VoidResult> AttenuationRefresh()
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.AttenuationRefresh());
        }

        public Response<VoidResult> MppcPowerOn(MppcCollector collector, bool powerOn)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.MppcPowerOn(collector, powerOn));
        }

        public Response<VoidResult> MppcSetOutputVoltage(MppcCollector collector, double voltage)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.MppcSetOutputVoltage(collector, voltage));
        }

        public Response<VoidResult> MppcManageRelay(MppcCollector collector, bool relayActivated)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.MppcManageRelay(collector, relayActivated));
        }

        public Response<VoidResult> SetIoValue(string identifier, string name, bool value)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.SetIoValue(identifier, name, value));
        }

        public Response<VoidResult> GetIoValue(string identifier, string name)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.GetIoValue(identifier, name));
        }

        public Response<VoidResult> PowermeterEnableAutoRange(PowerIlluminationFlow flow, bool activate)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowermeterEnableAutoRange(flow, activate));
        }

        public Response<VoidResult> PowermeterRangesVariation(PowerIlluminationFlow flow, string range)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowermeterRangesVariation(flow, range));
        }

        public Response<VoidResult> PowermeterStartDarkAdjust(PowerIlluminationFlow flow)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowermeterStartDarkAdjust(flow));
        }

        public Response<VoidResult> PowermeterCancelDarkAdjust(PowerIlluminationFlow flow)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowermeterCancelDarkAdjust(flow));
        }

        public Response<VoidResult> PowermeterEditResponsivity(PowerIlluminationFlow flow, double responsivity_mA_W)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowermeterEditResponsivity(flow,  responsivity_mA_W));
        }

        public Response<VoidResult> PowermeterEditRFactorsCalib(PowerIlluminationFlow flow, double rfactorS, double rfactorP)
        {
            return _rotatorsKitCalibrationService.InvokeAndGetMessages(s => s.PowermeterEditRFactorsCalib(flow, rfactorS, rfactorP));
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterPowerChangedCallback(PowerIlluminationFlow flow, double power, double powerCal_mW, double rfactor)
        {
            _messenger.Send(new PowermeterPowerChangedMessage() { Flow = flow, Power = power, PowerCal_mW = powerCal_mW, RFactor = rfactor });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterCurrentChangedCallback(PowerIlluminationFlow flow, double current_mA)
        {
            _messenger.Send(new PowermeterCurrentChangedMessage() { Flow = flow, Current_mA = current_mA});
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterIdentifierChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new PowermeterIdentifierChangedMessage() { Flow = flow, Identifier = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterSensorTypeChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new PowermeterSensorTypeChangedMessage() { Flow = flow, SensorType = value});
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterSensorAttenuationChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new PowermeterSensorAttenuationChangedMessage() { Flow = flow, SensorAttenuation = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterWavelengthChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new PowermeterWavelengthChangedMessage() { Flow = flow, Wavelength = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterRangeChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new PowermeterRangesChangedMessage() { Flow = flow, PowermeterRange = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterBeamDiameterChangedCallback(PowerIlluminationFlow flow, uint value)
        {
            _messenger.Send(new PowermeterBeamDiameterChangedMessage() { Flow = flow, BeamDiameter = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterDarkAdjustStateChangedCallback(PowerIlluminationFlow flow, string value)
        {
            _messenger.Send(new PowermeterDarkAdjustStateChangedMessage() { Flow = flow, DarkAdjustState = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterDarkOffsetChangedCallback(PowerIlluminationFlow flow, double value)
        {
            _messenger.Send(new PowermeterDarkOffsetChangedMessage() { Flow = flow, DarkOffset = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterResponsivityChangedCallback(PowerIlluminationFlow flow, double value)
        {
            _messenger.Send(new PowermeterResponsivityChangedMessage() { Flow = flow, Responsivity = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PowermeterRFactorsCalibChangedCallback(PowerIlluminationFlow flow, double rfactorS, double rfactorP)
        {
            _messenger.Send(new PowermeterRFactorsCalibChangedMessage() { Flow = flow, RFactorS = rfactorS, RFactorP = rfactorP });
        }

        void IRotatorsKitCalibrationServiceCallback.PowerLaserChangedCallback(double power)
        {
            _messenger.Send(new PowerLaserChangedMessage() { Power = power });
        }

        void IRotatorsKitCalibrationServiceCallback.InterlockStatusChangedCallback(string value)
        {
            _messenger.Send(new InterlockStatusChangedMessage() { InterlockStatus = value });
        }

        void IRotatorsKitCalibrationServiceCallback.LaserTemperatureChangedCallback(double value)
        {
            _messenger.Send(new LaserTemperatureChangedMessage() { LaserTemperature = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PsuTemperatureChangedCallback(double value)
        {
            _messenger.Send(new PsuTemperatureChangedMessage() { PsuTemperature = value });
        }

        void IRotatorsKitCalibrationServiceCallback.AttenuationPositionChangedCallback(double value)
        {
            _messenger.Send(new AttenuationPositionChangedMessage() { AttenuationPosition = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PolarisationPositionChangedCallback(double value)
        {
            _messenger.Send(new PolarisationPositionChangedMessage() { PolarisationPosition = value });
        }

        void IRotatorsKitCalibrationServiceCallback.PolarisationCalibConfigChangedCallback(double polarAngleHsS, double polarAngleHsP, double polarAngleHtS, double polarAngleHtP)
        {
            _messenger.Send(new PolarisationAngleCalibChangedMessage() { PolarAngleHsS = polarAngleHsS, PolarAngleHsP = polarAngleHsP, PolarAngleHtS = polarAngleHtS, PolarAngleHtP = polarAngleHtP });
        }

        void IRotatorsKitCalibrationServiceCallback.ShutterIrisPositionChangedCallback(string value)
        {
            _messenger.Send(new ShutterIrisPositionChangedMessage() { ShutterIrisPosition = value });
        }

        void IRotatorsKitCalibrationServiceCallback.MppcStateSignalsChangedCallback(MppcCollector collector, MppcStateModule value)
        {
            _messenger.Send(new MppcStateSignalsChangedMessage() { Collector = collector, StateSignals = value });
        }

        void IRotatorsKitCalibrationServiceCallback.MppcOutputVoltageChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new MppcOutputVoltageChangedMessage() { Collector = collector, OutputVoltage = value });
        }

        void IRotatorsKitCalibrationServiceCallback.MppcOutputCurrentChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new MppcOutputCurrentChangedMessage() { Collector = collector, OutputCurrent = value });
        }

        void IRotatorsKitCalibrationServiceCallback.MppcSensorTemperatureChangedCallback(MppcCollector collector, double value)
        {
            _messenger.Send(new MppcSensorTemperatureChangedMessage() { Collector = collector, SensorTemperature = value });
        }        

        void IRotatorsKitCalibrationServiceCallback.UpdateDataAttributesCallback(List<DataAttribute> values)
        {
            _messenger.Send(new DataAttributesChangedMessages() { DataAttributes = values });
        }        
    }
}
