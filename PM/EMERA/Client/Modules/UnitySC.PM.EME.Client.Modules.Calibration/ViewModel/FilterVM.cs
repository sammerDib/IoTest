using System;
using System.Collections.Generic;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.EME.Service.Interface.Calibration;
using UnitySC.PM.EME.Service.Interface.FilterWheel;
using UnitySC.Shared.Data.Enum.Module;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.EME.Client.Modules.Calibration.ViewModel
{
    public class FilterVM : ObservableRecipient
    {
        public FilterVM(int index, FilterSlot filterSlot)
        {
            Index = index;           
            _item = new Filter
            {
                Name = string.Empty,
                Position = filterSlot.Position,
                Type = EMEFilter.Unknown,                
            };
            InstallationState = FilterInstallationState.Missing;
            CalibrationState = FilterCalibrationState.Uncalibrated;
        }

        public FilterVM(int index, Filter filter)
        {
            Index = index;
            _item = filter;
            InstallationState = FilterInstallationState.Validated;
            CalibrationState = filter.CalibrationStatus.State;
        }
        public FilterVM(Filter filter)
        {
            _item = filter;
            InstallationState = FilterInstallationState.Validated;
            CalibrationState = filter.CalibrationStatus.State;
        }
        private Filter _item;
        public Filter Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);                
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Type)); 
                OnPropertyChanged(nameof(InstallationState));
                OnPropertyChanged(nameof(DistanceOnFocus));
            }
        }

        private int _index;

        public int Index
        {
            get => _index;
            set => SetProperty(ref _index, value);
        }                

        public List<EMEFilter> FilterTypes => GetAllFilterTypes();

        private List<EMEFilter> GetAllFilterTypes()
        {
            return Enum.GetValues(typeof(EMEFilter)).Cast<EMEFilter>().ToList();
        }
        public string Name
        {
            get => _item.Name;
            set
            {
                if (_item.Name != value)
                {
                    _item.Name = value;
                    OnPropertyChanged();
                }
            }
        }
        public EMEFilter Type
        {
            get => _item.Type;
            set
            {
                if (_item.Type != value)
                {
                    _item.Type = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Position
        {
            get => _item.Position;
            set
            {
                if (_item.Position != value)
                {
                    _item.Position = value;
                    OnPropertyChanged();
                }
            }
        }
        public Length ShiftX
        {
            get => _item.ShiftX;
            set
            {
                if (_item.ShiftX != value)
                {
                    _item.ShiftX = value;
                    OnPropertyChanged();
                }
            }
        }
        public Length ShiftY
        {
            get => _item.ShiftY;
            set
            {
                if (_item.ShiftY != value)
                {
                    _item.ShiftY = value;
                    OnPropertyChanged();
                }
            }
        }
        public Length PixelSize
        {
            get => _item.PixelSize;
            set
            {
                if (_item.PixelSize != value)
                {
                    _item.PixelSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public FilterCalibrationState CalibrationState
        {
            get => _item.CalibrationStatus.State;
            set
            {
                if (_item.CalibrationStatus.State != value)
                {
                    _item.CalibrationStatus.State = value;
                    OnPropertyChanged();
                }
            }
        }
        public double DistanceOnFocus
        {
            get => _item.DistanceOnFocus;
            set
            {
                if (_item.DistanceOnFocus != value)
                {
                    _item.DistanceOnFocus = value;
                    OnPropertyChanged();
                }
            }           
        }
        
        private FilterInstallationState _installationState;
        public FilterInstallationState InstallationState
        {
            get => _installationState;
            set => SetProperty(ref _installationState, value);
        }       
        public override string ToString()
        {
            return Name;
        }       
    }
}
