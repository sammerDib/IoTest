using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.PM.Shared.Data.Image;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.DMT.Hardware.ClientProxy.Screen
{
    public class ScreenVM : ObservableObject
    {
        private ScreenSupervisor _supervisor;
        private string _deviceId;

        public ScreenVM()
        {
            if (!DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                throw new ApplicationException("This constructor is for design mode only.");
        }

        public ScreenVM(ScreenSupervisor supervisor, string deviceId)
        {
            _supervisor = supervisor;
            _deviceId = deviceId;

            Messenger.Register<ScreenImageChangedMessage>(this, (r, m) =>
                {
                    if (m.ScreenId != _deviceId)
                        return;
                    Application.Current?.Dispatcher.BeginInvoke((Delegate)(() =>
                    {
                        ScreenColor = Color.FromRgb(m.Color.R, m.Color.G, m.Color.B);
                        ScreenBrush = new SolidColorBrush(ScreenColor);
                        if (m.ServiceImage == null)
                            ScreenBitmapSource = null;
                        else
                            ScreenBitmapSource = m.ImageSource;
                    }));
                });
        }

        private delegate void Delegate();

        private Color _screenColor = Colors.Black;

        public Color ScreenColor
        {
            get => _screenColor;
            set
            {
                if (_screenColor != value)
                {
                    _screenColor = value;
                   
                    Color c = Color.FromRgb( _screenColor.R, _screenColor.G, _screenColor.B);
                    OnPropertyChanged();
                    Task.Run(() => _supervisor.SetScreenColor(_deviceId, c));
                }
            }
        }

        private Brush _screenBrush = Brushes.SlateGray;

        public Brush ScreenBrush
        {
            get => _screenBrush; set { if (_screenBrush != value) { _screenBrush = value; OnPropertyChanged(); } }
        }

        private BitmapSource _screenBitmapSource;

        public BitmapSource ScreenBitmapSource
        {
            get => _screenBitmapSource; set { if (_screenBitmapSource != value) { _screenBitmapSource = value; OnPropertyChanged(); } }
        }

        //=================================================================
        // Commandes
        //=================================================================

        #region Commandes

        private OpenFileDialogSettings _fileDialogSettings;
        private AutoRelayCommand _loadImageCommand;

        public AutoRelayCommand LoadImageCommand
        {
            get
            {
                return _loadImageCommand ?? (_loadImageCommand = new AutoRelayCommand(
                    () =>
                    {
                        _fileDialogSettings = _fileDialogSettings ?? new OpenFileDialogSettings()
                        {
                            Title = "Select image to display",
                            Filter = "Images|*.tif;*.tiff;*.bmp|BMP Files (*.bmp)|*.bmp|TIFF Files (*.tif *.tiff)|*.tif;*.tiff|All files (*.*)|*.*"
                        };
                        bool? res = DialogService.ShowOpenFileDialog(_fileDialogSettings);
                        if (res == true)
                        {
                            ServiceImage img = new ServiceImage();
                            img.LoadFromFile(_fileDialogSettings.FileName);
                            Task.Run(() => _supervisor.DisplayImage(_deviceId, img));
                        }
                    }));
            }
        }

        private AutoRelayCommand<bool> _livePreviewCommand;

        public AutoRelayCommand<bool> LivePreviewCommand
        {
            get
            {
                return _livePreviewCommand ?? (_livePreviewCommand = new AutoRelayCommand<bool>(
                    isChecked =>
                    {
                        if (isChecked)
                            _supervisor.Subscribe();
                        else
                            _supervisor.Unsubscribe();
                    }));
            }
        }

        #endregion Commandes

        //=================================================================
        // Utilitaires
        //=================================================================

        #region Utilitaires

        private IMessenger _messenger;

        public IMessenger Messenger
        {
            get
            {
                if (_messenger == null)
                    _messenger = ClassLocator.Default.GetInstance<IMessenger>();
                return _messenger;
            }
        }

        private IDialogOwnerService _dialogService;

        public IDialogOwnerService DialogService
        {
            get
            {
                if (_dialogService == null)
                    _dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();

                return _dialogService;
            }
        }

        #endregion Utilitaires
    }
}
