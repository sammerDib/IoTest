using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using UnitySC.PM.Shared.Hardware.Camera;
using UnitySC.PM.Shared.Hardware.Camera.DummyCamera;
using UnitySC.PM.Shared.Hardware.Camera.IDSCamera;
using UnitySC.PM.Shared.Hardware.Camera.MatroxCamera;
using UnitySC.PM.Shared.Hardware.Chamber;
using UnitySC.PM.Shared.Hardware.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Led;
using UnitySC.PM.Shared.Hardware.Mppc;
using UnitySC.PM.Shared.Hardware.OpticalPowermeter;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Controller;
using UnitySC.PM.Shared.Hardware.Service.Interface.DistanceSensor;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ffu;
using UnitySC.PM.Shared.Hardware.Service.Interface.Ionizer;
using UnitySC.PM.Shared.Hardware.Service.Interface.Laser;
using UnitySC.PM.Shared.Hardware.Service.Interface.Plc;
using UnitySC.PM.Shared.Hardware.Service.Interface.PlcScreen;
using UnitySC.PM.Shared.Hardware.Service.Interface.Rfid;
using UnitySC.PM.Shared.Hardware.Service.Interface.Shutter;
using UnitySC.PM.Shared.Hardware.Service.Interface.Spectrometer;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Common
{
    [Serializable]
    public class HardwareConfiguration
    {
        [XmlArrayItem(typeof(NSTAxesConfig))]
        [XmlArrayItem(typeof(TMAPAxesConfig))]
        [XmlArrayItem(typeof(PSDAxesConfig))]
        [XmlArrayItem(typeof(LSAxesConfig))]
        [XmlArrayItem(typeof(ArgosAxesConfig))]
        [XmlArrayItem(typeof(AerotechAxesConfig))]
        [XmlArrayItem(typeof(PhotoLumAxesConfig))]
        public List<AxesConfig> AxesConfigs { get; set; }

        [XmlArrayItem(typeof(AxesConfig))]
        [XmlArrayItem(typeof(LiseHfAxesConfig))]
        [XmlArrayItem(typeof(PSDAxesConfig))]
        [XmlArrayItem(typeof(PhotoLumAxesConfig))]
        public List<AxesConfig> MotionAxesConfigs { get; set; }

        [XmlArrayItem(typeof(ControllerConfig))]
        [XmlArrayItem(typeof(DummyMotionControllerConfig))]
        [XmlArrayItem(typeof(ThorlabsMotionControllerConfig))]
        [XmlArrayItem(typeof(OwisMotionControllerConfig))]
        [XmlArrayItem(typeof(ParallaxMotionControllerConfig))]
        [XmlArrayItem(typeof(IoMotionControllerConfig))]
        [XmlArrayItem(typeof(CNCMotionControllerConfig))]
        public List<ControllerConfig> MotionControllerConfigs { get; set; }

        [XmlArrayItem(typeof(DummyControllerConfig))]
        [XmlArrayItem(typeof(ACSControllerConfig))]
        [XmlArrayItem(typeof(AerotechControllerConfig))]
        [XmlArrayItem(typeof(ControllerConfig))]
        [XmlArrayItem(typeof(RCMControllerConfig))]
        [XmlArrayItem(typeof(MCCControllerConfig))]
        [XmlArrayItem(typeof(NICouplerControllerConfig))]
        [XmlArrayItem(typeof(BeckhoffPlcControllerConfig))]
        [XmlArrayItem(typeof(LaserPiano450ControllerConfig))]
        [XmlArrayItem(typeof(LaserSMD12ControllerConfig))]
        [XmlArrayItem(typeof(ShutterSh10pilControllerConfig))]
        [XmlArrayItem(typeof(PSDChamberControllerConfig))]
        [XmlArrayItem(typeof(EMEChamberControllerConfig))]
        [XmlArrayItem(typeof(PSDChuckControllerConfig))]
        [XmlArrayItem(typeof(EMEChuckControllerConfig))]
        [XmlArrayItem(typeof(FfuAstrofan612ControllerConfig))]
        [XmlArrayItem(typeof(RfidBisL405ControllerConfig))]
        [XmlArrayItem(typeof(ArduinoLightControllerConfig))]
        [XmlArrayItem(typeof(EvosensLightControllerConfig))]
        [XmlArrayItem(typeof(MicroEpsilonDistanceSensorControllerConfig))]
        [XmlArrayItem(typeof(IonizerKeyenceControllerConfig))]
        public List<ControllerConfig> ControllerConfigs { get; set; }

        public List<ChuckBaseConfig> USPChuckConfigs { get; set; }

        public LedConfig LedConfig { get; set; }

        public List<PlcConfig> PlcConfigs { get; set; }

        public List<ChamberConfig> ChamberConfigs { get; set; }

        [XmlArrayItem(typeof(VieworksVtMatroxCameraConfig))]
        [XmlArrayItem(typeof(M1280MatroxCameraConfig))]
        [XmlArrayItem(typeof(VX29MGMatroxCameraConfig))]
        [XmlArrayItem(typeof(VieworksVH16MG2M4MatroxCameraConfig))]
        [XmlArrayItem(typeof(VieworksVC65MXM31CameraConfig))]
        [XmlArrayItem(typeof(AVTGE4900CameraConfig))]
        [XmlArrayItem(typeof(AVTGE4400CameraConfig))]
        [XmlArrayItem(typeof(BaslerAce2ProCameraConfig))]
        [XmlArrayItem(typeof(VC155MXMatroxCameraConfig))]
        [XmlArrayItem(typeof(VC151MXMatroxCameraConfig))]
        [XmlArrayItem(typeof(MatroxCameraConfigBase))]
        [XmlArrayItem(typeof(UI524xCpNirIDSCameraConfig))]
        [XmlArrayItem(typeof(UI324xCpNirIDSCameraConfig))]
        [XmlArrayItem(typeof(IDSCameraConfigBase))]
        [XmlArrayItem(typeof(DummyCameraConfig))]
        [XmlArrayItem(typeof(ELIIXA16KMatroxCameraConfig))]
        [XmlArrayItem(typeof(Spyder3MatroxCameraConfig))]
        public List<CameraConfigBase> CameraConfigs { get; set; }
       
        public List<DistanceSensorConfig> DistanceSensorConfigs { get; set; }

        public List<LaserConfig> LaserConfigs { get; set; }

        [XmlArrayItem(typeof(C13336MppcConfig))]
        public List<MppcConfig> MppcConfigs { get; set; }

        [XmlArrayItem(typeof(PM101OpticalPowermeterConfig))]
        public List<OpticalPowermeterConfig> OpticalPowermeterConfigs { get; set; }

        public List<ShutterConfig> ShutterConfigs { get; set; }

        [XmlArrayItem(typeof(SpectrometerConfig))]
        public List<SpectrometerConfig> SpectrometerConfigs { get; set; }

        public List<LightModuleConfig> LightModuleConfigs { get; set; }

        public List<ScreenConfig> PlcScreenConfigs { get; set; }

        public List<FfuConfig> FfuConfigs { get; set; }

        public List<RfidConfig> RfidConfigs { get; set; }

        public List<IonizerConfig> IonizerConfigs { get; set; }

        public virtual void SetAllHardwareInSimulation()
        {
            var subDeviceConfigs = SubObjectFinder.GetAllSubObjectOfTypeT<IDeviceConfiguration>(this, 2);
            foreach (var deviceConfig in subDeviceConfigs)
            {
                deviceConfig.Value.IsSimulated = true;
            }
        }
    }
}
