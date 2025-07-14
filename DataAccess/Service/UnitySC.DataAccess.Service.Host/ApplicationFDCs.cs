using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.FDC;
using UnitySC.Shared.FDC.PersistentData;
using UnitySC.Shared.Tools;

namespace UnitySC.DataAccess.Service.Host
{
    public class ApplicationFDCs : IFDCProvider
    {
        private readonly FDCManager _fdcManager;

        private readonly Dictionary<string, ProviderFDCs> _providerFDCNames = new Dictionary<string, ProviderFDCs>();
        private readonly DateTime _upTimeSinceStart_dt = DateTime.UtcNow;
        private PersistentFDCCounter<ulong> _pfdc_StartCounter;
        private PersistentFDCCounter<int> _pfdc_RecipesCounter;
        private PersistentFDCCounter<int> _pfdc_ResultsCounter;
        private PersistentFDCCounter<int> _pfdc_AcquisitionsCounter;
        private PersistentFDCCounter<int> _pfdc_ProductsCounter;

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
            ServerUptime,               //DataAccess_Server_Uptime,
            ServerStartCounter,         //DataAccess_Server_StartCounter,

            DatabaseSize,               //DataAccess_DataBase_Size
            DatabasePercentFree,       //DataAccess_Database_Percent_Free
            DatabaseRecipesCounter,      //DataAccess_Recipes_Counter

            DatabaseResultsCounter,   //DataAccess_Results_Counter
            DatabaseAcquisitionsCounter, //DataAccess_Acquisitions_Counter
            DatabaseProductsCounter,  //DataAccess_Products_Counter
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

                case ProviderFDCs.DatabaseSize:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFdcDb.GetDatabaseSize(), "Mo");
                    break;

                case ProviderFDCs.DatabasePercentFree:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFdcDb.GetDatabaseFreeSpacePercentage(), "%");
                    break;

                case ProviderFDCs.DatabaseRecipesCounter:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFdcDb.GetRecipesCount());
                    break;

                case ProviderFDCs.DatabaseResultsCounter:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFdcDb.GetResultsCount());
                    break;

                case ProviderFDCs.DatabaseAcquisitionsCounter:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFdcDb.GetAcquisitionsCount());
                    break;

                case ProviderFDCs.DatabaseProductsCounter:
                    fdcdata = FDCData.MakeNew(fdcName, HelperFdcDb.GetProductsCount());
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
                    fdcName = "DataAccess_Server_Uptime";
                    break;

                case ProviderFDCs.ServerStartCounter:
                    fdcName = "DataAccess_Server_StartCounter";
                    break;

                case ProviderFDCs.DatabaseSize:
                    fdcName = "DataAccess_DataBase_Size";
                    break;

                case ProviderFDCs.DatabasePercentFree:
                    fdcName = "DataAccess_Database_Percent_Free";
                    break;

                case ProviderFDCs.DatabaseRecipesCounter:
                    fdcName = "DataAccess_Recipes_Counter";
                    break;

                case ProviderFDCs.DatabaseResultsCounter:
                    fdcName = "DataAccess_Results_Counter";
                    break;

                case ProviderFDCs.DatabaseAcquisitionsCounter:
                    fdcName = "Database_Acquisitions_Counter";
                    break;

                case ProviderFDCs.DatabaseProductsCounter:
                    fdcName = "DataAccess_Products_Counter";
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

                case ProviderFDCs.DatabaseRecipesCounter:
                    if (persistentFDCData is PersistentFDCCounter<int> pfdcRecipeCounter)
                        _pfdc_RecipesCounter = pfdcRecipeCounter;
                    break;

                case ProviderFDCs.DatabaseResultsCounter:
                    if (persistentFDCData is PersistentFDCCounter<int> pfdcResultsCounter)
                        _pfdc_ResultsCounter = pfdcResultsCounter;
                    break;

                case ProviderFDCs.DatabaseAcquisitionsCounter:
                    if (persistentFDCData is PersistentFDCCounter<int> pfdcAcqCounter)
                        _pfdc_AcquisitionsCounter = pfdcAcqCounter;
                    break;

                case ProviderFDCs.DatabaseProductsCounter:
                    if (persistentFDCData is PersistentFDCCounter<int> pfdcProductsCounter)
                        _pfdc_ProductsCounter = pfdcProductsCounter;
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
           // nothing to do here - no reinit fdc 
        }
    }
}
