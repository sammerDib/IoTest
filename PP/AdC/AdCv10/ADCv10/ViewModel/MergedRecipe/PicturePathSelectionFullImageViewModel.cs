using System.IO;
using System.Linq;

using AcquisitionAdcExchange;

using BasicModules.DataLoader;

using UnitySC.Shared.Tools;

namespace ADC.ViewModel.MergedRecipe
{
    /// <summary>
    ///  View model de selection d'une image pour la modification des inputs d'une recette mergée
    /// </summary>
    [System.Reflection.Obfuscation(Exclude = true)]
    public class PicturePathSelectionFullImageViewModel : PicturePathSelectionViewModelBase
    {
        private AcquisitionData _acquisitionData;

        public PicturePathSelectionFullImageViewModel(InspectionInputInfoBase inputInfoBase) : base(inputInfoBase)
        {
        }

        public override string TypeName => "Full image";

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
            _inputInfoBase.Folder = Path.GetDirectoryName(_currentPath);
            _acquisitionData.Filename = _currentPath;
        }

        internal override void Init()
        {
            _acquisitionData = _inputInfoBase.InputDataList.FirstOrDefault();
            _currentPath = _acquisitionData.Filename;
            Validate();
        }

        internal override void OpenPath(string path)
        {
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();

            openFileDlg.Filter = "Tif Files (*.tiff *.tif)|*.tiff;*.tif|Bitmap (*.bmp)|*.bmp|All files (*.*)|*.*";
            PathString dir = new PathString(path).Directory;
            if (Directory.Exists(dir))
                openFileDlg.InitialDirectory = dir;
            if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                CurrentPath = openFileDlg.FileName;
        }

        private void Validate()
        {
            IsValid = File.Exists(_currentPath);
        }

        internal override void SetAcquistionFolder(string acquisitionFolder)
        {
            CurrentPath = Path.Combine(acquisitionFolder, Path.GetFileName(_acquisitionData.Filename));
        }
    }
}
