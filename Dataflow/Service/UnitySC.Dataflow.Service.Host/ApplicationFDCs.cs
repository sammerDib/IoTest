using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Tools;

namespace UnitySC.Dataflow.Service.Host
{
    public class ApplicationFDCs : IFDCProvider
    {
        private readonly FDCManager _fdcManager;

        private readonly Dictionary<string, ProviderFDCs> _providerFDCNames = new Dictionary<string, ProviderFDCs>();
        private readonly DateTime _upTimeSinceStart_dt = DateTime.UtcNow;
        private PersistentFDCCounter<ulong> _pfdc_StartCounter;
        private PersistentFDCCounter<int> _pfdc_RecipeCounter;

        public ApplicationFDCs()
        {
            _fdcManager = ClassLocator.Default.GetInstance<FDCManager>();
        }

        private List<string> GetProviderFDCNames()
        {
            return _providerFDCNames.Keys.ToList();
        }

        private enum ProviderFDCs
        {
            ServerUptime,               //Dataflow_Server_Uptime,
            ServerStartCounter,         //Dataflow_Server_StartCounter,
        }

        public FDCData GetFDC(string fdcName)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out ProviderFDCs enumFdc))
                return null;

            FDCData fdcdata = null;
            switch (enumFdc)
            {
                case ProviderFDCs.ServerUptime:
                    var ts = DateTime.UtcNow - _upTimeSinceStart_dt;
                    fdcdata = FDCData.MakeNew(fdcName, ts);
                    break;

                case ProviderFDCs.ServerStartCounter:
                    fdcdata = FDCData.MakeNew(fdcName, _pfdc_StartCounter.Counter);
                    break;
            }
            return fdcdata;
        }

        private void InitProviderFDCNames()
        {
            var enumlist = Enum.GetValues(typeof(ProviderFDCs)).Cast<ProviderFDCs>().ToList();
            foreach (var fdc in enumlist)
            {
                _providerFDCNames.Add(ToFDCName(fdc), fdc);
            }
        }

        private static string ToFDCName(ProviderFDCs fdc)
        {
            string fdcName = null;
            switch (fdc)
            {
                case ProviderFDCs.ServerUptime:
                    fdcName = "Dataflow_Server_Uptime";
                    break;

                case ProviderFDCs.ServerStartCounter:
                    fdcName = "Dataflow_Server_StartCounter";
                    break;
            }
            return fdcName;
        }

        public void Register()
        {
            InitProviderFDCNames();
            _fdcManager.RegisterFDCProvider(this, GetProviderFDCNames());
        }

        public void ResetFDC(string fdcName)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out var enumFdc))
                return;

            switch (enumFdc)
            {
                case ProviderFDCs.ServerStartCounter:
                    _pfdc_StartCounter.Counter = 0uL;
                    _fdcManager.SetPersistentFDCData(_pfdc_StartCounter);
                    break;
            }
        }

        public void SetPersistentData(string fdcName, IPersistentFDCData persistentFDCData)
        {
            if (!_providerFDCNames.TryGetValue(fdcName, out var enumFdc))
                return;

            switch (enumFdc)
            {
                case ProviderFDCs.ServerStartCounter:
                    if (persistentFDCData is PersistentFDCCounter<ulong> pfdcStartCounter)
                        _pfdc_StartCounter = pfdcStartCounter;
                    break;

            }
        }

        public void StartFDCMonitor()
        {
            if (_pfdc_StartCounter == null)
            {
                _pfdc_StartCounter = new PersistentFDCCounter<ulong>(ToFDCName(ProviderFDCs.ServerStartCounter));
            }
            _pfdc_StartCounter.Counter++;
            _fdcManager.SetPersistentFDCData(_pfdc_StartCounter);

            var fdcs = new List<FDCData>()
            {
                FDCData.MakeNew(ToFDCName(ProviderFDCs.ServerStartCounter), _pfdc_StartCounter.Counter),
            };

            _fdcManager.SendFDCs(fdcs);
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            // nothing to do - hre - no fdc to reinit
        }
    }
}
