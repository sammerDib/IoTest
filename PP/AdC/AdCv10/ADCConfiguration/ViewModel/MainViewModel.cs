using System.Collections.ObjectModel;

using ADCConfiguration.Services;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCConfiguration.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ObservableRecipient
    {

        public AuthentificationService AuthentificationService { get; private set; }
        public ShutdownService ShutdownService { get; private set; }


        public ObservableCollection<MenuItemViewModel> MenuItems { get; private set; } = new ObservableCollection<MenuItemViewModel>();


        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(AuthentificationService authentificationService, ShutdownService shutdownService)
        {
            AuthentificationService = authentificationService;
            ShutdownService = shutdownService;
        }

        public AutoRelayCommand<System.ComponentModel.CancelEventArgs> _closingApplicationCommand = null;
        public AutoRelayCommand<System.ComponentModel.CancelEventArgs> ClosingApplicationCommand
        {
            get
            {
                return _closingApplicationCommand != null ? _closingApplicationCommand : _closingApplicationCommand
                = new AutoRelayCommand<System.ComponentModel.CancelEventArgs>((arg) =>
                {
                    arg.Cancel = true;
                    // on transfert le traitement et la decision au service d'arret (ShutdownService);
                    ShutdownService.ShutdownApp();
                }, (arg) => { return true; });
            }
        }



        public AutoRelayCommand _loadedApplicationCommand = null;
        public AutoRelayCommand LoadedApplicationCommand
        {
            get
            {
                return _loadedApplicationCommand != null ? _loadedApplicationCommand : _loadedApplicationCommand
                = new AutoRelayCommand(() =>
                {
                    Connection();
                });
            }
        }

        public string Login
        {
            get { return AuthentificationService.CurrentUser?.Login; }
        }

        private AutoRelayCommand _logOffCommand;
        public AutoRelayCommand LogOffCommand
        {
            get
            {
                return _logOffCommand ?? (_logOffCommand = new AutoRelayCommand(
              () =>
              {
#warning ** USP ** Disconnect to do
                  //ClassLocator.Default.GetInstance<ILogService>().Disconnect(Services.Services.Instance.MapperService.Mapper.Map<Database.Service.Dto.User>(AuthentificationService.CurrentUser));
                  AuthentificationService.CurrentUser = null;
                  OnPropertyChanged(nameof(Login));
                  Connection();
              },
              () => { return true; }));
            }
        }

        private void Connection()
        {
            if (!AuthentificationService.Logon("admin"))
            {
                ShutdownService.ShutdownApp();
            }
            else
            {
                OnPropertyChanged(nameof(Login));
                Services.Services.Instance.NavigationService.NavigateTo(Services.NavNameEnum.MainMenu);
            }
        }
    }

}
