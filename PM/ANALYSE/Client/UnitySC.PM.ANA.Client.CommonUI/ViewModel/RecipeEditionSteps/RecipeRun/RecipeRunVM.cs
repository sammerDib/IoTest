using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.RecipeRun;
using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Measure;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Display.Metro;
using UnitySC.Shared.Format.Metro;
using UnitySC.Shared.Logger;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Bow;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.EdgeTrim;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Trench;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Nanotopo;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Thickness;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Topography;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Tsv;
using UnitySC.Shared.ResultUI.Metro.ViewModel.WaferDetail.Warp;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps
{
    public class RecipeRunVM : RecipeWizardStepBaseVM, IDisposable
    {
        public ANARecipeVM EditedRecipe { get; set; }

        private bool _isBusy = false;

        private bool _isRecipeRunning=false;

        public bool IsBusy
        {
            get => _isBusy; set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        private string _busyMessage;

        public string BusyMessage
        {
            get => _busyMessage; set { if (_busyMessage != value) { _busyMessage = value; OnPropertyChanged(); } }
        }

        private MetroDisplay _resultDisplay = new MetroDisplay();

        private MetroResultVM _metroResultVM = null;

        public RecipeRunVM(ANARecipeVM editedRecipe)
        {
            Name = "Recipe Run";
            IsEnabled = true;
            IsMeasure = false;
            IsValidated = false;
            EditedRecipe = editedRecipe;
            RecipeRunScreens.Add(new RecipeRunDashboardVM(EditedRecipe, this));

            SelectedRecipeRunScreen = RecipeRunScreens[0];
            ServiceLocator.ANARecipeSupervisor.RecipeProgressChangedEvent += ANARecipeSupervisor_RecipeProgressChangedEvent;
            ServiceLocator.ANARecipeSupervisor.RecipeFinishedChangedEvent += ANARecipeSupervisor_RecipeFinishedChangedEvent;
        }

        private void ANARecipeSupervisor_RecipeProgressChangedEvent(Service.Interface.Recipe.Execution.RecipeProgress recipeProgress)
        {
            if (recipeProgress.RecipeProgressState == Service.Interface.Recipe.Execution.RecipeProgressState.Success)
                RecipeRunSaveAs.ResultFolderPath = recipeProgress.ResultFolderPath;
            else
                RecipeRunSaveAs.ResultFolderPath = null;

            _isRecipeRunning = true;
        }

        private void ANARecipeSupervisor_RecipeFinishedChangedEvent(List<MetroResult> results)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var recipeRunMeasureResultScreens = _recipeRunScreens.OfType<RecipeRunMeasureResultVM>().ToList();

                for (int i = 0; i < recipeRunMeasureResultScreens.Count; i++)
                {
                    RecipeRunMeasureResultVM measureScreen = null;
                    MetroResultVM metroVM = null;
                    try
                    {
                        measureScreen = recipeRunMeasureResultScreens[i];
                        measureScreen?.MeasureResult?.Dispose();
                        metroVM = BuildMetroVM(measureScreen.ResType);

                        metroVM.SetHeaderNames(measureScreen.Title, "Recipe Run Results", string.Empty);

                        var resFound = results.FirstOrDefault(r => r.ResType == measureScreen.ResType && r.MeasureResult.Name == measureScreen.Title);
                        if (resFound != null)
                            metroVM.UpdateResData(resFound);

                        measureScreen.MeasureResult = metroVM;
                    }
                    catch (Exception)
                    {
                        metroVM?.Dispose();
                        var logger = ClassLocator.Default.GetInstance<ILogger>();
                        logger.Error($"Failed to display the results for the measure {measureScreen?.Title}");
                    }
                }
                _isRecipeRunning = false;
                IsValidated = true;
            }));
        }

        internal void EnableAllMeasures()
        {
            EnableDisableAllMeasures(true);
        }

        private MetroResultVM BuildMetroVM(ResultType resType)
        {
            if (resType.GetResultFormat() != ResultFormat.Metrology)
                throw new ArgumentException($"ResultType {resType} is not a Metrology format type");

            switch (resType)
            {
                case ResultType.ANALYSE_TSV:
                    _metroResultVM = new TsvResultVM(_resultDisplay);
                    break;

                case ResultType.ANALYSE_NanoTopo:
                    _metroResultVM = new NanotopoResultVM(_resultDisplay);
                    break;

                case ResultType.ANALYSE_Thickness:
                    _metroResultVM = new ThicknessResultVM(_resultDisplay);
                    break;

                case ResultType.ANALYSE_Topography:
                    _metroResultVM = new TopographyResultVM(_resultDisplay);
                    break;

                case ResultType.ANALYSE_Bow:
                    _metroResultVM = new BowResultVM(_resultDisplay);
                    break;

                case ResultType.ANALYSE_Warp:
                    _metroResultVM = new WarpResultVM(_resultDisplay);
                    break;

                case ResultType.ANALYSE_EdgeTrim:
                    _metroResultVM = new EdgeTrimResultVM(_resultDisplay);
                    break;
                case ResultType.ANALYSE_Trench:
                    _metroResultVM = new TrenchResultVM(_resultDisplay);
                    break;
                default:
                    throw new NotImplementedException($"ResultType <{resType}> is not IMPLEMENTED in recipe RUN");
            }

            return _metroResultVM;
        }

        private void EnableDisableAllMeasures(bool enable)
        {
            foreach (var measureScreen in _recipeRunScreens)
            {
                if (measureScreen is RecipeRunMeasureResultVM)
                    measureScreen.IsEnabled = enable;
            }
        }

        internal void DisableAllMeasures()
        {
            EnableDisableAllMeasures(false);
        }

        private List<TabViewModelBase> _recipeRunScreens;

        public List<TabViewModelBase> RecipeRunScreens
        {
            get
            {
                if (_recipeRunScreens is null)
                {
                    _recipeRunScreens = new List<TabViewModelBase>();
                }
                return _recipeRunScreens;
            }
            set
            {
                if (_recipeRunScreens != value)
                {
                    _recipeRunScreens = value;
                    OnPropertyChanged();
                }
            }
        }

        private TabViewModelBase _selectedRecipeRunScreen = null;

        public TabViewModelBase SelectedRecipeRunScreen
        {
            get
            {
                return _selectedRecipeRunScreen;
            }

            set
            {
                if (_selectedRecipeRunScreen == value)
                {
                    return;
                }

                if (_selectedRecipeRunScreen != null)
                {
                    if (!_selectedRecipeRunScreen.CanChangeTab())
                        return;
                    _selectedRecipeRunScreen.PrepareDisplay();
                }
                _selectedRecipeRunScreen = value;
                OnPropertyChanged();
            }
        }

        private RecipeRunSaveAsVM _recipeRunSaveAs = null;

        public RecipeRunSaveAsVM RecipeRunSaveAs
        {
            get
            {
                if (_recipeRunSaveAs is null)
                    _recipeRunSaveAs = new RecipeRunSaveAsVM(this);
                return _recipeRunSaveAs;
            }

            set { if (_recipeRunSaveAs != value) { _recipeRunSaveAs = value; OnPropertyChanged(); } }
        }

        #region INavigable

        public override Task PrepareToDisplay()
        {
            foreach (var measure in EditedRecipe.Measures.Where(m => m.IsActive))
            {
                if (RecipeRunScreens.Find(r => r.Title == measure.Name) is null)
                {
                    RecipeRunScreens.Add(new RecipeRunMeasureResultVM(measure.Name, measure.MeasureType.GetResultType()));
                }
            }

            foreach (var recipeRunScreen in RecipeRunScreens.Where(r => r is RecipeRunMeasureResultVM).ToList())
            {
                if (EditedRecipe.Measures.Find(m => m.IsActive && m.Name == recipeRunScreen.Title) is null)
                {
                    RecipeRunScreens.Remove(recipeRunScreen);
                }
            }

            // TODO : if measures are modified we must clear the dashboard and the measures
            foreach (var recipeRunScreen in RecipeRunScreens)
            {
                recipeRunScreen.Update();
            }

            OnPropertyChanged(nameof(EditedRecipe));

            // We select the main objective
            var objectiveToUse = ClassLocator.Default.GetInstance<CamerasSupervisor>().MainObjective;
            ClassLocator.Default.GetInstance<CamerasSupervisor>().Objective = objectiveToUse;
            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StartStreaming();
            return Task.CompletedTask;
        }

        public override bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            return !_isRecipeRunning;
        }

        #endregion INavigable

        public void Dispose()
        {
            ServiceLocator.ANARecipeSupervisor.RecipeProgressChangedEvent -= ANARecipeSupervisor_RecipeProgressChangedEvent;
            ServiceLocator.ANARecipeSupervisor.RecipeFinishedChangedEvent -= ANARecipeSupervisor_RecipeFinishedChangedEvent;
            RecipeRunSaveAs?.Dispose();
            _metroResultVM?.Dispose();
            foreach (var recipeRunScreen in RecipeRunScreens)
            {
                if (recipeRunScreen is IDisposable disposableScreen)
                    disposableScreen.Dispose();
            }
            ServiceLocator.CamerasSupervisor.GetMainCamera()?.StopStreaming();
        }
    }
}
