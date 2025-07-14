using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Image;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.Shared.Hardware.ClientProxy.Camera
{
    public class CameraVM : ObservableRecipient
    {
        private ICameraService _cameraSupervisor;

        public ICameraService CameraSupervisor
        {
            get
            {
                if (_cameraSupervisor == null)
                    _cameraSupervisor = ClassLocator.Default.GetInstance<CameraSupervisor>();

                return _cameraSupervisor;
            }
        }

        private readonly string _deviceId;

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            StopContinuousGrab();
        }

        //=================================================================

        #region Constructeurs

        //=================================================================

        public CameraVM(ICameraService supervisor, string deviceId)
        {
            _cameraSupervisor = supervisor;
            _deviceId = deviceId;
        }

        #endregion Constructeurs

        //=================================================================

        #region Propriétés bindables

        //=================================================================
        private int _imageCount;

        public int ImageCount
        {
            get => _imageCount; set { if (_imageCount != value) { _imageCount = value; OnPropertyChanged(); } }
        }

        private BitmapSource _cameraBitmapSource;

        public BitmapSource CameraBitmapSource
        {
            get => _cameraBitmapSource; set { if (_cameraBitmapSource != value) { _cameraBitmapSource = value; OnPropertyChanged(); } }
        }

        #endregion Propriétés bindables

        //=================================================================

        #region Commandes WPF

        //=================================================================
        private AutoRelayCommand<bool> _videoGrabCommand;

        public AutoRelayCommand<bool> VideoGrabCommand
        {
            get
            {
                return _videoGrabCommand ?? (_videoGrabCommand = new AutoRelayCommand<bool>(
                    isChecked =>
                    {
                        if (isChecked)
                            StartContinuousGrab();
                        else
                            StopContinuousGrab();
                    }));
            }
        }

        private AutoRelayCommand _singleGrabCommand;

        public AutoRelayCommand SingleGrabCommand
        {
            get
            {
                return _singleGrabCommand ?? (_singleGrabCommand = new AutoRelayCommand(() =>
                    {
                        //TODO: Verify the implementation
                        ServiceImage svcimg = _cameraSupervisor.GetCameraImage(_deviceId)?.Result;
                        CameraBitmapSource = svcimg?.WpfBitmapSource;
                        if (CameraBitmapSource != null)
                            ImageCount++;
                    },
                    () => false));
            }
        }

        #endregion Commandes WPF

        //=================================================================

        #region Gestion du Grab

        //=================================================================
        private bool _isGrabbing;

        private void StartContinuousGrab()
        {
            _isGrabbing = true;
            _cameraSupervisor.StartAcquisition(_deviceId);
            Task.Run(() => GrabTask());
        }

        private void StopContinuousGrab()
        {
            if (_isGrabbing)
            {
                _cameraSupervisor.StopAcquisition(_deviceId);
                _isGrabbing = false;
            }
        }

        private void GrabTask()
        {
            // Acquisition
            //............
            if (!_isGrabbing)
                return;
            ServiceImage svcimage = _cameraSupervisor.GetCameraImage(_deviceId)?.Result;
            if (svcimage == null)
            {
                Thread.Sleep(100);
                Task.Run(() => GrabTask());
            }
            else
            {
                var app = Application.Current;
                if (app == null)
                    _isGrabbing = false;    // bidouille pour arrêter le thread quand l'application est fermée

                if (_isGrabbing)
                    app.Dispatcher.Invoke(new Action(() =>
                    {
                        if (!_isGrabbing)   // Ne pas mettre à jour l'IHM si on est en train de fermer la vue
                            return;

                        CameraBitmapSource = svcimage.WpfBitmapSource;
                        ImageCount++;

                        // On relance la tâche de grab
                        //............................
                        // On relance seulement après que l'image est affichée
                        Task.Run(() => GrabTask());
                    }));
            }
        }

        #endregion Gestion du Grab
    }
}
