using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.ANA.Client.Controls.WaferMap;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class WaferMapDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool _dieReferenceChanged = false;

        private WaferMapResult _waferMap;

        public WaferMapResult WaferMap
        {
            get { return _waferMap; }
            internal set
            {
                if (_waferMap != value)
                {
                    _waferMap = value;
                    UpdateDieReference(_waferMap.DieReference);
                    if (!(_waferMap.DieReference is null))
                        StepState = StepStates.Done;

                    OnPropertyChanged();
                }
            }
        }     

        private DieIndex _dieReference = new DieIndex(0, 0);

        public DieIndex DieReference
        {
            get => _dieReference;
            set
            {
                if ((_dieReference != value) && (StepState == UnitySC.Shared.UI.Controls.StepStates.InProgress))
                {
                    UpdateDieReference(value);
                }
            }
        }

        private void UpdateDieReference(DieIndex newDieReference)
        {
            _dieReference = newDieReference;
            OnPropertyChanged(nameof(DieReference));
            OnPropertyChanged(nameof(DieReferenceRow));
            OnPropertyChanged(nameof(DieReferenceColumn));
        }

        public int DieReferenceRow
        {
            get => WaferMap.NbRows-_dieReference.Row-1;
            set
            {
                if (_dieReference.Row != value)
                {
                    _dieReference = new DieIndex(_dieReference.Column, WaferMap.NbRows-value-1);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DieReference));
                }
            }
        }

        public int DieReferenceColumn
        {
            get => _dieReference.Column;
            set
            {
                if (_dieReference.Column != value)
                {
                    _dieReference = new DieIndex(value, _dieReference.Row);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DieReference));
                }
            }
        }

        private AutoRelayCommand _selectCenterDie;

        public AutoRelayCommand SelectCenterDie
        {
            get
            {
                return _selectCenterDie ?? (_selectCenterDie = new AutoRelayCommand(
                    () =>
                    {
                        DieReference = new DieIndex((int)WaferMap.NbColumns / 2, (int)WaferMap.NbRows / 2);
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _editDieReference;

        public AutoRelayCommand EditDieReference
        {
            get
            {
                return _editDieReference ?? (_editDieReference = new AutoRelayCommand(
                    () =>
                    {
                        StepState = UnitySC.Shared.UI.Controls.StepStates.InProgress;
                        //DieReference = new DieIndex(0, WaferMap.NbRows);
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _submitDieReference;

        public AutoRelayCommand SubmitDieReference
        {
            get
            {
                return _submitDieReference ?? (_submitDieReference = new AutoRelayCommand(
                    () =>
                    {
                        StepState = UnitySC.Shared.UI.Controls.StepStates.Done;
                        _dieReferenceChanged = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _cancelDieReference;

        public AutoRelayCommand CancelDieReference
        {
            get
            {
                return _cancelDieReference ?? (_cancelDieReference = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = false;
                    },
                    () => { return true; }
                ));
            }
        }

        //using UnitySC.Shared.UI.AutoRelayCommandExt;
        private AutoRelayCommand _validateDieReference;

        public AutoRelayCommand ValidateDieReference
        {
            get
            {
                return _validateDieReference ?? (_validateDieReference = new AutoRelayCommand(
                    () =>
                    {
                        DialogResult = true;

                        WaferMap.DieReference = DieReference;
                    },
                    () => { return _dieReferenceChanged; }
                ));
            }
        }

        private bool? _dialogResult;

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
