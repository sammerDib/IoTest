using AutoMapper;

using DeepLearningSoft48.Models.DefectAnnotations;
using DeepLearningSoft48.ViewModels.DefectAnnotations;

namespace DeepLearningSoft48.Services
{
    public class Mapper
    {
        private IMapper _mapper;

        // Use for auto mapping
        public IMapper AutoMap
        {
            get
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<BoundingBox, BoundingBoxVM>().ReverseMap();
                        cfg.CreateMap<LineAnnotation, LineAnnotationVM>().ReverseMap();
                        cfg.CreateMap<PolygonAnnotation, PolygonAnnotationVM>().ReverseMap();
                        cfg.CreateMap<PolylineAnnotation, PolylineAnnotationVM>().ReverseMap();
                    });
                    _mapper = configuration.CreateMapper();
                }

                return _mapper;
            }
        }
    }
}
