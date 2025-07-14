using AutoMapper;

using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Proxy;

namespace UnitySC.PM.DMT.Service.Interface.Proxy
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
                        cfg.CreateMap<DMTRecipe, DataAccess.Dto.Recipe>()
                            .ForMember(r => r.XmlContent, m => m.MapFrom<XmlContentResolver>())
                            .ForMember(r => r.Type, m => m.MapFrom<RecipeTypeResolver>())
                            .ForMember(dbRecipe => dbRecipe.KeyForAllVersion, recipe => recipe.MapFrom("Key"))
                            .ForMember(dbRecipe => dbRecipe.CreatorUserId, recipe => recipe.MapFrom("UserId"));
                        cfg.CreateMap<DataAccess.Dto.Recipe, DMTRecipe>()
                        .ForMember(recipe => recipe.Key, recipe => recipe.MapFrom("KeyForAllVersion"))
                        .ForMember(recipe => recipe.UserId, recipe => recipe.MapFrom("CreatorUserId"))
                        .AfterMap((src, dest) => DeserializeRecipeFromXml(src.XmlContent, dest));
                        //.ConvertUsing(new XmlRecipeConverter());
                    });
                    _mapper = configuration.CreateMapper();
                }

                return _mapper;
            }
        }

        private void DeserializeRecipeFromXml(string xmlContent, DMTRecipe dest)
        {
            var recipeDeserialized = XML.DeserializeFromString<DMTRecipe>(xmlContent);
            //            recipe.Version = source.Version;
            dest.Measures = recipeDeserialized.Measures;
            dest.IsBSPerspectiveCalibrationUsed = recipeDeserialized.IsBSPerspectiveCalibrationUsed;
            dest.IsFSPerspectiveCalibrationUsed = recipeDeserialized.IsFSPerspectiveCalibrationUsed;
            dest.AreAcquisitionsSavedInDatabase = recipeDeserialized.AreAcquisitionsSavedInDatabase;
        }
    }

    internal class XmlContentResolver : IValueResolver<DMTRecipe, DataAccess.Dto.Recipe, string>
    {
        public string Resolve(DMTRecipe source, DataAccess.Dto.Recipe destination, string destMember, ResolutionContext context)
        {
            string xmlContent = XML.SerializeToString(source);
            return xmlContent;
        }
    }

    internal class RecipeTypeResolver : IValueResolver<DMTRecipe, DataAccess.Dto.Recipe, ActorType>
    {
        public ActorType Resolve(DMTRecipe source, DataAccess.Dto.Recipe destination, ActorType destMember, ResolutionContext context)
        {
            return ActorType.DEMETER;
        }
    }

    internal class ChamberResolver : IValueResolver<DMTRecipe, DataAccess.Dto.Recipe, DataAccess.Dto.Chamber>
    {
        public DataAccess.Dto.Chamber Resolve(DMTRecipe source, DataAccess.Dto.Recipe destination, DataAccess.Dto.Chamber destMember, ResolutionContext context)
        {
            if (source.CreatorChamberId == null)
                return null;
            var dbToolService = ClassLocator.Default.GetInstance<DbToolServiceProxy>();

            var chamber = dbToolService.GetChamber((int)source.CreatorChamberId);
            //destination.CreatorChamberId = chamber.Id;
            return chamber;
        }
    }

    internal class XmlRecipeConverter : ITypeConverter<DataAccess.Dto.Recipe, DMTRecipe>
    {
        public DMTRecipe Convert(DataAccess.Dto.Recipe source, DMTRecipe destination, ResolutionContext context)
        {
            string updatedXmlContent = DMTRecipeHelper.UpdateRecipeXmlIfNeeded(source.XmlContent);
            var recipe = XML.DeserializeFromString<DMTRecipe>(updatedXmlContent);
            //            recipe.Version = source.Version;

            return recipe;
        }
    }
}
