using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Implementation;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Data.Composer;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.DataAccess.ResultScanner.Implementation
{
    public class ResultScanner : DataAccesServiceBase, IResultScanner, IResultScannerServer
    {
        private readonly ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private readonly ManualResetEvent _monitorNewItemEvent = new ManualResetEvent(false);
        private Thread _threadResultMonitor;
        private readonly ConcurrentPriorityQueue<ResultPrio> _queuetask;
        private readonly ConcurrentDictionary<long, ResultPrio> _queuetaskMgr; //<ResultItem.ID, ResultItem item ref in queue>

        // //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
        //private readonly ManualResetEvent _shutdownAcqEvent = new ManualResetEvent(false);
        //private readonly ManualResetEvent _monitorNewItemAcqEvent = new ManualResetEvent(false);
        //private Thread _threadResultMonitorAcq;
        //private readonly ConcurrentPriorityQueue<ResultPrioAcq> _queueAcqtask;
        //private readonly ConcurrentDictionary<long, ResultPrioAcq> _queueAcqtaskMgr; //<ResultItem.ID, ResultItem item ref in queue>

        private readonly IResultDataFactory _resfactory;

        private static readonly object s_lockRoughBinPrm = new object();
        private readonly DefectBins _defectBins = new DefectBins();

        public DefectBins GetKlarfDefectBins()
        {
            return _defectBins;
        }

        private static readonly object s_lockSizeBinPrm = new object();
        private SizeBins _sizeBins = new SizeBins();

        public SizeBins GetKlarfSizeBins()
        {
            return _sizeBins;
        }

        private static readonly object s_lockHazePrm = new object();
        private string _hazeDefaultColorMap = ColorMapHelper.ColorMaps.First().Name;

        private string _fmtModelDirectory = DataAccessConfiguration.Instance.TemplateResultFolderPath;
        private string _fmtModelFileName = DataAccessConfiguration.Instance.TemplateResultFileName;
        private PathComposer _pathComposer;

        private long _currentScanId = -1;
        //private long _currentScanId_ACQ = -1;

        public event StateChangeEventHandler StateChanged;

        public event StatisticsChangeEventHandler StatisticsChanged;

        public ResultScanner(ILogger logger) : base(logger)
        {
            _ = Shared.Data.ColorMap.ColorMapHelper.ColorMaps; // for init

            int initialCapacity = 512;
            _queuetask = new ConcurrentPriorityQueue<ResultPrio>(initialCapacity);
            //_queueAcqtask = new ConcurrentPriorityQueue<ResultPrioAcq>(initialCapacity);

            int concurrencyLevel = 8;
            _queuetaskMgr = new ConcurrentDictionary<long, ResultPrio>(concurrencyLevel, initialCapacity);
            //_queueAcqtaskMgr = new ConcurrentDictionary<long, ResultPrioAcq>(concurrencyLevel, initialCapacity);

            _resfactory = ClassLocator.Default.GetInstance<IResultDataFactory>();
        }

        private void InsertNewRoughBinSettings(List<int> pNewRbinsToAdd)
        {
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var Rnd = new Random();
                lock (s_lockRoughBinPrm)
                {
                    foreach (int nNewRbin in pNewRbinsToAdd)
                    {
                        var newRBsetting = new SQL.KlarfRoughSettings
                        {
                            RoughBin = nNewRbin,
                            Label = string.Format("Def_{0}", nNewRbin),
                            Color = Color.FromArgb(Rnd.Next(5, 255), Rnd.Next(5, 255), Rnd.Next(5, 255)).ToArgb()
                        };
                        unitOfWork.KlarfRoughSettingsRepository.Add(newRBsetting);
                    }
                    unitOfWork.Save();
                }
            };
            RefreshRoughBinSettings();
        }

        public void RefreshRoughBinSettings()
        {
            try
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var rbins = unitOfWork.KlarfRoughSettingsRepository.CreateQuery().ToList();
                    lock (s_lockRoughBinPrm)
                    {
                        _defectBins.Reset();
                        foreach (var rbin in rbins)
                        {
                            _defectBins.Add(new DefectBin()
                            {
                                RoughBin = rbin.RoughBin,
                                Label = rbin.Label,
                                Color = rbin.Color
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"RefreshRoughBinSettings Fatal error");
            }
        }

        public void RemoteUpdateKlarfDefectBins(DefectBins defbins)
        {
            try
            {
                var defbinList = defbins.DefectBinList;

                // parse, compare, and update
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var rbins = unitOfWork.KlarfRoughSettingsRepository.CreateQuery(true).ToList();

                    // add & update
                    foreach (var dbin in defbinList)
                    {
                        var dtorbin = rbins.SingleOrDefault(x => x.RoughBin == dbin.RoughBin);
                        if (dtorbin == null)
                        {
                            // add
                            _logger.Verbose($"--- Add remote Defect rough bin : <{dbin.RoughBin}> {dbin.Label} / clr={dbin.Color}");
                            unitOfWork.KlarfRoughSettingsRepository.Add(new SQL.KlarfRoughSettings()
                            {
                                RoughBin = dbin.RoughBin,
                                Label = dbin.Label,
                                Color = dbin.Color
                            }); ;
                        }
                        else
                        {
                            // update if différent
                            if (dtorbin.Label != dbin.Label || dtorbin.Color != dbin.Color)
                            {
                                _logger.Verbose($"--- Update remote Defect rough bin : <{dbin.RoughBin}> {dbin.Label} / clr={dbin.Color}");
                                dtorbin.Label = dbin.Label;
                                dtorbin.Color = dbin.Color;
                            }
                        }
                    }

                    // remove
                    foreach (var dtorbin in rbins)
                    {
                        var dbin = defbinList.SingleOrDefault(x => x.RoughBin == dtorbin.RoughBin);
                        if (dbin == null)
                        {
                            // remove dtobin
                            _logger.Information($"--- Remove remote Defect rough bin : <{dtorbin.RoughBin}> {dtorbin.Label} / clr={dtorbin.Color}");

                            unitOfWork.KlarfRoughSettingsRepository.Remove(dtorbin);
                        }
                    }
                    unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"RemoteUpdateKlarfDefectBins Fatal error");
            }
            finally
            {
                RefreshRoughBinSettings();
            }
        }

        public void RefreshSizeBinSettings()
        {
            try
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sizebins = unitOfWork.KlarfBinSettingsRepository.CreateQuery().OrderBy(x => x.AreaIntervalMax).ToList();

                    var szbins = new SizeBins();
                    foreach (var szbin in sizebins)
                    {
                        szbins.AddBin(szbin.AreaIntervalMax, szbin.SquareWidth);
                    }
                    lock (s_lockSizeBinPrm)
                    {
                        _sizeBins = szbins;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"RefreshSizeBinSettings Fatal error");
            }
        }

        public void RefreshHazeSettings()
        {
            try
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var Hazeclrmaps = unitOfWork.GlobalResultSettingsRepository.CreateQuery(false).OrderByDescending(x => x.Date).ToList();
                    string sColorMapName;
                    if (Hazeclrmaps.Count() == 0)
                    {
                        sColorMapName = ColorMapHelper.ColorMaps.First().Name;

                        unitOfWork.GlobalResultSettingsRepository.Add(new SQL.GlobalResultSettings()
                        {
                            ResultFormat = (int)ResultFormat.Haze,
                            Date = DateTime.Now,
                            DataSetting = sColorMapName,
                            XmlSetting = null
                        });

                        unitOfWork.Save();
                    }
                    else 
                    {
                        sColorMapName = Hazeclrmaps.First().DataSetting;
                    }

                    lock (s_lockHazePrm)
                    {
                        _hazeDefaultColorMap = sColorMapName;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"RefreshHazeSettings Fatal error");
            }
        }

        public void RemoteUpdateKlarfSizeBins(SizeBins szbins)
        {
            try
            {
                var szbinList = szbins.ListBins;

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sizebins = unitOfWork.KlarfBinSettingsRepository.CreateQuery(true).OrderBy(x => x.AreaIntervalMax).ToList();

                    // parse, compare, and update
                    // add & update
                    foreach (var sbin in szbinList)
                    {
                        var dtoszbin = sizebins.SingleOrDefault(x => x.AreaIntervalMax == sbin.AreaMax_um);
                        if (dtoszbin == null)
                        {
                            // add
                            _logger.Verbose($"--- Add remote Size bin : <{sbin.AreaMax_um}> {sbin.Size_um}");
                            unitOfWork.KlarfBinSettingsRepository.Add(new SQL.KlarfBinSettings()
                            {
                                AreaIntervalMax = sbin.AreaMax_um,
                                SquareWidth = sbin.Size_um
                            });
                        }
                        else
                        {
                            // update if différent
                            if (dtoszbin.SquareWidth != sbin.Size_um)
                            {
                                _logger.Verbose($"--- Update remote size bin : <{sbin.AreaMax_um}> {sbin.Size_um}");
                                dtoszbin.SquareWidth = sbin.Size_um;
                            }
                        }
                    }

                    // remove
                    foreach (var dtoszbin in sizebins)
                    {
                        var dbin = szbinList.SingleOrDefault(x => x.AreaMax_um == dtoszbin.AreaIntervalMax);
                        if (dbin == null)
                        {
                            // remove dtobin
                            _logger.Information($"--- Remove remote size bin : <{dtoszbin.AreaIntervalMax}> {dtoszbin.SquareWidth}");
                            unitOfWork.KlarfBinSettingsRepository.Remove(dtoszbin);
                        }
                    }
                    unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"RemoteUpdateKlarfSizeBins Fatal error");
            }
            finally
            {
                RefreshSizeBinSettings();
            }
        }

        public void RemoteUpdateHazeColorMap(string colormapname)
        {
            try
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var hazesettings = unitOfWork.GlobalResultSettingsRepository.CreateQuery(true).OrderByDescending(x => x.Date).ToList();

                    if (hazesettings.Count == 0)
                    {
                        //add
                        _logger.Verbose($"--- Add new Haze Settings colormap = {colormapname}");
                        unitOfWork.GlobalResultSettingsRepository.Add(new SQL.GlobalResultSettings()
                        {
                            ResultFormat = (int)ResultFormat.Haze,
                            Date = DateTime.Now,
                            DataSetting = colormapname,
                            XmlSetting = null
                        });
                    }
                    else
                    {
                        _logger.Verbose($"--- Update Haze Settings colormap = {colormapname}");

                        hazesettings[0].Date = DateTime.Now;
                        hazesettings[0].DataSetting = colormapname;

                        if (hazesettings.Count > 1)
                        {
                            string FmtDatetime = "yyyy/MM/dd HH:mm:ss";
                            // remove older ones (issue in database ?)
                            for (int i = 1; i < hazesettings.Count; i++)
                            {                 
                                _logger.Debug($"--- Remove older Haze settings : <{hazesettings[i].Date}>  Date : {hazesettings[i].Date.ToString(FmtDatetime)}");
                                unitOfWork.GlobalResultSettingsRepository.Remove(hazesettings[i]);
                            }
                        }
                    }
   
                    unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, $"RemoteUpdateHazeColorMap Fatal error");
            }
            finally
            {
                RefreshHazeSettings();
            }
           
        }


        public void ResultScanRequest(long databaseResultID, bool isAcquisition)
        {
            if (isAcquisition)
            {
                // //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
                //if (databaseResultID == _currentScanId_ACQ)
                //    return; // skip it scan is on going

                //if (_queueAcqtaskMgr.ContainsKey(databaseResultID))
                //{
                //    _logger.Debug("Raised Scan request priority on ACQ <" + databaseResultID.ToString() + ">");

                //    _queueAcqtaskMgr[databaseResultID].PrioCounter++;
                //    _queueAcqtask.Update(_queueAcqtaskMgr[databaseResultID]);
                //}
                //else
                //{
                //    _logger.Debug("New Scan request on ACQ <" + databaseResultID.ToString() + ">");

                //    using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                //    {
                //        var result = unitOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq, x => x.ResultAcq.Chamber.Tool, x => x.ResultAcq.WaferResult.Job).Where(x => x.Id == databaseResultID).FirstOrDefault();
                //        if (result != null)
                //        {
                //            var ResPrioAcq = new ResultPrioAcq(result);
                //            _queueAcqtaskMgr.TryAdd(ResPrioAcq.Id, ResPrioAcq);
                //            _queueAcqtaskMgr[ResPrioAcq.Id].PrioCounter = 2; // External request start with better prio than start up
                //            _queueAcqtask.Add(ResPrioAcq);
                //        }
                //    };
                //}
                //_monitorNewItemAcqEvent.Set();
            }
            else
            {
                if (databaseResultID == _currentScanId)
                    return; // skip it scan is on going

                if (_queuetaskMgr.ContainsKey(databaseResultID))
                {
                    _logger.Debug("Raised Scan request priority on <" + databaseResultID.ToString() + ">");

                    _queuetaskMgr[databaseResultID].PrioCounter++;
                    _queuetask.Update(_queuetaskMgr[databaseResultID]);
                }
                else
                {
                    _logger.Debug("New Scan request on <" + databaseResultID.ToString() + ">");

                    using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    {
                        var result = unitOfWork.ResultItemRepository.CreateQuery(false, x => x.Result,
                                                                                        x => x.Result.Chamber.Tool,
                                                                                        x => x.Result.WaferResult.Job,
                                                                                        x => x.Result.WaferResult.Product,
                                                                                        x => x.Result.Recipe.Step)
                                                                                .Where(x => x.Id == databaseResultID).FirstOrDefault();
                        if (result != null)
                        {
                            var ResPrio = new ResultPrio(result);
                            _queuetaskMgr.TryAdd(ResPrio.Id, ResPrio);
                            _queuetaskMgr[ResPrio.Id].PrioCounter = 2; // External request start with better prio than start up
                            _queuetask.Add(ResPrio);
                        }
                    };
                }
                _monitorNewItemEvent.Set();
            }
        }

        public void ResultReScanRequest(long databaseResultID, bool isAcquisition)
        {
            if (isAcquisition)
            {
                //  //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
                //if (databaseResultID == _currentScanId_ACQ)
                //    return; // skip it scan is on going

                //if (_queueAcqtaskMgr.ContainsKey(databaseResultID))
                //{
                //    _logger.Debug("Raised RE-Scan request on ACQ <" + databaseResultID.ToString() + ">");

                //    _queueAcqtaskMgr[databaseResultID].PrioCounter++;
                //    _queueAcqtask.Update(_queueAcqtaskMgr[databaseResultID]);
                //}
                //else
                //{
                //    _logger.Debug("RE-Scan request on ACQ <" + databaseResultID.ToString() + ">");

                //    using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                //    {
                //        var resultacq = unitOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq, x => x.ResultAcq.Chamber.Tool, x => x.ResultAcq.WaferResult.Job)
                //                                                 .Where(x => x.Id == databaseResultID).FirstOrDefault();
                //        if (resultacq != null)
                //        {
                //            int Jobid = resultacq.ResultAcq.WaferResult.JobId;
                //            // reinitialize internal status
                //            resultacq.InternalState = 0; // NotProcess
                //            unitOfWork.ResultAcqItemRepository.Update(resultacq);
                //            unitOfWork.Save();

                //            // refresh needed,  perfom the same query to have result with all needed contents
                //            resultacq = unitOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq, x => x.ResultAcq.Chamber.Tool, x => x.ResultAcq.WaferResult.Job)
                //                                                 .Where(x => x.Id == databaseResultID).FirstOrDefault();
                //            var ResPrioacq = new ResultPrioAcq(resultacq);
                //            _queueAcqtaskMgr.TryAdd(ResPrioacq.Id, ResPrioacq);
                //            _queueAcqtaskMgr[ResPrioacq.Id].PrioCounter = 20;
                //            _queueAcqtask.Add(ResPrioacq);

                //            var statemsg = new ResultScannerStateNotificationMessage()
                //            {
                //                //JobID = result.WaferResult.JobId
                //                ResultID = resultacq.Id,
                //                State = resultacq.State,
                //                InternalState = resultacq.InternalState,
                //            };
                //            StateChanged?.Invoke(statemsg);
                //        }
                //    };
                //}
                //_monitorNewItemAcqEvent.Set();
            }
            else
            {
                if (databaseResultID == _currentScanId)
                    return; // skip it scan is on going

                if (_queuetaskMgr.ContainsKey(databaseResultID))
                {
                    _logger.Debug("Raised RE-Scan request on <" + databaseResultID.ToString() + ">");

                    _queuetaskMgr[databaseResultID].PrioCounter++;
                    _queuetask.Update(_queuetaskMgr[databaseResultID]);
                }
                else
                {
                    _logger.Debug("RE-Scan request on <" + databaseResultID.ToString() + ">");

                    using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    {
                        // var reslid = unitOfWork.ResultRepository.FindByLongId(databaseResultID);
                        var result = unitOfWork.ResultItemRepository.CreateQuery(false, x => x.Result, 
                                                                                        x => x.Result.Chamber.Tool,
                                                                                        x => x.Result.WaferResult.Job,
                                                                                        x => x.Result.WaferResult.Product,
                                                                                        x => x.Result.Recipe.Step)
                                                                                        .Where(x => x.Id == databaseResultID).FirstOrDefault();
                        if (result != null)
                        {
                            int Jobid = result.Result.WaferResult.JobId;
                            // reinitialize internal status
                            result.InternalState = 0; // NotProcess
                            unitOfWork.ResultItemRepository.Update(result);

                            var resStats = unitOfWork.ResultItemValueRepository.CreateQuery().Where(x => x.ResultItemId == databaseResultID).ToList();
                            unitOfWork.ResultItemValueRepository.RemoveRange(resStats);
                            unitOfWork.Save();

                            // refresh needed,  perfom the same query to have result with all needed contents
                            result = unitOfWork.ResultItemRepository.CreateQuery(false,
                                                                    x => x.Result, 
                                                                    x => x.Result.Chamber.Tool,
                                                                    x => x.Result.WaferResult.Job,
                                                                    x => x.Result.WaferResult.Product,
                                                                    x => x.Result.Recipe.Step)
                                                                 .Where(x => x.Id == databaseResultID).FirstOrDefault();
                            var ResPrio = new ResultPrio(result);
                            _queuetaskMgr.TryAdd(ResPrio.Id, ResPrio);
                            _queuetaskMgr[ResPrio.Id].PrioCounter = 20;
                            _queuetask.Add(ResPrio);

                            var statemsg = new ResultScannerStateNotificationMessage()
                            {
                                //JobID = result.WaferResult.JobId
                                ResultID = result.Id,
                                State = result.State,
                                InternalState = result.InternalState,
                            };
                            StateChanged?.Invoke(statemsg);
                        }
                    };
                }
                _monitorNewItemEvent.Set();
            }
        }

        private void AddOrUpdateStatsResultValues(List<ResultDataStats> stats)
        {
            bool bSend = false;
            long dbresid = -1;
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                foreach (var dt in stats)
                {
                    dbresid = dt.DBResultItemId;
                    bSend = true;
                    var restatvalue = unitOfWork.ResultItemValueRepository.CreateQuery(true).Where(x => (x.Id == dt.DBResultItemId) && (x.Name == dt.Name) && (x.Type == dt.Type)).FirstOrDefault();
                    if (restatvalue == null)
                    {
                        _logger.Verbose($"--- Add Stats : <{dt.DBResultItemId}> {dt.Name}/{dt.Type}");
                        unitOfWork.ResultItemValueRepository.Add(new SQL.ResultItemValue()
                        {
                            ResultItemId = dt.DBResultItemId,
                            Name = dt.Name,
                            Value = dt.Value,
                            Type = dt.Type,
                            UnitType = dt.UnitType,
                        }); ;
                    }
                    else
                    {
                        _logger.Verbose($"--- Update Stats : <{dt.DBResultItemId}> {dt.Name}/{dt.Type}");

                        restatvalue.Value = dt.Value;
                        restatvalue.UnitType = dt.UnitType;
                    }
                }
                unitOfWork.Save();
            }

            if (bSend)
            {
                StatisticsChanged?.Invoke(dbresid);
            }
        }

        private void UpdateInternalStatus(long databaseResultItemID, Dto.ModelDto.Enum.ResultInternalState internalState)
        {
            bool bsend = false;
            var msg = new ResultScannerStateNotificationMessage();
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var result = unitOfWork.ResultItemRepository.FindByLongId(databaseResultItemID);
                if (result != null)
                {
                    result.InternalState = (int)internalState;
                    unitOfWork.ResultItemRepository.Update(result);
                    unitOfWork.Save();

                    // msg.JobID = result.WaferResult.JobId; comment faire ? sans nvlle requete ?
                    msg.ResultID = result.Id;
                    msg.State = result.State;
                    msg.InternalState = result.InternalState;
                    bsend = true;
                }
            }
            if (bsend)
                StateChanged?.Invoke(msg);
        }

        private void UpdateACQInternalStatus(long databaseResultID, Dto.ModelDto.Enum.ResultInternalState internalState)
        {
            bool bsend = false;
            var msg = new ResultScannerStateNotificationMessage();
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var resultacq = unitOfWork.ResultAcqItemRepository.FindByLongId(databaseResultID);
                if (resultacq != null)
                {
                    resultacq.InternalState = (int)internalState;
                    unitOfWork.ResultAcqItemRepository.Update(resultacq);
                    unitOfWork.Save();

                    // msg.JobID = result.WaferResult.JobId; comment faire ? sans nvlle requete ?
                    msg.ResultID = resultacq.Id;
                    msg.State = resultacq.State;
                    msg.InternalState = resultacq.InternalState;
                    bsend = true;
                }
            }
            if (bsend)
                StateChanged?.Invoke(msg);
        }

        public void Start()
        {
            string fmt;
            fmt = DataAccessConfiguration.Instance.TemplateResultFolderPath;
            if (fmt != null)
                SetModel_DirectoryName(fmt);
            fmt = DataAccessConfiguration.Instance.TemplateResultFileName;
            if (fmt != null)
                SetModel_FileName(fmt);

            _logger.Information(".");
            _logger.Information("------------------------");
            _logger.Information("-- Init Path Composer --");
            _logger.Information("------------------------");
            _logger.Information(" DBResults");
            _logger.Information($"-- Template.FileName = ${_fmtModelFileName}");
            _logger.Information($"-- Template.FolderPath = ${_fmtModelDirectory}");
            // SetModel_FileName and SetModel_DirectoryName should be caller prior to PathComposer initialisation
            _pathComposer = new PathComposer(_fmtModelFileName, _fmtModelDirectory);
            _logger.Information("-------------------------------------");
            _logger.Information($"-- RootExternalFilePath = ${DataAccessConfiguration.Instance.RootExternalFilePath}");
            _logger.Information($"-- Template.ExternalFilePath = ${DataAccessConfiguration.Instance.TemplateExternalFilePath}");
            _logger.Information("-------------------------------------");
            _logger.Information($"-- Template.TCPMRecipeName = ${DataAccessConfiguration.Instance.TemplateTCPMRecipeName}");
            _logger.Information("-------------------------------------\n");


            _logger.Debug("Refresh RoughBin Settings");
            RefreshRoughBinSettings();
            _logger.Debug("Refresh SizeBin Settings");
            RefreshSizeBinSettings();
            _logger.Debug("Refresh Globale Haze Settings");
            RefreshHazeSettings();

            _logger.Debug("Launch Monitoring ResultItem Thread");
            _threadResultMonitor = new Thread(ResultMonitorThreadFunc)
            {
                Name = "DataAccessResMonitorThread",
                IsBackground = true
            };
            _threadResultMonitor.Start();

            bool useAcqScannerThread = false; // pour le moment on ne store pas les acquisition en database... - TODO
            if (useAcqScannerThread)
            {
                // //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
                //_logger.Debug("Launch Monitoring Acquisition Thread");
                //_threadResultMonitorAcq = new Thread(ResultAcqMonitorThreadFunc)
                //{
                //    Name = "DataAccessResACQMonitorThread",
                //    IsBackground = true
                //};
                //_threadResultMonitorAcq.Start();
            }

            ////// init result
            var resultToScan = new List<ResultPrio>();
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                IEnumerable<SQL.ResultItem> sqlResultsToScan = unitOfWork.ResultItemRepository.CreateQuery(false,
                                                                            x => x.Result,
                                                                            x => x.Result.Chamber.Tool,
                                                                            x => x.Result.WaferResult.Job,
                                                                            x => x.Result.WaferResult.Product,
                                                                            x => x.Result.Recipe.Step)
                                                                         .Where(x => x.State >= 0 && x.InternalState == 0)
                                                                         .ToList();
                foreach (var res in sqlResultsToScan)
                    resultToScan.Add(new ResultPrio(res));
            };
            foreach (var resitem in resultToScan)
            {
                _queuetaskMgr.TryAdd(resitem.Id, resitem);
                _queuetask.Add(resitem);
            }

            if (resultToScan.Count != 0)
            {
                _logger.Debug("At startup : Queued " + resultToScan.Count + " results");
                _monitorNewItemEvent.Set();
            }

            // //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
            //////// init acquisition
            //var acqToScan = new List<ResultPrioAcq>();
            //using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            //{
            //    IEnumerable<SQL.ResultAcqItem> sqlAcqToScan = unitOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq, x => x.ResultAcq.Chamber.Tool, x => x.ResultAcq.WaferResult.Job)
            //                                                             .Where(x => x.State >= 0 && x.InternalState == 0)
            //                                                             .ToList();
            //    foreach (var acq in sqlAcqToScan)
            //        acqToScan.Add(new ResultPrioAcq(acq));
            //};
            //foreach (var acqitem in acqToScan)
            //{
            //    _queueAcqtaskMgr.TryAdd(acqitem.Id, acqitem);
            //    _queueAcqtask.Add(acqitem);
            //}

            //if (acqToScan.Count != 0)
            //{
            //    _logger.Debug("At startup : Queued " + acqToScan.Count + " ACQ results");
            //    _monitorNewItemAcqEvent.Set();
            //}
        }

        public void Stop()
        {
            _logger.Debug("rs Stop");

            _shutdownEvent.Set();

            if (!_threadResultMonitor.Join(3500))
            { // give the thread 3 seconds to stop
                _threadResultMonitor.Abort();
            }

            // //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
            //_shutdownAcqEvent.Set();

            //if (_threadResultMonitorAcq != null)
            //{
            //    if (!_threadResultMonitorAcq.Join(1000))
            //    { // give the thread 1000 seconds to stop
            //        _threadResultMonitorAcq.Abort();
            //    }
            //}
        }

        private void ResultMonitorThreadFunc()
        {
            _logger.Information($"Results Monitor launched");
            while (!_shutdownEvent.WaitOne(0))
            {
                _logger.Verbose($"Scanner Thread Idle : Queue Size = {_queuetask.Count}");

                if (_monitorNewItemEvent.WaitOne(3000))
                {
                    while (_queuetask.Count > 0)
                    {
                        var res2scan = _queuetask.Take();
                        _currentScanId = res2scan.Id;
                        try
                        {
                            _logger.Debug($"Scan start on <{res2scan.Id}> : {res2scan.Result.Chamber.Tool.Name}/{res2scan.Result.Chamber.Name} {res2scan.Result.WaferResult.WaferName} - {res2scan.ResultType}");

                            // get file path
                            string sResPath = ResultPathFromResult(res2scan);

                            var obj = _resfactory.CreateFromFile(res2scan.ResultTypeEnum, res2scan.Id, sResPath);
                            if (obj.ResType.GetResultFormat() == ResultFormat.Klarf) // Klarf - Specific
                            {
                                var NewBinToAdd = (List<int>)(obj.InternalTableToUpdate((object)_defectBins.RoughBinList));
                                if (NewBinToAdd.Count > 0)
                                {
                                    InsertNewRoughBinSettings(NewBinToAdd);
                                }
                            }

                            object[] inStatsprm = null;
                            var resstatvalue = _resfactory.GenerateStatisticsValues(obj, inStatsprm);
                            if (resstatvalue.Count > 0)
                            {
                                // maj db
                                // update result value table
                                AddOrUpdateStatsResultValues(resstatvalue);
                            }

                            object[] inThumbprm = null;
                            if (obj.ResType.GetResultFormat() == ResultFormat.Klarf)
                            { inThumbprm = new object[] { _defectBins, _sizeBins }; }
                            else if (obj.ResType.GetResultFormat() == ResultFormat.Haze)
                            { inThumbprm = new object[] { _hazeDefaultColorMap }; }
                            bool bThumbSuccess = _resfactory.GenerateThumbnailFile(obj, inThumbprm);
                            UpdateInternalStatus(res2scan.Id, bThumbSuccess ? Dto.ModelDto.Enum.ResultInternalState.Ok : Dto.ModelDto.Enum.ResultInternalState.Error);

                            _logger.Information($"Scan Complete on <{res2scan.Id}> : {res2scan.Result.Chamber.Tool.Name}/{res2scan.Result.Chamber.Name} {res2scan.Result.WaferResult.WaferName} - {res2scan.ResultType}");
                        }
                        catch (Exception ex)
                        {
                            UpdateInternalStatus(res2scan.Id, Dto.ModelDto.Enum.ResultInternalState.Error);
                            _logger.Error(ex, $"Scan Error on <{res2scan.Id}> : {res2scan.Result.Chamber.Tool.Name}/{res2scan.Result.Chamber.Name} {res2scan.Result.WaferResult.WaferName} - {res2scan.ResultType}");
                        }
                        finally
                        {
                            _queuetaskMgr.TryRemove(res2scan.Id, out var resprio);
                            _currentScanId = -1;
                        }
                    }
                    _monitorNewItemEvent.Reset();
                }
            }
            _logger.Information($"Results Monitor terminated");
        }

        //  //ON HOLD :  when decision will be made of who will created acquisition results thumbnail (Rti : 02/2022)
        //private void ResultAcqMonitorThreadFunc()
        //{
        //    _logger.Information($"Acquisition Monitor launched");
        //    while (!_shutdownAcqEvent.WaitOne(0))
        //    {
        //        //_logger.Verbose($"Scanner Acq Thread Idle");

        //        if (_monitorNewItemAcqEvent.WaitOne(3000))
        //        {
        //            while (_queueAcqtask.Count > 0)
        //            {
        //                ResultPrioAcq acq2scan = _queueAcqtask.Take();
        //                _currentScanId_ACQ = acq2scan.Id;
        //                try
        //                {
        //                    _logger.Debug($"Scan start on ACQ <{acq2scan.Id}> : {acq2scan.ResultAcq.Chamber.Tool.Name}/{acq2scan.ResultAcq.Chamber.Name} {acq2scan.ResultAcq.WaferResult.WaferName} - {acq2scan.ResultType}");

        //                    // get file path
        //                    string sAcqPath = System.IO.Path.Combine(acq2scan.ResultAcq.PathName, acq2scan.FileName);
        //                    bool bThumbSuccess = MakeAcqThumbnail(acq2scan);

        //                    UpdateACQInternalStatus(acq2scan.Id, bThumbSuccess ? Dto.ModelDto.Enum.ResultInternalState.Ok : Dto.ModelDto.Enum.ResultInternalState.Error);

        //                    _logger.Information($"Scan Complete on ACQ <{acq2scan.Id}> : {acq2scan.ResultAcq.Chamber.Tool.Name}/{acq2scan.ResultAcq.Chamber.Name} {acq2scan.ResultAcq.WaferResult.WaferName} - {acq2scan.ResultType}");
        //                }
        //                catch (Exception ex)
        //                {
        //                    UpdateInternalStatus(acq2scan.Id, Dto.ModelDto.Enum.ResultInternalState.Error);
        //                    _logger.Error(ex, $"Scan Error on ACQ <{acq2scan.Id}> : {acq2scan.ResultAcq.Chamber.Tool.Name}/{acq2scan.ResultAcq.Chamber.Name} {acq2scan.ResultAcq.WaferResult.WaferName} - {acq2scan.ResultType}");
        //                }
        //                finally
        //                {
        //                    _queuetaskMgr.TryRemove(acq2scan.Id, out ResultPrio resprio);
        //                    _currentScanId = -1;
        //                }
        //            }
        //        }
        //    }
        //    _logger.Information($"Acquisition Monitor terminated");
        //}

        public void SetModel_FileName(string sModel)
        {
            _fmtModelFileName = sModel;
        }

        public void SetModel_DirectoryName(string sModel)
        {
            _fmtModelDirectory = sModel;
        }

        public string ResultPathFromResultId(long databaseResultID)
        {
            Dto.Result result = null;
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                result = Mapper.Map<Dto.Result>(unitOfWork.ResultRepository.CreateQuery(false,
                    x => x.Chamber.Tool,
                    x => x.WaferResult.Job,
                    x => x.WaferResult.Product,
                    x => x.Recipe.Step)
                    .Where(x => x.Id == databaseResultID).DefaultIfEmpty(null).FirstOrDefault());
            };
            return ResultPathFromResult(result);
        }

        public string ResultPathFromResult(object resultItemDBQueryObj)
        {
            var res = resultItemDBQueryObj as Dto.ResultItem;
            string sFileName = res.FileName + "." + ResultFormatExtension.GetExt(res.ResultTypeEnum);
            var prm = new ResultPathParams
            {
                ToolName = res.Result.Chamber.Tool.Name,
                ToolId = res.Result.Chamber.Tool.Id,
                ToolKey = res.Result.Chamber.Tool.ToolKey,      
                ChamberName = res.Result.Chamber.Name,
                ChamberId = res.Result.Chamber.Id,
                ChamberKey = res.Result.Chamber.ChamberKey,
                JobName = res.Result.WaferResult.Job.JobName,
                JobId = res.Result.WaferResult.JobId,
                LotName = res.Result.WaferResult.Job.LotName,
                RecipeName = res.Result.WaferResult.Job.RecipeName,
                StartProcessDate = res.Result.WaferResult.Job.Date,
                Slot = res.Result.WaferResult.SlotId,
                RunIter = res.Result.WaferResult.Job.RunIter,
                WaferName = res.Result.WaferResult.WaferName,
                ResultType= res.ResultTypeEnum,
                Index = res.Idx,
                ProductName = res.Result.WaferResult.Product.Name,
                StepName = res.Result.Recipe.Step.Name
            };

            return System.IO.Path.Combine(BuildResultDirectoryName(prm), sFileName);
        }

        public void InitResultPathFromResult(object resultDBQueryObj)
        {
            if (!(resultDBQueryObj is Dto.ResultItem res))
                throw new NullReferenceException("Dto ResultItem object is null in InitResultPathFromResult");

            string sFileName = res.FileName + "." + ResultFormatExtension.GetExt(res.ResultTypeEnum);
            var prm = new ResultPathParams
            {
                ToolName = res.Result.Chamber.Tool.Name,
                ToolId = res.Result.Chamber.Tool.Id,
                ToolKey = res.Result.Chamber.Tool.ToolKey,
                ChamberName = res.Result.Chamber.Name,
                ChamberId = res.Result.Chamber.Id,
                ChamberKey = res.Result.Chamber.ChamberKey,
                JobName = res.Result.WaferResult.Job.JobName,
                JobId = res.Result.WaferResult.JobId,
                LotName = res.Result.WaferResult.Job.LotName,
                RecipeName = res.Result.WaferResult.Job.RecipeName,
                StartProcessDate = res.Result.WaferResult.Job.Date,
                Slot = res.Result.WaferResult.SlotId,
                RunIter = res.Result.WaferResult.Job.RunIter,
                WaferName = res.Result.WaferResult.WaferName,
                ResultType = res.ResultTypeEnum,
                Index = res.Idx,
                ProductName = res.Result.WaferResult.Product.Name,
                StepName = res.Result.Recipe.Step.Name
            };

            res.ResPath = System.IO.Path.Combine(BuildResultDirectoryName(prm), sFileName);
            res.ResThumbnailPath = FormatHelper.ThumbnailPathOf(res.ResPath);
        }

        public string BuildResultFileName(ResultPathParams prm)
        {
            return _pathComposer.GetFileName(prm);
        }

        public string BuildResultDirectoryName(ResultPathParams prm)
        {
            return _pathComposer.GetDirPath(prm);
        }
    }
}
