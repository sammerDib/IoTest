using AutoMapper;

using LocalDto = UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using LocalSQL = UnitySC.DataAccess.SQL.ModelSQL.LocalSQL;

namespace UnitySC.DataAccess.Service.Implementation
{
    public class SQLMapper
    {
        public static IMapper GetMapping()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SQL.Chamber, Dto.Chamber>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.ConfigurationData, Dto.ConfigurationData>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.ConfigurationHistory, Dto.ConfigurationHistory>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.ProductConfiguration, Dto.ProductConfiguration>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.DatabaseVersion, Dto.DatabaseVersion>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Recipe, Dto.Recipe>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.RecipeFile, Dto.RecipeFile>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Tag, Dto.Tag>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Tool, Dto.Tool>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.User, Dto.User>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.Vid, Dto.Vid>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Product, Dto.Product>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.Log, Dto.Log>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Job, Dto.Job>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.WaferResult, Dto.WaferResult>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.Result, Dto.Result>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.ResultAcq, Dto.ResultAcq>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.ResultAcqItem, Dto.ResultAcqItem>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.ResultItem, Dto.ResultItem>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.ResultItemValue, Dto.ResultItemValue>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<LocalSQL.SearchParam, LocalDto.SearchParam>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Recipe, LocalDto.DataflowRecipeComponent>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.Step, Dto.Step>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.Input, Dto.Input>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Output, Dto.Output>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.WaferCategory, Dto.WaferCategory>().ReverseMap().MaxDepth(2);
                cfg.CreateMap<SQL.Layer, Dto.Layer>().ReverseMap().MaxDepth(1);
                cfg.CreateMap<SQL.Material, Dto.Material>().ReverseMap().MaxDepth(2);
            });
            return configuration.CreateMapper();
        }
    }
}
