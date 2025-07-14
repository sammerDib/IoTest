using System.Threading.Tasks;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Result.CommonUI.Proxy;
using UnitySC.Result.CommonUI.ViewModel.Search;
using UnitySC.Result.CommonUI.ViewModel.Wafers;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.Result.CommonUI.ViewModel
{
    public class MainResultVM : NavigationVM
    {
        #region Properties

        public override string PageName { get => "Main window"; }
        private readonly DuplexServiceInvoker<IResultService> _resultService;
        private readonly ILogger _logger;

        // propriété de la zone de recherche
        private SearchViewModel _searchVM;

        public SearchViewModel SearchVM
        {
            get => _searchVM; set
            {
                if (_searchVM != value)
                {
                    _searchVM = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isServiceConnected = false;

        public bool IsServiceConnected
        {
            get => _isServiceConnected;

            set
            {
                if (_isServiceConnected != value)
                {
                    _isServiceConnected = value; OnPropertyChanged();
                }
            }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;

            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value; OnPropertyChanged();
                }
            }
        }

        // propriété de la zone d'affichage de la liste des process modules
        private DisplayViewModel _displayVM;

        public DisplayViewModel DisplayVM
        {
            get => _displayVM;
            set
            {
                if (_displayVM != value) { _displayVM = value; OnPropertyChanged(); }
            }
        }

        private WaferPageVM _waferDetailPageVM;

        public WaferPageVM WaferDetailPageVM
        {
            get => _waferDetailPageVM;
            set
            {
                if (_waferDetailPageVM != value) { _waferDetailPageVM = value; OnPropertyChanged(); }
            }
        }

        private SettingsPageVM _viewerSettingsPageVM;

        public SettingsPageVM ViewerSettingsPageVM
        {
            get => _viewerSettingsPageVM;
            set
            {
                if (_viewerSettingsPageVM != value) { _viewerSettingsPageVM = value; OnPropertyChanged(); }
            }
        }

        #endregion Properties

        #region Selections

        public void OnChangeWaferPage(int npgid_notusedyet)
        {
            Navigate(WaferDetailPageVM);
        }

        public void OnChangeSettingPage(int npgid_notusedyet)
        {
            Navigate(ViewerSettingsPageVM);
        }

        private AutoRelayCommand _initCommand;

        public AutoRelayCommand InitCommand
        {
            get
            {
                return _initCommand ?? (_initCommand = new AutoRelayCommand(
              () =>
              {
                  InitComboboxAsync();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _retryConnectionCommand;

        public AutoRelayCommand RetryConnectionCommand
        {
            get
            {
                return _retryConnectionCommand ?? (_retryConnectionCommand = new AutoRelayCommand(
              () =>
              {
                  // Todo
                  InitComboboxAsync();
                  _searchVM.JobResultsList = null;
                  InitRessources();
              },
              () => { return true; }));
            }
        }

        #endregion Selections

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultService"></param>
        public MainResultVM(ResultSupervisor resultSupervisor, ILogger<MainResultVM> logger)
        {
            _resultService = resultSupervisor.Service;
            _logger = logger;
        }

        #endregion Constructor

        #region Methods

        public async void InitComboboxAsync()
        {
            bool isServiceConnectionOk = await Task.Run(() => _displayVM.GetConnectionAsync());
            if (isServiceConnectionOk)
                _searchVM.Init();
            //_displayVM.ShowConnectionErrorPopup = !isServiceConnectionOk;
        }

        public void Init()
        {
            //Instanciation du ViewModel de la page de recherche
            SearchVM = new SearchViewModel(_resultService, _logger);
            //Initialisation des données de DisplayVM
            _displayVM = new DisplayViewModel(_resultService);

            //wafer detail page
            _waferDetailPageVM = new WaferPageVM
            {
                DisplayVM = _displayVM
            };

            _displayVM.WaferPage = _waferDetailPageVM;
            _displayVM.OnDisplayWaferPage += OnChangeWaferPage;

            //settings detail pages
            _viewerSettingsPageVM = new SettingsPageVM(_resultService, _logger);
            SearchVM.OnDisplaySettings += OnChangeSettingPage;
        }

        public void InitRessources()
        {
            _displayVM.Init();
            _viewerSettingsPageVM.Init();

            // Raise all
            OnPropertyChanged(string.Empty);
        }

        #endregion Methods
    }
}
