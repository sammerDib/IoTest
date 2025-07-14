using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.DataAccess.Service.Interface;
using UnitySC.Result.CommonUI.ViewModel.Search.SettingsPages;
using UnitySC.Shared.Format.Base;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.ViewModel.Navigation;

namespace UnitySC.Result.CommonUI.ViewModel.Search
{
    public class SettingsPageVM : PageNavigationVM
    {
        // private
        private readonly ILogger _logger;

        private readonly DuplexServiceInvoker<IResultService> _resultService;
        private readonly IResultDataFactory _resfactory;
        private readonly IMessenger _messenger;

        // Public
        public override string PageName => "Settings";     // Nom de la page

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="resultService"></param>
        /// <param name="logger"></param>
        public SettingsPageVM(DuplexServiceInvoker<IResultService> resultService, ILogger logger)
        {
            _resultService = resultService;
            _resfactory = ClassLocator.Default.GetInstance<IResultDataFactory>();
            _logger = logger;
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();

            KlarfSettingsPage = new KlarfSettingsPageViewModel(_resultService);
            HazeSettingsPage = new HazeSettingsPageViewModel(_resultService);
            ThumbnailSettingsPage = new ThumbnailSettingsPageVM(_resultService);

            SettingsPages.Add(KlarfSettingsPage);
            SettingsPages.Add(HazeSettingsPage);
            SettingsPages.Add(ThumbnailSettingsPage);

            SelectedPage = KlarfSettingsPage;
        }

        #endregion Constructor

        #region Properties

        public KlarfSettingsPageViewModel KlarfSettingsPage { get; }

        public HazeSettingsPageViewModel HazeSettingsPage { get; }

        public ThumbnailSettingsPageVM ThumbnailSettingsPage { get; }

        public List<BaseSettingsPageViewModel> SettingsPages { get; } = new List<BaseSettingsPageViewModel>();

        private BaseSettingsPageViewModel _selectedPage;

        public BaseSettingsPageViewModel SelectedPage
        {
            get { return _selectedPage; }
            set { SetProperty(ref _selectedPage, value); }
        }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        #endregion Properties

        public void Init()
        {
            IsBusy = true;

            KlarfSettingsPage.RefreshKlarfSettings();
            HazeSettingsPage.RefreshHazeSettings();
            ThumbnailSettingsPage.LoadColorMapSettings();

            IsBusy = false;
        }
    }
}
