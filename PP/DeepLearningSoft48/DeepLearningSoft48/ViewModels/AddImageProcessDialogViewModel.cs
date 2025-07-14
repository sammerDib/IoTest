using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Modules;

using MvvmDialogs;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to AddImageProcessDialog.xaml 
    /// Permits to add one or multiple processes to the ApplyImageProcesses.xaml modal window.
    /// </summary>
    public class AddImageProcessDialogViewModel : ObservableRecipient, IModalDialogViewModel
    {
        //====================================================================
        // Commands and DialogResult for the ViewModelLocator class
        //====================================================================
        private readonly AutoRelayCommand _addModuleCommand;
        public ICommand AddModuleCommand => _addModuleCommand;

        private bool? _dialogResult;
        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }

        //====================================================================
        // Image Processing Modules Initialization
        //====================================================================

        /// <summary>
        /// Available Modules List & Collection.
        /// </summary>
        public List<ModuleBase> Modules { get; }
        public ICollectionView ModulesCollectionView { get; }

        /// <summary>
        /// List of Selected Modules that have to be added.
        /// </summary>
        public List<ModuleBase> SelectedItems = new List<ModuleBase>();
        public bool CanAdd => SelectedItems.Count() != 0;

        //====================================================================
        // Search Filter
        //====================================================================
        /// <summary>
        /// Filter allowing to search for a specific module in the available modules list.
        /// </summary>
        private string _modulesFilter = string.Empty;
        public string ModulesFilter
        {
            get
            {
                return _modulesFilter;
            }
            set
            {
                _modulesFilter = value;
                OnPropertyChanged(nameof(ModulesFilter));
                ModulesCollectionView.Refresh();
            }
        }

        //====================================================================
        // Constructor
        //====================================================================
        public AddImageProcessDialogViewModel(List<ModuleBase> moduleBases)
        {
            Modules = moduleBases;

            ModulesCollectionView = CollectionViewSource.GetDefaultView(Modules);

            ModulesCollectionView.Filter = FilterModules;
            ModulesCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(ModuleBase.DisplayName)));
            ModulesCollectionView.SortDescriptions.Add(new SortDescription(nameof(ModuleBase.DisplayName), ListSortDirection.Ascending));

            _addModuleCommand = new AutoRelayCommand(Add);
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Command to add all available processes at a time.
        /// </summary>
        private AutoRelayCommand _addAllProcessesCommand;
        public AutoRelayCommand AddAllProcessesCommand
        {
            get
            {
                return _addAllProcessesCommand ?? (_addAllProcessesCommand = new AutoRelayCommand(
              () =>
              {
                  SelectedItems = new List<ModuleBase>(Modules);
                  AddModuleCommand.Execute(SelectedItems);
              }));
            }
        }

        /// <summary>
        /// Command to selected a module to add.
        /// </summary>
        private AutoRelayCommand<ModuleBase> _checkProcessCommand;
        public AutoRelayCommand<ModuleBase> CheckProcessCommand
        {
            get
            {
                return _checkProcessCommand ?? (_checkProcessCommand = new AutoRelayCommand<ModuleBase>(
              module =>
              {
                  SelectedItems.Add(module);
                  OnPropertyChanged(nameof(CanAdd));
              }));
            }
        }

        /// <summary>
        /// Command to unselected a module.
        /// </summary>
        private AutoRelayCommand<ModuleBase> _uncheckProcessCommand;
        public AutoRelayCommand<ModuleBase> UncheckProcessCommand
        {
            get
            {
                return _uncheckProcessCommand ?? (_uncheckProcessCommand = new AutoRelayCommand<ModuleBase>(
              module =>
              {
                  SelectedItems.Remove(module);
                  OnPropertyChanged(nameof(CanAdd));
              }));
            }
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Method linked to the called command allonwing to add processes.
        /// </summary>
        private void Add()
        {
            if (DialogResult != null)
            {
                DialogResult = null;
            }
            if (CanAdd)
            {
                DialogResult = true;
            }
        }

        /// <summary>
        /// Filter method to search a module
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private bool FilterModules(object obj)
        {
            if ((obj is ModuleBase module) && (module.DisplayName.IndexOf(ModulesFilter, StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                return true;
            }
            return false;
        }
    }
}
