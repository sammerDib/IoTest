using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.Interface;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Implementation
{
    public class ClientFDCs : IDisposable, IFDCProvider
    {
        private FDCManager _fdcManager;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private CancellationToken _cancellationToken;

        private ILogger _logger;

        #region FDC Name Enum Association

        private enum ProviderFDCs
        {
            ClientUpTime,               //ANA_Client_Uptime,
            ClientStartCounter,         //ANA_Client_StartCounter,
            ClientUpTimeLocal,          //ANA_Client_UptimeLocal,
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
                case ProviderFDCs.ClientUpTime:
                    fdcName = "ANA_Client_Uptime";
                    break;

                case ProviderFDCs.ClientStartCounter:
                    fdcName = "ANA_Client_StartCounter";
                    break;

                case ProviderFDCs.ClientUpTimeLocal:
                    fdcName = "ANA_Client_UptimeLocal";
                    break;
            }
            return fdcName;
        }

        private void InitProviderFDCNames()
        {
            var enumlist = Enum.GetValues(typeof(ProviderFDCs)).Cast<ProviderFDCs>().ToList();

            foreach (var fdc in enumlist)
            {
                _providerFDCNames.Add(ToFDCName(fdc), fdc);
            }
        }

        #endregion FDC Name Enum Association

        #region NonPersitentData

        private DateTime? _upTimeSinceStart_dt = null;

        private DateTime? _upTimeLocalSinceStart_dt = null;

        private DateTime? _lastHeartBeat_dt = null;

        private bool _isCheckClientIsAliveRunning = false;

        #endregion NonPersitentData

        #region PersitentData

        private PersistentFDCCounter<UInt64> _pfdc_StartCounter;

        #endregion PersitentData

        public ClientFDCs()
        {
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
            _logger = ClassLocator.Default.GetInstance<ILogger<object>>();
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
        }

        public FDCData GetFDC(string fdcName)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return null;

            FDCData fdcdata = null;
            switch (enumFdc)
            {
                case ProviderFDCs.ClientUpTime:
                    if (_upTimeSinceStart_dt == null)
                    {
                        fdcdata = FDCData.MakeNew(fdcName, new TimeSpan());
                    }
                    else
                    {
                        var ts = DateTime.Now - _upTimeSinceStart_dt;
                        fdcdata = FDCData.MakeNew(fdcName, ts);
                    }
                    break;

                case ProviderFDCs.ClientStartCounter:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdc_StartCounter.Counter);
                    break;

                case ProviderFDCs.ClientUpTimeLocal:
                    if (_upTimeLocalSinceStart_dt == null)
                    {
                        fdcdata = FDCData.MakeNew(fdcName, new TimeSpan());
                    }
                    else
                    {
                        var ts = DateTime.Now - _upTimeLocalSinceStart_dt;
                        fdcdata = FDCData.MakeNew(fdcName, ts);
                    }
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
                case ProviderFDCs.ClientStartCounter:
                    _pfdc_StartCounter.Counter = 0uL;
                    _fdcManager.SetPersistentFDCData(_pfdc_StartCounter);
                    break;

                default:
                    break;
            }
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return;
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return;

            switch (enumFdc)
            {
                case ProviderFDCs.ClientStartCounter:
                    if (persistentFDCData is PersistentFDCCounter<UInt64> pfdcStartCounter)
                        _pfdc_StartCounter = pfdcStartCounter;
                    break;

                default:
                    break;
            }
        }

        private bool _isFDCMonitorStarted = false;

        public void StartFDCMonitor()
        {
            _cancellationToken = _cancellationTokenSource.Token;

            if (_pfdc_StartCounter == null)
            {
                _pfdc_StartCounter = new PersistentFDCCounter<UInt64>(ToFDCName(ProviderFDCs.ClientStartCounter));
            }

            var fdcs = new List<FDCData>()
            {
                FDCData.MakeNew(ToFDCName(ProviderFDCs.ClientStartCounter), _pfdc_StartCounter.Counter),
            };

            _fdcManager.SendFDCs(fdcs);

            _isFDCMonitorStarted = true;
        }

        public void Register()
        {
            // Call Set SetPersistentData if it has already been saved and then start monitoring
            InitProviderFDCNames();
            _fdcManager.RegisterFDCProvider(this, GetProviderFDCNames());
        }

        internal void ClientStarted(string name)
        {
            _logger.Information($"[ClientFDC] Client started {name}");
            Task.Run(() =>
                {
                    // Wait FDC monitor is started
                    while (!_isFDCMonitorStarted)
                    {
                        Task.Delay(1000).Wait();
                    }
                    _upTimeSinceStart_dt = DateTime.Now;
                    _lastHeartBeat_dt = DateTime.Now;
                    _upTimeLocalSinceStart_dt = null;
                    _pfdc_StartCounter.Counter++;
                    _fdcManager.SetPersistentFDCData(_pfdc_StartCounter);
                    StartCheckClientIsAlive(name);
                }
             );
        }

        private void StartCheckClientIsAlive(string name)
        {
            if (_isCheckClientIsAliveRunning)
                return;
            _isCheckClientIsAliveRunning = true;
            Task.Run(() =>
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (_cancellationToken.IsCancellationRequested)
                        break;
                    if ((_lastHeartBeat_dt != null && (DateTime.Now - _lastHeartBeat_dt.Value).TotalMilliseconds > 2* FDCsConsts.HeartBeatDelay_ms) || (_upTimeSinceStart_dt == null))
                    {
                        _logger.Information($"[ClientFDC] Client is not alive {name}");
                        _upTimeSinceStart_dt = null;
                        _upTimeLocalSinceStart_dt = null;
                        _isCheckClientIsAliveRunning = false;
                        break;
                    }
                    Task.Delay(FDCsConsts.HeartBeatDelay_ms, _cancellationToken).Wait();
                }
            }
            );
        }

        internal void ClientIsRunning(string name)
        {
            // TODO: use name in the future
            _lastHeartBeat_dt = DateTime.Now;
        }

        internal void ClientStopped(string name)
        {
            _logger.Information($"[ClientFDC] Client is stopped {name}");
            _upTimeSinceStart_dt = null;
            _upTimeLocalSinceStart_dt = null;
        }

        internal void ApplicationModeLocalChanged(bool isInLocalMode)
        {
            if (isInLocalMode)
            {
                _upTimeLocalSinceStart_dt = DateTime.Now;
            }
            else
            {
                _upTimeLocalSinceStart_dt = null;
            }
        }

 
    }
}
