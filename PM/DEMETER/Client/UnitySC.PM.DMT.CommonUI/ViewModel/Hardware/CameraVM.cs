using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Hardware.ClientProxy.Screen;
using UnitySC.PM.Shared.Hardware.ClientProxy.Camera;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using CameraSupervisor = UnitySC.PM.DMT.Shared.UI.Proxy.CameraSupervisor;

namespace UnitySC.PM.DMT.CommonUI.ViewModel.Hardware
{
    public class CameraVM : ObservableRecipient, IRecipient<ImageGrabbedMessage>, IRecipient<ScreenImageChangedMessage>
    {
        private readonly CameraSupervisor _cameraSupervisor;

        public CameraVM(CameraSupervisor cameraSupervisor)
        {
            _cameraSupervisor = cameraSupervisor;
            Messenger.RegisterAll(this);
        }

        private Brush _screenColor = Brushes.SlateGray;

        public Brush ScreenColor
        {
            get => _screenColor; set { if (_screenColor != value) { _screenColor = value; OnPropertyChanged(); } }
        }

        private BitmapSource _screenBitmapSource;

        public BitmapSource ScreenBitmapSource
        {
            get => _screenBitmapSource; set { if (_screenBitmapSource != value) { _screenBitmapSource = value; OnPropertyChanged(); } }
        }

        private BitmapSource _cameraBitmapSource;

        public BitmapSource CameraBitmapSource
        {
            get => _cameraBitmapSource; set { if (_cameraBitmapSource != value) { _cameraBitmapSource = value; OnPropertyChanged(); } }
        }

        private long _imageWidth;

        public long ImageWidth
        {
            get => Math.Max(10, _imageWidth); set { if (_imageWidth != value) { _imageWidth = value; OnPropertyChanged(); } }
        }

        private long _imageHeight;

        public long ImageHeight
        {
            get => Math.Max(10, _imageHeight); set { if (_imageHeight != value) { _imageHeight = value; OnPropertyChanged(); } }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked; set { if (_isChecked != value) { _isChecked = value; OnPropertyChanged(); } }
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
                            _cameraSupervisor.Subscribe();
                        else
                            _cameraSupervisor.Unsubscribe();
                    }));
            }
        }

        public void Receive(ImageGrabbedMessage message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                CameraBitmapSource = message.ServiceImage.WpfBitmapSource;
                ImageWidth = CameraBitmapSource.PixelWidth;
                ImageHeight = CameraBitmapSource.PixelHeight;
            });
        }

        public void Receive(ScreenImageChangedMessage message)
        {
            Application.Current?.Dispatcher.Invoke(() =>
            {
                ScreenColor = new SolidColorBrush(Color.FromRgb(message.Color.R, message.Color.G, message.Color.B));
                if (message.ServiceImage == null)
                    ScreenBitmapSource = null;
                else
                    ScreenBitmapSource = message.ImageSource;
            });
        }
    }
}
