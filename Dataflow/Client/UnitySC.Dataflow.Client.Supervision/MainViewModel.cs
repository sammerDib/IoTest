using System;
using System.ServiceModel;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Dataflow.Client.Supervision
{
    public class MainViewModel : ObservableRecipient
    {
        private ServiceInvoker<IDAP> _iDAPClient;
        public ServiceInvoker<IDAP> IDAPClient { get => _iDAPClient; }



        //89949FAB-D940-417E-A638-95F500EA33EC

        // BC6C7BA3-E176-4C78-A88C-607194D1A28D 
        // new dataflow10/20/2021 14:57:06

        // DEMETER
        // FBDFBC2F-D18B-4042-8FF8-0243AD42A1BC
        // 6b8b8005-e3cb-4e2c-b0b9-98d216be6c6a


        private string _recipeName = "89949FAB-D940-417E-A638-95F500EA33EC";
        public string RecipeName { get => _recipeName; set { _recipeName = value; OnPropertyChanged(); } }



        private ResultType _dataType = ResultType.Empty;
        public ResultType DataType { get => _dataType; set { _dataType = value; OnPropertyChanged(); } }

        private ActorType _actorType = ActorType.DEMETER;
        public ActorType ActorType { get => _actorType; set { _actorType = value; OnPropertyChanged(); } }


        private WorkflowActorDto _dataflowActorSelected = null;
        public WorkflowActorDto DataflowActorSelected { get => _dataflowActorSelected; set { _dataflowActorSelected = value; OnPropertyChanged(); } }

        private string _dataflowID = "WF01";
        public string DataflowID { get => _dataflowID; set { _dataflowID = value; OnPropertyChanged(); } }


        private bool _isDEMETERSimu = true;
        public bool IsDEMETERSimu { get => _isDEMETERSimu; set { _isDEMETERSimu = value; OnPropertyChanged(); } }

        private bool _isADCSimu = false;
        public bool IsADCSimu { get => _isADCSimu; set { _isADCSimu = value; OnPropertyChanged(); } }


        private bool _isADC_wfADCActor = true;
        public bool IsADC_wfADCActor { get => _isADC_wfADCActor; set { _isADC_wfADCActor = value; OnPropertyChanged(); } }

        private bool _isANALYSESimu = true;
        public bool IsANALYSESimu { get => _isANALYSESimu; set { _isANALYSESimu = value; OnPropertyChanged(); } }




        private ILogger _logger = ClassLocator.Default.GetInstance<ILogger>();

        public MainViewModel(
                                //IDataflowManagerSupervision dataflowManagerSupervision,
                                //ITCDataflowManager tCDataflowManager,
                                //IDataflowManagerDAP dataflowManagerDAP,
                                //DuplexServiceInvoker<IPMDataflowManager> pMDataflowManager,
                                //IPPDataflowManager pPDataflowManager,
                                ServiceInvoker<IDAP> dap


            )
        {


            // _dataflowManagerSupervision = dataflowManagerSupervision;
            // _iTCDataflowManager = tCDataflowManager;
            //_dataflowManagerDAP = dataflowManagerDAP;
            // _iPMDataflowManager = pMDataflowManager;
            // _iPPDataflowManager = pPDataflowManager;

            _iDAPClient = dap;

            //IDataflowManagerPM iDataflowManagerPM = new DataflowManagerPM();
            //IDataflowManagerPP iDataflowManagerPP = new DataflowManagerPP();
            // init


            // _dataflowManager.AddDataflowActorPM("PSD01", ActorType.DEMETER, iDataflowManagerPM);
            // _dataflowManager.AddDataflowActorPP("ADC01", ActorType.ADC, iDataflowManagerPP);




            _logger.Information($"Démarrage Application MainViewModel");
        }




        /* DataflowManager Refresh */
        private AutoRelayCommand _dataflowManagerRefresh = null;
        public AutoRelayCommand DataflowManagerRefresh
        {
            get
            {
                return _dataflowManagerRefresh ?? (_dataflowManagerRefresh = new AutoRelayCommand(
              () =>
              {
                  _logger.Information($"DataflowManager Refresh");


                  OnPropertyChanged("DataflowManager");
              }));
            }
        }

        /* DataflowManager Refresh */
        private AutoRelayCommand _initActorCommand = null;
        public AutoRelayCommand InitActorCommand
        {
            get
            {
                return _initActorCommand ?? (_initActorCommand = new AutoRelayCommand(
              () =>
              {


              }));
            }
        }


        private AutoRelayCommand _startDataflowManagerServiceCmd = null;
        public AutoRelayCommand StartDataflowManagerServiceCmd
        {
            get
            {
                return _startDataflowManagerServiceCmd ?? (_startDataflowManagerServiceCmd = new AutoRelayCommand(
              () => { }));
            }
        }

        private void StartService(string name, object service)
        {


            ServiceHost host = new ServiceHost(service);
            foreach (var endpoint in host.Description.Endpoints)
            {
                _logger.Information($"Creating {name} service on {endpoint.Address}");
            }
            host.Open();
            //_hosts.Add(name, host);
        }

        /*  ITCDataflowManager  */
        private AutoRelayCommand _tCDataflowManager_PrepareStartOfRecipe = null;
        public AutoRelayCommand TCDataflowManager_PrepareStartOfRecipe
        {
            get
            {
                return _tCDataflowManager_PrepareStartOfRecipe ?? (_tCDataflowManager_PrepareStartOfRecipe = new AutoRelayCommand(
              () =>
              {
                  ITCDataflowManager.PrepareStartOfRecipe(RecipeName, DataflowID);

                  _logger.Information($"[TC] to WFM WFID {DataflowID} RecipeName {RecipeName} => PrepareStartOfRecipe");

              }));
            }
        }

        private AutoRelayCommand _tCDataflowManager_RecipeCanBeExecuted = null;
        public AutoRelayCommand TCDataflowManager_RecipeCanBeExecuted
        {
            get
            {
                return _tCDataflowManager_RecipeCanBeExecuted ?? (_tCDataflowManager_RecipeCanBeExecuted = new AutoRelayCommand(
              () =>
              {
                  string actorID = DataflowActorSelected.ID;

                  ITCDataflowManager.RecipeCanBeExecuted(RecipeName, actorID, DataflowID);


                  _logger.Information($"[TC] to WFM {actorID} WFID {DataflowID} RecipeName {RecipeName} => RecipeCanBeExecuted");

              },
              () =>
              {
                  return DataflowActorSelected != null;
              }
              ));
            }
        }




        /*  IPMDataflowManager  */

        private AutoRelayCommand _iPMDataflowManager_CanRecipeBeExecuted = null;
        public AutoRelayCommand IPMDataflowManager_CanRecipeBeExecuted
        {
            get
            {
                return _iPMDataflowManager_CanRecipeBeExecuted ?? (_iPMDataflowManager_CanRecipeBeExecuted = new AutoRelayCommand(
              () =>
              {
                  IPMDataflowManager.Invoke(x => x.CanRecipeBeExecuted(RecipeName, DataflowID, ActorType));
              }));
            }
        }

        private AutoRelayCommand _iPMDataflowManager_DataAvailable = null;
        public AutoRelayCommand IPMDataflowManager_DataAvailable
        {
            get
            {
                return _iPMDataflowManager_DataAvailable ?? (_iPMDataflowManager_DataAvailable = new AutoRelayCommand(
              () =>
              {

                  //string recipeName,
                  //string dataflowID = "";

                  ResultType dataType = DataType;
                  //Guid dapWriteToken = Guid.NewGuid();


                  Guid dapWriteToken = IDAPClient.Invoke(i => i.GetWriteToken());


                  IDAPClient.Invoke(i => i.SendData(dapWriteToken, new DAPData() { Data = $"Mes Données {dataType}", DataDeletable = true, Token = dapWriteToken }));


                  //Task.Run(() =>
                  IPMDataflowManager.Invoke(x => x.DataAvailable(DataflowID, ActorType, dataType, dapWriteToken));
                  //);
              }));
            }
        }

        private AutoRelayCommand _iPMDataflowManager_RecipeEnded = null;
        public AutoRelayCommand IPMDataflowManager_RecipeEnded
        {
            get
            {
                return _iPMDataflowManager_RecipeEnded ?? (_iPMDataflowManager_RecipeEnded = new AutoRelayCommand(
              () =>
              {

                  IPMDataflowManager.Invoke(x => x.RecipeEnded(DataflowID, ActorType, true));
              }));
            }
        }



        /*  IPPDataflowManager  */

        private AutoRelayCommand _iPPDataflowManager_PrepareStartOfRecipe = null;
        public AutoRelayCommand IPPDataflowManager_PrepareStartOfRecipe
        {
            get
            {
                return _iPPDataflowManager_PrepareStartOfRecipe ?? (_iPPDataflowManager_PrepareStartOfRecipe = new AutoRelayCommand(
              () =>
              {
                  string actorID = DataflowActorSelected.ID;

                  IPPDataflowManager.RecipeCanBeExecuted(DataflowID, actorID);
              }));
            }
        }

        private AutoRelayCommand _iPPDataflowManager_DataAvalabled = null;
        public AutoRelayCommand IPPDataflowManager_DataAvalabled
        {
            get
            {
                return _iPPDataflowManager_DataAvalabled ?? (_iPPDataflowManager_DataAvalabled = new AutoRelayCommand(
              () =>
              {

                  //string recipeName,
                  //string dataflowID = "";

                  string actorID = DataflowActorSelected.ID;
                  ResultType dataType = DataType;
                  Guid dapToken = Guid.NewGuid();

                  IPPDataflowManager.DataAvailable(DataflowID, actorID, dataType, dapToken);
              }));
            }
        }

        private AutoRelayCommand _iPPDataflowManager_RecipeEnded = null;
        public AutoRelayCommand IPPDataflowManager_RecipeEnded
        {
            get
            {
                return _iPMDataflowManager_RecipeEnded ?? (_iPPDataflowManager_RecipeEnded = new AutoRelayCommand(
              () =>
              {
                  string actorID = DataflowActorSelected.ID;

                  IPPDataflowManager.RecipeEnded(actorID, DataflowID);
              }));
            }
        }
    }


    public class DataflowManagerCallback : IPMDataflowManagerCallback
    {
        void IPMDataflowManagerCallback.ExecuteRecipe(Guid actorRecipeID, string dataflowID, ResultType dataType, Guid dapToken)
        {
            Console.WriteLine($"IPMDataflowManagerCallback.ExecuteRecipe( {actorRecipeID} {dataflowID} {dataType}, {dapToken})");

        }

    }


    public class DataflowManagerPM : IDataflowManagerPM
    {
        private ILogger<IDataflowManagerPM> _logger = ClassLocator.Default.GetInstance<ILogger<IDataflowManagerPM>>();

        private bool _isStarted = false;


        private ServiceInvoker<IDataflowManagerPM> _serviceInvoker = null;


        public DataflowManagerPM() { }



        public DataflowManagerPM(string endPointName)
        {
            _serviceInvoker = new ServiceInvoker<IDataflowManagerPM>(endPointName, _logger);
        }





        Response<bool> IDataflowManagerPM.DataAvailable(string actorID, string dataflowID, Guid dapToken)
        {

            if (!_isStarted)
            {
                _logger.Error($"[DMT] DataAvalabled from {actorID} WFID {dataflowID} data {dapToken}, not started");

                return new Response<bool>() { Result = false };

            }


            _logger.Information($"[DMT] DataAvalabled from {actorID} WFID {dataflowID} data {dapToken}");

            if (_serviceInvoker != null)
                return new Response<bool>() { Result = _serviceInvoker.Invoke(x => x.DataAvailable(actorID, dataflowID, dapToken)) };

            return new Response<bool>() { Result = true };
        }


        Response<VoidResult> IDataflowManagerPM.StartPMRecipe(string actorRecipeName, string actorId, string dataFlowId)
        {

            _logger.Information($"[DMT] StartPPRecipe {actorRecipeName} from {actorId} WFID {dataFlowId}");

            _isStarted = true;


            if (_serviceInvoker != null)
                return new Response<VoidResult>() { Result = _serviceInvoker.Invoke(x => x.StartPMRecipe(actorRecipeName, actorId, dataFlowId)) };

            return new Response<VoidResult>();

        }
    }

    public class DataflowManagerPP : IDataflowManagerPP
    {
        private ILogger<IDataflowManagerPP> _logger = ClassLocator.Default.GetInstance<ILogger<IDataflowManagerPP>>();


        private bool _isStarted = false;

        private ServiceInvoker<IDataflowManagerPP> _serviceInvoker = null;
        private ServiceInvoker<IDAP> _iDAPClient = ClassLocator.Default.GetInstance<ServiceInvoker<IDAP>>();



        public DataflowManagerPP() { }

        public DataflowManagerPP(string endPointName)
        {
            _serviceInvoker = new ServiceInvoker<IDataflowManagerPP>(endPointName, _logger);
        }


        Response<bool> IDataflowManagerPP.DataAvailable(string actorID, string dataflowID, Guid dapToken)
        {
            if (!_isStarted)
            {
                _logger.Error($"[ADC] DataAvalabled from {actorID} WFID {dataflowID} data {dapToken}, not started");

                return new Response<bool>() { Result = false };

            }

            var data = _iDAPClient.Invoke(i => i.GetData(dapToken));

            _logger.Information($"[ADC] DataAvalabled from {actorID} WFID {dataflowID} data {dapToken} {data.Data}");

            if (_serviceInvoker != null)
                return new Response<bool>() { Result = _serviceInvoker.Invoke(x => x.DataAvailable(actorID, dataflowID, dapToken)) };

            return new Response<bool>() { Result = true };
        }

        Response<bool> IDataflowManagerPP.RecipeEnded(string actorID, string dataflowID)
        {
            _logger.Information($"[ADC] RecipeEnded from {actorID} WFID {dataflowID}");

            if (_serviceInvoker != null)
                return new Response<bool>() { Result = _serviceInvoker.Invoke(x => x.RecipeEnded(actorID, dataflowID)) };

            _isStarted = false;

            return new Response<bool>() { Result = true };

        }

        Response<VoidResult> IDataflowManagerPP.StartPPRecipe(string actorRecipeName, string actorId, string dataFlowId)
        {

            _logger.Information($"[ADC] StartPPRecipe {actorRecipeName} from {actorId} WFID {dataFlowId}");
            _isStarted = true;

            if (_serviceInvoker != null)
                return new Response<VoidResult>() { Result = _serviceInvoker.Invoke(x => x.StartPPRecipe(actorRecipeName, actorId, dataFlowId)) };

            return new Response<VoidResult>();

        }
    }

}
