using AutoMapper;

using UnitySC.PM.ANA.Client.Proxy.Camera;
using UnitySC.PM.ANA.Client.Proxy.Light;
using UnitySC.PM.ANA.Client.Proxy.Probe;
using UnitySC.PM.ANA.Client.Proxy.Recipe;
using UnitySC.PM.ANA.Service.Interface;
using UnitySC.PM.ANA.Service.Interface.Context;
using UnitySC.PM.ANA.Service.Interface.Probe.ProbeLise;
using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.Shared.Hardware.Service.Interface.Camera;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Client.Proxy
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
                        cfg.CreateMap<IProbeConfig, ProbeConfigurationLiseVM>().ReverseMap();
                        cfg.CreateMap<SingleLiseInputParams, ProbeInputParametersLiseVM>().ReverseMap();
                        cfg.CreateMap<DualLiseInputParams, ProbeInputParametersLiseDoubleVM>().ReverseMap();
                        cfg.CreateMap<ProbeLiseConfig, ProbeConfigurationLiseVM>().ReverseMap();
                        cfg.CreateMap<ProbeDualLiseConfig, ProbeConfigurationLiseDoubleVM>().ReverseMap();
                        cfg.CreateMap<LiseResult, ProbeResultsLiseVM>().ReverseMap();
                        cfg.CreateMap<Sample, ProbeSample>().ReverseMap();
                        cfg.CreateMap<SampleLayer, ProbeSampleLayer>().ReverseMap();
                        cfg.CreateMap<ANARecipe, ANARecipeVM>().ReverseMap();
                        cfg.CreateMap<CameraInputParams, CameraInputParametersVM>().ReverseMap();
                        cfg.CreateMap<LiseHFInputParams, ProbeInputParametersLiseHFVM>().ReverseMap();
                        cfg.CreateMap<ANARecipe, DataAccess.Dto.Recipe>()
                            .ForMember(r => r.XmlContent, m => m.MapFrom<XmlContentResolver>())
                            .ForMember(r => r.Type, m => m.MapFrom<RecipeTypeResolver>())
                            .ForMember(dbRecipe => dbRecipe.KeyForAllVersion, recipe => recipe.MapFrom("Key"))
                            .ForMember(dbRecipe => dbRecipe.CreatorUserId, recipe => recipe.MapFrom("UserId"));
                        cfg.CreateMap<DataAccess.Dto.Recipe, ANARecipe>()
                        .ForMember(recipe => recipe.Key, recipe => recipe.MapFrom("KeyForAllVersion"))
                        .ForMember(recipe => recipe.UserId, recipe => recipe.MapFrom("CreatorUserId"));

                        cfg.CreateMap<LightVM, LightContext>().ReverseMap();
                    });
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }
    }

    internal class XmlContentResolver : IValueResolver<ANARecipe, DataAccess.Dto.Recipe, string>
    {
        public string Resolve(ANARecipe source, DataAccess.Dto.Recipe destination, string destMember, ResolutionContext context)
        {
            string xmlContent = XML.SerializeToString(source);
            return xmlContent;
        }
    }

    internal class RecipeTypeResolver : IValueResolver<ANARecipe, DataAccess.Dto.Recipe, ActorType>
    {
        public ActorType Resolve(ANARecipe source, DataAccess.Dto.Recipe destination, ActorType destMember, ResolutionContext context)
        {
            return ActorType.ANALYSE;
        }
    }
}
