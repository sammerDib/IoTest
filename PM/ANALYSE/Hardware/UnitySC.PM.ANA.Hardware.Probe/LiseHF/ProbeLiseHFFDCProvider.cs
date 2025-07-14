using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.Status.Service.Interface;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Tools;

using UnitySCPMSharedAlgosLiseHFWrapper;

namespace UnitySC.PM.ANA.Hardware.Probe.LiseHF
{
    

    public class ProbeLiseHFFDCProvider : IFDCProvider
    {
        public enum ObjectiveSuffix
        {
            x5NIR,
            x10NIR,
            x20NIR,
            x50NIR,
            x50VIS,
        };

        // WARNING we are using reflection here folliwings FielNames are parts of fdcNaming
        private struct SpotObjectiveFDC
        {
            public double OffsetDeviationX;
            public double OffsetDeviationY;
            public double Diameter;
            public double Shape;
            public int Intensity;
        }

        private struct CalibRefObjectiveFDC
        {
            public double Zone1;
            public double Zone2;
            public double Zone3;
            public double Zone4;
            public double Zone5;
        }
        private readonly FDCManager _fdcManager;
        private readonly IGlobalStatusServer _globalStatusServer;

        #region NonPersitentData
        private double _fdcLiseHFDarkCalibrationMax;
        private double _fdcLiseHFDarkCalibrationNormalisedEnergy;
        private double _fdcLiseHFRefCalibrationMax;
        private double _fdcLiseHFRefCalibrationNormalisedEnergy;
        private double _fdcLiseHFSignalZeroY;
        private double _fdcLiseHFTSVDepthPeakX;
        private double _fdcLiseHFTSVDepthPeakY;
        private double _fdcLiseHFSpotOffsetDeviationX;
        private double _fdcLiseHFSpotOffsetDeviationY;
        private double _fdcLiseHFLaserTemperature;    

        private Dictionary<ObjectiveSuffix, SpotObjectiveFDC> _fdcLiseHFSpotByObjectiveID;

        private Dictionary<ObjectiveSuffix, CalibRefObjectiveFDC> _fdcLiseHFRefCalibSTD_ByObjectiveID;
        private Dictionary<ObjectiveSuffix, CalibRefObjectiveFDC> _fdcLiseHFRefCalibLOW_ByObjectiveID;
        #endregion NonPersitentData

        #region PersitentData
        private PersitentFDCCountdown _pfdcLiseHFLightMaintenanceCountdown;
        #endregion PersitentData

        private const string ANA_LiseHFDarkCalibrationMax = "ANA_LiseHFDarkCalibrationMax";
        private const string ANA_LiseHFDarkCalibrationNormalisedEnergy = "ANA_LiseHFDarkCalibrationNormalisedEnergy";
        private const string ANA_LiseHFRefCalibrationMax = "ANA_LiseHFRefCalibrationMax";
        private const string ANA_LiseHFRefCalibrationNormalisedEnergy = "ANA_LiseHFRefCalibrationNormalisedEnergy";
        private const string ANA_LiseHFSignalZeroY = "ANA_LiseHFSignalZeroY";
        private const string ANA_LiseHFTSVDepthPeakX = "ANA_LiseHFTSVDepthPeakX";
        private const string ANA_LiseHFTSVDepthPeakY = "ANA_LiseHFTSVDepthPeakY";
        private const string ANA_LiseHFSpotOffsetDeviationX = "ANA_LiseHFSpotOffsetDeviationX";
        private const string ANA_LiseHFSpotOffsetDeviationY = "ANA_LiseHFSpotOffsetDeviationY";

        private const string ANA_LiseHFLaserTemperature = "ANA_LiseHFLaserTemperature";

        private const string ANA_LiseHFLightMaintenanceCountdown = "ANA_LiseHFLightMaintenanceCountdown";

        private const string SpotByObjectivePrefix = "ANA_LiseHFObjSpot"; // use as prefix for multples FDC from SpotObjectiveFDC structure
        private const string RefCalibSTD_ByObjectivePrefix = "ANA_LiseHFObjRefCalib_Std_"; // use as prefix for multples FDC from CalibRefObjectiveFDC structure with Standard attenuation filter
        private const string RefCalibLOW_ByObjectivePrefix = "ANA_LiseHFObjRefCalib_Low_"; // use as prefix for multples FDC from CalibRefObjectiveFDC structure with Low Illumination attenuation filter



