using System.IO;

using AdcTools.Widgets;

using BasicModules.DataLoader;

namespace ADC.ViewModel.MergedRecipe
{
    /// <summary>
    /// View model de selection d'un repertoire pour la modification des inputs d'une recette mergée
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class PicturePathSelectionFolderViewModel : PicturePathSelectionViewModelBase
    {
        public PicturePathSelectionFolderViewModel(InspectionInputInfoBase inputInfoBase) : base(inputInfoBase)
        {
        }

        public override string TypeName => "Folder";

        private string _currentPath;
        public override string CurrentPath
        {
            get => _currentPath; set { if (_currentPath != value) { _currentPath = value; OnPropertyChanged(); Validate(); } }
        }

        private bool _isValid;
        public override bool IsValid
        {
            get => _isValid; set { if (_isValid != value) { _isValid = value; OnPropertyChanged(); } }
        }

        public override void ApplyChange()
        {
            // On met à jour les chemins de l'ensemble des fichiers 
            foreach (var acquisitionData in _inputInfoBase.InputDataList)
            {
                acquisitionData.Filename = acquisitionData.Filename.Replace(_inputInfoBase.Folder, _currentPath);
            }

            _inputInfoBase.Folder = _currentPath;
        }

        internal override void Init()
        {
            _currentPath = _inputInfoBase.Folder;
            Validate();
        }

        internal override void OpenPath(string path)
        {
            CurrentPath = SelectFolderDialog.ShowDialog(CurrentPath);
        }

        private void Validate()
        {
            if (!Directory.Exists(_currentPath))
            {
                IsValid = false;
                return;
            }

            // On  verfie que l'ensemble des fichiers existes avec le repertoire séléctionné par l'utilisateur
            foreach (var acquisitionData in _inputInfoBase.InputDataList)
            {
                if (!File.Exists(acquisitionData.Filename.Replace(_inputInfoBase.Folder, _currentPath)))
                {
                    IsValid = false;
                    return;
                }
            }

            IsValid = true;
        }

        internal override void SetAcquistionFolder(string acquisitionFolder)
        {
            CurrentPath = acquisitionFolder;
        }
    }
}
