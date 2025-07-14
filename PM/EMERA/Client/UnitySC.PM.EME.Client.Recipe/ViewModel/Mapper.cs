using AutoMapper;

using UnitySC.PM.EME.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Recipe.ViewModel
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
                        cfg.CreateMap<EMERecipe, EMERecipeVM>().ReverseMap();                       
                        cfg.CreateMap<EMERecipe, DataAccess.Dto.Recipe>()
                            .ForMember(r => r.XmlContent, m => m.MapFrom<XmlContentResolver>())
                            .ForMember(r => r.Type, m => m.MapFrom<RecipeTypeResolver>())
                            .ForMember(dbRecipe => dbRecipe.KeyForAllVersion, recipe => recipe.MapFrom("Key"))
                            .ForMember(dbRecipe => dbRecipe.CreatorUserId, recipe => recipe.MapFrom("UserId"));
                        cfg.CreateMap<DataAccess.Dto.Recipe, EMERecipe>()
                        .ForMember(recipe => recipe.Key, recipe => recipe.MapFrom("KeyForAllVersion"))
                        .ForMember(recipe => recipe.UserId, recipe => recipe.MapFrom("CreatorUserId"));                       
                    });
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }
    }

    public class XmlContentResolver : IValueResolver<EMERecipe, DataAccess.Dto.Recipe, string>
    {
        public string Resolve(EMERecipe source, DataAccess.Dto.Recipe destination, string destMember, ResolutionContext context)
        {
            string xmlContent = XML.SerializeToString(source);
            return xmlContent;
        }
    }

    public class RecipeTypeResolver : IValueResolver<EMERecipe, DataAccess.Dto.Recipe, ActorType>
    {
        public ActorType Resolve(EMERecipe source, DataAccess.Dto.Recipe destination, ActorType destMember, ResolutionContext context)
        {
            return ActorType.EMERA;
        }
    }
}