        private readonly List<string> _providerFDCNames = new List<string>();

        public ProbeLiseHFFDCProvider()
        {
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
            _globalStatusServer = ClassLocator.Default.GetInstance<IGlobalStatusServer>();

            _fdcLiseHFSpotByObjectiveID = new Dictionary<ObjectiveSuffix, SpotObjectiveFDC>();

            _fdcLiseHFRefCalibSTD_ByObjectiveID = new Dictionary<ObjectiveSuffix, CalibRefObjectiveFDC>();
            _fdcLiseHFRefCalibLOW_ByObjectiveID = new Dictionary<ObjectiveSuffix, CalibRefObjectiveFDC>();

            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();           
            messenger.Register<PM.Shared.Hardware.Service.Interface.Laser.LaserQuantum.LaserTemperatureMessage>(this, (r, m) => { OnLaserTemperatureChanged(m.LaserTemperature); });            
            messenger.Register<PM.Shared.Hardware.Service.Interface.Laser.Leukos.LaserTemperatureMessage>(this, (r, m) => { OnLaserTemperatureChanged(m.LaserTemperature); });

        }

        ~ProbeLiseHFFDCProvider()
        {
            IMessenger messenger = ClassLocator.Default.GetInstance<IMessenger>();
            messenger.Unregister<PM.Shared.Hardware.Service.Interface.Laser.LaserQuantum.LaserTemperatureMessage>(this);
            messenger.Unregister<PM.Shared.Hardware.Service.Interface.Laser.Leukos.LaserTemperatureMessage>(this);
        }

        private void OnLaserTemperatureChanged(double temperatureValue)
        {
            _fdcLiseHFLaserTemperature = temperatureValue;
        }
        

        public FDCData GetFDC(string fdcName)
        {
            FDCData fdc = null;

            switch (fdcName)
            {
                case ANA_LiseHFDarkCalibrationMax:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFDarkCalibrationMax);
                    break;

                case ANA_LiseHFDarkCalibrationNormalisedEnergy:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFDarkCalibrationNormalisedEnergy);
                    break;

                case ANA_LiseHFRefCalibrationMax:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFRefCalibrationMax);
                    break;

