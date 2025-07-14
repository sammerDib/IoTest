using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.GUI;
using Agileo.GUI.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Views.Panels.Help.WebVisu;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.WebVisu
{
    /// <summary>
    /// Template Model representing the ViewModel (Context) of the panel
    /// </summary>
    public class WebVisuPanel : BusinessPanel
    {
        private DelegateCommand _backCommand;
        private DelegateCommand _forwardCommand;
        private DelegateCommand _goCommand;
        private DelegateCommand _refreshCommand;

        static WebVisuPanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(WebVisuPanelResources)));
        }

        public WebVisuPanel()
            : this("DesignTime Constructor")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebVisu"/> class.
        /// </summary>
        /// <param name="relativeId">Graphical identifier of the View Model. Can be either a <seealso cref="string"/> either a localizable resource.</param>
        /// <param name="icon">Optional parameter used to define the representation of the panel inside the application.</param>
        public WebVisuPanel(string relativeId, IIcon icon = null)
            : base(relativeId, icon)
        {
            LinkList = new List<WebLink>();
        }

        private DelegateCommand<string> _goToCommand;

        public ICommand GoToCommand
            => _goToCommand ??= new DelegateCommand<string>(GoToCommandExecute, GoToCommandCanExecute);

        private bool GoToCommandCanExecute(string link) => !string.IsNullOrWhiteSpace(link);

        private void GoToCommandExecute(string link)
        {
            Address = link;
            GoCommandExecute();
        }

        public ICommand BackCommand
            => _backCommand ??= new DelegateCommand(BackCommandExecute, BackCommandCanExecute);

        private void BackCommandExecute() => BackRequired?.Invoke(this, EventArgs.Empty);

        public bool BackCommandCanExecute() => CanGoBack;

        public ICommand ForwardCommand
            => _forwardCommand ??= new DelegateCommand(ForwardCommandExecute, ForwardCommandCanExecute);

        private void ForwardCommandExecute() => ForwardRequired?.Invoke(this, EventArgs.Empty);

        public bool ForwardCommandCanExecute() => CanGoForward;

        public ICommand GoCommand
            => _goCommand ??= new DelegateCommand(GoCommandExecute);

        private void GoCommandExecute()
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                return;
            }

            NavigationRequired?.Invoke(this, EventArgs.Empty);
        }
        
        public ICommand RefreshCommand
            => _refreshCommand ??= new DelegateCommand(RefreshCommandExecute);

        private void RefreshCommandExecute() => RefreshRequired?.Invoke(this, EventArgs.Empty);
        
        private string _address;

        public string Address
        {
            get => _address;
            set => SetAndRaiseIfChanged(ref _address, value);
        }

        public bool CanGoBack { get; set; }

        public bool CanGoForward { get; set; }

        #region Events

        public event EventHandler NavigationRequired;

        public event EventHandler ForwardRequired;

        public event EventHandler BackRequired;

        public event EventHandler RefreshRequired;

        #endregion Events

        private Uri _internalSource;

        public Uri InternalSource
        {
            get => _internalSource;
            set
            {
                if (EqualityComparer<Uri>.Default.Equals(_internalSource, value))
                {
                    return;
                }

                _internalSource = value;
                Address = _internalSource.ToString();
            }
        }

        #region Explorer Visibility

        public Visibility ExplorerVisibility
        {
            get
            {
                if (IsInDesignMode)
                {
                    return Visibility.Visible;
                }

                if (Popups.Current != null)
                {
                    return Visibility.Hidden;
                }

                return AgilControllerApplication.Current.UserInterface.Popups.Current != null
                    ? Visibility.Hidden
                    : Visibility.Visible;
            }
        }

        private void Popups_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Popups.Current))
            {
                OnPropertyChanged(nameof(ExplorerVisibility));
            }
        }

        #endregion Explorer Visibility

        #region Override

        public override void OnSetup()
        {
            Popups.PropertyChanged += Popups_PropertyChanged;
            App.Instance.UserInterface.Popups.PropertyChanged += Popups_PropertyChanged;
            var firstLink = LinkList.FirstOrDefault();
            if (firstLink != null)
            {
                GoToCommandExecute(firstLink.Link);
            }

            base.OnSetup();
        }

        protected override void Dispose(bool disposing)
        {
            Popups.PropertyChanged -= Popups_PropertyChanged;
            App.Instance.UserInterface.Popups.PropertyChanged -= Popups_PropertyChanged;
            base.Dispose(disposing);
        }

        #endregion Override

        public List<WebLink> LinkList { get; }
    }
}
