using System.ComponentModel;

using UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation;
using UnitySC.PM.EME.Service.Interface.Recipe.Execution;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public sealed class ExecutionSettingsViewModel : RecipeWizardStepBaseViewModel
    {
        private readonly EMERecipeVM _editedRecipe;
        
        public ExecutionSettingsViewModel(EMERecipeVM editedRecipe)
        {
            Name = "Execution Settings";
            IsEnabled = true;
            IsValidated = true;
            _editedRecipe = editedRecipe;
            Item = editedRecipe.Execution ?? new ExecutionSettings();
            _editedRecipe.Execution = Item;
            PropertyChanged += OnExecutionSettingsViewModelPropertyChanged;
        }

        private void OnExecutionSettingsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _editedRecipe.Execution = Item;
            _editedRecipe.IsModified = true;
        }

        private ExecutionSettings _item;
        public ExecutionSettings Item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }

        public bool RunAutoFocus
        {
            get => _item.RunAutoFocus;
            set
            {
                if (_item.RunAutoFocus != value)
                {
                    _item.RunAutoFocus = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunAutoExposure
        {
            get => _item.RunAutoExposure;
            set
            {
                if (_item.RunAutoExposure != value)
                {
                    _item.RunAutoExposure = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunBwa
        {
            get => _item.RunBwa;
            set
            {
                if (_item.RunBwa != value)
                {
                    _item.RunBwa = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public AcquisitionStrategy CurrentAcquisitionStrategy
        {
            get => _item.Strategy;
            set
            {
                if (_item.Strategy != value)
                {
                    _item.Strategy = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool ReduceResolution
        {
            get => _item.ReduceResolution;
            set
            {
                if (_item.ReduceResolution != value)
                {
                    _item.ReduceResolution = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ConvertTo8Bits
        {
            get => _item.ConvertTo8Bits;
            set
            {
                if (_item.ConvertTo8Bits != value)
                {
                    _item.ConvertTo8Bits = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool CorrectDistortion
        {
            get => _item.CorrectDistortion;
            set
            {
                if (_item.CorrectDistortion != value)
                {
                    _item.CorrectDistortion = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool NormalizePixelValue
        {
            get => _item.NormalizePixelValue;
            set
            {
                if (_item.NormalizePixelValue != value)
                {
                    _item.NormalizePixelValue = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public bool IsSaveResultsEnabled
        {
            get => _editedRecipe.IsSaveResultsEnabled;
            set
            {
                if (_editedRecipe.IsSaveResultsEnabled != value)
                {
                    _editedRecipe.IsSaveResultsEnabled = value;
                    _editedRecipe.IsModified = true;
                    OnPropertyChanged();
                }
            }
        }

        public bool RunStitchFullImage
        {
            get => _item.RunStitchFullImage;
            set
            {
                if (_item.RunStitchFullImage != value)
                {
                    _item.RunStitchFullImage = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
