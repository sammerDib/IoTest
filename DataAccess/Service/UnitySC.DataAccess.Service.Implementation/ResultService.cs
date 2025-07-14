using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.DataAccess.Dto.ModelDto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ColorMap;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    /// <summary>
    /// Service to access to result data
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class ResultService : DataAccesDuplexServiceBase<IResultServiceCallback>, IResultService
    {
        private const int NB_SLOT_MAX = 25;
        private IResultScanner _resultScanner;

        #region Constructor

        public ResultService(ILogger logger, IResultScanner resultScanner, IRegisterResultService registerservice,
                            IResultScannerServer resultScannerServer, IRegisterResultServer registerserver)
        : base(logger)
        {
            _resultScanner = resultScanner;

            resultScannerServer.StateChanged += OnResultStateChangedFromScanner;
            resultScannerServer.StatisticsChanged += OnResultStatisticChangedFromScanner;

            registerserver.StateChanged += OnResultStateChanged;
            registerserver.ResultAdded += OnResultChanged;
        }

        #endregion Constructor

        #region Methods

        //=================================================================
        // Implementation du CallBack du service
        //=================================================================
        private void OnResultChanged(ResultNotificationMessage msg)
        {
            InvokeCallback(x => x.OnResultChanged(msg));
        }

        private void OnResultStatsChanged(ResultStatsNotificationMessage msg)
        {
            InvokeCallback(x => x.OnResultStatsChanged(msg));
        }

        private void OnResultStateChanged(ResultStateNotificationMessage msg)
        {
            InvokeCallback(x => x.OnResultStateChanged(msg));
        }

        private void OnResultStateChangedFromScanner(ResultScannerStateNotificationMessage msg)
        {
            var resmsg = new ResultStateNotificationMessage()
            {
                ResultID = msg.ResultID,
                State = msg.State,
                InternalState = msg.InternalState,
                IsAcquisitionResult = false
            };
            OnResultStateChanged(resmsg);
        }

        private void OnResultStatisticChangedFromScanner(long dBresId)
        {
            ResultStatsNotificationMessage msg = null;
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                // we need to have a complete result dto to send to call back with chamber, tool and resultsvalues
                var FullDepthDtoResultItem = unitOfWork.ResultItemRepository.CreateQuery(false,
                    x => x.Result,
                    x => x.Result.Chamber.Tool,
                    x => x.Result.WaferResult.Job,
                    x => x.Result.Recipe,
                    x => x.ResultItemValues,
                    x => x.Result.WaferResult.Product,
                    x => x.Result.Recipe.Step)
                   .Where(r => r.Id == dBresId).FirstOrDefault();

                if (FullDepthDtoResultItem != null)
                {
                    var waferdata = SpecificMapper.MapSQLtoWaferData(FullDepthDtoResultItem);
                    msg = new ResultStatsNotificationMessage()
                    {
                        JobID = FullDepthDtoResultItem.Result.WaferResult.JobId,
                        WaferResultData = waferdata
                    };
                }
            }

            if (msg != null)
                OnResultStatsChanged(msg);
        }

        //=================================================================
        // Implementation du service
        //=================================================================

        /// <summary>
        /// Check if databade version is up to date 
        /// </summary>
        /// <returns> true if version is valid</returns>
        public Response<bool> CheckDatabaseVersion()
        {
             return InvokeDataResponse(messageContainer =>
            {
                return DataAccessHelper.CheckDatabaseVersion(messageContainer, Mapper);
            });
        }



        /// <summary>
        /// Returns the list of products.(WaferTypes)
        /// </summary>
        /// <returns></returns>
        public Response<List<ResultQuery>> GetProducts()
        {
            return InvokeDataResponse(messageContainer =>
            {
                var productsList = new List<ResultQuery>();
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    // recover all productsid include database. all productid are shared amonsgt all tools no need to différentiate them from toolid
                    productsList = unitOfWork.ProductRepository.CreateQuery().Select(x => new ResultQuery() { Id = x.Id, Name = x.Name, IsArchived = x.IsArchived })
                                                                             .OrderBy(x => x.Name).ToList();
                }
                return productsList;
            });
        }

        /// <summary>
        /// Returns the list of recipe.
        /// </summary>
        /// <param name="Tool Key"></param>
        /// <returns>Liste de ResultQuery</returns>
        public Response<List<string>> GetRecipes(int? pToolkey)
        {
            var recipes = new List<string>();

            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (pToolkey.HasValue)
                    {
                        var toolid = GetToolIdFromToolKey(pToolkey.Value, unitOfWork);
                        recipes = unitOfWork.JobRepository.CreateQuery()
                                                            .Where( x => x.ToolId == toolid)
                                                            .Select(x => x.RecipeName).Distinct().ToList();
                    }
                    else
                        recipes = unitOfWork.JobRepository.CreateQuery().Select(x => x.RecipeName).Distinct().ToList();
                    return recipes;
                }
            });
        }

        /// <summary>
        /// RReturns the list of lots.
        /// </summary>
        /// <param name="pToolKey"></param>
        /// <returns></returns>
        public Response<List<string>> GetLots(int? pToolKey)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (pToolKey.HasValue)
                    {
                        var toolId = GetToolIdFromToolKey(pToolKey.Value, unitOfWork);
                        return unitOfWork.JobRepository.CreateQuery().Where(job => job.ToolId == toolId)
                                                                     .OrderBy(x => x.LotName)
                                                                     .Select(x => x.LotName).Distinct().ToList();
                    }
           
                    var lots = unitOfWork.JobRepository.CreateQuery().OrderBy(x => x.LotName)
                                                                     .Select(x => x.LotName).Distinct().ToList();
                    return lots;
                }
            });
        }

        /// <summary>
        /// Returns the list of tools with their Names & tool database IDs (db primary keys)
        /// </summary>
        /// <returns></returns>
        public Response<List<ResultQuery>> GetTools()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var tools = unitOfWork.ToolRepository.CreateQuery()
                                                         .Select(x => new ResultQuery()
                                                         {
                                                             Id = x.Id,
                                                             Name = x.Name,
                                                             IsArchived = x.IsArchived
                                                         }).OrderBy(x => x.Name).ToList();
                    return tools;
                }
            });
        }

        /// <summary>
        /// Returns the list of tools with their Names & tool Configuration Keys
        /// </summary>
        /// <returns></returns>
        public Response<List<ResultQuery>> GetToolsKey()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var toolsKey = unitOfWork.ToolRepository.CreateQuery()
                                                         .Select(x => new ResultQuery()
                                                         {
                                                             Id = x.ToolKey,
                                                             Name = x.Name,
                                                             IsArchived = x.IsArchived
                                                         }).OrderBy(x => x.Name).ToList();
                    return toolsKey;
                }
            });
        }

        /// <summary>
        /// Returns the list of chambers (process modules)
        /// </summary>
        /// <param name="pToolKey"></param>
        /// <returns></returns>
        public Response<List<ResultChamberQuery>> GetChambers(int? pToolKey, int? pToolDatabaseID, bool hasAnyResult = true)
        {
            var chambers = new List<ResultChamberQuery>();
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int? pToolId = null;
                    if (pToolDatabaseID.HasValue)
                        pToolId = pToolDatabaseID;
                    if (pToolKey.HasValue)
                        pToolId = GetToolIdFromToolKey(pToolKey.Value, unitOfWork);

                    if (hasAnyResult)
                    {
                        // On recupère uniquement les chambres liées au moins à un job c'est à dire qui ont des résultats( les chambres dont les id figurent dans la table ResultItem).
                        chambers = unitOfWork.ChamberRepository.CreateQuery().Where(chb => (!pToolId.HasValue || chb.ToolId == pToolId.Value) && chb.Results.Any()).AsEnumerable()
                                                                             .GroupBy(x => x.ActorType)
                                                                             .Select(x => new ResultChamberQuery()
                                                                             {
                                                                                 Id = x.Select(g => g.Id).First(),
                                                                                 Name = x.Select(g => g.Name).First(), //   Name = ((ActorType)x.Select(g => g.ActorType).First()).ToString(),
                                                                                 IsArchived = x.Select(g => g.IsArchived).First(),
                                                                                 ActorType = ((ActorType)x.Select(g => g.ActorType).First()),
                                                                                 ToolIdOwner = x.Select(g => g.ToolId).First(),

                                                                             })
                                                                             .OrderBy(x => x.Id).ToList();
                    }
                    else
                    {
                        // On recupère uniquement les chambres liées 
                        chambers = unitOfWork.ChamberRepository.CreateQuery().Where(chb => (!pToolId.HasValue || chb.ToolId == pToolId.Value)).AsEnumerable()
                                                                             .GroupBy(x => x.ActorType)
                                                                             .Select(x => new ResultChamberQuery()
                                                                             {
                                                                                 Id = x.Select(g => g.Id).First(),
                                                                                 Name = x.Select(g => g.Name).First(), //   Name = ((ActorType)x.Select(g => g.ActorType).First()).ToString(),
                                                                                 IsArchived = x.Select(g => g.IsArchived).First(),
                                                                                 ActorType = ((ActorType)x.Select(g => g.ActorType).First()),
                                                                                 ToolIdOwner = x.Select(g => g.ToolId).First(),

                                                                             })
                                                                             .OrderBy(x => x.Id).ToList();

                    }

                    return chambers;
                }
            });
        }

        /// <summary>
        /// Returns a list of jobs matching the specified search criteria
        /// </summary>
        /// <param name="pOb"></param>
        /// <param name="pTop"></param>
        /// <returns></returns>
        public Response<List<Dto.Job>> GetSearchJobs(SearchParam pOb)
        {
            return InvokeDataResponse(messageContainer =>
            {
                using (var unityWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sqlParam = Mapper.Map<SQL.ModelSQL.LocalSQL.SearchParam>(pOb);
                    var jobsList = unityWork.GetJobsList(sqlParam).OrderByDescending(x => x.Date);

                    //foreach (var job in jobsList)
                    //    job.Date = DateTime.ParseExact(job.Date.Date.ToString(), "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    // Message to client exemple
                    //messageContainer.Add(new Message(MessageLevel.Information, jobsList.Count() + " results found"));
                    return Mapper.Map<List<Dto.Job>>(jobsList);
                }
            });
        }

        /// <summary>
        /// Returns the results of process modules for a given job. with or without acquisition data
        /// </summary>
        /// <param name="pToolId"></param>
        /// <param name="pJobId"></param>
        /// <returns></returns>
        public Response<List<ProcessModuleResult>> GetJobProcessModulesResults(int pToolId, int pJobId, bool bQueryAcqData = false)
        {
            return InvokeDataResponse(messageContainer =>
            {
              
                var listPM = new List<ProcessModuleResult>();
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var resItems = unitOfWork.ResultItemRepository.CreateQuery(false, x => x.Result,
                                                                                        x => x.Result.Chamber.Tool,
                                                                                        x => x.Result.WaferResult.Job,
                                                                                        x => x.Result.Recipe,
                                                                                        x => x.ResultItemValues,
                                                                                        x => x.Result.WaferResult.Product,
                                                                                        x => x.Result.Recipe.Step)
                                                                       .Where(r => r.Result.Chamber.ToolId == pToolId && r.Result.WaferResult.JobId == pJobId)
                                                                       .OrderBy(x => x.Result.WaferResult.SlotId).ToList();

                    foreach (var x in resItems)
                    {
                        if (! Enum.IsDefined(typeof(ActorType), x.Result.ActorType))
                             throw new Exception($"Unknown Actortype <{x.Result.ActorType}> in Result id = {x.ResultId}");

                        var actor = (ActorType)x.Result.ActorType;
                        string labelActor = actor.GetLabelName();
                        var restyp = (ResultType)x.ResultType;
                        string labelResType = x.Name ?? restyp.DefaultLabelName((byte)x.Idx);

                        var PM = listPM.Find(r => (r.ActorType == actor && r.ChamberId == x.Result.ChamberId));
                        if (PM == null)
                        {
                            // PM not exist add it
                            PM = new ProcessModuleResult
                            {
                                ChamberId = x.Result.ChamberId,
                                ActorType = actor,
                                LabelPMName = labelActor,
                                PostProcessingResults = new Dictionary<string, WaferResultData[]>(),
                                AcquisitionResults = new Dictionary<string, WaferAcquisitionResult[]>()
                            };
                            listPM.Add(PM);
                        }

                        if (!PM.PostProcessingResults.ContainsKey(labelResType))
                        {
                            PM.PostProcessingResults.Add(labelResType, new WaferResultData[NB_SLOT_MAX]);
                        }

                        // Compose result and thumbnail result path --> update ResPath & ResThumbnailPath property of Dto.ResultItem
                        var xdto = Mapper.Map<Dto.ResultItem>(x);
                        _resultScanner.InitResultPathFromResult(xdto);
                        if (x.State >= (int)ResultState.Ok && x.InternalState == (int)ResultInternalState.NotProcess)
                            _resultScanner.ResultScanRequest(xdto.Id, false);

                        var lotdata = PM.PostProcessingResults[labelResType];
                        int slotId = x.Result.WaferResult.SlotId;
                        lotdata[slotId-1] = new WaferResultData
                        {
                            SlotId = slotId,
                            WaferName = $"{slotId} - {x.Result.WaferResult.WaferName}",
                            ResultItem = xdto,
                            ActorType = actor
                        };
                    }

                    if (bQueryAcqData == true)
                    {
                        var resAcqItems = unitOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq,
                                                                                        x => x.ResultAcq.Chamber.Tool,
                                                                                        x => x.ResultAcq.WaferResult.Job,
                                                                                        x => x.ResultAcq.Recipe)
                                                                       .Where(r => r.ResultAcq.Chamber.ToolId == pToolId && r.ResultAcq.WaferResult.JobId == pJobId)
                                                                       .OrderBy(x => x.ResultAcq.WaferResult.SlotId).ToList();

                        foreach (var x in resAcqItems)
                        {
                            if (!Enum.IsDefined(typeof(ActorType), x.ResultAcq.ActorType))
                                throw new Exception($"Unknown Acquisition Actortype <{x.ResultAcq.ActorType}> in Result id = {x.ResultAcqId}");

                            var actor = (ActorType)x.ResultAcq.ActorType;
                            var restyp = (ResultType)x.ResultType;
                            string labelAcqType = x.Name ?? restyp.DefaultLabelName((byte)x.Idx);

                            var PM = listPM.Find(r => (r.ActorType == actor && r.ChamberId == x.ResultAcq.ChamberId));
                            if (PM == null)
                            {
                                // PM not exist add it
                                PM = new ProcessModuleResult
                                {
                                    ChamberId = x.ResultAcq.ChamberId,
                                    ActorType = actor,
                                    LabelPMName = actor.GetLabelName(),
                                    PostProcessingResults = new Dictionary<string, WaferResultData[]>(),
                                    AcquisitionResults = new Dictionary<string, WaferAcquisitionResult[]>()
                                };
                                listPM.Add(PM);
                            }

                            if (!PM.AcquisitionResults.ContainsKey(labelAcqType))
                            {
                                PM.AcquisitionResults.Add(labelAcqType, new WaferAcquisitionResult[NB_SLOT_MAX]);
                            }

                            var xdto = Mapper.Map<Dto.ResultAcqItem>(x);
                            xdto.ResPath = InPreRegisterAcqHelper.FullAcqFilePath(xdto.ResultAcq.PathName, xdto.FileName);
                            xdto.ResThumbnailPath = InPreRegisterAcqHelper.FullAcqFilePathThumbnail(xdto.ResultAcq.PathName, xdto.FileName);

                            var lotdata = PM.AcquisitionResults[labelAcqType];
                            int slotId = x.ResultAcq.WaferResult.SlotId;
                            lotdata[slotId - 1] = new WaferAcquisitionResult
                            {
                                SlotId = slotId,
                                PathName = x.ResultAcq.PathName,
                                AcqItem = xdto,
                                ActorType = actor
                            };
                        }
                    }
                }

                return listPM;
            });
        }

        public Response<KlarfSettingsData> GetKlarfSettingsFromScanner()
        {
            return InvokeDataResponse(messageContainer =>
            {
                var klarfsettings = new KlarfSettingsData();
                klarfsettings.RoughBins = _resultScanner.GetKlarfDefectBins();
                klarfsettings.SizeBins = _resultScanner.GetKlarfSizeBins();
                return klarfsettings;
            });
        }

        public Response<KlarfSettingsData> GetKlarfSettingsFromTables()
        {
            return InvokeDataResponse(messageContainer =>
            {
                var klarfsettings = new KlarfSettingsData();
                klarfsettings.RoughBins = new DefectBins();
                klarfsettings.SizeBins = new SizeBins();

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var rbins = unitOfWork.KlarfRoughSettingsRepository.CreateQuery().ToList();
                    foreach (var rbin in rbins)
                    {
                        klarfsettings.RoughBins.Add(new DefectBin()
                        {
                            RoughBin = rbin.RoughBin,
                            Label = rbin.Label,
                            Color = rbin.Color
                        });
                    }

                    var sizebins = unitOfWork.KlarfBinSettingsRepository.CreateQuery().OrderBy(x => x.AreaIntervalMax).ToList();
                    foreach (var szbin in sizebins)
                    {
                        klarfsettings.SizeBins.AddBin(szbin.AreaIntervalMax, szbin.SquareWidth);
                    }
                };
                return klarfsettings;
            });
        }

        public Response<string> GetHazeSettingsFromTables()
        {
            return InvokeDataResponse(messageContainer =>
            {
                string sColorMapName = ColorMapHelper.ColorMaps.First().Name;
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var setting = unitOfWork.GlobalResultSettingsRepository.CreateQuery(false)
                                                      .Where(x => x.ResultFormat == (int)ResultFormat.Haze)
                                                      .FirstOrDefault();

                    if (setting != null && !string.IsNullOrEmpty(setting.DataSetting))
                    {
                        sColorMapName = setting.DataSetting;
                    }
                }
                return sColorMapName;
            });

        }



        /// <summary>
        /// Add/Increase result scan request priority in scanner queue.
        /// </summary>
        /// <param name="resultDBId"> database result id primary key </param>
        public Response<VoidResult> ResultScanRequest(long resultDBId, bool isAcquisition)
        {
            return InvokeVoidResponse<object>(() =>
            {
                _resultScanner.ResultScanRequest(resultDBId, isAcquisition);
                return null;
            });
        }

        public Response<VoidResult> ResultReScanRequest(long resultDBId, bool isAcquisition)
        {
            return InvokeVoidResponse<object>(() =>
            {
                _resultScanner.ResultReScanRequest(resultDBId, isAcquisition);
                return null;
            });
        }

        /// <summary>
        /// Tests if the connection is available.
        /// </summary>
        /// <returns></returns>
        public Response<bool> IsConnectionAvailable()
        {
            return InvokeDataResponse(() =>
            {
                bool isConnectionOk = false;
                GetTools();
                isConnectionOk = true;
                return isConnectionOk;
            });
        }

        public Response<VoidResult> RemoteUpdateKlarfSizeBins(SizeBins szbins)
        {
            return InvokeVoidResponse<object>(() =>
            {
                _resultScanner.RemoteUpdateKlarfSizeBins(szbins);
                return null;
            });
        }

        public Response<VoidResult> RemoteUpdateKlarfDefectBins(DefectBins defbins)
        {
            return InvokeVoidResponse<object>(() =>
            {
                _resultScanner.RemoteUpdateKlarfDefectBins(defbins);
                return null;
            });
        }

        public Response<VoidResult> RemoteUpdateHazeColorMap(string colormapname)
        {
            return InvokeVoidResponse<object>(() =>
            {
                _resultScanner.RemoteUpdateHazeColorMap(colormapname);
                return null;
            });
        }

        public Response<List<ResultQuery>> RetrieveToolIdFromToolKey(int toolKey)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    // tool keys are not priùary key and then we could have some several occurance of tool with same key if it has not been well configured
                    var toolsId = unitOfWork.ToolRepository.CreateQuery()
                                                         .Where(x => x.ToolKey == toolKey)
                                                         .Select(x => new ResultQuery()
                                                         {
                                                             Id = x.Id,
                                                             Name = x.Name,
                                                             IsArchived = x.IsArchived
                                                         }).OrderBy(x => x.Name).ToList();
                    return toolsId;
                }
            });
        }

        public Response<ResultQuery> RetrieveToolKeyFromToolId(int toolId)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    //toolId is unique, it is a primary database key then if exist we should have a unique result
                    ResultQuery res = null;
                    var tool = unitOfWork.ToolRepository.FindById(toolId);
                    if (tool != null)
                    {
                        res = new ResultQuery()
                        {
                            Id = tool.ToolKey,
                            Name = tool.Name,
                            IsArchived = tool.IsArchived
                        };
                    }
                    return res;
                }
            });
        }

        public Response<int> RetrieveChamberIdFromKeys(int toolKey, int chamberKey)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return GetChamberIdFromKeys(toolKey, chamberKey, unitOfWork);
                }
            });
        }


        //=================================================================
        // Fonctions internes
        //=================================================================

        private int GetChamberIdFromKeys(int toolKey, int chamberKey, UnitOfWorkUnity unitOfWorkUnity)
        {
            int toolid = GetToolIdFromToolKey(toolKey, unitOfWorkUnity);
            var chamber = unitOfWorkUnity.ChamberRepository.CreateQuery(false).FirstOrDefault(x => x.ToolId == toolid);
            if (chamber == null)
                throw new InvalidOperationException($"No Chamber of toolkey(#{toolKey}) has a ChamberKey = {chamberKey}");
            return chamber.Id;
        }

        private int GetToolIdFromToolKey(int toolKey, UnitOfWorkUnity unitOfWorkUnity)
        {
            var tool = unitOfWorkUnity.ToolRepository.CreateQuery(false).FirstOrDefault(x => x.ToolKey == toolKey);
            if (tool == null)
                throw new InvalidOperationException($"No Tool has a ToolKey = {toolKey}");
            return tool.Id;
        }

        #endregion Methods
    }
}
