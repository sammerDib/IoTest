
namespace UnitySC.DataAccess.SQL
{
    using System;
    using System.Collections.Generic;
    
    using UnitySC.DataAccess.Base;
    using UnitySC.DataAccess.Base.Implementation;
    
    public partial class UnitOfWorkUnity : UnitOfWorkBase
    {
    
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Chamber> ChamberRepository { get => GetRepository<UnitySC.DataAccess.SQL.Chamber>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ConfigurationData> ConfigurationDataRepository { get => GetRepository<UnitySC.DataAccess.SQL.ConfigurationData>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ConfigurationHistory> ConfigurationHistoryRepository { get => GetRepository<UnitySC.DataAccess.SQL.ConfigurationHistory>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.DatabaseVersion> DatabaseVersionRepository { get => GetRepository<UnitySC.DataAccess.SQL.DatabaseVersion>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Dataflow> DataflowRepository { get => GetRepository<UnitySC.DataAccess.SQL.Dataflow>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.GlobalResultSettings> GlobalResultSettingsRepository { get => GetRepository<UnitySC.DataAccess.SQL.GlobalResultSettings>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Input> InputRepository { get => GetRepository<UnitySC.DataAccess.SQL.Input>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Job> JobRepository { get => GetRepository<UnitySC.DataAccess.SQL.Job>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.KlarfBinSettings> KlarfBinSettingsRepository { get => GetRepository<UnitySC.DataAccess.SQL.KlarfBinSettings>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.KlarfRoughSettings> KlarfRoughSettingsRepository { get => GetRepository<UnitySC.DataAccess.SQL.KlarfRoughSettings>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Layer> LayerRepository { get => GetRepository<UnitySC.DataAccess.SQL.Layer>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Log> LogRepository { get => GetRepository<UnitySC.DataAccess.SQL.Log>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Material> MaterialRepository { get => GetRepository<UnitySC.DataAccess.SQL.Material>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Output> OutputRepository { get => GetRepository<UnitySC.DataAccess.SQL.Output>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Product> ProductRepository { get => GetRepository<UnitySC.DataAccess.SQL.Product>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ProductConfiguration> ProductConfigurationRepository { get => GetRepository<UnitySC.DataAccess.SQL.ProductConfiguration>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Recipe> RecipeRepository { get => GetRepository<UnitySC.DataAccess.SQL.Recipe>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.RecipeDataflowMap> RecipeDataflowMapRepository { get => GetRepository<UnitySC.DataAccess.SQL.RecipeDataflowMap>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.RecipeFile> RecipeFileRepository { get => GetRepository<UnitySC.DataAccess.SQL.RecipeFile>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.RecipeResultType> RecipeResultTypeRepository { get => GetRepository<UnitySC.DataAccess.SQL.RecipeResultType>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Result> ResultRepository { get => GetRepository<UnitySC.DataAccess.SQL.Result>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ResultAcq> ResultAcqRepository { get => GetRepository<UnitySC.DataAccess.SQL.ResultAcq>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ResultAcqItem> ResultAcqItemRepository { get => GetRepository<UnitySC.DataAccess.SQL.ResultAcqItem>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ResultItem> ResultItemRepository { get => GetRepository<UnitySC.DataAccess.SQL.ResultItem>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.ResultItemValue> ResultItemValueRepository { get => GetRepository<UnitySC.DataAccess.SQL.ResultItemValue>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Step> StepRepository { get => GetRepository<UnitySC.DataAccess.SQL.Step>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Tag> TagRepository { get => GetRepository<UnitySC.DataAccess.SQL.Tag>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Tool> ToolRepository { get => GetRepository<UnitySC.DataAccess.SQL.Tool>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.User> UserRepository { get => GetRepository<UnitySC.DataAccess.SQL.User>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.Vid> VidRepository { get => GetRepository<UnitySC.DataAccess.SQL.Vid>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.WaferCategory> WaferCategoryRepository { get => GetRepository<UnitySC.DataAccess.SQL.WaferCategory>(); }
    		public IRepositoryBase<UnitySC.DataAccess.SQL.WaferResult> WaferResultRepository { get => GetRepository<UnitySC.DataAccess.SQL.WaferResult>(); }
    } 
}

