using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.PM.ANA.Client.Modules.Calibration.View;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Calibration;
using UnitySC.PM.ANA.Client.Proxy.Chuck;
using UnitySC.PM.ANA.Client.Proxy.Context;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Calibration;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.Shared.Data;
using UnitySC.Shared.Format.Helper;
using UnitySC.Shared.ResultUI.Common.ViewModel.Export;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface;

namespace UnitySC.PM.ANA.Client.Modules.Calibration.ViewModel.XY
{
    public class XYCalibrationVM : CalibrationVMBase
    {
        #region Fields
        private int _gridVectorMin = 13;
        private int _gridHeatMapMin = 70;

        private CalibrationSupervisor _calibrationSupervisor;
        private AlgosSupervisor _algoSupervisor;
        private ContextSupervisor _contextSupervisor;
        private ChuckSupervisor _chuckSupervisor;
        private const int MinNumberOfCalibrationPoints = 32;
        private int _nbOfRemainingTests = 0;
        public const string CsvExport = "CSV";
        #endregion Fields

        #region Constructor
        public XYCalibrationVM(WaferDimensionalCharacteristic waferDimensionalCharacteristic) : base("XY")
        {
            XYVectorHeatMapVM = new XYCalibResultVectorHeatMapVM(_gridVectorMin, _gridHeatMapMin);
            _calibrationSupervisor = ClassLocator.Default.GetInstance<CalibrationSupervisor>();
            _algoSupervisor = ClassLocator.Default.GetInstance<AlgosSupervisor>();
            _contextSupervisor = ClassLocator.Default.GetInstance<ContextSupervisor>();
            _chuckSupervisor = ClassLocator.Default.GetInstance<ChuckSupervisor>();
            _calibrationSupervisor.XYCalibrationProgressEvent += XYCalibrationProgressEvent;
            _calibrationSupervisor.XYCalibrationChangedEvent += XYCalibrationChangedEvent;
            _calibrationSupervisor.XYCalibrationTestProgressEvent += XYCalibrationTestProgressEvent;
            _calibrationSupervisor.XYCalibrationTestChangedEvent += XYCalibrationTestChangedEvent;
            _algoSupervisor.BWAChangedEvent += BWAChangedEvent;
            Recipes = _calibrationSupervisor.GetXYCalibrationRecipes()?.Result;
            SelectedRecipe = Recipes?.FirstOrDefault();
            ExportResultVM = new ExportResultVM() { DisplayZipArchive = true, DisplayExportResultData = false, DisplayExportResultThumbnails = false, DisplayExportSnapshot = false };
            ExportResultVM.AdditionalEntries.Add(new ExportEntry(CsvExport));
            ExportResultVM.OnSaveExportCommand += OnSaveExport;

            ExportTestResultVM = new ExportResultVM() { DisplayZipArchive = true, DisplayExportResultData = false, DisplayExportResultThumbnails = false, DisplayExportSnapshot = false };
            ExportTestResultVM.AdditionalEntries.Add(new ExportEntry(CsvExport));
            ExportTestResultVM.OnSaveExportCommand += OnSaveExportTest;

            SelectedScanDirection = XYCalibDirection.LeftRightThenTopBottom;
        }


        #endregion Constructor

        #region Properties
        private XYCalibResultVectorHeatMapVM _xyVectorHeatMapVM;

        public XYCalibResultVectorHeatMapVM XYVectorHeatMapVM
        {
            get => _xyVectorHeatMapVM; set { if (_xyVectorHeatMapVM != value) { _xyVectorHeatMapVM = value; OnPropertyChanged(); } }
        }

        private bool _startTestAfterCalibration;

        public bool StartTestAfterCalibration
        {
            get => _startTestAfterCalibration; set { if (_startTestAfterCalibration != value) { _startTestAfterCalibration = value; OnPropertyChanged(); } }
        }

        private XYCalibrationRecipe _selectedRecipe;

