using System;
using System.Windows.Input;
using System.Windows.Threading;

using Agileo.Common.Access.Users;
using Agileo.Common.Localization;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

namespace UnitySC.GUI.Common.Vendor.Views.TitlePanel
{
    /// <summary>
    /// TitlePanel ViewModel
    /// </summary>
    public class TitlePanel : Agileo.GUI.Components.TitlePanel
    {
        private BusinessPanel _currentPanel;
        private readonly UserInterface _userInterface;
        private readonly DispatcherTimer _dateTimeTimer;

        /// <summary>
        /// Initializes the <see cref="TitlePanel"/> class.
        /// </summary>
        static TitlePanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(TitlePanelResources)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePanel"/> class.
        /// </summary>
        public TitlePanel()
            : this(null)
        {
            // Design Time data
            if (IsInDesignMode)
            {
                CurrentUserName = "SUPERVISOR";
                Messages = new UserMessageDisplayer();
                Messages.Show(new UserMessage("Design time message") { Level = MessageLevel.Info });
            }
            else
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public TitlePanel(UserInterface userInterface)
        {
            if (userInterface == null && !IsInDesignMode)
            {
                throw new ArgumentNullException(nameof(userInterface));
            }

            _userInterface = userInterface;

            // timer used to update DateTime
            _dateTimeTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _dateTimeTimer.Tick += timer_Tick;
            _dateTimeTimer.Start();
        }

        #region Event Handlers

        /// <summary>
        /// Method called when user logged off
        /// </summary>
        public virtual void AccessRights_UserLogoff(User user, UserEventArgs e)
        {
            CurrentUserName = string.Empty;
        }

        /// <summary>
        /// Method called when user logged on
        /// </summary>
        public virtual void AccessRights_UserLogon(User user, UserEventArgs e)
        {
            CurrentUserName = user.Name;
        }

        private void Navigation_SelectedBusinessPanelChanged(object sender, BusinessPanel panel)
        {
            _currentPanel = panel;
            SelectedBusinessPanelId = GetLocalizedNavigationPath(_currentPanel);
        }

        private string GetLocalizedNavigationPath(BusinessPanel panel)
        {
            var parent = panel.Parent;
            var navigationPath = panel.LocalizedName;
            while (parent != null)
            {
                if (parent.RelativeId != "Root"
                    && parent.RelativeId != "Navigation"
                    && parent.RelativeId != "UserInterface")
                {
                    navigationPath = parent.LocalizedName + string.Concat(" - ", navigationPath);
                }

                parent = parent.Parent;
            }

            return navigationPath;
        }

        #endregion Event Handlers

        #region Properties

        /// <summary>
        /// Get the application name
        /// </summary>
        public string AppName => App.Instance?.ApplicationName;

        private string _currentDate;

        /// <summary>
        /// Gets or sets the current date.
        /// </summary>
        /// <value>
        /// The current date.
        /// </value>
        public string CurrentDate
        {
            get => _currentDate;
            set => SetAndRaiseIfChanged(ref _currentDate, value);
        }

        private string _currentTime;

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>
        /// The current time.
        /// </value>
        public string CurrentTime
        {
            get => _currentTime;
            set => SetAndRaiseIfChanged(ref _currentTime, value);
        }

        private string _currentUserName;

        /// <summary>
        /// Current User name displayed on logon button
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                if (!string.IsNullOrEmpty(_currentUserName))
                {
                    return _currentUserName;
                }

                var logOn = LocalizationManager.GetString("S_TITLE_LOGON");
                return string.IsNullOrEmpty(logOn) ? "Logon Here" : logOn;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _currentUserName = null;
                }
                else
                {
                    _currentUserName = value;
                }

                OnPropertyChanged(nameof(CurrentUserName));
            }
        }

        /// <summary>
        /// Get the userMessage manager of the titlePanel.
        /// In general it is the instance of <see cref="UserMessageDisplayer"/> of the <see cref="UserInterface"/>
        /// </summary>
        public UserMessageDisplayer Messages { get; private set; }

        /// <summary>
        /// Get the navigation path of current displayed view
        /// </summary>
        private string _selectedBusinessPanelId;

        public string SelectedBusinessPanelId
        {
            get => _selectedBusinessPanelId;
            private set => SetAndRaiseIfChanged(ref _selectedBusinessPanelId, value);
        }

        #endregion Properties

        #region Commands

        /// <summary>
        /// Open Login Panel Command
        /// </summary>
        public ICommand GoMainPageCommand { get; protected set; }

        #region OpenLoginPanel Command

        /// <summary>
        /// Open Login Panel Command
        /// </summary>
        public ICommand OpenLoginPanelCommand { get; protected set; }

        #endregion OpenLoginPanel Command

        #endregion Commands

        #region Methods

        private void timer_Tick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToLongTimeString();
            CurrentDate = DateTime.Now.ToShortDateString();
        }

        #endregion Methods

        #region EventHandlers

        public override void LocalizerCultureChanged(object localizationManager)
            => SelectedBusinessPanelId = GetLocalizedNavigationPath(_currentPanel);
        #endregion

        #region IInstanciable

        public override void OnSetup()
        {
            // Do not call AgilControllerApplication with design instance
            if (IsInDesignMode)
            {
                return;
            }

            _userInterface.Navigation.SelectedBusinessPanelChanged += Navigation_SelectedBusinessPanelChanged;

            // The messages displayed in the title panel are the application level messages
            Messages = AgilControllerApplication.Current.UserInterface.Messages;

            OpenLoginPanelCommand = _userInterface.OpenLoginPanelCommand;
            var mainPanel = _userInterface.Navigation.RootMenu.Items[0];
            GoMainPageCommand =
                new DelegateCommand(() => _userInterface.Navigation.NavigateTo(mainPanel.NavigationAddress));
            OnPropertyChanged(nameof(OpenLoginPanelCommand));
        }

        #endregion IInstanciable

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            _dateTimeTimer.Stop();
            _dateTimeTimer.Tick -= timer_Tick;
            _userInterface.Navigation.SelectedBusinessPanelChanged -= Navigation_SelectedBusinessPanelChanged;

            base.Dispose(disposing);
        }

        #endregion
    }
}
