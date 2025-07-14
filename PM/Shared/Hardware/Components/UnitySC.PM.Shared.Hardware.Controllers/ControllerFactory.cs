using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.CNC;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.IO;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.Owis;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Axes.PI.Dummy;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chambers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Chuck;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.CNC;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ffus;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Ionizers;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Laser;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Light;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Owis;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Parallax;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Phytron;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Plc;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Reliance;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Rfids;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Screens;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Shutters;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Spectrometer;
using UnitySC.PM.Shared.Hardware.Controllers.Controllers.Thorlabs;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.Hardware.Controllers
{
    public static class ControllerFactory
    {
        public static ControllerBase CreateController(ControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            return config.IsSimulated ? CreateSimulatedController(config, globalStatusServer, logger) : CreateNominalController(config, globalStatusServer, logger);
        }

        private static ControllerBase CreateSimulatedController(ControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            switch (config)
            {
                case ACSControllerConfig acsConfig: return new ACSDummyController(acsConfig, globalStatusServer, logger);
                case NICouplerControllerConfig nICouplerControllerConfig: return new NICouplerDummyController(nICouplerControllerConfig, globalStatusServer, logger);
                case BeckhoffPlcControllerConfig beckhoffPlcConfig: return new BeckhoffPlcDummyController(beckhoffPlcConfig, globalStatusServer, logger);
                case LaserSMD12ControllerConfig laserSMD12Config: return new SMD12LaserDummyController(laserSMD12Config, globalStatusServer, logger);
                case LaserPiano450ControllerConfig laserPiano450Config: return new Piano450LaserDummyController(laserPiano450Config, globalStatusServer, logger);
                case ShutterSh10pilControllerConfig shutterSh10pilConfig: return new Sh10pilShutterDummyController(shutterSh10pilConfig, globalStatusServer, logger);
                case ScreenDensitronDM430GNControllerConfig densitronDM640GNControllerConfig: return new DummyScreenController(densitronDM640GNControllerConfig, globalStatusServer, logger);
                case ArduinoLightControllerConfig arduinoLightConfig: return new DummyArduinoLightController(arduinoLightConfig, globalStatusServer, logger);
                case EvosensLightControllerConfig evosensLightConfig: return new DummyEvosensLightController(evosensLightConfig, globalStatusServer, logger);
                case MicroEpsilonDistanceSensorControllerConfig microEpsilonDistanceSensorConfig: return new DistanceSensorDummyController(microEpsilonDistanceSensorConfig, globalStatusServer, logger);
                case FfuAstrofan612ControllerConfig ffuControllerConfig: return new DummyFfuController(ffuControllerConfig, globalStatusServer, logger);
                case PSDChamberControllerConfig psdChamberConfig: return new ChamberDummyController(psdChamberConfig, globalStatusServer, logger);
                case EMEChamberControllerConfig emeChamberConfig: return new ChamberDummyController(emeChamberConfig, globalStatusServer, logger);
                case PSDChuckControllerConfig psdChuckConfig: return new IoChuckDummyController(psdChuckConfig, globalStatusServer, logger);
                case EMEChuckControllerConfig emeChuckConfig: return new IoChuckDummyController(emeChuckConfig, globalStatusServer, logger);
                case RfidBisL405ControllerConfig rfidConfig: return new DummyRfidController(rfidConfig, globalStatusServer, logger);
                case IonizerKeyenceControllerConfig ionizerConfig: return new KeyenceIonizerDummyController(ionizerConfig, globalStatusServer, logger);
                case OpcControllerConfig opcControllerConfig: return new OpcDummyController(opcControllerConfig, globalStatusServer, logger);
                case SpectrometerAvantesControllerConfig avantesControllerConfig: return new SpectrometerDummyController(avantesControllerConfig, globalStatusServer, logger);
                case ACSLightControllerConfig acsLightConfig: return new DummyLightController(acsLightConfig, globalStatusServer, logger);
                case ENTTECLightControllerConfig enttecLightConfig: return new DummyLightController(enttecLightConfig, globalStatusServer, logger);
                case PIE709ControllerConfig piezoConfig: return new PIE709DummyController(piezoConfig, globalStatusServer, logger);                

                default: return null;
            }
        }

        private static ControllerBase CreateNominalController(ControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            switch (config)
            {
                case ACSControllerConfig acsConfig: return new ACSController(acsConfig, globalStatusServer, logger);
                case PIE709ControllerConfig piezoConfig: return new PIE709Controller(piezoConfig, globalStatusServer, logger);
                case SMC100ControllerConfig smc100Config: return new SMC100Controller(smc100Config, globalStatusServer, logger);
                case MCCControllerConfig mccConfig: return new MCCController(mccConfig, globalStatusServer, logger);
                case RCMControllerConfig rcmConfig: return new RCMController(rcmConfig, globalStatusServer, logger);
                case AerotechControllerConfig aerotechConfig: return new AerotechController(aerotechConfig, globalStatusServer, logger);
                case NICouplerControllerConfig rackNiConfig: return new NICouplerController(rackNiConfig, globalStatusServer, logger);
                case BeckhoffPlcControllerConfig beckhoffPlcConfig: return new BeckhoffPlcController(beckhoffPlcConfig, globalStatusServer, logger);
                case LaserPiano450ControllerConfig laserPiano450Config: return new Piano450LaserController(laserPiano450Config, globalStatusServer, logger);
                case LaserSMD12ControllerConfig laserSMD12Config: return new SMD12LaserController(laserSMD12Config, globalStatusServer, logger);
                case ShutterSh10pilControllerConfig shutterSh10pilConfig: return new Sh10pilShutterController(shutterSh10pilConfig, globalStatusServer, logger);
                case SpectrometerAvantesControllerConfig spectroConfig: return new SpectrometerAVSController(spectroConfig, globalStatusServer, logger);
                case PSDChamberControllerConfig psdChamberConfig: return new PSDChamberController(psdChamberConfig, globalStatusServer, logger);
                case EMEChamberControllerConfig emeChamberConfig: return new EMEChamberController(emeChamberConfig, globalStatusServer, logger);
                case PSDChuckControllerConfig psdChuckConfig: return new PSDChuckController(psdChuckConfig, globalStatusServer, logger);
                case EMEChuckControllerConfig emeChuckConfig: return new EMEChuckController(emeChuckConfig, globalStatusServer, logger);
                case ACSLightControllerConfig acsLightConfig: return new ACSLightController(acsLightConfig, globalStatusServer, logger);
                case ENTTECLightControllerConfig enttecLightConfig: return new ENTTECLightController(enttecLightConfig, globalStatusServer, logger);
                case ArduinoLightControllerConfig arduinoLightConfig: return new ArduinoLightController(arduinoLightConfig, globalStatusServer, logger);
                case EvosensLightControllerConfig evosensLightConfig: return new EvosensLightController(evosensLightConfig, globalStatusServer, logger);
                case ScreenDensitronDM430GNControllerConfig densitronDM640GNConfig: return new DensitronDM430GNScreenController(densitronDM640GNConfig, globalStatusServer, logger);
                case FfuAstrofan612ControllerConfig ffuAstrofanConfig: return new Astrofan612FfuController(ffuAstrofanConfig, globalStatusServer, logger);
                case RfidBisL405ControllerConfig rfidConfig: return new BisL405RfidController(rfidConfig, globalStatusServer, logger);
                case MicroEpsilonDistanceSensorControllerConfig microEpsilonDistanceSensorConfig: return new MicroEpsilonDistanceSensorController(microEpsilonDistanceSensorConfig, globalStatusServer, logger);
                case IonizerKeyenceControllerConfig ionizerConfig: return new KeyenceIonizerController(ionizerConfig, globalStatusServer, logger);
                default: return null;
            }
        }

        #region Motion controller

        public static MotionControllerBase CreateMotionController(ControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            return config.IsSimulated ? CreateSimulatedMotionController(config, globalStatusServer, logger) : CreateNominalMotionController(config, globalStatusServer, logger);
        }

        private static MotionControllerBase CreateSimulatedMotionController(ControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            switch (config)
            {
                case DummyMotionControllerConfig DummyMotionConfig: return new DummyMotionController(DummyMotionConfig, globalStatusServer, logger);
                case ThorlabsMotionControllerConfig thorlabsMotionConfig: return new ThorlabsMotionDummyController(thorlabsMotionConfig, globalStatusServer, logger);
                case OwisMotionControllerConfig owisMotionConfig: return new OwisMotionDummyController(owisMotionConfig, globalStatusServer, logger);
                case ParallaxMotionControllerConfig parallaxMotionConfig: return new ParallaxMotionDummyController(parallaxMotionConfig, globalStatusServer, logger);
                case IoMotionControllerConfig ioMotionConfig: return new DummyPSDIoMotionController(ioMotionConfig, globalStatusServer, logger);
                case AerotechControllerConfig aerotechControllerConfig: return new AerotechMotionDummyController(aerotechControllerConfig, globalStatusServer, logger);
                case CNCMotionControllerConfig cncControllerConfig: return new CNCMotionDummyController(cncControllerConfig, globalStatusServer, logger);
                default: return null;
            }
        }

        private static MotionControllerBase CreateNominalMotionController(ControllerConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
        {
            switch (config)
            {
                case ThorlabsMotionControllerConfig thorlabsMotionConfig: return new ThorlabsMotionController(thorlabsMotionConfig, globalStatusServer, logger);
                case OwisMotionControllerConfig owisMotionConfig: return new OwisMotionController(owisMotionConfig, globalStatusServer, logger);
                case ParallaxMotionControllerConfig parallaxMotionConfig: return new ParallaxMotionController(parallaxMotionConfig, globalStatusServer, logger);
                case IoMotionControllerConfig ioMotionConfig: return new IoMotionController(ioMotionConfig, globalStatusServer, logger);
                case AerotechControllerConfig aerotechControllerConfig: return new AerotechMotionController(aerotechControllerConfig, globalStatusServer, logger);
                case CNCMotionControllerConfig cncControllerConfig: return new CNCMotionController(cncControllerConfig, globalStatusServer, logger);
                default: return null;
            }
        }

        #endregion Motion controller
    }
}
