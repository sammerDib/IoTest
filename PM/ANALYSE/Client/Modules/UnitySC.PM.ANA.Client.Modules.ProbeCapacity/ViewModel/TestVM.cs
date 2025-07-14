using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Service.Interface.Compatibility;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Modules.ProbeCapacity.ViewModel
{
    public class TestVM : ObservableObject
    {
        public ObservableCollection<LayerVM> Layers { get; set; }
        public IEnumerable<TestMeasureType> MeasureTypes { get; private set; }
        private ProbeCapacityVM _probeCapacity;


        private TestMeasureType _selectedMeasureType;
        public TestMeasureType SelectedMeasureType
        {
            get => _selectedMeasureType; set { if (_selectedMeasureType != value) { _selectedMeasureType = value; OnPropertyChanged(); } }
        }

        public TestVM(ProbeCapacityVM probeCapacity)
        {
            Layers = new ObservableCollection<LayerVM>();
            MeasureTypes = Enum.GetValues(typeof(TestMeasureType)).Cast<TestMeasureType>();
            SelectedMeasureType = MeasureTypes.First();
            _probeCapacity = probeCapacity;
        }

        private AutoRelayCommand _addLayer;
        public AutoRelayCommand AddLayer
        {
            get
            {
                return _addLayer ?? (_addLayer = new AutoRelayCommand(
              () =>
              {
                  foreach(var layer in Layers)
                  {
                      layer.InEdition = false;
                  }
                  var newLayer = new LayerVM();
                  newLayer.Name = "MeasurableLayers "+(Layers.Count +1);
                  newLayer.RefractiveIndex = 1.5;
                  newLayer.Thickness = 100;
                  newLayer.InEdition = true;
                  Layers.Add(newLayer);
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand<LayerVM> _deleteLayer;
        public AutoRelayCommand<LayerVM> DeleteLayer
        {
            get
            {
                return _deleteLayer ?? (_deleteLayer = new AutoRelayCommand<LayerVM>(
              (layer) =>
              {
                  var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Do you really want to remove this layer ?", "Remove item Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                  if (msgresult == MessageBoxResult.Yes)
                  {
                      Layers.Remove(layer);
                  }
              },
              (layer) => { return layer != null; }));
            }
        }

        private AutoRelayCommand _startTest;
        public AutoRelayCommand StartTest
        {
            get
            {
                return _startTest ?? (_startTest = new AutoRelayCommand(
              () =>
              {
                  var probeCapacity = _probeCapacity.VMToModel();

              },
              () => { return true; }));
            }
        }
    }
}
