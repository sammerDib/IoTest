using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using UnitySC.PM.LIGHTSPEED.Data;
using UnitySC.PM.LIGHTSPEED.Service.Interface;
using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.LIGHTSPEED.Client.CommonUI.ViewModel.Maintenance
{
    public class RotatorsKitCalibrationVM : ViewModelBase
    {
        private IRotatorsKitCalibrationService _supervisor;

        private const double MAX_LASER_POWER_MW = 500.0;
        private const string CSV_EXT = ".csv";
        private const double ENCODER_RESOLUTION = 143360.0;
        private const double TURN = 360.0;
        private const int MAX_STEP = 4;

        private DateTime _dtnow;

        private object _pos_lock = new object();
        private Queue<double> _positionQueue;

        public enum ESelectedLut { None, HS, HT, Both }

        public ObservableCollection<DataAttributeObject> InputList { get; set; } = new ObservableCollection<DataAttributeObject>();

        public ObservableCollection<DataAttributeObject> OutputList { get; set; } = new ObservableCollection<DataAttributeObject>();

        public int PowerSetpoint { get; set; }
        public int CurrentSetpoint { get; set; }
        public double AbsPositionSetpoint { get; set; }

        public double WideOutputVoltage { get; set; }

        public double NarrowOutputVoltage { get; set; }

        public MppcCollector MppcCollector { get; set; }

        public RangeCalibrate HsRangeCalibrate { get; set; } = new RangeCalibrate() { CalibrateStartAngle = 177.9, CalibrateEndAngle = 181.8 };
        public RangeCalibrate HtRangeCalibrate { get; set; } = new RangeCalibrate() { CalibrateStartAngle = 86.2, CalibrateEndAngle = 92.2 };

        public uint NumberAverages { get; set; } = 50;

        public string DefaultDirectory { get; set; } = "C:\\Unity\\Calibrate\\";
        public string DefaultDirectoryMonitoring { get; set; } = "C:\\Unity\\Monitoring\\";
        public string SaveFilenameLut { get; set; } = "Lut";
        public string SaveFilenameShutter { get; set; } = "Shutter";

        private StreamWriter _streamWriter;
        private StreamWriter _streamWriterShutter;
        private CultureInfo _culture = CultureInfo.InvariantCulture;

        public ObservableCollection<LutProfile> LutNames { get; set; }
        private ObservableCollection<LutProfile> _lutNames;

        public ObservableCollection<PowermeterRange> PowermeterRangesHs { get; set; } = new ObservableCollection<PowermeterRange>();

        public ObservableCollection<PowermeterRange> PowermeterRangesHt { get; set; } = new ObservableCollection<PowermeterRange>();

        private bool _errorShutterMonitoring;
        private bool _abortWait;

        public uint NumberOfCycles { get; set; } = 3;

        public RotatorsKitCalibrationVM(IRotatorsKitCalibrationService supervisor)
        {
           _supervisor = supervisor;

            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterPowerChangedMessage>(this, (m) => { UpdatePowermeterPower(m.Flow, m.Power, m.PowerCal_mW, m.RFactor); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterCurrentChangedMessage>(this, (m) => { UpdatePowermeterCurrent(m.Flow, m.Current_mA); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterIdentifierChangedMessage>(this, (m) => { UpdatePowermeterIdentifier(m.Flow, m.Identifier); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterSensorTypeChangedMessage>(this, (m) => { UpdatePowermeterSensorType(m.Flow, m.SensorType); });            
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterSensorAttenuationChangedMessage>(this, (m) => { UpdatePowermeterSensorAttenuation(m.Flow, m.SensorAttenuation); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterWavelengthChangedMessage>(this, (m) => { UpdatePowermeterWavelength(m.Flow, m.Wavelength); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterRangesChangedMessage>(this, (m) => { UpdatePowermeterRanges(m.Flow, m.PowermeterRange); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterBeamDiameterChangedMessage>(this, (m) => { UpdatePowermeterBeamDiameter(m.Flow, m.BeamDiameter); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterDarkAdjustStateChangedMessage>(this, (m) => { UpdatePowermeterDarkAdjustState(m.Flow, m.DarkAdjustState); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterDarkOffsetChangedMessage>(this, (m) => { UpdatePowermeterDarkOffset(m.Flow, m.DarkOffset); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterResponsivityChangedMessage>(this, (m) => { UpdatePowermeterResponsivity(m.Flow, m.Responsivity); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterRFactorsCalibChangedMessage>(this, (m) => { UpdatePowermeterRFactorsCalib(m.Flow, m.RFactorS, m.RFactorP); });

            Messenger.Register<Proxy.RotatorsKitCalibration.PowerLaserChangedMessage>(this, (m) => { UpdatePowerLaser(m.Power); });
            Messenger.Register<Proxy.RotatorsKitCalibration.InterlockStatusChangedMessage>(this, (m) => { UpdateInterlockStatus(m.InterlockStatus); });
            Messenger.Register<Proxy.RotatorsKitCalibration.LaserTemperatureChangedMessage>(this, (m) => { UpdateLaserTemperature(m.LaserTemperature); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PsuTemperatureChangedMessage>(this, (m) => { UpdatePsuTemperature(m.PsuTemperature); });
            Messenger.Register<Proxy.RotatorsKitCalibration.AttenuationPositionChangedMessage>(this, (m) => { UpdateAttenuationPosition(m.AttenuationPosition); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PolarisationPositionChangedMessage>(this, (m) => { UpdatePolarisationPosition(m.PolarisationPosition); });
            Messenger.Register<Proxy.RotatorsKitCalibration.PolarisationAngleCalibChangedMessage>(this, (m) => { UpdatePolarisationAngleCalib(m.PolarAngleHsS, m.PolarAngleHsP, m.PolarAngleHtS, m.PolarAngleHtP); });
            Messenger.Register<Proxy.RotatorsKitCalibration.ShutterIrisPositionChangedMessage>(this, (m) => { UpdateShutterIrisPosition(m.ShutterIrisPosition); });
            Messenger.Register<Proxy.RotatorsKitCalibration.MppcStateSignalsChangedMessage>(this, (m) => { UpdateMppcStateSignals(m.Collector, m.StateSignals); });
            Messenger.Register<Proxy.RotatorsKitCalibration.MppcOutputVoltageChangedMessage>(this, (m) => { UpdateMppcOutputVoltage(m.Collector, m.OutputVoltage); });
            Messenger.Register<Proxy.RotatorsKitCalibration.MppcOutputCurrentChangedMessage>(this, (m) => { UpdateMppcOutputCurrent(m.Collector, m.OutputCurrent); });
            Messenger.Register<Proxy.RotatorsKitCalibration.MppcSensorTemperatureChangedMessage>(this, (m) => { UpdateMppcSensorTemperature(m.Collector, m.SensorTemperature); });

            Messenger.Register<Proxy.RotatorsKitCalibration.DataAttributesChangedMessages>(this, (m) => { UpdateIoList(m.DataAttributes); });

            Task.Run(() => _supervisor.InitializeUpdate());

            _lutNames = new ObservableCollection<LutProfile>(
                new[]
                {
                    new LutProfile
                    {
                        Name = "HS_S",
                        PowerIlluminationFlow = PowerIlluminationFlow.HS,
                        WaveplateAnglesPolarisation = WaveplateAnglesPolarisation.S_Polar
                    },
                    new LutProfile
                    {
                        Name = "HS_P",
                        PowerIlluminationFlow = PowerIlluminationFlow.HS,
                        WaveplateAnglesPolarisation = WaveplateAnglesPolarisation.P_Polar
                    },
                    new LutProfile
                    {
                        Name = "HT_S",
                        PowerIlluminationFlow = PowerIlluminationFlow.HT,
                        WaveplateAnglesPolarisation = WaveplateAnglesPolarisation.S_Polar
                    },
                    new LutProfile
                    {
                        Name = "HT_P",
                        PowerIlluminationFlow = PowerIlluminationFlow.HT,
                        WaveplateAnglesPolarisation = WaveplateAnglesPolarisation.P_Polar
                    }
                 });

            LutNames = new ObservableCollection<LutProfile>(_lutNames);
            CultureInfo.CurrentCulture = _culture;
        }

        private GalaSoft.MvvmLight.Messaging.IMessenger _messenger;

        public GalaSoft.MvvmLight.Messaging.IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<GalaSoft.MvvmLight.Messaging.IMessenger>();
                return _messenger;
            }
        }

        private void UpdatePowermeterPower(PowerIlluminationFlow flow, double power_mW, double powerCal_mW, double rfactor)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterRawPowerHs = Double.Parse(power_mW.ToString("E04"));
                PowermeterCalcPowerHs = Double.Parse(powerCal_mW.ToString("E04"));
            }
            else
            {
                PowermeterRawPowerHt = Double.Parse(power_mW.ToString("E04"));
                PowermeterCalcPowerHt = Double.Parse(powerCal_mW.ToString("E04"));
            }
        }

        private void UpdatePowermeterCurrent(PowerIlluminationFlow flow, double current_mA)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterCurrentHs = current_mA;
            }
            else
            {
                PowermeterCurrentHt = current_mA;
            }
        }

        private void UpdatePowermeterIdentifier(PowerIlluminationFlow flow, string identifier)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterIdentifierHs = identifier;
            }
            else
            {
                PowermeterIdentifierHt = identifier;
            }
        }        

        private void UpdatePowermeterDarkAdjustState(PowerIlluminationFlow flow, string value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterDarkAdjustStateHs = value;
            }
            else
            {
                PowermeterDarkAdjustStateHt = value;
            }
        }

        private void UpdatePowermeterDarkOffset(PowerIlluminationFlow flow, double value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterDarkOffsetHs = value;
            }
            else
            {
                PowermeterDarkOffsetHt = value;
            }
        }

        private void UpdatePowermeterResponsivity(PowerIlluminationFlow flow, double value)
        {            
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterResponsivityHs = value;                
            }
            else
            {
                PowermeterResponsivityHt = value;                               
            }
        }

        private void UpdatePowermeterRFactorsCalib(PowerIlluminationFlow flow, double rfactorS, double rfactorP)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                HSS_RFactor = rfactorS;
                HSP_RFactor = rfactorP;
            }
            else
            {
                HTS_RFactor = rfactorS;
                HTP_RFactor = rfactorP;
            }
        }        

        private void UpdatePowermeterSensorType(PowerIlluminationFlow flow, string value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterSensorTypeHs = value;
            }
            else
            {
                PowermeterSensorTypeHt = value;
            }
        }
        
        private void UpdatePowermeterSensorAttenuation(PowerIlluminationFlow flow, uint value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterSensorAttenuationHs = value;
            }
            else
            {
                PowermeterSensorAttenuationHt = value;
            }
        }

        private void UpdatePowermeterWavelength(PowerIlluminationFlow flow, uint value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterWavelengthHs = value;
            }
            else
            {
                PowermeterWavelengthHt = value;
            }
        }

        private void UpdatePowermeterRanges(PowerIlluminationFlow flow, string value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                if (!PowermeterRangesHs.Any(p => p.Range == value))
                    PowermeterRangesHs.Add(new PowermeterRange { Range = value });
            }
            else
            {
                if (!PowermeterRangesHt.Any(p => p.Range == value))
                    PowermeterRangesHt.Add(new PowermeterRange { Range = value });
            }
        }        

        private void UpdatePowermeterBeamDiameter(PowerIlluminationFlow flow, uint value)
        {
            if (flow == PowerIlluminationFlow.HS)
            {
                PowermeterBeamDiameterHs = value;
            }
            else
            {
                PowermeterBeamDiameterHt = value;
            }
        }     

        private ECalibrationType _eCalibrationType = ECalibrationType.Auto;

        public ECalibrationType CalibrationType
        {
            get => _eCalibrationType;
            set
            {
                if (_eCalibrationType != value)
                {
                    _eCalibrationType = value;
                    RaisePropertyChanged();
                }
            }
        }

        private PowerIlluminationFlow _powerIlluminationFlow = PowerIlluminationFlow.HS;

        public PowerIlluminationFlow PowerIlluminationFlow
        {
            get => _powerIlluminationFlow;
            set
            {
                if (_powerIlluminationFlow != value)
                {
                    _powerIlluminationFlow = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void UpdatePowerLaser(double value)
        {
            PowerLaser = value;
        }

        private void UpdateInterlockStatus(string interlockStatus)
        {
            InterlockStatus = interlockStatus;
        }

        private void UpdateLaserTemperature(double laserTemperature)
        {
            LaserTemperature = laserTemperature;
        }

        private void UpdatePsuTemperature(double psuTemperature)
        {
            PsuTemperature = psuTemperature;
        }

        private void UpdateAttenuationPosition(double attenuationPosition)
        {
            if (BeamShaperFlow == BeamShaperFlow.Attenuation)
            {
                BeamShaperPosition = attenuationPosition;
            }
            LastAttenuationPosition = attenuationPosition;
        }

        private void UpdatePolarisationPosition(double polarisationPosition)
        {
            if (Math.Round(polarisationPosition) == 64d) // WTF ???
            {
                _waveplateAnglesPolarisation = WaveplateAnglesPolarisation.S_Polar;
            }
            else if (Math.Round(polarisationPosition) == 19d) //WTF ??
            {
                _waveplateAnglesPolarisation = WaveplateAnglesPolarisation.P_Polar;
            }

            if (BeamShaperFlow == BeamShaperFlow.Polarisation)
            {
                BeamShaperPosition = polarisationPosition;
                WaveplateAnglesPolarisation = _waveplateAnglesPolarisation;

            }
            LastPolarPosition = polarisationPosition;
        }

        private void UpdatePolarisationAngleCalib(double polarAngleHsS, double polarAngleHsP, double polarAngleHtS, double polarAngleHtP)
        {
            HSS_PolarAngle = polarAngleHsS;
            HSP_PolarAngle = polarAngleHsP;
            HTS_PolarAngle = polarAngleHtS;
            HTP_PolarAngle = polarAngleHtP;
        }

        private void UpdateShutterIrisPosition(string shutterIrisPosition)
        {
            ShutterIrisPosition = shutterIrisPosition;
        }

        private void UpdateMppcStateSignals(MppcCollector collector, MppcStateModule stateSignals)
        {
            MppcCollector = collector;
            _ = (collector == MppcCollector.WIDE) ? MppcWideStateSignals = stateSignals : MppcNarrowStateSignals = stateSignals;
        }

        private void UpdateMppcOutputVoltage(MppcCollector collector, double voltage)
        {
            MppcCollector = collector;

            _ = (collector == MppcCollector.WIDE) ? MppcWideOutputVoltage = voltage : MppcNarrowOutputVoltage = voltage;
        }

        private void UpdateMppcOutputCurrent(MppcCollector collector, double current)
        {
            MppcCollector = collector;
            _ = (collector == MppcCollector.WIDE) ? MppcWideOutputCurrent = current : MppcNarrowOutputCurrent = current;
        }

        private void UpdateMppcSensorTemperature(MppcCollector collector, double sensorTemperature)
        {
            MppcCollector = collector;
            _ = (collector == MppcCollector.WIDE) ? MppcWideSensorTemperature = sensorTemperature : MppcNarrowSensorTemperature = sensorTemperature;
        }

        private void UpdateIoList(List<DataAttribute> dataAttributes)
        {            
            foreach (var item in dataAttributes)
            {
                UpdateDataAttributes(item, InputList, "INPUT");
            }
            RaisePropertyChanged(() => InputList);

            foreach (var item in dataAttributes)
            {
                UpdateDataAttributes(item, OutputList, "OUTPUT");
            }
            RaisePropertyChanged(() => OutputList);
        }

        private void UpdateDataAttributes(DataAttribute item, ObservableCollection<DataAttributeObject> dataAttributes, string dataAttributeTypeSearch)
        {
            DataAttributeObject foundDataAttributeObj = (dataAttributes.ToList<DataAttributeObject>().Find(dataAttributeObj => item.Name.ToString() == dataAttributeObj.Name));
            if (foundDataAttributeObj == null)
            {
                DataAttributeObject newDataAttribute = new DataAttributeObject();
                newDataAttribute.Identifier = item.Identifier;
                newDataAttribute.Name = item.Name.ToString();
                newDataAttribute.Value = item.DigitalValue;
                newDataAttribute.OnDigitalValueChanged = new SetDigitalIoChanged(SetIoValue);
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (newDataAttribute.Name.ToString().ToUpper().Contains(dataAttributeTypeSearch) &&
                        !dataAttributes.Any(p => p.Name == item.Name))
                    
                        dataAttributes.Add(newDataAttribute);
                }));
            }
            else
                foundDataAttributeObj.UpdateValue(item.DigitalValue);
        }

        private void SetIoValue(string identifier, string name, bool value)
        {
            Task.Run(() => _supervisor.SetIoValue(identifier, name, value));
        }

        private double _powermeterRawPowerHs;

        public double PowermeterRawPowerHs
        {
            get => _powermeterRawPowerHs;
            set
            {
                if (_powermeterRawPowerHs != value)
                {
                    _powermeterRawPowerHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterRawPowerHt;

        public double PowermeterRawPowerHt
        {
            get => _powermeterRawPowerHt;
            set
            {
                if (_powermeterRawPowerHt != value)
                {
                    _powermeterRawPowerHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterCalcPowerHs;

        public double PowermeterCalcPowerHs
        {
            get => _powermeterCalcPowerHs;
            set
            {
                if (_powermeterCalcPowerHs != value)
                {
                    _powermeterCalcPowerHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterCalcPowerHt;

        public double PowermeterCalcPowerHt
        {
            get => _powermeterCalcPowerHt;
            set
            {
                if (_powermeterCalcPowerHt != value)
                {
                    _powermeterCalcPowerHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterCurrentHs;

        public double PowermeterCurrentHs
        {
            get => _powermeterCurrentHs;
            set
            {
                if (_powermeterCurrentHs != value)
                {
                    _powermeterCurrentHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterCurrentHt;

        public double PowermeterCurrentHt
        {
            get => _powermeterCurrentHt;
            set
            {
                if (_powermeterCurrentHt != value)
                {
                    _powermeterCurrentHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _powermeterIdentifierHs;

        public string PowermeterIdentifierHs
        {
            get => _powermeterIdentifierHs;
            set
            {
                if (_powermeterIdentifierHs != value)
                {
                    _powermeterIdentifierHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _powermeterIdentifierHt;

        public string PowermeterIdentifierHt
        {
            get => _powermeterIdentifierHt;
            set
            {
                if (_powermeterIdentifierHt != value)
                {
                    _powermeterIdentifierHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _powermeterHsAutoRangeIsEnabled = true;

        public bool PowermeterHsAutoRangeIsEnabled
        {
            get => _powermeterHsAutoRangeIsEnabled; 
            set 
            { 
                if (_powermeterHsAutoRangeIsEnabled != value) 
                { 
                    _powermeterHsAutoRangeIsEnabled = value;
                    PowermeterEnableAutoRange(PowerIlluminationFlow.HS, _powermeterHsAutoRangeIsEnabled);
                    RaisePropertyChanged();
                }
            }
        }

        private bool _powermeterHtAutoRangeIsEnabled = true;

        public bool PowermeterHtAutoRangeIsEnabled
        {
            get => _powermeterHtAutoRangeIsEnabled;
            set
            {
                if (_powermeterHtAutoRangeIsEnabled != value)
                {
                    _powermeterHtAutoRangeIsEnabled = value;
                    PowermeterEnableAutoRange(PowerIlluminationFlow.HT, _powermeterHtAutoRangeIsEnabled);
                    RaisePropertyChanged();
                }
            }
        }

        private void PowermeterEnableAutoRange(PowerIlluminationFlow flow, bool activate)
        {
            Task.Run(() => _supervisor.PowermeterEnableAutoRange(flow, activate));
        }

        private string _powermeterDarkAdjustStateHs;

        public string PowermeterDarkAdjustStateHs
        {
            get => _powermeterDarkAdjustStateHs;
            set
            {
                if (_powermeterDarkAdjustStateHs != value)
                {
                    _powermeterDarkAdjustStateHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _powermeterDarkAdjustStateHt;

        public string PowermeterDarkAdjustStateHt
        {
            get => _powermeterDarkAdjustStateHt;
            set
            {
                if (_powermeterDarkAdjustStateHt != value)
                {
                    _powermeterDarkAdjustStateHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterDarkOffsetHs;

        public double PowermeterDarkOffsetHs
        {
            get => _powermeterDarkOffsetHs;
            set
            {
                if (_powermeterDarkOffsetHs != value)
                {
                    _powermeterDarkOffsetHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterDarkOffsetHt;

        public double PowermeterDarkOffsetHt
        {
            get => _powermeterDarkOffsetHt;
            set
            {
                if (_powermeterDarkOffsetHt != value)
                {
                    _powermeterDarkOffsetHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterResponsivityHs;

        public double PowermeterResponsivityHs
        {
            get => _powermeterResponsivityHs;
            set
            {
                if (_powermeterResponsivityHs != value)
                {
                    _powermeterResponsivityHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterResponsivityHt;

        public double PowermeterResponsivityHt
        {
            get => _powermeterResponsivityHt;
            set
            {
                if (_powermeterResponsivityHt != value)
                {
                    _powermeterResponsivityHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterResponsivityHtS;

        public double PowermeterResponsivityHtS
        {
            get => _powermeterResponsivityHtS;
            set
            {
                if (_powermeterResponsivityHtS != value)
                {
                    _powermeterResponsivityHtS = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterResponsivityHtP;

        public double PowermeterResponsivityHtP
        {
            get => _powermeterResponsivityHtP;
            set
            {
                if (_powermeterResponsivityHtP != value)
                {
                    _powermeterResponsivityHtP = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _powermeterSensorTypeHs;

        public string PowermeterSensorTypeHs
        {
            get => _powermeterSensorTypeHs;
            set
            {
                if (_powermeterSensorTypeHs != value)
                {
                    _powermeterSensorTypeHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _powermeterSensorTypeHt;

        public string PowermeterSensorTypeHt
        {
            get => _powermeterSensorTypeHt;
            set
            {
                if (_powermeterSensorTypeHt != value)
                {
                    _powermeterSensorTypeHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _powermeterSensorAttenuationHs;

        public uint PowermeterSensorAttenuationHs
        {
            get => _powermeterSensorAttenuationHs;
            set
            {
                if (_powermeterSensorAttenuationHs != value)
                {
                    _powermeterSensorAttenuationHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _powermeterSensorAttenuationHt;

        public uint PowermeterSensorAttenuationHt
        {
            get => _powermeterSensorAttenuationHt;
            set
            {
                if (_powermeterSensorAttenuationHt != value)
                {
                    _powermeterSensorAttenuationHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _powermeterWavelengthHs;

        public uint PowermeterWavelengthHs
        {
            get => _powermeterWavelengthHs;
            set
            {
                if (_powermeterWavelengthHs != value)
                {
                    _powermeterWavelengthHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _powermeterWavelengthHt;

        public uint PowermeterWavelengthHt
        {
            get => _powermeterWavelengthHt;
            set
            {
                if (_powermeterWavelengthHt != value)
                {
                    _powermeterWavelengthHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _powermeterBeamDiameterHs;

        public uint PowermeterBeamDiameterHs
        {
            get => _powermeterBeamDiameterHs;
            set
            {
                if (_powermeterBeamDiameterHs != value)
                {
                    _powermeterBeamDiameterHs = value;
                    RaisePropertyChanged();
                }
            }
        }

        private uint _powermeterBeamDiameterHt;

        public uint PowermeterBeamDiameterHt
        {
            get => _powermeterBeamDiameterHt;
            set
            {
                if (_powermeterBeamDiameterHt != value)
                {
                    _powermeterBeamDiameterHt = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powerLaser;

        public double PowerLaser
        {
            get => _powerLaser;
            set
            {
                if (_powerLaser != value)
                {
                    _powerLaser = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _interlockStatus;

        public string InterlockStatus
        {
            get => _interlockStatus;
            set
            {
                if (_interlockStatus != value)
                {
                    _interlockStatus = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _laserTemperature;

        public double LaserTemperature
        {
            get => _laserTemperature;
            set
            {
                if (_laserTemperature != value)
                {
                    _laserTemperature = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _psuTemperature;

        public double PsuTemperature
        {
            get => _psuTemperature;
            set
            {
                if (_psuTemperature != value)
                {
                    _psuTemperature = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _beamShaperPosition;

        public double BeamShaperPosition
        {
            get => _beamShaperPosition;
            set
            {
                if (_beamShaperPosition != value)
                {
                    _beamShaperPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _lastAttenuationPosition;
        public double LastAttenuationPosition
        {
            get => _lastAttenuationPosition;
            set
            {
                if (_lastAttenuationPosition != value)
                {
                    _lastAttenuationPosition = value;
                    RaisePropertyChanged();
                }
            }
        }
        private double _lastPolarPosition;
        public double LastPolarPosition
        {
            get => _lastPolarPosition;
            set
            {
                if (_lastPolarPosition != value)
                {
                    _lastPolarPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _shutterIrisPosition;

        public string ShutterIrisPosition
        {
            get => _shutterIrisPosition;
            set
            {
                if (_shutterIrisPosition != value)
                {
                    _shutterIrisPosition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private MppcStateModule _mppcWideStateSignals;

        public MppcStateModule MppcWideStateSignals
        {
            get => _mppcWideStateSignals;
            set
            {
                if (_mppcWideStateSignals != value)
                {
                    _mppcWideStateSignals = value;
                    RaisePropertyChanged();
                }
            }
        }

        private MppcStateModule _mppcNarrowStateSignals;

        public MppcStateModule MppcNarrowStateSignals
        {
            get => _mppcNarrowStateSignals;
            set
            {
                if (_mppcNarrowStateSignals != value)
                {
                    _mppcNarrowStateSignals = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _mppcWideOutputVoltage;

        public double MppcWideOutputVoltage
        {
            get => _mppcWideOutputVoltage;
            set
            {
                if (_mppcWideOutputVoltage != value)
                {
                    _mppcWideOutputVoltage = value;
                    RaisePropertyChanged();
                }
            }
        }


        private double _mppcNarrowOutputVoltage;

        public double MppcNarrowOutputVoltage
        {
            get => _mppcNarrowOutputVoltage;
            set
            {
                if (_mppcNarrowOutputVoltage != value)
                {
                    _mppcNarrowOutputVoltage = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _mppcWideOutputCurrent;

        public double MppcWideOutputCurrent
        {
            get => _mppcWideOutputCurrent;
            set
            {
                if (_mppcWideOutputCurrent != value)
                {
                    _mppcWideOutputCurrent = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _mppcNarrowOutputCurrent;

        public double MppcNarrowOutputCurrent
        {
            get => _mppcNarrowOutputCurrent;
            set
            {
                if (_mppcNarrowOutputCurrent != value)
                {
                    _mppcNarrowOutputCurrent = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _mppcWideSensorTemperature;

        public double MppcWideSensorTemperature
        {
            get => _mppcWideSensorTemperature;
            set
            {
                if (_mppcWideSensorTemperature != value)
                {
                    _mppcWideSensorTemperature = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _mppcNarrowSensorTemperature;

        public double MppcNarrowSensorTemperature
        {
            get => _mppcNarrowSensorTemperature;
            set
            {
                if (_mppcNarrowSensorTemperature != value)
                {
                    _mppcNarrowSensorTemperature = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _hsS_RFactor;

        public double HSS_RFactor
        {
            get => _hsS_RFactor;
            set
            {
                if (_hsS_RFactor != value)
                {
                    _hsS_RFactor = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _hsP_RFactor;

        public double HSP_RFactor
        {
            get => _hsP_RFactor;
            set
            {
                if (_hsP_RFactor != value)
                {
                    _hsP_RFactor = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _htS_RFactor;

        public double HTS_RFactor
        {
            get => _htS_RFactor;
            set
            {
                if (_htS_RFactor != value)
                {
                    _htS_RFactor = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _htP_RFactor;

        public double HTP_RFactor
        {
            get => _htP_RFactor;
            set
            {
                if (_htP_RFactor != value)
                {
                    _htP_RFactor = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _hsS_PolarAngle = 0.0;

        public double HSS_PolarAngle
        {
            get => _hsS_PolarAngle;
            set
            {
                if (_hsS_PolarAngle != value)
                {
                    _hsS_PolarAngle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _hsP_PolarAngle = 0.0;

        public double HSP_PolarAngle
        {
            get => _hsP_PolarAngle;
            set
            {
                if (_hsP_PolarAngle != value)
                {
                    _hsP_PolarAngle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _htS_PolarAngle = 0.0;

        public double HTS_PolarAngle
        {
            get => _htS_PolarAngle;
            set
            {
                if (_htS_PolarAngle != value)
                {
                    _htS_PolarAngle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _htP_PolarAngle = 0.0;

        public double HTP_PolarAngle
        {
            get => _htP_PolarAngle;
            set
            {
                if (_htP_PolarAngle != value)
                {
                    _htP_PolarAngle = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterEditResponsivityHs;

        public double PowermeterEditResponsivityHs
        {
            get => _powermeterEditResponsivityHs;
            set
            {
                if (_powermeterEditResponsivityHs != value)
                {
                    _powermeterEditResponsivityHs = value;
                    PowermeterEditResponsivity(PowerIlluminationFlow.HS, _powermeterEditResponsivityHs);
                    RaisePropertyChanged();
                }
            }
        }

        private double _powermeterEditResponsivityHt;

        public double PowermeterEditResponsivityHt
        {
            get => _powermeterEditResponsivityHt;
            set
            {
                if (_powermeterEditResponsivityHt != value)
                {
                    _powermeterEditResponsivityHt = value;
                    PowermeterEditResponsivity(PowerIlluminationFlow.HT, _powermeterEditResponsivityHt);
                    RaisePropertyChanged();
                }
            }
        }

        private void PowermeterEditResponsivity(PowerIlluminationFlow flow, double repsonsivity_mA_W)
        {
            Task.Run(() => _supervisor.PowermeterEditResponsivity(flow, repsonsivity_mA_W));
        }

        private int _selectedIndexLutName;

        public int SelectedIndexLutName
        {
            get => _selectedIndexLutName;
            set
            {
                if (_selectedIndexLutName != value)
                {
                    _selectedIndexLutName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _selectedRangeHs;

        public int SelectedRangeHs
        {
            get => _selectedRangeHs;
            set
            {
                if (_selectedRangeHs != value)
                {
                    _selectedRangeHs = value;
                    Task.Run(() => _supervisor.PowermeterRangesVariation(PowerIlluminationFlow.HS, PowermeterRangesHs[_selectedRangeHs].Range));
                    RaisePropertyChanged();
                }
            }
        }

        private int _selectedRangeHt;

        public int SelectedRangeHt
        {
            get => _selectedRangeHt;
            set
            {
                if (_selectedRangeHt != value)
                {
                    _selectedRangeHt = value;
                    Task.Run(() => _supervisor.PowermeterRangesVariation(PowerIlluminationFlow.HS, PowermeterRangesHs[_selectedRangeHs].Range));
                    RaisePropertyChanged();
                }
            }
        }

        private string _step = "0/0";

        public string Step
        {
            get => _step;
            set
            {
                if (_step != value)
                {
                    _step = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _stepShutterMonitoring = "0/0";

        public string StepShutterMonitoring
        {
            get => _stepShutterMonitoring;
            set
            {
                if (_stepShutterMonitoring != value)
                {
                    _stepShutterMonitoring = value;
                    RaisePropertyChanged();
                }
            }
        }        

        private BeamShaperFlow _beamShaperFlow = BeamShaperFlow.Attenuation;

        public BeamShaperFlow BeamShaperFlow
        {
            get => _beamShaperFlow;
            set
            {
                if (_beamShaperFlow != value)
                {
                    _beamShaperFlow = value;
                    Task.Run(() => _supervisor.InitializeUpdate());
                    RaisePropertyChanged();
                }
            }
        }

        private string _incLabel = string.Format("{0:0.00} %", 0);
        private string _incLabelShutterMonitoring = string.Format("{0:0.00} %", 0);

        public string IncLabel
        {
            get => _incLabel;
            set
            {
                if (_incLabel != value)
                {
                    _incLabel = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string IncLabelShutterMonitoring
        {
            get => _incLabelShutterMonitoring;
            set
            {
                if (_incLabelShutterMonitoring != value)
                {
                    _incLabelShutterMonitoring = value;
                    RaisePropertyChanged();
                }
            }
        }

        private WaveplateAnglesPolarisation _waveplateAnglesPolarisation;

        public WaveplateAnglesPolarisation WaveplateAnglesPolarisation
        {
            get => _waveplateAnglesPolarisation;
            set
            {
                if (_waveplateAnglesPolarisation != value)
                {
                    _waveplateAnglesPolarisation = value;
                    RaisePropertyChanged();
                }
            }
        }

        private ESelectedLut SelectedLut()
        {
            ESelectedLut selectedLut = ESelectedLut.None;
            bool isHs = false;

            if (HsRangeCalibrate.CalibrateStartAngle > 0.0 &&
                HsRangeCalibrate.CalibrateEndAngle > 0.0 &&
                HsRangeCalibrate.AngleSteps >= 0.01)
            {
                isHs = true;
                selectedLut = ESelectedLut.HS;
            }
            if (HtRangeCalibrate.CalibrateStartAngle > 0.0 &&
                HtRangeCalibrate.CalibrateEndAngle > 0.0 &&
                HtRangeCalibrate.AngleSteps >= 0.01)
            {
                if (isHs)
                {
                    selectedLut = ESelectedLut.Both;
                }
                else
                {
                    selectedLut = ESelectedLut.HT;
                }
            }

            return selectedLut;
        }

        #region RelayCommands

        private RelayCommand _powerOnCommand;

        public RelayCommand PowerOnCommand
        {
            get
            {
                return _powerOnCommand ?? (_powerOnCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerOn());
                    }));
            }
        }

        private RelayCommand _powerOffCommand;

        public RelayCommand PowerOffCommand
        {
            get
            {
                return _powerOffCommand ?? (_powerOffCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowerOff());
                    }));
            }
        }

        private RelayCommand _applyPowerCommand;

        public RelayCommand ApplyPowerCommand
        {
            get
            {
                return _applyPowerCommand ?? (_applyPowerCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SetPower(PowerSetpoint));
                    }));
            }
        }

        private RelayCommand _applyCurrentCommand;

        public RelayCommand ApplyCurrentCommand
        {
            get
            {
                return _applyCurrentCommand ?? (_applyCurrentCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.SetCurrent(CurrentSetpoint));
                    }));
            }
        }

        private RelayCommand _homePositionCommand;

        public RelayCommand HomePositionCommand
        {
            get
            {
                return _homePositionCommand ?? (_homePositionCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.HomePosition(BeamShaperFlow));
                    }));
            }
        }

        private RelayCommand _moveAbsPositionCommand;

        public RelayCommand MoveAbsPositionCommand
        {
            get
            {
                return _moveAbsPositionCommand ?? (_moveAbsPositionCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.MoveAbsPosition(BeamShaperFlow, AbsPositionSetpoint));
                    }));
            }
        }

        private RelayCommand _openShutterCommand;

        public RelayCommand OpenShutterCommand
        {
            get
            {
                return _openShutterCommand ?? (_openShutterCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.OpenShutterCommand());
                    }));
            }
        }

        private RelayCommand _closeShutterCommand;

        public RelayCommand CloseShutterCommand
        {
            get
            {
                return _closeShutterCommand ?? (_closeShutterCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.CloseShutterCommand());
                    }));
            }
        }

        private bool _isCalibrateRunning;

        public bool IsCalibrateRunning
        {
            get => _isCalibrateRunning;
            set
            {
                if (_isCalibrateRunning != value)
                {
                    _isCalibrateRunning = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isShutterMonitoringRunning;

        public bool IsShutterMonitoringRunning
        {
            get => _isShutterMonitoringRunning;
            set
            {
                if (_isShutterMonitoringRunning != value)
                {
                    _isShutterMonitoringRunning = value;
                    RaisePropertyChanged();
                }
            }
        }        

        private double _increment;

        public double Increment
        {
            get => _increment;
            set
            {
                if (_increment != value)
                {
                    _increment = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _incrementShutterMonitoring;
        public double ShutterMonitoringIncrement
        {
            get => _incrementShutterMonitoring;
            set
            {
                if (_incrementShutterMonitoring != value)
                {
                    _incrementShutterMonitoring = value;
                    RaisePropertyChanged();
                }
            }
        }

        private RelayCommand _abortCalibrateCommand;

        public RelayCommand AbortCalibrateCommand
        {
            get
            {
                return _abortCalibrateCommand ?? (_abortCalibrateCommand = new RelayCommand(
                    () =>
                    {
                        _abortWait = true;
                        IsCalibrateRunning = false;
                        StopCalibrate();
                    }));
            }
        }

        private RelayCommand _runCalibrateCommand;

        public RelayCommand RunCalibrateCommand
        {
            get
            {
                return _runCalibrateCommand ?? (_runCalibrateCommand = new RelayCommand(
                    () =>
                    {
                        _abortWait = false;
                        IsCalibrateRunning = true;
                        var calibType = (CalibrationType.ToString() == ECalibrationType.Custom.ToString()) ? ECalibrationType.Custom : ECalibrationType.Auto;

                        if (NumberAverages <= 0)
                        {
                            throw new Exception("NumberAverages is null");
                        }

                        _dtnow = DateTime.Now;
                        if (calibType == ECalibrationType.Custom)
                        {
                            Task.Run(() => RunCustomCalibrate(true));
                        }
                        else
                        {
                            Task.Run(() => RunAutoCalibrate());
                        }
                    }));
            }
        }

        private RelayCommand<bool> _widePowerOnCommand;

        public RelayCommand<bool> WidePowerOnCommand
        {
            get
            {
                return _widePowerOnCommand ?? (_widePowerOnCommand = new RelayCommand<bool>(
                    isChecked =>
                    {
                        Task.Run(() => _supervisor.MppcPowerOn(MppcCollector.WIDE, isChecked));
                    }));
            }
        }

        private RelayCommand<bool> _narrowPowerOnCommand;

        public RelayCommand<bool> NarrowPowerOnCommand
        {
            get
            {
                return _narrowPowerOnCommand ?? (_narrowPowerOnCommand = new RelayCommand<bool>(
                    isChecked =>
                    {
                        Task.Run(() => _supervisor.MppcPowerOn(MppcCollector.NARROW, isChecked));
                    }));
            }
        }

        private RelayCommand _wideSetOutputVoltageCommand;

        public RelayCommand WideSetOutputVoltageCommand
        {
            get
            {
                return _wideSetOutputVoltageCommand ?? (_wideSetOutputVoltageCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.MppcSetOutputVoltage(MppcCollector.WIDE, WideOutputVoltage));

                        Task.Run(() => _supervisor.InitializeUpdate());
                    }));
            }
        }

        private RelayCommand _narrowSetOutputVoltageCommand;

        public RelayCommand NarrowSetOutputVoltageCommand
        {
            get
            {
                return _narrowSetOutputVoltageCommand ?? (_narrowSetOutputVoltageCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.MppcSetOutputVoltage(MppcCollector.NARROW, NarrowOutputVoltage));

                        Task.Run(() => _supervisor.InitializeUpdate());
                    }));
            }
        }

        private RelayCommand<bool> _mppcManageRelayCommand;

        public RelayCommand<bool> MppcManageRelayCommand
        {
            get
            {
                return _mppcManageRelayCommand ?? (_mppcManageRelayCommand = new RelayCommand<bool>(
                     isChecked =>
                     {
                         Task.Run(() => _supervisor.MppcManageRelay(MppcCollector.WIDE, isChecked));
                     }));
            }
        }

        private RelayCommand _runShutterMonitoringCommand;

        public RelayCommand RunShutterMonitoringCommand
        {
            get
            {
                return _runShutterMonitoringCommand ?? (_runShutterMonitoringCommand = new RelayCommand(
                    () =>
                    {
                        IsShutterMonitoringRunning = true;
                        Task.Run(() => ShutterMonitoring());
                    }));
            }
        }

        private RelayCommand _abortShutterMonitoringCommand;

        public RelayCommand AbortShutterMonitoringCommand
        {
            get
            {
                return _abortShutterMonitoringCommand ?? (_abortShutterMonitoringCommand = new RelayCommand(
                    () =>
                    {
                        _errorShutterMonitoring = true;
                        _streamWriterShutter.WriteLine("Aborted", _culture);
                        StopShutterMonitoring();
                    }));
            }
        }

        private RelayCommand _startDarkAdjustHsCommand;

        public RelayCommand StartDarkAdjustHsCommand
        {
            get
            {
                return _startDarkAdjustHsCommand ?? (_startDarkAdjustHsCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowermeterStartDarkAdjust(PowerIlluminationFlow.HS));
                    }));
            }
        }

        private RelayCommand _startDarkAdjustHtCommand;

        public RelayCommand StartDarkAdjustHtCommand
        {
            get
            {
                return _startDarkAdjustHtCommand ?? (_startDarkAdjustHtCommand = new RelayCommand(
                    () =>
                    {
                        Task.Run(() => _supervisor.PowermeterStartDarkAdjust(PowerIlluminationFlow.HT));
                    }));
            }
        }

        #endregion RelayCommands

        public async Task<bool> WaitAngleReadyAsync(double dtimeOut_ms, double target_dg, BeamShaperFlow angleFlow, double espilon_dg)
        {
            var sw = new Stopwatch();
            sw.Start();

            bool isAngleReach = false;
            double current_dg = -666.6;

            Action<Proxy.RotatorsKitCalibration.AttenuationPositionChangedMessage> getAttenuationAngleAction = m => { current_dg = m.AttenuationPosition; };
            Action<Proxy.RotatorsKitCalibration.PolarisationPositionChangedMessage> getPolarAngleAction = m => { current_dg = m.PolarisationPosition; };
            if (angleFlow == BeamShaperFlow.Attenuation)
            {
                current_dg = LastAttenuationPosition;
                Messenger.Register<Proxy.RotatorsKitCalibration.AttenuationPositionChangedMessage>(this, getAttenuationAngleAction);
            }
            else
            {
                current_dg = _lastPolarPosition;
                Messenger.Register<Proxy.RotatorsKitCalibration.PolarisationPositionChangedMessage>(this, getPolarAngleAction);
            }
            await Task.Delay(100).ConfigureAwait(false);

            double dDiff = target_dg - current_dg;
            isAngleReach = (-espilon_dg <= dDiff && dDiff <= espilon_dg);
            while (!_abortWait && !isAngleReach && sw.ElapsedMilliseconds < dtimeOut_ms)
            {
                dDiff = target_dg - current_dg;
                isAngleReach = (-espilon_dg <= dDiff && dDiff <= espilon_dg);
                // wait for angle
                await Task.Delay(500).ConfigureAwait(false);
            }
            sw.Stop();

            if (angleFlow == BeamShaperFlow.Attenuation)
                Messenger.Unregister<Proxy.RotatorsKitCalibration.AttenuationPositionChangedMessage>(this, getAttenuationAngleAction);
            else
                Messenger.Unregister<Proxy.RotatorsKitCalibration.PolarisationPositionChangedMessage>(this, getPolarAngleAction);
            return (sw.ElapsedMilliseconds <= dtimeOut_ms) && !_abortWait;
        }

        private async Task RunCustomCalibrate(bool runMono = false)
        {
         
            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            string sp = ";";
            double incMsg = 0;

            var powerIlluminationFlow = LutNames[_selectedIndexLutName].PowerIlluminationFlow.ToString();
            var waveplateAnglesPolarisation = LutNames[_selectedIndexLutName].WaveplateAnglesPolarisation.ToString();

            double calibrateStartAngle = (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? HsRangeCalibrate.CalibrateStartAngle : HtRangeCalibrate.CalibrateStartAngle;
            double calibrateEndAngle = (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? HsRangeCalibrate.CalibrateEndAngle : HtRangeCalibrate.CalibrateEndAngle;
            double angleSteps = (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? HsRangeCalibrate.AngleSteps : HtRangeCalibrate.AngleSteps;
            double epsilon_dg = 0.01; // in degree


            if (!IsParamsValid(powerIlluminationFlow))
            {
                StopCalibrate(runMono);
                return;
            }

            double polarAngle;
            if (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString())
            {
                polarAngle = (waveplateAnglesPolarisation == WaveplateAnglesPolarisation.S_Polar.ToString()) ? HSS_PolarAngle : HSP_PolarAngle;
            }
            else if (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HT.ToString())
            {
                polarAngle = (waveplateAnglesPolarisation == WaveplateAnglesPolarisation.S_Polar.ToString()) ? HTS_PolarAngle : HTP_PolarAngle;
            }
            else
            {
                throw new Exception("Unknown power illuminationFflow");
            }

           

            int MaxResend = 3;
            int count = 0;
            bool isPolarReached = await WaitAngleReadyAsync(2500, polarAngle, BeamShaperFlow.Polarisation, epsilon_dg);
            while (!_abortWait && !isPolarReached && count < MaxResend)
            {
                count++;
                _supervisor.MoveAbsPosition(BeamShaperFlow.Polarisation, polarAngle);
                 isPolarReached = await WaitAngleReadyAsync(2500, polarAngle, BeamShaperFlow.Polarisation, epsilon_dg);
            }
            if (!isPolarReached)
            {
                if (!_abortWait)
                    throw new Exception($"Polar Angle Not reached within time");
                else
                {
                    StopCalibrate(runMono);
                    return;
                }
            }

            double intiangle = calibrateStartAngle - 0.015;
            _supervisor.MoveAbsPosition(BeamShaperFlow.Attenuation, intiangle);
            bool isAttModeReached = await WaitAngleReadyAsync(3500, intiangle, BeamShaperFlow.Attenuation, 2*epsilon_dg);
            count = 0;
            while (!_abortWait && !isAttModeReached && count < MaxResend)
            {
                count++;
                _supervisor.MoveAbsPosition(BeamShaperFlow.Attenuation, intiangle);
                isAttModeReached = await WaitAngleReadyAsync(2000, intiangle, BeamShaperFlow.Attenuation, 2*epsilon_dg);
            }
            if (!isAttModeReached)
            {
                if (!_abortWait)
                    throw new Exception($"Attenuation Start Angle Not reached within time");
                else
                {
                    StopCalibrate(runMono);
                    return;
                }
            }

            if (ShutterIrisPosition == "CLOSE")
            {
                _supervisor.OpenShutterCommand();

                _supervisor.PowermeterEnableAutoRange(LutNames[_selectedIndexLutName].PowerIlluminationFlow, true);
            }

            var currentModeFlow = LutNames[_selectedIndexLutName].PowerIlluminationFlow;
            List<double> Qpowerlas_mW = new List<double>(512);
            Action<Proxy.RotatorsKitCalibration.PowerLaserChangedMessage> LaserPowAction = m =>
            {
                Qpowerlas_mW.Add(m.Power); //mW
            };

            List<double> QTemplas = new List<double>(512);
            Action<Proxy.RotatorsKitCalibration.LaserTemperatureChangedMessage> LaserTempAction = m =>
            {
                QTemplas.Add(m.LaserTemperature);
            };

            List<double> QPowOnePercent_mW= new List<double>(512);
            Action<Proxy.RotatorsKitCalibration.PowermeterPowerChangedMessage> PowermetersPow = m =>
            {
                if (m.Flow == currentModeFlow)
                {
                    QPowOnePercent_mW.Add(m.Power); // mW
                }
            };

            List<double> QCurrentOnePercent_mA = new List<double>(512);
            Action<Proxy.RotatorsKitCalibration.PowermeterCurrentChangedMessage> PowermetersCurrent = m =>
            {
                if (m.Flow == currentModeFlow)
                {
                    QCurrentOnePercent_mA.Add(m.Current_mA); // Attention we expect mA !!! need to x1000 if we receveid A -need also to change variable name)

                }
            };
            try
            {
                if (!Directory.Exists(DefaultDirectory))
                {
                    Directory.CreateDirectory(DefaultDirectory);
                }

                _streamWriter = File.CreateText(DefaultDirectory + SaveFilenameLut + "_" + LutNames[_selectedIndexLutName].Name + "_" +
                                                _dtnow.ToString("yyyyMMdd-HHmmss") + CSV_EXT);

                String lutName = LutNames[_selectedIndexLutName].Name;
                double Responsivity_mAperW = 1.0;//mA/W
                double Rfactor = 1.0; 
                switch (LutNames[_selectedIndexLutName].PowerIlluminationFlow)
                {
                    case PowerIlluminationFlow.HS:
                        Responsivity_mAperW = PowermeterResponsivityHs;
                        Rfactor = LutNames[_selectedIndexLutName].WaveplateAnglesPolarisation == WaveplateAnglesPolarisation.S_Polar ? HSS_RFactor : HSP_RFactor;
                        break;
                    case PowerIlluminationFlow.HT: 
                        Responsivity_mAperW = PowermeterResponsivityHt;
                        Rfactor = LutNames[_selectedIndexLutName].WaveplateAnglesPolarisation == WaveplateAnglesPolarisation.S_Polar ? HTS_RFactor : HTP_RFactor;
                        break;
                    case PowerIlluminationFlow.Unknown: break;
                }
                _streamWriter.WriteLine($"Time{sp}Angle Att(deg){sp}{lutName} Power1% (mW){sp}Laser (mW){sp}TLaser (°C){sp}{lutName} Current1% (mA){sp}TargetAngle{sp}" +
                        $"Resp. (mA/W){sp}{Responsivity_mAperW}{sp}" +
                        $"RFactor{sp}{Rfactor}");

                // Stabilize Open shutter
                await Task.Delay(1000).ConfigureAwait(true);

                // compute approximative delay to wait at each step
                double approxOneStepDelay = 25.0;
                double dStepWaitDelay_ms = approxOneStepDelay * (double)NumberAverages;
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)dStepWaitDelay_ms);
                double numberIter =( (calibrateEndAngle - calibrateStartAngle) / angleSteps) + 1; // on inclut calibrateEndAngle
                for (double angle= calibrateStartAngle; angle <= calibrateEndAngle; angle += angleSteps)
                {
                    if (!IsCalibrateRunning || _abortWait)
                        return;

                    // reaching angle attenuation position

                    _supervisor.MoveAbsPosition(BeamShaperFlow.Attenuation, angle);
                    isAttModeReached = await WaitAngleReadyAsync(800, angle, BeamShaperFlow.Attenuation, epsilon_dg);
                    count = 0;
                    while (!_abortWait && !isAttModeReached && count < MaxResend)
                    {
                        count++;
                        _supervisor.MoveAbsPosition(BeamShaperFlow.Attenuation, angle);
                        isAttModeReached = await WaitAngleReadyAsync(1000, angle, BeamShaperFlow.Attenuation, epsilon_dg);
                    }
                    if (!isAttModeReached)
                    {
                        if (!_abortWait)
                            _streamWriter.WriteLine($"0{sp}{angle}{sp}{sp}{sp}{sp}{sp}# Last angle={LastAttenuationPosition}");
                       // move to next position
                        continue;
                    }

                    // clear queues
                    Qpowerlas_mW.Clear();
                    QTemplas.Clear();
                    QPowOnePercent_mW.Clear();
                    QCurrentOnePercent_mA.Clear();
                    Qpowerlas_mW.Add(PowerLaser);
                    QTemplas.Add(LaserTemperature);
                    await Task.Delay(50).ConfigureAwait(true);

                    Messenger.Register<Proxy.RotatorsKitCalibration.PowerLaserChangedMessage>(this, LaserPowAction);
                    Messenger.Register<Proxy.RotatorsKitCalibration.LaserTemperatureChangedMessage>(this, LaserTempAction);
                    Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterPowerChangedMessage>(this, PowermetersPow);
                    Messenger.Register<Proxy.RotatorsKitCalibration.PowermeterCurrentChangedMessage>(this, PowermetersCurrent);

                    await Task.Delay(ts).ConfigureAwait(true);

                    Messenger.Unregister<Proxy.RotatorsKitCalibration.PowerLaserChangedMessage>(this, LaserPowAction);
                    Messenger.Unregister<Proxy.RotatorsKitCalibration.LaserTemperatureChangedMessage>(this, LaserTempAction);
                    Messenger.Unregister<Proxy.RotatorsKitCalibration.PowermeterPowerChangedMessage>(this, PowermetersPow);
                    Messenger.Unregister<Proxy.RotatorsKitCalibration.PowermeterCurrentChangedMessage>(this, PowermetersCurrent);

                    if (IsCalibrateRunning && !_abortWait)
                    {
                        await Task.Delay(100).ConfigureAwait(true);
                       
                        double TempLaser_C = QTemplas.Count == 0 ? 0.0 : QTemplas.Average();
                        double Plaser_mW = Qpowerlas_mW.Count == 0 ? 0.0 : Qpowerlas_mW.Average();
                        double Power_mW = QPowOnePercent_mW.Count == 0 ? 0.0 : QPowOnePercent_mW.Average();
                        double PowCurrent_mA = QCurrentOnePercent_mA.Count == 0 ? 0.0 : QCurrentOnePercent_mA.Average();

                        _streamWriter.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")}{sp}{LastAttenuationPosition}{sp}{Power_mW}{sp}{Plaser_mW}{sp}{TempLaser_C}{sp}{PowCurrent_mA}{sp}{angle}{sp}");                       
                        _streamWriter.Flush();

                        incMsg++;
                        // Update progress bar
                        Increment = (incMsg * 100.0) / numberIter;
                        IncLabel = string.Format("{0:0.00} %", Increment);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in StartCalibrate v2 : " + ex.Message);
            }
            finally
            {
                Messenger.Unregister<Proxy.RotatorsKitCalibration.PowerLaserChangedMessage>(this, LaserPowAction);
                Messenger.Unregister<Proxy.RotatorsKitCalibration.LaserTemperatureChangedMessage>(this, LaserTempAction);
                Messenger.Unregister<Proxy.RotatorsKitCalibration.PowermeterPowerChangedMessage>(this, PowermetersPow);
                Messenger.Unregister<Proxy.RotatorsKitCalibration.PowermeterCurrentChangedMessage>(this, PowermetersCurrent);

                StopCalibrate(runMono);
            }
        }

        private void RunAutoCalibrate()
        {
            for (int i = 0; i <= MAX_STEP - 1; i++)
            {
                Step = string.Format("{0}{1}{2}", i + 1, "/", MAX_STEP);
                SelectedIndexLutName = i;
                Task.Run(() => RunCustomCalibrate()).GetAwaiter().GetResult();
            }

            _supervisor.CloseShutterCommand();
            IsCalibrateRunning = false;
        }

        private void StopCalibrate(bool trigEndRunning = false)
        {
            _supervisor.PowermeterEnableAutoRange(PowerIlluminationFlow.HS, false);
            _supervisor.PowermeterEnableAutoRange(PowerIlluminationFlow.HT, false);

            _supervisor.CloseShutterCommand();
            
            Increment = 0;
            IncLabel = string.Format("{0:0.00} %", Increment);
            if (_streamWriter != null)
                _streamWriter.Close();

            if (trigEndRunning)
                IsCalibrateRunning = false;
        }

        private bool IsParamsValid(string powerIlluminationFlow)
        {
            ESelectedLut selectedLut = SelectedLut();
            if (selectedLut == ESelectedLut.None)
            {
                MessageBox.Show("Missing range");
                return false;
            }

            if (selectedLut.ToString() != powerIlluminationFlow &&
                selectedLut != ESelectedLut.Both)
            {
                MessageBox.Show("Missing range");
                return false;
            }

            if (NumberAverages == 0)
            {
                MessageBox.Show("Number of average is null");
                return false;
            }

            if (SaveFilenameLut.IsNullOrEmpty())
            {
                MessageBox.Show("Missing file name");
                return false;
            }

            return true;
        }

        #region OLD_RunLut
        // Original Calibrate Methode with locks
        private async Task RunCustomCalibrate_OLD(bool RunMono = false)
        {
            string delimiter = ";";
            double incMsg = 0;

            var powerIlluminationFlow = LutNames[_selectedIndexLutName].PowerIlluminationFlow.ToString();
            var waveplateAnglesPolarisation = LutNames[_selectedIndexLutName].WaveplateAnglesPolarisation.ToString();

            double calibrateStartAngle = (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? HsRangeCalibrate.CalibrateStartAngle : HtRangeCalibrate.CalibrateStartAngle;
            double calibrateEndAngle = (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? HsRangeCalibrate.CalibrateEndAngle : HtRangeCalibrate.CalibrateEndAngle;
            double angleSteps = (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString()) ? HsRangeCalibrate.AngleSteps : HtRangeCalibrate.AngleSteps;

            if (!IsParamsValid(powerIlluminationFlow))
            {
                StopCalibrate(RunMono);
                return;
            }

            if (ShutterIrisPosition == "CLOSE")
            {
                _supervisor.OpenShutterCommand();
            }

            double polarAngle;
            if (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HS.ToString())
            {
                polarAngle = (waveplateAnglesPolarisation == WaveplateAnglesPolarisation.S_Polar.ToString()) ? HSS_PolarAngle : HSP_PolarAngle;
            }
            else if (powerIlluminationFlow.ToString() == PowerIlluminationFlow.HT.ToString())
            {
                polarAngle = (waveplateAnglesPolarisation == WaveplateAnglesPolarisation.S_Polar.ToString()) ? HTS_PolarAngle : HTP_PolarAngle;
            }
            else
            {
                throw new Exception("Unknown power illuminationFflow");
            }

            _supervisor.MoveAbsPosition(BeamShaperFlow.Polarisation, polarAngle);
            await Task.Delay(1000);

            _supervisor.MoveAbsPosition(BeamShaperFlow.Attenuation, calibrateStartAngle - 0.01);
            await Task.Delay(2000);

            try
            {
                if (!Directory.Exists(DefaultDirectory))
                {
                    Directory.CreateDirectory(DefaultDirectory);
                }

                _streamWriter = File.CreateText(DefaultDirectory + SaveFilenameLut + "_" + LutNames[_selectedIndexLutName].Name + "_" +
                                                _dtnow.ToString("yyyyMMdd-HHmmss") + CSV_EXT);
                _streamWriter.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", "Time", delimiter, "Angular " + BeamShaperFlow + " (deg)",
                                        delimiter, LutNames[_selectedIndexLutName].PowerIlluminationFlow + " (W)", delimiter, "Laser (mW)",
                                        delimiter, "Polarization : " + LutNames[_selectedIndexLutName].WaveplateAnglesPolarisation), _culture);

                int count = 0;
                double newAttenuationAnglePosition = calibrateStartAngle;
                double numberIter = (calibrateEndAngle - calibrateStartAngle) / angleSteps;
                double oldBeamShaperPosition = calibrateStartAngle;

                lock (_pos_lock)
                {
                    if (_positionQueue == null)
                        _positionQueue = new Queue<double>((int)numberIter);
                    else
                        _positionQueue.Clear();
                }
                double epsilon = 2.0 / (ENCODER_RESOLUTION / TURN); // excluding 2 positions
                Stopwatch sw = new Stopwatch();
                while (_positionQueue.Count <= numberIter)
                {
                    if (!IsCalibrateRunning || _abortWait)
                        return;

                    _supervisor.MoveAbsPosition(BeamShaperFlow.Attenuation, newAttenuationAnglePosition);
                    await Task.Delay(300);

                    var last = 0.0;
                    if (_positionQueue.Count == 0)
                        last = 0;
                    else
                        last = _positionQueue.Last();

                    // queue value
                    lock (_pos_lock)
                    {
                        Console.WriteLine(Convert.ToString(count) + " =" + (newAttenuationAnglePosition - BeamShaperPosition));

                        if (_positionQueue.Count <= numberIter &&
                            ((newAttenuationAnglePosition - BeamShaperPosition) <= epsilon))
                        {
                            _positionQueue.Enqueue(BeamShaperPosition);
                            CreateDataLut(_positionQueue.Last());

                            incMsg++;
                            newAttenuationAnglePosition = calibrateStartAngle + incMsg * angleSteps;

                            // Update progress bar
                            Increment = (incMsg * 100.0) / numberIter;
                            IncLabel = string.Format("{0:0.00} %", Increment);

                            count++;
                        }
                        else
                            sw.Restart();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in StartCalibrate : " + ex.Message);
            }
            finally
            {
                StopCalibrate(RunMono);
            }
        }

        private void CreateDataLut(double attenuationCurrentPosition)
        {
            string delimiter = ";";
            double[] powers = AveragePower();

            _streamWriter.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}", DateTime.Now.ToString("HH:mm:ss:fff"), delimiter,
                                    attenuationCurrentPosition, delimiter, powers[0], delimiter, powers[1]), _culture);
        }

        private double[] AveragePower()
        {
            uint count = 0;

            var powerQueue = new Queue<double>((int)NumberAverages);
            var laserPowerQueue = new Queue<double>((int)NumberAverages);
            
            double[] powers = new double[2];

            double powerIlluminationSum = 0.0;
            double laserPowerSum = 0.0;
            double oldpowerIllumination = 0;
            powerQueue.Clear();
            laserPowerQueue.Clear();

            while (count < (NumberAverages) && IsCalibrateRunning)
            {
                string powerIlluminationFlow = LutNames[_selectedIndexLutName].PowerIlluminationFlow.ToString();
                var powerIllumination_W = (powerIlluminationFlow == PowerIlluminationFlow.HS.ToString()) ? PowermeterRawPowerHs : PowermeterRawPowerHt;
                if (powerQueue.Count < NumberAverages &&
                    oldpowerIllumination != powerIllumination_W)
                {
                    oldpowerIllumination = powerIllumination_W;

                    powerQueue.Enqueue(powerIllumination_W);
                    laserPowerQueue.Enqueue(PowerLaser);
                    count++;
                }
                
            }

            foreach (double power_W in powerQueue)
            {
                powerIlluminationSum = powerIlluminationSum + power_W;
            }

            foreach (double laserPower in laserPowerQueue)
            {
                laserPowerSum = laserPowerSum + laserPower;
            }

            powers[0] = powerIlluminationSum / NumberAverages;
            powers[1] = laserPowerSum / NumberAverages;

            return powers;
        }
        #endregion OLD_RunLut

        #region Shutter monitoring

        private void ShutterMonitoring()
        {
            _errorShutterMonitoring = false;            

            _supervisor.CloseShutterCommand();

            try
            {
                if (!Directory.Exists(DefaultDirectoryMonitoring))
{
                    Directory.CreateDirectory(DefaultDirectoryMonitoring);
                }

                _streamWriterShutter = File.CreateText(DefaultDirectoryMonitoring + SaveFilenameShutter + "_"+ DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt");

                for (int i = 0; i <= NumberOfCycles - 1; i++)
                {
                    if (!IsShutterMonitoringRunning)
                        return;

                    Task.Run(() => RunShutterMonitoring(i)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in StartCalibrate : " + ex.Message);
            }
            finally
            {
                _supervisor.CloseShutterCommand();

                if (!_errorShutterMonitoring)
                    _streamWriterShutter.WriteLine("No errors", _culture);

                StopShutterMonitoring();
            }
        }

        private void StopShutterMonitoring()
        {
            IsShutterMonitoringRunning = false;
            ShutterMonitoringIncrement = 0;
            IncLabelShutterMonitoring = string.Format("{0:0.00} %", ShutterMonitoringIncrement);
            if (_streamWriterShutter != null)
                _streamWriterShutter.Close();
        }

        private async Task RunShutterMonitoring(int cycle)
        {
            string requiredPosition = "OPEN";
            await Task.Delay(1000);
            
            _supervisor.OpenShutterCommand();
            await Task.Delay(500);
            if (ShutterIrisPosition != "OPEN")
            {
                _errorShutterMonitoring = true;
                _streamWriterShutter.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("HH:mm:ss:fff"), "Cycle:" + cycle ,
                                    "Required position:" + requiredPosition, "Position:" + ShutterIrisPosition), _culture);
            }

            if (!IsShutterMonitoringRunning)
                return;

            requiredPosition = "CLOSE";
            await Task.Delay(1000);            

            _supervisor.CloseShutterCommand();
            await Task.Delay(500);
            if (ShutterIrisPosition != "CLOSE")
            {
                _errorShutterMonitoring = true;
                _streamWriterShutter.WriteLine(string.Format("{0} {1} {2} {3}", DateTime.Now.ToString("HH:mm:ss:fff"), "Cycle:" + cycle,
                                    "Required position:" + requiredPosition, "Position:" + ShutterIrisPosition), _culture);
            }

            // Update UI
            StepShutterMonitoring = string.Format("{0}{1}{2}", cycle + 1, "/", NumberOfCycles);            
            _incrementShutterMonitoring = ((cycle + 1) * 100.0) / NumberOfCycles;
            IncLabelShutterMonitoring = string.Format("{0:0.00} %", _incrementShutterMonitoring);
        }

        #endregion
    }

    public class RangeCalibrate
    {
        public double CalibrateStartAngle { get; set; } = 0.0;
        public double CalibrateEndAngle { get; set; } = 0.0;
        public double AngleSteps { get; set; } = 0.1;
    }

    public class LutProfile
    {
        public string Name { get; set; }
        public PowerIlluminationFlow PowerIlluminationFlow { get; set; }
        public WaveplateAnglesPolarisation WaveplateAnglesPolarisation { get; set; }
    }

    public class PowermeterRange
    {
        public string Range { get; set; }        
    }
}
