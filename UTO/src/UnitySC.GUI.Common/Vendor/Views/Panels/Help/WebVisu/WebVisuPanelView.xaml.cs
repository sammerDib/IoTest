using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Help.WebVisu
{
    /// <summary>
    /// Interaction logic for BlankPanelView .xaml
    /// </summary>
    public partial class WebVisuPanelView
    {
        #region Constructor

        public WebVisuPanelView()
        {
            DataContextChanged += WebVisuAutomatePanelView_DataContextChanged;
            InitializeComponent();
            WebBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            WebBrowser.Navigating += WebBrowser_Navigating;
        }

        #endregion Constructor

        #region Properties

        private WebVisuPanel _viewModel;

        private WebVisuPanel ViewModel
        {
            get => _viewModel;
            set
            {
                if (_viewModel == value)
                {
                    return;
                }

                if (_viewModel != null)
                {
                    _viewModel.NavigationRequired -= ViewModel_NavigationRequired;
                    _viewModel.ForwardRequired -= ViewModel_ForwardRequired;
                    _viewModel.BackRequired -= ViewModel_BackRequired;
                    _viewModel.RefreshRequired -= ViewModel_RefreshRequired;
                }

                _viewModel = value;
                if (_viewModel != null)
                {
                    _viewModel.NavigationRequired += ViewModel_NavigationRequired;
                    _viewModel.ForwardRequired += ViewModel_ForwardRequired;
                    _viewModel.BackRequired += ViewModel_BackRequired;
                    _viewModel.RefreshRequired += ViewModel_RefreshRequired;
                    ShowInternalSource();
                }
            }
        }

        #endregion Properties

        #region EventHandlers

        private void WebBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (!e.Uri.ToString().StartsWith("http://") && !e.Uri.ToString().StartsWith("https://"))
            {
                return;
            }

            ViewModel.InternalSource = e.Uri;
        }

        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs navigationEventArgs)
        {
            if (ViewModel == null)
            {
                return;
            }

            ViewModel.CanGoBack = WebBrowser.CanGoBack;
            ViewModel.CanGoForward = WebBrowser.CanGoForward;
        }

        private void ViewModel_RefreshRequired(object sender, EventArgs e)
        {
            try
            {
                WebBrowser.Refresh();
            }
            catch (Exception)
            {
                ViewModel.Messages.Show(
                    new UserMessage(MessageLevel.Warning, nameof(WebVisuPanelResources.CANNOT_REFRESH_USER_MESSAGE))
                    {
                        SecondsDuration = 3
                    });
            }
        }

        private void ViewModel_ForwardRequired(object sender, EventArgs e) => WebBrowser.GoForward();

        private void ViewModel_BackRequired(object sender, EventArgs e) => WebBrowser.GoBack();

        private void ViewModel_NavigationRequired(object sender, EventArgs e)
        {
            try
            {
                var address = ViewModel.Address;
                if (!address.StartsWith("http://") && !address.StartsWith("https://"))
                {
                    address = "http://" + address;
                }
                WebBrowser.Navigate(address);

                // Suppress any script error popup
                HideJsScriptErrors();
            }
            catch (Exception)
            {
                ViewModel.Messages.Show(
                    new UserMessage(MessageLevel.Warning, nameof(WebVisuPanelResources.URL_NOT_FOUND))
                    {
                        SecondsDuration = 3
                    });
                _viewModel.CanGoBack = WebBrowser.CanGoBack;
                _viewModel.CanGoForward = WebBrowser.CanGoForward;
            }
        }

        private void WebVisuAutomatePanelView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = e.NewValue as WebVisuPanel;
            if (ViewModel != null && ViewModel.GoCommand.CanExecute(null))
            {
                ViewModel.GoCommand.Execute(null);
            }
        }

        private void AddressTextBox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel?.GoCommand.Execute(null);
            }
        }

        #endregion EventHandlers

        #region Private Methods

        private void ShowInternalSource()
        {
            try
            {
                if (ViewModel.InternalSource != null)
                {
                    WebBrowser.Navigate(ViewModel.InternalSource);
                }
            }
            catch (Exception)
            {
                ViewModel.Messages.Show(
                    new UserMessage(MessageLevel.Warning, nameof(WebVisuPanelResources.URL_NOT_FOUND))
                    {
                        SecondsDuration = 3
                    });
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "It is safe here, we only get the value")]
        private void HideJsScriptErrors()
        {
            // from "http://wpf-tutorial-net.blogspot.com/2013/11/disable-script-errors-webbrowser-wpf.html"

            // IWebBrowser2 interface
            // Exposes methods that are implemented by the WebBrowser control
            // Searches for the specified field, using the specified binding constraints.
            var fld = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);

            if (fld == null)
            {
                return;
            }

            var obj = fld.GetValue(WebBrowser);

            // Silent: Sets or gets a value that indicates whether the object can display dialog boxes.
            obj?.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, obj, new object[] { true });
        }

        #endregion Private Methods
    }
}
