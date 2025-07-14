using AutoMapper;

using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.AGS.Service.Implementation.Proxy
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
                        cfg.CreateMap<ArgosRecipe, DataAccess.Dto.Recipe>()
                            .ForMember(r => r.XmlContent, m => m.MapFrom<XmlContentResolver>())
                            .ForMember(r => r.Type, m => m.MapFrom<RecipeTypeResolver>())
                            .ForMember(dbRecipe => dbRecipe.KeyForAllVersion, recipe => recipe.MapFrom("Key"))
                            .ForMember(dbRecipe => dbRecipe.CreatorUserId, recipe => recipe.MapFrom("UserId"));
                        cfg.CreateMap<DataAccess.Dto.Recipe, ArgosRecipe>()
                            .ForMember(recipe => recipe.Key, recipe => recipe.MapFrom("KeyForAllVersion"))
                            .ForMember(recipe => recipe.UserId, recipe => recipe.MapFrom("CreatorUserId"))
                            .AfterMap((src,dest)=>DeserializeRecipeFromXml(src.XmlContent,dest)) ;
                    });
                    _mapper = configuration.CreateMapper();
                }
                return _mapper;
            }
        }
        
        private void DeserializeRecipeFromXml(string xmlContent, ArgosRecipe dest)
        {
            var recipeDeserialized = XML.DatacontractDeserializeFromString<ArgosRecipe>(xmlContent);
            dest.Frequency_Hz = recipeDeserialized.Frequency_Hz;
            dest.StartAngle_deg = recipeDeserialized.StartAngle_deg;
            dest.ChuckBernouilliEnable = recipeDeserialized.ChuckBernouilliEnable;
            dest.SensorRecipes = recipeDeserialized.SensorRecipes;
        }
    }
    
    internal class XmlContentResolver : IValueResolver<ArgosRecipe, DataAccess.Dto.Recipe, string>
    {
        public string Resolve(ArgosRecipe source, DataAccess.Dto.Recipe destination, string destMember, ResolutionContext context)
        {
            return source.DatacontractSerializeToString();
        }
    }
    
    internal class RecipeTypeResolver : IValueResolver<ArgosRecipe, DataAccess.Dto.Recipe, ActorType>
    {
        public ActorType Resolve(ArgosRecipe source, DataAccess.Dto.Recipe destination, ActorType destMember, ResolutionContext context)
        {
            return source.ActorType;
        }
    }
}
