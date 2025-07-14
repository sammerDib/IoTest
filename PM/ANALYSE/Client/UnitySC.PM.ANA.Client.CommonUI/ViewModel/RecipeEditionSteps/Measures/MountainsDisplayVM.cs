using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class MountainsDisplayVM : ObservableObject, MvvmDialogs.IModalDialogViewModel
    {
        private string _currentFilePath;
        public MountainsWPFControl MountainsWPFControl => ClassLocator.Default.GetInstance<MountainsWPFControl>();

        public List<ExternalProcessingResultItem> OutputsResult { get; set; } = new List<ExternalProcessingResultItem>();


        private bool? _dialogResult;

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        private AutoRelayCommand _applyCommand;
        public AutoRelayCommand ApplyCommand
        {
            get
            {
                return _applyCommand ?? (_applyCommand = new AutoRelayCommand(
              () =>
              {
                  MountainsWPFControl.Save(_currentFilePath);
                  OutputsResult = MountainsWPFControl.GetResultsDefinedInCurrentTemplate();
                  MountainsWPFControl.ClearContent();
                  DialogResult = true;

              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _cancelCommand;
        public AutoRelayCommand CancelCommand
        {
            get
            {
                return _cancelCommand ?? (_cancelCommand = new AutoRelayCommand(
              () =>
              {
                  MountainsWPFControl.ClearContent();
                  DialogResult = false;
              },
              () => { return true; }));
            }
        }

        public void InitExistingDoc(string filePath)
        {
            DialogResult = null;
            MountainsWPFControl.InitDoc(filePath);
            _currentFilePath = filePath;
            OutputsResult.Clear();
        }

        public void SubstituteStudiable(string filePath)
        {
            DialogResult = null;
            MountainsWPFControl.SubstituteStudiable(filePath);
        }

        public void InitNewDoc(string filePath)
        {
            DialogResult = null;
            MountainsWPFControl.InitDoc();
            _currentFilePath = filePath;
            OutputsResult.Clear();
        }
    }
}
