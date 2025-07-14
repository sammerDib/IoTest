using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using DeepLearningSoft48.Services;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace DeepLearningSoft48.ViewModels
{
    /// <summary>
    /// View Model linked to AnnotationsListingView.xaml 
    /// Contains the list of annotations.
    /// </summary>
    public class AnnotationsListingViewModel : ObservableRecipient
    {
        //====================================================================
        // Annotate Wafer Layer View Model
        //====================================================================
        /// <summary>
        /// Permits to link the collection of the view model to this collection.
        /// </summary>
        private AnnotateWaferLayerViewModel _annotateWaferLayerViewModel;

        //====================================================================
        // Defect Annotations Collection 
        //==================================================================== 
        public IEnumerable<DefectAnnotationVM> DefectAnnotations => _annotateWaferLayerViewModel.DefectsAnnotationsCollection;

        /// <summary>
        /// Selected Item of the ListView
        /// </summary>
        private DefectAnnotationVM _selectedItem;
        public DefectAnnotationVM SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        /// <summary>
        /// Collection of all annotations sources.
        /// </summary>
        public ObservableCollection<string> Sources { get; set; }

        /// <summary>
        /// Distinct collection of all available sources and whether they're selected or not. 
        /// Bound to the checkbox of each source, as well as whether or not it is selected.  
        /// E.g. {DarkPSD, IsSelected = true}, etc..
        /// </summary>
        private ObservableCollection<KeyValuePairWrapper<string, bool>> _sourceIsSelectedStates;
        public ObservableCollection<KeyValuePairWrapper<string, bool>> SourceIsSelectedStates
        {
            get { return _sourceIsSelectedStates; }
            set
            {
                _sourceIsSelectedStates = value;
                OnPropertyChanged(nameof(SourceIsSelectedStates));
            }
        }

        /// <summary>
        /// Collection of all Selected annotations sources.
        /// </summary>
        public ObservableCollection<string> SelectedSources { get; set; }

        /// <summary>
        /// Filtered DefectAnnotations Collection based on which sources are selected.
        /// </summary>
        private ObservableCollection<DefectAnnotationVM> _filteredDefectAnnotations;
        public ObservableCollection<DefectAnnotationVM> FilteredDefectAnnotations
        {
            get
            {
                return _filteredDefectAnnotations;
            }
            set
            {
                if (_filteredDefectAnnotations != value)
                {
                    _filteredDefectAnnotations = value;
                    OnPropertyChanged(nameof(FilteredDefectAnnotations));
                }
            }
        }

        /// <summary>
        /// Helps the UI identify whether there are defectAnnotations present on said wafer to determine whether or not the filtering checkboxes be displayed.
        /// </summary>
        private bool _isAnnotationsPresent;
        public bool IsAnnotationsPresent
        {
            get
            {
                return _isAnnotationsPresent;
            }
            set
            {
                if (_isAnnotationsPresent != value)
                {
                    _isAnnotationsPresent = value;
                    OnPropertyChanged(nameof(IsAnnotationsPresent));
                }
            }
        }

        /// <summary>
        /// Helps the UI identify whether the All checkbox has been ticked. 
        /// </summary>
        private bool _isAllSourcesChecked;
        public bool IsAllSourcesChecked
        {
            get { return _isAllSourcesChecked; }
            set
            {
                _isAllSourcesChecked = value;
                OnPropertyChanged(nameof(IsAllSourcesChecked));
            }
        }

        //====================================================================
        // Constructors
        //====================================================================
        public AnnotationsListingViewModel(LearningTabViewModel learningTabViewModel)
        {
            _annotateWaferLayerViewModel = learningTabViewModel.WaferContentViewModel.SelectedLayerViewModel;
            _annotateWaferLayerViewModel.DefectsAnnotationsCollection.CollectionChanged += DefectsAnnotationsCollection_CollectionChanged;

            Sources = new ObservableCollection<string>();
            SelectedSources = new ObservableCollection<string>();
            SourceIsSelectedStates = new ObservableCollection<KeyValuePairWrapper<string, bool>>();
            FilteredDefectAnnotations = new ObservableCollection<DefectAnnotationVM>();
        }

        public AnnotationsListingViewModel(TestTabViewModel testTabViewModel)
        {
            _annotateWaferLayerViewModel = testTabViewModel.WaferContentViewModel.SelectedLayerViewModel;
            _annotateWaferLayerViewModel.DefectsAnnotationsCollection.CollectionChanged += DefectsAnnotationsCollection_CollectionChanged;

            Sources = new ObservableCollection<string>();
            SelectedSources = new ObservableCollection<string>();
            SourceIsSelectedStates = new ObservableCollection<KeyValuePairWrapper<string, bool>>();
            FilteredDefectAnnotations = new ObservableCollection<DefectAnnotationVM>();
        }

        //====================================================================
        // Commands
        //====================================================================

        /// <summary>
        /// Command triggered upon the ticking or unticking of the All checkbox to either select or deselect all sources.
        /// </summary>
        private AutoRelayCommand<string> _allSourceCommand;
        public AutoRelayCommand<string> AllSourceCommand
        {
            get
            {
                return _allSourceCommand ?? (_allSourceCommand = new AutoRelayCommand<string>(
              representationName =>
              {
                  SelectDeselectAllSources();
              }));
            }
        }

        /// <summary>
        /// Command to selected a source to display its related annotations
        /// </summary>
        private AutoRelayCommand<string> _checkSourceCommand;
        public AutoRelayCommand<string> CheckSourceCommand
        {
            get
            {
                return _checkSourceCommand ?? (_checkSourceCommand = new AutoRelayCommand<string>(
              representationName =>
              {
                  if (!string.IsNullOrEmpty(representationName) && !SelectedSources.Contains(representationName))
                  {
                      SelectedSources.Add(representationName);
                      FilterDefects();
                  }
                  IsAllSourcesChecked = false;
              },

            representationName => !SelectedSources.Contains(representationName))); // only execute if the source is not already selected
            }
        }

        /// <summary>
        /// Command to selected a source to hide its related annotations.
        /// </summary>
        private AutoRelayCommand<string> _uncheckSourceCommand;
        public AutoRelayCommand<string> UncheckSourceCommand
        {
            get
            {
                return _uncheckSourceCommand ?? (_uncheckSourceCommand = new AutoRelayCommand<string>(
              representationName =>
              {
                  if (!string.IsNullOrEmpty(representationName) && SelectedSources.Contains(representationName))
                  {
                      SelectedSources.Remove(representationName);
                      FilterDefects();
                  }
                  IsAllSourcesChecked = false;
              },

            representationName => SelectedSources.Contains(representationName))); // only execute if the source is already selected
            }
        }

        //====================================================================
        // Methods
        //====================================================================

        /// <summary>
        /// Initialises the Filtered DefectAnnotations collection and updates it when the primary DefectAnnotations collection undergoes change such as:
        ///     * Insertion (add) of a new defect annotation on a wafer
        ///     * Erasure (removal) of a defect annotation on a wafer 
        /// </summary>
        public void OnChangeFilteredDefectAnnotations()
        {
            FilteredDefectAnnotations.Clear();

            foreach (DefectAnnotationVM defectAnnotationVM in DefectAnnotations)
            {
                if (defectAnnotationVM != null && !FilteredDefectAnnotations.Contains(defectAnnotationVM))
                    FilteredDefectAnnotations.Add(defectAnnotationVM);
            }
        }

        /// <summary>
        /// Called initially to set all available (defectAnnotationVM) sources as selected.
        /// It is equally used to update the state of all present sources' checkbox state upon the (un)tick of the All checkbox.
        /// Unticking the All checkbox would mean that all other sources' checkbox should also get unticked, and vice-versa.
        /// </summary>
        public void UpdateCheckboxesState(bool isCheckAll)
        {
            if (isCheckAll)
            {
                var selectedStates = Sources.ToDictionary(source => source, _ => true);
                var wrappedStates = selectedStates.Select(kvp => new KeyValuePairWrapper<string, bool>(kvp.Key, kvp.Value)).ToList();
                SourceIsSelectedStates = new ObservableCollection<KeyValuePairWrapper<string, bool>>(wrappedStates);
            }
            else
            {
                var selectedStates = Sources.ToDictionary(source => source, _ => false);
                var wrappedStates = selectedStates.Select(kvp => new KeyValuePairWrapper<string, bool>(kvp.Key, kvp.Value)).ToList();
                SourceIsSelectedStates = new ObservableCollection<KeyValuePairWrapper<string, bool>>(wrappedStates);
            }
        }

        /// <summary>
        /// Update SourceIsSelectedStates dictionary by removing irrelevant source entry.
        /// In other words, if there are no more defectAnnotationsVMs of source per se "DarkPSD", the SourceIsSelectedStates dictionary will reflect:
        /// Entry: DarkPSD and its IsSelected property would be removed.
        /// In that case, this removes the DarkPSD checkbox if there are no more defectAnnotationVMs of that source.
        /// </summary>
        public void UpdateSourceIsSelectedStates()
        {
            // Remove entries for removed sources
            // Checks if there are any keys (sources) in the SourceIsSelectedStates dictionary that are not present in the Sources collection.
            // If it's the case, It creates a new list called removedSources that contains these keys (sources) that are present in the dictionary but not in the Sources collection.
            var removedSources = SourceIsSelectedStates
                    .Where(kvp => !Sources.Contains(kvp.Key))
                    .Select(kvp => kvp.Key)
                    .ToList();

            foreach (var source in removedSources)
            {
                var sourceToRemove = SourceIsSelectedStates.FirstOrDefault(item => item.Key.Equals(source)); // if source matches one of the sources in the removedSources list
                if (sourceToRemove != null)
                {
                    SourceIsSelectedStates.Remove(sourceToRemove);
                }
            }
        }

        /// <summary>
        /// Called upon the ticking or unticking of the All checkbox to either select or deselect all sources.
        /// </summary>
        private void SelectDeselectAllSources()
        {
            FilteredDefectAnnotations.Clear();

            if (!IsAllSourcesChecked) // Uncheck "All"
            {
                SelectedSources.Clear();
                foreach (var defectAnnotationVM in DefectAnnotations)
                {
                    defectAnnotationVM.IsSelected = false;
                    defectAnnotationVM.IsVisible = false;
                }
            }
            else // Check "All"
            {
                foreach (var defectAnnotationVM in DefectAnnotations)
                {
                    defectAnnotationVM.IsSelected = true;
                    defectAnnotationVM.IsVisible = true;
                    if (!SelectedSources.Contains(defectAnnotationVM.Source))
                        SelectedSources.Add(defectAnnotationVM.Source);
                    FilteredDefectAnnotations.Add(defectAnnotationVM);
                }
            }

            UpdateCheckboxesState(IsAllSourcesChecked);
            OnPropertyChanged(nameof(SelectedSources));
        }

        /// <summary>
        /// Permits to filter the DefectAnnotations based on their source by setting the visibility property (IsVisible) of each DefectAnnotationVM.
        /// </summary>
        private void FilterDefects()
        {
            FilteredDefectAnnotations.Clear();

            foreach (var defectAnnotationVM in DefectAnnotations)
            {
                if (SelectedSources.Contains(defectAnnotationVM.Source))
                {
                    defectAnnotationVM.IsVisible = true; // Show the defect if its source is selected
                    defectAnnotationVM.IsSelected = true;
                    if (!FilteredDefectAnnotations.Contains(defectAnnotationVM))
                        FilteredDefectAnnotations.Add(defectAnnotationVM);
                }
                else
                {
                    defectAnnotationVM.IsVisible = false; // Hide the defect if its source is not selected
                    defectAnnotationVM.IsSelected = false;
                    if (FilteredDefectAnnotations.Contains(defectAnnotationVM))
                        FilteredDefectAnnotations.Remove(defectAnnotationVM);
                }
            }
        }

        /// <summary>
        /// Permits to get the collection of sources from the collection of annotations.
        /// </summary>
        private void GetSources()
        {
            IEnumerable<DefectAnnotationVM> defectAnnotationsSources = DefectAnnotations?.Where(y => Sources.Contains(y.Source.Distinct()) == false);

            if (defectAnnotationsSources != null && defectAnnotationsSources.Any())
            {
                foreach (DefectAnnotationVM annotation in defectAnnotationsSources)
                {
                    if (!Sources.Contains(annotation.Source) && !SelectedSources.Contains(annotation.Source))
                    {
                        Sources.Add(annotation.Source);
                        SelectedSources.Add(annotation.Source);
                    }
                    annotation.IsVisible = true;
                    annotation.IsSelected = true;
                    OnPropertyChanged(nameof(Sources));
                    OnPropertyChanged(nameof(SelectedSources));
                }

                // Remove the sources that are no longer present by
                // finding sources within 'Sources' collection that do not exist in the 'DefectAnnotations' collection 
                // i.e. There isn't any DefectAnnotationVMs in the DefectAnnotations collection that has a Source property equal to any of the sources in the 'Sources' list.
                var removedSources = Sources.Where(source => !DefectAnnotations.Any(annotation => annotation.Source.Equals(source))).ToList();

                // If there are any removed sources (i.e. source no longer present, no remianing defectAnnotationVMs are of particular source)
                if (removedSources != null && removedSources.Any())
                {
                    foreach (var removedSource in removedSources)
                    {
                        Sources.Remove(removedSource);
                        SelectedSources.Remove(removedSource);
                    }
                    UpdateSourceIsSelectedStates();
                }

                IsAnnotationsPresent = true;
                IsAllSourcesChecked = true;
                UpdateCheckboxesState(true);
                OnPropertyChanged(nameof(IsAnnotationsPresent));
            }
            else if (defectAnnotationsSources != null && !defectAnnotationsSources.Any())
            {
                Sources.Clear();
                SelectedSources.Clear();
                IsAnnotationsPresent = false;
                IsAllSourcesChecked = false;
                UpdateCheckboxesState(false);
                OnPropertyChanged(nameof(IsAnnotationsPresent));
            }
        }

        //====================================================================
        // Events
        //====================================================================
        /// <summary>
        /// Update the collection when an annotation is added or removed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefectsAnnotationsCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(DefectAnnotations));
            GetSources();
            OnChangeFilteredDefectAnnotations();
        }

        /// <summary>
        /// Permits to update the selected item of the listview when an anotation is selected on the canvas.
        /// </summary>
        private void Annotation_IsSelectedChanged()
        {
            SelectedItem = DefectAnnotations.Find(y => y.IsSelected);
        }
    }
}