                case ANA_LiseHFRefCalibrationNormalisedEnergy:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFRefCalibrationNormalisedEnergy);
                    break;

                case ANA_LiseHFSignalZeroY:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFSignalZeroY);
                    break;

                case ANA_LiseHFTSVDepthPeakX:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFTSVDepthPeakX);
                    break;

                case ANA_LiseHFTSVDepthPeakY:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFTSVDepthPeakY);
                    break;

                case ANA_LiseHFSpotOffsetDeviationX:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFSpotOffsetDeviationX);
                    break;

                case ANA_LiseHFSpotOffsetDeviationY:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFSpotOffsetDeviationY);
                    break;

                case ANA_LiseHFLightMaintenanceCountdown:
                    fdc = FDCData.MakeNew(fdcName, _pfdcLiseHFLightMaintenanceCountdown.CountdownHours, "h");
                    break;

                case ANA_LiseHFLaserTemperature:
                    fdc = FDCData.MakeNew(fdcName, _fdcLiseHFLaserTemperature); 
                    break;

                default:
                    fdc = HandleOtherFDCs(fdcName);
                    break;
            }

            return fdc;
        }

        private FDCData HandleOtherFDCs(string fdcName)
        {
            FDCData fdc = null;
            // SPOT OFFSET FDC BY OBJECTIVE
            if (fdcName.StartsWith(SpotByObjectivePrefix))
            {
                var suffix = fdcName.Split('_').Last();
                if (!Enum.TryParse<ObjectiveSuffix>(suffix, out ObjectiveSuffix objsuffix))
                {
                    return null;
                }
                if (!_fdcLiseHFSpotByObjectiveID.TryGetValue(objsuffix, out SpotObjectiveFDC spotObjFdc))
                {
                    return null;
                }

                // extract fieldname from fdcname
                var fieldName = fdcName.Remove(fdcName.Length - (suffix.Length + 1)).Substring(SpotByObjectivePrefix.Length);
                FieldInfo field = typeof(SpotObjectiveFDC).GetField(fieldName);
                if (field != null)
                {
                    var obj = field.GetValue(spotObjFdc);
                    if (obj != null)
                    {
                        if (obj is double dblValue)
                            fdc = FDCData.MakeNew(fdcName, dblValue);
                        else if (obj is int intValue)
                            fdc = FDCData.MakeNew(fdcName, intValue);
                    }
                }
            }
            // Reference Calibration UOH - STD or LOW ILLUM - BY OBJECTIVE
            else if (fdcName.StartsWith(RefCalibSTD_ByObjectivePrefix) || fdcName.StartsWith(RefCalibLOW_ByObjectivePrefix))
            {
                var splits = fdcName.Split('_');
                var suffix = splits.Last();
                if (!Enum.TryParse<ObjectiveSuffix>(suffix, out ObjectiveSuffix objsuffix))
                {
                    return null;
                }

                CalibRefObjectiveFDC refCalibObjFdc;
                string prefix;
                switch (splits[2])
                {
                    case "Std":
                        if (!_fdcLiseHFRefCalibSTD_ByObjectiveID.TryGetValue(objsuffix, out refCalibObjFdc))
                        {
                            return null;
                        }
                        prefix = RefCalibSTD_ByObjectivePrefix;
                        break;
                    case "Low":
                        if (!_fdcLiseHFRefCalibLOW_ByObjectiveID.TryGetValue(objsuffix, out refCalibObjFdc))
                        {
                            return null;
                        }
                        prefix = RefCalibLOW_ByObjectivePrefix;
                        break;
                    default:
                        return null;
                }

                // extract fieldname from fdcname
                var fieldName = fdcName.Remove(fdcName.Length - (suffix.Length + 1)).Substring(prefix.Length);
                FieldInfo field = typeof(CalibRefObjectiveFDC).GetField(fieldName);
                if (field != null)
                {
                    var obj = field.GetValue(refCalibObjFdc);
                    if (obj != null)
                    {
                        if (obj is double dblValue)
                            fdc = FDCData.MakeNew(fdcName, dblValue);
                    }
                }
            }
           
            return fdc;
        }

        public void Register()
        {
            InitProviderFDCNames();
            _fdcManager.RegisterFDCProvider(this, _providerFDCNames);
        }

        public void ResetFDC(string fdcName)
        {
            switch (fdcName)
            {
                case ANA_LiseHFLightMaintenanceCountdown:
                    _pfdcLiseHFLightMaintenanceCountdown.ResetDate = DateTime.Now;
                    _fdcManager.SetPersistentFDCData(_pfdcLiseHFLightMaintenanceCountdown);
                    break;

                default:
                    break;
            }
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            switch (fdcName)
            {
                case ANA_LiseHFLightMaintenanceCountdown:
                    // for now only handle hours - here see in the future how to handle différent kind of unit (on custommer request)
                    _pfdcLiseHFLightMaintenanceCountdown.InitialCountTime = new TimeSpan((int)initvalue, 0, 0);
                    _fdcManager.SetPersistentFDCData(_pfdcLiseHFLightMaintenanceCountdown);
                    break;

                default:
                    break;
            }
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
           
            switch (fdcName)
            {
                case ANA_LiseHFLightMaintenanceCountdown:
                    if (persistentFDCData is PersitentFDCCountdown pfdcLiseHFLightMaintenanceCountdown)
                        _pfdcLiseHFLightMaintenanceCountdown = pfdcLiseHFLightMaintenanceCountdown;
                    break;

                default:
                    break;
            }
        }

        public void StartFDCMonitor()
        {
            if (_pfdcLiseHFLightMaintenanceCountdown == null)
            {
                var fdcItemConfig = _fdcManager.GetFDCsConfig();
                var countdownconfig = fdcItemConfig?.Find(x => x.Name == ANA_LiseHFLightMaintenanceCountdown) ?? new FDCItemConfig() { InitValue = 50000.0};
                // if FDC is not well define create fdc with some defaulft value
                double initialCountTimeHours = countdownconfig.InitValue; 
                TimeSpan initialCountTime = new TimeSpan((int)initialCountTimeHours, 0, 0);
                _pfdcLiseHFLightMaintenanceCountdown = new PersitentFDCCountdown(ANA_LiseHFLightMaintenanceCountdown, DateTime.Now, initialCountTime);
            }
            _fdcManager.SetPersistentFDCData(_pfdcLiseHFLightMaintenanceCountdown);

            var fdcs = new List<FDCData>()
            {
                FDCData.MakeNew(ANA_LiseHFLightMaintenanceCountdown, _pfdcLiseHFLightMaintenanceCountdown.CountdownHours, "h")
            };

            _fdcManager.SendFDCs(fdcs);

            // if needed launch  FDC monitoring thread here
        }

        private void InitProviderFDCNames()
        {
            _providerFDCNames.Add(ANA_LiseHFDarkCalibrationMax);
            _providerFDCNames.Add(ANA_LiseHFDarkCalibrationNormalisedEnergy);
            _providerFDCNames.Add(ANA_LiseHFRefCalibrationMax);
            _providerFDCNames.Add(ANA_LiseHFRefCalibrationNormalisedEnergy);
            _providerFDCNames.Add(ANA_LiseHFSignalZeroY);
            _providerFDCNames.Add(ANA_LiseHFTSVDepthPeakX);
            _providerFDCNames.Add(ANA_LiseHFTSVDepthPeakY);
            _providerFDCNames.Add(ANA_LiseHFSpotOffsetDeviationX);
            _providerFDCNames.Add(ANA_LiseHFSpotOffsetDeviationY);
            _providerFDCNames.Add(ANA_LiseHFLaserTemperature);

            _providerFDCNames.Add(ANA_LiseHFLightMaintenanceCountdown);

            InitSpotDicoFDC();

            InitRefCalibDicoFDC();
        }

        private void InitSpotDicoFDC()
        {
            // Here select objective we want to monitor for spots
            //_fdcLiseHFSpotByObjectiveID.Add(ObjectiveSuffix.x2VIS, new SpotObjectiveFDC());
            _fdcLiseHFSpotByObjectiveID.Add(ObjectiveSuffix.x5NIR , new SpotObjectiveFDC());
            _fdcLiseHFSpotByObjectiveID.Add(ObjectiveSuffix.x10NIR, new SpotObjectiveFDC());
            _fdcLiseHFSpotByObjectiveID.Add(ObjectiveSuffix.x20NIR, new SpotObjectiveFDC());
            _fdcLiseHFSpotByObjectiveID.Add(ObjectiveSuffix.x50NIR, new SpotObjectiveFDC());
            _fdcLiseHFSpotByObjectiveID.Add(ObjectiveSuffix.x50VIS, new SpotObjectiveFDC());

            FieldInfo[] fields = typeof(SpotObjectiveFDC).GetFields();
            foreach (var objSuffix in _fdcLiseHFSpotByObjectiveID.Keys)
            {
                foreach (var field in fields)
                {
                    _providerFDCNames.Add($"{SpotByObjectivePrefix}{field.Name}_{objSuffix}");
                }
            }
        }

        private void InitRefCalibDicoFDC()
        {
            FieldInfo[] fields = typeof(CalibRefObjectiveFDC).GetFields();

            // STANDARD ILLUM
            // Here select objective we want to monitor for Calibration reference signal Zones with Standard Illumination

            //_fdcLiseHFRefCalibSTD_ByObjectiveID.Add(ObjectiveSuffix.x2VIS, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibSTD_ByObjectiveID.Add(ObjectiveSuffix.x5NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibSTD_ByObjectiveID.Add(ObjectiveSuffix.x10NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibSTD_ByObjectiveID.Add(ObjectiveSuffix.x20NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibSTD_ByObjectiveID.Add(ObjectiveSuffix.x50NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibSTD_ByObjectiveID.Add(ObjectiveSuffix.x50VIS, new CalibRefObjectiveFDC());

         
            foreach (var objSuffix in _fdcLiseHFRefCalibSTD_ByObjectiveID.Keys)
            {
                foreach (var field in fields)
                {
                    _providerFDCNames.Add($"{RefCalibSTD_ByObjectivePrefix}{field.Name}_{objSuffix}");
                }
            }

            // LOW ILLUM

            // Here select objective we want to monitor for Calibration reference signal Zones with Standard Illumination
            //_fdcLiseHFRefCalibLOW_ByObjectiveID.Add(ObjectiveSuffix.x2VIS, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibLOW_ByObjectiveID.Add(ObjectiveSuffix.x5NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibLOW_ByObjectiveID.Add(ObjectiveSuffix.x10NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibLOW_ByObjectiveID.Add(ObjectiveSuffix.x20NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibLOW_ByObjectiveID.Add(ObjectiveSuffix.x50NIR, new CalibRefObjectiveFDC());
            _fdcLiseHFRefCalibLOW_ByObjectiveID.Add(ObjectiveSuffix.x50VIS, new CalibRefObjectiveFDC());

            foreach (var objSuffix in _fdcLiseHFRefCalibLOW_ByObjectiveID.Keys)
            {
                foreach (var field in fields)
                {
                    _providerFDCNames.Add($"{RefCalibLOW_ByObjectivePrefix}{field.Name}_{objSuffix}");
                }
            }
        }

        public void CreateFDCLiseHFDarkCalibration(string objectiveID, bool isLowIlluminationPower, RawCalibrationSignal rawCalibrationSignal)
        {
            if (_globalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcLiseHFDarkCalibrationMax = rawCalibrationSignal?.ComputeRawSignalMax() ?? 0.0;
                _fdcLiseHFDarkCalibrationNormalisedEnergy = rawCalibrationSignal?.ComputeNormalisedEnergy() ?? 0.0;

                var fdcList = new List<FDCData>()
                {
                    FDCData.MakeNew(ANA_LiseHFDarkCalibrationMax, _fdcLiseHFDarkCalibrationMax),
                    FDCData.MakeNew(ANA_LiseHFDarkCalibrationNormalisedEnergy, _fdcLiseHFDarkCalibrationNormalisedEnergy),
                };

                _fdcManager.SendFDCs(fdcList);
            }
        }

        public void CreateFDCLiseHFRefCalibration(string objectiveID, bool isLowIlluminationPower, RawCalibrationSignal rawCalibrationSignal)
        {
            if (_globalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcLiseHFRefCalibrationMax = rawCalibrationSignal?.ComputeRawSignalMax() ?? 0.0;
                _fdcLiseHFRefCalibrationNormalisedEnergy = rawCalibrationSignal?.ComputeNormalisedEnergy() ?? 0.0;

                var fdcList = new List<FDCData>()
                {
                    FDCData.MakeNew(ANA_LiseHFRefCalibrationMax, _fdcLiseHFRefCalibrationMax),
                    FDCData.MakeNew(ANA_LiseHFRefCalibrationNormalisedEnergy, _fdcLiseHFRefCalibrationNormalisedEnergy),
                };

                _fdcManager.SendFDCs(fdcList);


                CreateFDCLiseHFRefCalibByObjective(objectiveID, isLowIlluminationPower, rawCalibrationSignal);
            }
        }

        public bool IsNeededToSendFDCLiseHFSpotOffsetDeviation()
        {
            var fdcManager = ClassLocator.Default.GetInstance<FDCManager>();

            if ((fdcManager.GetFDCSendFrequency(ANA_LiseHFSpotOffsetDeviationX) == FDCSendFrequency.Once)
                || (fdcManager.GetFDCSendFrequency(ANA_LiseHFSpotOffsetDeviationY) == FDCSendFrequency.Once))
            {
                return true;
            }
            return false;
        }

        private ObjectiveSuffix ObjectiveDeviceIDToSuffix(string objectiveID)
        {
            switch (objectiveID)
            {
                case "ID-5XNIR01":
                    return ObjectiveSuffix.x5NIR;
                case "ID-10XNIR01":
                    return ObjectiveSuffix.x10NIR;
                case "ID-20XNIR01":
                    return ObjectiveSuffix.x20NIR;
                case "ID-50XNIR01":
                    return ObjectiveSuffix.x50NIR;
                case "ID-50XVIS01":
                case "ID-50X VIS":
                    return ObjectiveSuffix.x50VIS;
                default: break;
            }
            throw new Exception($"Unknown Objective ID in ProbeLiseHFFDCProvider : <{objectiveID}>");
        }

        public void CreateFDCLiseHFSpotOffsetDeviation(LiseHFObjectiveSpotCalibration liseHFSpotOffsetMeasured, IProbeSpotCalibration probeCalibration)
        {
            if (_globalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcLiseHFSpotOffsetDeviationX = liseHFSpotOffsetMeasured.XOffset.Micrometers - probeCalibration.XOffset.Micrometers;
                _fdcLiseHFSpotOffsetDeviationY = liseHFSpotOffsetMeasured.YOffset.Micrometers - probeCalibration.YOffset.Micrometers;

                var fdcList = new List<FDCData>()
                {
                    FDCData.MakeNew(ANA_LiseHFSpotOffsetDeviationX, _fdcLiseHFSpotOffsetDeviationX),
                    FDCData.MakeNew(ANA_LiseHFSpotOffsetDeviationY, _fdcLiseHFSpotOffsetDeviationY),
                };
                _fdcManager.SendFDCs(fdcList);
            }

            CreateFDCLiseHFSpotByObjective(liseHFSpotOffsetMeasured, probeCalibration);
        }

        public void CreateFDCLiseHFSpotByObjective(LiseHFObjectiveSpotCalibration liseHFSpotOffsetMeasured, IProbeSpotCalibration probeCalibration)
        {
            var objsuffix = ObjectiveDeviceIDToSuffix(liseHFSpotOffsetMeasured.ObjectiveDeviceId);
            if (!_fdcLiseHFSpotByObjectiveID.TryGetValue(objsuffix, out SpotObjectiveFDC spotObjFdc))
            {
                return;
            }

            spotObjFdc.OffsetDeviationX = liseHFSpotOffsetMeasured.XOffset.Micrometers - probeCalibration.XOffset.Micrometers;
            spotObjFdc.OffsetDeviationY = liseHFSpotOffsetMeasured.YOffset.Micrometers - probeCalibration.YOffset.Micrometers;
            spotObjFdc.Diameter = liseHFSpotOffsetMeasured.SpotDiameter.Micrometers;
            spotObjFdc.Shape = liseHFSpotOffsetMeasured.SpotShape;
            spotObjFdc.Intensity = liseHFSpotOffsetMeasured.SpotIntensity;
            // update dico values
            _fdcLiseHFSpotByObjectiveID[objsuffix] = spotObjFdc;

            var fdcList = new List<FDCData>()
            {
                FDCData.MakeNew($"{SpotByObjectivePrefix}OffsetDeviationX_{objsuffix}", spotObjFdc.OffsetDeviationX),
                FDCData.MakeNew($"{SpotByObjectivePrefix}OffsetDeviationY_{objsuffix}", spotObjFdc.OffsetDeviationY),
                FDCData.MakeNew($"{SpotByObjectivePrefix}Diameter_{objsuffix}", spotObjFdc.Diameter),
                FDCData.MakeNew($"{SpotByObjectivePrefix}Shape_{objsuffix}", spotObjFdc.Shape),
                FDCData.MakeNew($"{SpotByObjectivePrefix}Intensity_{objsuffix}", spotObjFdc.Intensity),
            };
            _fdcManager.SendFDCs(fdcList);
        }

        public void CreateFDCLiseHFRefCalibByObjective(string objectiveID, bool isLowIlluminationPower, RawCalibrationSignal rawCalibrationSignal)
        {
            var objsuffix = ObjectiveDeviceIDToSuffix(objectiveID);
            CalibRefObjectiveFDC refCalibObjFdc;
            Dictionary<ObjectiveSuffix, CalibRefObjectiveFDC> refDICOCalibObjFdc;
            string prefix;
            if (isLowIlluminationPower)
            {
                if (!_fdcLiseHFRefCalibLOW_ByObjectiveID.TryGetValue(objsuffix, out refCalibObjFdc))
                {
                    return;
                }
                refDICOCalibObjFdc = _fdcLiseHFRefCalibLOW_ByObjectiveID;
                prefix = RefCalibLOW_ByObjectivePrefix;
            }
            else
            {
                if (!_fdcLiseHFRefCalibSTD_ByObjectiveID.TryGetValue(objsuffix, out refCalibObjFdc))
                {
                    return ;
                }
                refDICOCalibObjFdc = _fdcLiseHFRefCalibSTD_ByObjectiveID;
                prefix = RefCalibSTD_ByObjectivePrefix;
            }

           var zones = rawCalibrationSignal?.ComputeZoneIntensity() ?? new List<double>(5) { 0.0, 0.0, 0.0, 0.0, 0.0 };
            refCalibObjFdc.Zone1 = zones[0];
            refCalibObjFdc.Zone2 = zones[1];
            refCalibObjFdc.Zone3 = zones[2];
            refCalibObjFdc.Zone4 = zones[3];
            refCalibObjFdc.Zone5 = zones[4];

            // update dico values
            refDICOCalibObjFdc[objsuffix] = refCalibObjFdc;
         
            var fdcList = new List<FDCData>(5);
            for (int i = 0; i < 5; i++)
            {
                fdcList.Add(FDCData.MakeNew($"{prefix}Zone{i + 1}_{objsuffix}", zones[i]));
            }
            _fdcManager.SendFDCs(fdcList);
        }

        public void CreateFDCLiseHFMeasure(LiseHFAlgoOutputs probeLiseHFAlgoOutputs)
        {
            if (_globalStatusServer.GetControlMode() == PMControlMode.Production)
            {
                _fdcLiseHFSignalZeroY = 0.0;
                _fdcLiseHFTSVDepthPeakX = 0.0;
                _fdcLiseHFTSVDepthPeakY = 0.0;

                if (probeLiseHFAlgoOutputs?.FTTy != null && probeLiseHFAlgoOutputs.FTTy.Count > 0)
                {
                    _fdcLiseHFSignalZeroY = probeLiseHFAlgoOutputs.FTTy[0];
                }

                // Sum peaksX, each peak corresponds to the thickness of a layer
                if (probeLiseHFAlgoOutputs?.PeaksX != null && probeLiseHFAlgoOutputs.PeaksX.Count > 0)
                {
                    foreach (var peak in probeLiseHFAlgoOutputs.PeaksX)
                    {
                        _fdcLiseHFTSVDepthPeakX += peak;
                    }
                }

                // PeaksY are corrected and can be slightly different from the real Y value because
                // the algorithm try to fit a gaussian. We need to retrieve the Y value by hand.
                if (probeLiseHFAlgoOutputs?.FTTx != null && probeLiseHFAlgoOutputs?.FTTx.Count > 0
                    && probeLiseHFAlgoOutputs?.FTTy != null && probeLiseHFAlgoOutputs?.FTTy.Count > 0
                    && _fdcLiseHFTSVDepthPeakX > 0)
                {
                    for (int i = 1; i < probeLiseHFAlgoOutputs.FTTx.Count; i++)
                    {
                        if (probeLiseHFAlgoOutputs.FTTx[i] > _fdcLiseHFTSVDepthPeakX)
                        {
                            var peakYPrev = probeLiseHFAlgoOutputs.FTTy[i - 1];
                            var peakYNext = probeLiseHFAlgoOutputs.FTTy[i];
                            var peakXPrev = probeLiseHFAlgoOutputs.FTTx[i - 1];
                            var peakXNext = probeLiseHFAlgoOutputs.FTTx[i];

                            double a = (peakYNext - peakYPrev) / (peakXNext - peakXPrev);
                            double b = peakYNext - (a * probeLiseHFAlgoOutputs.FTTx[i]);

                            _fdcLiseHFTSVDepthPeakY = (a * _fdcLiseHFTSVDepthPeakX) + b;
                            break;
                        }
                    }
                }

                var fdcList = new List<FDCData>()
                {
                    FDCData.MakeNew(ANA_LiseHFSignalZeroY, _fdcLiseHFSignalZeroY),
                    FDCData.MakeNew(ANA_LiseHFTSVDepthPeakX, _fdcLiseHFTSVDepthPeakX),
                    FDCData.MakeNew(ANA_LiseHFTSVDepthPeakY, _fdcLiseHFTSVDepthPeakY),
                };

                _fdcManager.SendFDCs(fdcList);
            }
        }

       
    }
}
