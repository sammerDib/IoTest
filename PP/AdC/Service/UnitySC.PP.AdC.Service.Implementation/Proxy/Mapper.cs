using AutoMapper;

using UnitySC.PP.ADC.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PP.ADC.Service.Implementation.Proxy
{
    public class Mapper
    {
        private IMapper _mapper;

        public IMapper AutoMap
        {
            get
            {
                if (_mapper == null)
                {
                    var configuration = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<ADCRecipe, DataAccess.Dto.Recipe>()
                            .ForMember(r => r.XmlContent, m => m.MapFrom<XmlContentResolver>())
                            .ForMember(r => r.Type, m => m.MapFrom<RecipeTypeResolver>())
                            .ForMember(dbRecipe => dbRecipe.KeyForAllVersion, recipe => recipe.MapFrom("Key"))
                            .ForMember(dbRecipe => dbRecipe.CreatorUserId, recipe => recipe.MapFrom("UserId"));
                        cfg.CreateMap<DataAccess.Dto.Recipe, ADCRecipe>()
                        .ForMember(recipe => recipe.Key, recipe => recipe.MapFrom("KeyForAllVersion"))
                        .ForMember(recipe => recipe.UserId, recipe => recipe.MapFrom("CreatorUserId"))
                        .AfterMap((src, dest) => DeserializeRecipeFromXml(src.XmlContent, dest));
                    });
                    _mapper = configuration.CreateMapper();
                }

                return _mapper;
            }
        }

        private void DeserializeRecipeFromXml(string xmlContent, ADCRecipe dest)
        {
            var recipeDeserialized = XML.DeserializeFromString<ADCRecipe>(xmlContent);

            dest.ADCEngineRecipeXml = recipeDeserialized.ADCEngineRecipeXml;
        }
    }

    internal class XmlContentResolver : IValueResolver<ADCRecipe, DataAccess.Dto.Recipe, string>
    {
        public string Resolve(ADCRecipe source, DataAccess.Dto.Recipe destination, string destMember, ResolutionContext context)
        {
            string xmlContent = XML.SerializeToString(source);
            return xmlContent;
        }
    }

    internal class RecipeTypeResolver : IValueResolver<ADCRecipe, DataAccess.Dto.Recipe, ActorType>
    {
        public ActorType Resolve(ADCRecipe source, DataAccess.Dto.Recipe destination, ActorType destMember, ResolutionContext context)
        {
            return ActorType.ADC;
        }
    }
}

