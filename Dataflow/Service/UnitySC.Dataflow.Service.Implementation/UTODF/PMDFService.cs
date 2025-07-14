using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using IniFile;
using UnitySC.ADCAS300Like.Common;
using UnitySC.DataAccess.Dto;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.PM.Operations.Interface.UTOOperations;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Service;

using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.dataflow.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PMDFService : DuplexServiceBase<IPMDFServiceCB>, IPMDFService, IPMDFServiceCB
    {
        private IPMDFOperations _pmDfOperations;
        public IPMDFOperations PMDFOperations { get => _pmDfOperations; set => _pmDfOperations = value; }
        private IUTODFServiceCB _utodfServiceCB;
        private IAlarmOperations _alarmOperations;
        
        public PMDFService(ILogger logger) : base(logger, ExceptionType.DataflowException)
        {
        }

        public override void Init()
        {
            base.Init();
   

            PMDFOperations = ClassLocator.Default.GetInstance<IPMDFOperations>();
            PMDFOperations.Init();
            _alarmOperations = ClassLocator.Default.GetInstance<IAlarmOperations>();
            _utodfServiceCB = ClassLocator.Default.GetInstance<IUTODFServiceCB>();
        }

        public Response<VoidResult> UnSubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Unsubscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Unsubscribe to PM change"));
            });
        }

        public Response<VoidResult> SubscribeToChanges()
        {
            return InvokeVoidResponse(messageContainer =>
            {
                base.Subscribe();
                messageContainer.Add(new Message(MessageLevel.Information, "Subscribe to PM change"));
            });
        }

        public Response<VoidResult> StartRecipeRequest(Identity identity, Material wafer)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    _logger.Information("StartRecipeRequest from PM");
                    var canStartPMRecipe = PMDFOperations.CanStartPMRecipe(identity, wafer);
                    var recipeKey = PMDFOperations.RetrieveTheAssociatedPMRecipeKey(identity, wafer);
                    var dfRecipeInfo = PMDFOperations.GetDataflowRecipeInfo(identity, wafer);
                    if (canStartPMRecipe)
                    {
                        _logger.Information("Notify the PM to execute the recipe");
                        Task.Run(() => StartRecipeExecution(identity, recipeKey, dfRecipeInfo, wafer));
                        messageContainer.Add(new Message(MessageLevel.Information, $"StartRecipeExecution Notify the PM to execute the recipe from PM/ChamberID : {identity.ActorType}/{identity.ChamberID}/{identity.ToolKey}"));
                    }
                    else
                    {
                        _logger.Warning($"Unable to run Recipe from PM/ChamberID : {identity.ActorType}/{identity.ChamberID} with wafer = {wafer.ToString()}");
                        Task.Run(() => AbortRecipeExecution(identity));
                    }
                }
                else
                {
                    string msgErr = "PMDFOperations is null in StartRecipeRequest from PM";
                    messageContainer.Add(new Message(MessageLevel.Error, msgErr));
                    _logger.Error(msgErr);
                }
            });
        }

        public Response<VoidResult> RecipeExecutionComplete(Identity identity, Material wafer, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    PMDFOperations.RecipeExecutionComplete(identity, wafer, recipeKey, results, status);
                    messageContainer.Add(new Message(MessageLevel.Information, $"RecipeExecutionComplete from PM/ChamberID: {identity.ActorType}/{identity.ChamberID}/{identity.ToolKey}"));                    
                    var dataflowRecipeInfo = PMDFOperations.GetDataflowRecipeInfo(identity, wafer);
                    _logger.Debug("PMRecipeProcessComplete for DF = " + dataflowRecipeInfo?.Name + " and the wafer = " + wafer.ToString());
                    Task.Run(() => _utodfServiceCB.PMRecipeProcessComplete(identity.ActorType, dataflowRecipeInfo, wafer, status));
                    messageContainer.Add(new Message(MessageLevel.Information, $"PMRecipeProcessComplete from PM/ChamberID : {identity.ActorType}/{identity.ChamberID}/{identity.ToolKey}"));
                }
                else
                {
                    string msgErr = $"PMDFOperations is null in RecipeExecutionComplete";  
                    messageContainer.Add(new Message(MessageLevel.Error, msgErr));
                    _logger.Error(msgErr); 
                }
            });
        }
        public Response<VoidResult> RecipeAcquisitionComplete(Identity identity, Material material, Guid? recipeKey, string results, RecipeTerminationState status)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    var dataflowRecipeInfo = PMDFOperations.GetDataflowRecipeInfo(identity, material);
                    _logger.Debug("PMRecipeAcquisitionComplete for DF = " + dataflowRecipeInfo?.Name + " and the wafer = " + material.ToString());
                    Task.Run(() => _utodfServiceCB.PMRecipeAcquisitionComplete(identity.ActorType, dataflowRecipeInfo, material, status));
                    messageContainer.Add(new Message(MessageLevel.Information, $"PMRecipeAcquisitionComplete from PM/ChamberID : {identity.ActorType}/{identity.ChamberID}/{identity.ToolKey}"));
                }
                else
                {
                    string msgErr = $"PMDFOperations is null in PMRecipeAcquisitionComplete";
                    messageContainer.Add(new Message(MessageLevel.Error, msgErr));
                    _logger.Error(msgErr);
                }
            });
        }


        public Response<VoidResult> RecipeStarted(Identity identity, Material wafer)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    _logger.Information("StartRecipe from PM");
                    PMDFOperations.RecipeStarted(identity, wafer);
                    messageContainer.Add(new Message(MessageLevel.Information, $"RecipeStarted from PM/ChamberID : {identity.ActorType}/{identity.ChamberID}/{identity.ToolKey}"));
                    var dataflowRecipeInfo = PMDFOperations.GetDataflowRecipeInfo(identity, wafer);
                    _logger.Debug("PMRecipeProcessStarted for DF = " + dataflowRecipeInfo?.Name + " and the wafer = " + wafer.ToString());
                    Task.Run(() => _utodfServiceCB.PMRecipeProcessStarted(identity.ActorType, dataflowRecipeInfo, wafer));
                    messageContainer.Add(new Message(MessageLevel.Information, $"Notify the DF that the recipe has been started: {identity.ActorType}/{identity.ChamberID}/{identity.ToolKey}"));
                }
                else
                {
                    string msgErr = $"PMDFOperations is null in RecipeStarted";
                    messageContainer.Add(new Message(MessageLevel.Error, msgErr));
                    _logger.Error(msgErr);
                }
            });
        }

        public Response<VoidResult> NotifyError(Identity identity, Message errorMessage)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    string msgLog = $"Notify error from PM/ChamberID:{identity.ChamberID} error:{errorMessage.Error} msg: {errorMessage.UserContent}";
                    _logger.Information(msgLog);
                    messageContainer.Add(new Message(MessageLevel.Information, msgLog));
                    _alarmOperations.SetAlarm(identity, errorMessage.Error);
                }
                else
                {
                    string msgErr = "PMDFOperations is null in NotifyError method";
                    messageContainer.Add(new Message(MessageLevel.Error, msgErr));
                    _logger.Error(msgErr);
                }
            });
        }

        public Response<VoidResult> NotifyDataCollectionChanged(Identity identity, ModuleDataCollection pmDataCollection)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    string msgLog = $"Notify DataCollectionChanged from PM/ChamberID:{identity.ChamberID}";
                    _logger.Information(msgLog);
                    messageContainer.Add(new Message(MessageLevel.Information, msgLog));
                    Task.Run(() => { PMDFOperations.NotifyDataCollectionChanged(identity, pmDataCollection); });
                }
                else
                {
                    _logger.Error("PMDFOperations is null in NotifyDataCollectionChanged method");
                }
            });
        }

        

        public Response<VoidResult> NotifyFDCCollectionChanged(Identity identity, List<FDCData> fdcsDataCollection)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations != null)
                {
                    string msgLog = $"Notify NotifyFDCCollectionChanged from Module actor={identity.ActorType.ToString()} ChamberID={identity.ChamberID}";
                    _logger.Verbose(msgLog);
                    messageContainer.Add(new Message(MessageLevel.Information, msgLog));
                    if (fdcsDataCollection != null)
                        Task.Run(() => _utodfServiceCB.NotifyFDCCollectionChanged(fdcsDataCollection));
                    else
                        _logger.Error("FDCData parameter is null in NotifyFDCCollectionChanged method");
                }
                else
                {
                    _logger.Error("PMDFOperations is null in NotifyFDCCollectionChanged method");
                }
            });
        }

        public Response<VoidResult> SendAda(Identity identity, Material wafer, string adaContent, String adaFullPathFileName)
        {
            return InvokeVoidResponse(messageContainer =>
            {
                AutoSubscribe();
                if (PMDFOperations is null)
                {
                    _logger.Error("PMDFOperations is null in NotifyFDCCollectionChanged method");
                    return;
                }
                
                _logger.Information($"Ada reception from PM : SendAda called by PM {identity.ActorType} chamber {identity.ChamberID}");

                var adcsConfigs = ClassLocator.Default.GetInstance<ADCsConfigs>();
                var adaFile = Ini.Load(adaContent);
                var infoWaferSection = adaFile["INFO WAFER"];
                
                DataflowActorRecipe adcRecipe;
                string adcInputPathConfig = String.Empty;

                switch (identity.ActorType)
                {
                    case ActorType.DEMETER:
                        var waferSide = GetSideFromDemeterAda(infoWaferSection);
                        switch (waferSide)
                        {
                            case Side.Front:
                                adcInputPathConfig =adcsConfigs.ADCItemsConfig.FirstOrDefault(adc => adc.ADCType == ADCType.atPSD_FRONTSIDE)?.AdaInputPath ?? String.Empty;
                                break;
                            case Side.Back:
                                adcInputPathConfig = adcsConfigs.ADCItemsConfig.FirstOrDefault(adc => adc.ADCType == ADCType.atPSD_BACKSIDE)?.AdaInputPath ?? String.Empty;
                                break;
                            case Side.Unknown:
                                break;
                            case null:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        adcRecipe = GetRecipeFromDemeterAda(identity, wafer, infoWaferSection);
                        break;
                    default:
                        adcInputPathConfig = adcsConfigs.ADCItemsConfig.FirstOrDefault(adc => adc.ADCType == ADCType.atPSD_FRONTSIDE)?.AdaInputPath ?? String.Empty;
                        adcRecipe = PMDFOperations.GetADCRecipeForSide(identity, wafer, Side.Unknown);
                        break;
                }
                if (adcRecipe is null)
                {
                    _logger.Error("Cannot find associated ADC recipe. No ADA file generated");
                    return;
                }
                
                // Add the information to the Ada
                var recipeNameProperty = new Property("ADCRecipeName", adcRecipe.Name);
                infoWaferSection.Add(recipeNameProperty);
                var recipeKeyProperty = new Property("ADCRecipeGUID", adcRecipe.KeyForAllVersion.ToString());
                infoWaferSection.Add(recipeKeyProperty);

                var adaPathFromPM = new PathString(adaFullPathFileName);
                var adaDestPath = new PathString(adcInputPathConfig);
                if (adaDestPath.ToString().IsNullOrEmpty() || adaDestPath.Directory.ToString().IsNullOrEmpty())
                {
                    _logger.Error("Cannot find a valid directory for ADA files in ADC configuration. No ADA file generated");
                    return;
                }
                var adaDestinationFullFilename = Path.Combine(adaDestPath, adaPathFromPM.Filename);               
                if (!Directory.Exists(adaDestPath))
                    Directory.CreateDirectory(adaDestPath);
                adaFile.SaveTo(adaDestinationFullFilename);
                _logger.Information($"ADA file generated. fileName= {adaDestinationFullFilename}");
               
            });
        }

        private PathString GetAdaFileNameAndPathFromConfigAndRecipe(Section infoWaferSection, ADCsConfigs adcsConfigs, DataflowActorRecipe adcRecipe)
        {
            var waferSide = GetSideFromDemeterAda(infoWaferSection).Value;
            var adcType = waferSide == Side.Front ? ADCType.atPSD_FRONTSIDE : ADCType.atPSD_BACKSIDE;
            var adcConfig = adcsConfigs.ADCItemsConfig.FirstOrDefault(a => a.ADCType == adcType);
            if (adcConfig != null)
            {
                return new PathString(adcConfig.AdaInputPath) / $"{adcRecipe.Name}_{waferSide}.ada";
            }
            return null;
        }

        private DataflowActorRecipe GetRecipeFromDemeterAda(Identity identity, Material wafer,
            Section infoWaferSection)
        {
            var waferSide = GetSideFromDemeterAda(infoWaferSection);
                    
            if (!waferSide.HasValue || waferSide == Side.Unknown)
            {
                _logger.Error("Cannot handle Unknown side");
                return null;
            }

            return PMDFOperations.GetADCRecipeForSide(identity, wafer, waferSide.Value);
        }

        private Side? GetSideFromDemeterAda(Section infoWaferSection)
        {
            if (!Enum.TryParse(infoWaferSection["Side"], out Side waferSide))
            {
                _logger.Error("Side information missing from ADA file, cannot proceed");
                return null;
            }

            return waferSide;
        }


        #region Callback

        public void AreYouThere()
        {
            InvokeCallback(i => i.AreYouThere());
        }

        public bool AskAreYouThere()
        {
            bool result = false;
            if ((GetNbClientsConnected() > 0))
            {
                AreYouThere();
                result = true;
            }
            return result;
        }

        public void StartRecipeExecution(Identity identity, Guid? recipeKey, DataflowRecipeInfo dfRecipeInfo, Material wafer)
        {
            InvokeCallback(i => i.StartRecipeExecution(identity, recipeKey, dfRecipeInfo, wafer));
        }

        public void AbortRecipeExecution(Identity identity)
        {
            InvokeCallback(i => i.AbortRecipeExecution(identity));
        }

        private void AutoSubscribe()
        {
            if (GetNbClientsConnected() <= 0)
                base.Subscribe();
        }


        public void OnErrorAcknowledged(Identity identity, ErrorID error)
        {
            InvokeCallback(i => i.OnErrorAcknowledged(identity, error));
        }

        public void OnErrorReset(Identity identity, ErrorID error)
        {
            InvokeCallback(i => i.OnErrorReset(identity, error));
        }

        public void SetPMInCriticalErrorState(Identity identity, ErrorID error)
        {
            InvokeCallback(i => i.SetPMInCriticalErrorState(identity, error));
        }

        public void RequestAllFDCsUpdate()
        {
            InvokeCallback(i => i.RequestAllFDCsUpdate());
        }

    

        #endregion Callback
    }
}
