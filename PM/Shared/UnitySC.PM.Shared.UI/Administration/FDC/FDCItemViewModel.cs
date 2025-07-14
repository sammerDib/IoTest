using System;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.Shared.Hardware.ClientProxy.FDC;
using UnitySC.Shared.Data.FDC;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.UI.Administration.FDC
{
    public class FDCItemViewModel : ObservableObject
    {
        private FDCSupervisor _fdcSupervisor;
        public FDCActorViewModel FDCsActorViewModel { get; set; }
        public FDCItemViewModel(FDCSupervisor fdcSupervisor)
        {
            _fdcSupervisor = fdcSupervisor;
        }

        private string _name = string.Empty;
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private string _value = string.Empty;
        public string Value
        {
            get => _value; set { if (_value != value) { _value = value; OnPropertyChanged(); } }
        }


        private FDCValueType _valueType = FDCValueType.TypeString;
        public FDCValueType ValueType
        {
            get => _valueType; set { if (_valueType != value) { _valueType = value; OnPropertyChanged(); } }
        }

        private string _unit = string.Empty;
        public string Unit
        {
            get => _unit; set { if (_unit != value) { _unit = value; OnPropertyChanged(); } }
        }

        private DateTime? _sendDate = null;
        public DateTime? SendDate
        {
            get => _sendDate; set { if (_sendDate != value) { _sendDate = value; OnPropertyChanged(); } }
        }

        private FDCSendFrequency _sendFrequency = FDCSendFrequency.Never;
        public FDCSendFrequency SendFrequency
        {
            get => _sendFrequency; set { if (_sendFrequency != value) { _sendFrequency = value; OnPropertyChanged(); } }
        }

        private bool _canBeReset = true;
        public bool CanBeReset
        {
            get => _canBeReset; set { if (_canBeReset != value) { _canBeReset = value; OnPropertyChanged(); } }
        }

        private bool _canInitValue = false;
        public bool CanInitValue
        {
            get => _canInitValue; set { if (_canInitValue != value) { _canInitValue = value; OnPropertyChanged(); } }
        }

        private int _initValue = 1000;
        public int InitValue
        {
            get => _initValue; set { if (_initValue != value) { _initValue = value; OnPropertyChanged(); } }
        }

        private bool _isEditing = false;
        public bool IsEditing
        {
            get => _isEditing; set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

        private bool _isModified = false;
        public bool IsModified
        {
            get => _isModified; set { if (_isModified != value) { _isModified = value; OnPropertyChanged(); } }
        }


        #region Commands

        private AutoRelayCommand _startEditCommand;
        public AutoRelayCommand StartEditCommand
        {
            get
            {
                return _startEditCommand ?? (_startEditCommand = new AutoRelayCommand(
                    () =>
                    {
                        IsEditing = true;
                    },
                    () => { return IsEditing==false; }
                ));
            }
        }

        private AutoRelayCommand _stopEditCommand;
        public AutoRelayCommand StopEditCommand
        {
            get
            {
                return _stopEditCommand ?? (_stopEditCommand = new AutoRelayCommand(
                    () =>
                    {
                        if (string.IsNullOrEmpty(Name))
                        {
                            var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The FDC Name can not be empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }


                        IsEditing = false;
                        IsModified = true;
                    },
                    () => { return IsEditing == true; }
                ));
            }
        }

        private AutoRelayCommand _resetFDCCommand;
        public AutoRelayCommand ResetFDCCommand
        {
            get
            {
                return _resetFDCCommand ?? (_resetFDCCommand = new AutoRelayCommand(
                    () =>
                    {
                        var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Are you sure you want to reset the FDC Value ?", "Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                        if (res == MessageBoxResult.Yes)
                        {
                            _fdcSupervisor.ResetFDC(Name);
                            if (FDCsActorViewModel!=null)
                                FDCsActorViewModel.UpdateFDCValue(Name);
                        }
                    },
                    () => { return CanBeReset; }
                ));
            }
        }

        #endregion Commands
    }
}
