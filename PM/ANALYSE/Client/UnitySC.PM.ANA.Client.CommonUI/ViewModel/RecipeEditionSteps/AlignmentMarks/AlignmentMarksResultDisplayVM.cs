using System;
using System.Collections.Generic;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using MvvmDialogs;

using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Format.Metro.TSV;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Controls;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class AlignmentMarksResultDisplayVM : StepBaseVM, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public AlignmentMarksResultDisplayVM(AlignmentMarksResult alignmentMarksResult)
        {
            AlignmentMarksResult = alignmentMarksResult;
        }

        private AlignmentMarksResult _alignmentMarksResult = null;

        public AlignmentMarksResult AlignmentMarksResult
        {
            get => _alignmentMarksResult;
            set
            {
                if (_alignmentMarksResult != value)
                {
                    _alignmentMarksResult = value;

                    OnPropertyChanged();
                }
            }
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
