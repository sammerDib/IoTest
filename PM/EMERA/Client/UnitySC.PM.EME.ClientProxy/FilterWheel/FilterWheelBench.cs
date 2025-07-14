using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.PM.Shared.Hardware.Service.Interface.Axes;
using UnitySC.Shared.Logger;

using Filter = UnitySC.PM.EME.Service.Interface.Calibration.Filter;

namespace UnitySC.PM.EME.Client.Proxy.FilterWheel
{
    public class FilterWheelBench : ObservableObject
    {
        private readonly IFilterWheelService _filterWheelSupervisor;
        private readonly ICalibrationService _calibrationSupervisor;
        private readonly ILogger _logger;
        private const int MotionEndTimeout = 10000;

        private double _rotationPosition;

        public double RotationPosition
        {
            get => _rotationPosition;
            set => SetProperty(ref _rotationPosition, value);
        }

        private ObservableCollection<Filter> _filters;

        public ObservableCollection<Filter> Filters
        {
            get => _filters;
            private set => SetProperty(ref _filters, value);
        }

        private Filter _currentFilter;

        public Filter CurrentFilter
        {
            get => _currentFilter;
            set
            {
                if (SetProperty(ref _currentFilter, value))
                {
                    MoveAsync(CurrentFilter.Position);
                }
            }
        }

        public async Task MoveAsync(double currentFilterPosition)
        {
            try
            {
                IsLoading = true;
                await Task.Run(() =>
                {
                    Move(currentFilterPosition);
                });
                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "FilterWheel change current filter failed");
            }
        }
        
        public AxisConfig AxisConfiguration { get; private set; }

        public FilterWheelBench(IFilterWheelService filterWheelSupervisor, ICalibrationService calibrationSupervisor, ILogger logger)
        {
            _filterWheelSupervisor = filterWheelSupervisor;
            _calibrationSupervisor = calibrationSupervisor;
            _logger = logger;
            Init();
        }

        private void Init()
        {
            var filters = _calibrationSupervisor.GetFilters()?.Result ?? new List<Filter>();
            Filters = new ObservableCollection<Filter>(filters);

            try
            {
                AxisConfiguration = _filterWheelSupervisor.GetAxisConfiguration()?.Result;
                RotationPosition = _filterWheelSupervisor.GetCurrentPosition().Result;
                CurrentFilter = Filters.ToList().Find(x => Math.Abs(x.Position - RotationPosition) < 1);
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "FilterWheelViewModel Init Failed");
            }
        }

        public void Move(double newPosition)
        {
            _filterWheelSupervisor.GoToPosition(newPosition);
            _filterWheelSupervisor.WaitMotionEnd(MotionEndTimeout);
            RotationPosition = _filterWheelSupervisor.GetCurrentPosition().Result;
        }

        public double GetCurrentPosition()
        {
            return _filterWheelSupervisor.GetCurrentPosition().Result;
        }

        public List<FilterSlot> GetFilterSlots()
        {
            return _filterWheelSupervisor.GetFilterSlots().Result;
        }

        public List<Filter> GetFilters()
        {
            return _calibrationSupervisor.GetFilters().Result;
        }
        
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                SetProperty(ref _isLoading, value);
            }
        }
    }
}
