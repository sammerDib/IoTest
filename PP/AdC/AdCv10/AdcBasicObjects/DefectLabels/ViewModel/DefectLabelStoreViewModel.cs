using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using MvvmValidation;

using UnitySC.Shared.UI.AutoRelayCommandExt;

using Utils.ViewModel;

namespace AdcBasicObjects.DefectLabels.ViewModel
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class DefectLabelStoreViewModel : ValidationViewModelBase
    {
        ///<summary> Objet gérant le stockage des Labels sur disque </summary>
        public DefectLabelStore DefectLabelStore;

        ///<summary> La liste des labels pour la listbox </summary>
        private ObservableCollection<LabelViewModel> _defectClassList { get; set; } = new ObservableCollection<LabelViewModel>();

        ///<summary> Une liste de labels non-selectionnables (déjà utilisés) </summary>
        private List<string> DisabledClassList = new List<string>();

        //=================================================================
        // Propriétés bindables
        //=================================================================
        // Est-ce qu'il y a des items sélectionnés ?
        //..........................................
        private bool IsSelectionEmpty()
        {
            return NbSelectedItems == 0;
        }

        private int _nbSelectedItems = 0;
        public int NbSelectedItems
        {
            get { return _nbSelectedItems; }
            set
            {
                if (value == _nbSelectedItems)
                    return;
                _nbSelectedItems = value;
                OnPropertyChanged();
            }
        }

        // Item selectionné dans la liste
        //...............................
        private LabelViewModel _selectedItem;
        public LabelViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value == _selectedItem)
                    return;
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        // Liste des labels sélectionnés
        //..............................
        public List<string> SelectedDefectClasses
        {
            get
            {
                var classes = from cl in _defectClassList where cl.IsSelected == true select cl.DefectLabel;
                return classes.ToList();
            }
        }

        // Filtre de recherche
        //....................
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
                    _defectClasses.Refresh();
                }
            }
        }

        private ICollectionView _defectClasses;
        public ICollectionView DefectClasses
        {
            get { return _defectClasses; }
            set
            {
                _defectClasses = value;
                OnPropertyChanged();
            }
        }


        //=================================================================
        // Constructeur
        //=================================================================
        public DefectLabelStoreViewModel() { }  // Pour le XAML seulement
        public DefectLabelStoreViewModel(DefectLabelStore DefectLabelStore)
        {
            this.DefectLabelStore = DefectLabelStore;
            Validator.AddRule("VidLabel", () => RuleResult.Assert(!_defectClassList.Where(i => i.HasError).Any(), "Label must be unique"));
            FilterDefectClassList();
        }

        //=================================================================
        // 
        //=================================================================
        public bool Validate()
        {
            ValidationResult res = Validator.ValidateAll();
            return res.IsValid;
        }

        //=================================================================
        // 
        //=================================================================
        public void FilterDefectClassList(IEnumerable<string> disabledClassList = null)
        {
            DisabledClassList.Clear();
            if (disabledClassList != null)
                DisabledClassList.AddRange(disabledClassList);

            List<string> classlist = new List<string>(DefectLabelStore.LabelList);

            List<LabelViewModel> labelViewModels = new List<LabelViewModel>();
            foreach (string label in classlist)
            {
                LabelViewModel item = new LabelViewModel(this);
                item.DefectLabel = label;
                item.IsEnabled = !DisabledClassList.Contains(label);

                labelViewModels.Add(item);
            }

            _defectClassList = new ObservableCollection<LabelViewModel>(labelViewModels.OrderBy(x => x.DefectLabel).OrderByDescending(x => x.IsEnabled));
            DefectClasses = CollectionViewSource.GetDefaultView(_defectClassList);
            DefectClasses.Filter = LabelFilter;
            Validator.ValidateAll();
        }

        //=================================================================
        // 
        //=================================================================
        private AutoRelayCommand _createCommand = null;
        public AutoRelayCommand CreateCommand
        {
            get
            {
                return _createCommand ?? (_createCommand = new AutoRelayCommand(
            () =>
            {
                foreach (LabelViewModel item in _defectClassList)
                    item.IsSelected = false;

                int count = 0;
                string newlabel;
                bool added;
                do
                {
                    if (++count == 1)
                        newlabel = "new label";
                    else
                        newlabel = "new label " + count.ToString();
                    added = DefectLabelStore.AddLabel(newlabel);
                }
                while (!added);

                LabelViewModel newitem = new LabelViewModel(this);
                newitem.DefectLabel = newlabel;
                newitem.IsEnabled = true;
                newitem.IsSelected = true;
                _defectClassList.Add(newitem);
                SelectedItem = newitem;

                Filter = null;
            }
            ));
            }
        }

        //=================================================================
        // 
        //=================================================================
        private AutoRelayCommand _deleteCommand = null;
        public AutoRelayCommand DeleteCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new AutoRelayCommand(
            () =>
            {
                for (int i = _defectClassList.Count() - 1; i >= 0; i--)
                {
                    LabelViewModel item = _defectClassList[i];
                    if (item.IsSelected)
                    {
                        _defectClassList.RemoveAt(i);
                        DefectLabelStore.RemoveLabel(item.DefectLabel);
                    }
                }
            },
            () => !IsSelectionEmpty()));
            }
        }

        //=================================================================
        // 
        //=================================================================
        private AutoRelayCommand _okCommand = null;
        public AutoRelayCommand OkCommand
        {
            get
            {
                return _okCommand ?? (_okCommand = new AutoRelayCommand(
            () => { },
            () => !IsSelectionEmpty()
            ));
            }
        }

        //=================================================================
        // 
        //=================================================================
        private bool LabelFilter(object obj)
        {
            if (!string.IsNullOrEmpty(_filter))
            {
                LabelViewModel item = obj as LabelViewModel;
                return item.DefectLabel.ToLower().Contains(_filter.ToLower());
            }
            else
                return true;
        }


    }
}
