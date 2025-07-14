using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.ViewModel
{
    public enum LogicalFilter
    {
        Or,
        And
    };

    public class DefectBinsVM : ObservableObject
    {
        private readonly DefectBins _model;
        private bool _hasBeenModified = false;
        private bool _hasBeenSaved = true;

        private readonly Random _rnd = new Random();

        /// <summary>
        /// Called when valid defect bins model are saved, could allow to perform any update on table for example.
        /// </summary>
        public event Action<DefectBins> OnSaveDefectBins;

        public event Action<DefectBins> OnExportDefectBins;

        public event Action OnImportDefectBins;

        private bool _isValid = true;

        public bool IsValid
        {
            get => _isValid; set { if (_isValid != value) { _isValid = value; OnPropertyChanged(); SaveBinCommand.NotifyCanExecuteChanged(); } }
        }

        public ICollectionView BinsCV { get; private set; }

        private ObservableCollection<DefectBinVM> _defbinsList;

        public ObservableCollection<DefectBinVM> ListBins
        {
            get => _defbinsList; set { if (_defbinsList != value) { _defbinsList = value; OnPropertyChanged(); } }
        }

        private int _newRoughBin = 0;

        public int NewRoughBin
        {
            get => _newRoughBin; set { if (_newRoughBin != value) { _newRoughBin = value; OnPropertyChanged(); } }
        }

        private string _newLabel = "New Defect";

        public string NewLabel
        {
            get => _newLabel; set { if (_newLabel != value) { _newLabel = value; OnPropertyChanged(); } }
        }

        private Color _newcolor = Color.Blue;

        public Color NewColor
        {
            get => _newcolor; set { if (_newcolor != value) { _newcolor = value; OnPropertyChanged(); } }
        }

        private LogicalFilter _logicalfilterenum;

        public LogicalFilter LogicalFilterEnum
        {
            get => _logicalfilterenum; set { if (_logicalfilterenum != value) { _logicalfilterenum = value; OnPropertyChanged(); BinsCV.Refresh(); } }
        }

        private string _rbfilters;

        public string RoughBinFilters
        {
            get => _rbfilters; set { if (_rbfilters != value) { _rbfilters = value; OnPropertyChanged(); BinsCV.Refresh(); } }
        }

        private string _lblfilters;

        public string LabelFilters
        {
            get => _lblfilters; set { if (_lblfilters != value) { _lblfilters = value; OnPropertyChanged(); BinsCV.Refresh(); } }
        }

        private bool CollectionFilter(object obj)
        {
            var result = obj as DefectBinVM;
            if (result == null)
                return false;

            bool bUseRB = false;
            bool bRB = true;
            if (!string.IsNullOrEmpty(_rbfilters))
            {
                bUseRB = true;
                string sRB = result.RoughBin.ToString();
                bRB = sRB.Contains(_rbfilters);
            }

            bool bUseLbl = false;
            bool bLbl = true;
            if (!string.IsNullOrEmpty(_lblfilters))
            {
                bUseLbl = true;
                bLbl = result.Label.ToLower().Contains(_lblfilters.ToLower());
            }

            if (!bUseRB && !bUseLbl)
                return true; // aucun filtre on affiche tout

            if (bUseRB == bUseLbl) //on utilise les  2
                return (LogicalFilterEnum == LogicalFilter.Or) ? bLbl || bRB : bLbl && bRB;
            else
                return bUseRB ? bRB : bLbl;
        }

        public void Bin_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // OnPropertyChanged(nameof(NbDefects));
            _hasBeenModified = true;
            _hasBeenSaved = false;

            // Check if entries are valid
            // Unique area max key
            var ll = _defbinsList.ToList().Select(x => x.RoughBin).Distinct();
            IsValid = ll.Count() == _defbinsList.Count;

            SaveBinCommand.NotifyCanExecuteChanged();
            ExportXmlCommand.NotifyCanExecuteChanged();
            ImportXmlCommand.NotifyCanExecuteChanged();
        }

        public DefectBinsVM(DefectBins defetcBinsmodel)
        {
            _model = defetcBinsmodel;
            _defbinsList = new ObservableCollection<DefectBinVM>();
            foreach (var bin in _model.DefectBinList)
            {
                var newbin = new DefectBinVM(bin);
                newbin.OnDefBinChange += Bin_CollectionChanged;
                ListBins.Add(newbin);
            }
            _defbinsList.CollectionChanged += Bin_CollectionChanged;

            BinsCV = CollectionViewSource.GetDefaultView(_defbinsList);
            BinsCV.SortDescriptions.Add(new SortDescription("RoughBin", ListSortDirection.Ascending));
            BinsCV.Filter = CollectionFilter;

            NewColor = RandomColor();
        }

        private Color RandomColor(bool randomOpacity = false)
        {
            if (!randomOpacity)
                return Color.FromArgb(255, (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255));
            return Color.FromArgb((byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255), (byte)_rnd.Next(0, 255));
        }

        public void AddNewBin()
        {
            _hasBeenModified = true;
            _hasBeenSaved = false;
            SaveBinCommand.NotifyCanExecuteChanged();
            ExportXmlCommand.NotifyCanExecuteChanged();
            ImportXmlCommand.NotifyCanExecuteChanged();

            var newemptybin = new DefectBinVM(NewRoughBin, NewLabel, NewColor.ToArgb());
            newemptybin.OnDefBinChange += Bin_CollectionChanged;
            ListBins.Add(newemptybin);
        }

        public void DeleteSelectedBin()
        {
            if (SelectedBin != null)
            {
                if (System.Windows.Forms.MessageBox.Show(string.Format("Do you want to delete the Rough Bin = {0} ?", SelectedBin.RoughBin), "Delete",
                  System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question,
                  System.Windows.Forms.MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    return;

                _hasBeenModified = true;
                _hasBeenSaved = false;
                SaveBinCommand.NotifyCanExecuteChanged();
                ExportXmlCommand.NotifyCanExecuteChanged();
                ImportXmlCommand.NotifyCanExecuteChanged();

                SelectedBin.OnDefBinChange -= Bin_CollectionChanged;
                ListBins.Remove(SelectedBin);
            }
        }

        public void BinsSaved()
        {
            _hasBeenModified = false;
            _hasBeenSaved = true;

            SaveBinCommand.NotifyCanExecuteChanged();
            ExportXmlCommand.NotifyCanExecuteChanged();
            ImportXmlCommand.NotifyCanExecuteChanged();
        }

        public void SaveBinsToModel()
        {
            _model.Reset();
            foreach (var bin in ListBins)
            {
                _model.Add(new DefectBin(bin.RoughBin, bin.Label, bin.Color.ToArgb()));
            }
            OnSaveDefectBins?.Invoke(_model);
        }

        public void ExportModel()
        {
            OnExportDefectBins?.Invoke(_model);
        }

        public void ImportModel()
        {
            OnImportDefectBins?.Invoke();
        }

        /// <summary>
        /// Selected Bin
        /// </summary>
        private DefectBinVM _selectedItem;

        public DefectBinVM SelectedBin
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                    DeleteBinCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private AutoRelayCommand _addBinCommand;

        public AutoRelayCommand AddBinCommand
        {
            get
            {
                return _addBinCommand ?? (_addBinCommand = new AutoRelayCommand(
              () =>
              {
                  AddNewBin();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _deleteBinCommand;

        public AutoRelayCommand DeleteBinCommand
        {
            get
            {
                return _deleteBinCommand ?? (_deleteBinCommand = new AutoRelayCommand(
              () =>
              {
                  DeleteSelectedBin();
              },
              () => { return SelectedBin != null; }));
            }
        }

        private AutoRelayCommand _randomNewColorCommand;

        public AutoRelayCommand RandomNewColorCommand
        {
            get
            {
                return _randomNewColorCommand ?? (_randomNewColorCommand = new AutoRelayCommand(
              () =>
              {
                  NewColor = RandomColor();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand _saveBinCommand;

        public AutoRelayCommand SaveBinCommand
        {
            get
            {
                return _saveBinCommand ?? (_saveBinCommand = new AutoRelayCommand(
              () =>
              {
                  SaveBinsToModel();
              },
              () => { return _hasBeenModified && IsValid; }));
            }
        }

        private AutoRelayCommand _exportXmlCommand;

        public AutoRelayCommand ExportXmlCommand
        {
            get
            {
                return _exportXmlCommand ?? (_exportXmlCommand = new AutoRelayCommand(
              () =>
              {
                  ExportModel();
              },
              () => { return _hasBeenSaved; }));
            }
        }

        private AutoRelayCommand _importXmlCommand;

        public AutoRelayCommand ImportXmlCommand
        {
            get => _importXmlCommand ?? (_importXmlCommand = new AutoRelayCommand(() =>
            {
                ImportModel();
            },
                () => { return _hasBeenSaved; }));
        }
    }
}
