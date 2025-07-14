using System.Collections.Generic;
using System.Linq;

using AdcBasicObjects;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace BasicModules.MilCharacterization
{
    [System.Reflection.Obfuscation(Exclude = true)]
    internal class MilCharacterizationViewModel : ObservableRecipient
    {
        public MilCharacterizationParameter Parameter;
        private MilCharacterizationModule Module { get { return (MilCharacterizationModule)Parameter.Module; } }
        private List<Characteristic> _sizeCharacteristics = new List<Characteristic>()
        {
            ClusterCharacteristics.Area,
            ClusterCharacteristics.Perimeter,
            ClusterCharacteristics.Length,
            ClusterCharacteristics.Breadth,
            ClusterCharacteristics.ConvexPerimeter,
            ClusterCharacteristics.SurroundingRectangleArea,
            ClusterCharacteristics.RealDiameter,
            ClusterCharacteristics.RealHeight,
            ClusterCharacteristics.RealWidth
        };

        private List<Characteristic> _shapeCharacteristics = new List<Characteristic>()
        {
            ClusterCharacteristics.SymetricDispersion,
            ClusterCharacteristics.EulerNumber,
            ClusterCharacteristics.Roughness,
            ClusterCharacteristics.FillingValue,
            ClusterCharacteristics.Compactness,
            ClusterCharacteristics.Elongation,
            ClusterCharacteristics.RatioVertical,
            ClusterCharacteristics.BlobCount,
            ClusterCharacteristics.Barycenter
        };

        private List<Characteristic> _positionCharacteristics = new List<Characteristic>()
        {
            ClusterCharacteristics.AxisPrincipalAngle,
            ClusterCharacteristics.AxisSecondaryAngle,
            ClusterCharacteristics.RadialPosition,
            ClusterCharacteristics.AbsolutePosition,
            ClusterCharacteristics.AnglePosition
        };

        private List<Characteristic> _greyLevelCharacteristics = new List<Characteristic>()
        {
            ClusterCharacteristics.BlobMinGreyLevel,
            ClusterCharacteristics.ClusterMinGreyLevel,
            ClusterCharacteristics.BlobMaxGreyLevel,
            ClusterCharacteristics.ClusterMaxGreyLevel,
            ClusterCharacteristics.ClusterAverageGreyLevel,
            ClusterCharacteristics.BlobAverageGreyLevel,
            ClusterCharacteristics.BlobStandardDev,
            ClusterCharacteristics.ClusterStandardDev,
            ClusterCharacteristics.SumLevel,
        };


        //=================================================================
        // Propriétées bindables
        //=================================================================
        public List<ListViewItem> ItemList { get; private set; }
        private ListViewItem _selectedItem;
        public ListViewItem SelectedItem
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


        public bool SelectAll
        {
            get => ItemList.All(x => x.IsSelected);
            set
            {
                if (value != SelectAll)
                {
                    ItemList.ForEach(x => x.IsSelected = value);
                }
                OnPropertyChanged();
            }
        }

        public IEnumerable<ListViewItem> SizeCharacteristics
        {
            get => ItemList.Where(x => _sizeCharacteristics.Contains(x.Characteristic));
        }

        public IEnumerable<ListViewItem> ShapeCharacteristics
        {
            get => ItemList.Where(x => _shapeCharacteristics.Contains(x.Characteristic));
        }

        public IEnumerable<ListViewItem> PositionCharacteristics
        {
            get => ItemList.Where(x => _positionCharacteristics.Contains(x.Characteristic));
        }

        public IEnumerable<ListViewItem> GreyLevelCharacteristics
        {
            get => ItemList.Where(x => _greyLevelCharacteristics.Contains(x.Characteristic));
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public MilCharacterizationViewModel(MilCharacterizationParameter parameter)
        {
            Parameter = parameter;

            ItemList = new List<ListViewItem>();
            foreach (Characteristic carac in Module.SupportedCharacteristics)
                ItemList.Add(new ListViewItem(Parameter, carac));
        }

        private AutoRelayCommand<ListViewItem> _focusedItemChangedCommand;
        public AutoRelayCommand<ListViewItem> FocusedItemChangedCommand
        {
            get
            {
                return _focusedItemChangedCommand ?? (_focusedItemChangedCommand = new AutoRelayCommand<ListViewItem>(
              (listViewItem) =>
              {
                  FocusedItemChanged(listViewItem);
              }));
            }
        }

        private void FocusedItemChanged(ListViewItem listViewItem)
        {
            Parameter.SelectedOption = listViewItem?.Characteristic.ToString();
            SelectedItem = listViewItem;
        }
    }
}
