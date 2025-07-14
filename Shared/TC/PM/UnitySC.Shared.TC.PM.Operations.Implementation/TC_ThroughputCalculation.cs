using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.Shared.TC.PM.Operations.Implementation
{
    public delegate void EventHandlerStats(Dictionary<String, CStats> pParam1, bool bShow);

    public class CStats
    {
        public String StationID;
        public List<DateTime> CheckpointsWaferPlaced = new List<DateTime>();
        public List<DateTime> CheckpointsProcessStarted = new List<DateTime>();
        public List<DateTime> CheckpointsProcessFinished = new List<DateTime>();
        public List<DateTime> CheckpointsWaferRemoved = new List<DateTime>();

        public String CurrentWaferIDPlaced;
        public String InfoWafer;
        public String InfoPM;
        public String InfoThroughtput = "Throughtput calculated = Wafer count / (Time [placed from Arm to PMi] - Time [pick up from PMi to Arm])";

        private int _waferCounter = 0;

        public int WaferCounter
        {
            get { return _waferCounter; }
        }

        public void CalculateWaferCounter()
        {
            int cpt_Placed = CheckpointsWaferPlaced.Count;
            int cpt_Started = CheckpointsProcessStarted.Count;
            int cpt_Finished = CheckpointsProcessFinished.Count;
            int cpt_Removed = CheckpointsWaferRemoved.Count;

            _waferCounter = Convert.ToInt32(Math.Floor((double)(cpt_Placed + cpt_Started + cpt_Finished + cpt_Removed) / (double)4));
        }

        public String TimeInPM;
        public String ProcessTime;
        public String WaferCycleTime;
        public String ThroughtputMeasured;

        public CStats(String stationID)
        {
            StationID = stationID;
        }
    }

    public class CThroughtputStatistics
    {
        private bool _enable = false;
        private Dictionary<String, CStats> _stationStats = new Dictionary<String, CStats>();
        private double _throughputWafer;
        private EventHandlerStats _displayTrhoughtputResult;
        private Object _stopStatsSynchro = new object();
        private bool _throughputDataFileEnable = false;
        private String _throughputFullFilePathNameBase;
        private String _currentThroughputFullFilePathName;
        private DateTime _referenceTimeForFile;
        private int _cpt = 0;
        private ILogger _logger;
        private int _floatingWaferNumber;

        public bool Enable_TS { get => _enable; set => _enable = value; }

        public CThroughtputStatistics(EventHandlerStats displayTrhoughtputResult, int floatingWaferNumber, String filePathName)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<CThroughtputStatistics>>();
            _displayTrhoughtputResult = displayTrhoughtputResult;
            _floatingWaferNumber = floatingWaferNumber;
            _stationStats.Add("stidToolStation1", new CStats("stidToolStation1"));
            _stationStats.Add("stidToolStation2", new CStats("stidToolStation2"));
            _stationStats.Add("stidToolStation3", new CStats("stidToolStation3"));
            _stationStats.Add("stidToolStation4", new CStats("stidToolStation4"));

            _throughputFullFilePathNameBase = filePathName;
            CreateThroughputDataFile();
        }

        private void CreateThroughputDataFile()
        {
            StreamWriter throughputFile = null;
            try
            {
                // Check if using file for throughput is enabled
                _throughputDataFileEnable = !String.IsNullOrEmpty(Path.GetFileName(_throughputFullFilePathNameBase));
                if (!_throughputDataFileEnable) return;

                // Update file extension if needed

                string filename = Path.GetFileNameWithoutExtension(_throughputFullFilePathNameBase);
                _currentThroughputFullFilePathName = _throughputFullFilePathNameBase.Replace(filename, $"{filename}_{DateTime.Now.ToString("yyyyMMdd_HHmmss")}.txt");

                if (File.Exists(_currentThroughputFullFilePathName)) File.Delete(_currentThroughputFullFilePathName);
                throughputFile = new StreamWriter(_currentThroughputFullFilePathName);
                throughputFile.WriteLine("===================================================================================================================");
                throughputFile.WriteLine($"Statistics on jobs started at {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")} - Througtput calculation moving on last " + _floatingWaferNumber.ToString() + " Wafers");
                throughputFile.WriteLine("wafer#,checkpoint WaferPlaced,checkpoint ProcessStarted,checkpoint ProcessFinished,checkpoint WaferRemoved");
                throughputFile.WriteLine("Wafer count,Time last wafer, Time First wafer");
                throughputFile.WriteLine("wafer#,ProcessTime,WaferCycleInPM, ThroughtputCalculated");
                _cpt = 1;
            }
            catch (Exception ex)
            {
                _logger.Error($"Throughput file can not be created. Error = {ex.Message} - {ex.StackTrace}");
            }
            finally
            {
                if (throughputFile != null) throughputFile.Close();
            }
        }

        private void AddDataInThroughputDatafile(String data)
        {
            if (string.IsNullOrEmpty(_currentThroughputFullFilePathName))
            {
                _logger.Error("Throughtput data file path name is invalid");
                return;
            }

            StreamWriter throughputFile = null;
            try
            {
                throughputFile = File.AppendText(_currentThroughputFullFilePathName);
                throughputFile.WriteLine(data);
            }
            catch (Exception ex)
            {
                _logger.Error($"Throughput file can not be updated. Error = {ex.Message} - {ex.StackTrace}");
            }
            finally
            {
                if (throughputFile != null) throughputFile.Close();
            }
        }

        public void SetCheckpoint_WaferRemoved(string stationID, String waferID)
        {
            lock (_stopStatsSynchro)
            {
                CStats stats;
                _stationStats.TryGetValue(stationID, out stats);
                if (Enable_TS && (stats.CheckpointsWaferPlaced.Count > 0))
                {
                    if (stats.CurrentWaferIDPlaced == waferID)
                    {
                        stats.CheckpointsWaferRemoved.Add(DateTime.Now);
                        stats.CalculateWaferCounter();
                        stats.CurrentWaferIDPlaced = "";
                    }
                    else
                        return;
                }
            }
        }

        public void SetCheckpoint_WaferPlaced(string stationID, String waferID)
        {
            lock (_stopStatsSynchro)
            {
                CStats stats;
                _stationStats.TryGetValue(stationID, out stats);
                if (Enable_TS)
                {
                    stats.CurrentWaferIDPlaced = waferID;
                    stats.CheckpointsWaferPlaced.Add(DateTime.Now);
                    stats.CalculateWaferCounter();
                    SendCalculationStatsStart(stationID);
                }
            }
        }

        public void SetCheckpoint_ProcessStart(string stationID)
        {
            lock (_stopStatsSynchro)
            {
                CStats stats;
                _stationStats.TryGetValue(stationID, out stats);
                if (Enable_TS && (stats.CheckpointsWaferPlaced.Count > 0))
                {
                    stats.CheckpointsProcessStarted.Add(DateTime.Now);
                    stats.CalculateWaferCounter();
                }
            }
        }

        public void SetCheckpoint_ProcessFinished(string stationID)
        {
            lock (_stopStatsSynchro)
            {
                CStats stats;
                _stationStats.TryGetValue(stationID, out stats);
                if (Enable_TS && (stats.CheckpointsWaferPlaced.Count > 0))
                {
                    stats.CheckpointsProcessFinished.Add(DateTime.Now);
                    stats.CalculateWaferCounter();
                }
            }
        }

        private void SendCalculationStatsStart(string stationId)
        {
            Thread threadCalculation = new Thread(new ParameterizedThreadStart(CalcuclateThroughtputWaferStats_Execute));
            threadCalculation.Name = "THD_Calculation_Execute" + DateTime.Now.ToShortTimeString();
            threadCalculation.Start(stationId);
        }

        public void CalcuclateThroughtputWaferStats_Execute(Object pStationID)
        {
            try
            {
                string stationID = (string)pStationID;
                CStats stats = null;
                if (!_stationStats.TryGetValue(stationID, out stats)) return;
                if (stats == null) return;

                if (stats.WaferCounter == 1)
                    _referenceTimeForFile = stats.CheckpointsWaferPlaced[0];

                if (_throughputDataFileEnable && ((stats.WaferCounter - 1) >= 0) && (stats.CheckpointsWaferPlaced.Count > 0) && (stats.CheckpointsProcessStarted.Count > 0) && (stats.CheckpointsWaferRemoved.Count > 0) && (stats.CheckpointsProcessFinished.Count > 0) &&
                    (stats.CheckpointsWaferPlaced[stats.WaferCounter - 1] != null) && (stats.CheckpointsProcessStarted[stats.WaferCounter - 1] != null) && (stats.CheckpointsProcessFinished[stats.WaferCounter - 1] != null) && (stats.CheckpointsWaferRemoved[stats.WaferCounter - 1] != null))
                {
                    string t1 = stats.CheckpointsWaferPlaced[stats.WaferCounter - 1].Subtract(_referenceTimeForFile).ToString(@"mm\:ss\.fff");
                    string t2 = stats.CheckpointsProcessStarted[stats.WaferCounter - 1].Subtract(_referenceTimeForFile).ToString(@"mm\:ss\.fff");
                    string t3 = stats.CheckpointsProcessFinished[stats.WaferCounter - 1].Subtract(_referenceTimeForFile).ToString(@"mm\:ss\.fff");
                    string t4 = stats.CheckpointsWaferRemoved[stats.WaferCounter - 1].Subtract(_referenceTimeForFile).ToString(@"mm\:ss\.fff");
                    AddDataInThroughputDatafile($"{_cpt},{t1},{t2},{t3},{t4}");
                    _cpt++;
                }

                // Process time in PM per wafer
                TimeSpan totalProcessTimeInPM = TimeSpan.Zero;
                if ((stats.CheckpointsProcessStarted.Count > 0) && (stats.CheckpointsProcessStarted.Count == stats.CheckpointsProcessFinished.Count))
                {
                    totalProcessTimeInPM = stats.CheckpointsProcessFinished[stats.CheckpointsProcessFinished.Count - 1].Subtract(stats.CheckpointsProcessStarted[stats.CheckpointsProcessStarted.Count - 1]);
                    stats.ProcessTime = totalProcessTimeInPM.ToString(@"mm\:ss\.fff");
                    stats.InfoPM = "Process Time = Time [Process Finished] - Time [Process started]";
                }

                //Throughtput
                if (stats.CheckpointsWaferPlaced.Count - 2 >= 0)
                {
                    TimeSpan waferCylceTime = TimeSpan.Zero;
                    waferCylceTime = stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1].Subtract(stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 2]);
                    stats.WaferCycleTime = waferCylceTime.ToString(@"mm\:ss\.fff");
                    TimeSpan TotalwaferCylceTime = TimeSpan.Zero;
                    int waferMaximumNumber = _floatingWaferNumber;
                    if (waferMaximumNumber < 5) waferMaximumNumber = 5;
                    if (stats.CheckpointsWaferPlaced.Count <= waferMaximumNumber)
                    {
                        TotalwaferCylceTime = stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1].Subtract(stats.CheckpointsWaferPlaced[0]);
                        // Throughput wafer
                        _throughputWafer = ((double)(stats.CheckpointsWaferPlaced.Count - 1) / TotalwaferCylceTime.TotalSeconds) * (double)3600;
                        stats.ThroughtputMeasured = _throughputWafer.ToString("G5", CultureInfo.InvariantCulture);
                        AddDataInThroughputDatafile($"{waferMaximumNumber},{stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1].ToString(@"HH\:mm\:ss\.fff")},{stats.CheckpointsWaferPlaced[0].ToString(@"HH\:mm\:ss\.fff")}");
                    }
                    else
                    {
                        TotalwaferCylceTime = stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1].Subtract(stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1 - waferMaximumNumber]);

                        AddDataInThroughputDatafile($"{waferMaximumNumber},{stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1].ToString(@"HH\:mm\:ss\.fff")},{stats.CheckpointsWaferPlaced[stats.CheckpointsWaferPlaced.Count - 1 - waferMaximumNumber].ToString(@"HH\:mm\:ss\.fff")}");
                        // Throughput wafer
                        _throughputWafer = ((double)(waferMaximumNumber) / TotalwaferCylceTime.TotalSeconds) * (double)3600;
                        stats.ThroughtputMeasured = _throughputWafer.ToString("G5", CultureInfo.InvariantCulture);
                        if (stats.CheckpointsWaferPlaced.Count - 1 - waferMaximumNumber >= 1)
                        {
                            stats.CheckpointsProcessFinished.RemoveAt(0);
                            stats.CheckpointsProcessStarted.RemoveAt(0);
                            stats.CheckpointsWaferPlaced.RemoveAt(0);
                            stats.CheckpointsWaferRemoved.RemoveAt(0);
                        }
                    }

                    AddDataInThroughputDatafile($"{_cpt - 1},{stats.ProcessTime},{stats.WaferCycleTime},{stats.ThroughtputMeasured}");
                }
                if (_displayTrhoughtputResult != null)
                    _displayTrhoughtputResult(_stationStats, true);
            }
            catch
            {
            }
        }

        internal void Reset()
        {
            lock (_stopStatsSynchro)
            {
                _stationStats.Clear();
                _stationStats.Add("stidToolStation1", new CStats("stidToolStation1"));
                _stationStats.Add("stidToolStation2", new CStats("stidToolStation2"));
                _stationStats.Add("stidToolStation3", new CStats("stidToolStation3"));
                _stationStats.Add("stidToolStation4", new CStats("stidToolStation4"));
                _displayTrhoughtputResult(_stationStats, true);
            }
            CreateThroughputDataFile();
        }
    }
}
