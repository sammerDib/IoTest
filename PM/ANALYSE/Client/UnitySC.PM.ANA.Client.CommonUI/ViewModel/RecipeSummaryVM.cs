using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.PM.ANA.Client.CommonUI.Helpers;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure.MeasureSettings;
using UnitySC.PM.ANA.Service.Interface.Referential;
using UnitySC.PM.ANA.Shared;
using UnitySC.PM.Shared.Referentials.Interface;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel
{
    public class RecipeSummaryVM : ObservableObject
    {
        public class AdvancedInfo
        {
            public AdvancedInfo(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class MeasureSummary
        {
            public BitmapImage Image { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public bool IsConfigured { get; set; }
            public string Description { get; set; }
            public List<string> Probes { get; set; }
            public List<string> Objectives { get; set; }
            public List<string> Lights { get; set; }
            public string Autofocus { get; set; }
            public string PatternRec { get; set; }
            public int Points { get; set; }
            public int PointsPerDie { get; set; }
            public int Repeat { get; set; }
            public List<AdvancedInfo> AdvancedInfos { get; set; } = new List<AdvancedInfo>();
        }

        private UserSupervisor _userSupervisor;

        #region Public Constructors

        public RecipeSummaryVM()
        {
            _userSupervisor = ClassLocator.Default.GetInstance<UserSupervisor>();
            Measures = new ObservableCollection<MeasureSummary>();
        }

        #endregion Public Constructors

        #region Public Properties

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private ANARecipe _displayedRecipe;

        public ANARecipe DisplayedRecipe
        {
            get => _displayedRecipe; 
            set 
            { 
                if (_displayedRecipe != value) 
                {
                    _displayedRecipe = value;
                    RecipeName = _displayedRecipe.Name;
                    OnPropertyChanged(); } }
        }


        private string _recipeName;

        public string RecipeName
        {
            get => _recipeName; 
            set 
            { 
                if (_recipeName != value) 
                {
                    _recipeName = new PathString(value).RemoveInvalidFilePathCharacters("_", false);
                    if (_recipeName != DisplayedRecipe.Name)
                    {
                       
                        RenameRecipeAsync(_recipeName);
                        
                       
                    }
                    OnPropertyChanged(); 
                } 
            }
        }

        private async void RenameRecipeAsync(string newName)
        {
            string previousName = _recipeName;
            try
            {

                IsBusy = true;
                DisplayedRecipe.Name = newName;
                await SaveAndRefreshRecipeAsync();
            }
            catch (Exception)
            {
                ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox("Failed to change the recipe name", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                DisplayedRecipe.Name = previousName;
            }
            finally 
            { 
                IsBusy = false; 
            }
           
        }

        private async Task SaveAndRefreshRecipeAsync()
        {
            await Task.Run(() =>
            {
                ANARecipeHelper.UpdateRecipeWithExternalFiles(DisplayedRecipe, null);
                ServiceLocator.ANARecipeSupervisor.SaveRecipe(DisplayedRecipe);
            });
            var rmViewModel = ClassLocator.Default.GetInstance<RecipesManagementViewModel>();
            rmViewModel.RefreshSelectedRecipe();
        }

        private List<Point> _measurePoints;

        public List<Point> MeasurePoints
        {
            get => _measurePoints; set { if (_measurePoints != value) { _measurePoints = value; OnPropertyChanged(); } }
        }

        private int _points;

        public int Points
        {
            get => _points; set { if (_points != value) { _points = value; OnPropertyChanged(); } }
        }

        private string _selectedDies;

        public string SelectedDies
        {
            get => _selectedDies; set { if (_selectedDies != value) { _selectedDies = value; OnPropertyChanged(); } }
        }

        private int _dies;

        public int Dies
        {
            get => _dies; set { if (_dies != value) { _dies = value; OnPropertyChanged(); } }
        }

        private bool _waferMapIsDefined;

        public bool WaferMapIsDefined
        {
            get => _waferMapIsDefined; set { if (_waferMapIsDefined != value) { _waferMapIsDefined = value; OnPropertyChanged(); } }
        }

        private string _userName;

        public string UserName
        {
            get => _userName; set { if (_userName != value) { _userName = value; OnPropertyChanged(); } }
        }

        private StepStates _globalAF;

        public StepStates GlobalAF
        {
            get => _globalAF; set { if (_globalAF != value) { _globalAF = value; OnPropertyChanged(); } }
        }

        private StepStates _bwa;

        public StepStates BWA
        {
            get => _bwa; set { if (_bwa != value) { _bwa = value; OnPropertyChanged(); } }
        }

        private StepStates _marksAlignment;

        public StepStates MarksAlignment
        {
            get => _marksAlignment; set { if (_marksAlignment != value) { _marksAlignment = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<MeasureSummary> Measures { get; set; }

        #endregion Public Properties
        public void Update(ANARecipe anaRecipe)
        {
            DisplayedRecipe = anaRecipe;
            if (anaRecipe == null)
            {
                throw new Exception("Recipe not found.");
            }

            UserName = _userSupervisor.GetUser(anaRecipe.UserId.Value).Name;

            if (anaRecipe?.Execution?.Alignment != null)
            {
                GlobalAF = anaRecipe.Execution.Alignment.RunAutoFocus ? StepStates.Done : StepStates.NotDone;
                BWA = anaRecipe.Execution.Alignment.RunBwa ? StepStates.Done : StepStates.NotDone;
                MarksAlignment = anaRecipe.Execution.Alignment.RunMarkAlignment ? StepStates.Done : StepStates.NotDone;
            }

            var measuresWithoutSubMeasures = anaRecipe.Measures.Where(m => !m.IsMeasureWithSubMeasurePoints);
            var nbPointsForMeasuresWithoutSubMeasures = 0;
            int? nbSelectedDies = null;
            if (measuresWithoutSubMeasures != null)
            {
                nbPointsForMeasuresWithoutSubMeasures = measuresWithoutSubMeasures.SelectMany(m => m.MeasurePoints).Count();
                WaferMapIsDefined = anaRecipe.WaferMap != null;
                if (WaferMapIsDefined)
                {
                    nbSelectedDies = anaRecipe.Dies.Count();
                    // We count the number of dies
                    int nbDies = 0;
                    var waferMap = anaRecipe.WaferMap.WaferMapData;
                    Parallel.For(0, waferMap.NbColumns, column =>
                    {
                        for (int row = 0; row < waferMap.NbRows; row++)
                        {
                            if (waferMap.DiesPresence.GetValue(row, column))
                            {
                                Interlocked.Increment(ref nbDies);
                            }
                        }
                    });
                    SelectedDies = $"{nbSelectedDies} / {nbDies}";
                    nbPointsForMeasuresWithoutSubMeasures *= nbSelectedDies.Value;
                }
            }

            var measuresWithSubMeasures = anaRecipe.Measures.Where(m => m.IsMeasureWithSubMeasurePoints);
            var nbPointsForMeasuresWithSubMeasures = 0;
            if (measuresWithSubMeasures != null)
            {
                nbPointsForMeasuresWithSubMeasures = measuresWithSubMeasures.SelectMany(m => m.MeasurePoints).Count();
            }

            Points = nbPointsForMeasuresWithoutSubMeasures + nbPointsForMeasuresWithSubMeasures;

            // Update measure points for wafer control
            UpdateMeasurePoints();

            // Measures
            Measures.Clear();
            var pointsWithPatternRec = anaRecipe.Points.Where(x => x.PatternRec != null).Select(x => x.Id);
            foreach (var measureSettings in anaRecipe.Measures)
            {
                var measureAsPatternRec = pointsWithPatternRec.Any(pt => measureSettings.MeasurePoints.Contains(pt));
                AddMeasureSummary(measureSettings, measureAsPatternRec, nbSelectedDies);
            }

            MakeListsUniform();
        }

        // In order to have measure information aligned, we need to have lists with all the same number of items
        private void MakeListsUniform()
        {
            var maxNbProbes = 0;
            var maxNbLights = 0;
            var maxNbObjectives = 0;
            foreach (var measure in Measures)
            {
                maxNbProbes=Math.Max(maxNbProbes, measure.Probes?.Count ?? 0);
                maxNbLights = Math.Max(maxNbLights, measure.Lights?.Count ?? 0);
                maxNbObjectives = Math.Max(maxNbObjectives, measure.Objectives?.Count ?? 0);
            }

            foreach (var measure in Measures)
            {
                if (measure.Probes is null)
                    measure.Probes = new List<string>();
                for (int i = measure.Probes.Count; i < maxNbProbes;i++)
                    measure.Probes.Add("");
                if (measure.Lights is null)
                    measure.Lights = new List<string>();
                for (int i = measure.Lights.Count; i < maxNbLights;i++)
                    measure.Lights.Add("");
                if (measure.Objectives is null)
                    measure.Objectives = new List<string>();
                for (int i = measure.Objectives.Count; i < maxNbObjectives;i++)
                    measure.Objectives.Add("");
            }
        }

        private void UpdateMeasurePoints()
        {
            var newMeasurePoints = new List<Point>();

            foreach (var measure in DisplayedRecipe.Measures)
            {
                foreach (var measurePointIndex in measure.MeasurePoints)
                {
                    var measurePoint = DisplayedRecipe.Points.Find(p => p.Id == measurePointIndex);
                    // Measure with sub measures don't rely on die
                    if (DisplayedRecipe.WaferMap is null || measure.IsMeasureWithSubMeasurePoints)
                    {
                        var positionOnWafer = new XYZTopZBottomPosition(new WaferReferential(), measurePoint.Position.X, measurePoint.Position.Y, measurePoint.Position.ZTop, measurePoint.Position.ZBottom);
                        newMeasurePoints.Add(new Point(positionOnWafer.X, positionOnWafer.Y));
                    }
                    else
                    {
                        foreach (var dieIndex in DisplayedRecipe.Dies)
                        {
                            var dieReferential = new DieReferential(dieIndex.Column, dieIndex.Row);
                            var position = new XYZTopZBottomPosition(dieReferential, measurePoint.Position.X, measurePoint.Position.Y, measurePoint.Position.ZTop, measurePoint.Position.ZBottom);
                            var positionOnWafer = DieToWaferPosition(DisplayedRecipe.WaferMap.WaferMapData, position);

                            newMeasurePoints.Add(new Point(positionOnWafer.X, positionOnWafer.Y));
                        }
                    }
                }
            }
            MeasurePoints = newMeasurePoints;
        }

        private void AddMeasureSummary(MeasureSettingsBase measureSettings, bool measureAsPatternRec, int? selectedDies = null)
        {
            MeasureSummary measureSummary = new MeasureSummary()
            {
                Image = ResourceHelper.GetMeasureImage(measureSettings.MeasureType),
                Description = ResourceHelper.GetMeasureDescription(measureSettings.MeasureType),
                Name = measureSettings.Name,
                IsActive = measureSettings.IsActive,
                IsConfigured=measureSettings.IsConfigured,
                Repeat = measureSettings.NbOfRepeat,
                PatternRec = measureAsPatternRec ? "Yes" : "No"
            };

            // Points
            if (selectedDies.HasValue && !measureSettings.IsMeasureWithSubMeasurePoints)
            {
                measureSummary.PointsPerDie = measureSettings.MeasurePoints.Count();
                measureSummary.Points = measureSummary.PointsPerDie * selectedDies.Value;
            }
            else
            {
                measureSummary.Points = measureSettings.MeasurePoints.Count();
            }

            if (measureSettings is ISummaryProvider summaryProvider) 
            {
                measureSummary.Probes = summaryProvider.GetProbesUsed();
                measureSummary.Objectives = summaryProvider.GetObjectivesUsed();
                measureSummary.Lights = summaryProvider.GetLightsUsed();
                measureSummary.Autofocus = GetAutoFocusDisplay(summaryProvider.GetAutoFocusTypeUsed());
            }

            Measures.Add(measureSummary);
        }


        private XYZTopZBottomPosition DieToWaferPosition(WaferMapResult waferMap, XYZTopZBottomPosition xyzPosition)
        {
            var dieReferentialSettings = new DieReferentialSettings(waferMap.RotationAngle, waferMap.DieDimensions, waferMap.DieGridTopLeft, waferMap.DiesPresence);
            var dieReferential = xyzPosition.Referential as DieReferential;

            var position = ReferentialConvertersHelper.ConvertDiePositionToWafer(xyzPosition, dieReferential, dieReferentialSettings);

            return position;
        }


        private string GetAutoFocusDisplay(AutoFocusType? autoFocusType)
        {
            string autoFocusDisplay=string.Empty;

            if (autoFocusType == null)
            {
                autoFocusDisplay = "No";
            }
            else
            {
                switch (autoFocusType)
                {
                    case AutoFocusType.Camera:
                        autoFocusDisplay = "Camera";
                        break;

                    case AutoFocusType.Lise:
                        autoFocusDisplay = "Lise";
                        break;

                    case AutoFocusType.LiseAndCamera:
                        autoFocusDisplay = "Lise + Camera";
                        break;
                }
            }
            return autoFocusDisplay;
        }


    }
}
