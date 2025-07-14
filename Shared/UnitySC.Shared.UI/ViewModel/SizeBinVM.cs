using System;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.Data;

namespace UnitySC.Shared.UI.ViewModel
{
    public class SizeBinVM : ObservableObject, IComparable<SizeBinVM>
    {
        public event Action<object, System.Collections.Specialized.NotifyCollectionChangedEventArgs> OnSizeBinChange;

        private long _areamaxum;

        public long AreaMax_um
        {
            get => _areamaxum; set { if (_areamaxum != value) { _areamaxum = value; OnPropertyChanged(); OnSizeBinChange?.Invoke(this, null); } }
        }

        private int _sizeum;

        public int Size_um
        {
            get => _sizeum; set { if (_sizeum != value) { _sizeum = value; OnPropertyChanged(); OnSizeBinChange?.Invoke(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, null)); } }
        }

        public SizeBinVM()
        {
            AreaMax_um = 0;
            Size_um = 0;
        }

        public SizeBinVM(long areaMax_um, int size_um)
        {
            AreaMax_um = areaMax_um;
            Size_um = size_um;
        }

        public SizeBinVM(SizeBin model)
        {
            if (model != null)
            {
                AreaMax_um = model.AreaMax_um;
                Size_um = model.Size_um;
            }
        }

        public int CompareTo(SizeBinVM that)
        {
            return AreaMax_um.CompareTo(that.AreaMax_um);
        }
    }
}