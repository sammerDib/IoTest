using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

using UnitySC.PM.ANA.Client.CommonUI.View.RecipeEditionSteps.Measures.CustomPointsManagement;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.CustomPointsManagement
{
    public class WarpCustomPointsManagementVM : CustomPointsManagementVM
    {
      
        private PointsPresetsManager _pointsPresetsManager;
        

        private ObservableCollection<string> _presets;
        public ObservableCollection<string> Presets
        {
            get
            {
                if (_presets is null)
                    _presets = new ObservableCollection<string>();
                return _presets;
            }
                
            set => SetProperty(ref _presets, value);
        }



        private string _selectedPreset;

        public string SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (_selectedPreset!=value)
                {
                    if (LoadPreset(value))
                        SetProperty(ref _selectedPreset, value);

                }
            }
        }

        private bool LoadPreset(string presetToLoad)
        {
            if (presetToLoad != null)
            {
                var res = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The preset \"{presetToLoad}\" will replace the current points. \nDo you want to apply the preset ?", "Load preset", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                if (res == MessageBoxResult.No)
                {
                    return false;
                }

                var presetPoints = _pointsPresetsManager.GetPresetPoints(MeasurePoints.RecipeMeasure.MeasureType, MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory, presetToLoad);
                if (presetPoints != null)
                    MeasurePoints.AddMeasurePoints(presetPoints, true);
            }

            return true;
        }

        public WarpCustomPointsManagementVM(MeasurePointsVM measurePoints) : base(measurePoints)
        {
            _pointsPresetsManager = ClassLocator.Default.GetInstance<PointsPresetsManager>();
            UpdatePresets();
        }

        private void UpdatePresets()
        {
            Presets.Clear();
            var presets = _pointsPresetsManager.GetPresets(MeasurePoints.RecipeMeasure.MeasureType, MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory);
            Presets.AddRange(presets);
        }


        private AutoRelayCommand _loadSelectedPresetCommand;

        public AutoRelayCommand LoadSelectedPresetCommand
        {
            get
            {
                return _loadSelectedPresetCommand ?? (_loadSelectedPresetCommand = new AutoRelayCommand(
                    () =>
                    {
                        LoadPreset(_selectedPreset);
                    },
                    () => { return !string.IsNullOrEmpty(SelectedPreset); }
                ));
            }
        }

        private AutoRelayCommand<string> _deletePointsPresetCommand;

        public AutoRelayCommand<string> DeletePointsPresetCommand
        {
            get
            {
                return _deletePointsPresetCommand ?? (_deletePointsPresetCommand = new AutoRelayCommand<string>(
                    (preset) =>
                    {
                        _pointsPresetsManager.DeletePreset(MeasurePoints.RecipeMeasure.MeasureType, MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory,preset);

                        Presets.Remove(preset);
                        if (preset == SelectedPreset)
                            SelectedPreset = null;
                    },
                    (preset) => { return true; }
                ));
            }
        }

        private AutoRelayCommand _savePointsPresetCommand;

        public AutoRelayCommand SavePointsPresetCommand
        {
            get
            {
                return _savePointsPresetCommand ?? (_savePointsPresetCommand = new AutoRelayCommand(
                    () =>
                    {
                        _pointsPresetsManager.SavePointsPreset(MeasurePoints.RecipeMeasure.MeasureType, MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory, SelectedPreset, MeasurePoints.Points.ToList());
                    },
                    () => { return !SelectedPreset.IsNullOrEmpty(); }
                ));
            }
        }

        private AutoRelayCommand _saveAsPointsPresetCommand;

        public AutoRelayCommand SaveAsPointsPresetCommand
        {
            get
            {
                return _saveAsPointsPresetCommand ?? (_saveAsPointsPresetCommand = new AutoRelayCommand(
                    () =>
                    {
                        var dialogViewModel = new InputPresetNameViewModel();
                        bool? success = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowCustomDialog<InputPresetNameDialog>(dialogViewModel);
                        if (success != true)
                            return;
                
                        if (Presets.Any(preset => preset == dialogViewModel.PresetName))
                        {
                            var res=ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"The preset {dialogViewModel.PresetName} already exists. Do you want to overwrite it ?", "Create preset", System.Windows.MessageBoxButton.YesNo,System.Windows.MessageBoxImage.Question);
                            if (res == MessageBoxResult.No)
                            {
                                return;
                            }
                        }

                        _pointsPresetsManager.SavePointsPreset(MeasurePoints.RecipeMeasure.MeasureType, MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory, dialogViewModel.PresetName, MeasurePoints.Points.ToList());
                        UpdatePresets();
                        if (Presets.Contains(dialogViewModel.PresetName))
                        {
                            _selectedPreset = dialogViewModel.PresetName;
                            OnPropertyChanged(nameof(SelectedPreset));
                        }
                    },
                    () => { return true; }
                ));
            }
        }


        private AutoRelayCommand<string> _viewPresetCommand;

        public AutoRelayCommand<string> ViewPresetCommand
        {
            get
            {
                return _viewPresetCommand ?? (_viewPresetCommand = new AutoRelayCommand<string>(
                    (preset) =>
                    {
                        var dialogViewModel = new ViewPresetViewModel();
                        dialogViewModel.DimentionalCharacteristic = MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory.DimentionalCharacteristic;
                        var presetPoints = _pointsPresetsManager.GetPresetPoints(MeasurePoints.RecipeMeasure.MeasureType, MeasurePoints.RecipeMeasure.EditedRecipe.Step.Product.WaferCategory, preset);
                        if (presetPoints is null)
                        {
                            ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"Failed to load the preset \"{preset}\".", "Load preset", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                            return;
                        }
                        
                        var presetPointsList= new List<Point>();
                        presetPointsList.AddRange(presetPoints.Select(point => new Point(point.X, point.Y)));
                        dialogViewModel.PresetPoints= presetPointsList;
                        bool? success = ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowCustomDialog<ViewPresetDialog>(dialogViewModel);
                        if (success != true)
                            return;
                    },
                    (preset) => { return true; }
                ));
            }
        }
    }
}
