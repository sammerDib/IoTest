using System;
using System.Windows.Media;

using AutoMapper;

using CommunityToolkit.Mvvm.ComponentModel;

using UnitySC.DataAccess.Dto;
using UnitySC.Shared.Tools.Units;


namespace UnitySC.PM.Shared.UI.Recipes.Management.ViewModel
{
    public class LayerViewModel : ObservableObject, ICloneable
    {
        private Step _step;
        public bool IsClone = false;
        public LayerViewModel(Step step)
        {
            _step = step;
        }

        private bool _isGroupedWithNext;
        public bool IsGroupedWithNext
        {
            get => _isGroupedWithNext; set { if (_isGroupedWithNext != value) { _isGroupedWithNext = value; OnPropertyChanged(); } }
        }

        private bool _inEdition;
        public bool InEdition
        {
            get => _inEdition; set { if (_inEdition != value) { _inEdition = value; OnPropertyChanged(); } }
        }

        private string _name;
        public string Name
        {
            get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(); } }
        }

        private Length _thickness;
        public Length Thickness
        {
            get => _thickness; set { if (_thickness != value) { _thickness = value; OnPropertyChanged(); } }
        }

        private bool _isRefractiveIndexUnknown = false;
        public bool IsRefractiveIndexUnknown
        {
            get => _isRefractiveIndexUnknown;
            set
            {
                if (_isRefractiveIndexUnknown != value)
                {
                    _isRefractiveIndexUnknown = value;
                    if (_isRefractiveIndexUnknown)
                    {
                        RefractiveIndex = float.NaN;
                    }
                    else
                    {
                        RefractiveIndex = 1;
                    }

                    OnPropertyChanged();
                }
            }
        }

        private float? _refractiveIndex;
        public float? RefractiveIndex
        {
            get => _refractiveIndex; set { if (_refractiveIndex != value) { _refractiveIndex = value; OnPropertyChanged(); } }
        }

        private Length _thicknessTolerance = 0.Micrometers();

        public Length ThicknessTolerance
        {
            get => _thicknessTolerance; set { if (_thicknessTolerance != value) { _thicknessTolerance = value; OnPropertyChanged(); } }
        }

        private Material _layerMaterial = null;
        public Material LayerMaterial
        {
            get => _layerMaterial; set { if (_layerMaterial != value) { _layerMaterial = value; OnPropertyChanged(); } }
        }

        private Color _layerColor = Colors.DarkGray;
        public Color LayerColor
        {
            get => _layerColor; set { if (_layerColor != value) { _layerColor = value; OnPropertyChanged(); } }
        }

        public int StepId => _step.Id;

        public object Clone()
        {
            LayerViewModel newClone = (LayerViewModel)MemberwiseClone();
            newClone.IsClone = true;
            return newClone;
        }

        private IMapper _mapper;
        public IMapper AutoMap
        {
            get
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<LayerViewModel, Layer>()
                            .ForMember(l => l.Thickness, m => m.MapFrom(y => (float)y.Thickness.Micrometers))
                            .ForMember(l => l.RefractiveIndex, m
                                => m.MapFrom(la => la.IsRefractiveIndexUnknown ? null : la.RefractiveIndex));

                        cfg.CreateMap<Layer, LayerViewModel>()
                            .ForMember(l => l.Thickness, m => m.MapFrom(y => ((double)y.Thickness).Micrometers()))
                            .ForMember(l => l.RefractiveIndex, m
                                => m.MapFrom(la => la.RefractiveIndex == null ? double.NaN : 1));
                    });
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }

        public class LengthToFloatConverter : ITypeConverter<Length, float>
        {
            public float Convert(Length source, float destination, ResolutionContext context)
            {
                return (float)(source?.Micrometers ?? 0);
            }
        }

        public class FloatToLengthConverter : ITypeConverter<float, Length>
        {

            public Length Convert(float source, Length destination, ResolutionContext context)
            {
                return ((double)source).Micrometers();
            }
        }

        public Layer GetLayerFromLayerVM()
        {
            return AutoMap.Map<Layer>(this);
        }

    }
}
