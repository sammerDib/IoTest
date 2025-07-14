using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.FDC
{
    public class FDCManager
    {
        private FDCsConfiguration _currentFDCConfiguration;

        private Dictionary<IFDCProvider, List<string>> _fdcsByProviders;

        protected static IMessenger Messenger => ClassLocator.Default.GetInstance<IMessenger>();
        private const int TimeBetweenTwoPersistantDataSaves = 10000; //ms

        private bool _updateAllFDCsRequested = false;
        private object _synchroUpdateAllFDCsRequested = new object();

        private PersistentFDCsData _persistentFDCsData;

        private string _fdcPersistentFilePath;
        private string _fdcConfigFilePath;

        private Thread _threadMonitoring;
        private Thread _threadSavePersistantData;

        private ILogger _logger;
        private bool _arePersitantDataModified = false;

        public FDCManager(string fdcConfigFilePath, string fdcPersistentFilePath)
        {
            // Just to generate an example file
            //var fdcsConfiguration= new FDCsConfiguration();
            //fdcsConfiguration.Save(fdcConfigFilePath);
            _fdcConfigFilePath = fdcConfigFilePath;
            _fdcPersistentFilePath = fdcPersistentFilePath;
            _persistentFDCsData = PersistentFDCsData.Load(fdcPersistentFilePath);

            if (_persistentFDCsData is null)
            {
                _persistentFDCsData = new PersistentFDCsData();
            }

            _currentFDCConfiguration = FDCsConfiguration.Load(fdcConfigFilePath);

            if (_currentFDCConfiguration is null)
            {
                _currentFDCConfiguration = new FDCsConfiguration();
                _currentFDCConfiguration.FDCItemsConfig = new List<FDCItemConfig>();
            }

            _fdcsByProviders = new Dictionary<IFDCProvider, List<string>>();

            _logger = ClassLocator.Default.GetInstance<ILogger>();
        }

        public void StartMonitoringFDC()
        {
            if (_threadMonitoring != null)
                return;

            _threadMonitoring = new Thread(() => FDCMonitoring());
            _threadMonitoring.Start();

            _threadSavePersistantData = new Thread(() => SavingPersistantData());
            _threadSavePersistantData.Start();
        }

        private void SavingPersistantData()
        {
            while (true)
            {
                Thread.Sleep(TimeBetweenTwoPersistantDataSaves);
                if (_arePersitantDataModified)
                {
                    _arePersitantDataModified = false;
                    _persistentFDCsData.Save(_fdcPersistentFilePath);
                }
            }
        }

        public void ForceSavingPersistantData()
        {
            if (_arePersitantDataModified)
            {
                
                _arePersitantDataModified = false;
                _persistentFDCsData.Save(_fdcPersistentFilePath);
            }
        }

        public void RegisterFDCProvider(IFDCProvider fdcProvider, List<string> fdcNames)
        {
            // Check that each fdcName is in the fdcConfigurations
            foreach (string fdcName in fdcNames)
            {
#if DEBUG
                if (!_currentFDCConfiguration.FDCItemsConfig.Exists(fdci => fdci.Name == fdcName))
                {
                    _logger.Warning($"The FDC {fdcName} is not available in the FDCs configuration file");
                }
#endif

                // Check that the FDC has not already been registered by another provider
                var existingProvider = _fdcsByProviders.FirstOrDefault(fp => fp.Value.Contains(fdcName)).Key;
                if (existingProvider != null)
                    _logger.Error($"The FDC {fdcName} has already been registered by another provider : {existingProvider} ");
            }

            // Send the persited data to the provider
            foreach (string fdcName in fdcNames)
            {
                var persistentData = _persistentFDCsData.Data.FirstOrDefault(x => x.FDCName == fdcName);
                if (persistentData != null)
                {
                    fdcProvider.SetPersistentData(fdcName, persistentData);
                }
            }

            fdcProvider.StartFDCMonitor();

            if (_fdcsByProviders.ContainsKey(fdcProvider))
            {
                _fdcsByProviders[fdcProvider] = fdcNames;
            }
            else
            {
                _fdcsByProviders.Add(fdcProvider, fdcNames);
            }
        }

        public void SendFDC(FDCData fdcToSend)
        {
            SendFDCs(new List<FDCData> { fdcToSend });
        }

        public void SendFDCs(List<FDCData> fdcsToSend)
        {
            foreach (var fdc in fdcsToSend)
            {
                UpdateFDCStatus(fdc.Name, new FDCStatus() { LastSentDate = DateTime.UtcNow });
                Messenger.Send(new SendFDCMessage() { Data = fdc });
            }

            if (fdcsToSend.Count > 0)
                Messenger.Send(new SendFDCListMessage() { FDCsData = fdcsToSend });
        }

        private void UpdateFDCStatus(string fdcName, FDCStatus newStatus)
        {
            if (!_persistentFDCsData.FdcsStatus.ContainsKey(fdcName))
                _persistentFDCsData.FdcsStatus.Add(fdcName, newStatus);
            else
                _persistentFDCsData.FdcsStatus[fdcName] = newStatus;
        }

        public void SetPersistentFDCData(IPersistentFDCData persistentFDCData)
        {
            int fdcDataIndex = _persistentFDCsData.Data.FindIndex(s => s.FDCName == persistentFDCData.FDCName);

            if (fdcDataIndex != -1)
                _persistentFDCsData.Data[fdcDataIndex] = persistentFDCData;
            else
                _persistentFDCsData.Data.Add(persistentFDCData);

            _arePersitantDataModified = true;
        }

        private void FDCMonitoring()
        {
            List<FDCData> fdcsDataToSend = new List<FDCData>();
            while (true)
            {
                fdcsDataToSend.Clear();
                var forceAllFDCsUpdate = GetUpdateAllFDCsRequested();

                foreach (var fdcItem in _currentFDCConfiguration.FDCItemsConfig)
                {
                    var fdcProvider = GetFDCProvider(fdcItem.Name);

                    if (fdcProvider != null)
                    {
                        var fdcStatus = _persistentFDCsData.FdcsStatus.ContainsKey(fdcItem.Name) ? _persistentFDCsData.FdcsStatus[fdcItem.Name] : null;

                        if (forceAllFDCsUpdate || IsFDCSendNeeded(fdcItem, fdcStatus))
                        {
                            var fdcToSend = fdcProvider.GetFDC(fdcItem.Name);
                            if (fdcToSend != null)
                                fdcsDataToSend.Add(fdcToSend);
                        }
                    }
                }

                if (fdcsDataToSend.Count > 0)
                    SendFDCs(fdcsDataToSend);

                Thread.Sleep(1000);
                GenerateFDCDefinitionFile();
            }
        }

        public void GenerateFDCDefinitionFile()
        {
            string FDCsDefinitionFilePath = $"FDCsDefinition_{Assembly.GetEntryAssembly().GetName().Name.Replace("UnitySC.","").Replace(".Service.Host","")}_GeneratedOnStarting.xml";
            if (!File.Exists(FDCsDefinitionFilePath))
            {
                List<FDCData> fdcsDataToWrite = new List<FDCData>();
                foreach (var fdcItem in _currentFDCConfiguration.FDCItemsConfig)
                {
                    var fdcProvider = GetFDCProvider(fdcItem.Name);

                    if (fdcProvider != null)
                    {
                        var fdcStatus = _persistentFDCsData.FdcsStatus.ContainsKey(fdcItem.Name) ? _persistentFDCsData.FdcsStatus[fdcItem.Name] : null;
                        var fdcToWrite = fdcProvider.GetFDC(fdcItem.Name);
                        if (fdcToWrite != null)
                            fdcsDataToWrite.Add(fdcToWrite);
                    }
                }
                XML.Serialize(fdcsDataToWrite, FDCsDefinitionFilePath);
            }
        }

        private bool IsFDCSendNeeded(FDCItemConfig fdcItem, FDCStatus fDCStatus)
        {
            var dateNow = DateTime.UtcNow;

            if (fDCStatus == null)
                fDCStatus = new FDCStatus() { LastSentDate = DateTime.MinValue };

            switch (fdcItem.SendFrequency)
            {
                case FDCSendFrequency.Year:
                    if (dateNow.Year != fDCStatus.LastSentDate.Year)
                        return true;
                    break;

                case FDCSendFrequency.Month:
                    if (dateNow.Month != fDCStatus.LastSentDate.Month)
                        return true;
                    break;

                case FDCSendFrequency.Week:
                    if (GetWeekOfYear(dateNow) != GetWeekOfYear(fDCStatus.LastSentDate))
                        return true;
                    break;

                case FDCSendFrequency.Day:
                    if (dateNow.Day != fDCStatus.LastSentDate.Day)
                        return true;
                    break;

                case FDCSendFrequency.Hour:
                    if (dateNow.Hour != fDCStatus.LastSentDate.Hour)
                        return true;
                    break;

                case FDCSendFrequency.Hours12:
                    if ((dateNow.Hour % 12 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(11, 0, 0)))
                        return true;
                    break;

                case FDCSendFrequency.Minutes30:
                    if ((dateNow.Minute % 30 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 29, 0)))
                        return true;
                    break;

                case FDCSendFrequency.Minutes20:
                    if ((dateNow.Minute % 20 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 19, 0)))
                        return true;
                    break;

                case FDCSendFrequency.Minutes15:
                    if ((dateNow.Minute % 15 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 14, 0)))
                        return true;
                    break;

                case FDCSendFrequency.Minutes10:
                    if ((dateNow.Minute % 10 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 9, 0)))
                        return true;
                    break;

                case FDCSendFrequency.Minutes5:
                    if ((dateNow.Minute % 5 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 4, 0)))
                        return true;
                    break;

                case FDCSendFrequency.Minute:
                    if (dateNow.Minute != fDCStatus.LastSentDate.Minute)
                        return true;
                    break;

                case FDCSendFrequency.Seconds30:
                    if ((dateNow.Second % 30 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 0, 25)))
                        return true;
                    break;

                case FDCSendFrequency.Seconds10:
                    if ((dateNow.Second % 10 == 0) && ((dateNow - fDCStatus.LastSentDate) > new TimeSpan(0, 0, 5)))
                        return true;
                    break;

                case FDCSendFrequency.CustomDelay:
                    if ((dateNow - fDCStatus.LastSentDate) > fdcItem.CustomDelay)
                        return true;
                    break;

                case FDCSendFrequency.Once:
                    break;

                case FDCSendFrequency.Never:
                    break;

                default:
                    break;
            }
            return false;
        }

        // Function to get the ISO 8601 week number
        private int GetWeekOfYear(DateTime date)
        {
            System.Globalization.CultureInfo ci = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.Calendar cal = ci.Calendar;

            return cal.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        private IFDCProvider GetFDCProvider(string fdcName)
        {
            foreach (var fdcs in _fdcsByProviders)
            {
                if (fdcs.Value.Exists(name => name == fdcName))
                {
                    return fdcs.Key;
                }
            }
            return null;
        }

        public List<FDCItemConfig> GetFDCsConfig()
        {
            return _currentFDCConfiguration.FDCItemsConfig;
        }

        public FDCSendFrequency? GetFDCSendFrequency(string fdcName)
        {
            var fdcConfig = _currentFDCConfiguration.FDCItemsConfig.Find(fdc => fdc.Name == fdcName);

            return fdcConfig?.SendFrequency;
        }

        public void SetFDCsConfig(List<FDCItemConfig> fdcItemsConfig)
        {
            _currentFDCConfiguration.FDCItemsConfig = fdcItemsConfig;

            _currentFDCConfiguration.Save(_fdcConfigFilePath);
        }

        public void ResetFDC(string fdcName)
        {
            var fdcProvider = GetFDCProvider(fdcName);

            if (fdcProvider != null)
            {
                fdcProvider.ResetFDC(fdcName);
            }
        }

        public void SetInitialCountdownValue(string fdcName, double initvalue)
        {
            var fdcProvider = GetFDCProvider(fdcName);

            if (fdcProvider != null)
            {
                fdcProvider.SetInitialCountdownValue(fdcName, initvalue);
            }
        }

        public FDCData GetFDC(string fdcName)
        {
            var fdcProvider = GetFDCProvider(fdcName);

            if (fdcProvider != null)
            {
                return fdcProvider.GetFDC(fdcName);
            }
            return null;
        }

        public void RequestAllFDCsUpdate()
        {
            lock (_synchroUpdateAllFDCsRequested)
                _updateAllFDCsRequested = true;
        }

        private bool GetUpdateAllFDCsRequested()
        {
            bool IsRequested = false;
            lock (_synchroUpdateAllFDCsRequested)
            {
                IsRequested = _updateAllFDCsRequested;
                _updateAllFDCsRequested = false; // In case negative values
            }
            return IsRequested;
        }
    }
}
