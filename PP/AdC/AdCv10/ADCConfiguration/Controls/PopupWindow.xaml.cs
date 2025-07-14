using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADCConfiguration.Controls
{
    /// <summary>
    /// Logique d'interaction pour PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window, INotifyPropertyChanged
    {
        public PopupWindow()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            DataContext = this;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                if (OKButtonCommand != null)
                    if (OKButtonCommand.CanExecute(null))
                        OKButtonCommand.Execute(null);
            }

            if (e.Key == Key.Escape)
            {
                if (CancelButtonCommand != null)
                    if (CancelButtonCommand.CanExecute(null))
                        CancelButtonCommand.Execute(null);
            }



        }


        private bool _warningVisible;
        public bool WarningVisible
        {
            get => _warningVisible; set { if (_warningVisible != value) { _warningVisible = value; OnPropertyChanged(); } }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion INotifyPropertyChanged


        private string _popupTitle = "";
        public string PopupTitle { get => _popupTitle; set { _popupTitle = value; OnPropertyChanged(); } }


        private ObservableRecipient _viewModel = null;
        public ObservableRecipient ViewModel { get => _viewModel; set { _viewModel = value; OnPropertyChanged(); } }



        private string _okButtonText = string.Empty;
        public string OKButtonText { get => _okButtonText; set { _okButtonText = value; OnPropertyChanged(); } }

        public bool OKButtonVisible { get { return !string.IsNullOrEmpty(OKButtonText); } }

        private AutoRelayCommand _okButtonViewModelCommand = null;
        public AutoRelayCommand OKButtonViewModelCommand { get => _okButtonViewModelCommand; set => _okButtonViewModelCommand = value; }


        private string _cancelButtonText = "Cancel";
        public string CancelButtonText { get => _cancelButtonText; set { _cancelButtonText = value; OnPropertyChanged(); } }

        private AutoRelayCommand _cancelButtonViewModelCommand = null;
        public AutoRelayCommand CancelButtonViewModelCommand { get => _cancelButtonViewModelCommand; set => _cancelButtonViewModelCommand = value; }


        /// <summary>
        /// Fonction de validation pour le Bouton OK, renvoyer True pour valider et fermer la fenetre.
        /// </summary>
        public Func<bool> OKButtonValidateFunction = null;
        private AutoRelayCommand _okButtonCommand = null;
        public AutoRelayCommand OKButtonCommand
        {
            get
            {
                return _okButtonCommand ?? (_okButtonCommand = new AutoRelayCommand(
            () =>
            {
                bool canClose = true;

                if (OKButtonViewModelCommand != null)
                {
                    OKButtonViewModelCommand.Execute(null);
                }

                if (OKButtonValidateFunction != null)
                {
                    canClose = OKButtonValidateFunction();
                }

                if (canClose)
                {
                    // on ferme la fenetre 
                    DialogResult = true;
                    Close();
                }
            },
            () =>
            {
                if (OKButtonViewModelCommand != null)
                {
                    return OKButtonViewModelCommand.CanExecute(null);
                }
                return true;
            }));
            }
        }

        private AutoRelayCommand _cancelButtonCommand = null;
        public AutoRelayCommand CancelButtonCommand
        {
            get
            {
                return _cancelButtonCommand ?? (_cancelButtonCommand = new AutoRelayCommand(
            () =>
            {
                if (CancelButtonViewModelCommand != null)
                {
                    CancelButtonViewModelCommand.Execute(null);
                }

                // on ferme la fenetre 
                DialogResult = false;
                Close();
            },
            () =>
            {
                if (CancelButtonViewModelCommand != null)
                {
                    return CancelButtonViewModelCommand.CanExecute(null);
                }

                return true;
            }));
            }
        }


    }



    internal class MessageBoxViewModel : ObservableRecipient
    {

        private string _text = "";

        public string Text { get => _text; set { _text = value; OnPropertyChanged(); } }
    }

}