        public XYCalibrationRecipe SelectedRecipe
        {
            get => _selectedRecipe;
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    UpdatePrecision();

                    SelectedWaferDiameter = _selectedRecipe.Settings.WaferCalibrationDiameter;
                    SelectedPitchX = _selectedRecipe.WaferMap.DieDimensions.DieWidth;
                    SelectedPitchY = _selectedRecipe.WaferMap.DieDimensions.DieHeight;

                    OnPropertyChanged();
                }
            }
        }

        private Length _selectedWaferDiameter;

        public Length SelectedWaferDiameter
        {
            get => _selectedWaferDiameter; set { if (_selectedWaferDiameter != value) { _selectedWaferDiameter = value; OnPropertyChanged(); } }
        }

        private Length _selectedPitchX;

        public Length SelectedPitchX
        {
            get => _selectedPitchX; set { if (_selectedPitchX != value) { _selectedPitchX = value; OnPropertyChanged(); } }
        }

        private Length _selectedPitchY;

        public Length SelectedPitchY
        {
            get => _selectedPitchY; set { if (_selectedPitchY != value) { _selectedPitchY = value; OnPropertyChanged(); } }
        }

        private List<XYCalibrationRecipe> _recipes;

        public List<XYCalibrationRecipe> Recipes
        {
            get => _recipes; set { if (_recipes != value) { _recipes = value; OnPropertyChanged(); } }
        }

        private int _selectedPrecision;

        public int SelectedPrecision
        {
            get => _selectedPrecision;
            set
            {
                if (_selectedPrecision != value)
                {
                    _selectedPrecision = value;
                    OnPropertyChanged();
                }
                if (SelectedRecipe != null)
                {
                    NbOfCalibrationPoints = DieIndexSelector.SelectDiesOnGrid(SelectedRecipe.WaferMap, (MaxPrecision - SelectedPrecision) + 1, SelectedScanDirection).Count;
                }
            }
        }

        private int _selectedTestPrecision;

        public int SelectedTestPrecision
        {
            get => _selectedTestPrecision;
            set
            {
                if (_selectedTestPrecision != value)
                {
                    _selectedTestPrecision = value;
                    OnPropertyChanged();
                }
                if (SelectedRecipe != null)
                {
                    NbOfTestPoints = DieIndexSelector.SelectDiesOnGrid(SelectedRecipe.WaferMap, (MaxTestPrecision - SelectedTestPrecision) + 1, SelectedScanDirection).Count;
                }
            }
        }

        private XYCalibDirection _selectedScanDirection;

        public XYCalibDirection SelectedScanDirection
        {
            get => _selectedScanDirection; set { if (_selectedScanDirection != value) { _selectedScanDirection = value; OnPropertyChanged(); } }
        }

        private Length _initShiftCenterX = 0.Millimeters();

        public Length InitShiftCenterX
        {
            get => _initShiftCenterX; set { if (_initShiftCenterX != value) { _initShiftCenterX = value; OnPropertyChanged(); } }
        }

        private Length _initShiftCenterY = 0.Millimeters();

        public Length InitShiftCenterY
        {
            get => _initShiftCenterY; set { if (_initShiftCenterY != value) { _initShiftCenterY = value; OnPropertyChanged(); } }
        }

        private int _maxPrecision;

        public int MaxPrecision
        {
            get => _maxPrecision; set { if (_maxPrecision != value) { _maxPrecision = value; OnPropertyChanged(); } }
        }

        private int _maxTestPrecision;

        public int MaxTestPrecision
        {
            get => _maxTestPrecision; set { if (_maxTestPrecision != value) { _maxTestPrecision = value; OnPropertyChanged(); } }
        }

        private int _nbOfCalibrationPoints;

        public int NbOfCalibrationPoints
        {
            get => _nbOfCalibrationPoints; set { if (_nbOfCalibrationPoints != value) { _nbOfCalibrationPoints = value; OnPropertyChanged(); } }
        }

        private int _nbOfTestPoints;

        public int NbOfTestPoints
        {
            get => _nbOfTestPoints; set { if (_nbOfTestPoints != value) { _nbOfTestPoints = value; OnPropertyChanged(); } }
        }

        private bool _autoFocusIsEnabled;

        public bool AutoFocusIsEnabled
        {
            get => _autoFocusIsEnabled; set { if (_autoFocusIsEnabled != value) { _autoFocusIsEnabled = value; OnPropertyChanged(); } }
        }

        private bool _calibrationIsDefined;

        public bool CalibrationIsDefined
        {
            get => _calibrationIsDefined;
            set
            {
                if (_calibrationIsDefined != value)
                {
                    _calibrationIsDefined = value;
                    CalibrationTestIsDefined = CalibrationIsDefined && !CalibrationTestInProgress ? true : false;
                    OnPropertyChanged();
                }
            }
        }

        private bool _calibrationTestIsDefined;

        public bool CalibrationTestIsDefined
        {
            get => _calibrationTestIsDefined; set { if (_calibrationTestIsDefined != value) { _calibrationTestIsDefined = value; OnPropertyChanged(); } }
        }

        private XYCalibrationData _calibrationData;

        public XYCalibrationData CalibrationData
        {
            get => _calibrationData;
            set
            {
                if (_calibrationData != value)
                {
                    _calibrationData = value;
                    CalibrationIsDefined = _calibrationData != null;
                    DisplayXYCalibrationResult.NotifyCanExecuteChanged();
                    OnPropertyChanged();
                }
                IsValidated = _calibrationData != null;
            }
        }

        private XYCalibrationTestResultVM _selectedTestResult;

        public XYCalibrationTestResultVM SelectedTestResult
        {
            get => _selectedTestResult; set { if (_selectedTestResult != value) { _selectedTestResult = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<XYCalibrationTestResultVM> TestResults { get; set; } = new ObservableCollection<XYCalibrationTestResultVM>();

        private ExportResultVM _exportResultVM;

        public ExportResultVM ExportResultVM
        { get => _exportResultVM; set { if (_exportResultVM != value) { _exportResultVM = value; OnPropertyChanged(); } } }

        private ExportResultVM _exportTestResultVM;

        public ExportResultVM ExportTestResultVM
        { get => _exportTestResultVM; set { if (_exportTestResultVM != value) { _exportTestResultVM = value; OnPropertyChanged(); } } }


        private bool _isProbeLiseAcquiring = false;

        public bool IsProbeLiseAcquiring
        {
            get => _isProbeLiseAcquiring; set { if (_isProbeLiseAcquiring != value) { _isProbeLiseAcquiring = value; OnPropertyChanged(); } }
        }

        public bool InProgress
        {
            get
            {
                return CalibrationInProgress || CalibrationTestInProgress;
            }
        }

        private bool _calibrationTestInProgress;

        public bool CalibrationTestInProgress
        {
            get => _calibrationTestInProgress;
            set
            {
                if (_calibrationTestInProgress != value)
                {
                    _calibrationTestInProgress = value;
                    CalibrationTestIsDefined = CalibrationIsDefined && !CalibrationTestInProgress ? true : false;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(InProgress));
                }
            }
        }

        private string _calibrationProgress;

        public string CalibrationProgress
        {
            get => _calibrationProgress; set { if (_calibrationProgress != value) { _calibrationProgress = value; OnPropertyChanged(); OnPropertyChanged(nameof(InProgress)); } }
        }

        private bool _calibrationInProgress;

        public bool CalibrationInProgress
        {
            get => _calibrationInProgress; set { if (_calibrationInProgress != value) { _calibrationInProgress = value; OnPropertyChanged(); OnPropertyChanged(nameof(InProgress)); } }
        }

        private XYCalibrationTestResultVM _currentCalibrationTest;

        public XYCalibrationTestResultVM CurrentCalibrationTest
        {
            get => _currentCalibrationTest; set { if (_currentCalibrationTest != value) { _currentCalibrationTest = value; OnPropertyChanged(); } }
        }

        private int _nbOfTestRepetition = 1;

        public int NbOfTestRepetition
        {
            get => _nbOfTestRepetition; set { if (_nbOfTestRepetition != value) { _nbOfTestRepetition = value; OnPropertyChanged(); } }
        }

        public List<SpecificPositions> AvailablePositions
        {
            get => new List<SpecificPositions>
            {
                SpecificPositions.PositionChuckCenter,
                SpecificPositions.PositionHome,
                SpecificPositions.PositionManualLoad,
                SpecificPositions.PositionPark
            };
        }

        public SpecificPositions DefaultSpecificPosition
        {
            get => SpecificPositions.PositionChuckCenter;
        }

        #endregion Properties

        #region Methods
        private void OnSaveExport()
        {
            var correctionsList = new List<List<Correction>>();
            correctionsList.Add(CalibrationData.Corrections);

            OnExportResults(ExportResultVM, correctionsList);
        }

        private void OnSaveExportTest()
        {
            var correctionsList = new List<List<Correction>>();
            foreach (var testresult in TestResults)
            {
                correctionsList.Add(testresult.TestCalibResult.Corrections);
            }
            correctionsList.Add(CalibrationData.Corrections);
            OnExportResults(ExportTestResultVM, correctionsList);
        }

        private void OnExportResults(ExportResultVM exportDialog, List<List<Correction>> corrections)
        {
            if (exportDialog != null && !exportDialog.IsExporting)
            {
                exportDialog.IsExporting = true;

                Task.Run(() =>
                {
                    try
                    {
                        var additionalExports = ExportResultVM.AdditionalEntries.Where(entry => entry.IsChecked).Select(entry => entry.EntryName).ToList();
                        if (!additionalExports.Contains("CSV"))
                            throw new ArgumentException("CSV has not been selected, export will be empty");

                        ExportResultsToCSV(corrections, exportDialog);
                    }
                    catch (Exception ex)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Error during XY calibration export");
                        });
                    }
                    finally
                    {
                        exportDialog.IsExporting = false;
                        exportDialog.IsStayPopup = false;
                    }
                }).ConfigureAwait(false);
            }
        }

        private void ExportResultsToCSV(List<List<Correction>> correctionsList, ExportResultVM exportDialog)
        {
            string exportFilePath = exportDialog.GetTargetFullPath();

            if (exportDialog.UseZipArchive)
            {
                if (!exportFilePath.EndsWith(".zip")) exportFilePath += ".zip";
                string directoryName = Path.GetDirectoryName(exportFilePath);
                Directory.CreateDirectory(directoryName);

                using (var zipToOpen = new FileStream(exportFilePath, FileMode.OpenOrCreate))
                {
                    using (var archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                    {
                        int entryNameIndex = 1;
                        foreach (var corrections in correctionsList)
                        {
                            string entryName;

                            if (correctionsList.Count == 1)
                            {
                                entryName = exportDialog.TargetName + "_xycalib.csv";
                            }
                            else
                            {
                                entryName = exportDialog.TargetName + $"_xycalib_{entryNameIndex}.csv";
                            }

                            var csvZipEntry = archive.CreateEntry(entryName);
                            string separator = CSVStringBuilder.GetCSVSeparator();
                            using (var zipfileStream = csvZipEntry.Open())
                            {
                                using (StreamWriter file = new StreamWriter(zipfileStream, Encoding.UTF8))
                                {
                                    //Header for file
                                    var corUnit = corrections.FirstOrDefault();

                                    var header = string.Format($"X position ({corUnit.XTheoricalPosition.UnitSymbol}){separator}" +
                                                                $"Y position ({corUnit.YTheoricalPosition.UnitSymbol}){separator}" +
                                                                $"Shift X ({corUnit.ShiftX.UnitSymbol}){separator}" +
                                                                $"Shift Y ({corUnit.ShiftY.UnitSymbol})");
                                    file.WriteLine(header);

                                    //Results for file
                                    foreach (var item in corrections)
                                    {
                                        var listResults = string.Format($"{item.XTheoricalPosition.GetValueAs(corUnit.XTheoricalPosition.Unit)}{separator}" +
                                                                        $"{item.YTheoricalPosition.GetValueAs(corUnit.YTheoricalPosition.Unit)}{separator}" +
                                                                        $"{item.ShiftX.GetValueAs(corUnit.ShiftX.Unit)}{separator}" +
                                                                        $"{item.ShiftY.GetValueAs(corUnit.ShiftY.Unit)}");
                                        file.WriteLine(listResults);
                                    }
                                }
                            }
                            entryNameIndex++;
                        }
                    }
                }
            }
            else
            {
                string directoryName = Path.GetDirectoryName(exportFilePath);
                Directory.CreateDirectory(directoryName);
                int entryNameIndex = 1;
                foreach (var corrections in correctionsList)
                {
                    string fileName;

                    if (correctionsList.Count == 1)
                    {
                        fileName = exportFilePath + "_xycalib.csv";
                    }
                    else
                    {
                        fileName = exportFilePath + $"_xycalib_{entryNameIndex}.csv";
                    }

                    using (StreamWriter file = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        //Header for file
                        var corUnit = corrections.FirstOrDefault();

                        string separator = CSVStringBuilder.GetCSVSeparator();
                        var header = string.Format($"X position ({corUnit.XTheoricalPosition.UnitSymbol}){separator}" +
                                                    $"Y position ({corUnit.YTheoricalPosition.UnitSymbol}){separator}" +
                                                    $"Shift X ({corUnit.ShiftX.UnitSymbol}){separator}" +
                                                    $"Shift Y ({corUnit.ShiftY.UnitSymbol}){separator}");
                        file.WriteLine(header);

                        //Results for file
                        foreach (var item in corrections)
                        {
                            var listResults = string.Format($"{item.XTheoricalPosition.GetValueAs(corUnit.XTheoricalPosition.Unit)}{separator}" +
                                                            $"{item.YTheoricalPosition.GetValueAs(corUnit.YTheoricalPosition.Unit)}{separator}" +
                                                            $"{item.ShiftX.GetValueAs(corUnit.ShiftX.Unit)}{separator}" +
                                                            $"{item.ShiftY.GetValueAs(corUnit.ShiftY.Unit)}");
                            file.WriteLine(listResults);
                        }
                    }
                    entryNameIndex++;
                }
            }
        }

 
        private void UpdatePrecision()
        {
            if (SelectedRecipe != null)
            {
                int measureEveryNbDie = 0;

                // Find max precision
                int nbOfUsedDies = DieIndexSelector.SelectDiesOnGrid(SelectedRecipe.WaferMap, 1, SelectedScanDirection).Count;
                while (nbOfUsedDies >= MinNumberOfCalibrationPoints)
                {
                    measureEveryNbDie++;
                    nbOfUsedDies = DieIndexSelector.SelectDiesOnGrid(SelectedRecipe.WaferMap, measureEveryNbDie + 1, SelectedScanDirection).Count;
                }

                MaxPrecision = measureEveryNbDie;
                SelectedPrecision = MaxPrecision;
                MaxTestPrecision = MaxPrecision;
                SelectedTestPrecision = SelectedPrecision;
            }
            else
            {
                MaxPrecision = 0;
                SelectedPrecision = 0;
                MaxTestPrecision = 0;
                SelectedTestPrecision = 0;
            }
        }

        private void XYCalibrationTestProgressEvent(string progress, ProgressType progressType)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                if (progressType == ProgressType.Error)
                {
                    CalibrationTestInProgress = false;
                    CurrentCalibrationTest = null;
                    SelectedTestResult = null;
                    TestResults.Clear();
                    ClassLocator.Default.GetInstance<IMessenger>().Send(new Message(MessageLevel.Error, progress));
                }
                else
                {
                    CurrentCalibrationTest.Info = progress;
                }
            });
        }

        private void XYCalibrationChangedEvent(XYCalibrationData xYCalibrationData)
        {
            UpdateCalibration(xYCalibrationData);
            if (StartTestAfterCalibration)
                StartAllTestCalibration.Execute(null);
        }

        private void BWAChangedEvent(BareWaferAlignmentChangeInfo bwaResult)
        {
            //var res = bwaResult as BareWaferAlignmentResult;
        }

        public override void Dispose()
        {
            _calibrationSupervisor.XYCalibrationProgressEvent -= XYCalibrationProgressEvent;
            _calibrationSupervisor.XYCalibrationChangedEvent -= XYCalibrationChangedEvent;
            _calibrationSupervisor.XYCalibrationTestProgressEvent -= XYCalibrationTestProgressEvent;
            _calibrationSupervisor.XYCalibrationTestChangedEvent -= XYCalibrationTestChangedEvent;
            _algoSupervisor.BWAChangedEvent -= BWAChangedEvent;
            XYVectorHeatMapVM?.Dispose();
            TestResults.ToList().ForEach(x => x.XYVectorHeatMapVMTest.Dispose());
            XYVectorHeatMapVM = null;
            if (ExportResultVM != null)
            {
                ExportResultVM.OnSaveExportCommand -= OnSaveExport;
                ExportResultVM = null;
            }
            if (ExportTestResultVM != null)
            {
                ExportTestResultVM.OnSaveExportCommand -= OnSaveExportTest;
                ExportTestResultVM = null;
            }
        }

        public override void Init()
        {

            var xyCalibration = _calibrationSupervisor.GetCalibrations()?.Result.FirstOrDefault(c => c is XYCalibrationData);
            if (!(xyCalibration is null))
            {
                UpdateCalibration(xyCalibration as XYCalibrationData);
            }
            HasChanged = false;
        }

        public override void CancelChanges()
        {
            Init();
            TestResults.Clear();
        }

        public override bool CanCancelChanges()
        {
            if (CalibrationInProgress || CalibrationTestInProgress)
                return false;
            if (!HasChanged)
                return false;
            var xyCalibration = _calibrationSupervisor.GetCalibrations()?.Result.FirstOrDefault(c => c is XYCalibrationData);
            if (!(xyCalibration is null))
                return true;

            return false;
        }

        public override void Save()
        {
            CalibrationData.User = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser?.Name;
            _calibrationSupervisor.SaveCalibration(CalibrationData);
            HasChanged = false;
        }

        public override bool CanSave()
        {
            return CalibrationIsDefined && HasChanged;
        }

        public override void UpdateCalibration(ICalibrationData calibrationData)
        {
            if (calibrationData is XYCalibrationData xYCalibrationData)
            {
                CalibrationInProgress = false;
                CalibrationData = xYCalibrationData;

                SelectedRecipe = Recipes.Find(x => x.Name == CalibrationData.Input.RecipeName);

                double specValueMin = XYCalibrationResultDisplayVM.StagePrecisionScaleFactor * -XYCalibrationResultDisplayVM.StagePrecision.Micrometers;  // um NST7 stage precision * factor
                double specValueMax = XYCalibrationResultDisplayVM.StagePrecisionScaleFactor * XYCalibrationResultDisplayVM.StagePrecision.Micrometers; // um NST7 stage precision * factor
                XYVectorHeatMapVM.Update(xYCalibrationData, specValueMin, specValueMax);

                _contextSupervisor.Apply(xYCalibrationData.Input.ImageAcquisitionContext);
                AutoFocusIsEnabled = xYCalibrationData.Input.UseAutoFocus;
            }
        }

  
        private void DoCalibrationXY()
        {
            try
            {
                TestResults.ToList().ForEach(x => x.XYVectorHeatMapVMTest.Dispose());
                TestResults.Clear();
                CalibrationProgress = string.Empty;
                CalibrationInProgress = true;
                CalibrationData = null;
                HasChanged = true;
                var input = new XYCalibrationInput
                (
                    AutoFocusIsEnabled,
                    _contextSupervisor.GetTopImageAcquisitionContext()?.Result,
                    SelectedRecipe.Name,
                    (MaxPrecision - SelectedPrecision) + 1,
                    SelectedScanDirection
                );
                input.InitialShiftCenterX = InitShiftCenterX;
                input.InitialShiftCenterY = InitShiftCenterY;
                string userName = ClassLocator.Default.GetInstance<IUserSupervisor>().CurrentUser?.Name;
                _calibrationSupervisor.StartXYCalibration(input, userName);
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Error during XY calibration");
                });
            }
        }

  

        private void StartTestCalibration()
        {
            var newCalibrationTest = new XYCalibrationTestResultVM(TestResults.Count() + 1);
            newCalibrationTest.XYVectorHeatMapVMTest = new XYCalibResultVectorHeatMapVM(_gridVectorMin, _gridHeatMapMin);
            TestResults.Add(newCalibrationTest);
            CurrentCalibrationTest = newCalibrationTest;
            _nbOfRemainingTests = _nbOfRemainingTests - 1;
            Task.Factory.StartNew(() =>
            {
                CalibrationData.Input.EveryNbDie = (MaxPrecision - SelectedTestPrecision) + 1;
                _calibrationSupervisor.StartXYCalibrationTest(CalibrationData);
            });
        }

  

        private void XYCalibrationTestChangedEvent(XYCalibrationTest xYCalibrationTest)
        {
            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                CurrentCalibrationTest.SetResult(xYCalibrationTest, CalibrationData);

                double specValueMin = XYCalibrationResultDisplayVM.StagePrecisionScaleFactorTest * -XYCalibrationResultDisplayVM.StagePrecision.Micrometers;  // um NST7 stage precision * factor
                double specValueMax = XYCalibrationResultDisplayVM.StagePrecisionScaleFactorTest * XYCalibrationResultDisplayVM.StagePrecision.Micrometers; // um NST7 stage precision * factor

                CurrentCalibrationTest.XYVectorHeatMapVMTest.Update(xYCalibrationTest, specValueMin, specValueMax);
                SelectedTestResult = CurrentCalibrationTest;
                if (_nbOfRemainingTests <= 0)
                {
                    CalibrationTestInProgress = false;
                    CalibrationTestIsDefined = true;
                }
                else
                {
                    StartTestCalibration();
                }
            });
        }

        private void XYCalibrationProgressEvent(string progress, ProgressType progressType)
        {
            if (progressType == ProgressType.Error)
            {
                CalibrationInProgress = false;
                ClassLocator.Default.GetInstance<IMessenger>().Send(new Message(MessageLevel.Error, progress));
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("The calibration failed : " + progress, "XY Calibration", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None);
                }));
            }
            else
            {
                CalibrationProgress = progress;
            }
        }


        #endregion Methods

        #region Commands


        private AutoRelayCommand _startAllTestCalibration;

        public AutoRelayCommand StartAllTestCalibration
        {
            get
            {
                return _startAllTestCalibration ?? (_startAllTestCalibration = new AutoRelayCommand(
              () =>
              {
                  if (!_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                  {
                      //The wafer must be clamped before calibration.
                      _chuckSupervisor.ClampWafer(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic);
                      Thread.Sleep(1000);
                      if (_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                      {
                          CalibrationTestInProgress = true;
                          _nbOfRemainingTests = NbOfTestRepetition;
                          StartTestCalibration();
                      }
                      else
                      {
                          ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("We cannot clamp the wafer", "Wafer", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
                          return;
                      }
                  }
                  else
                  {
                      CalibrationTestInProgress = true;
                      _nbOfRemainingTests = NbOfTestRepetition;
                      StartTestCalibration();
                  }
              },
              () => { return !CalibrationInProgress; }));
            }
        }

        private AutoRelayCommand _startCalibration;

        public AutoRelayCommand StartCalibration
        {
            get
            {
                return _startCalibration ?? (_startCalibration = new AutoRelayCommand(
              () =>
              {
                  if (!_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                  {
                      _chuckSupervisor.ClampWafer(_chuckSupervisor.ChuckVM.SelectedWaferCategory.DimentionalCharacteristic);
                      Thread.Sleep(1000);
                      if (_chuckSupervisor.ChuckVM.Status.IsWaferClamped)
                      {
                          DoCalibrationXY();
                      }
                      else
                      {
                          ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("We cannot clamp the wafer", "Wafer", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.None);
                          return;
                      }
                  }
                  else
                  {
                      DoCalibrationXY();
                  }
              },
              () => { return (!(SelectedRecipe is null)) && !CalibrationTestInProgress; }));
            }
        }


        private AutoRelayCommand _displayXYCalibrationResult;

        public AutoRelayCommand DisplayXYCalibrationResult
        {
            get
            {
                return _displayXYCalibrationResult ?? (_displayXYCalibrationResult = new AutoRelayCommand(
              () =>
              {
                  var resultDisplayVM = new XYCalibrationResultDisplayVM(_calibrationData);
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<XYCalibrationResultDisplay>(resultDisplayVM);
              },
              () => { return CalibrationIsDefined; }));
            }
        }

        private AutoRelayCommand _displayXYCalibrationTest;

        public AutoRelayCommand DisplayXYCalibrationTest
        {
            get
            {
                return _displayXYCalibrationTest ?? (_displayXYCalibrationTest = new AutoRelayCommand(
              () =>
              {
                  var resultDisplayVM = new XYCalibrationResultDisplayVM(SelectedTestResult.TestCalibResult, _calibrationData, SelectedTestResult.DisplayTestLabel);
                  ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowDialog<XYCalibrationResultDisplay>(resultDisplayVM);
              },
              () => { return SelectedTestResult != null; }));
            }
        }

        private AutoRelayCommand _exportResultCommand;

        public AutoRelayCommand ExportResultCommand
        {
            get
            {
                return _exportResultCommand ?? (_exportResultCommand = new AutoRelayCommand(
              () =>
              {
                  ExportResultVM.GenerateNewTargetPath();
                  ExportResultVM.IsStayPopup = true;
              },
              () => { return CalibrationIsDefined; }));
            }
        }

        private AutoRelayCommand _exportTestResultCommand;

        public AutoRelayCommand ExportTestResultCommand
        {
            get
            {
                return _exportTestResultCommand ?? (_exportTestResultCommand = new AutoRelayCommand(
              () =>
              {
                  ExportTestResultVM.GenerateNewTargetPath();
                  ExportTestResultVM.IsStayPopup = true;
              },
              () => { return SelectedTestResult != null; }));
            }
        }

        private AutoRelayCommand _stopCalibration;

        public AutoRelayCommand StopCalibration
        {
            get
            {
                return _stopCalibration ?? (_stopCalibration = new AutoRelayCommand(
                    () =>
                    {
                        _calibrationSupervisor.StopXYCalibration();
                    },
                    () => { return CalibrationInProgress; }
                ));
            }
        }

        private AutoRelayCommand _stopAllTestCalibration;

        public AutoRelayCommand StopAllTestCalibration
        {
            get
            {
                return _stopAllTestCalibration ?? (_stopAllTestCalibration = new AutoRelayCommand(
                    () =>
                    {
                        _calibrationSupervisor.StopXYCalibration();
                    },
                    () => { return CalibrationTestInProgress; }
                ));
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
                  try
                  {
                      Save();
                  }
                  catch (Exception ex)
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Error during save");
                  }
                  finally
                  {
                      _calibrationSupervisor.UpdateCalibrationCache();
                  }
              },
              () => { return CanSave(); }));
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

                  var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Calibration has changed. Do you really want to undo change", "Undo calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                  if (res == MessageBoxResult.Yes)
                  {
                      CancelChanges();
                  }
              },
              () => { return (HasChanged) && (CanCancelChanges()); }));
            }
        }

        #endregion Commands

        #region INavigable implementation

        public override Task PrepareToDisplay()
        {
            // Disable referentials converter
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(PM.Shared.Referentials.Interface.ReferentialTag.Stage, PM.Shared.Referentials.Interface.ReferentialTag.Motor);
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(PM.Shared.Referentials.Interface.ReferentialTag.Motor, PM.Shared.Referentials.Interface.ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(PM.Shared.Referentials.Interface.ReferentialTag.Wafer, PM.Shared.Referentials.Interface.ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.DisableReferentialConverter(PM.Shared.Referentials.Interface.ReferentialTag.Stage, PM.Shared.Referentials.Interface.ReferentialTag.Wafer);


            IsProbeLiseAcquiring = true;
            return Task.CompletedTask;
        }

        private void LeaveDisplay()
        {
            // Re-enable referentials converter
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Motor);
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Motor, ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Wafer, ReferentialTag.Stage);
            ServiceLocator.ReferentialSupervisor.EnableReferentialConverter(ReferentialTag.Stage, ReferentialTag.Wafer);

            HasChanged = false;
            IsProbeLiseAcquiring = false;

        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            if (HasChanged && !forceClose)
            {
                var dialogRes = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("The objectives calibration has changed. Do you really want to quit without saving ?", "Objectives calibration", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (dialogRes == MessageBoxResult.Yes)
                {
                    Init();
                    LeaveDisplay();
                    return true;
                }
                return false;
            }

            LeaveDisplay();
            return true;
        }

        #endregion INavigable implementation
    }
}
