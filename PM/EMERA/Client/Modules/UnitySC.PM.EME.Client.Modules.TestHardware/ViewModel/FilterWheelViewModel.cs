using System.Collections.Generic;
using System.ComponentModel;

using CommunityToolkit.Mvvm.Input;

using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.EME.Client.Modules.TestHardware.ViewModel
{
    public class FilterWheelViewModel : TabViewModelBase
    {
        public FilterWheelViewModel(FilterWheelBench filterWheelBench)
        {
            _filterWheelBench = filterWheelBench;
            _filterWheelBench.PropertyChanged += FilterWheelBench_PropertyChanged;
            Init();
        }

        private void FilterWheelBench_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_filterWheelBench.RotationPosition))
            {
                PositionToMove = _filterWheelBench.RotationPosition;
            }
        }

        private void Init()
        {
            AxisConfiguration = FilterWheelBench.AxisConfiguration;
            FilterSlots = FilterWheelBench.GetFilterSlots();
        }

        private FilterWheelBench _filterWheelBench;

        public FilterWheelBench FilterWheelBench
        {
            get => _filterWheelBench;
            set => SetProperty(ref _filterWheelBench, value);
        }

        private AxisConfig _axisConfig;

        public AxisConfig AxisConfiguration
        {
            get => _axisConfig;
            set => SetProperty(ref _axisConfig, value);
        }

        private List<FilterSlot> _filterSlots;

        public List<FilterSlot> FilterSlots
        {
            get => _filterSlots;
            set => SetProperty(ref _filterSlots, value);
        }

        private double _positionToMove;

        public double PositionToMove
        {
            get => _positionToMove;
            set => SetProperty(ref _positionToMove, value);
        }

        private AutoRelayCommand<double> _move;

        public AutoRelayCommand<double> Move
        {
            get
            {
                return _move ?? (_move = new AutoRelayCommand<double>((position) => _filterWheelBench.Move(position)));
            }
        }

        private IRelayCommand _moveLeft;

        public IRelayCommand MoveLeft
        {
            get
            {
                if (_moveLeft == null)
                    _moveLeft = new RelayCommand(PerformMoveLeft);

                return _moveLeft;
            }
        }

        private void PerformMoveLeft()
        {
            PositionToMove = FilterWheelBench.GetCurrentPosition() - AxisConfiguration.PositionMax.Value / 180;
            _filterWheelBench.Move(PositionToMove);
        }

        private IRelayCommand _moveRight;

        public IRelayCommand MoveRight
        {
            get
            {
                if (_moveRight == null)
                    _moveRight = new RelayCommand(PerformMoveRight);

                return _moveRight;
            }
        }

        private void PerformMoveRight()
        {
            PositionToMove = FilterWheelBench.GetCurrentPosition() + AxisConfiguration.PositionMax.Value / 180;
            _filterWheelBench.Move(PositionToMove);
        }
    }
}
