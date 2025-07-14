using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.MeasuresSettings;
using UnitySC.PM.ANA.Client.CommonUI.ViewModel.RecipeEditionSteps.Measures.ProbeSelector;
using UnitySC.PM.ANA.Service.Interface.Measure;
using UnitySC.Shared.Tools.Collection;
using UnitySC.Shared.Tools.Tolerances;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.Extensions;

namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class ThicknessLayerToMeasureViewModel : ObservableObject, IDisposable
    {
        #region properties

        private bool _isWaferThickness = false;

        public bool IsWaferThickness
        {
            get => _isWaferThickness; set { if (_isWaferThickness != value) { _isWaferThickness = value; UpdateName(); OnPropertyChanged(); } }
        }

        private bool _isMeasured;

        public bool IsMeasured
        {
            get => _isMeasured; set { if (_isMeasured != value) { _isMeasured = value; OnPropertyChanged(); } }
        }

        private bool _isEditing = false;

        public bool IsEditing
        {
            get => _isEditing; set { if (_isEditing != value) { _isEditing = value; OnPropertyChanged(); } }
        }

        private TrulyObservableCollection<LayerViewModel> _layers;

        public TrulyObservableCollection<LayerViewModel> Layers
        {
            get
            {
                if (_layers is null)
                {
                    _layers = new TrulyObservableCollection<LayerViewModel>();
                    _layers.CollectionChanged += LayersCollectionChanged;
                    _layers.ItemPropertyChanged += LayersItemPropertyChanged;
                }
                return _layers;
            }
            set
            {
                if (_layers != value)
                {
                    if (_layers != null)
                    {
                        _layers.CollectionChanged -= LayersCollectionChanged;
                        _layers.ItemPropertyChanged -= LayersItemPropertyChanged;
                    }

                    _layers = value;
                    _layers.CollectionChanged += LayersCollectionChanged;
                    _layers.ItemPropertyChanged += LayersItemPropertyChanged;

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Thickness));
                    OnPropertyChanged(nameof(HasMultipleLayers));
                    OnPropertyChanged(nameof(DisplayHeight));
                    UpdateName();
                }
            }
        }

        private void LayersItemPropertyChanged(System.Collections.IList sourceList, object item, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Thickness));
            OnPropertyChanged(nameof(DisplayHeight));
            OnPropertyChanged(nameof(ThicknessTolerance));
        }

        private void LayersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(HasMultipleLayers));
            OnPropertyChanged(nameof(LayerColor));
            OnPropertyChanged(nameof(Thickness));
            UpdateName();
        }

        public bool HasMultipleLayers => Layers.Count > 1;

        private string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        private void UpdateName()
        {
            if (IsWaferThickness)
            {
                _name = ThicknessSettingsVM.WaferThicknessName;
            }
            else
            {
                string newName = string.Empty;

                foreach (var layer in Layers)
                {
                    if (!string.IsNullOrEmpty(newName))
                        newName += " - ";
                    newName += layer.Name;
                }
                _name = newName;
            }
            OnPropertyChanged(nameof(Name));
        }

        public Length Thickness => GetThickness();

        private Length GetThickness()
        {
            var totalThickness = new Length(0, LengthUnit.Micrometer);
            foreach (var layer in Layers)
            {
                totalThickness += layer.Thickness;
            }
            return totalThickness;
        }

        private double? _refractiveIndex;

        public double? RefractiveIndex
        {
            get => _refractiveIndex; set { if (_refractiveIndex != value) { _refractiveIndex = value; OnPropertyChanged(); } }
        }

        public double DisplayHeight => Layers?.Count is null?100:Layers.Count* 100;

        private LengthTolerance _thicknessTolerance = new LengthTolerance(1, LengthToleranceUnit.Micrometer);

        public LengthTolerance ThicknessTolerance
        {
            get => _thicknessTolerance; set { if (_thicknessTolerance != value) { _thicknessTolerance = value; OnPropertyChanged(); } }
        }

        private Length _thicknessOffset = 0.Micrometers();

        public Length ThicknessOffset
        {
            get => _thicknessOffset; set { if (_thicknessOffset != value) { _thicknessOffset = value; OnPropertyChanged(); } }
        }

        public Color LayerColor => GetLayerColor();

        private Color GetLayerColor()
        {
            if (Layers.Count == 0)
                return Colors.Gray;

            if (IsWaferThickness)
                return Colors.LightGray;

            if (Layers.Count == 1)
                return Layers[0].LayerColor;


            int SumR = 0, SumG = 0, SumB = 0;
            foreach (var layer in Layers)
            {
                SumR += layer.LayerColor.R;
                SumG += layer.LayerColor.G;
                SumB += layer.LayerColor.B;
            }

            var blendedColor = Color.FromRgb((byte)(SumR / Layers.Count), (byte)(SumG / Layers.Count), (byte)(SumB / Layers.Count));

            return blendedColor;
        }

        private ProbeSelectorVM _probeSelector = new ProbeSelectorVM();

        public ProbeSelectorVM ProbeSelector
        {
            get => _probeSelector; set { if (_probeSelector != value) { _probeSelector = value; OnPropertyChanged(); } }
        }

        private ThicknessMeasureToolsForLayer _measureTools;

        public ThicknessMeasureToolsForLayer MeasureTools
        {
            get => _measureTools;
            set
            {
                if (_measureTools != value)
                {
                    _measureTools = value;
                    UpdateProbes();
                    OnPropertyChanged();
                }
            }
        }

        #endregion properties

        #region RelayCommands

        private AutoRelayCommand _startEdit;

        public AutoRelayCommand StartEdit
        {
            get
            {
                return _startEdit ?? (_startEdit = new AutoRelayCommand(
                    () =>
                    {
                        IsEditing = true;
                        ProbeSelector.IsEditing = true;
                    },
                    () => { return true; }
                ));
            }
        }

        private AutoRelayCommand _submit;

        public AutoRelayCommand Submit
        {
            get
            {
                return _submit ?? (_submit = new AutoRelayCommand(
                    () =>
                    {
                        IsEditing = false;
                        ProbeSelector.IsEditing = false;
                    },
                    () => { return true; }
                ));
            }
        }

        private bool _isEnabled = true;

        public bool IsEnabled
        {
            get => _isEnabled; set { if (_isEnabled != value) { _isEnabled = value; OnPropertyChanged(); } }
        }

        #endregion RelayCommands

        #region Methods

        private void UpdateProbes()
        {
            List<ProbeMaterialBase> compatibleProbes = new List<ProbeMaterialBase>();

            // Add the compatible probes
            foreach (var probe in MeasureTools.UpProbes)
            {
                compatibleProbes.Add(probe);
            }

            foreach (var probe in MeasureTools.DownProbes)
            {
                compatibleProbes.Add(probe);
            }

            foreach (var dualProbe in MeasureTools.DualProbes)
            {
                compatibleProbes.Add(dualProbe);
            }

            // Add the new probes
            foreach (var compatibleProbe in compatibleProbes)
            {
                ProbeSelector.AddProbe(compatibleProbe);
            }

            // Remove the probes that are not anymore compatible
            foreach (var probe in ProbeSelector.Probes.ToList())
            {
                if (!compatibleProbes.Any(p => p.ProbeId == probe.ProbeMaterial.ProbeId))
                    ProbeSelector.Probes.Remove(probe);
            }

            if (ProbeSelector.Probes.IsEmpty())
            {
                ProbeSelector.SelectedProbe = null;
            }
            else if (ProbeSelector.SelectedProbe == null || IsSelectedProbeUncompatible(compatibleProbes))
            {
                ProbeSelector.SelectedProbe = ProbeSelector.Probes.FirstOrDefault();
            }
         }

        private bool IsSelectedProbeUncompatible(List<ProbeMaterialBase> compatibleProbes)
        {
            return !compatibleProbes.Any(probe => probe.ProbeId == ProbeSelector.SelectedProbe.ProbeMaterial.ProbeId);
        }

        #endregion Methods

        #region IDisposable
        public void Dispose()
        {
            if (_layers != null)
            {
                _layers.CollectionChanged -= LayersCollectionChanged;
                _layers.ItemPropertyChanged -= LayersItemPropertyChanged;
            }
        }
        #endregion IDisposable
    }
}
