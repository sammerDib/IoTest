using System.Collections.Generic;
using System.Linq;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Modules.TestAlgo.ViewModel;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Tools;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.Modules.TestAlgo
{
    public class TestAlgoVM : ObservableObject, IMenuContentViewModel
    {
        public List<AlgoBaseVM> Algos { get; private set; }

        private AlgoBaseVM _selectedAlgo;
        private ProbesSupervisor _probeSupervisor;
        private CamerasSupervisor _camerasSupervisor;

        public ProbeLiseVM ProbeLise => (GetCurrentProbe() is ProbeLiseVM probeLise) ? probeLise : null;

        private ProbeBaseVM GetCurrentProbe()
        {
            var probes = _probeSupervisor.Probes;
            var position = _camerasSupervisor.Camera.Configuration.ModulePosition;
            var probeLise = probes.FirstOrDefault(p => (p is ProbeLiseVM) && (p as ProbeLiseVM).Configuration.ModulePosition == position);
            return probeLise;
        }

        public AlgoBaseVM SelectedAlgo
        {
            get => _selectedAlgo;
            set
            {
                if (_selectedAlgo != value)
                {
                    _selectedAlgo = value;

                    if (SelectedAlgo is PatternRecVM)
                    {
                        IsRoiSelectorVisible = true;
                    }
                    else
                    {
                        IsRoiSelectorVisible = false;
                    }
                    _selectedAlgo.RoiRect = RoiRect;
                    _selectedAlgo.IsCenteredROI = IsCenteredROI;

                    OnPropertyChanged();
                }
            }
        }

        public TestAlgoVM()
        {
            Algos = new List<AlgoBaseVM>
            {
                new AFLiseCameraVM(),
                new AutolightVM(),
                new PatternRecVM(),
                new BwaVM()
            };
            SelectedAlgo = Algos.First();
            _probeSupervisor = ClassLocator.Default.GetInstance<ProbesSupervisor>();
            _camerasSupervisor = ClassLocator.Default.GetInstance<CamerasSupervisor>();
        }

        public bool IsEnabled => true;

        public bool CanClose()
        {
            bool canClose = true;
            foreach (AlgoBaseVM algo in Algos)
            {
                if (algo is INavigable navigableAlgo)
                    canClose &= navigableAlgo.CanLeave(null);
            }
            if (canClose)
            {
                _camerasSupervisor.ApplyObjectiveOffset = true;
                _camerasSupervisor.ObjectiveChangedEvent -= CamerasSupervisor_ObjectiveChangedEvent;
                _camerasSupervisor.StopAllStreaming();
                ProbeLise.IsAcquiring = false;
                foreach (var cameraVM in Algos.OfType<AFLiseCameraVM>())
                {
                    if (cameraVM.AutoFocusSettings != null)
                    {
                        cameraVM.AutoFocusSettings.PropertyChanged -= cameraVM.AutoFocusType_PropertyChanged;
                    }
                }
                Algos.ForEach(algo => algo.Dispose());
            }

            return true;
        }

        public void Refresh()
        {
            _camerasSupervisor.ApplyObjectiveOffset = true;
            ProbeLise.IsAcquiring = true;
            _camerasSupervisor.ObjectiveChangedEvent += CamerasSupervisor_ObjectiveChangedEvent;
            foreach (var cameraVM in Algos.OfType<AFLiseCameraVM>())

            {
                if (cameraVM.AutoFocusSettings != null)
                {
                    cameraVM.AutoFocusSettings.PropertyChanged -= cameraVM.AutoFocusType_PropertyChanged;
                    cameraVM.AutoFocusSettings.PropertyChanged += cameraVM.AutoFocusType_PropertyChanged;
                }
            }
        }

        private void CamerasSupervisor_ObjectiveChangedEvent(string objectiveID)
        {
            OnPropertyChanged(nameof(ProbeLise));
        }

        private Rect _roiRect = Rect.Empty;

        public Rect RoiRect
        {
            get
            {
                if (_roiRect == Rect.Empty)
                    _roiRect = new Rect(0, 0, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Width, ServiceLocator.CamerasSupervisor.Camera.CameraInfo.Height);

                return _roiRect;
            }

            set { if (_roiRect != value) { _roiRect = value; SelectedAlgo.RoiRect = _roiRect; OnPropertyChanged(); } }
        }

        public bool _isCenteredROI = false;

        public bool IsCenteredROI
        {
            get => _isCenteredROI;
            set
            {
                if (_isCenteredROI != value)
                {
                    _isCenteredROI = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool _isRoiSelectorVisible = false;

        public bool IsRoiSelectorVisible
        {
            get => _isRoiSelectorVisible;
            set
            {
                if (_isRoiSelectorVisible != value)
                {
                    _isRoiSelectorVisible = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions>
            {
                SpecificPositions.PositionChuckCenter,
                SpecificPositions.PositionHome,
                SpecificPositions.PositionManualLoad,
                SpecificPositions.PositionPark
            };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionChuckCenter;
        }

    }
}
