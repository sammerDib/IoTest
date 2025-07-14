using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Helpers;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Host
{
    public class ChamberFDCs : IDisposable, IFDCProvider
    {
        private const string ErrorFilePath = "errorAppCrash.txt";

        private FDCManager _fdcManager;

        private List<Thread> _monitorThreads = new List<Thread>(5);

        private PerformanceCounter _cpuCounter;
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        private ILogger _logger;

        private string _performanceCounterCPUUsageCategory;
        private string _performanceCounterCPUusageLabel;
        private bool _performanceCounterCPUUsageExists;

        #region FDC Name Enum Association

        private enum ProviderFDCs
        {
            ServerUptime,               //ANA_Server_Uptime,
            ServerStartCounter,         //ANA_Server_StartCounter,
            ServerFatalErrorCounter,    //ANA_Server_FatalErrorCounter,

            CPU_Usage,                  //CPU_PCANA,
            CPU_UsageOnTimePeriod48h,   //CPU_PCANA_AVGon48H, // TODO check if 48 could be set depnding config, what if we have several instance of this kind with several period

            MemUsage,                   //MemUsage_PCANA,
            MemUsageOnTimePeriod48h,    //MemUsage_PCANA_AVGon48H,

            DiskC_Usage,                //DiskCUsage_PCANA,
            DiskC_Free,                 //DiskCFree_PCANA,
            DiskC_PercentFree,          //DiskCPercentFree_PCANA,

            DiskD_Usage,                //DiskDUsage_PCANA,
            DiskD_Free,                 //DiskDFree_PCANA,
            DiskD_PercentFree,          //DiskDPercentFree_PCANA,

            DiskE_Usage,                //DiskEUsage_PCANA,
            DiskE_Free,                 //DiskEFree_PCANA,
            DiskE_PercentFree,          //DiskEPercentFree_PCANA,

            LightMaintenanceCountdown_White,       //ANA_LightMaintenanceCountdown_White
            LightMaintenanceCountdown_Red,         //ANA_LightMaintenanceCountdown_Red
            LightMaintenanceCountdown_NIR,         //ANA_LightMaintenanceCountdown_NIR
            LightMaintenanceCountdown_BottomNIR,   //ANA_LightMaintenanceCountdown_BottomNIR
        }

        private Dictionary<string, ProviderFDCs> _providerFDCNames = new Dictionary<string, ProviderFDCs>();

        private List<string> GetProviderFDCNames()
        {
            return _providerFDCNames.Keys.ToList();
        }

        private static string ToFDCName(ProviderFDCs fdc)
        {
            string fdcName = null;
            switch (fdc)
            {
                case ProviderFDCs.ServerUptime:
                    fdcName = "ANA_Server_Uptime";
                    break;

                case ProviderFDCs.ServerStartCounter:
                    fdcName = "ANA_Server_StartCounter";
                    break;

                case ProviderFDCs.ServerFatalErrorCounter:
                    fdcName = "ANA_Server_FatalErrorCounter";
                    break;

                case ProviderFDCs.CPU_Usage:
                    fdcName = "CPU_PCANA";
                    break;

                case ProviderFDCs.CPU_UsageOnTimePeriod48h: // TODO check if 48 could be set depnding config, what if we have several instance of this kind with several period
                    fdcName = "CPU_PCANA_AVGon48H";
                    break;

                case ProviderFDCs.MemUsage:
                    fdcName = "MemUsage_PCANA";
                    break;

                case ProviderFDCs.MemUsageOnTimePeriod48h:
                    fdcName = "MemUsage_PCANA_AVGon48H";
                    break;

                case ProviderFDCs.DiskC_Usage:
                    fdcName = "DiskCUsage_PCANA";
                    break;

                case ProviderFDCs.DiskC_Free:
                    fdcName = "DiskCFree_PCANA";
                    break;

                case ProviderFDCs.DiskC_PercentFree:
                    fdcName = "DiskCPercentFree_PCANA";
                    break;

                case ProviderFDCs.DiskD_Usage:
                    fdcName = "DiskDUsage_PCANA";
                    break;

                case ProviderFDCs.DiskD_Free:
                    fdcName = "DiskDFree_PCANA";
                    break;

                case ProviderFDCs.DiskD_PercentFree:
                    fdcName = "DiskDPercentFree_PCANA";
                    break;

                case ProviderFDCs.DiskE_Usage:
                    fdcName = "DiskEUsage_PCANA";
                    break;

                case ProviderFDCs.DiskE_Free:
                    fdcName = "DiskEFree_PCANA";
                    break;

                case ProviderFDCs.DiskE_PercentFree:
                    fdcName = "DiskEPercentFree_PCANA";
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_White:
                    fdcName = "ANA_LightMaintenanceCountdown_White";
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_Red:
                    fdcName = "ANA_LightMaintenanceCountdown_Red";
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_NIR:
                    fdcName = "ANA_LightMaintenanceCountdown_NIR";
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_BottomNIR:
                    fdcName = "ANA_LightMaintenanceCountdown_BottomNIR";
                    break;
            }
            return fdcName;
        }

        private void InitProviderFDCNames()
        {
            var enumlist = Enum.GetValues(typeof(ProviderFDCs)).Cast<ProviderFDCs>().ToList();
            if (!HelperFDCDriveInfo.IsDriveValid("C"))
            {
                enumlist.Remove(ProviderFDCs.DiskC_Usage);
                enumlist.Remove(ProviderFDCs.DiskC_Free);
                enumlist.Remove(ProviderFDCs.DiskC_PercentFree);
            }
            if (!HelperFDCDriveInfo.IsDriveValid("D"))
            {
                enumlist.Remove(ProviderFDCs.DiskD_Usage);
                enumlist.Remove(ProviderFDCs.DiskD_Free);
                enumlist.Remove(ProviderFDCs.DiskD_PercentFree);
            }
            if (!HelperFDCDriveInfo.IsDriveValid("E"))
            {
                enumlist.Remove(ProviderFDCs.DiskE_Usage);
                enumlist.Remove(ProviderFDCs.DiskE_Free);
                enumlist.Remove(ProviderFDCs.DiskE_PercentFree);
            }

            foreach (var fdc in enumlist)
            {
                _providerFDCNames.Add(ToFDCName(fdc), fdc);
            }
        }

        #endregion FDC Name Enum Association

        #region NonPersitentData

        private DateTime _upTimeSinceStart_dt = DateTime.Now;

        #endregion NonPersitentData

        #region PersitentData

        private PersistentFDCCounter<UInt64> _pfdc_StartCounter;
        private PersistentFDCCounter<UInt64> _pfdc_FatalErrorCounter;

        private PersistentWindowTimeData<byte> _pfdc_windowTimeCPU_48h;
        private PersistentWindowTimeData<byte> _pfdc_windowMemory_48h;

        private PersitentFDCCountdown _pfdcLightMaintenanceCountdown_White;
        private PersitentFDCCountdown _pfdcLightMaintenanceCountdown_Red;
        private PersitentFDCCountdown _pfdcLightMaintenanceCountdown_NIR;
        private PersitentFDCCountdown _pfdcLightMaintenanceCountdown_BottomNIR;

        #endregion PersitentData

        public ChamberFDCs()
        {
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
            _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();

            var joinTasks = new List<Task<bool>>(_monitorThreads.Count);
            int i = 0;
            int waitTimeOut_ms = 2000;
            foreach (var mthread in _monitorThreads)
            {
                if (mthread != null && mthread.IsAlive)
                {
                    joinTasks.Add(Task.Run(() => mthread.Join(waitTimeOut_ms)));
                }
                ++i;
            }
            Task.WaitAll(joinTasks.ToArray());

            _cpuCounter?.Close();
            _monitorThreads.Clear();
        }

        public FDCData GetFDC(string fdcName)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return null;

            FDCData fdcdata = null;
            switch (enumFdc)
            {
                case ProviderFDCs.ServerUptime:
                    var ts = DateTime.Now - _upTimeSinceStart_dt;
                    fdcdata = FDCData.MakeNew(fdcName, ts);
                    break;

                case ProviderFDCs.ServerStartCounter:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdc_StartCounter.Counter);
                    break;

                case ProviderFDCs.ServerFatalErrorCounter:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdc_FatalErrorCounter.Counter);
                    break;

                case ProviderFDCs.CPU_Usage:
                    fdcdata = FDCData.MakeNew(fdcName, (int)Math.Round(GetCPUUsage(_cpuCounter)), "%");
                    break;

                case ProviderFDCs.CPU_UsageOnTimePeriod48h:
                    fdcdata = FDCData.MakeNew(fdcName, (HelperFDCWindowTime.ComputeDblAverage(_pfdc_windowTimeCPU_48h)), "%");
                    break;

                case ProviderFDCs.MemUsage:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFDCSystemInfo.GetMemoryUsagePercentage(), "%");
                    break;

                case ProviderFDCs.MemUsageOnTimePeriod48h:
                    fdcdata = FDCData.MakeNew(fdcName, (HelperFDCWindowTime.ComputeDblAverage(_pfdc_windowMemory_48h)), "%");
                    break;

                /// Drive Disk C
                case ProviderFDCs.DiskC_Usage:
                    {
                        var diskusage = HelperFDCDriveInfo.DiskUsage("C");
                        if (diskusage.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskusage.Value, "Mo");
                        }
                    }
                    break;

                case ProviderFDCs.DiskC_Free:
                    {
                        var diskfree = HelperFDCDriveInfo.DiskFree("C");
                        if (diskfree.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskfree.Value, "Mo");
                        }
                    }
                    break;

                case ProviderFDCs.DiskC_PercentFree:
                    {
                        var diskpctfree = HelperFDCDriveInfo.DiskPercentFree("C");
                        if (diskpctfree.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskpctfree.Value);
                        }
                    }
                    break;

                /// Drive Disk D
                case ProviderFDCs.DiskD_Usage:
                    {
                        var diskusage = HelperFDCDriveInfo.DiskUsage("D");
                        if (diskusage.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskusage.Value, "Mo");
                        }
                    }
                    break;

                case ProviderFDCs.DiskD_Free:
                    {
                        var diskfree = HelperFDCDriveInfo.DiskFree("D");
                        if (diskfree.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskfree.Value, "Mo");
                        }
                    }
                    break;

                case ProviderFDCs.DiskD_PercentFree:
                    {
                        var diskpctfree = HelperFDCDriveInfo.DiskPercentFree("D");
                        if (diskpctfree.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskpctfree.Value);
                        }
                    }
                    break;

                /// Drive Disk E
                case ProviderFDCs.DiskE_Usage:
                    {
                        var diskusage = HelperFDCDriveInfo.DiskUsage("E");
                        if (diskusage.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskusage.Value, "Mo");
                        }
                    }
                    break;

                case ProviderFDCs.DiskE_Free:
                    {
                        var diskfree = HelperFDCDriveInfo.DiskFree("E");
                        if (diskfree.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskfree.Value, "Mo");
                        }
                    }
                    break;

                case ProviderFDCs.DiskE_PercentFree:
                    {
                        var diskpctfree = HelperFDCDriveInfo.DiskPercentFree("E");
                        if (diskpctfree.HasValue)
                        {
                            fdcdata = FDCData.MakeNew(fdcName, diskpctfree.Value);
                        }
                    }
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_White:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdcLightMaintenanceCountdown_White.CountdownHours, "h");
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_Red:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdcLightMaintenanceCountdown_Red.CountdownHours, "h");
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_NIR:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdcLightMaintenanceCountdown_NIR.CountdownHours, "h");
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_BottomNIR:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdcLightMaintenanceCountdown_BottomNIR.CountdownHours, "h");
                    break;
            }
            return fdcdata;
        }

        public void ResetFDC(string fdcName)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return;

            switch (enumFdc)
            {
                case ProviderFDCs.CPU_UsageOnTimePeriod48h:
                    _pfdc_windowTimeCPU_48h.WindowTimeData.Clear();
                    _fdcManager.SetPersistentFDCData(_pfdc_windowTimeCPU_48h);
                    break;

                case ProviderFDCs.MemUsageOnTimePeriod48h:
                    _pfdc_windowMemory_48h.WindowTimeData.Clear();
                    _fdcManager.SetPersistentFDCData(_pfdc_windowMemory_48h);
                    break;

                case ProviderFDCs.ServerStartCounter:
                    _pfdc_StartCounter.Counter = 0uL;
                    _fdcManager.SetPersistentFDCData(_pfdc_StartCounter);
                    break;

                case ProviderFDCs.ServerFatalErrorCounter:
                    _pfdc_FatalErrorCounter.Counter = 0uL;
                    _fdcManager.SetPersistentFDCData(_pfdc_FatalErrorCounter);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_White:
                    _pfdcLightMaintenanceCountdown_White.ResetDate = DateTime.Now;
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_White);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_Red:
                    _pfdcLightMaintenanceCountdown_Red.ResetDate = DateTime.Now;
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_Red);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_NIR:
                    _pfdcLightMaintenanceCountdown_NIR.ResetDate = DateTime.Now;
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_NIR);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_BottomNIR:
                    _pfdcLightMaintenanceCountdown_BottomNIR.ResetDate = DateTime.Now;
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_BottomNIR);
                    break;

                default:
                    break;
            }
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return;
            //for now countdown only handle hours in init - here see in the future how to handle différent kind of unit(on custommer request)
            switch (enumFdc)
            {
                case ProviderFDCs.LightMaintenanceCountdown_White:
                    _pfdcLightMaintenanceCountdown_White.InitialCountTime = new TimeSpan((int)initvalue, 0, 0);
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_White);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_Red:
                    _pfdcLightMaintenanceCountdown_Red.InitialCountTime = new TimeSpan((int)initvalue, 0, 0);
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_Red);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_NIR:
                    _pfdcLightMaintenanceCountdown_NIR.InitialCountTime = new TimeSpan((int)initvalue, 0, 0);
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_NIR);
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_BottomNIR:
                    _pfdcLightMaintenanceCountdown_BottomNIR.InitialCountTime = new TimeSpan((int)initvalue, 0, 0);
                    _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_BottomNIR);
                    break;

                default:
                    break;
            }
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return;

            switch (enumFdc)
            {
                case ProviderFDCs.ServerStartCounter:
                    if (persistentFDCData is PersistentFDCCounter<UInt64> pfdcStartCounter)
                        _pfdc_StartCounter = pfdcStartCounter;
                    break;

                case ProviderFDCs.ServerFatalErrorCounter:
                    if (persistentFDCData is PersistentFDCCounter<UInt64> pfdcFatalErrorCounter)
                        _pfdc_FatalErrorCounter = pfdcFatalErrorCounter;
                    break;

                case ProviderFDCs.CPU_UsageOnTimePeriod48h:
                    if (persistentFDCData is PersistentWindowTimeData<byte> pfdcWindow)
                        _pfdc_windowTimeCPU_48h = pfdcWindow;
                    break;

                case ProviderFDCs.MemUsageOnTimePeriod48h:
                    if (persistentFDCData is PersistentWindowTimeData<byte> pfdc)
                        _pfdc_windowMemory_48h = pfdc;
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_White:
                    if (persistentFDCData is PersitentFDCCountdown pfdcLightMaintenanceCountdown_White)
                        _pfdcLightMaintenanceCountdown_White = pfdcLightMaintenanceCountdown_White;
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_Red:
                    if (persistentFDCData is PersitentFDCCountdown pfdcLightMaintenanceCountdown_Red)
                        _pfdcLightMaintenanceCountdown_Red = pfdcLightMaintenanceCountdown_Red;
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_NIR:
                    if (persistentFDCData is PersitentFDCCountdown pfdcLightMaintenanceCountdown_NIR)
                        _pfdcLightMaintenanceCountdown_NIR = pfdcLightMaintenanceCountdown_NIR;
                    break;

                case ProviderFDCs.LightMaintenanceCountdown_BottomNIR:
                    if (persistentFDCData is PersitentFDCCountdown pfdcLightMaintenanceCountdown_BottomNIR)
                        _pfdcLightMaintenanceCountdown_BottomNIR = pfdcLightMaintenanceCountdown_BottomNIR;
                    break;

                default:
                    break;
            }
        }

        public void StartFDCMonitor()
        {
            _cancellationToken = _cancellationTokenSource.Token;

            // TODO SENT ONCE COUNTER

            if (_pfdc_StartCounter == null)
            {
                _pfdc_StartCounter = new PersistentFDCCounter<UInt64>(ToFDCName(ProviderFDCs.ServerStartCounter));
            }
            _pfdc_StartCounter.Counter++;
            _fdcManager.SetPersistentFDCData(_pfdc_StartCounter);

            if (_pfdc_FatalErrorCounter == null)
            {
                _pfdc_FatalErrorCounter = new PersistentFDCCounter<UInt64>(ToFDCName(ProviderFDCs.ServerFatalErrorCounter));
            }
            CreateFatalErrorCounter();
            _fdcManager.SetPersistentFDCData(_pfdc_FatalErrorCounter);

            if (_pfdcLightMaintenanceCountdown_White == null)
            {
                _pfdcLightMaintenanceCountdown_White = CreateFDCCountdown(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_White));
            }
            _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_White);

            if (_pfdcLightMaintenanceCountdown_Red == null)
            {
                _pfdcLightMaintenanceCountdown_Red = CreateFDCCountdown(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_Red));
            }
            _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_Red);

            if (_pfdcLightMaintenanceCountdown_NIR == null)
            {
                _pfdcLightMaintenanceCountdown_NIR = CreateFDCCountdown(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_NIR));
            }
            _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_NIR);

            if (_pfdcLightMaintenanceCountdown_BottomNIR == null)
            {
                _pfdcLightMaintenanceCountdown_BottomNIR = CreateFDCCountdown(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_BottomNIR));
            }
            _fdcManager.SetPersistentFDCData(_pfdcLightMaintenanceCountdown_BottomNIR);

            var fdcs = new List<FDCData>()
            {
                FDCData.MakeNew(ToFDCName(ProviderFDCs.ServerStartCounter), _pfdc_StartCounter.Counter),
                FDCData.MakeNew(ToFDCName(ProviderFDCs.ServerFatalErrorCounter), _pfdc_FatalErrorCounter.Counter),
                FDCData.MakeNew(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_White), _pfdcLightMaintenanceCountdown_White.CountdownHours, "h"),
                FDCData.MakeNew(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_Red), _pfdcLightMaintenanceCountdown_Red.CountdownHours, "h"),
                FDCData.MakeNew(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_NIR), _pfdcLightMaintenanceCountdown_NIR.CountdownHours, "h"),
                FDCData.MakeNew(ToFDCName(ProviderFDCs.LightMaintenanceCountdown_BottomNIR), _pfdcLightMaintenanceCountdown_BottomNIR.CountdownHours, "h")
            };

            _fdcManager.SendFDCs(fdcs);

            // Start CPU FDC Monitoring
            StartMonitoringFDCs_CPU();

            StartMonitoringFDCs_Memory();

            //// Start EXAMPLE FDC Monitoring
            //_monitorThreads.Add(new Thread(new ThreadStart(MonitoringFDCs_EXAMPLE)));
            //_monitorThreads.Last().Start();
        }

        private PersitentFDCCountdown CreateFDCCountdown(string fdcname)
        {
            var fdcItemConfig = _fdcManager.GetFDCsConfig();
            var countdownconfig = fdcItemConfig?.Find(x => x.Name == fdcname) ?? new FDCItemConfig() { InitValue = 50000.0 };
            // if FDC is not well define create fdc with some defaulft value
            double initialCountTimeHours = countdownconfig.InitValue;
            TimeSpan initialCountTime = new TimeSpan((int)initialCountTimeHours, 0, 0);
            return new PersitentFDCCountdown(fdcname, DateTime.Now, initialCountTime);
        }

        public void Register()
        {
            _cpuCounter = GetPerformanceCounterCPU();

            // Call Set SetPersistentData if it has already been saved and then start monitoring
            InitProviderFDCNames();
            _fdcManager.RegisterFDCProvider(this, GetProviderFDCNames());
        }

        /**
         * Try to create a PerfomanceCounter to calculate CPU usage
         * Check among 2 known categories and 2 known counters to find if one of them exists
         * If yes, returns the PerformanceCounter / Else return null
         * It also sets _performanceCounterCPUUsageCategory, _performanceCounterCPUusageLabel & _performanceCounterCPUUsageExists variables
         * Those variables are used to perform PerformanceCounterCategory.Exists() & PerformanceCounterCategory.CounterExists() only in the first call
         * Because PerformanceCounterCategory methods invoke can be quite long
         */
        private PerformanceCounter GetPerformanceCounterCPU()
        {
            const string CPUCounterCategoryProcessorUtility = "% Processor Utility";
            const string CPUCounterCategoryProcessorTime = "% Processor Time";
            const string CPUCounterLabelProcessorInfo = "Processor Information";
            const string CPUCounterLabelProcessor = "Processor";

            PerformanceCounter cpuCounter = null;

            try
            {
                // Important :  PerformanceCounterCategory.CounterExists(counter, category) raises an Exception if category doesn't exist
                //              That's why we need to first check if category exists by using PerformanceCounterCategory.Exists

                // Check if PerformanceCounter category and counter names for CPU usage have already been initialized
                if (string.IsNullOrEmpty(_performanceCounterCPUUsageCategory) && string.IsNullOrEmpty(_performanceCounterCPUusageLabel))
                {
                    // Initilization to default performance counter
                    _performanceCounterCPUUsageCategory = CPUCounterLabelProcessorInfo;
                    _performanceCounterCPUusageLabel = CPUCounterCategoryProcessorUtility;
                    _performanceCounterCPUUsageExists = true;

                    // Check if default category or the second one exists in PerformanceCounterCategory
                    bool performanceCounterCategoryExists = PerformanceCounterCategory.Exists(_performanceCounterCPUUsageCategory);
                    if (!performanceCounterCategoryExists)
                    {
                        _performanceCounterCPUUsageCategory = CPUCounterLabelProcessor;
                        performanceCounterCategoryExists = PerformanceCounterCategory.Exists(_performanceCounterCPUUsageCategory);
                        _performanceCounterCPUUsageExists = false;
                    }

                    // If one category from preset ones exists in PerformanceCounterCategory, we check if one target counter exists in this category
                    if (performanceCounterCategoryExists)
                    {
                        _performanceCounterCPUUsageExists = PerformanceCounterCategory.CounterExists(_performanceCounterCPUusageLabel, _performanceCounterCPUUsageCategory);
                        if (!_performanceCounterCPUUsageExists)
                        {
                            _performanceCounterCPUusageLabel = CPUCounterCategoryProcessorTime;
                            _performanceCounterCPUUsageExists = PerformanceCounterCategory.CounterExists(_performanceCounterCPUusageLabel, _performanceCounterCPUUsageCategory);
                        }
                    }
                }

                if (_performanceCounterCPUUsageExists)
                {
                    cpuCounter = new PerformanceCounter(_performanceCounterCPUUsageCategory, _performanceCounterCPUusageLabel, "_Total");
                    // First value is not always valid, we retrieve it and don't use it
                    cpuCounter.NextValue();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to create the CPU% PerformanceCounter");
            }

            return cpuCounter;
        }

        /**
         * Returns CPU usage calculated with given PerformanceCounter if it exists, else returns WMI calculate value
         */
        private double GetCPUUsage(PerformanceCounter cpuCounter)
        {
            double cpuUsage = 0d;

            if (cpuCounter != null)
            {
                cpuUsage = cpuCounter.NextValue();
            }
            else
            {
                cpuUsage = HelperFDCSystemInfo.GetCPUUsagePercentageVMI();
            }

            return cpuUsage;
        }

        private void CreateFatalErrorCounter()
        {
            var nbLines = 0UL;
            if (File.Exists(ErrorFilePath))
            {
                nbLines = (ulong)File.ReadAllLines(ErrorFilePath).Length;
                File.Delete(ErrorFilePath);
            }
            _pfdc_FatalErrorCounter.Counter += nbLines;
        }

        private void StartMonitoringFDCs_CPU()
        {
            // Initialise Missing Persistent Data
            // comment savoir si en config on doit monitor ce fdc --- TODO
            if (_pfdc_windowTimeCPU_48h == null)
            {
                _pfdc_windowTimeCPU_48h = new PersistentWindowTimeData<byte>(ToFDCName(ProviderFDCs.CPU_UsageOnTimePeriod48h), new TimeSpan(0, 48, 0, 0));
            }

            _monitorThreads.Add(new Thread(new ThreadStart(MonitoringFDCs_CPU)));
            _monitorThreads.Last().Start();
        }

        private void StartMonitoringFDCs_Memory()
        {
            if (_pfdc_windowMemory_48h == null)
            {
                _pfdc_windowMemory_48h = new PersistentWindowTimeData<byte>(ToFDCName(ProviderFDCs.MemUsageOnTimePeriod48h), new TimeSpan(0, 48, 0, 0));
            }

            _monitorThreads.Add(new Thread(new ThreadStart(MonitoringFDCs_MemoryUsage)));
            _monitorThreads.Last().Start();
        }

        private async void MonitoringFDCs_CPU()
        {
            PerformanceCounter cpuCounter = GetPerformanceCounterCPU();

            // sampling rate in Config ? TODO
            int samplingTime_ms = 10000;
            while (!_cancellationToken.IsCancellationRequested)
            {
                _pfdc_windowTimeCPU_48h.AddData((byte)Math.Round(GetCPUUsage(cpuCounter)));
                _fdcManager.SetPersistentFDCData(_pfdc_windowTimeCPU_48h);

                Task task = Task.Delay(samplingTime_ms, _cancellationToken);
                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            // Monitor cleaning if needed
            cpuCounter?.Close();
        }

        private async void MonitoringFDCs_MemoryUsage()
        {
            int samplingTime_ms = 10000;
            while (!_cancellationToken.IsCancellationRequested)
            {
                _pfdc_windowMemory_48h.AddData((byte)Math.Round(HelperFDCSystemInfo.GetMemoryUsagePercentage()));
                _fdcManager.SetPersistentFDCData(_pfdc_windowMemory_48h);

                try
                {
                    await Task.Delay(samplingTime_ms, _cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        /*
        private async void MonitoringFDCs_EXAMPLE()
        {
            // call SetPersistentData if need

            // sampling rate in Config ? TODO
            int samplingTime_ms = 1000;
            while (!_cancellationToken.IsCancellationRequested)
            {
                if (_cancellationToken.IsCancellationRequested)
                    break;

                // todo

                Task task = Task.Delay(samplingTime_ms, _cancellationToken);
                try
                {
                    await task;
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }

            // Monitor cleaning if needed
        }
        */
    }
}
