using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Windows.Navigation;

using Agileo.Common.Configuration;
using Agileo.Common.Localization;
using Agileo.GUI;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Configuration;
using Agileo.GUI.Properties;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Vendor.Views.Panels.Help.About.SupportRequest;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.About
{
    /// <summary>
    /// About
    /// </summary>
    public class About : BusinessPanel
    {
        #region Fields

        private CompanyData _customer;

        private CompanyData _supplier;

        private readonly BusyIndicator _busyIndicator;

        #endregion

        public SupportRequestManager SupportRequestManager { get; }

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="About"/> class.
        /// </summary>
        static About()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(AboutResources)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="About"/> class.
        /// </summary>
        public About() : this(nameof(Agileo.GUI.Properties.Resources.S_HELP_ABOUT), PathIcon.About)
        {
        }

        public About(string id, IIcon icon = null) : base(id, icon)
        {
            _busyIndicator = new BusyIndicator(nameof(AboutResources.SUPPORT_REQUEST_CREATION)){IsIndeterminate = true};

            SupportRequestManager = new SupportRequestManager();
            SupportRequestManager.PropertyChanged += ReportManager_PropertyChanged;
            SupportRequestManager.ArchiveComplete += ReportManager_ArchiveComplete;

            Commands.Add(new BusinessPanelCommand(nameof(Agileo.GUI.Properties.Resources.S_HELP_ABOUT_REPORT), SupportRequestManager.SendReportCommand, PathIcon.Report));

            if (IsInDesignMode)
            {
                Supplier = new CompanyData
                {
                    Address = @"11, rue Victor Grignard
            86000 POITIERS - France",
                    Name = "Agileo Automation",
                    WebMailContact = "contact@agileo-automation.com ",
                    WebSite = "http://www.agileo-automation.com/en/",
                    Description = @"
Explore the World of Equipment Controllers.

Agileo Automation specializes in industrial automation with a high expertise in software services for semiconductor and photovoltaic equipment manufacturers."
                };
            }
            else
            {
                _supplier = new CompanyData();

                LocalizationManager.Instance.CultureChanged += LocalizationManager_CultureChanged;
            }
        }

        private void ReportManager_ArchiveComplete(object sender, ArchiveCompleteEventArgs e)
        {
            if (e.DoesArchiveExists && e.IsArchiveComplete)
            {
                Messages.Show(CreateFileSuccessMessage(e));
            }
            else if (!e.DoesArchiveExists || e.ExceptionThrown)
            {
                Messages.Show(new UserMessage(MessageLevel.Error, nameof(InfoStringResources.I_ABOUT_SUPPORT_RQ_NOT_CREATED))
                { CanUserCloseMessage = true });
            }
            else
            {
                Messages.Show(new UserMessage(MessageLevel.Error, nameof(InfoStringResources.I_ABOUT_SUPPORT_RQ_NOK))
                { CanUserCloseMessage = true });
            }
        }

        private void ReportManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SupportRequestManager.IsExecuting))
            {
                if (SupportRequestManager.IsExecuting)
                {
                    Messages.HideAll();
                    Popups.Show(_busyIndicator);
                }
                else
                {
                    _busyIndicator.Close();
                }
            }

            if (e.PropertyName == nameof(SupportRequestManager.ZipProgression))
            {
                _busyIndicator.IsIndeterminate = false;
                _busyIndicator.Value = SupportRequestManager.ZipProgression;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Data related to the customer
        /// (could be the end user or Agileo's customer).
        /// </summary>
        public CompanyData Customer
        {
            get { return _customer; }
            set
            {
                _customer = value;
                OnPropertyChanged(nameof(Customer));
            }
        }

        /// <summary>
        /// Data related to Agileo Automation
        /// (could be data related to Agileo's customer, if required).
        /// </summary>
        public CompanyData Supplier
        {
            get { return _supplier; }
            set
            {
                _supplier = value;
                OnPropertyChanged(nameof(Supplier));
            }
        }

        #endregion

        #region event Handlers

        /// <summary>
        /// Handles the RequestNavigate event of the Hyperlink control.
        /// MUST BE PUBLIC BECAUSE it is a method called by view
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RequestNavigateEventArgs"/> instance containing the event data.</param>
        public void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(
                e.Uri.OriginalString.Contains('@') ? "mailto:" + e.Uri.OriginalString : e.Uri.OriginalString));
            e.Handled = true;
        }

        private void LocalizationManager_CultureChanged(object sender, EventArgs e)
        {
            Supplier.Description = Agileo.GUI.Properties.Resources.S_HELP_ABOUT_COMPANY_DESC;
        }

        #endregion event Handlers

        #region Private
        private UserMessage CreateFileSuccessMessage(ArchiveCompleteEventArgs e)
        {
            var successMessage = new UserMessage(MessageLevel.Success, e.Message);
            successMessage.Commands.Add(new UserMessageCommand(nameof(AboutResources.OPEN_FOLDER), new DelegateCommand(() =>
            {
                var uiConfig = AgilControllerApplication.Current.ConfigurationManager.Current as AgileoGuiConfiguration;
                if (uiConfig != null)
                    Process.Start(uiConfig.Diagnostics.ExportPath);
                else
                    Messages.Show(new UserMessage(MessageLevel.Error, nameof(AboutResources.ERROR_OPENING_FOLDER))
                    { CanUserCloseMessage = true });
            })));

            successMessage.CanUserCloseMessage = true;

            return successMessage;
        }

        #endregion Private

        #region IInstanciable

        public override void OnSetup()
        {
            if (IsInDesignMode) return;

            if (AgilControllerApplication.Current.ConfigurationManager == null)
            {
                throw new ConfigurationErrorsException("ConfigurationManager is null in AgilControllerApplication.Current");
            }

            UpdateConfiguration();
            AgilControllerApplication.Current.ConfigurationManager.CurrentChanged += ConfigurationManager_CurrentChanged;
        }

        private void ConfigurationManager_CurrentChanged(object sender, ConfigurationChangedEventArgs e)
        {
            UpdateConfiguration();
        }

        private void UpdateConfiguration()
        {
            if (App.Instance.Config == null)
            {
                throw new InvalidOperationException(
                    "ConfigurationManager.Current is null or is not a IAgileoUIConfig in App.Instance");
            }

            SupportRequestManager.Setup(
                App.Instance.Config.Diagnostics.FilesOrFolders,
                App.Instance.Config.Diagnostics.ExportPath,
                App.Instance.Config.Diagnostics.TracingConfig.FilePaths.TracesPath);
        }

        protected override void Dispose(bool disposing)
        {
            SupportRequestManager.PropertyChanged -= ReportManager_PropertyChanged;
            SupportRequestManager.ArchiveComplete -= ReportManager_ArchiveComplete;
            AgilControllerApplication.Current.ConfigurationManager.CurrentChanged -= ConfigurationManager_CurrentChanged;
            base.Dispose(disposing);
        }

        #endregion IInstanciable
    }

    /// <summary>
    /// Represent data of the company like name, address..
    /// </summary>
    public class CompanyData : Notifier
    {
        #region Properties

        private string _address;

        private string _description;

        private string _name;

        private string _phoneNumber;

        private string _webMailContact;

        private string _webSite;

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        /// <summary>
        /// Summary of the company's activities, or company's slogan, etc.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        /// <summary>
        /// Webmail to contact the company (e.g. for requesting support or additional information).
        /// </summary>
        public string WebMailContact
        {
            get { return _webMailContact; }
            set
            {
                _webMailContact = value;
                OnPropertyChanged(nameof(WebMailContact));
            }
        }

        /// <summary>
        /// Gets or sets the web site.
        /// </summary>
        /// <value>
        /// The web site.
        /// </value>
        public string WebSite
        {
            get { return _webSite; }
            set
            {
                _webSite = value;
                OnPropertyChanged(nameof(WebSite));
            }
        }

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyData"/> class.
        /// </summary>
        public CompanyData()
        {
            _address =
@"11 Rue Victor Grignard
86000 POITIERS";
            _description = Agileo.GUI.Properties.Resources.S_HELP_ABOUT_COMPANY_DESC;
            _name = "Agileo Automation";
            _phoneNumber = "+33 5 49 49 61 79";
            _webMailContact = "contact@agileo-automation.com";
            _webSite = "www.agileo.com";
        }
    }
}
