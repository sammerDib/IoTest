using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Dataflow.Shared.Configuration;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.Shared.Data;
using Material = UnitySC.Shared.Data.Material;

namespace UnitySC.TC.UI.Dataflow.ViewModel
{
    public class DataflowUTOSimulatorViewModel : ObservableRecipient
    {
        private InstanceContext _instanceContext;
        private ILogger _logger;
        private UtoDfSupervisor _utoDfSupervisor;
        private PmDfSupervisor _pmDfSupervisor;
        private List<Material> _wafers;
        private Identity _identity = new Identity(4, ActorType.ANALYSE, 3);
        private IDFClientConfiguration _dfClientConfiguration;

        public DataflowUTOSimulatorViewModel() : base()
        {
            var messenger = new WeakReferenceMessenger();
            _logger = new SerilogLogger<IUTODFService>();
            _instanceContext = new InstanceContext(this);

            _utoDfSupervisor = new UtoDfSupervisor();
            _pmDfSupervisor = new PmDfSupervisor();
            StartRecipe = false;

            _dfClientConfiguration = ClassLocator.Default.GetInstance<IDFClientConfiguration>();

            //Init wafer
            var wafer1 = new Material()
            {
                GUIDWafer = new Guid(),
                ControlJobID = "job_1",
                AcquiredID = "AcquiredID_1",
                MaterialType = 1,
                LoadportID = 1,
                OrientationAngle = 10,
                SlotID = 1,
                CarrierID = "CarrierID_1",
                ProcessJobID = "ProcessJobID_1",
                LotID = "LotID_1",
                SubstrateID = "SubstrateID_1",
            };
            var wafer2 = new Material()
            {
                GUIDWafer = new Guid(),
                ControlJobID = "job_2",
                AcquiredID = "AcquiredID_2",
                MaterialType = 1,
                LoadportID = 1,
                OrientationAngle = 10,
                SlotID = 1,
                CarrierID = "CarrierID_1",
                ProcessJobID = "ProcessJobID_1",
                LotID = "LotID_1",
                SubstrateID = "SubstrateID_1",
            };
            _wafers = new List<Material> { wafer1, wafer2 };
        }

        public bool StartRecipe { get; set; }

        internal void Init()
        {
            // var  result = _iUTODataflowManager.InvokeAndGetMessages(r => r.GetDataTest());
        }

        private ObservableCollection<DataflowRecipeInfo> _dfCollection;
        public ObservableCollection<DataflowRecipeInfo> DFCollection
        {
            get
            {
                if (_dfCollection == null)
                {
                    _dfCollection = new ObservableCollection<DataflowRecipeInfo>();
                }
                return _dfCollection;
            }
            set { if (_dfCollection != value) { _dfCollection = value; OnPropertyChanged(); } }
        }
        private DataflowRecipeInfo _dfRecipeSelected;
        public DataflowRecipeInfo DFRecipeSelected
        {
            get
            {
                return _dfRecipeSelected;
            }
            set { if (_dfRecipeSelected != value) { _dfRecipeSelected = value; StartRecipeDFCommand.NotifyCanExecuteChanged(); StartJobCommand.NotifyCanExecuteChanged(); AbortRecipeCmd.NotifyCanExecuteChanged(); InitializeCmd.NotifyCanExecuteChanged(); OnPropertyChanged(); } }
        }

        private ObservableCollection<PMItem> _pmItems;
        public ObservableCollection<PMItem> PMItems
        {
            get
            {
                if (_pmItems == null)
                {
                    _pmItems = new ObservableCollection<PMItem>();
                }
                return _pmItems;
            }
            set { if (_pmItems != value) { _pmItems = value; OnPropertyChanged(); } }
        }

        private AutoRelayCommand _dfRecipeCommand;
        public AutoRelayCommand DFARecipeCommand
        {
            get
            {
                return _dfRecipeCommand ?? (_dfRecipeCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      DFCollection.Clear();
                      var dfRecipes = _utoDfSupervisor.GetAllDataflowRecipes(_dfClientConfiguration.AvailableModules);
                      foreach (var dfrecipe in dfRecipes.Result)
                      {
                          DFCollection.Add(dfrecipe);
                      }
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }

              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _startRecipeDFCommand;
        public AutoRelayCommand StartRecipeDFCommand
        {
            get
            {
                return _startRecipeDFCommand ?? (_startRecipeDFCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      var dfRecipes = _utoDfSupervisor.StartRecipeDF(DFRecipeSelected, "jobId1234", _wafers.Select(x => x.GUIDWafer).ToList());
                      PMItems.Clear();
                      if (dfRecipes?.Result?.PMItems != null)
                      {
                          PMItems.AddRange(dfRecipes.Result.PMItems);
                      }
                      //CanExecute
                      StartRecipe = true;
                      StartRecipeRequestCommand.NotifyCanExecuteChanged();
                      RecipeStartedCmd.NotifyCanExecuteChanged();
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }
              },
              () => { return DFRecipeSelected != null; }));
            }
        }

        private AutoRelayCommand _startJobCommand;
        public AutoRelayCommand StartJobCommand
        {
            get
            {
                return _startJobCommand ?? (_startJobCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      _utoDfSupervisor.StartJob_Material(DFRecipeSelected, _wafers.FirstOrDefault());
                      MessageBox.Show("Job started from UTO");
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }

              },
              () => { return DFRecipeSelected != null; }));
            }
        }

        private AutoRelayCommand _startRecipeRequestCommand;
        public AutoRelayCommand StartRecipeRequestCommand
        {
            get
            {
                return _startRecipeRequestCommand ?? (_startRecipeRequestCommand = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      var pmDfRecipe = _pmDfSupervisor.StartRecipeRequest(_identity, _wafers.FirstOrDefault());
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }
              },
              () => { return StartRecipe; }));
            }
        }


        private AutoRelayCommand _recipeStartedCmd;
        public AutoRelayCommand RecipeStartedCmd
        {
            get
            {
                return _recipeStartedCmd ?? (_recipeStartedCmd = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      _pmDfSupervisor.RecipeStarted(_identity, _wafers.FirstOrDefault());
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }
              },
              () => { return StartRecipe; }));
            }
        }
        private AutoRelayCommand _abortRecipeCmd;
        public AutoRelayCommand AbortRecipeCmd
        {
            get
            {
                return _abortRecipeCmd ?? (_abortRecipeCmd = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      _utoDfSupervisor.AbortRecipe(DFRecipeSelected);
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }
              },
                 () => { return DFRecipeSelected != null; }));
            }
        }

        private AutoRelayCommand _initializeCmd;
        public AutoRelayCommand InitializeCmd
        {
            get
            {
                return _initializeCmd ?? (_initializeCmd = new AutoRelayCommand(
              () =>
              {
                  try
                  {
                      _utoDfSupervisor.Initialize();
                  }
                  catch (Exception ex)
                  {
                      MessageBox.Show(ex.Message);
                  }
              },
                 () => { return DFRecipeSelected != null; }));
            }
        }

    }
}
