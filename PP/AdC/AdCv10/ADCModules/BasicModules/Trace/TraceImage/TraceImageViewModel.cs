using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using ADCEngine;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BasicModules.Trace
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class TraceImageViewModel : ObservableRecipient
    {
        public TraceImageModule TraceImageModule;

        //=================================================================
        // Constructeur
        //=================================================================
        public TraceImageViewModel(TraceImageModule module)
        {
            TraceImageModule = module;
            _traceListView = CollectionViewSource.GetDefaultView(_traceList);
            _traceListView.Filter = CustomerFilter;
        }

        //=================================================================
        // SelectedImage
        //=================================================================
        private TraceImage _selectedTrace;
        public TraceImage SelectedTrace
        {
            get { return _selectedTrace; }
            set
            {
                if (_selectedTrace == value)
                    return;
                _selectedTrace = value;

                if (_selectedTrace != null)
                {
                    bool reallocated = _selectedTrace.CopyToWriteableBitmap(ref _writeableBitmap);
                    if (reallocated)
                        OnPropertyChanged(nameof(WriteableBitmap));
                }
                else if (_writeableBitmap != null)
                {
                    _writeableBitmap = null;
                    OnPropertyChanged(nameof(WriteableBitmap));
                }
                OnPropertyChanged(nameof(SelectedTrace));
            }
        }

        // 
        //.................
        public void UpdateSelectedImage()
        {
            if (Paused)
                return;
            TraceImage trace = TraceImageModule.GetLastTrace();
            if (trace != null)
            {
                using (trace)
                    SelectedTrace = trace;
            }
        }

        // Bitmap WPF pour l'affichage
        //............................
        private WriteableBitmap _writeableBitmap;
        public WriteableBitmap WriteableBitmap
        {
            get { return _writeableBitmap; }
        }

        //=================================================================
        // Paused / Stop / Eject
        //=================================================================
        private bool _paused;
        public bool Paused
        {
            get { return _paused; }
            set
            {
                if (value == _paused)
                    return;
                _paused = value;
                if (_paused && !_ejected)
                {
                    _traceList.Clear();
                    foreach (TraceImage trace in TraceImageModule.GetTraceList())
                        _traceList.Add(trace);

                    _traceListView.Refresh();
                    if (_traceList.Count() > 0)
                        SelectedTrace = _traceList.Last();
                    else
                        SelectedTrace = null;
                }
                OnPropertyChanged();
            }
        }

        //=================================================================
        // Eject
        //=================================================================
        private bool _ejected;
        public bool Ejected
        {
            get
            {
                return _ejected;
            }
            set
            {
                if (value == _ejected)
                    return;
                _ejected = value;
                if (_ejected)
                {
                    Paused = true;

                    // Nettoyage de l'IHM
                    //...................
                    ImageSourceList.Clear();
                    _traceList.Clear();
                    _traceListView.Refresh();
                    SelectedTrace = null;

                    // Stop du module
                    //...............
                    TraceImageModule.Eject();
                }
                OnPropertyChanged();
            }
        }

        //=================================================================
        // List of Modules that can produce images
        //=================================================================
        private List<object> _imageSourceList = new List<object>();
        public List<object> ImageSourceList
        {
            get
            {
                return _imageSourceList;
            }
            set
            {
                _imageSourceList = value;
                _imageSourceList.Add("Show all");
            }
        }


        //=================================================================
        // The list of Images
        //=================================================================
        // ObservableCollection pour creer la ICollectionView
        //...................................................
        private ObservableCollection<TraceImage> _traceList = new ObservableCollection<TraceImage>();
        public IEnumerable<TraceImage> TraceList
        {
            get { return new List<TraceImage>(_traceList); }
        }

        // ICollectionView pour la liste des traces dans la listebox
        //..........................................................
        private ICollectionView _traceListView; // Cf constructeur pour l'init
        public ICollectionView TraceListView
        {
            get { return _traceListView; }
        }

        //=================================================================
        // Filters
        //=================================================================
        public ModuleBase _sourceModuleFilter;
        public object SourceModuleFilter
        {
            get { return _sourceModuleFilter; }
            set
            {
                _sourceModuleFilter = value as ModuleBase;
                OnPropertyChanged();
                _traceListView.Refresh();
            }
        }

        public string _stringFilter = "";
        public string StringFilter
        {
            get { return _stringFilter; }
            set
            {
                _stringFilter = value;
                OnPropertyChanged();
                _traceListView.Refresh();
            }
        }

        private bool CustomerFilter(object item)
        {
            TraceImage trace = item as TraceImage;

            bool bMatch = true;
            bMatch = bMatch && (
                    (SourceModuleFilter == null) ||
                    (trace.sourceModule == SourceModuleFilter)
                );
            bMatch = bMatch && (trace.ToString().Contains(_stringFilter));

            return bMatch;
        }

    }

}
