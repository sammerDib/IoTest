using System;

using AutoMapper;

using UnitySC.PM.ANA.Service.Interface.Recipe;
using UnitySC.PM.ANA.Service.Interface.Recipe.Execution;
using UnitySC.PM.ANA.Shared;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.ANA.Service.Core.Shared
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
                        cfg.CreateMap<ANARecipe, DataAccess.Dto.Recipe>()
                            .ForMember(r => r.XmlContent, m => m.MapFrom<XmlContentResolver>())
                            .ForMember(r => r.Type, m => m.MapFrom<RecipeTypeResolver>())
                            .ForMember(dbRecipe => dbRecipe.KeyForAllVersion, recipe => recipe.MapFrom("Key"))
                            .ForMember(dbRecipe => dbRecipe.CreatorUserId, recipe => recipe.MapFrom("UserId"));
                        cfg.CreateMap<DataAccess.Dto.Recipe, ANARecipe>()
                        .ForMember(recipe => recipe.Key, recipe => recipe.MapFrom("KeyForAllVersion"))
                        .ForMember(recipe => recipe.UserId, recipe => recipe.MapFrom("CreatorUserId"))
                        .AfterMap((src, dest) => DeserializeRecipeFromXml(src, dest));
                    });
                    _mapper = configuration.CreateMapper();
                }

                return _mapper;
            }
        }

        private void DeserializeRecipeFromXml(DataAccess.Dto.Recipe recipe, ANARecipe dest)
        {
            // Measurement Strategy was not used before 2024/04/01 and was set to Optimized
            // while the actual strategy was PerMeasurement
            var xmlContent = recipe.XmlContent;

            xmlContent = ANARecipeHelper.ConvertAnaRecipeIfNeeded(xmlContent);
  
            var recipeDeserialized = XML.DeserializeFromString<ANARecipe>(xmlContent);

            dest.Points = recipeDeserialized.Points;
            dest.Measures = recipeDeserialized.Measures;
            dest.Alignment = recipeDeserialized.Alignment;
            dest.IsWaferMapSkipped = recipeDeserialized.IsWaferMapSkipped;
            dest.IsWaferLessModified = recipeDeserialized.IsWaferLessModified;
            dest.WaferMap = recipeDeserialized.WaferMap;
            dest.IsAlignmentMarksSkipped = recipeDeserialized.IsAlignmentMarksSkipped;
            dest.AlignmentMarks = recipeDeserialized.AlignmentMarks;
            dest.Dies = recipeDeserialized.Dies;
            dest.Execution = recipeDeserialized.Execution;
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
