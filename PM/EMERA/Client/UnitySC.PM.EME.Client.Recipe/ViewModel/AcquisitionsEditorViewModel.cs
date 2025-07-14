using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.EME.Client.Proxy.Camera;
using UnitySC.PM.EME.Client.Proxy.FilterWheel;
using UnitySC.PM.EME.Client.Proxy.Light;
using UnitySC.PM.EME.Client.Recipe.ViewModel.Navigation;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Controls.WizardNavigationControl;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
{
    public sealed class AcquisitionsEditorViewModel : RecipeWizardStepBaseViewModel, INavigable
    {
        private const int MaxAcquisitions = 5;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly CameraBench _camera;
        private readonly FilterWheelBench _filterWheelBench;
        private readonly LightBench _lightBench;
        private readonly IDialogOwnerService _dialogOwnerService;
        private readonly EMERecipeVM _editedRecipe;        
        private ObservableCollection<AcquisitionViewModel> _acquisitionsViewModel; 

        public AcquisitionsEditorViewModel(EMERecipeVM editedRecipe, CameraBench camera,
            FilterWheelBench filterWheelBench, LightBench lightBench, IDialogOwnerService dialogOwnerService, ILogger logger,
            IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
            _editedRecipe = editedRecipe;
            _camera = camera;
            _filterWheelBench = filterWheelBench;
            _lightBench = lightBench;
            _dialogOwnerService = dialogOwnerService;
            AcquisitionsViewModel = new ObservableCollection<AcquisitionViewModel>();            
            LoadRecipeData();
            Name = "Acquisitions";
            IsEnabled = true;
        }

        public ObservableCollection<AcquisitionViewModel> AcquisitionsViewModel
        {
            get => _acquisitionsViewModel;
            set => SetProperty(ref _acquisitionsViewModel, value);
        }

        public void LoadRecipeData()
        {
            var acquisitionViewModels = new List<AcquisitionViewModel>();
            if (_editedRecipe != null)
            {               
                acquisitionViewModels = _editedRecipe.Acquisitions.ConvertAll(x =>
                    AcquisitionViewModel.LoadAcquisitionAndCreate(_camera, _filterWheelBench, _lightBench, x, _logger, _messenger));
            }
            else
            {
                _logger.Error("Error on LoadRecipeData. EditedRecipe cannot be null");
                _messenger?.Send(new Message(MessageLevel.Error, "Recipe not loaded completely"));
            }

            AcquisitionsViewModel = new ObservableCollection<AcquisitionViewModel>(acquisitionViewModels);
        }

        public void SaveRecipeData()
        {
            _editedRecipe.Acquisitions.Clear();
            _editedRecipe.Acquisitions.AddRange(AcquisitionsViewModel.Select(x => x.Item).ToList());
        }

        private AutoRelayCommand _deleteAll;

        public AutoRelayCommand DeleteAll
        {
            get
            {
                return _deleteAll ?? (_deleteAll = new AutoRelayCommand(
                    () =>
                    {
                        var messageBoxResult = _dialogOwnerService.ShowMessageBox(
                            "Are you sure you want to delete all acquisitions ?",
                            "Remove items Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question,
                            MessageBoxResult.No);

                        if (messageBoxResult != MessageBoxResult.Yes)
                            return;

                        AcquisitionsViewModel.Clear();
                        _editedRecipe.Acquisitions.Clear();
                        _editedRecipe.IsModified = true;
                    },
                    () => AcquisitionsViewModel.Any()));
            }
        }

        private AutoRelayCommand _addAcquisition;

        public AutoRelayCommand AddAcquisition
        {
            get
            {
                return _addAcquisition ?? (_addAcquisition = new AutoRelayCommand(
                    () =>
                    {
                        var acquisitionViewModel = CreateNewAcquisition("Acquisition " + (AcquisitionsViewModel.Count + 1));
                        AcquisitionsViewModel.Add(acquisitionViewModel);
                        _editedRecipe.Acquisitions.Add(acquisitionViewModel.Item);
                        _editedRecipe.IsModified = true;
                    },
                    () => AcquisitionsViewModel.Count < MaxAcquisitions));
            }
        }

        private AcquisitionViewModel CreateNewAcquisition(string name)
        {        
            var acquisitionViewModel = AcquisitionViewModel.Create(_camera, _filterWheelBench, _lightBench, _logger, _messenger, name);
            acquisitionViewModel.InEdition = true;
            return acquisitionViewModel;
        }       
        private AutoRelayCommand<AcquisitionViewModel> _deleteAcquisition;

        public AutoRelayCommand<AcquisitionViewModel> DeleteAcquisition
        {
            get
            {
                return _deleteAcquisition ?? (_deleteAcquisition = new AutoRelayCommand<AcquisitionViewModel>(
                    acquisition =>
                    {
                        var messageBoxResult = _dialogOwnerService.ShowMessageBox(
                            "Do you really want to remove this acquisition ?", "Remove item Confirmation",
                            MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                        if (messageBoxResult != MessageBoxResult.Yes)
                            return;

                        AcquisitionsViewModel.Remove(acquisition);
                        _editedRecipe.Acquisitions.Remove(acquisition.Item);
                        _editedRecipe.IsModified = true;
                    },
                    acquisition => acquisition != null));
            }
        }

        private AutoRelayCommand<AcquisitionViewModel> _changeAcquisitionEdition;

        public AutoRelayCommand<AcquisitionViewModel> ChangeAcquisitionEdition
        {
            get
            {
                return _changeAcquisitionEdition ?? (_changeAcquisitionEdition =
                    new AutoRelayCommand<AcquisitionViewModel>(
                        acquisition =>
                        {
                            acquisition.InEdition = !acquisition.InEdition;
                            _editedRecipe.Acquisitions.Clear();
                            _editedRecipe.Acquisitions.AddRange(AcquisitionsViewModel.Select(x => x.Item).ToList());
                            _editedRecipe.IsModified = true;
                        },
                        acquisition => acquisition != null));
            }
        }

        public Task PrepareToDisplay()
        {
            return Task.CompletedTask;
        }

        public bool CanLeave(INavigable nextPage, bool forceClose = false)
        {
            return true;
        }

        public bool IsValid()
        {
            return AcquisitionsViewModel.Any();
        }
    }
}
