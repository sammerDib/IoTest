using System.Collections.Generic;
using System.IO;
using System.Linq;

using ADC.Model;

using AdcTools.Widgets;

using BasicModules.DataLoader;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel.MergedRecipe
{
    /// <summary>
    /// View model pour la modification des inputs d'une recette mergée
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class PicturesSelectionViewModel : ClosableViewModel
    {
        private List<PicturePathSelectionViewModelBase> _picturePathSelectionList = new List<PicturePathSelectionViewModelBase>();
        public IEnumerable<PicturePathSelectionViewModelBase> PicturePathSelectionList => _picturePathSelectionList;

        public PicturesSelectionViewModel()
        {
            Init();
        }

        private void Init()
        {
            _picturePathSelectionList.Clear();
            // Création des view models asssociés aux inputinfos
            _picturePathSelectionList.AddRange(ServiceRecipe.Instance().RecipeCurrent.InputInfoList.
                OfType<FullImageInputInfo>()
                .Select(x => new PicturePathSelectionFullImageViewModel(x)));
            _picturePathSelectionList.AddRange(ServiceRecipe.Instance().RecipeCurrent.InputInfoList.
                OfType<MosaicInputInfo>()
                .Select(x => new PicturePathSelectionFolderViewModel(x)));
            _picturePathSelectionList.AddRange(ServiceRecipe.Instance().RecipeCurrent.InputInfoList.
                OfType<DieInputInfo>()
                .Select(x => new PicturePathSelectionFolderViewModel(x)));

            OutputFolder = ServiceRecipe.Instance().RecipeCurrent.OutputDir;
        }

        private string _acquisitionFolder;
        public string AcquisitionFolder
        {
            get => _acquisitionFolder;
            set
            {
                if (_acquisitionFolder != value)
                {
                    _acquisitionFolder = value;
                    OnPropertyChanged();
                    _picturePathSelectionList.ForEach(x => x.SetAcquistionFolder(_acquisitionFolder));
                }
            }
        }

        private bool _outputFolderIsValid;
        public bool OutputFolderIsValid
        {
            get => _outputFolderIsValid;
            set { if (_outputFolderIsValid != value) { _outputFolderIsValid = value; OnPropertyChanged(); SaveCommand.NotifyCanExecuteChanged(); } }
        }

        private string _outputFolder;
        public string OutputFolder
        {
            get => _outputFolder;
            set
            {
                if (_outputFolder != value)
                {
                    _outputFolder = value;
                    OnPropertyChanged();
                    OutputFolderIsValid = Directory.Exists(_outputFolder);
                }
            }
        }

        private void OpenAcquisitionFolder()
        {
            AcquisitionFolder = SelectFolderDialog.ShowDialog(AcquisitionFolder);
        }

        private void OpenOutputFolder()
        {
            OutputFolder = SelectFolderDialog.ShowDialog(OutputFolder);
        }

        private void Save()
        {
            _picturePathSelectionList.ForEach(x => x.ApplyChange());
            ServiceRecipe.Instance().RecipeCurrent.OutputDir = OutputFolder;
            ServiceRecipe.Instance().MustBeSaved = true;
            CloseSignal = true;
        }

        private void Cancel()
        {
            CloseSignal = true;
        }

        #region Commands

        private AutoRelayCommand _openAcquisitionFolderCommand;
        public AutoRelayCommand OpenAcquisitionFolderCommand
        {
            get
            {
                return _openAcquisitionFolderCommand ?? (_openAcquisitionFolderCommand = new AutoRelayCommand(
              () =>
              {
                  OpenAcquisitionFolder();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _openOutputFolderCommand;
        public AutoRelayCommand OpenOutputFolderCommand
        {
            get
            {
                return _openOutputFolderCommand ?? (_openOutputFolderCommand = new AutoRelayCommand(
              () =>
              {
                  OpenOutputFolder();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand _saveCommand;
        public AutoRelayCommand SaveCommand
        {
            get
            {
                return _saveCommand ?? (_saveCommand = new AutoRelayCommand(
              () =>
              {
                  Save();
              },
              () => { return _picturePathSelectionList.All(x => x.IsValid) && OutputFolderIsValid; }));
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
                  Cancel();
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
