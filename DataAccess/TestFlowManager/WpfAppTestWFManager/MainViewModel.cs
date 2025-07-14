using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;


using UnitySC.DataAccess.Service.Implementation.Workflow;
using UnitySC.DataAccess.Service.Interface.Workflow;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace WpfAppTestFlowManager
{
    public class MainViewModel : ViewModelBase
    {

        private WorkflowManager _workflowManager = null;
        public WorkflowManager WorkflowManager { get => _workflowManager; }


        private IWorkflowManagerSupervision _workflowManagerSupervision;
        public IWorkflowManagerSupervision WorkflowManagerSupervision { get => _workflowManagerSupervision; }

        private ITCWorkflowManager _iTCWorkflowManager;
        public ITCWorkflowManager ITCWorkflowManager { get => _iTCWorkflowManager; }

        private IWorkflowManagerDAP _workflowManagerDAP;
        public IWorkflowManagerDAP WorkflowManagerDAP { get => _workflowManagerDAP; }

        private DuplexServiceInvoker<IPMWorkflowManager> _iPMWorkflowManager;
        public DuplexServiceInvoker<IPMWorkflowManager> IPMWorkflowManager { get => _iPMWorkflowManager; }

        private IPPWorkflowManager _iPPWorkflowManager;
        public IPPWorkflowManager IPPWorkflowManager { get => _iPPWorkflowManager; }


        private ServiceInvoker<IDAP> _iDAPClient;
        public ServiceInvoker<IDAP> IDAPClient { get => _iDAPClient; }



        //89949FAB-D940-417E-A638-95F500EA33EC

        // BC6C7BA3-E176-4C78-A88C-607194D1A28D 
        // new workflow10/20/2021 14:57:06

        // PSD
        // FBDFBC2F-D18B-4042-8FF8-0243AD42A1BC
        // 6b8b8005-e3cb-4e2c-b0b9-98d216be6c6a


        private string _recipeName = "CRI_DeLaRussie_GoulagWorkFlow_PSD6/23/2022 10:59:09 AM"; 
        public string RecipeName { get => _recipeName; set { _recipeName = value; RaisePropertyChanged(); } }



        private ResultType _dataType = ResultType.Empty;
        public ResultType DataType { get => _dataType; set { _dataType = value; RaisePropertyChanged(); } }

        private ActorType _actorType = ActorType.PSD;
        public ActorType ActorType { get => _actorType; set { _actorType = value; RaisePropertyChanged(); } }


        private WorkflowActor _workflowActorSelected = null;
        public WorkflowActor WorkflowActorSelected { get => _workflowActorSelected; set { _workflowActorSelected = value; RaisePropertyChanged(); } }

        private string _workflowID = "WF01";
        public string WorkflowID { get => _workflowID; set { _workflowID = value; RaisePropertyChanged(); } }


        private bool _isPSDSimu = true;
        public bool IsPSDSimu { get => _isPSDSimu; set { _isPSDSimu = value; RaisePropertyChanged(); } }

        private bool _isADCSimu = false;
        public bool IsADCSimu { get => _isADCSimu; set { _isADCSimu = value; RaisePropertyChanged(); } }


        private bool _isADC_wfADCActor = true;
        public bool IsADC_wfADCActor { get => _isADC_wfADCActor; set { _isADC_wfADCActor = value; RaisePropertyChanged(); } }

        private bool _isANALYSESimu = true;
        public bool IsANALYSESimu { get => _isANALYSESimu; set { _isANALYSESimu = value; RaisePropertyChanged(); } }




        ILogger logger = ClassLocator.Default.GetInstance<ILogger>();

        public MainViewModel(WorkflowManager workflowManager,
                                IWorkflowManagerSupervision workflowManagerSupervision,
                                ITCWorkflowManager tCWorkflowManager,
                                IWorkflowManagerDAP workflowManagerDAP,
                                DuplexServiceInvoker<IPMWorkflowManager> pMWorkflowManager,
                                IPPWorkflowManager pPWorkflowManager,
                                ServiceInvoker<IDAP> dap
            )
        {

            _workflowManager = workflowManager;

            _workflowManagerSupervision = workflowManagerSupervision;
            _iTCWorkflowManager = tCWorkflowManager;
            _workflowManagerDAP = workflowManagerDAP;
            _iPMWorkflowManager = pMWorkflowManager;
            _iPPWorkflowManager = pPWorkflowManager;

            _iDAPClient = dap;

            //IWorkflowManagerPM iWorkflowManagerPM = new WorkflowManagerPM();
            //IWorkflowManagerPP iWorkflowManagerPP = new WorkflowManagerPP();
            // init


           // _workflowManager.AddWorkFlowActorPM("PSD01", ActorType.PSD, iWorkflowManagerPM);
           // _workflowManager.AddWorkFlowActorPP("ADC01", ActorType.ADC, iWorkflowManagerPP);




            logger.Information($"Démarrage Application MainViewModel");
        }




        /* WorkflowManager Refresh */
        private RelayCommand _workflowManagerRefresh = null;
        public RelayCommand WorkflowManagerRefresh
        {
            get
            {
                return _workflowManagerRefresh ?? (_workflowManagerRefresh = new RelayCommand(
              () =>
              {
                  logger.Information($"WorkflowManager Refresh");


                  RaisePropertyChanged("WorkflowManager");
              }));
            }
        }

        /* WorkflowManager Refresh */
        private RelayCommand _initActorCommand = null;
        public RelayCommand InitActorCommand
        {
            get
            {
                return _initActorCommand ?? (_initActorCommand = new RelayCommand(
              () =>
              {


              }));
            }
        }


        private RelayCommand _startWorkflowManagerServiceCmd = null;
        public RelayCommand StartWorkflowManagerServiceCmd
        {
            get
            {
                return _startWorkflowManagerServiceCmd ?? (_startWorkflowManagerServiceCmd = new RelayCommand(
              () =>
              {                  
                  logger.Information($"Init Actor");

                  //IWorkflowManagerPM iWorkflowManagerPSD = IsPSDSimu ? new WorkflowManagerPM() :  new WorkflowManagerPM("wfPSDActor");
                  //IWorkflowManagerPM iWorkflowManagerANALYSE = IsANALYSESimu ? new WorkflowManagerPM() : new WorkflowManagerPM("wfANALYSEActor");


                  if(IsADCSimu)
                  {
                      logger.Information($"Actor ADC_SIM");
                      _workflowManager.AddWorkFlowActorPP("ADC_SIM", ActorType.ADC,  new WorkflowManagerPP());
                  }

                  if(IsADC_wfADCActor)
                  {
                      logger.Information($"Actor ADC_01 wfADCActor");
                      _workflowManager.AddWorkFlowActorPP("ADC_01", ActorType.ADC, new WorkflowManagerPP("wfADCActor"));
                  }

                  _workflowManager.AddWorkFlowActorPM("PSD01", ActorType.PSD);

                  //_workflowManager.AddWorkFlowActorPM("PSD01", ActorType.PSD, iWorkflowManagerPSD);
                  //_workflowManager.AddWorkFlowActorPM("ANALYSE01", ActorType.ANALYSE, iWorkflowManagerANALYSE);


                  logger.Information($"Start WorkflowManager Service");


                  UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManagerSupervision sWorkflowManagerSupervision = ClassLocator.Default.GetInstance<UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManagerSupervision>();

                  UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManager sWorkflowManager = ClassLocator.Default.GetInstance<UnitySC.DataAccess.Service.Interface.Workflow.IWorkflowManager>();
               
                  UnitySC.DataAccess.Service.Interface.Workflow.IDAP sDAP = ClassLocator.Default.GetInstance<UnitySC.DataAccess.Service.Interface.Workflow.IDAP>();


                  try
                  {
                      Task.Run(() => StartService("IWorkflowManagerSupervision", sWorkflowManagerSupervision));

                      Task.Run(() => StartService("WorkflowManager", sWorkflowManager));

                      Task.Run(() => StartService("DAP", sDAP));
                  }
                  catch
                  { }

              }));
            }
        }

        private void StartService(string name, object service)
        {


            ServiceHost host = new ServiceHost(service);
            foreach (var endpoint in host.Description.Endpoints)
            {
                logger.Information($"Creating {name} service on {endpoint.Address}");
            }
            host.Open();
            //_hosts.Add(name, host);
        }

        /*  ITCWorkflowManager  */
        private RelayCommand _TCWorkflowManager_PrepareStartOfRecipe = null;
        public RelayCommand TCWorkflowManager_PrepareStartOfRecipe
        {
            get
            {
                return _TCWorkflowManager_PrepareStartOfRecipe ?? (_TCWorkflowManager_PrepareStartOfRecipe = new RelayCommand(
              () =>
              {
                  ITCWorkflowManager.PrepareStartOfRecipe(RecipeName, WorkflowID);

                  logger.Information($"[TC] to WFM WFID {WorkflowID} RecipeName {RecipeName} => PrepareStartOfRecipe");

              }));
            }
        }

        private RelayCommand _TCWorkflowManager_RecipeCanBeExecuted = null;
        public RelayCommand TCWorkflowManager_RecipeCanBeExecuted
        {
            get
            {
                return _TCWorkflowManager_RecipeCanBeExecuted ?? (_TCWorkflowManager_RecipeCanBeExecuted = new RelayCommand(
              () =>
              {
                  string actorID = WorkflowActorSelected.ID;

                  ITCWorkflowManager.RecipeCanBeExecuted(RecipeName, actorID, WorkflowID);


                  logger.Information($"[TC] to WFM {actorID} WFID {WorkflowID} RecipeName {RecipeName} => RecipeCanBeExecuted");

              },
              ()=>
              {
                  return WorkflowActorSelected != null;
              }
              ));
            }
        }




        /*  IPMWorkflowManager  */

        private RelayCommand _IPMWorkflowManager_CanRecipeBeExecuted = null;
        public RelayCommand IPMWorkflowManager_CanRecipeBeExecuted
        {
            get
            {
                return _IPMWorkflowManager_CanRecipeBeExecuted ?? (_IPMWorkflowManager_CanRecipeBeExecuted = new RelayCommand(
              () =>
              {
                  IPMWorkflowManager.Invoke(x => x.CanRecipeBeExecuted(RecipeName, WorkflowID, ActorType));
              }));
            }
        }

        private RelayCommand _IPMWorkflowManager_DataAvailable = null;
        public RelayCommand IPMWorkflowManager_DataAvailable
        {
            get
            {
                return _IPMWorkflowManager_DataAvailable ?? (_IPMWorkflowManager_DataAvailable = new RelayCommand(
              () =>
              {

                  //string recipeName,
                  //string workflowID = "";

                  ResultType dataType = DataType;
                  //Guid dapWriteToken = Guid.NewGuid();


                  Guid dapWriteToken = IDAPClient.Invoke(i => i.GetWriteToken());


                  IDAPClient.Invoke(i => i.SendData(dapWriteToken , new UnitySC.DataAccess.Service.Interface.Workflow.dto.DAPData() { Data = $"Mes Données {dataType}", DataDeletable = true, Token = dapWriteToken }));


                  //Task.Run(() =>
                  IPMWorkflowManager.Invoke(x => x.DataAvailable(WorkflowID, ActorType, dataType, dapWriteToken));
                  //);
              }));
            }
        }

        private RelayCommand _IPMWorkflowManager_RecipeEnded = null;
        public RelayCommand IPMWorkflowManager_RecipeEnded
        {
            get
            {
                return _IPMWorkflowManager_RecipeEnded ?? (_IPMWorkflowManager_RecipeEnded = new RelayCommand(
              () =>
              {

                  IPMWorkflowManager.Invoke(x => x.RecipeEnded(WorkflowID, ActorType ,true));
              }));
            }
        }



        /*  IPPWorkflowManager  */

        private RelayCommand _IPPWorkflowManager_PrepareStartOfRecipe = null;
        public RelayCommand IPPWorkflowManager_PrepareStartOfRecipe
        {
            get
            {
                return _IPPWorkflowManager_PrepareStartOfRecipe ?? (_IPPWorkflowManager_PrepareStartOfRecipe = new RelayCommand(
              () =>
              {
                  string actorID = WorkflowActorSelected.ID;

                  IPPWorkflowManager.RecipeCanBeExecuted( WorkflowID, actorID );
              }));
            }
        }

        private RelayCommand _IPPWorkflowManager_DataAvalabled = null;
        public RelayCommand IPPWorkflowManager_DataAvalabled
        {
            get
            {
                return _IPPWorkflowManager_DataAvalabled ?? (_IPPWorkflowManager_DataAvalabled = new RelayCommand(
              () =>
              {

                  //string recipeName,
                  //string workflowID = "";

                  string actorID = WorkflowActorSelected.ID;
                  ResultType dataType = DataType;
                  Guid dapToken = Guid.NewGuid();

                  IPPWorkflowManager.DataAvailable( WorkflowID, actorID, dataType, dapToken);
              }));
            }
        }

        private RelayCommand _IPPWorkflowManager_RecipeEnded = null;
        public RelayCommand IPPWorkflowManager_RecipeEnded
        {
            get
            {
                return _IPMWorkflowManager_RecipeEnded ?? (_IPPWorkflowManager_RecipeEnded = new RelayCommand(
              () =>
              {
                  string actorID = WorkflowActorSelected.ID;

                  IPPWorkflowManager.RecipeEnded( actorID, WorkflowID);
              }));
            }
        }



        /*  IWorkflowManagerDAP  */



        /* I[ PP PM ]WorkflowManager IPPWorkflowManager */
        /*
        private RelayCommand _IPPPMWorkflowManager_PrepareStartOfRecipe = null;
        public RelayCommand IPPPMWorkflowManager_PrepareStartOfRecipe
        {
            get
            {
                return _IPPPMWorkflowManager_PrepareStartOfRecipe ?? (_IPPPMWorkflowManager_PrepareStartOfRecipe = new RelayCommand(
              () =>
              {
                  string actorID = WorkflowActorSelected.ID;


                  switch (WorkflowActorSelected.ActorType.GetCatgory())
                  {
                      case ActorCategory.ProcessModule:
                          logger.Information($"[WFM] to {actorID} WFID {WorkflowID} RecipeName {RecipeName} => RecipeCanBeExecuted");

                          IPMWorkflowManager.RecipeCanBeExecuted(WorkflowID  , actorID );
                          break;

                      case ActorCategory.PostProcessing:
                          logger.Information($"[WFM] to {actorID} WFID {WorkflowID} RecipeName {RecipeName} => RecipeCanBeExecuted");


                          IPPWorkflowManager.RecipeCanBeExecuted(RecipeName, actorID, WorkflowID);
                          break;
                  }


              },
              () =>
              {
                  return WorkflowActorSelected != null;
              }));
            }
        }

        private RelayCommand _IPPPMWorkflowManager_DataAvalabled = null;
        public RelayCommand IPPPMWorkflowManager_DataAvalabled
        {
            get
            {
                return _IPPPMWorkflowManager_DataAvalabled ?? (_IPPPMWorkflowManager_DataAvalabled = new RelayCommand(
              () =>
              {

                  //string recipeName,
                  //string workflowID = "";

                  string actorID = WorkflowActorSelected.ID;

                  ResultType dataType = DataType;


                  Guid dapToken = Guid.NewGuid();

                  logger.Information($"[{WorkflowID}] to WFM RecipeName {RecipeName} => DataAvailabled, {dataType} {dapToken}");


                  switch (WorkflowActorSelected.ActorType.GetCatgory())
                  {
                      case ActorCategory.ProcessModule:
                          logger.Information($"[WFM] to {actorID} WFID {WorkflowID} RecipeName {RecipeName} => DataAvailabled, {dataType} {dapToken}");

                          IPMWorkflowManager.DataAvailable(RecipeName, WorkflowID, actorID, dataType, dapToken);
                          break;

                      case ActorCategory.PostProcessing:
                          logger.Information($"[WFM] to {actorID} WFID {WorkflowID} RecipeName {RecipeName} => DataAvailabled, {dataType} {dapToken}");

                          IPPWorkflowManager.DataAvailable(RecipeName, WorkflowID, actorID, dataType, dapToken);
                          break;
                  }


                  
              },
              () =>
              {
                  return WorkflowActorSelected != null;
              }));
            }
        }

        private RelayCommand _IPPPMWorkflowManager_RecipeEnded = null;
        public RelayCommand IPPPMWorkflowManager_RecipeEnded
        {
            get
            {
                return _IPPPMWorkflowManager_RecipeEnded ?? (_IPPPMWorkflowManager_RecipeEnded = new RelayCommand(
              () =>
              {
                  string actorID = WorkflowActorSelected.ID;

                  switch (WorkflowActorSelected.ActorType.GetCatgory())
                  {
                      case ActorCategory.ProcessModule:
                          logger.Information($"[WFM] to {actorID} WFID {WorkflowID} RecipeName {RecipeName} => RecipeEnded");

                          IPMWorkflowManager.RecipeEnded(RecipeName, actorID, WorkflowID);
                          break;

                      case ActorCategory.PostProcessing:
                          logger.Information($"[WFM] to {actorID} WFID {WorkflowID} RecipeName {RecipeName} => RecipeEnded");

                          IPPWorkflowManager.RecipeEnded(RecipeName, actorID, WorkflowID);
                          break;
                  }


                  IPMWorkflowManager.RecipeEnded(RecipeName, actorID, WorkflowID);
              },
              () =>
              {
                  return WorkflowActorSelected != null;
              }));
            }
        }
        */
    }


    public class WorkflowManagerCallback : IPMWorkflowManagerCallback
    {
        void IPMWorkflowManagerCallback.ExecuteRecipe(Guid actorRecipeID, string workflowID, ResultType dataType, Guid dapToken)
        {
            Console.WriteLine($"IPMWorkflowManagerCallback.ExecuteRecipe( {actorRecipeID} {workflowID} {dataType}, {dapToken})");

        }

    }


    public class WorkflowManagerPM : IWorkflowManagerPM
    {
        ILogger<IWorkflowManagerPM> logger = ClassLocator.Default.GetInstance<ILogger<IWorkflowManagerPM>>();

        bool isStarted = false;


        private ServiceInvoker<IWorkflowManagerPM> serviceInvoker = null;


        public WorkflowManagerPM() { }



        public WorkflowManagerPM(string endPointName)
        {
            serviceInvoker = new ServiceInvoker<IWorkflowManagerPM>(endPointName, logger);
        }





        Response<bool> IWorkflowManagerPM.DataAvailable(string actorID, string workflowID, Guid dapToken)
        {

            if(!isStarted)
            {
                logger.Error($"[PSD] DataAvalabled from {actorID} WFID {workflowID} data {dapToken}, not started");

                return new Response<bool>() { Result = false };

            }


            logger.Information($"[PSD] DataAvalabled from {actorID} WFID {workflowID} data {dapToken}");

                if (serviceInvoker != null)
                    return new Response<bool>() { Result = serviceInvoker.Invoke(x => x.DataAvailable(actorID, workflowID, dapToken)) };

            return new Response<bool>() { Result = true };
        }


        Response<VoidResult> IWorkflowManagerPM.StartPPRecipe(string actorRecipeName, string actorId, string workFlowId)
        {

            logger.Information($"[PSD] StartPPRecipe {actorRecipeName} from {actorId} WFID {workFlowId}");

            isStarted = true;


            if (serviceInvoker != null)
                return new Response<VoidResult>() { Result = serviceInvoker.Invoke(x => x.StartPPRecipe(actorRecipeName,  actorId,  workFlowId)) };

            return new Response<VoidResult>();

        }
    }

    public class WorkflowManagerPP : IWorkflowManagerPP
    {
        ILogger<IWorkflowManagerPP> logger = ClassLocator.Default.GetInstance<ILogger<IWorkflowManagerPP>>();


        bool isStarted = false;

        private ServiceInvoker<IWorkflowManagerPP> serviceInvoker = null;
        private ServiceInvoker<IDAP> _iDAPClient = ClassLocator.Default.GetInstance<ServiceInvoker<IDAP>>();



        public WorkflowManagerPP() { }

        public WorkflowManagerPP(string endPointName)
        {
            serviceInvoker = new ServiceInvoker<IWorkflowManagerPP>(endPointName, logger);
        }


        Response<bool> IWorkflowManagerPP.DataAvailable(string actorID, string workflowID, Guid dapToken)
        {
            if (!isStarted)
            {
                logger.Error($"[ADC] DataAvalabled from {actorID} WFID {workflowID} data {dapToken}, not started");

                return new Response<bool>() { Result = false };

            }

            var data = _iDAPClient.Invoke(i => i.GetData(dapToken));

            logger.Information($"[ADC] DataAvalabled from {actorID} WFID {workflowID} data {dapToken} {data.Data}");

            if (serviceInvoker != null)
                return new Response<bool>() { Result = serviceInvoker.Invoke(x => x.DataAvailable(actorID, workflowID, dapToken)) };

            return new Response<bool>() { Result = true };
        }

        Response<bool> IWorkflowManagerPP.RecipeEnded( string actorID, string workflowID)
        {
            logger.Information($"[ADC] RecipeEnded from {actorID} WFID {workflowID}");

            if (serviceInvoker != null)
                return new Response<bool>() { Result = serviceInvoker.Invoke(x => x.RecipeEnded( actorID, workflowID)) };

            isStarted = false;

            return new Response<bool>() { Result = true };

        }

        Response<VoidResult> IWorkflowManagerPP.StartPPRecipe(string actorRecipeName, string actorId, string workFlowId)
        {

            logger.Information($"[ADC] StartPPRecipe {actorRecipeName} from {actorId} WFID {workFlowId}");
            isStarted = true;

            if (serviceInvoker != null)
                return new Response<VoidResult>() { Result = serviceInvoker.Invoke(x => x.StartPPRecipe(actorRecipeName, actorId, workFlowId)) };

            return new Response<VoidResult>();

        }
    }

}
