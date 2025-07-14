using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Xml;

using AcquisitionAdcExchange;

using ADC.Model;
using ADC.ViewModel.Ada.ChamberTreeView;

using AdcTools;
using AdcTools.Widgets;

using Serilog;

using UnitySC.DataAccess.Service.Interface;
using Dto = UnitySC.DataAccess.Dto;

using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.Proxy;



namespace ADC.ViewModel.Ada
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class NewAdaViewModel : ClosableViewModel
    {
        //Const
        public const string SectionHeader = "HEADER";
        public const string SectionModule = "Module {0}";
        public const string SectionInfoWafer = "INFO WAFER";
        public const string AdaDatetimeFormat = "M-d-yyyy  HH:mm:ss";
        public const string AdaImage = "image_{0}";
        public const string MosaicColumnFolderName = "Column_";

        // Private
        private RootTreeViewModel _rootTreeViewModel = new RootTreeViewModel();

        private ObservableCollection<Dto.WaferCategory> _waferTypes = new ObservableCollection<Dto.WaferCategory>();

        private ICollectionView _waferTypesView;
        private ObservableCollection<DataloaderViewModel> _dataLoaders = new ObservableCollection<DataloaderViewModel>();
        private string _recipeFileName;
        private IniFile _adaIniFile;

        // Public
        public RootTreeViewModel RootTreeViewModel => _rootTreeViewModel;
        public ObservableCollection<DataloaderViewModel> DataLoaders => _dataLoaders;
        public string AdaPath;

        // Notified properties
        private Dto.WaferCategory _selectedWaferType;
        public Dto.WaferCategory SelectedWaferType
        {
            get => _selectedWaferType;
            set
            {
                if (_selectedWaferType != value)
                {
                    _selectedWaferType = value;
                    OnPropertyChanged();
                    WaferTypeSelectionIsOpen = false;
                }
            }
        }

        public ICollectionView WaferTypes
        {
            get { return _waferTypesView; }
            set
            {
                _waferTypesView = value;
                OnPropertyChanged();
            }
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    OnPropertyChanged();
                    _waferTypesView.Refresh();
                }
            }
        }

        private bool _chamberSelectionIsOpen;
        public bool ChamberSelectionIsOpen
        {
            get => _chamberSelectionIsOpen; set { if (_chamberSelectionIsOpen != value) { _chamberSelectionIsOpen = value; OnPropertyChanged(); } }
        }


        private bool _waferTypeSelectionIsOpen;
        public bool WaferTypeSelectionIsOpen
        {
            get => _waferTypeSelectionIsOpen;
            set
            {
                if (_waferTypeSelectionIsOpen != value)
                {
                    _waferTypeSelectionIsOpen = value;
                    OnPropertyChanged();
                    if (!_waferTypeSelectionIsOpen)
                        Filter = null;
                }
            }
        }

        public NewAdaViewModel(string recipeFileName)
        {
            _recipeFileName = string.IsNullOrEmpty(recipeFileName) ? "NoRecipeFileName" : recipeFileName;
            Init();
        }

        private void Init()
        {
            var toolServiceProxy = ClassLocator.Default.GetInstance<DbToolServiceProxy>();

            // Initilisation du treeview de selection de chambre
            _rootTreeViewModel.Init(toolServiceProxy.GetAllTools(includeChambers: true), new Action(() => { ChamberSelectionIsOpen = false; }));
            _rootTreeViewModel.SelectedTreeViewItem = _rootTreeViewModel.Children.FirstOrDefault()?.Children.FirstOrDefault();

            // Initialisation des wafer types disponible
            _waferTypes.AddRange(toolServiceProxy.GetWaferCategories());
            SelectedWaferType = _waferTypes.FirstOrDefault();
            WaferTypes = CollectionViewSource.GetDefaultView(_waferTypes);
            _waferTypesView.Filter = WaferTypeFilter;


            // Creation d'un DataloaderViewLodel par couple module Id / Channel ID
            _dataLoaders.AddRange(ServiceRecipe.Instance().RecipeCurrent.DataLoaders
                .GroupBy(dl => (dl.DataLoaderActorType, dl.CompatibleResultTypes.First()))
                .Select(dls => new DataloaderViewModel(dls.First())));

            // Valeur par défaut pour les informations du wafer
            _slotId = 1;
            _loadPortId = 1;
            _lotId = "Lot" + DateTime.Now.ToString("yyyyMdHHmmss");
        }

        /// <summary>
        ///  Filtre sur le nom du wafer.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool WaferTypeFilter(object obj)
        {
            var wafer = obj as Dto.WaferCategory;
            return (_filter == null || wafer.Name.ToLower().Contains(_filter.ToLower()));
        }

        #region Wafer infos


        private string _aDCOutputDataFilePath;
        public string ADCOutputDataFilePath
        {
            get => _aDCOutputDataFilePath; set { if (_aDCOutputDataFilePath != value) { _aDCOutputDataFilePath = value; OnPropertyChanged(); } }
        }


        private int _slotId;
        public int SlotId
        {
            get => _slotId; set { if (_slotId != value) { _slotId = value; OnPropertyChanged(); } }
        }


        private int _loadPortId;
        public int LoadPortId
        {
            get => _loadPortId; set { if (_loadPortId != value) { _loadPortId = value; OnPropertyChanged(); } }
        }


        private string _lotId;
        public string LotId
        {
            get => _lotId; set { if (_lotId != value) { _lotId = value; OnPropertyChanged(); } }
        }

        private double? _waferPositionOnChuckX = 0;
        public double? WaferPositionOnChuckX
        {
            get => _waferPositionOnChuckX; set { if (_waferPositionOnChuckX != value) { _waferPositionOnChuckX = value; OnPropertyChanged(); } }
        }

        private double? _waferPositionOnChuckY = 0;
        public double? WaferPositionOnChuckY
        {
            get => _waferPositionOnChuckY; set { if (_waferPositionOnChuckY != value) { _waferPositionOnChuckY = value; OnPropertyChanged(); } }
        }

        private double? _alignerAngle = 0;
        public double? AlignerAngle
        {
            get => _alignerAngle; set { if (_alignerAngle != value) { _alignerAngle = value; OnPropertyChanged(); } }
        }

        public bool IsEdge { get { return _dataLoaders.Any(vm => vm.IsEdge); } }

        #endregion

        private void OpenADCOutputDataFilePath()
        {
            ADCOutputDataFilePath = SelectFolderDialog.ShowDialog(ADCOutputDataFilePath);
        }

        /// <summary>
        /// Sauvegarde dans le fichier ini ada
        /// </summary>
        private void Save()
        {
            try
            {
                // Validation
                _dataLoaders.ToList().ForEach(x => x.Validate());
                if (_dataLoaders.Any(x => !x.IsValid))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("Invalid input data in dataloaders:");
                    foreach (DataloaderViewModel dataLoader in _dataLoaders.Where(x => !x.IsValid))
                    {
                        builder.AppendLine(string.Format("Error in {0} : {1}", dataLoader.Name, dataLoader.ErrorMessage));
                    }
                    AdcTools.AttentionMessageBox.Show(builder.ToString());
                    return;
                }

                System.Windows.Forms.SaveFileDialog saveFileDlg = new System.Windows.Forms.SaveFileDialog();
                saveFileDlg.Filter = "ADA Files (*.ada *.adc)|*.ada;*.adc|All files (*.*)|*.*";
                saveFileDlg.InitialDirectory = ConfigurationManager.AppSettings["AdaFolder"];
                if (saveFileDlg.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;
                AdaPath = saveFileDlg.FileName;

                if (File.Exists(AdaPath))
                    File.Delete(AdaPath);

                _adaIniFile = new IniFile(AdaPath);

                // HEADER
                WriteHeader();

                // INFO WAFER
                WriteInfoWafer();

                int moduleIndex = 0;

                // Modules
                foreach (DataloaderViewModel dataLoader in _dataLoaders)
                {
                    string currentModuleSection = string.Format(SectionModule, moduleIndex);

                    // Propriétes communes à tous les types de modules
                    _adaIniFile.Write(currentModuleSection, "ChannelID", (int)Enum.Parse(typeof(eChannelID), dataLoader.SelectedChannel));
                    _adaIniFile.Write(currentModuleSection, "ActorTypeId", (int)dataLoader.ActorType);
                    _adaIniFile.Write(currentModuleSection, "ChamberID", ((ChamberViewModel)_rootTreeViewModel.SelectedTreeViewItem).ChamberId);

                    if (dataLoader.PixelSizeX.HasValue)
                        _adaIniFile.Write(currentModuleSection, "pixel_size_x_0", dataLoader.PixelSizeX);
                    if (dataLoader.PixelSizeY.HasValue)
                        _adaIniFile.Write(currentModuleSection, "pixel_size_y_0", dataLoader.PixelSizeY);

                    // Cas spécifique du edge
                    if (dataLoader.IsEdge)
                    {
                        if (dataLoader.StartAngle.HasValue)
                            _adaIniFile.Write(currentModuleSection, "start_angle_0", dataLoader.StartAngle);
                        if (dataLoader.GlitchAnglePosition.HasValue)
                            _adaIniFile.Write(currentModuleSection, "GlitchAnglePosition", dataLoader.GlitchAnglePosition);
                        if (dataLoader.RadiusPosition.HasValue)
                            _adaIniFile.Write(currentModuleSection, "RadiusPosition", dataLoader.RadiusPosition);
                        if (dataLoader.NotchY0.HasValue)
                            _adaIniFile.Write(currentModuleSection, "notch_y_0", dataLoader.NotchY0);
                        if (dataLoader.ChuckOriginY0.HasValue)
                            _adaIniFile.Write(currentModuleSection, "chuck_origin_y_0", dataLoader.ChuckOriginY0);
                    }
                    else
                    {
                        if (dataLoader.WaferCenterX.HasValue)
                            _adaIniFile.Write(currentModuleSection, "wafer_center_x_0", dataLoader.WaferCenterX);
                        if (dataLoader.WaferCenterY.HasValue)
                            _adaIniFile.Write(currentModuleSection, "wafer_center_y_0", dataLoader.WaferCenterY);
                    }


                    // Toutes les images sources pour le dataloader doivent provenir du même repertoire
                    string currentDirectory = null;
                    foreach (PathString path in dataLoader.Paths.Select(x => x.Path).Where(p => !String.IsNullOrEmpty(p)))
                    {
                        if (currentDirectory != null)
                        {
                            if (currentDirectory != path.Directory)
                                throw new InvalidOperationException(string.Format("All source images for the {0} must come from the same directory", dataLoader.Name));
                        }
                        else
                        {
                            currentDirectory = path.Directory;
                        }
                    }

                    // Repertoire des images
                    PathString adcInputDataFilePath = dataLoader.Paths.First().Path;
                    if (dataLoader.ImageSelection == DataloaderViewModel.ImageSelectionType.Mosaic)
                        _adaIniFile.Write(currentModuleSection, "ADCInputDataFilePath", adcInputDataFilePath);
                    else
                        _adaIniFile.Write(currentModuleSection, "ADCInputDataFilePath", adcInputDataFilePath.Directory);

                    // Ajout des autres informations en fonction du mode de sélection des images
                    switch (dataLoader.ImageSelection)
                    {
                        case DataloaderViewModel.ImageSelectionType.MultiFullImage:
                        case DataloaderViewModel.ImageSelectionType.OneFullImage:
                            WriteFullImage(dataLoader, currentModuleSection);
                            break;
                        case DataloaderViewModel.ImageSelectionType.Mosaic:
                            WriteMosaic(dataLoader, currentModuleSection);
                            break;
                        case DataloaderViewModel.ImageSelectionType.Die:
                            WriteDie(dataLoader, currentModuleSection);
                            break;
                    }

                    moduleIndex++;
                }

                CloseSignal = true;
            }
            catch (Exception ex)
            {
                AdaPath = null;
                ExceptionMessageBox.Show("Failed to create ada file", ex);
            }
        }

        /// <summary>
        ///  Write Die info in ini file
        /// </summary>
        private void WriteDie(DataloaderViewModel dataLoader, string currentModuleSection)
        {
            PathString pathString = dataLoader.Paths.First().Path;
            _adaIniFile.Write(currentModuleSection, "max_input_die_map_image", 1);
            _adaIniFile.Write(currentModuleSection, "image_dies_name_0", GetImageDiesName(pathString));

            _adaIniFile.Write(currentModuleSection, "CutDieConfiguration_0", pathString.Filename);
        }

        /// <summary>
        /// Get image die file name from xml and picture
        /// </summary>
        /// <param name="dieFileName"></param>
        /// <returns></returns>
        private string GetImageDiesName(PathString dieFileName)
        {
            string res;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(dieFileName);
                XmlNode root = doc.DocumentElement;
                XmlNode firstDieIndexX = root.SelectSingleNode("/root/DieBlock/Die[1]/IndexX/@value");
                XmlNode firstDieIndexY = root.SelectSingleNode("/root/DieBlock/Die[1]/IndexY/@value");
                string searchPattern = string.Format("*_{0}_{1}.*", firstDieIndexX.Value, firstDieIndexY.Value);
                PathString pictureFile = Directory.GetFiles(dieFileName.Directory, searchPattern, SearchOption.TopDirectoryOnly).First();
                // On conserve le nom du fichier sans la position des dies
                res = pictureFile.Basename.Substring(0, pictureFile.Basename.Length - (searchPattern.Length - 3));
            }
            catch (Exception ex)
            {
                res = "CutUp_DieTo";
                Log.Warning("[NewAda] Error while reading image_dies_name", ex);
            }

            return res;

        }

        /// <summary>
        /// Write Mosaic info in ini file
        /// </summary>
        private void WriteMosaic(DataloaderViewModel dataLoader, string currentModuleSection)
        {
            _adaIniFile.Write(currentModuleSection, "max_input_mosaic_image", 1);

            if (dataLoader.Is3D)
            {
                _adaIniFile.Write(currentModuleSection, "nb_line_0", 1);
                _adaIniFile.Write(currentModuleSection, "nb_column_0", dataLoader.NbColumnsMosaic3D());
                _adaIniFile.Write(currentModuleSection, "image_mosaic_0", Path.GetFileName(dataLoader.Paths.First().Path));
            }
            else
            {
                // Columns
                PathString pathString = dataLoader.Paths.First().Path;
                IEnumerable<string> columnFolderNames = Directory.GetDirectories(pathString, MosaicColumnFolderName + "*");
                _adaIniFile.Write(currentModuleSection, "nb_column_0", columnFolderNames.Count());

                // Lines
                IEnumerable<string> lineFileNames = Directory.GetFiles(columnFolderNames.First());
                _adaIniFile.Write(currentModuleSection, "nb_line_0", lineFileNames.Count());

                // Image Mosaic Name
                PathString firstLineFileName = lineFileNames.First();
                string endPartToRemove = "_C0_L0" + firstLineFileName.Extension;
                string str = firstLineFileName.Filename;
                int len = str.Length - endPartToRemove.Length;
                string imageMosaicName = str.Substring(0, len);
                _adaIniFile.Write(currentModuleSection, "image_mosaic_0", imageMosaicName);
            }
        }

        /// <summary>
        /// Write FullImage info in ini file
        /// </summary>
        private void WriteFullImage(DataloaderViewModel dataLoader, string currentModuleSection)
        {

            IEnumerable<string> validPaths = dataLoader.Paths.Where(p => !String.IsNullOrEmpty(p.Path)).Select(x => x.Path);

            _adaIniFile.Write(currentModuleSection, "max_input_full_image", validPaths.Count());

            // Add Images
            int imageIndex = 0;
            foreach (var path in validPaths)
            {
                PathString pathString = path;
                _adaIniFile.Write(currentModuleSection, string.Format(AdaImage, imageIndex), pathString.Filename);
                imageIndex++;
            }
        }

        private void WriteInfoWafer()
        {
            _adaIniFile.Write(SectionInfoWafer, "UniqueID ", "999999");
            _adaIniFile.Write(SectionInfoWafer, "ADCOutputDataFilePath", ADCOutputDataFilePath);
            _adaIniFile.Write(SectionInfoWafer, "CarrierStatus", 3);
            _adaIniFile.Write(SectionInfoWafer, "SlotID", SlotId);
            _adaIniFile.Write(SectionInfoWafer, "LoadPortID", LoadPortId);
            _adaIniFile.Write(SectionInfoWafer, "WaferID", "RPCS_WAFER_ID");
            _adaIniFile.Write(SectionInfoWafer, "StepID", "RPCS_STEP_ID");
            _adaIniFile.Write(SectionInfoWafer, "DeviceID", "RPCS_DEVICE_ID");
            _adaIniFile.Write(SectionInfoWafer, "ToolRecipe", "RPCS_RECIPE_ID");
            _adaIniFile.Write(SectionInfoWafer, "LotID", LotId);
            _adaIniFile.Write(SectionInfoWafer, "StartProcess", DateTime.Now.ToString(AdaDatetimeFormat));
            _adaIniFile.Write(SectionInfoWafer, "ADCRecipeFileName", _recipeFileName);
            _adaIniFile.Write(SectionInfoWafer, "WaferType", SelectedWaferType.Name);
            ToolViewModel tool = (ToolViewModel)(_rootTreeViewModel.SelectedTreeViewItem.Parent);
            _adaIniFile.Write(SectionInfoWafer, "InspectionStationID", string.Format(@"UnitySc\{0}\{1}", tool.ToolCategory, tool.Name));
            if (IsEdge)
            {
                _adaIniFile.Write(SectionInfoWafer, "WaferPositionOnChuckX", WaferPositionOnChuckX);
                _adaIniFile.Write(SectionInfoWafer, "WaferPositionOnChuckY", WaferPositionOnChuckY);
            }
            _adaIniFile.Write(SectionInfoWafer, "AlignerAngle", AlignerAngle);
        }

        private void WriteHeader()
        {
            _adaIniFile.Write(SectionHeader, "ProcessModuleID", "0");
            _adaIniFile.Write(SectionHeader, "ModuleSequence", "0");
            _adaIniFile.Write(SectionHeader, "ModuleNbr", _dataLoaders.Count());
        }

        #region Commands

        private AutoRelayCommand _openADCOutputDataFilePathCommand;
        public AutoRelayCommand OpenADCOutputDataFilePathCommand
        {
            get
            {
                return _openADCOutputDataFilePathCommand ?? (_openADCOutputDataFilePathCommand = new AutoRelayCommand(
              () =>
              {
                  OpenADCOutputDataFilePath();
              },
              () => { return true; }));
            }
        }


        private AutoRelayCommand __saveCommand;
        public AutoRelayCommand SaveCommand
        {
            get
            {
                return __saveCommand ?? (__saveCommand = new AutoRelayCommand(
              () =>
              {
                  Save();
              },
              () =>
              {
                  return SelectedWaferType != null
              && _rootTreeViewModel.SelectedTreeViewItem != null
              && _alignerAngle != null
              && !(IsEdge && (_waferPositionOnChuckX == null || _waferPositionOnChuckY == null))
              && !string.IsNullOrEmpty(_lotId)
              && !string.IsNullOrEmpty(_aDCOutputDataFilePath);
              }));
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
                  CloseSignal = true;
              },
              () => { return true; }));
            }
        }

        #endregion
    }
}
