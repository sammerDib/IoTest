using System;
using System.IO;
using System.Linq;
using System.ServiceModel;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.Enum;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.ResultScanner.Interface;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Composer;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Single)]
    public class RegisterResultService : DataAccesServiceBase, IRegisterResultService, IRegisterResultServer
    {
        private readonly IResultScanner _resultScanner;

        public event ResultAddedEventHandler ResultAdded;

        public event ResultAcqAddedEventHandler ResultAcquistionAdded;

        public event StateUpdateEventHandler StateChanged;

        #region Constructor

        public RegisterResultService(ILogger logger, IResultScanner resultScanner) : base(logger)
        {
            _resultScanner = resultScanner;
        }

        #endregion Constructor

        #region Methods

        //=================================================================
        // Implementation du service
        //=================================================================

        #region Registering Result Item - public

        public Response<OutPreRegister> PreRegisterResult(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, ResultType resultType, byte resultIdx, ResultFilterTag tag, string resultLabelName = null)
        {
            return InvokeDataResponse(() =>
            {
                var preRegister = CreateInPreRegisterResult(sourceToolKey, sourceChamberKey, recipeinfo, autominfo, resultType, resultIdx, tag, resultLabelName);
                return ExecutePreRegisterResult(preRegister);
            });
        }

        public Response<OutPreRegister> PreRegisterResult_SameParent(long parentResultId, DateTime moduleStartRecipeTime, ResultType resultType, byte resultIdx, ResultFilterTag tag, string resultLabelName = null)
        {
            return InvokeDataResponse(() =>
            {
                var preRegister = new InPreRegister(resultType, parentResultId, resultIdx, tag);
                if (moduleStartRecipeTime > DateTime.MinValue)
                    preRegister.DateTimeRun = moduleStartRecipeTime;
                preRegister.LabelName = resultLabelName;
                return ExecutePreRegisterResult(preRegister);
            });
        }

        /// <summary>
        /// Prevent a new Result Item entry in Database (to call before new result addition).
        /// When result transfer is completed call UpdateResultState
        /// Register result InPreRegister object may be modified 
        /// </summary>
        /// <param name="preRegister"></param>
        /// <returns>OutPreRegister </returns>
        public Response<OutPreRegister> PreRegisterResultWithPreRegisterObject(InPreRegister preRegister)
        {
            return InvokeDataResponse(() =>
            {
                return ExecutePreRegisterResult(preRegister);
            });
        }

        /// <summary>
        /// Update Result Item State. (to call at the end of generation /transfer or copy)
        /// Result need to be Preregistered first.
        /// </summary>
        /// <param name="resultItemId"></param>
        /// <param name="resultState"></param>
        /// <returns>bool</returns>
        public Response<bool> UpdateResultState(long resultItemId, ResultState resultState)
        {
            return InvokeDataResponse(() =>
            {
                using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var resultitem = unityOfWork.ResultItemRepository.FindByLongId(resultItemId);
                    if (resultitem == null)
                    {
                        string msg = $"Register Update result state could not found resultitem dbid=<{resultItemId}>";
                        _logger.Error(msg);
                        throw new Exception(msg);
                    }

                    resultitem.State = (int)resultState;
                    unityOfWork.ResultItemRepository.Update(resultitem);
                    unityOfWork.Save();

                    _logger.Debug($"Register Update result state ({resultitem.Id}) <{resultitem.Name}> ");

                    var parentRes = unityOfWork.ResultRepository.CreateQuery(false, x => x.ResultItems).Where(g => g.Id == resultitem.ResultId).FirstOrDefault();
                    bool bProcessOK = true;
                    foreach (var resitem in parentRes.ResultItems)
                    {
                        if (resitem.State == (int)ResultState.NotProcess)
                        {
                            bProcessOK &= false;
                            break;
                        }
                    }
                    int parentstate = (int)(bProcessOK ? ResultState.Ok : ResultState.NotProcess);
                    if (parentRes.State != parentstate)
                    {
                        parentRes.State = parentstate;
                        unityOfWork.ResultRepository.Update(parentRes);
                        unityOfWork.Save();
                    }
                }

                StateChanged?.Invoke(new ResultStateNotificationMessage()
                {
                    ResultID = resultItemId,
                    State = (int)resultState,
                    InternalState = 0,
                    IsAcquisitionResult = false
                });

                if (resultState >= ResultState.Ok)
                    _resultScanner.ResultScanRequest(resultItemId, false);

                return true;
            });
        }

        #endregion //Registering Result Item - public

        #region Registering Acquisition Result Item - public 

        // Acquisition result map (PM)
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        public Response<OutPreRegisterAcquisition> PreRegisterAcquisition(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, string filename, string pathname, ResultType resultType, byte idx = 0, string acqLabelName = null, ResultFilterTag tag = ResultFilterTag.None)
        {
            return InvokeDataResponse(() =>
            {
                var preRegister = CreateInPreRegisterAcquistion(sourceToolKey, sourceChamberKey, recipeinfo, autominfo, filename, pathname, resultType, idx, acqLabelName, tag);
                return ExecutePreRegisterAcqusition(preRegister);
            });
        }

        // Acquisition result map (PM) - with same parent eg. same Parent Root Folder
        //      pre registering result should be done idealy prior to its computing (During Init or before buffer recording)
        public Response<OutPreRegisterAcquisition> PreRegisterAcquisition_SameParent(long parentResultId, DateTime moduleStartRecipeTime, string filename, ResultType resultType, byte idx = 0, string acqLabelName = null, ResultFilterTag tag = ResultFilterTag.None)
        {
            return InvokeDataResponse(() =>
            {
                var preRegister = new InPreRegisterAcquisition(resultType, filename, string.Empty, parentResultId,  idx, tag);
                if (moduleStartRecipeTime > DateTime.MinValue)
                    preRegister.DateTimeRun = moduleStartRecipeTime;
                preRegister.LabelName = acqLabelName;
                return ExecutePreRegisterAcqusition(preRegister);
            });
        }


        /// <summary>
        /// Prevent a new Result ACQUISITION Item entry in Database (to call before new result addition).
        /// When result transfer is completed call UpdateResultAcquisitionState
        /// Register result InPreRegisterAcquisition object may be modified 
        /// </summary>
        /// <param name="preRegister"></param>
        /// <returns>OutPreRegisterAcquisition </returns>
        public Response<OutPreRegisterAcquisition> PreRegisterAcquisitionWithPreRegisterObject(InPreRegisterAcquisition preRegister)
        {
            return InvokeDataResponse(() =>
            {
                return ExecutePreRegisterAcqusition(preRegister);
            });
        }

        /// <summary>
        /// Update Result ACQUISITION Item State. (to call at the end of generation /transfer or copy)
        /// Result need to be PreRegistered first.
        /// </summary>
        /// <param name="resultitemacqId"></param>
        /// <param name="resultacqState"></param>
        /// <returns>bool</returns>
        public Response<bool> UpdateResultAcquisitionState(long resultitemacqId, ResultState resultacqState, string thumbnailExtension = null)
        {
            return InvokeDataResponse(() =>
            {
                string acqfilePath = string.Empty;
                string acqfilePath_ThumbnailSmall = string.Empty;
                using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var resultacqitem = unityOfWork.ResultAcqItemRepository.FindByLongId(resultitemacqId);
                    resultacqitem.State = (int)resultacqState;
                    resultacqitem.InternalState = (int)ResultInternalState.Ok;

                    unityOfWork.ResultAcqItemRepository.Update(resultacqitem);
                    unityOfWork.Save();

                    _logger.Debug($"Register Update result acq state ({resultacqitem.Id}) ");

                    var parentRes = unityOfWork.ResultAcqRepository.CreateQuery(false, x => x.ResultAcqItems).Where(g => g.Id == resultacqitem.ResultAcqId).FirstOrDefault();
                    bool bProcessOK = true;
                    foreach (var resacqitem in parentRes.ResultAcqItems)
                    {
                        if (resacqitem.State == (int)ResultState.NotProcess)
                        {
                            bProcessOK &= false;
                            break;
                        }
                    }
                    int parentstate = (int)(bProcessOK ? ResultState.Ok : ResultState.NotProcess);
                    if (parentRes.State != parentstate)
                    {
                        parentRes.State = parentstate;
                        unityOfWork.ResultAcqRepository.Update(parentRes);
                        unityOfWork.Save();
                    }

                    acqfilePath = InPreRegisterAcqHelper.FullAcqFilePath(parentRes.PathName, resultacqitem.FileName);
                    acqfilePath_ThumbnailSmall = InPreRegisterAcqHelper.FullAcqFilePathThumbnail(parentRes.PathName, resultacqitem.FileName, thumbnailExtension);
                }

                StateChanged?.Invoke(new ResultStateNotificationMessage()
                {
                    ResultID = resultitemacqId,
                    State = (int)resultacqState,
                    InternalState = 0, //(idem ResultState.Ok)
                    IsAcquisitionResult = true
                });

                // Check Path existency
                if (!File.Exists(acqfilePath))
                {
                    throw new Exception($"Acquisition Result file does not exist for res acq item ID={resultitemacqId} => <{acqfilePath}>");
                }
                if (!File.Exists(acqfilePath_ThumbnailSmall))
                {
                    throw new Exception($"Thumbnail Acquisition Result file does not exist for res acq item ID={resultitemacqId} => <{acqfilePath_ThumbnailSmall}>");
                }

                return true;
            });
        }
        #endregion Registering Result Item - public 

        /// <summary>
        /// Retourne true si la connexion à la base des données est disponible.
        /// </summary>
        /// <returns></returns>
        public Response<bool> IsDatabaseConnectionOk()
        {
            bool connectionOk;

            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    unitOfWork.ChamberRepository.CreateQuery();
                    connectionOk = true;
                }
                return connectionOk;
            });
        }

        //=================================================================
        // Fonctions internes.
        //=================================================================

        #region Registering Result Item - private 

        private InPreRegister CreateInPreRegisterResult(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, ResultType resultType, byte resultIdx, ResultFilterTag tag, string resultLabelName)
        {
            if (recipeinfo == null || string.IsNullOrEmpty(recipeinfo.Name))
            {
                string msg = $"Recipe Info Data is Null or not fully provided";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (autominfo == null || autominfo.ProcessedMaterial == null || string.IsNullOrEmpty(autominfo.DFRecipeName))
            {
                string msg = $"Remote Production Info Data is null or not fully provided";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (resultType.GetActorType() != recipeinfo.ActorType )
            {
                string msg = $"Resistering Result type does not match Recipe actor type [{resultType} => {recipeinfo.ActorType}";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            var preRegister = new InPreRegister(resultType);
            preRegister.Idx = resultIdx;
            preRegister.FilterTag = tag;
            preRegister.LabelName = resultLabelName; // could be null if we delegate to register to format name with idx

            // from Remote Production Information
            preRegister.JobName = autominfo.ProcessedMaterial.ProcessJobID;
            preRegister.LotName = autominfo.ProcessedMaterial.LotID;
            preRegister.SlotId = autominfo.ProcessedMaterial.SlotID;
            preRegister.WaferBaseName = autominfo.ProcessedMaterial.WaferBaseName;
            preRegister.TCRecipeName = autominfo.DFRecipeName;
            if (autominfo.ModuleStartRecipeTime > DateTime.MinValue)
                preRegister.DateTimeRun = autominfo.ModuleStartRecipeTime;

            // from Recipe Info and else
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var chamber = unitOfWork.ChamberRepository.CreateQuery(false, x => x.Tool).SingleOrDefault(x => (x.ChamberKey == sourceChamberKey) && (x.Tool.ToolKey == sourceToolKey));
                if (chamber == null)
                {
                    string msg = $"Chamber not found in database (ToolKey = {sourceToolKey}, ChamberKey = {sourceChamberKey}";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }
                preRegister.ChamberId = chamber.Id;
                preRegister.ToolId = chamber.ToolId;

                int recipeversion = recipeinfo.Version;
                if (recipeinfo.Version < 1)
                {
                    // use the lastest recipe version
                    recipeversion = unitOfWork.RecipeRepository.CreateQuery().Where(x => x.KeyForAllVersion == recipeinfo.Key && !x.IsArchived).Select(x => x.Version).DefaultIfEmpty().Max();
                }

                var recipesql = unitOfWork.RecipeRepository.CreateQuery().Where(x => x.KeyForAllVersion == recipeinfo.Key && x.Version == recipeversion).ToList();
                if (recipesql.Count != 1)
                {
                    if (recipesql.Count == 0)
                    {
                        string msg = $"Recipe has not been found in database ({recipeinfo.Name} - v{recipeversion} [{recipeinfo.Key}])";
                        _logger.Error(msg);
                        throw new Exception(msg);
                    }

                    _logger.Warning($"Several Recipes with same GUID & version has been found in database ({recipeinfo.Name} - v{recipeversion} [{recipeinfo.Key}])");
                }
                preRegister.RecipeId = recipesql[0].Id;

                int? recipeStepID = recipesql[0].StepId;
                bool matchRecipeStepid = (recipeinfo.StepId.HasValue ? (recipeStepID == recipeinfo.StepId) : true);
                if (recipeStepID == null || !matchRecipeStepid)
                {
                    string msg = $"Recipe StepId does not match the one in database or is null)";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                var stepsql = unitOfWork.StepRepository.CreateQuery().SingleOrDefault(x => x.Id == recipeStepID);
                if (stepsql == null)
                {
                    string msg = $"Step has not been found in database (stepid={recipeStepID}))";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }
                preRegister.ProductId = stepsql.ProductId; 
            }

            return preRegister;
        }

        private OutPreRegister ExecutePreRegisterResult(InPreRegister preRegister)
        {
            var outputData = new OutPreRegister();

            if (preRegister.ResultType == ResultType.Empty || preRegister.ResultType == ResultType.NotDefined)
            {
                string msg = $"Missing ResultType for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            var actor = PMEnumHelper.GetActorType(preRegister.ResultType);
            // problem: in the case of results coming from the adc such as ADC_KLARF, we lose the PM module source actor in the resulttype (default actor == ADC)
            // ==> solution propose == in the case of adc result, we will set the actor with of the one coming from the chamber ID

            if (preRegister.ParentResultId != -1)
            {
                PreRegisterByParentId(ref preRegister, ref actor, ref outputData);
            }
            else
            {

                UpdateKeyToDB(ref preRegister);

                // we do NOT Know result parent id, we have to find it or create it in case it doesnot exist yet
                CheckInPregisterDataWithoutParentID(preRegister);

                if (actor == ActorType.ADC || actor == ActorType.Unknown)
                {
                    using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    {
                        var sqlchamber = unityOfWork.ChamberRepository.CreateQuery().Where(x => x.Id == preRegister.ChamberId).FirstOrDefault();
                        actor = (ActorType)sqlchamber.ActorType;
                    }
                }

                // Step -- Job
                bool createNewJob = PreRegisterStepJob(ref preRegister, out var regJob);
                if (createNewJob)
                {
                    RegisterResultItemWithNewJob(actor, ref regJob, ref preRegister, ref outputData);
                }
                else
                {
                    RegisterResultItem(actor, regJob, ref preRegister, ref outputData);
                }
            }

            // ----------------------
            // callback on New result
            // ----------------------
            if (ResultAdded != null)
                ResultItemAddedCallback(preRegister, outputData);

            _logger.Debug($"Register Result Item [{(ResultType)preRegister.ResultType}] S{preRegister.SlotId} {outputData.ResultFileName} from Chamber {preRegister.ChamberId} - ({outputData.InternalDBResId}) [{outputData.InternalDBResItemId}] - tool [{preRegister.ToolId}]");
            return outputData;
            
        }

        private void PreRegisterByParentId(ref InPreRegister preRegister, ref ActorType actor, ref OutPreRegister outputData)
        {
            // we know its parent id, it exist so start from there
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var parentResult = unityOfWork.ResultRepository.FindByLongId(preRegister.ParentResultId);
                if (parentResult == null)
                {
                    string msg = $"Pre Registering error unknown parent result id({ preRegister.ParentResultId})";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                preRegister.ChamberId = parentResult.ChamberId;
                preRegister.RecipeId = parentResult.RecipeId;

                var dtoWaferResult = Mapper.Map<Dto.WaferResult>(unityOfWork.WaferResultRepository.CreateQuery(false, x => x.Job).Where(j => j.Id == parentResult.WaferResultId).ToList().DefaultIfEmpty(null).FirstOrDefault());
                preRegister.ProductId = dtoWaferResult.ProductId;
                preRegister.SlotId = dtoWaferResult.SlotId;
                preRegister.WaferBaseName = dtoWaferResult.WaferName;

                var dtoJob = dtoWaferResult.Job;

                preRegister.JobId = dtoJob.Id;
                preRegister.ToolId = dtoJob.ToolId;
                preRegister.JobName = dtoJob.JobName;
                preRegister.LotName = dtoJob.LotName;
                preRegister.TCRecipeName = dtoJob.RecipeName;
                if (string.IsNullOrEmpty(preRegister.LabelName))
                    preRegister.LabelName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);

                bool needCreateNewJob = false;
                bool exist = CheckResultExistency(preRegister);
                if (exist)
                {
                    dtoJob.RunIter++;
                    _logger.Information($"Pre Registering Increment Runiter ({dtoJob.RunIter}) New job to create Jobname={dtoJob.JobName} Lot={dtoJob.LotName} Rcp={dtoJob.RecipeName} ToolId={dtoJob.ToolId}");
                    needCreateNewJob = true;
                }

                if (!needCreateNewJob)
                {
                    // Retrieving the name of the directory for saving the results and the name of the file
                    var resultTpath = GetWaferResultsPath(unityOfWork, dtoJob, preRegister, out int actortypeid);
                    if (actor == ActorType.ADC || actor == ActorType.Unknown)
                        actor = (ActorType)actortypeid;
                    //retrieving output data
                    preRegister.FileName = resultTpath.FileName;

                    var resultItem = new SQL.ResultItem()
                    {
                        ResultId = preRegister.ParentResultId,
                        ResultType = (int)preRegister.ResultType,
                        FileName = preRegister.FileName,
                        Date = preRegister.DateTimeRun,
                        Name = preRegister.LabelName,
                        Idx = preRegister.Idx,
                        State = (int)ResultState.NotProcess,
                        InternalState = (int)ResultState.NotProcess,
                        Tag = (int?)preRegister.FilterTag
                    };

                    unityOfWork.ResultItemRepository.Add(resultItem);
                    unityOfWork.Save();

                    _logger.Verbose($"Pre Register result item by parentid ({preRegister.ParentResultId}) ");

                    outputData.InternalDBResId = resultItem.ResultId;
                    outputData.InternalDBResItemId = resultItem.Id;
                    outputData.ResultFileName = resultTpath.FileName;
                    outputData.ResultPathRoot = resultTpath.FolderPath;
                }
                else
                {
                   var regJob = new Dto.Job()
                    {
                        JobName = preRegister.JobName,
                        LotName = preRegister.LotName,
                        RecipeName = preRegister.TCRecipeName,
                        Date = preRegister.DateTimeRun,
                        ToolId = preRegister.ToolId,
                        RunIter = dtoJob.RunIter
                    };
                    RegisterResultItemWithNewJob(actor, ref regJob, ref preRegister, ref outputData);
                }
            }
        }

        private void UpdateKeyToDB(ref InPreRegister preRegister)
        {
            // if toolid and chamber are already set do not try to update them
            if (preRegister.ToolId != -1 && preRegister.ChamberId != -1)
                return;

            if (preRegister.ToolKey == -1)
            {
                string msg = $"Missing ToolKey for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.ChamberKey == -1)
            {
                string msg = $"Missing ChamberKey for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            int toolkey = preRegister.ToolKey;
            int chamberkey = preRegister.ChamberKey;
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var tool = unityOfWork.ToolRepository.CreateQuery()
                                                     .Where(x => x.ToolKey == toolkey)
                                                     .DefaultIfEmpty(null).First();
                if (tool != null)
                {
                    preRegister.ToolId = tool.Id;
                    var chamber = unityOfWork.ChamberRepository.CreateQuery().Where(x => x.ToolId == tool.Id && x.ChamberKey == chamberkey)
                                          .DefaultIfEmpty(null).First();
                    preRegister.ChamberId = (chamber == null) ? -1 : chamber.Id;    
                }
            }
        }

        private void CheckInPregisterDataWithoutParentID(InPreRegister preRegister)
        {
            if (preRegister.SlotId == -1)
            {
                string msg = $"Missing SlotId for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrEmpty(preRegister.WaferBaseName))
            {
                string msg = $"Missing WaferBaseName for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.ChamberId == -1)
            {
                string msg = $"Missing ChamberId for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.ProductId == -1)
            {
                string msg = $"Missing ProductId for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.RecipeId == -1)
            {
                string msg = $"Missing RecipeId for result registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }
        }

        private bool CheckResultExistency(InPreRegister preRegister)
        {
            bool resultItemAlreadyExist = false;

            String labelItemName = preRegister.LabelName;
            if (string.IsNullOrEmpty(preRegister.LabelName))
                labelItemName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);

            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {

                resultItemAlreadyExist = unityOfWork.ResultItemRepository.CreateQuery(false,
                                                    x => x.Result,
                                                    x => x.Result.Recipe,
                                                    x => x.Result.WaferResult)
                   .Any(x => x.ResultType == (int)preRegister.ResultType
                               && x.Result.WaferResult.JobId == preRegister.JobId
                               && x.Result.WaferResult.SlotId == preRegister.SlotId
                               && x.Result.WaferResult.ProductId == preRegister.ProductId
                               && x.Result.ChamberId == preRegister.ChamberId
                               && x.Result.RecipeId == preRegister.RecipeId
                               && x.Name == labelItemName);

                // // Note Rti : 19/04/2024 : below commented code , new code is above
                // // Need to test new way to check result existency,
                // // below is the old way, where Idx is discriminant and Name is not compare
                //resultItemAlreadyExist = unityOfWork.ResultItemRepository.CreateQuery(false,
                //                                     x => x.Result,
                //                                     x => x.Result.Recipe,
                //                                     x => x.Result.WaferResult)
                //    .Any(x => x.ResultType == (int)preRegister.ResultType
                //                && x.Result.WaferResult.JobId == preRegister.JobId
                //                && x.Result.WaferResult.SlotId == preRegister.SlotId
                //                && x.Result.WaferResult.ProductId == preRegister.ProductId
                //                && x.Result.ChamberId == preRegister.ChamberId
                //                && x.Result.RecipeId == preRegister.RecipeId
                //                && x.Idx == preRegister.Idx);
            }

            return resultItemAlreadyExist;
        }

        private bool PreRegisterStepJob(ref InPreRegister preRegister, out Dto.Job regJob)
        {
            regJob = null;
            if (preRegister.JobId == -1)
            {
                regJob = GetLastJob(preRegister.JobName, preRegister.LotName, preRegister.TCRecipeName, preRegister.ToolId);
            }
            else
            {
                using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var job = unityOfWork.JobRepository.FindById(preRegister.JobId);
                    if (job == null)
                    {
                        string msg = $"Pre Registering error unknown job id ({preRegister.JobId})";
                        _logger.Error(msg);
                        throw new Exception(msg);
                    }

                    regJob = Mapper.Map<Dto.Job>(job);
                }

                preRegister.JobId = regJob.Id;
                preRegister.ToolId = regJob.ToolId;
                preRegister.JobName = regJob.JobName;
                preRegister.LotName = regJob.LotName;
                preRegister.TCRecipeName = regJob.RecipeName;
            }

            bool needCreateNewJob = regJob == null;
            if (regJob != null)
            {
                int nRunIter = regJob.RunIter;
                // check if result already exists other wise increase run iter 
                // si exist => run iter ++ et creation d'un nouveau job avec run iter incrémenté, mise à jour du job id, creation du result et du result item
                preRegister.JobId = regJob.Id;
                bool exist = CheckResultExistency(preRegister);
                if (exist)
                {

                    regJob.RunIter++;
                    regJob.Date = preRegister.DateTimeRun; // Note RTi : voir si cela suffit ou si le preregister contient l'ancien time alors utilisé un DateTime.Now
                    _logger.Information($"Pre Registering Increment Runiter ({regJob.RunIter}) New job to create Jobname={regJob.JobName} Lot={regJob.LotName} Rcp={regJob.RecipeName} ToolId={regJob.ToolId}");

                    needCreateNewJob = true;
                }
            }
            else
            {
                regJob = new Dto.Job()
                {
                    JobName = preRegister.JobName,
                    LotName = preRegister.LotName,
                    RecipeName = preRegister.TCRecipeName,
                    Date = preRegister.DateTimeRun,
                    ToolId = preRegister.ToolId
                };
            }
            return needCreateNewJob;
        }

        private void RegisterResultItemWithNewJob(ActorType actor, ref Dto.Job regJob, ref InPreRegister preRegister, ref OutPreRegister outputData)
        {
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var newdate = DateTime.Now;
                // add job
                var newsqlJob = unityOfWork.JobRepository.Add(Mapper.Map<SQL.Job>(regJob));
                unityOfWork.Save();
                regJob = Mapper.Map<Dto.Job>(newsqlJob);

                preRegister.JobId = regJob.Id;
                outputData.RunIter = regJob.RunIter;
                _logger.Verbose($"Add New Job : Id={regJob.Id} Jobname={regJob.JobName} Lot={regJob.LotName} Rcp={regJob.RecipeName} ToolId={regJob.ToolId} #{regJob.RunIter}");

                // add Wafer ResultItem
                var newWaferResult = new Dto.WaferResult()
                {
                    JobId = regJob.Id,
                    SlotId = preRegister.SlotId,
                    WaferName = preRegister.WaferBaseName,
                    Date = newdate,
                    ProductId = preRegister.ProductId
                };
                var newSqlWaferResult = unityOfWork.WaferResultRepository.Add(Mapper.Map<SQL.WaferResult>(newWaferResult));
                unityOfWork.Save();
                _logger.Verbose($"Add New WaferResult : Id={newSqlWaferResult.Id} Slot={newSqlWaferResult.SlotId} Name={newSqlWaferResult.WaferName} ProdId={newSqlWaferResult.ProductId}");

                // add Result
                var newResult = new Dto.Result()
                {
                    WaferResultId = newSqlWaferResult.Id,
                    ChamberId = preRegister.ChamberId,
                    RecipeId = preRegister.RecipeId,
                    ActorType = (int)actor,
                    Date = newdate,
                    State = (int)ResultState.NotProcess
                };
                var newSqlResult = unityOfWork.ResultRepository.Add(Mapper.Map<SQL.Result>(newResult));
                unityOfWork.Save();
                _logger.Verbose($"Add New Result : Id={newSqlResult.Id} Actor= {(ActorType)newSqlResult.ActorType} ChamberId={newSqlResult.ChamberId} RecipeId={newSqlResult.RecipeId}");
           
                // add result item
                if (string.IsNullOrEmpty(preRegister.LabelName))
                    preRegister.LabelName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);
                // Retrieve of the directory name or saving the results and the name of the file
                var resultTpath = GetWaferResultsPath(unityOfWork, regJob, preRegister, out int moduletypeid);
                ////  output data retrieval
                preRegister.FileName = resultTpath.FileName;
            
                var resultItem = new SQL.ResultItem()
                {
                    ResultId = newSqlResult.Id,
                    ResultType = (int)preRegister.ResultType,
                    FileName = preRegister.FileName,
                    Date = newdate,
                    Name = preRegister.LabelName,
                    Idx = preRegister.Idx,
                    State = (int)ResultState.NotProcess,
                    InternalState = (int)ResultState.NotProcess,
                    Tag = (int?)preRegister.FilterTag
                };

                var newSqlResultItem = unityOfWork.ResultItemRepository.Add(Mapper.Map<SQL.ResultItem>(resultItem));
                unityOfWork.Save();

                _logger.Verbose($"Pre Register result item : Id={newSqlResultItem.Id} ResultType={(ResultType)newSqlResultItem.ResultType} Idx={newSqlResultItem.Idx} - {newSqlResultItem.Name} | {newSqlResultItem.FileName} ");

                preRegister.ParentResultId = resultItem.ResultId;

                outputData.InternalDBResId = resultItem.ResultId;
                outputData.InternalDBResItemId = resultItem.Id;
                outputData.ResultFileName = resultTpath.FileName;
                outputData.ResultPathRoot = resultTpath.FolderPath;
            }
        }

        private void RegisterResultItem(ActorType actor, Dto.Job regJob, ref InPreRegister preRegister, ref OutPreRegister outputData)
        {
            preRegister.JobId = regJob.Id;

            outputData.RunIter = regJob.RunIter;

            int slotId = preRegister.SlotId;
            string waferBaseName = preRegister.WaferBaseName;
            int chamberId = preRegister.ChamberId;

            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var newdate = DateTime.Now;

                //
                // Step -- Wafer Result
                var waferResult = unityOfWork.WaferResultRepository.CreateQuery()
                                                                     .Where(x => x.JobId == regJob.Id && x.SlotId == slotId && x.WaferName == waferBaseName)
                                                                     .DefaultIfEmpty(null).First();
                if (waferResult == null)
                {
                    // no such result exist - so add it
                    waferResult = new SQL.WaferResult()
                    {
                        JobId = regJob.Id,
                        SlotId = preRegister.SlotId,
                        WaferName = preRegister.WaferBaseName,
                        Date = newdate,
                        ProductId = preRegister.ProductId
                    };
                    var newSqlWaferResult = unityOfWork.WaferResultRepository.Add(waferResult);
                    unityOfWork.Save();
                    waferResult = newSqlWaferResult;

                    _logger.Verbose($"Add New WaferResult(2) : Id={newSqlWaferResult.Id} Slot={newSqlWaferResult.SlotId} Name={newSqlWaferResult.WaferName} ProdId={newSqlWaferResult.ProductId}");

                }

                //
                // Step -- ResultItem
                var Result = unityOfWork.ResultRepository.CreateQuery(false, x => x.Chamber)
                                                         .Where(x => x.WaferResultId == waferResult.Id && x.ChamberId == chamberId)
                                                         .DefaultIfEmpty(null).First();
                if (Result == null)
                {
                    // no such result exist - so add it
                    Result = new SQL.Result()
                    {
                        WaferResultId = waferResult.Id,
                        ChamberId = preRegister.ChamberId,
                        RecipeId = preRegister.RecipeId,
                        ActorType = (int)actor,
                        Date = newdate,
                        State = (int)ResultState.NotProcess,
                    };
                    var newSqlResult = unityOfWork.ResultRepository.Add(Result);
                    unityOfWork.Save();
                    Result = newSqlResult;

                    _logger.Verbose($"Add New Result(2) : Id={newSqlResult.Id} Actor= {(ActorType)newSqlResult.ActorType} ChamberId={newSqlResult.ChamberId} RecipeId={newSqlResult.RecipeId}");

                }

                //
                // Step -- ResultItem
                if (string.IsNullOrEmpty(preRegister.LabelName))
                    preRegister.LabelName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);
                // Récuperation du nom du repertoire  pour la sauvegarde des résultats et le nom du fichier
                var resultTpath = GetWaferResultsPath(unityOfWork, regJob, preRegister, out int moduletypeid);
                // récuperation des données de sortie
                preRegister.FileName = resultTpath.FileName;
            
                var resultItem = new SQL.ResultItem()
                {
                    ResultId = Result.Id,
                    ResultType = (int)preRegister.ResultType,
                    FileName = preRegister.FileName,
                    Date = newdate,
                    Name = preRegister.LabelName,
                    Idx = preRegister.Idx,
                    State = (int)ResultState.NotProcess,
                    InternalState = (int)ResultState.NotProcess,
                    Tag = (int?)preRegister.FilterTag
                };

                var newSqlResultItem = unityOfWork.ResultItemRepository.Add(resultItem);
                unityOfWork.Save();

                _logger.Verbose($"Pre Register result item(2) : Id={newSqlResultItem.Id} ResultType={(ResultType)newSqlResultItem.ResultType} Idx={newSqlResultItem.Idx} - {newSqlResultItem.Name} | {newSqlResultItem.FileName} ");

                preRegister.ParentResultId = resultItem.ResultId;

                outputData.InternalDBResId = resultItem.ResultId;
                outputData.InternalDBResItemId = resultItem.Id;
                outputData.ResultFileName = resultTpath.FileName;
                outputData.ResultPathRoot = resultTpath.FolderPath;
            }
        }

        private void ResultItemAddedCallback(InPreRegister preRegister, OutPreRegister outputData)
        {
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {    // we need to have a complete result dto to send to call back with chamber, tool and resultsvalues
                var FullDepthDtoResult = unityOfWork.ResultItemRepository.CreateQuery(false, x => x.Result, 
                                                                                        x => x.Result.Chamber.Tool,
                                                                                        x => x.Result.WaferResult.Job, 
                                                                                        x => x.Result.Recipe,  
                                                                                        x => x.ResultItemValues,
                                                                                        x => x.Result.WaferResult.Product,
                                                                                        x => x.Result.Recipe.Step)
                                                          .Where(r => r.Id == outputData.InternalDBResItemId).FirstOrDefault();
                var waferdata = SpecificMapper.MapSQLtoWaferData(FullDepthDtoResult);
                _resultScanner.InitResultPathFromResult(waferdata.ResultItem);

                var msg = new ResultNotificationMessage()
                {
                    JobID = FullDepthDtoResult.Result.WaferResult.JobId, // should like  preRegister.JobId if correctly updated above
                    WaferResultData = waferdata
                };

                if (msg.ResultItem.Id != FullDepthDtoResult.Id)
                    _logger.Warning($"Register Result Item [{(ResultType)preRegister.ResultType}] S{preRegister.SlotId} : Notified  ITEM Id is different <{msg.ResultItem.Id}> from expected <{FullDepthDtoResult.Id}>)");
                if (msg.JobID != preRegister.JobId)
                    _logger.Warning($"Register Result Item [{(ResultType)preRegister.ResultType}] S{preRegister.SlotId} : Notified JOB Id is different <{msg.ResultItem.Id}> from expected <{FullDepthDtoResult.Id}>)");

                ResultAdded?.Invoke(msg);
            }
        }

        #endregion Registering Result Item  - private 

        #region Registering Result ACQUISITION Item - private 

        private InPreRegisterAcquisition CreateInPreRegisterAcquistion(int sourceToolKey, int sourceChamberKey, RecipeInfo recipeinfo, RemoteProductionInfo autominfo, string filename, string pathname, ResultType resultType, byte idx, string acqLabelName, ResultFilterTag tag)
        {
            if (recipeinfo == null || string.IsNullOrEmpty(recipeinfo.Name))
            {
                string msg = $"Recipe Info Data is Null or not fully provided for acquisition";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (autominfo == null || autominfo.ProcessedMaterial == null || string.IsNullOrEmpty(autominfo.DFRecipeName))
            {
                string msg = $"Remote Production Info Data is null or not fully provided for acquisition";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (resultType.GetActorType() != recipeinfo.ActorType)
            {
                string msg = $"Resistering Result type does not match Recipe actor type [{resultType} => {recipeinfo.ActorType} for acquisition";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            var preRegister = new InPreRegisterAcquisition(resultType, filename, pathname);
            preRegister.Idx = idx;
            preRegister.FilterTag = tag;
            preRegister.LabelName = acqLabelName;

            // from Remote Production Information
            preRegister.JobName = autominfo.ProcessedMaterial.ProcessJobID;
            preRegister.LotName = autominfo.ProcessedMaterial.LotID;
            preRegister.SlotId = autominfo.ProcessedMaterial.SlotID;
            preRegister.WaferBaseName = autominfo.ProcessedMaterial.WaferBaseName;
            preRegister.TCRecipeName = autominfo.DFRecipeName;
            if (autominfo.ModuleStartRecipeTime > DateTime.MinValue)
                preRegister.DateTimeRun = autominfo.ModuleStartRecipeTime;

            // from Recipe Info and else
            using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var chamber = unitOfWork.ChamberRepository.CreateQuery(false, x => x.Tool).SingleOrDefault(x => (x.ChamberKey == sourceChamberKey) && (x.Tool.ToolKey == sourceToolKey));
                if (chamber == null)
                {
                    string msg = $"Chamber not found in database (ToolKey = {sourceToolKey}, ChamberKey = {sourceChamberKey} for acquisition";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }
                preRegister.ChamberId = chamber.Id;
                preRegister.ToolId = chamber.ToolId;

                var recipesql = unitOfWork.RecipeRepository.CreateQuery().Where(x => x.KeyForAllVersion == recipeinfo.Key && x.Version == recipeinfo.Version).ToList();
                if (recipesql.Count != 1)
                {
                    if (recipesql.Count == 0)
                    {
                        string msg = $"Recipe has not been found in database ({recipeinfo.Name} - v{recipeinfo.Version} [{recipeinfo.Key}]) for acquisition";
                        _logger.Error(msg);
                        throw new Exception(msg);
                    }

                    _logger.Warning($"Several Recipes with same GUID & version has been found in database ({recipeinfo.Name} - v{recipeinfo.Version} [{recipeinfo.Key}]) for acquisition");
                }
                preRegister.RecipeId = recipesql[0].Id;

                int? recipeStepID = recipesql[0].StepId;
                bool matchRecipeStepid = (recipeinfo.StepId.HasValue ? (recipeStepID == recipeinfo.StepId) : true);
                if (recipeStepID == null || !matchRecipeStepid)
                {
                    string msg = $"Recipe StepId does not match the one in database or is null) for acquisition";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                var stepsql = unitOfWork.StepRepository.CreateQuery().SingleOrDefault(x => x.Id == recipeStepID);
                if (stepsql == null)
                {
                    string msg = $"Step has not been found in database (stepid={recipeStepID})) for acquisition";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }
                preRegister.ProductId = stepsql.ProductId;
            }

            return preRegister;
        }

        private OutPreRegisterAcquisition ExecutePreRegisterAcqusition(InPreRegisterAcquisition preRegister)
        {
        
            var outputData = new OutPreRegisterAcquisition();

            if (preRegister.ResultType == ResultType.Empty || preRegister.ResultType == ResultType.NotDefined)
            {
                string msg = $"Missing ResultType for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            var actor = PMEnumHelper.GetActorType(preRegister.ResultType);

            if (preRegister.ParentResultId != -1)
            {
                // we know its parent id, it exist so start from there
                PreRegisterAcquisitionByParentId(ref preRegister, ref actor, ref outputData);
            }
            else
            {
                // we do NOT Know result parent id, we have to find it or create it in case it doesnot exist yet
                CheckInPregisterAcquisitionDataWithoutParentID(preRegister);

                if (actor == ActorType.ADC || actor == ActorType.Unknown)
                {
                    using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    {
                        var sqlchamber = unityOfWork.ChamberRepository.CreateQuery().Where(x => x.Id == preRegister.ChamberId).FirstOrDefault();
                        actor = (ActorType)sqlchamber.ActorType;
                    }
                }
                // Step -- Job
                bool createNewJob = PreRegisterAcquisitionStepJob(ref preRegister, out var regJob);
                if (createNewJob)
                {
                    RegisterResultAcquisitionItemWithNewJob(actor, ref regJob, ref preRegister, ref outputData);
                }
                else
                {
                    RegisterResultAcquisitionItem(actor, regJob, ref preRegister, ref outputData);
                }
            }

            // ----------------------------------
            // callback on New result Acquisition
            // ----------------------------------
            if (ResultAcquistionAdded != null)
                ResultAcquisitionItemAddedCallback(preRegister, outputData);

            _logger.Debug($"Register Result Acq Item [{(ResultType)preRegister.ResultType}] S{preRegister.SlotId} {preRegister.FileName} from Chamber {preRegister.ChamberId} - ({outputData.InternalDBResId}) [{outputData.InternalDBResItemId}] - tool [{preRegister.ToolId}]");
            return outputData;
            
        }


        private void PreRegisterAcquisitionByParentId(ref InPreRegisterAcquisition preRegister, ref ActorType actor, ref OutPreRegisterAcquisition outputData)
        {
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var parentResult = unityOfWork.ResultAcqRepository.FindByLongId(preRegister.ParentResultId);
                if (parentResult == null)
                {
                    string msg = $"Pre Registering ACQ error unknown parent result id ({preRegister.ParentResultId})";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                preRegister.ChamberId = parentResult.ChamberId;
                preRegister.RecipeId = parentResult.RecipeId;

                var dtoWaferResult = Mapper.Map<Dto.WaferResult>(unityOfWork.WaferResultRepository.CreateQuery(false, x => x.Job).Where(j => j.Id == parentResult.WaferResultId).ToList().DefaultIfEmpty(null).FirstOrDefault());
                preRegister.ProductId = dtoWaferResult.ProductId;
                preRegister.SlotId = dtoWaferResult.SlotId;
                preRegister.WaferBaseName = dtoWaferResult.WaferName;

                var dtoJob = dtoWaferResult.Job;

                preRegister.JobId = dtoJob.Id;
                preRegister.ToolId = dtoJob.ToolId;
                preRegister.JobName = dtoJob.JobName;
                preRegister.LotName = dtoJob.LotName;
                preRegister.TCRecipeName = dtoJob.RecipeName;

                // update pre registering info  with valid root path
                preRegister.PathName = parentResult.PathName;

                if (string.IsNullOrEmpty(preRegister.LabelName))
                    preRegister.LabelName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);

                var resultAcqItem = new SQL.ResultAcqItem()
                {
                    ResultAcqId = preRegister.ParentResultId,
                    ResultType = (int)preRegister.ResultType,
                    FileName = preRegister.FileName,
                    Date = preRegister.DateTimeRun,
                    Name = preRegister.LabelName,
                    Idx = preRegister.Idx,
                    State = (int)ResultState.NotProcess,
                    InternalState = (int)ResultInternalState.NotProcess,
                    Tag = (int?) preRegister.FilterTag
                };

                unityOfWork.ResultAcqItemRepository.Add(resultAcqItem);
                unityOfWork.Save();

                _logger.Verbose($"Pre Register result Acq item by parentid ({preRegister.ParentResultId}) ");

                outputData.InternalDBResId = resultAcqItem.ResultAcqId;
                outputData.InternalDBResItemId = resultAcqItem.Id;
                outputData.Inputs = preRegister;
            }
        }

        private void CheckInPregisterAcquisitionDataWithoutParentID(InPreRegisterAcquisition preRegister)
        {
            if (preRegister.SlotId == -1)
            {
                string msg = $"Missing SlotId for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrEmpty(preRegister.WaferBaseName))
            {
                string msg = $"Missing WaferBaseName for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.ChamberId == -1)
            {
                string msg = $"Missing ChamberId for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.ProductId == -1)
            {
                string msg = $"Missing ProductId for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (preRegister.RecipeId == -1)
            {
                string msg = $"Missing RecipeId for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrEmpty(preRegister.PathName) || string.IsNullOrWhiteSpace(preRegister.PathName))
            {
                string msg = $"Missing PathName for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }

            if (string.IsNullOrEmpty(preRegister.FileName) || string.IsNullOrWhiteSpace(preRegister.FileName))
            {
                string msg = $"Missing FileName for result ACQ registering";
                _logger.Error(msg);
                throw new Exception(msg);
            }
        }

        private bool PreRegisterAcquisitionStepJob(ref InPreRegisterAcquisition preRegister, out Dto.Job regJob)
        {
            regJob = null;
            if (preRegister.JobId == -1)
            {
                regJob = GetLastJob(preRegister.JobName, preRegister.LotName, preRegister.TCRecipeName, preRegister.ToolId);
            }
            else
            {
                using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var job = unityOfWork.JobRepository.FindById(preRegister.JobId);
                    if (job == null)
                    {
                        string msg = $"Pre Registering Acq error unknown job id ({preRegister.JobId})";
                        _logger.Error(msg);
                        throw new Exception(msg);
                    }

                    regJob = Mapper.Map<Dto.Job>(job);
                }

                preRegister.JobId = regJob.Id;
                preRegister.ToolId = regJob.ToolId;
                preRegister.JobName = regJob.JobName;
                preRegister.LotName = regJob.LotName;
                preRegister.TCRecipeName = regJob.RecipeName;
            }

            bool createNewJob = (regJob == null);
            if (regJob != null)
            {
                int nRunIter = regJob.RunIter;
            }
            else
            {
                regJob = new Dto.Job()
                {
                    JobName = preRegister.JobName,
                    LotName = preRegister.LotName,
                    RecipeName = preRegister.TCRecipeName,
                    Date = preRegister.DateTimeRun,
                    ToolId = preRegister.ToolId
                };
            }
            return createNewJob;
        }

        private void RegisterResultAcquisitionItemWithNewJob(ActorType actor, ref Dto.Job regJob, ref InPreRegisterAcquisition preRegister, ref OutPreRegisterAcquisition outputData)
        {
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var newdate = DateTime.Now;
                // add job
                var newsqlJob = unityOfWork.JobRepository.Add(Mapper.Map<SQL.Job>(regJob));
                unityOfWork.Save();
                regJob = Mapper.Map<Dto.Job>(newsqlJob);

                preRegister.JobId = regJob.Id;
                _logger.Verbose($"Acq Add New Job : Id={regJob.Id} Jobname={regJob.JobName} Lot={regJob.LotName} Rcp={regJob.RecipeName} ToolId={regJob.ToolId} #{regJob.RunIter}");

                // add Wafer Result
                var newWaferResult = new Dto.WaferResult()
                {
                    JobId = regJob.Id,
                    SlotId = preRegister.SlotId,
                    WaferName = preRegister.WaferBaseName,
                    Date = newdate,
                    ProductId = preRegister.ProductId
                };
                var newSqlWaferResult = unityOfWork.WaferResultRepository.Add(Mapper.Map<SQL.WaferResult>(newWaferResult));
                unityOfWork.Save();
                _logger.Verbose($"Add New WaferResult (Acq) : Id={newSqlWaferResult.Id} Slot={newSqlWaferResult.SlotId} Name={newSqlWaferResult.WaferName} ProdId={newSqlWaferResult.ProductId}");

                // add Result Acq
                var newResultAcq = new Dto.ResultAcq()
                {
                    WaferResultId = newSqlWaferResult.Id,
                    ChamberId = preRegister.ChamberId,
                    RecipeId = preRegister.RecipeId,
                    ActorType = (int)actor,
                    Date = newdate,
                    PathName = preRegister.PathName,
                    State = (int)ResultState.NotProcess
                };
                var newSqlResultAcq = unityOfWork.ResultAcqRepository.Add(Mapper.Map<SQL.ResultAcq>(newResultAcq));
                unityOfWork.Save();
                _logger.Verbose($"Add New Acq Result : Id={newSqlResultAcq.Id} Actor= {(ActorType)newSqlResultAcq.ActorType} ChamberId={newSqlResultAcq.ChamberId} RecipeId={newSqlResultAcq.RecipeId}");

                // add result Acq item
                if (string.IsNullOrEmpty(preRegister.LabelName))
                    preRegister.LabelName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);

                var resultAcqItem = new SQL.ResultAcqItem()
                {
                    ResultAcqId = newSqlResultAcq.Id,
                    ResultType = (int)preRegister.ResultType,
                    FileName = preRegister.FileName,
                    Date = newdate,
                    Name = preRegister.LabelName,
                    Idx = preRegister.Idx,
                    State = (int)ResultState.NotProcess,
                    InternalState = (int)ResultInternalState.NotProcess,
                    Tag = (int?)preRegister.FilterTag
                };

                var newSqlResultAcqItem = unityOfWork.ResultAcqItemRepository.Add(Mapper.Map<SQL.ResultAcqItem>(resultAcqItem));
                unityOfWork.Save();

                _logger.Verbose($"Acq Pre Register result item : Id={newSqlResultAcqItem.Id} ResultType={(ResultType)newSqlResultAcqItem.ResultType} Idx={newSqlResultAcqItem.Idx} - {newSqlResultAcqItem.Name} | {newSqlResultAcqItem.FileName} ");

                preRegister.ParentResultId = resultAcqItem.ResultAcqId;

                outputData.InternalDBResId = resultAcqItem.ResultAcqId;
                outputData.InternalDBResItemId = resultAcqItem.Id;
                outputData.Inputs = preRegister;
            }
        }

        private void RegisterResultAcquisitionItem(ActorType actor, Dto.Job regJob, ref InPreRegisterAcquisition preRegister, ref OutPreRegisterAcquisition outputData)
        {
            preRegister.JobId = regJob.Id;

            int slotId = preRegister.SlotId;
            string waferBaseName = preRegister.WaferBaseName;
            int chamberId = preRegister.ChamberId;

            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var newdate = DateTime.Now;

                //
                // Step -- Wafer Result
                var waferResult = unityOfWork.WaferResultRepository.CreateQuery()
                                                                     .Where(x => x.JobId == regJob.Id && x.SlotId == slotId && x.WaferName == waferBaseName)
                                                                     .DefaultIfEmpty(null).First();
                if (waferResult == null)
                {
                    // no such result exist - so add it
                    waferResult = new SQL.WaferResult()
                    {
                        JobId = regJob.Id,
                        SlotId = preRegister.SlotId,
                        WaferName = preRegister.WaferBaseName,
                        Date = newdate,
                        ProductId = preRegister.ProductId
                    };
                    var newSqlWaferResult = unityOfWork.WaferResultRepository.Add(waferResult);
                    unityOfWork.Save();
                    waferResult = newSqlWaferResult;

                    _logger.Verbose($"Add New WaferResult(2) (Acq) : Id={newSqlWaferResult.Id} Slot={newSqlWaferResult.SlotId} Name={newSqlWaferResult.WaferName} ProdId={newSqlWaferResult.ProductId}");

                }

                //
                // Step -- ResultAcq
                var ResultAcq = unityOfWork.ResultAcqRepository.CreateQuery(false, x => x.Chamber)
                                                         .Where(x => x.WaferResultId == waferResult.Id && x.ChamberId == chamberId)
                                                         .DefaultIfEmpty(null).First();
                if (ResultAcq == null)
                {
                    // no such result exist - so add it
                    ResultAcq = new SQL.ResultAcq()
                    {
                        WaferResultId = waferResult.Id,
                        ChamberId = preRegister.ChamberId,
                        RecipeId = preRegister.RecipeId,
                        ActorType = (int)actor,
                        Date = newdate,
                        PathName = preRegister.PathName,
                        State = (int)ResultState.NotProcess,
                    };
                    var newSqlResultAcq = unityOfWork.ResultAcqRepository.Add(ResultAcq);
                    unityOfWork.Save();
                    ResultAcq = newSqlResultAcq;

                    _logger.Verbose($"Add New Result(2) : Id={newSqlResultAcq.Id} Actor= {(ActorType)newSqlResultAcq.ActorType} ChamberId={newSqlResultAcq.ChamberId} RecipeId={newSqlResultAcq.RecipeId}");

                }

                // update pre registering info  with valid root path
                if (System.IO.Path.GetFullPath(preRegister.PathName) != System.IO.Path.GetFullPath(ResultAcq.PathName))
                {
                    string msg = $"PathName in Preregister data differs from the Database AcqResult PathName\n" +
                                 $" -- Preregister = <{preRegister.PathName}>\n" +
                                 $" -- ResultAcd({ResultAcq.Id}) = <{ResultAcq.PathName}>";
                    _logger.Error(msg);
                    throw new Exception(msg);
                }

                preRegister.PathName = ResultAcq.PathName;

                //
                // Step -- ResultAcqItem

                if (string.IsNullOrEmpty(preRegister.LabelName))
                    preRegister.LabelName = preRegister.ResultType.DefaultLabelName(preRegister.Idx);

                var resultAcqItem = new SQL.ResultAcqItem()
                {
                    ResultAcqId = ResultAcq.Id,
                    ResultType = (int)preRegister.ResultType,
                    FileName = preRegister.FileName,
                    Date = newdate,
                    Name = preRegister.LabelName,
                    Idx = preRegister.Idx,
                    State = (int)ResultState.NotProcess,
                    InternalState = (int)ResultInternalState.NotProcess,
                    Tag = (int?)preRegister.FilterTag
                };

                var newSqlResultAcqItem = unityOfWork.ResultAcqItemRepository.Add(resultAcqItem);
                unityOfWork.Save();

                _logger.Verbose($"Pre Register result Acq item(2) : Id={newSqlResultAcqItem.Id} ResultType={(ResultType)newSqlResultAcqItem.ResultType} Idx={newSqlResultAcqItem.Idx} - {newSqlResultAcqItem.Name} | {newSqlResultAcqItem.FileName} ");

                preRegister.ParentResultId = newSqlResultAcqItem.ResultAcqId;

                outputData.InternalDBResId = newSqlResultAcqItem.ResultAcqId;
                outputData.InternalDBResItemId = newSqlResultAcqItem.Id;
                outputData.Inputs = preRegister;
            }
        }

        private void ResultAcquisitionItemAddedCallback(InPreRegisterAcquisition preRegister, OutPreRegisterAcquisition outputData)
        {
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {    // we need to have a complete result dto to send to call back with chamber, tool and resultsvalues
                var FullDepthDtoResult = unityOfWork.ResultAcqItemRepository.CreateQuery(false, x => x.ResultAcq, x => x.ResultAcq.Chamber.Tool, x => x.ResultAcq.WaferResult.Job, x => x.ResultAcq.Recipe)
                                                          .Where(r => r.Id == outputData.InternalDBResItemId).FirstOrDefault();

                var msg = new ResultAcqNotificationMessage()
                {
                    JobID = FullDepthDtoResult.ResultAcq.WaferResult.JobId, // should like  preRegister.JobId if correctly updated above
                    ResultAcqItem = Mapper.Map<Dto.ResultAcqItem>(FullDepthDtoResult)
                };

                if (msg.ResultAcqItem.Id != FullDepthDtoResult.Id)
                    _logger.Warning($"Register Result Acq Item [{(ResultType)preRegister.ResultType}] S{preRegister.SlotId} : Notified ITEM Id is different <{msg.ResultAcqItem.Id}> from expected <{FullDepthDtoResult.Id}>)");
                if (msg.JobID != preRegister.JobId)
                    _logger.Warning($"Register Result Acq Item [{(ResultType)preRegister.ResultType}] S{preRegister.SlotId} : Notified JOB Id is different <{msg.ResultAcqItem.Id}> from expected <{FullDepthDtoResult.Id}>)");

                ResultAcquistionAdded?.Invoke(msg);
            }
        }
        #endregion Registering Result ACQUISITION Item - private 

        /// <summary>
        /// return directory parent path in which results will be stored, also returned output filename to applied to result prior to preregistering
        /// </summary>
        /// <returns></returns>
        private WaferResultPath GetWaferResultsPath(UnitOfWorkUnity unityOfWork, Dto.Job job, InPreRegister preRegister, out int actorType)
        {
            actorType = -1;
            var resultPath = new WaferResultPath();

            var chamber = unityOfWork.ChamberRepository.CreateQuery(false, x => x.Tool).Where(x => x.Id == preRegister.ChamberId).FirstOrDefault();
            actorType = chamber.ActorType;

            string productname = unityOfWork.ProductRepository.FindById(preRegister.ProductId)?.Name;
            SQL.Recipe recipe = unityOfWork.RecipeRepository.FindById(preRegister.RecipeId);
            string stepname = "UnknownStep";
            if (recipe != null)
                stepname = unityOfWork.StepRepository.FindById(recipe.StepId.Value)?.Name;


            var prm = new ResultPathParams
            {
                ToolName = chamber.Tool.Name,
                ToolId = job.ToolId,//chamber.Tool.Id;
                ToolKey = chamber.Tool.ToolKey,
                ChamberName = chamber.Name,
                ChamberId = chamber.Id,
                ChamberKey =chamber.ChamberKey,
                JobName = job.JobName,
                JobId = job.Id,
                LotName = job.LotName,
                RecipeName = job.RecipeName,
                StartProcessDate = job.Date,
                Slot = preRegister.SlotId,
                RunIter = job.RunIter,
                WaferName = preRegister.WaferBaseName,
                ResultType = preRegister.ResultType,
                Index = preRegister.Idx,
                Label = preRegister.LabelName,
                ProductName =  productname,
                StepName = stepname,
            };

            resultPath.FolderPath = _resultScanner.BuildResultDirectoryName(prm);
            resultPath.FileName = _resultScanner.BuildResultFileName(prm);

            return resultPath;
        }

        /// <summary>
        /// return dto.job with the greatest runTter num or null in case job does not exist 
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        private Dto.Job GetLastJob(string jobName, string lotName, string recipeName, int tooId)
        {
            using (var unityOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
            {
                var job = unityOfWork.JobRepository.CreateQuery()
                                                   .Where(x => x.JobName == jobName && x.LotName == lotName && x.RecipeName == recipeName && x.ToolId == tooId)
                                                   .OrderByDescending(x => x.RunIter).ToList().DefaultIfEmpty(null).FirstOrDefault();
                return Mapper.Map<Dto.Job>(job);
            }
        }


        #endregion Methods
    }
}
