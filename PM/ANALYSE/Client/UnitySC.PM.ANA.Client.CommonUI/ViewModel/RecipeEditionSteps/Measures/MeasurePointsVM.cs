using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;

using MvvmDialogs.FrameworkDialogs.OpenFile;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement;
using UnitySC.PM.ANA.Client.Controls.Camera;
using UnitySC.PM.ANA.Client.Shared.Helpers;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Geometry;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures
{
    public class MeasurePointsVM : ObservableObject, IDisposable
    {
        private bool? _areAllPointsSelected = false;
        public const string CsvExport = "CSV";

        private IWaferShape _waferShape;

        private ExportPointsVM _exportPoints;
        public ExportPointsVM ExportPoints
        { get => _exportPoints; set { if (_exportPoints != value) { _exportPoints = value; OnPropertyChanged(); } } }

        public RecipeMeasureVM RecipeMeasure { get; set; }

        private CustomPointsManagementVM _customPointsManagement;

        public CustomPointsManagementVM CustomPointsManagement
        {
            get => _customPointsManagement;
            set => SetProperty(ref _customPointsManagement, value);
        }

        private ObservableCollection<ObjectiveConfig> _objectives;

        public ObservableCollection<ObjectiveConfig> Objectives
        {
            get => _objectives; set { if (_objectives != value) { _objectives = value; OnPropertyChanged(); } }
        }

        private ObjectiveConfig _selectedObjective;

        public ObjectiveConfig SelectedObjective
        {
            get => _selectedObjective;
            set
            {
                if (_selectedObjective != value)
                {
                    _selectedObjective = value;
                    OnPropertyChanged();
                    if (_selectedObjective != null)
                    {
                        _ = SetSelectedObjectiveAsync(_selectedObjective.DeviceID);
                    }
                }
            }
        }

        public MeasurePointsVM(RecipeMeasureVM recipeMeasure)
        {
            RecipeMeasure = recipeMeasure;
            _waferShape = WaferUtils.CreateWaferShapeFromWaferCharacteristics(RecipeMeasure.EditedRecipe.WaferDimentionalCharacteristic, 0.Millimeters());
        }

        public async Task PrepareToDisplayAsync()
        {
            await Task.Run(() => InitPatternRecObjectives());
        }

        public void DisplayMeasurePointsTab()
        {
            //If the POINTS LIST tab is selected, the ROI must be forced to be displayed.
            RecipeMeasure.DisplayROI = true;
            RecipeMeasure.IsCenteredRoi = false;
            var objectifUsedByMeasure = RecipeMeasure.MeasureSettings.GetObjectiveUsedByMeasure();
            SelectedObjective = objectifUsedByMeasure ?? ServiceLocator.CamerasSupervisor.Objective;
        }

        public void AddMeasurePoints(List<XYZTopZBottomPosition> pointPositions, bool clearPrevious = false, bool applyEdgeExclusion = true)
        {
            if (clearPrevious)
            {
                Points.Clear();
            }   

            var point = new PointUnits();
            foreach (var position in pointPositions)
            {
                point.X = position.X.Millimeters();
                point.Y = position.Y.Millimeters();
                if (_waferShape.IsInside(point, applyEdgeExclusion))
                {
                    var newMeasurePoint = new MeasurePointVM(this, position, GetNextPointId(), RecipeMeasure.MeasureSettings.ArePositionsOnDie);
                    Points.Add(newMeasurePoint);
                }
            }
        }

        private void OnSaveExport()
        {
            var pointPositions = Points.Select(s => s.PointPosition).ToList<XYZTopZBottomPosition>();

            if (ExportPoints != null && !ExportPoints.IsExporting)
            {
                ExportPoints.IsExporting = true;

                Task.Run(() =>
                {
                    try
                    {
                        ExportPointsToCSV(pointPositions);
                    }
                    catch (Exception ex)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "Error during export");
                        });
                    }
                    finally
                    {
                        ExportPoints.IsExporting = false;
                        ExportPoints.IsStayPopup = false;
                        ExportPoints.OnSaveExportCommand -= OnSaveExport;
                    }
                }).ConfigureAwait(false);
            }
        }
 
        private void ExportPointsToCSV(List<XYZTopZBottomPosition> pointPositions)
        {
            try
            {
                string exportFilePath = ExportPoints.GetTargetFullPath();

                string directoryName = Path.GetDirectoryName(exportFilePath);
                Directory.CreateDirectory(directoryName);


                string fileName = exportFilePath + ".csv";

                try
                {
                    PointsCsvFiles.SavePointsToFile(fileName, pointPositions, RecipeMeasure.EditedRecipe.WaferDimentionalCharacteristic);
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Points have been exported with success.", "Points Export", MessageBoxButton.OK, MessageBoxImage.Information);
                    });
                }
                catch (Exception ex)
                {
                    Application.Current?.Dispatcher.Invoke(() =>
                    {
                        ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to export the points", "Points Export", MessageBoxButton.OK, MessageBoxImage.Error);
                        var logger = ClassLocator.Default.GetInstance<ILogger>();
                        logger.Error($"Failed to export the Points {ex.Message}");
                    });
                }
            }
            catch (Exception ex)
            {
                Application.Current?.Dispatcher.Invoke(() =>
                {
                    ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "ExportPointsToCSV: Error during points export");
                });
            }

        }

        private ObservableCollection<MeasurePointVM> _points;

        public ObservableCollection<MeasurePointVM> Points
        {
            get
            {
                if (_points is null)
                {
                    _points = new ObservableCollection<MeasurePointVM>();
                    _points.CollectionChanged += MeasurePoints_CollectionChanged;
                }
                return _points;
            }
            set { if (_points != value) { _points = value; OnPropertyChanged(); } }
        }

        private List<Point> _displayPoints;

        // Coordinates of the points in wafer referential
        public List<Point> DisplayPoints
        {
            get
            {
                if (_displayPoints is null)
                {
                    _displayPoints = new List<Point>();

                }
                return _displayPoints;
            }
            set { if (_displayPoints != value) { _displayPoints = value; OnPropertyChanged(); } }
        }

        private List<Point> _displayPointsOnDies;

        // Coordinates of the points in wafer referential
        public List<Point> DisplayPointsOnDie
        {
            get
            {
                if (_displayPointsOnDies is null)
                {
                    _displayPointsOnDies = new List<Point>();

                }
                return _displayPointsOnDies;
            }
            set { if (_displayPointsOnDies != value) { _displayPointsOnDies = value; OnPropertyChanged(); } }
        }

        private bool _canAddPointsWithImages = true;

        public bool CanAddPointsWithImages
        {
            get => _canAddPointsWithImages;
            set => SetProperty(ref _canAddPointsWithImages, value);
        }

        private void UpdateDisplayPoints()
        {
            DisplayPoints = new List<Point>();
            foreach (var point in Points)
            {
                DisplayPoints.Add(point.DisplayPosition);
            }
        }

        private void UpdateDisplayPointsOnDie()
        {
            DisplayPointsOnDie = new List<Point>();
            foreach (var point in Points)
            {
                if (!(point.DisplayPositionOnDie is null))
                    DisplayPointsOnDie.Add((Point)point.DisplayPositionOnDie);
            }
        }

        private void MeasurePoints_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdatePointIndexes();

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var measure in Points)
                {
                    (measure as MeasurePointVM).PropertyChanged -= MeasurePoint_PropertyChanged;
                }
            }

            if (!(e.NewItems is null))
            {
                foreach (var measure in e.NewItems)
                {
                    (measure as MeasurePointVM).PropertyChanged += MeasurePoint_PropertyChanged;
                }
            }

            if (!(e.OldItems is null))
            {
                foreach (var measure in e.OldItems)
                {
                    (measure as MeasurePointVM).PropertyChanged -= MeasurePoint_PropertyChanged;
                }
            }

            UpdateDisplayPoints();
            UpdateDisplayPointsOnDie();
            OnPropertyChanged(nameof(CameraDisplayPoints));
            IsModified = true;
        }

        public ObservableCollection<MeasureSettingsBase> MeasuresToImportPoints
        {
            get
            {
                MeasurePointsToImport = null;

                return new ObservableCollection<MeasureSettingsBase>(RecipeMeasure.EditedRecipe.Measures.Where(mp => (mp.Name != RecipeMeasure.Name) && (mp.MeasurePoints.Count > 0) && mp.IsConfigured));
            }
        }

        public MeasureSettingsBase MeasurePointsToImport
        {
            set
            {
                var val = value;
                if (!(value is null))
                    ImportPointsFromMeasure(value);
            }
        }

        private bool _isModified = false;

        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isReadyToAddPointWithImage = true;

        public bool IsReadyToAddPointWithImage
        {
            get => _isReadyToAddPointWithImage;
            set => SetProperty(ref _isReadyToAddPointWithImage, value);
        }

        private void ImportPointsFromMeasure(MeasureSettingsBase measure)
        {
            if (measure.MeasurePoints is null)
                return;  // Nothing to import

            foreach (var measurePoint in measure.MeasurePoints)
            {
                Points.Add(MeasurePointVM.NewFromMeasurePoint(this, RecipeMeasure.EditedRecipe.Points.Find(p => p.Id == measurePoint), RecipeMeasure.MeasureSettings.ArePositionsOnDie, GetNextPointId()));
            }

            MeasurePointsToImport = null;
        }

        private void UpdatePointIndexes()
        {
            foreach (var measurePoint in Points)
            {
                measurePoint.Index = Points.IndexOf(measurePoint) + 1;
            }
        }

        public void InitPatternRecObjectives()
        {
            var objectiveConfigList = ServiceLocator.ProbesSupervisor.ObjectivesSelectors.FirstOrDefault(
                os => os.DeviceID == ServiceLocator.CamerasSupervisor.Camera.Configuration.ObjectivesSelectorID)
                .Objectives;
            if (objectiveConfigList is null)
            {
                Objectives = new ObservableCollection<ObjectiveConfig>();
            }
            else
            {
                Objectives = new ObservableCollection<ObjectiveConfig>(objectiveConfigList);
            }
        }

        public async Task SetSelectedObjectiveAsync(string objectiveId)
        {
            IsReadyToAddPointWithImage = false;
            if (string.IsNullOrWhiteSpace(objectiveId))
            {
                return;
            }
            else
            {
                if (Objectives is null || Objectives.Count == 0)
                {
                    return;
                }
                else
                {
                    var objectiveConfigToSelect = Objectives.FirstOrDefault(o => o.DeviceID == objectiveId);
                    if (objectiveConfigToSelect != null)
                    {
                        Console.WriteLine("Before Objective affectation");
                        ServiceLocator.CamerasSupervisor.Objective = objectiveConfigToSelect;
                    }
                }
            }

            await ServiceLocator.CamerasSupervisor.WaitObjectiveChanged(objectiveId, true);
            Console.WriteLine("After SetSelectedObjectiveAsync");

            IsReadyToAddPointWithImage = true;
        }

        // Clone of the measure points for the camera display
        public ObservableCollection<ICameraDisplayPoint> CameraDisplayPoints => new ObservableCollection<ICameraDisplayPoint>(Points.Cast<ICameraDisplayPoint>());

        public int GetNextPointId()
        {
            var editedRecipe = RecipeMeasure.EditedRecipe;

            int highestSiteIdInRecipe = 0;

            if (editedRecipe.Points.Count > 0)
                highestSiteIdInRecipe = editedRecipe.Points.Max(p => p.Id);

            int highestSiteIdInCurrentMeasure = Points.Max(p => p.Id) ?? 0;

            return Math.Max(highestSiteIdInRecipe, highestSiteIdInCurrentMeasure) + 1;
        }

        public MeasurePointVM CreateMeasurePointOnCurrentPosition()
        {
            return new MeasurePointVM(this, GetCurrentPositionOnRelevantReferential(), GetNextPointId(), RecipeMeasure.MeasureSettings.ArePositionsOnDie);
        }

        private ReferentialTag GetRelevantReferential()
        {
            // If we dont use a waferMap we store wafer positions instead of positions in die
            return RecipeMeasure.MeasureSettings.ArePositionsOnDie ? ReferentialTag.Die : ReferentialTag.Wafer;
        }

        public XYZTopZBottomPosition GetCurrentPositionOnRelevantReferential()
        {
            var stagePosition = ServiceLocator.AxesSupervisor.GetCurrentPosition()?.Result;
            if (stagePosition == null)
            {
                throw new Exception("Impossible to get current axes position");
            }

            PositionBase positionOnReferential = ServiceLocator.ReferentialSupervisor.ConvertTo(stagePosition, GetRelevantReferential())?.Result;
            if (positionOnReferential == null)
            {
                throw new Exception("Impossible to convert current axes position referential");
            }

            return positionOnReferential.ToXYZTopZBottomPosition();
        }

        private void MeasurePoint_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AreAllPointsSelected));
            if (e.PropertyName == nameof(MeasurePointVM.PointPosition) || e.PropertyName == nameof(MeasurePointVM.PointPatternRec))
            {
                IsModified = true;
            }
        }

        #region RelayCommands

        private AutoRelayCommand _renumberAllPoints;

        public AutoRelayCommand RenumberAllPoints
        {
            get
            {
                return _renumberAllPoints ?? (_renumberAllPoints = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            if (MessageBox.Show("Reseting will optimize All Recipe Site IDs for all measurements.\r\nPlease note that previous results based on these IDs may no longer be directly comparable." +
                                Environment.NewLine + Environment.NewLine + "Do you wish to proceed?",
                                "Important",
                                MessageBoxButton.OKCancel,
                                MessageBoxImage.Warning,
                                MessageBoxResult.Cancel) == MessageBoxResult.Cancel)
                            {
                                return;
                            }

                            var allMeasures = RecipeMeasure.EditedRecipe.Measures;
                            var allRecipeSavedPoints = RecipeMeasure.EditedRecipe.Points;

                            // First we remove all potential duplicate of id in recipe points collection
                            RemoveGhostRecipePoints(allMeasures, allRecipeSavedPoints);

                            int measureNewId = 1;
                            var allRecipePointsAndNewId = new Dictionary<int, MeasurePoint>();
                            // Loop on all measures to update each measure measurePoints and to build a list with recipe points and their new id
                            allMeasures.ForEach(measure =>
                            {
                                var measurePointIds = measure.IsMeasureWithSubMeasurePoints ? measure.SubMeasurePoints : measure.MeasurePoints;
                                var measurePointIdsNew = new List<int>();

                                if (measure.Name == RecipeMeasure.Name)
                                {
                                    // Loop on all MeasurePointVM for current measure
                                    // Because points collection in recipe might not be up to date if points have been added or deleted
                                    foreach (var measurePointVM in Points)
                                    {
                                        var recipePoint = allRecipeSavedPoints.FirstOrDefault(p => p.Id == measurePointVM.Id);
                                        if (recipePoint != null)
                                        {
                                            allRecipePointsAndNewId.Add(measureNewId, recipePoint);
                                            measurePointIdsNew.Add(measureNewId);
                                        }

                                        // Update measurePointVM for current MeasurePointsVM
                                        measurePointVM.Id = measureNewId++;
                                        measurePointVM.IsModified = true;
                                    }
                                }
                                else
                                {
                                    // Loop on all measure point Ids for other measures
                                    measurePointIds.ForEach(measurePointId =>
                                    {
                                        var measurePoint = allRecipeSavedPoints.FirstOrDefault(p => p.Id == measurePointId);
                                        if (measurePoint != null)
                                        {
                                            allRecipePointsAndNewId.Add(measureNewId, measurePoint);
                                            measurePointIdsNew.Add(measureNewId++);
                                        }
                                    });
                                }

                                // Update measure MeasurePoints/SubMeasurePoints
                                measurePointIds.Clear();
                                measurePointIdsNew.ForEach(id => measurePointIds.Add(id));
                            });

                            // Finally update recipe points Id
                            foreach (var item in allRecipePointsAndNewId)
                            {
                                item.Value.Id = item.Key;
                            }

                            IsModified = true;
                        }
                        catch (Exception ex)
                        {
                            ClassLocator.Default.GetInstance<ILogger>().Error(ex, "RenumberAllPoints: Error during points renumbering");
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _addPoint;

        public AutoRelayCommand AddPoint
        {
            get
            {
                return _addPoint ?? (_addPoint = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            var newSiteId = GetNextPointId();
                            var newMeasurePoint = new MeasurePointVM(this, GetCurrentPositionOnRelevantReferential(), newSiteId, RecipeMeasure.MeasureSettings.ArePositionsOnDie);
                            Points.Add(newMeasurePoint);
                        }
                        catch (Exception ex)
                        {
                            ClassLocator.Default.GetInstance<ILogger>().Error(ex, "AddPoint: Error during point creation");
                        }
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _addPointWithImage;

        public AutoRelayCommand AddPointWithImage
        {
            get
            {
                return _addPointWithImage ?? (_addPointWithImage = new AutoRelayCommand(
                    () =>
                    {
                        try
                        {
                            Rect rectRoi;
                            if (RecipeMeasure.IsCenteredRoi)
                            {
                                rectRoi = new Rect(RecipeMeasure.RoiSize);
                            }
                            else
                            {
                                rectRoi = RecipeMeasure.RoiRect;
                            }

                            var newPatternRecImage = PatternRecHelpers.CreatePatternRectWithContext(rectRoi, RecipeMeasure.IsCenteredRoi);
                            var newMeasurePointWithImage = new MeasurePointVM(this, newPatternRecImage, GetCurrentPositionOnRelevantReferential(), GetNextPointId(), RecipeMeasure.MeasureSettings.ArePositionsOnDie);
                            Points.Add(newMeasurePointWithImage);
                        }
                        catch (Exception ex)
                        {
                            Application.Current?.Dispatcher.Invoke(() =>
                            {
                                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowException(ex, "AddPointWithImage: Error during point with image creation");
                            });
                        }
                    },
                    () => { return CanAddPointsWithImages; }
                ));
            }
        }

        private AutoRelayCommand _deleteSelectedPoints;

        public AutoRelayCommand DeleteSelectedPoints
        {
            get
            {
                return _deleteSelectedPoints ?? (_deleteSelectedPoints = new AutoRelayCommand(
                    () =>
                    {
                        foreach (var selectedPoint in Points.Where(s => s.IsSelected).ToList())
                        {
                            DeletePoint(selectedPoint);
                            OnPropertyChanged(nameof(AreAllPointsSelected));
                        }
                    },
                    () => { return AreAllPointsSelected != false; }
                ));
            }
        }

        private AutoRelayCommand _importPointsFromFileCommand;
        public AutoRelayCommand ImportPointsFromFileCommand
        {
            get
            {
                return _importPointsFromFileCommand ?? (_importPointsFromFileCommand = new AutoRelayCommand(
              () =>
              {

                  var dialogService = ClassLocator.Default.GetInstance<IDialogOwnerService>();
                  var settings = new OpenFileDialogSettings
                  {
                      Title = "Open points file",
                      Filter = "ANALYSE points file (*.csv)|*.csv",
                  };

                  bool? result = dialogService.ShowOpenFileDialog(settings);
                  if (result.HasValue && result.Value)
                  {
                      var importedPoints=PointsCsvFiles.LoadPointsFromFile(settings.FileName);

                      var point = new UnitySC.Shared.Data.Geometry.PointUnits();
                      foreach (var importedPoint in importedPoints)
                      {
                          point.X = importedPoint.X.Millimeters();
                          point.Y = importedPoint.Y.Millimeters();
                          if (_waferShape.IsInside(point))
                          {
                              var newMeasurePoint = new MeasurePointVM(this, importedPoint, GetNextPointId(), RecipeMeasure.MeasureSettings.ArePositionsOnDie);
                              Points.Add(newMeasurePoint);
                          }
                      }

                  }
              },
              () => true));
            }
        }

        private AutoRelayCommand _exportPointsCommand;
        public AutoRelayCommand ExportPointsCommand
        {
            get
            {
                return _exportPointsCommand ?? (_exportPointsCommand = new AutoRelayCommand(
              () =>
              {
                  if (ExportPoints is null)
                  {
                      ExportPoints = new ExportPointsVM();
                      ExportPoints.OnSaveExportCommand += OnSaveExport;
                  }
                  ExportPoints.GenerateNewTargetPath();
                  ExportPoints.IsStayPopup = true;
              },
              () => { return Points.Count > 0; }));
            }
        }

        #endregion RelayCommands

        private void RemoveGhostRecipePoints(List<MeasureSettingsBase> allMeasures, List<MeasurePoint> allRecipeSavedPoints)
        {
            // First gather all points referenced in measures
            var allMeasureSavedPointIds = new List<int>();
            allMeasures.ForEach(measure => allMeasureSavedPointIds.AddRange(measure.IsMeasureWithSubMeasurePoints ? measure.SubMeasurePoints : measure.MeasurePoints));

            // Initialize a dictionary containing all points allready saved in recipe with a state value false
            var allRecipeSavedPointsUsedInMeasuresState = new Dictionary<MeasurePoint, bool>();
            allRecipeSavedPoints.ForEach(p => allRecipeSavedPointsUsedInMeasuresState.Add(p, false));

            // Iterate on all points referenced in measures and set to true the state of the first point with the same id found in recipe saved points
            // The goal here is to keep the first occurence in case of duplicate
            allMeasureSavedPointIds.ForEach(id =>
            {
                var firstPointSavedInRecipe = allRecipeSavedPoints.FirstOrDefault(m => m.Id == id);
                if (firstPointSavedInRecipe != null)
                {
                    allRecipeSavedPointsUsedInMeasuresState[firstPointSavedInRecipe] = true;
                }
            });

            // Finally we remove all points remaining with state "false"
            var ghostPoints = allRecipeSavedPointsUsedInMeasuresState.Where(p => !p.Value)?.ToList();
            ghostPoints.ForEach(p => allRecipeSavedPoints.Remove(p.Key));
        }

        public bool? AreAllPointsSelected
        {
            get
            {
                if (!Points.Any(m => m.IsSelected))
                    _areAllPointsSelected = false;
                else
                {
                    if (Points.Any(m => !m.IsSelected))
                        _areAllPointsSelected = null;
                    else
                        _areAllPointsSelected = true;
                }

                return _areAllPointsSelected;
            }

            set
            {
                if (_areAllPointsSelected == value)
                {
                    return;
                }

                _areAllPointsSelected = value;
                foreach (var measure in Points)
                {
                    measure.IsSelected = (bool)_areAllPointsSelected;
                }

                OnPropertyChanged();
            }
        }

        internal void DeletePoint(MeasurePointVM measurePoint)
        {
            measurePoint.PropertyChanged -= MeasurePoint_PropertyChanged;
            Points.Remove(measurePoint);
            measurePoint.Dispose();
        }

        internal void UpdatePoint(MeasurePointVM measurePoint)
        {
            OnPropertyChanged(nameof(CameraDisplayPoints));
            UpdateDisplayPoints();
            UpdateDisplayPointsOnDie();
        }

        public void Dispose()
        {
            if (!(_points is null))
            {
                foreach (var point in _points)
                {
                    point.Dispose();
                }
                _points.Clear();
                IsModified = false;
                _points.CollectionChanged -= MeasurePoints_CollectionChanged;
            }
            if (!(ExportPoints is null))
                ExportPoints.OnSaveExportCommand -= OnSaveExport;
        }
    }
}
