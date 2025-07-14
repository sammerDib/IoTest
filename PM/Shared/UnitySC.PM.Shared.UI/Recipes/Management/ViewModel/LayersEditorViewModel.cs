using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;
using UnitySC.Shared.UI.Dialog;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class LayersEditorViewModel : ObservableObject
    {
        private Step _step;
        private Action _onChange;
        public ObservableCollection<LayerViewModel> Layers { get; set; }

        public bool EditLayersThicknessTolerance { get; set; } = false;

        /// <summary>
        /// LayersEditorViewModel
        /// </summary>
        /// <param name="onChange"Action to invoke when the layers change </param>
        public LayersEditorViewModel(Action onChange = null)
        {
            Layers = new ObservableCollection<LayerViewModel>();
            _onChange = onChange;
        }

        private void SignalChange()
        {
            if (_onChange != null)
                _onChange.Invoke();
        }

        public void Init(DataAccess.Dto.Step step)
        {
            _step = step;
            Layers.Clear();
            int colorIndex = 0;
            foreach (var layer in _step.Layers)
            {
                var newLayer = new LayerViewModel(_step);
                newLayer.Name = layer.Name;
                newLayer.RefractiveIndex = layer.RefractiveIndex;
                newLayer.IsRefractiveIndexUnknown = newLayer.RefractiveIndex == null || double.IsNaN(layer.RefractiveIndex.Value);
                newLayer.Thickness = new Length(layer.Thickness, LengthUnit.Micrometer);
                newLayer.LayerColor = GetLayerColor(null, colorIndex);
                colorIndex++;

                Layers.Add(newLayer);
            }
        }

        public static Color GetLayerColor(Material material, int colorIndex)
        {
            if (material is null)
            {
                var brush = (Brush)Application.Current.FindResource("LayerBrushIndex" + colorIndex % 6);
                return (brush as SolidColorBrush).Color;
            }
            // TODO Manage the colors corresponding to the material
            return Colors.DarkGray;
        }

        private AutoRelayCommand _addTopLayer;
        public AutoRelayCommand AddTopLayer
        {
            get
            {
                return _addTopLayer ?? (_addTopLayer = new AutoRelayCommand(
              () =>
              {
                  InsertLayerAtPosition(0);
                  SignalChange();
              },
              () => { return true; }));
            }
        }

        private AutoRelayCommand<LayerViewModel> _addLayerAfter;
        public AutoRelayCommand<LayerViewModel> AddLayerAfter
        {
            get
            {
                return _addLayerAfter ?? (_addLayerAfter = new AutoRelayCommand<LayerViewModel>(
              (layer) =>
              {

                  int indexPosToInsert = Layers.IndexOf(layer);
                  indexPosToInsert++;
                  InsertLayerAtPosition(indexPosToInsert);
                  SignalChange();
              },
              (layer) => { return layer != null; }));
            }
        }

        private void InsertLayerAtPosition(int indexPosToInsert)
        {
            var newLayer = new LayerViewModel(_step);
            newLayer.Name = "Layer " + (Layers.Count + 1);
            newLayer.RefractiveIndex = 1.5f;
            newLayer.Thickness = 100.Micrometers();
            newLayer.InEdition = true;
            newLayer.LayerColor = GetLayerColor(null, Layers.Count);
            Layers.Insert(indexPosToInsert, newLayer);
        }



        public bool IsEditing => Layers.Any(l => l.InEdition);

        private AutoRelayCommand<LayerViewModel> _deleteLayer;
        public AutoRelayCommand<LayerViewModel> DeleteLayer
        {
            get
            {
                return _deleteLayer ?? (_deleteLayer = new AutoRelayCommand<LayerViewModel>(
              (layer) =>
              {
                  var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().ShowMessageBox("Do you really want to remove this layer ?", "Remove item Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                  if (msgresult == MessageBoxResult.Yes)
                  {
                      Layers.Remove(layer);
                      SignalChange();
                  }
              },
              (layer) => { return layer != null; }));
            }
        }


        private AutoRelayCommand<LayerViewModel> _changeLayerEditionState;
        public AutoRelayCommand<LayerViewModel> ChangeLayerEditionState
        {
            get
            {
                return _changeLayerEditionState ?? (_changeLayerEditionState = new AutoRelayCommand<LayerViewModel>(
              (layer) =>
              {
                  if (Layers.Any(l => l.Name == layer.Name && !l.Equals(layer)))
                  {
                      ClassLocator.Default.GetInstance<IDialogOwnerService>().ShowMessageBox($"A layer with the name {layer.Name} already exists",
                          "Layer Edition", MessageBoxButton.OK, MessageBoxImage.Error);
                  }
                  else
                  {
                      layer.InEdition = !layer.InEdition;
                      SignalChange();
                  }
              },
              (layer) => { return layer != null; }));
            }
        }


        private AutoRelayCommand _deleteAll;
        public AutoRelayCommand DeleteAll
        {
            get
            {
                return _deleteAll ?? (_deleteAll = new AutoRelayCommand(
              () =>
              {
                  var msgresult = ClassLocator.Default.GetInstance<UnitySC.Shared.UI.Dialog.IDialogOwnerService>().
                  ShowMessageBox("Are you sure you want to delete all the layers?", "Remove items Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

                  if (msgresult == MessageBoxResult.Yes)
                  {
                      Layers.Clear();
                      SignalChange();
                  }
              },
              () => { return Layers.Count > 0; }));
            }
        }

    }
}
