using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.Shared.UI.ViewModel
{
    public class SizeBinsVM : ObservableObject
    {
        private readonly SizeBins _model;
        private bool _hasBeenModified = false;
        private bool _hasBeenSaved = true;

        /// <summary>
        /// Called when valid size bins model are saved, could allow to perform any update on table for example.
        /// </summary>
        public event Action<SizeBins> OnSaveSizeBins;

        public event Action<SizeBins> OnExportSizeBins;

        public event Action OnImportSizeBins;

        private int _waferDiam_mm;

        public int PreviewWaferDiameter
        {
            get => _waferDiam_mm;
            set
            {
                if (_waferDiam_mm != value)
                {
                    _waferDiam_mm = value;
                    OnPropertyChanged();
                    UpdatePreview();
                }
            }
        }

        private double _previewsquaresize = 1.0;

        public double PreviewSquareSize
        {
            get => _previewsquaresize; set { if (_previewsquaresize != value) { _previewsquaresize = value; OnPropertyChanged(); } }
        }

        private bool _isValid = true;

        public bool IsValid
        {
            get => _isValid; set { if (_isValid != value) { _isValid = value; OnPropertyChanged(); SaveBinCommand.NotifyCanExecuteChanged(); } }
        }

        public ICollectionView BinsCV { get; private set; }

        private ObservableCollection<SizeBinVM> _szbinsList;

        public ObservableCollection<SizeBinVM> ListBins
        {
            get => _szbinsList; set { if (_szbinsList != value) { _szbinsList = value; OnPropertyChanged(); } }
        }

        private void UpdatePreview()
        {
            int thumbsize = 256;
            double ConversionMicronToPixelAdjusted = thumbsize / (double)PreviewWaferDiameter / 1000.0;
            int SquareWidth_um = 0;
            if (SelectedBin != null)
                SquareWidth_um = SelectedBin.Size_um;
            double dSize = SquareWidth_um * ConversionMicronToPixelAdjusted;
            if (dSize < 1.0)
                dSize = 1.0;
            PreviewSquareSize = dSize;
        }

        public void Bin_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // OnPropertyChanged(nameof(NbDefects));
            _hasBeenModified = true;
            _hasBeenSaved = false;

            // Check if entries are valid
            // Unique area max key
            var ll = _szbinsList.ToList().Select(x => x.AreaMax_um).Distinct();
            IsValid = ll.Count() == _szbinsList.Count;

            SaveBinCommand.NotifyCanExecuteChanged();
            ExportXmlCommand.NotifyCanExecuteChanged();
            ImportXmlCommand.NotifyCanExecuteChanged();

            // is modifcation came from size_um modification
            if (e != null)
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    // this specific message indicate existing size bin size has changed
                    // need update preview
                    UpdatePreview();
                }
            }
        }

        public SizeBinsVM(SizeBins sizeBinsmodel)
        {
            _waferDiam_mm = 200;

            _model = sizeBinsmodel;

            _szbinsList = new ObservableCollection<SizeBinVM>();
            foreach (var bin in _model.ListBins)
            {
                var newbin = new SizeBinVM(bin);
                newbin.OnSizeBinChange += Bin_CollectionChanged;
                ListBins.Add(newbin);
            }
            _szbinsList.CollectionChanged += Bin_CollectionChanged;

            BinsCV = CollectionViewSource.GetDefaultView(_szbinsList);
            BinsCV.SortDescriptions.Add(new SortDescription("AreaMax_um", ListSortDirection.Ascending));
        }

        public void AddNewBin()
        {
            _hasBeenModified = true;
            _hasBeenSaved = false;
            SaveBinCommand.NotifyCanExecuteChanged();
            ExportXmlCommand.NotifyCanExecuteChanged();
            ImportXmlCommand.NotifyCanExecuteChanged();

            if (SelectedBin == null)
            {
                var newemptybin = new SizeBinVM();
                newemptybin.OnSizeBinChange += Bin_CollectionChanged;
                ListBins.Add(newemptybin);
            }
            else
            {
                var newbin = new SizeBinVM(SelectedBin.AreaMax_um + 1, SelectedBin.Size_um + 1);
                newbin.OnSizeBinChange += Bin_CollectionChanged;
                ListBins.Add(newbin);
            }
        }

        public void DeleteSelectedBin()
        {
            if (SelectedBin != null)
            {
                if (System.Windows.Forms.MessageBox.Show(string.Format("Do you want to delete the Bin area = {0} ?", SelectedBin.AreaMax_um), "Delete",
                  System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question,
                  System.Windows.Forms.MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.No)
                    return;
                //{
                //    ListBins.Remove(SelectedBin);
                //}
                //

                _hasBeenModified = true;
                _hasBeenSaved = false;
                SaveBinCommand.NotifyCanExecuteChanged();
                ExportXmlCommand.NotifyCanExecuteChanged();
                ImportXmlCommand.NotifyCanExecuteChanged();

                SelectedBin.OnSizeBinChange -= Bin_CollectionChanged;
                ListBins.Remove(SelectedBin);
            }
        }

        public void SaveBinsToModel()
        {
            _model.Reset();
            foreach (var bin in ListBins)
            {
                _model.AddBin(bin.AreaMax_um, bin.Size_um);
            }
            _model.Arrange();
            OnSaveSizeBins?.Invoke(_model);
        }

        public void BinsSaved()
        {
            _hasBeenModified = false;
            _hasBeenSaved = true;

            SaveBinCommand.NotifyCanExecuteChanged();
            ExportXmlCommand.NotifyCanExecuteChanged();
            ImportXmlCommand.NotifyCanExecuteChanged();
        }

        public void ExportModel()
        {
            OnExportSizeBins?.Invoke(_model);
        }

        public void ImportModel()
        {
            OnImportSizeBins?.Invoke();
        }

        /// <summary>
        /// Selected Bin
        /// </summary>
        private SizeBinVM _selectedItem;

        public SizeBinVM SelectedBin
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                    DeleteBinCommand.NotifyCanExecuteChanged();

                    UpdatePreview();
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
