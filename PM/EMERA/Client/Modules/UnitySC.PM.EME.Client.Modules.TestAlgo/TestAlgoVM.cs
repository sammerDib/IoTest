using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Controls.Camera;
using UnitySC.PM.EME.Client.Modules.TestAlgo.ViewModel;
using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Modules.TestAlgo
{
    public class TestAlgoVM : ObservableObject, IMenuContentViewModel, IDisposable
    {
        private readonly CameraBench _cameraBench;
        private readonly IMessenger _messenger;

        public TestAlgoVM()
        {
            _cameraBench = ClassLocator.Default.GetInstance<CameraBench>();
            _messenger = ClassLocator.Default.GetInstance<IMessenger>();          
            var filterWheelBench = ClassLocator.Default.GetInstance<FilterWheelBench>();
            Algos = new List<AlgoBaseVM> { new AutoFocusVM(), new PatternRecVM(), new VignettingVM(filterWheelBench), new PixelSizeComputationVM(), new DistortionVM(), new AutoExposureVM(filterWheelBench), new DistanceSensorCalibrationVM() };
            SelectedAlgo = Algos.FirstOrDefault();           
            Initialize();
        }
        private void Initialize()
        {
            StandardCameraViewModel.UseRoi = true;
            StandardCameraViewModel.PropertyChanged += StandardCameraViewModel_PropertyChanged;
            PropertyChanged += TestAlgoVM_PropertyChanged;
            OnPropertyChanged(nameof(Algos));
        }

        private void TestAlgoVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(RoiRect))
            {
                StandardCameraViewModel.RoiRect = RoiRect;
            }
        }

        private void StandardCameraViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(StandardCameraViewModel.RoiRect))
            {
                RoiRect = StandardCameraViewModel.RoiRect;
            }
        }

        public List<AlgoBaseVM> Algos { get; private set; }

        private AlgoBaseVM _selectedAlgo;
        public AlgoBaseVM SelectedAlgo
        {
            get => _selectedAlgo;
            set
            {
                if (_selectedAlgo != value)
                {
                    _selectedAlgo = value;
                    if (_selectedAlgo.RoiRect == Rect.Empty)
                    {
                        _selectedAlgo.RoiRect = StandardCameraViewModel.DefaultFullRoi;
                        RoiRect = _selectedAlgo.RoiRect;
                        OnPropertyChanged();
                    }
                    else
                    {
                        RoiRect = _selectedAlgo.RoiRect;
                        OnPropertyChanged();
                    }
                }
            }
        }

        private Rect _roiRect = Rect.Empty;

        public Rect RoiRect
        {
            get
            {
                if (_roiRect == Rect.Empty)
                    _roiRect = StandardCameraViewModel.DefaultFullRoi;

                return _roiRect;
            }

            set { if (_roiRect != value) { _roiRect = value; SelectedAlgo.RoiRect = _roiRect; OnPropertyChanged(); } }
        }

        private StandardCameraViewModel _standardCameraViewModel;
        public StandardCameraViewModel StandardCameraViewModel
        {
            get
            {
                _standardCameraViewModel = _standardCameraViewModel
                    ?? new StandardCameraViewModel(_cameraBench, ClassLocator.Default.GetInstance<IMessenger>());
                return _standardCameraViewModel;
            }
        }

        public bool IsEnabled => true;

        private bool _canCloseState = true;

        private bool CanCloseState
        {
            get => _canCloseState;
            set => SetProperty(ref _canCloseState, value);
        }

        public bool CanClose()
        {
            foreach (AlgoBaseVM algo in Algos)
            {
                if (algo is INavigable navigableAlgo)
                    CanCloseState &= navigableAlgo.CanLeave(null);
            }

            if (CanCloseState)
            {
                Cleanup();
            }
            else
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Can't close");
            }
            StandardCameraViewModel?.Dispose();
            return CanCloseState;
        }
        public void Refresh()
        {
            Initialize();
            StandardCameraViewModel.Refresh();

        }        
        private void Cleanup()
        {
            StandardCameraViewModel.PropertyChanged -= StandardCameraViewModel_PropertyChanged;
            PropertyChanged -= TestAlgoVM_PropertyChanged;
            Algos.ForEach(algo => algo.Dispose());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
        private void Dispose(bool manualDisposing)
        {
            if (manualDisposing)
            {
                Cleanup();
            }
        }
    }
}
