using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using AcquisitionAdcExchange;

using ADCEngine;

using AdcTools.Widgets;

using BasicModules.DataLoader;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace ADC.ViewModel.Ada
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class DataloaderViewModel : ObservableRecipient
    {
        private IDataLoader _dataLoader;

        public enum ImageSelectionType { MultiFullImage, OneFullImage, Mosaic, Die };

        public bool IsMultiFile => _imageSelection == ImageSelectionType.MultiFullImage;

        public bool IsEdge => ActorType == ActorType.EdgeInspect || ActorType == ActorType.Edge || ActorType == ActorType.Argos;

        public bool Is3D => ActorType == ActorType.BrightField3D;

        public ActorType ActorType => _dataLoader.DataLoaderActorType;

        public DataloaderViewModel(IDataLoader dataLoader)
        {
            _dataLoader = dataLoader;
            _name = ((ModuleBase)dataLoader).Name;
            Channels = _dataLoader.CompatibleResultTypes.Select(x => DataLoaderHelper.GetResultTypeNameWithCheck(ActorType, x)).ToList();
            SelectedChannel = Channels.FirstOrDefault();
            InitPictureSelection();
        }

        /// <summary>
        ///   Mode de séléction des images en fonction du type de dataloader du ActorType et du ChannelId
        /// </summary>
        private void InitPictureSelection()
        {
            if (_dataLoader is DieDataLoaderBase)
            {
                ImageSelection = ImageSelectionType.Die;
                _paths.Add(new DataLoaderPathViewModel(this) { Name = ImageSelection.ToString() });
            }
            else if (_dataLoader is FullImageDataLoaderBase)
            {
               if ( (ActorType == ActorType.DEMETER) && IsDataloaderCompatibleWithDeflecto(_dataLoader.CompatibleResultTypes) )
                {
                    ImageSelection = ImageSelectionType.MultiFullImage;
                    _paths.Add(new DataLoaderPathViewModel(this) { Name = DataLoaderHelper.GetImageSuffixWithCheck(ActorType, ResultType.DMT_CurvatureX_Front) });
                    _paths.Add(new DataLoaderPathViewModel(this) { Name = DataLoaderHelper.GetImageSuffixWithCheck(ActorType, ResultType.DMT_CurvatureY_Front) });
                    _paths.Add(new DataLoaderPathViewModel(this) { Name = DataLoaderHelper.GetImageSuffixWithCheck(ActorType, ResultType.DMT_AmplitudeX_Front) });
                    _paths.Add(new DataLoaderPathViewModel(this) { Name = DataLoaderHelper.GetImageSuffixWithCheck(ActorType, ResultType.DMT_AmplitudeX_Front) });
                }
                else
                {
                    ImageSelection = ImageSelectionType.OneFullImage;
                    _paths.Add(new DataLoaderPathViewModel(this) { Name = ImageSelection.ToString() });
                }
            }
            else if (_dataLoader is MosaicDataLoaderBase)
            {
                ImageSelection = ImageSelectionType.Mosaic;
                _paths.Add(new DataLoaderPathViewModel(this) { Name = ImageSelection.ToString() });
            }
        }

        private bool IsDataloaderCompatibleWithDeflecto(IEnumerable<ResultType> dlCompatibleResultTypes)
        {
            var listofDemeterSpecificID = dlCompatibleResultTypes.Select(rt => rt.GetSpecificModuleId()).ToList();
            return (listofDemeterSpecificID.Contains(1))  // CX //cf enum UnitySC.Shared.Data.Enum.Module.DMTResult
                || (listofDemeterSpecificID.Contains(2))  // CY //cf enum UnitySC.Shared.Data.Enum.Module.DMTResult
                || (listofDemeterSpecificID.Contains(6))  // Ax //cf enum UnitySC.Shared.Data.Enum.Module.DMTResult
                || (listofDemeterSpecificID.Contains(7)); // Ay //cf enum UnitySC.Shared.Data.Enum.Module.DMTResult
        }

        private string _name;
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private double? _pixelSizeX;
        public double? PixelSizeX
        {
            get => _pixelSizeX; set { if (_pixelSizeX != value) { _pixelSizeX = value; OnPropertyChanged(); IsValid = true; } }
        }

        private double? _pixelSizeY;
        public double? PixelSizeY
        {
            get => _pixelSizeY; set { if (_pixelSizeY != value) { _pixelSizeY = value; OnPropertyChanged(); IsValid = true; } }
        }

        private double? _waferCenterX;
        public double? WaferCenterX
        {
            get => _waferCenterX; set { if (_waferCenterX != value) { _waferCenterX = value; OnPropertyChanged(); IsValid = true; } }
        }

        private double? _waferCenterY;
        public double? WaferCenterY
        {
            get => _waferCenterY; set { if (_waferCenterY != value) { _waferCenterY = value; OnPropertyChanged(); IsValid = true; } }
        }

        private double? _startAngle = 0;
        public double? StartAngle
        {
            get => _startAngle; set { if (_startAngle != value) { _startAngle = value; OnPropertyChanged(); IsValid = true; } }
        }

        private double? _glitchAnglePosition = 0;
        public double? GlitchAnglePosition
        {
            get => _glitchAnglePosition; set { if (_glitchAnglePosition != value) { _glitchAnglePosition = value; OnPropertyChanged(); IsValid = true; } }
        }

        private double? _radiusPosition;
        public double? RadiusPosition
        {
            get => _radiusPosition; set { if (_radiusPosition != value) { _radiusPosition = value; OnPropertyChanged(); IsValid = true; } }
        }

        private int? _notchY0;
        public int? NotchY0
        {
            get => _notchY0; set { if (_notchY0 != value) { _notchY0 = value; OnPropertyChanged(); IsValid = true; } }
        }

        private int? _chuckOriginY0;
        public int? ChuckOriginY0
        {
            get => _chuckOriginY0; set { if (_chuckOriginY0 != value) { _chuckOriginY0 = value; OnPropertyChanged(); IsValid = true; } }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage; set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        private bool _isValid = true;
        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    OnPropertyChanged();
                    if (_isValid)
                        ErrorMessage = null;
                }
            }
        }

        private ObservableCollection<DataLoaderPathViewModel> _paths = new ObservableCollection<DataLoaderPathViewModel>();
        public ObservableCollection<DataLoaderPathViewModel> Paths
        {
            get => _paths; set { if (_paths != value) { _paths = value; OnPropertyChanged(); } }
        }

        private ImageSelectionType _imageSelection;
        public ImageSelectionType ImageSelection
        {
            get => _imageSelection;
            set
            {
                if (_imageSelection != value)
                {
                    _imageSelection = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMultiFile));
                }
            }
        }

        private List<string> _channels;
        public List<string> Channels
        {
            get => _channels; set { if (_channels != value) { _channels = value; OnPropertyChanged(); } }
        }

        private string _selectedChannel;
        public string SelectedChannel
        {
            get => _selectedChannel;
            set
            {
                if (_selectedChannel != value)
                {
                    _selectedChannel = value;
                    OnPropertyChanged();
                }
            }
        }

        private AutoRelayCommand<int> _openPathCommand;
        public AutoRelayCommand<int> OpenPathCommand
        {
            get
            {
                return _openPathCommand ?? (_openPathCommand = new AutoRelayCommand<int>(
              (id) =>
              {
                  OpenPath(id);
              }));
            }
        }

        private void OpenPath(int pathId)
        {
            switch (_imageSelection)
            {
                case ImageSelectionType.MultiFullImage:
                case ImageSelectionType.OneFullImage:
                    System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
                    openFileDlg.Filter = "Tif Files (*.tiff *.tif)|*.tiff;*.tif|Bitmap (*.bmp)|*.bmp|Bitmap (*.3DA)|*.3da|All files (*.*)|*.*";
                    if (openFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        Paths[pathId].Path = openFileDlg.FileName;
                    break;

                case ImageSelectionType.Die:
                    System.Windows.Forms.OpenFileDialog openDieFileDlg = new System.Windows.Forms.OpenFileDialog();
                    openDieFileDlg.Filter = "Die xml file (*.xml)|*.xml";
                    if (openDieFileDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        Paths[0].Path = openDieFileDlg.FileName;
                    break;

                case ImageSelectionType.Mosaic:
                    Paths[0].Path = SelectFolderDialog.ShowDialog(Paths[0].Path);
                    break;
            }
        }

        public void Validate()
        {
            bool valid = true;
            _errorMessage = string.Empty;

            // Channel
            if (SelectedChannel == null)
            {
                valid = false;
                _errorMessage += "Channel ";
            }

            // Pixel Size
            if (!PixelSizeX.HasValue || !PixelSizeY.HasValue)
            {
                valid = false;
                _errorMessage += "Pixel size ";
            }

            if (IsEdge)
            {
                // Wafer center
                if (!GlitchAnglePosition.HasValue || !RadiusPosition.HasValue || !StartAngle.HasValue)
                {
                    valid = false;
                    _errorMessage += " Edge Wafer position ";
                }
            }
            else
            {
                // Wafer center
                if (!WaferCenterX.HasValue || !WaferCenterY.HasValue)
                {
                    valid = false;
                    _errorMessage += "Wafer center ";
                }
            }

            // Path
            switch (_imageSelection)
            {
                case ImageSelectionType.MultiFullImage:
                case ImageSelectionType.OneFullImage:
                case ImageSelectionType.Die:
                    if (!File.Exists(Paths[0].Path))
                    {
                        valid = false;
                        _errorMessage += "File(s) ";
                    }
                    break;
                case ImageSelectionType.Mosaic:
                    if (!Directory.Exists(Paths[0].Path))
                    {
                        valid = false;
                        _errorMessage += "Folder ";
                    }
                    break;
            }

            IsValid = valid;
        }


        public int NbColumnsMosaic3D()
        {
            // Recupération du numéro de column contenu dans les fichiers. 
            PathString folder = Paths.First().Path;
            List<string> columFiles = Directory.GetFiles(folder, string.Format("{0}_*.3da", folder.Filename)).Select(x => Path.GetFileNameWithoutExtension(x).Substring(string.Format("{0}_", folder.Filename).Length)).ToList();
            int columnNum = 0;

            // Numéro max de column de type int
            int maxColumnNumber = columFiles.Where(str => int.TryParse(str, out columnNum)).Select(str => columnNum).Max();
            return maxColumnNumber + 1;
        }
    }
}
