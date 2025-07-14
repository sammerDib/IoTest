using System.Threading.Tasks;

using UnitySC.PM.DMT.CommonUI;
using UnitySC.PM.DMT.CommonUI.ViewModel;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes;
using UnitySC.PM.Shared.Hardware.ClientProxy.Axes.ViewModel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.PM.DMT.Client.Modules.TestHardware.ViewModel
{
    public class StageViewModel : ViewModelBaseExt, ITabManager
    {
        private const int MotionEndTimeout = 5000;
        private const string LinearAxisID = "Linear";

        #region Properties

        public string Header { get; protected set; }

        private readonly MotionAxesSupervisor _motionAxesSupervisor;

        public MotionAxesVM MotionAxesVM => _motionAxesSupervisor.MotionAxesVM;

        public bool IsEnabled { get; protected set; } = false;

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _positionState;

        public string PositionState
        {
            get => _positionState; set { if (_positionState != value) { _positionState = value; OnPropertyChanged(); } }
        }

        private bool _isinMeasurementPosition = false;

        public bool IsinMeasurementPosition
        {
            get => _isinMeasurementPosition; set { if (_isinMeasurementPosition != value) { _isinMeasurementPosition = value; OnPropertyChanged(); } }
        }

        private bool _isLoadPosition = false;

        public bool IsLoadPosition
        {
            get => _isLoadPosition; set { if (_isLoadPosition != value) { _isLoadPosition = value; OnPropertyChanged(); } }
        }

        #endregion Properties

        public StageViewModel(MotionAxesSupervisor motionAxesSupervisor)
        {
            _motionAxesSupervisor = motionAxesSupervisor;

            Header = "Stage";
            Init();
        }

        public void Init()
        {
            Header = "Stage";
            Task.Run(() => _motionAxesSupervisor.TriggerUpdateEvent());
        }

        private DMTChuckPosition _chuckPosition;

        public DMTChuckPosition ChuckPosition
        {
            get => _chuckPosition;
            set
            {
                if (_chuckPosition != value)
                {
                    _chuckPosition = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(CanMoveToMeasurementPosition));
                    MoveToMeasurementPositionCommand.NotifyCanExecuteChanged();
                    MoveToLoadingPositionCommand.NotifyCanExecuteChanged();
                }
            }
        }

        public bool CanMoveToMeasurementPosition => ChuckPosition != DMTChuckPosition.Process;

        private AutoRelayCommand _moveToMeasurementPositionCommand;

        public AutoRelayCommand MoveToMeasurementPositionCommand
        {
            get
            {
                return _moveToMeasurementPositionCommand ?? (_moveToMeasurementPositionCommand = new AutoRelayCommand(() =>
                {
                    IsinMeasurementPosition = true;
                    MoveToPosition(DMTChuckPosition.Process);
                },
                () => CanMoveToMeasurementPosition));
            }
        }

        private AutoRelayCommand _moveToLoadingPositionCommand;

        public AutoRelayCommand MoveToLoadingPositionCommand
        {
            get
            {
                return _moveToLoadingPositionCommand ?? (_moveToLoadingPositionCommand = new AutoRelayCommand(() =>
                {
                    IsLoadPosition = true;
                    MoveToPosition(DMTChuckPosition.Loading);
                },
                () => !CanMoveToMeasurementPosition));
            }
        }

        private void MoveToPosition(DMTChuckPosition destination)
        {
            if (ChuckPosition != destination)
            {
                var position = new Length((double)destination, LengthUnit.Millimeter);
                var move = new PMAxisMove(LinearAxisID, position);
                _motionAxesSupervisor.Move(move);
                _motionAxesSupervisor.WaitMotionEnd(MotionEndTimeout);
                ChuckPosition = destination;
            }
        }

        public void Display()
        {
            var currentPosition = (XTPosition)_motionAxesSupervisor.GetCurrentPosition().Result;
            if (currentPosition != null)
            {
                ChuckPosition = (DMTChuckPosition)(int)currentPosition.X;
            }
        }

        public bool CanHide()
        {
            return true;
        }

        public void Hide()
        {
        }
    }
}
