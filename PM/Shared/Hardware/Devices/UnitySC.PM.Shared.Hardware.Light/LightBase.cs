using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Light;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.Hardware.Light
{
    public enum LightBaseFDCType
    {
        UsageDuration = 0,
    }

    public abstract class LightBase : DeviceBase, ILight, IFDCProvider
    {
        private IGlobalStatusServer _globalStatusServiceCallback;

        public LightConfig Config;
        private FDCManager _fdcManager;
        private readonly object _fdcManagerLock = new object();

        protected FDCManager FdcManager
        {
            get
            {
                // Optim DCL (double-check locking) + thread safe
                if (_fdcManager == null)
                {
                    lock (_fdcManagerLock)
                    {
                        if (_fdcManager == null) 
                        {
                            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
                        }
                    }
                }
                return _fdcManager;
            }
        }

        private Dictionary<LightBaseFDCType, string> _dicoFDCTypeToFDCName;
        public void SetFDCTypeToNameDico(Dictionary<LightBaseFDCType, string> dico) { _dicoFDCTypeToFDCName = dico; }
        private readonly List<string> _providerFDCNames = new List<string>();

        #region PersitentData

        private PersistentFDCTimeSpan _fdcLightUsage;

        #endregion PersitentData

        #region NonPersitentData

        // ANA_LightUsage_<shortLightName>  to find ShortLightName check AnaHardwareManager.InitializeAnaLightFDCProviderDico() as exemple
        private string _fdcNameLightUsage;
        private DateTime _fdcLightLastPowerOnDate = DateTime.MinValue;

        #endregion NonPersitentData
        private bool isUsageNeededUpdate() { return (_fdcLightUsage != null) && !(_fdcLightLastPowerOnDate.Equals(DateTime.MinValue)); }
   
     
        public virtual bool IsMainLight => Config.IsMainLight;

        // For test purpose only
        public LightBase() : base(null, ClassLocator.Default.GetInstance<ILogger>())
        {
          
        }

        public LightBase(LightConfig config, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(globalStatusServer, logger)
        {
            Config = config;
            _globalStatusServiceCallback = globalStatusServer;
        }

        public virtual void Init()
        {
            Name = Config.Name;
            DeviceID = Config.DeviceID;
            if (!double.IsNaN(Config.DefaultIntensity))
                SetIntensity(Config.DefaultIntensity);
            Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract double GetIntensity();

        /// <summary>
        /// Sets the light intensity. Value should be expressed between 0 and 100, handled as a percentage.
        /// </summary>
        public virtual void SetIntensity(double intensity)
        {
            if (isUsageNeededUpdate())
            {
                // add usage duration 
                _fdcLightUsage.Timespan = _fdcLightUsage.Timespan.Add(DateTime.UtcNow - _fdcLightLastPowerOnDate);
                FdcManager.SetPersistentFDCData(_fdcLightUsage);
            }
            _fdcLightLastPowerOnDate = (intensity > 0) ? DateTime.UtcNow : DateTime.MinValue;

            // TODO : We should use Messenger instead of referring directly ILightServiceCallbackProxy on
            // hardware side. Same thing others devices (Axes, ...)
            var lightServiceCallback = ClassLocator.Default.GetInstance<ILightServiceCallbackProxy>();
            lightServiceCallback.LightIntensityChanged(DeviceID, intensity);
        }

        public FDCData GetFDC(string fdcName)
        {
            FDCData fdc = null;

            if (!string.IsNullOrEmpty(_fdcNameLightUsage) && (fdcName == _fdcNameLightUsage))
            {
                if (isUsageNeededUpdate())
                {
                    // add usage duration 
                    _fdcLightUsage.Timespan = _fdcLightUsage.Timespan.Add(DateTime.UtcNow - _fdcLightLastPowerOnDate);
                    FdcManager.SetPersistentFDCData(_fdcLightUsage);
                    _fdcLightLastPowerOnDate = DateTime.UtcNow;
                }
                fdc = FDCData.MakeNew(_fdcNameLightUsage, _fdcLightUsage.Timespan.TotalHours);
            }

            return fdc;
        }

        public void ResetFDC(string fdcName)
        {
            if (string.IsNullOrEmpty(fdcName))
                return;

            if (!string.IsNullOrEmpty(_fdcNameLightUsage) && (fdcName == _fdcNameLightUsage))
            {
                _fdcLightUsage.Timespan = new TimeSpan(0,0,0);
                FdcManager.SetPersistentFDCData(_fdcLightUsage);
            }
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            if(string.IsNullOrEmpty(fdcName))
                return;

            if (!string.IsNullOrEmpty(_fdcNameLightUsage) && (fdcName == _fdcNameLightUsage))
            {
                if (persistentFDCData is PersistentFDCTimeSpan pfdctimespan)
                {
                    _fdcLightUsage = pfdctimespan;
                }
            }
        }

        public void StartFDCMonitor()
        {
            if (_fdcLightUsage == null && !string.IsNullOrEmpty(_fdcNameLightUsage))
            {
                _fdcLightUsage = new PersistentFDCTimeSpan(_fdcNameLightUsage);
            }

            var fdcs = new List<FDCData>()
            {
                FDCData.MakeNew(_fdcNameLightUsage, _fdcLightUsage.Timespan.TotalHours)
            };

            FdcManager.SendFDCs(fdcs);
        }

        private void InitFDCNames()
        {
            if (_dicoFDCTypeToFDCName == null)
            {
                // dictionnary has not been set by PM server previously to register- so FDC will be skipped
                Logger.Warning($"Missing FDC Type to Names Dictionnary for Light <{Name}>");
                return;
            }

            if (! _dicoFDCTypeToFDCName.ContainsKey(LightBaseFDCType.UsageDuration))
            {
                //Missing type
                Logger.Warning($"Missing FDC Type [UsageDuration] in Dictionnary for Light <{Name}>");
                return;
            }

            _fdcNameLightUsage = _dicoFDCTypeToFDCName[LightBaseFDCType.UsageDuration];
        }

        public void Register()
        {
            InitFDCNames();

            if (!string.IsNullOrEmpty(_fdcNameLightUsage))
            {
                _providerFDCNames.Add(_fdcNameLightUsage);
            }

            FdcManager.RegisterFDCProvider(this, _providerFDCNames);
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            // Nothing to do
        }
    }
}
