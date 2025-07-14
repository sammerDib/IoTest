using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.ServiceModel;
using System.Text.RegularExpressions;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

using Recipe = UnitySC.DataAccess.Dto.Recipe;

namespace UnitySC.DataAccess.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class RecipeService : DataAccesServiceBase, IDbRecipeService
    {
        public enum TemplateKeyword { RecipeKey, FileNameWithoutExtension, Version, FileName, FileExtension }

        public RecipeService(ILogger logger) : base(logger)
        {
            if (!CheckIfTemplateExternalFilePathIsValid())
                throw new Exception("Invalid TemplateExternalFilePath in configuration");
        }

        public Response<Dto.Chamber> GetChamberFromKeys(int toolKey, int chamberKey)
        {
            _logger.Debug("Get recipe.chamberfromKeys");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var chamber = unitOfWork.ChamberRepository.CreateQuery(false, x => x.Tool).SingleOrDefault(x => (x.ChamberKey == chamberKey) && (x.Tool.ToolKey == toolKey));
                    return Mapper.Map<Dto.Chamber>(chamber);
                }
            });
        }

        [Obsolete]
        public Response<List<RecipeInfo>> GetRecipeList_Obsolete(ActorType actorType, int stepId, int chamberId, bool takeArchivedRecipes = false, bool takeTemplateRecipes = false)
        {
            _logger.Debug($"GetRecipeList (OBSOLETE) {actorType} ");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return unitOfWork.RecipeRepository.CreateQuery()
                        .Where(x => x.ActorType == (int)actorType
                                && (takeArchivedRecipes || !x.IsArchived)
                                && x.StepId == stepId
                                && (takeTemplateRecipes || !x.IsTemplate)
                                && (x.CreatorChamberId == chamberId || x.IsShared))
                        .GroupBy(x => x.KeyForAllVersion)
                        .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault())
                        .Select(x => new RecipeInfo() { Name = x.Name, Version = x.Version, Comment = x.Comment, StepId = x.StepId, IsShared = x.IsShared, IsTemplate = x.IsTemplate, ActorType = (ActorType)x.ActorType, Key = x.KeyForAllVersion })
                        .ToList();
                }
            });
        }
        //to do : exludenotvalidated
        public Response<List<RecipeInfo>> GetRecipeList(ActorType actorType, int stepId, int chamberKey, int toolKey, bool takeArchivedRecipes = false, bool takeTemplateRecipes = false)
        {
            _logger.Debug($"GetRecipeList {actorType} from Ch{chamberKey}_T{toolKey}");

            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int chamberId = GetChamberIdFromKeys(chamberKey, toolKey, unitOfWork);
                    return unitOfWork.RecipeRepository.CreateQuery()
                        .Where(x => x.ActorType == (int)actorType
                                && (takeArchivedRecipes || !x.IsArchived)
                                && x.StepId == stepId
                                && (takeTemplateRecipes || !x.IsTemplate)
                                && (x.CreatorChamberId == chamberId || x.IsShared))
                        .GroupBy(x => x.KeyForAllVersion)
                        .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault())
                        .Select(x => new RecipeInfo() { Name = x.Name, Version = x.Version, Comment = x.Comment, StepId = x.StepId, IsShared = x.IsShared, IsTemplate = x.IsTemplate, ActorType = (ActorType)x.ActorType, Key = x.KeyForAllVersion })
                        .ToList();
                }
            });
        }

        public Response<List<Recipe>> GetADCRecipes(string recipeName = null, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            return InvokeDataResponse(() =>
            {

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    List<SQL.Recipe> sqlrecipes;

                    if (includeRecipeFileInfos)
                    {
                        sqlrecipes = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Step.Product, x => x.RecipeFiles)
                                    .Where(x => x.ActorType == (int)ActorType.ADC
                                            && (takeArchivedRecipes || !x.IsArchived))
                                    .GroupBy(x => x.KeyForAllVersion)
                                    .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault())
                                    .ToList();
                    }
                    else
                    {
                        sqlrecipes = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Step.Product)
                                .Where(x => x.ActorType == (int)ActorType.ADC
                                        && (takeArchivedRecipes || !x.IsArchived))
                                .GroupBy(x => x.KeyForAllVersion)
                                .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault())
                                .ToList();
                    }

                    if (!String.IsNullOrEmpty(recipeName))
                        sqlrecipes = sqlrecipes.Where(x => x.Name == recipeName).ToList(); ;


                    var dtoRecipes = new List<Dto.Recipe>();
                    foreach (var recipeSql in sqlrecipes)
                        dtoRecipes.Add(Mapper.Map<Dto.Recipe>(recipeSql));

                    return dtoRecipes;
                }
            });
        }

        public Response<int> GetRecipesCount(bool takeArchivedRecipes = false, bool countPMRecipes = true, bool countDataflowRecipes = true )
        {
            return InvokeDataResponse(() =>
            {
                int count = 0;
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (countPMRecipes)
                    {
                        count += unitOfWork.RecipeRepository.CreateQuery()
                            .Where(x => (takeArchivedRecipes || !x.IsArchived))
                            .GroupBy(x => x.KeyForAllVersion)
                            .Select(g => g.Max(x => x.Version))
                            .Count();
                    }

                    if (countDataflowRecipes)
                    {
                        count += unitOfWork.DataflowRepository.CreateQuery()
                        .Where(x => (takeArchivedRecipes || !x.IsArchived))
                        .GroupBy(x => x.KeyForAllVersion)
                        .Select(g => g.Max(x => x.Version))
                        .Count();
                    }
                }
                return count;
            });
        }

        public Response<int> GetResultsCount()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return unitOfWork.ResultRepository.Count();
                }
            });
        }

        public Response<int> GetAcquisitionsCount()
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return unitOfWork.ResultAcqRepository.Count();
                }
            });
        }

        public Response<int> GetProductsCount(bool takeArchivedRecipes = false)
        {
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return unitOfWork.ProductRepository.CreateQuery().Where(x => (takeArchivedRecipes || !x.IsArchived)).Count();
                }
            });
        }

        public Response<List<TCPMRecipe>> GetTCPMRecipeList(ActorType actorType, int toolKey)
        {
            _logger.Debug($"GetTCPMRecipeList Actor: {actorType}  ToolKey: {toolKey}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var toolId = GetToolIdFromToolKey(toolKey, unitOfWork);
                    var lastDataFlows = Buisness.GetLastDataflowsForTC(unitOfWork, toolId);
                    var dataflowKeys = lastDataFlows.Select(x => x.KeyForAllVersion);

                    var lastRecipes = Buisness.GetLastRecipesForTC(unitOfWork, actorType);
                    var recipeKeys = lastRecipes.Select(x => x.KeyForAllVersion);

                    var res = new List<TCPMRecipe>();
                    var recipeDataFlowMap = unitOfWork.RecipeDataflowMapRepository.CreateQuery().Where(x => dataflowKeys.Contains(x.DataflowKey));

                    foreach (var dataflow in lastDataFlows)
                    {
                        foreach (var recipeMap in recipeDataFlowMap.Where(x => x.DataflowKey == dataflow.KeyForAllVersion && recipeKeys.Contains(x.RecipeKey)))
                        {
                            var recipe = lastRecipes.SingleOrDefault(x => x.KeyForAllVersion == recipeMap.RecipeKey);
                            if (recipe != null)
                            {
                                string pmRecipeName = DataAccessConfiguration.Instance.TemplateTCPMRecipeName;
                                pmRecipeName = pmRecipeName.Replace("{Product}", recipe.Step.Product.Name);
                                pmRecipeName = pmRecipeName = pmRecipeName.Replace("{Step}", recipe.Step.Name);
                                pmRecipeName = pmRecipeName.Replace("{Dataflow}", dataflow.Name);
                                pmRecipeName = pmRecipeName.Replace("{Recipe}", recipe.Name);
                                res.Add(new TCPMRecipe()
                                {
                                    Author = recipe.User != null ? recipe.User.Name : string.Empty,
                                    Name = pmRecipeName,
                                    Description = recipe.Comment
                                });
                            }
                        }
                    }

                    return res;
                }
            });
        }

        public Response<Recipe> GetPMRecipeWithTC(string tcRecipeName)
        {
            _logger.Debug($"GetRecipeWithTC recipe name: {tcRecipeName}");

            return InvokeDataResponse(() =>
            {

                var tcRecipeInfo = GetTCRecipeInfo(tcRecipeName);

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var product = unitOfWork.ProductRepository.CreateQuery(false, x => x.Steps.Select(y => y.Recipes)).FirstOrDefault(x => x.Name == tcRecipeInfo.ProductName && !x.IsArchived);
                    if (product == null)
                        throw new Exception("Bad product name in TC Recipe name");
                    var step = product.Steps.FirstOrDefault(x => x.Name == tcRecipeInfo.StepName && !x.IsArchived);
                    if (step == null)
                        throw new Exception("Bad step name in TC Recipe name");
                    var recipe = step.Recipes.FirstOrDefault(x => x.Name == tcRecipeInfo.PMRecipeName && !x.IsArchived);
                    if (recipe == null)
                        throw new Exception("Bad recipe name in TC Recipe name");
                    int lastVersion = Buisness.GetLastRecipeVersionByKey(unitOfWork, recipe.KeyForAllVersion);

                    return Mapper.Map<Dto.Recipe>(unitOfWork.RecipeRepository.CreateQuery().First(x => x.KeyForAllVersion == recipe.KeyForAllVersion && x.Version == lastVersion));
                }
            });
        }

        public Response<DataflowRecipeComponent> GetDataflowWithTC(string tcRecipeName)
        {
            _logger.Debug($"GetRecipeWithTC  recipe name: {tcRecipeName}");

            return InvokeDataResponse(() =>
            {
                var tcRecipeInfo = GetTCRecipeInfo(tcRecipeName);

                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var product = unitOfWork.ProductRepository.CreateQuery(false, x => x.Steps.Select(y => y.Dataflows)).FirstOrDefault(x => x.Name == tcRecipeInfo.ProductName && !x.IsArchived);
                    if (product == null)
                        throw new Exception("Bad product name in TC Recipe name");
                    var step = product.Steps.FirstOrDefault(x => x.Name == tcRecipeInfo.StepName && !x.IsArchived);
                    if (step == null)
                        throw new Exception("Bad step name in TC Recipe name");
                    var dataflow = step.Dataflows.FirstOrDefault(x => x.Name == tcRecipeInfo.DataflowName && !x.IsArchived);
                    if (dataflow == null)
                        throw new Exception("Bad dataflows name in TC Recipe name");

                    return Buisness.GetLastDataflowComponent(unitOfWork, tcRecipeInfo.DataflowName, step.Id);
                }
            });
        }


        private TCRecipeInfo GetTCRecipeInfo(string tcPMRecipeName)
        {
            string templateTCPMRecipeName = DataAccessConfiguration.Instance.TemplateTCPMRecipeName;
            int firstSpliterIndex = templateTCPMRecipeName.IndexOf('}') + 1;
            char spliter = templateTCPMRecipeName[firstSpliterIndex];
            string[] templates = templateTCPMRecipeName.Split(spliter);
            string[] recipeValues = tcPMRecipeName.Split(spliter);
            if (templates.Count() != recipeValues.Count())
                throw new InvalidOperationException("The name of the recipe does not match the template");
            var dicTemplateValue = new Dictionary<string, string>();
            for (int i = 0; i < templates.Count(); i++)
            {
                dicTemplateValue.Add(templates[i], recipeValues[i]);
            }

            var tcRecipeInfo = new TCRecipeInfo();

            tcRecipeInfo.ProductName = dicTemplateValue["{Product}"];
            tcRecipeInfo.StepName = dicTemplateValue["{Step}"];
            tcRecipeInfo.DataflowName = dicTemplateValue["{Dataflow}"];
            tcRecipeInfo.PMRecipeName = dicTemplateValue["{Recipe}"];
            return tcRecipeInfo;
        }


        public Response<RecipeInfo> GetRecipeInfo(int id)
        {
            _logger.Debug($"GetRecipeInfo {id}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var recipe = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).Single(x => x.Id == id);
                    if (recipe == null)
                        throw new InvalidOperationException("Unknow recipe Id");
                    var recipeInfo = new RecipeInfo() { Name = recipe.Name, Version = recipe.Version, Comment = recipe.Comment, StepId = recipe.StepId, IsShared = recipe.IsShared, IsTemplate = recipe.IsTemplate, ActorType = (ActorType)recipe.ActorType, Key = recipe.KeyForAllVersion };
                    return recipeInfo;
                }
            });
        }
        //to do : exludenotvalidated
        public Response<Recipe> GetRecipe(ActorType actorType, int stepId, string recipeName, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            _logger.Debug($"GetRecipe {actorType} {recipeName}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int lastVersion = Buisness.GetLastRecipeVersionByName(unitOfWork, actorType, stepId, recipeName, takeArchivedRecipes);
                    var recipe = Mapper.Map<Dto.Recipe>(unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).SingleOrDefault(x => x.Name == recipeName && x.Version == lastVersion && x.StepId == stepId && (takeArchivedRecipes || !x.IsArchived)));
                    if (includeRecipeFileInfos && recipe != null)
                        recipe.RecipeFiles = Buisness.GetRecipeFilesInfo(unitOfWork, recipe.Id);

                    return recipe;
                }
            });
        }

        public Response<Recipe> GetRecipe(int id, bool includeRecipeFileInfos = false)
        {
            _logger.Debug($"GetRecipe {id}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var recipe = Mapper.Map<Dto.Recipe>(unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).SingleOrDefault(x => x.Id == id));
                    if (includeRecipeFileInfos && recipe != null)
                        recipe.RecipeFiles = Buisness.GetRecipeFilesInfo(unitOfWork, recipe.Id);

                    return recipe;
                }
            });
        }

        //to do : exludenotvalidated
        public Response<Recipe> GetLastRecipe(Guid key, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            _logger.Debug($"GetRecipe last recipe {key}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int lastVersion = Buisness.GetLastRecipeVersionByKey(unitOfWork, key, takeArchivedRecipes);
                    var recipe = Mapper.Map<Dto.Recipe>(unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).SingleOrDefault(x => x.Version == lastVersion && x.KeyForAllVersion == key));
                    if (includeRecipeFileInfos && recipe != null)
                        recipe.RecipeFiles = Buisness.GetRecipeFilesInfo(unitOfWork, recipe.Id);

                    return recipe;
                }
            });
        }

        public Response<Recipe> GetLastRecipeWithProductAndStep(Guid key)
        {
            _logger.Debug($"Get last recipe {key} with product and step");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int lastVersion = Buisness.GetLastRecipeVersionByKey(unitOfWork, key, false);
                    var recipe = Mapper.Map<Dto.Recipe>(unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs, x => x.Step.Product.WaferCategory).SingleOrDefault(x => x.Version == lastVersion && x.KeyForAllVersion == key));
                    recipe.Step.Product.WaferCategory.DimentionalCharacteristic = (WaferDimensionalCharacteristic)Buisness.Deserialize<WaferDimensionalCharacteristic>(recipe.Step.Product.WaferCategory.XmlContent);
                    return recipe;
                }
            });
        }

        public Response<Recipe> GetLastRecipe(ActorType actorType, int stepId, string name, bool includeRecipeFileInfos = false, bool takeArchivedRecipes = false)
        {
            _logger.Debug($"Get last Recipe {actorType} {name}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int lastVersion = Buisness.GetLastRecipeVersionByName(unitOfWork, actorType, stepId, name, takeArchivedRecipes);
                    var recipe = Mapper.Map<Dto.Recipe>(unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).SingleOrDefault(x => x.Version == lastVersion && x.Name == name && x.StepId == stepId && x.ActorType == (int)actorType));
                    if (includeRecipeFileInfos && recipe != null)
                        recipe.RecipeFiles = Buisness.GetRecipeFilesInfo(unitOfWork, recipe.Id);

                    return recipe;
                }
            });
        }

        public Response<int> GetRecipeVersion(ActorType actorType, int stepId, string recipeName, bool takeArchivedRecipes = false)
        {
            _logger.Debug($"GetRecipeVersion {actorType} {recipeName}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    return Buisness.GetLastRecipeVersionByName(unitOfWork, actorType, stepId, recipeName, takeArchivedRecipes);
            });
        }

        public Response<List<bool>> GetArchivedStatus(ActorType actorType, int recipeId)
        {
            _logger.Debug($"GetRecipeVersion {recipeId}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                    return unitOfWork.RecipeRepository.CreateQuery()
                        .Where(x => x.Id == recipeId && x.ActorType == (int)actorType)
                        .OrderBy(x => x.Version)
                        .Select(x => x.IsArchived).ToList();
            });
        }

        public Response<VoidResult> ChangeRecipeSharedState(Guid key, int userId, bool isShared)
        {
            _logger.Debug($"ChangeRecipeSharedState for recipe {key} by userId {userId} is shared {isShared} ");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var recipes = unitOfWork.RecipeRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
                    foreach (var recipe in recipes)
                    {
                        recipe.IsShared = isShared;
                    }

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Edit,
                       Dto.Log.TableTypeEnum.Recipe,
                       $"Change shared state for recipe {key} is shared {isShared}", _logger);

                    unitOfWork.Save();
                }
            });
        }


        public Response<VoidResult> ChangeRecipeValidateState(Guid key, int userId, bool isValidated)
        {
            _logger.Debug($"ChangeRecipeValidateState for recipe {key} by userId {userId} is Validated {isValidated} ");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var recipes = unitOfWork.RecipeRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
                    if (recipes != null && recipes.Count != 0)
                    {

                        foreach (var recipe in recipes)
                        {
                            recipe.IsValidated = isValidated;
                        }

                        DataAccessHelper.LogInDatabase(unitOfWork,
                           userId,
                           Dto.Log.ActionTypeEnum.Edit,
                           Dto.Log.TableTypeEnum.Recipe,
                           $"Change shared state for recipe <{recipes[0].Name}> [{key}] is validated {isValidated}", _logger);

                        unitOfWork.Save();
                    }
                }
            });
        }

        public Response<VoidResult> ArchiveAllVersionOfRecipe(Guid key, int userId)
        {
            _logger.Debug($"ArchiveAllVersion for recipe {key} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    DataAccessHelper.ArchiveAllVerionsOfRecipe(key, userId, unitOfWork, _logger);

                    unitOfWork.Save();
                }
            });
        }

  

        public Response<VoidResult> RestoreAllVersionOfRecipe(Guid key, int userId)
        {
            _logger.Debug($"RestoreAllVersion for recipe {key} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var recipes = unitOfWork.RecipeRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
                    foreach (var recipe in recipes)
                    {
                        recipe.IsArchived = false;
                    }

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Restore,
                       Dto.Log.TableTypeEnum.Recipe,
                       $"RestoreAllVersion for recipe {key}", _logger);

                    unitOfWork.Save();
                }
            });
        }

  
        public Response<int> SetDataflow(DataflowRecipeComponent dataflowRecipe, int userId, int stepId, int toolKey, bool incrementVersion = true)
        {
            _logger.Debug($"SetDataflow {dataflowRecipe}");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (!Buisness.DataflowIsValid(unitOfWork, dataflowRecipe, stepId))
                        throw new InvalidOperationException("Dataflow " + dataflowRecipe.Name + " is not valid ");

                    var toolId = GetToolIdFromToolKey(toolKey, unitOfWork);

                    var sqlDataflow = new SQL.Dataflow();
                    if (dataflowRecipe.Key == Guid.Empty) // New Dataflow
                    {
                        sqlDataflow.Version = 1;
                        sqlDataflow.KeyForAllVersion = Guid.NewGuid();
                        dataflowRecipe.Key = sqlDataflow.KeyForAllVersion;
                        _logger.Information($"Add dataflow {dataflowRecipe.Name} step Id {stepId}");
                    }
                    else if (incrementVersion) // New version
                    {
                        if (dataflowRecipe.Key == null)
                            throw new InvalidDataException("KeyForAllVersion is missing" + dataflowRecipe.Name);
                        sqlDataflow.Version = Buisness.GetLastDataflowVersionByKey(unitOfWork, dataflowRecipe.Key, false) + 1;
                        sqlDataflow.Id = 0;
                        _logger.Information($"New version for dataflow {dataflowRecipe.Name} step Id {stepId} version {sqlDataflow.Version}");
                    }
                    else // Update curent version
                    {
                        if (dataflowRecipe.Key == null)
                            throw new InvalidDataException("KeyForAllVersion is missing" + dataflowRecipe.Name);

                        int lastVersion = Buisness.GetLastDataflowVersionByKey(unitOfWork, dataflowRecipe.Key);
                        sqlDataflow = unitOfWork.DataflowRepository.CreateQuery(true).Single(x => x.Version == lastVersion && x.KeyForAllVersion == dataflowRecipe.Key);
                        _logger.Information($"Update dataflow {dataflowRecipe.Name} without new version step Id {stepId} version {sqlDataflow.Version}");
                    }

                    dataflowRecipe.Version = sqlDataflow.Version;
                    sqlDataflow.KeyForAllVersion = dataflowRecipe.Key;
                    sqlDataflow.Name = dataflowRecipe.Name;
                    sqlDataflow.Created = DateTime.Now;
                    sqlDataflow.IsArchived = false;
                    sqlDataflow.IsShared = false;//dataflowRecipe.IsShared;
                    sqlDataflow.IsValidated = false; //dataflowRecipe.IsValidated;
                    sqlDataflow.CreatorUserId = userId;
                    sqlDataflow.StepId = stepId;
                    sqlDataflow.XmlContent = Buisness.Serialize(dataflowRecipe);
                    sqlDataflow.Comment = dataflowRecipe.Comment;
                    sqlDataflow.CreatorTool = toolId;

                    if (sqlDataflow.Id == 0)
                    {
                        unitOfWork.DataflowRepository.Add(sqlDataflow);

                        DataAccessHelper.LogInDatabase(unitOfWork,
                            userId,
                            Dto.Log.ActionTypeEnum.Add,
                            Dto.Log.TableTypeEnum.Dataflow,
                            $"Import recipe {sqlDataflow.Name} v{sqlDataflow.Version} for step {sqlDataflow.StepId}", _logger);
                    }
                    else
                    {
                        unitOfWork.DataflowRepository.Update(sqlDataflow);

                        DataAccessHelper.LogInDatabase(unitOfWork,
                            userId,
                            Dto.Log.ActionTypeEnum.Edit,
                            Dto.Log.TableTypeEnum.Dataflow,
                            $"Import recipe {sqlDataflow.Name} v{sqlDataflow.Version} for step {sqlDataflow.StepId}",_logger);
                    }

                    // Update Dataflow recipe map

                    // Remove old
                    var toRemove = unitOfWork.RecipeDataflowMapRepository.CreateQuery().Where(x => x.DataflowKey == sqlDataflow.KeyForAllVersion).ToList();
                    foreach (var map in toRemove)
                    {
                        unitOfWork.RecipeDataflowMapRepository.Remove(map);
                    }

                    // Get linked recipe
                    var linkedRecipeGuids = Buisness.GetLinkedRecipe(new List<Guid>(), dataflowRecipe);

                    // Add  mapping
                    foreach (var linkedRecipeGuid in linkedRecipeGuids.Distinct())
                    {
                        unitOfWork.RecipeDataflowMapRepository.Add(new SQL.RecipeDataflowMap() { DataflowKey = sqlDataflow.KeyForAllVersion, RecipeKey = linkedRecipeGuid });
                    }

                    unitOfWork.Save();

                    return sqlDataflow.Id;
                }
            });
        }

        public Response<DataflowRecipeComponent> GetDataflow(int dataflowId)
        {
            _logger.Debug("Get dataflow");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return Buisness.GetDataflowComponent(unitOfWork, dataflowId);
                }
            });
        }

        public Response<DataflowRecipeComponent> GetLastDataflow(string name, int stepId)
        {
            _logger.Debug("Get last dataflow");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return Buisness.GetLastDataflowComponent(unitOfWork, name, stepId);
                }
            });
        }

        public Response<DataflowRecipeComponent> GetLastDataflow(Guid key, bool takeArchivedDataflow)
        {
            _logger.Debug("Get last dataflow bu key");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int lastVersion = Buisness.GetLastDataflowVersionByKey(unitOfWork, key);
                    int dataflowId = (unitOfWork.DataflowRepository.CreateQuery().Single(x => x.Version == lastVersion && x.KeyForAllVersion == key && (takeArchivedDataflow || !x.IsArchived))).Id;
                    return Buisness.GetDataflowComponent(unitOfWork, dataflowId);
                }
            });
        }

        Response<List<DataflowInfo>> IDbRecipeService.GetDataflowInfos(int stepId, int toolKey, bool takeArchivedDataflow)
        {
            _logger.Debug("Get dataflows");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var toolId = GetToolIdFromToolKey(toolKey, unitOfWork);
                    return unitOfWork.DataflowRepository.CreateQuery(false)
                    .Where(sql => sql.StepId == stepId
                    && (takeArchivedDataflow || !sql.IsArchived)
                    && (sql.CreatorTool == toolId || sql.IsShared))
                    .GroupBy(x => x.KeyForAllVersion)
                    .Select(grp => new
                    {
                        grp.Key,
                        Dataflow = grp
                         .OrderByDescending(x => x.Version)
                         .FirstOrDefault()
                    }).Select(x => new DataflowInfo() { Id = x.Dataflow.Id, Comment = x.Dataflow.Comment, Name = x.Dataflow.Name, StepId = x.Dataflow.StepId }).ToList();
                }
            });
        }
        Response<List<DataflowRecipeInfo>> IDbRecipeService.GetAllDataflow(List<ActorType> actorsRef, int toolKey, bool takeArchivedDataflow)
        {
            _logger.Debug("Get GetAllDataflow");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var toolId = GetToolIdFromToolKey(toolKey, unitOfWork);

                    List<DataflowRecipeInfo> result = new List<DataflowRecipeInfo>();
                    var dataflowCollection = Buisness.GetAllDataflow(unitOfWork, toolId, takeArchivedDataflow);
                    foreach (var dataflow in dataflowCollection)
                    {
                        try
                        {
                            DataflowRecipeComponent dataflowRecipeComponent = Buisness.GetDataflowComponent(unitOfWork, dataflow.Id);
                            var actorsList = Buisness.GetActorTypes(dataflowRecipeComponent);
                            //compatibility between the TC and the dataflow recipe.
                            if (actorsList.All(item => actorsRef.Contains(item)))
                            {
                                result.Add(dataflow);
                            }
                        }
                        catch (Exception ex) 
                        {
                            _logger.Debug($"Dataflow recipe loading failed : {dataflow.ProductName}/{dataflow.StepName}/{dataflow.Name} [{dataflow.IdGuid}]");
                        };
                    }
                    return result;
                }
            });
        }

        // Case is ignored
        public Response<bool> IsDataflowNameAlreadyUsedByOtherRecipe(string dataflowName, Guid key, bool takeArchivedDataflow = false)
        {
            _logger.Debug("Get GetAllDataflow");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    bool hasRecipe=false;
                    var recipesWithSameName = unitOfWork.DataflowRepository.CreateQuery(false)
                            .Where(x => x.Name.ToLower()==dataflowName.ToLower()
                                        && x.KeyForAllVersion != key
                                        && (takeArchivedDataflow || !x.IsArchived));

                    foreach (var recipe in recipesWithSameName)
                    {
                        if (Buisness.GetLastDataflowVersionByKey(unitOfWork, recipe.KeyForAllVersion) == recipe.Version)
                        {
                            hasRecipe = true;
                            break;
                        }
                    }
                    return hasRecipe;
                }
            });
        }

        Response<VoidResult> IDbRecipeService.UpdateArchivedRecipes(Dictionary<int, bool> recipeIdArchiveState, int userId)
        {
            _logger.Debug($"UpdateArchivedRecipes");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var recipesIdToUpdate = recipeIdArchiveState.Select(x => x.Key).ToList();
                    var recipesToUpdate = unitOfWork.RecipeRepository.CreateQuery(true).Where(sql => recipesIdToUpdate.Contains(sql.Id)).ToList();

                    foreach (var recipe in recipesToUpdate)
                    {
                        recipe.IsArchived = recipeIdArchiveState[recipe.Id];
                        DataAccessHelper.LogInDatabase(unitOfWork, userId, Dto.Log.ActionTypeEnum.Edit, Dto.Log.TableTypeEnum.Recipe, $"Update recipe {recipe.Name} v{recipe.Version} is archived {recipe.IsArchived}", _logger);
                    }

                    unitOfWork.Save();
                }
            });
        }

        public Response<int> SetRecipe(Recipe recipe, bool incrementVersion)
        {
            _logger.Debug($"Recipe {(ActorType)recipe.ActorType} {recipe.Name} for step {recipe.StepId} is Shared {recipe.IsShared} is Template {recipe.IsTemplate} is Archived {recipe.IsArchived}");
            return InvokeDataResponse(messagesContainer =>
            {
                // Clear unuse object
                recipe.Step = null;
                recipe.Chamber = null;
                recipe.User = null;
                recipe.RecipeFiles = null;

                Buisness.CheckRecipeValidity(recipe);
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var sqlRecipe = Mapper.Map<SQL.Recipe>(recipe);
                    if (recipe.KeyForAllVersion == Guid.Empty) // New Recipe
                    {
                        sqlRecipe.Version = 1;
                        sqlRecipe.KeyForAllVersion = Guid.NewGuid();
                        _logger.Information($"Add recipe {recipe.Name} step Id {recipe.StepId}");
                    }
                    else if (incrementVersion) // New version
                    {
                        if (recipe.KeyForAllVersion == null)
                            throw new InvalidDataException("KeyForAllVersion is missing" + recipe.Name);
                        sqlRecipe.Version = Buisness.GetLastRecipeVersionByKey(unitOfWork, recipe.KeyForAllVersion, false) + 1;
                        sqlRecipe.Id = 0;
                        _logger.Information($"New version for recipe {recipe.Name} step Id {recipe.StepId} version {sqlRecipe.Version}");
                    }
                    else // Update curent version
                    {
                        if (recipe.KeyForAllVersion == null)
                            throw new InvalidDataException("KeyForAllVersion is missing" + recipe.Name);
                        sqlRecipe.Id = Buisness.GetLastRecipeIdByKey(unitOfWork, recipe.KeyForAllVersion);
                        sqlRecipe = unitOfWork.RecipeRepository.CreateQuery(true).First(x => x.Id == sqlRecipe.Id);
                        sqlRecipe.Comment = recipe.Comment;
                        sqlRecipe.XmlContent = recipe.XmlContent;
                        _logger.Information($"Update recipe {recipe.Name} without new version step Id {recipe.StepId} version {recipe.Version}");
                    }

                    sqlRecipe.Created = DateTime.Now;
                    sqlRecipe.IsArchived = false;

                    if (sqlRecipe.Id == 0)
                    {
                        DataAccessHelper.LogInDatabase(unitOfWork,
                           recipe.CreatorUserId,
                           Dto.Log.ActionTypeEnum.Add,
                           Dto.Log.TableTypeEnum.Recipe,
                           $"Add recipe {recipe.Name} v{sqlRecipe.Version} for step {recipe.StepId.Value}", _logger);
                        unitOfWork.RecipeRepository.Add(sqlRecipe);
                    }
                    else
                    {
                        DataAccessHelper.LogInDatabase(unitOfWork,
                        recipe.CreatorUserId,
                        Dto.Log.ActionTypeEnum.Edit,
                        Dto.Log.TableTypeEnum.Recipe,
                        $"Edit recipe {recipe.Name} v{sqlRecipe.Version} for step {recipe.StepId.Value}", _logger);
                    }

                    unitOfWork.Save();

                    return sqlRecipe.Id;
                }
            });
        }

        public Response<List<RecipeInfo>> GetCompatibleRecipes(Guid? parentRecipeKey, int stepId, int toolKey)
        {
            _logger.Debug($"Get compatible recipe parent keys {parentRecipeKey}");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    // Comppatible with parent if parent is defined
                    if (parentRecipeKey.HasValue)
                    {
                        int version = Buisness.GetLastRecipeVersionByKey(unitOfWork, parentRecipeKey.Value);
                        var parentRecipe = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Outputs).SingleOrDefault(x => x.Version == version && x.KeyForAllVersion == parentRecipeKey);

                        if (parentRecipe == null)
                            throw new InvalidOperationException("Unknow recipe " + parentRecipeKey);
                        var parentOutputIds = parentRecipe.Outputs.Select(x => x.ResultType).ToList();

                        var toolId = GetToolIdFromToolKey(toolKey, unitOfWork);

                        var chamberIds = unitOfWork.ToolRepository.CreateQuery(false, x => x.Chambers).Single(x => x.Id == toolId).Chambers.Select(x => x.Id).ToList();

                        return unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs)
                                .Where(x => (x.IsTemplate || x.StepId == stepId) // Recipe is template or in current step
                                            && (x.IsArchived == false)
                                            && (x.IsShared || x.IsTemplate || chamberIds.Contains(x.CreatorChamberId))
                                            && parentOutputIds.Intersect(x.Inputs.Select(y => y.ResultType)).Count() > 0) // Recipe contains one input compatible with parent output. No ouput for the first recipe of th dataflow
                                .GroupBy(x => x.KeyForAllVersion)
                                .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault())
                                .Select(x => new RecipeInfo() { Name = x.Name, Version = x.Version, Comment = x.Comment, StepId = x.StepId, IsShared = x.IsShared, IsTemplate = x.IsTemplate, ActorType = (ActorType)x.ActorType, Key = x.KeyForAllVersion })
                                .ToList();
                    }
                    else
                    {
                        var res = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs)
                                .Where(x => (x.StepId == stepId || x.IsShared || x.IsTemplate) && (x.IsArchived == false))
                                .GroupBy(x => x.KeyForAllVersion)
                                .Select(x => x.OrderByDescending(y => y.Version).FirstOrDefault())
                                .Select(x => new RecipeInfo() { Name = x.Name, Version = x.Version, Comment = x.Comment, StepId = x.StepId, IsShared = x.IsShared, IsTemplate = x.IsTemplate, ActorType = (ActorType)x.ActorType, Key = x.KeyForAllVersion })
                                .ToList();
                        // at teh begin of dataflow tree only start with PM (process modules)
                        return res.Where(x => (((ActorType)x.ActorType).GetCatgory() == ActorCategory.ProcessModule)).ToList();

                        // made by Nicolas CHaux for Dataflow... try to understand why ..if no sens go remove the following
                        // return res.Where(x => 
                        //    (((ActorType)x.ActorType).GetCatgory() == ActorCategory.ProcessModule)
                        //    ||
                        //     (((ActorType)x.ActorType).GetCatgory() == ActorCategory.PostProcessing)
                        //).ToList();
                    }
                }
            });
        }

        public Response<int> CloneRecipe(Guid key, string newName, int userId)
        {
            _logger.Debug($"Clone recipe {key} new name {newName} user id {userId}");
            return InvokeDataResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int lastVersion = Buisness.GetLastRecipeVersionByKey(unitOfWork, key, false);
                    var sqlRecipe = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).SingleOrDefault(x => x.Version == lastVersion && x.KeyForAllVersion == key);
                    sqlRecipe.Id = 0;
                    sqlRecipe.Name = newName;
                    sqlRecipe.Version = 1;
                    sqlRecipe.KeyForAllVersion = Guid.NewGuid();
                    sqlRecipe.Created = DateTime.Now;
                    sqlRecipe.CreatorUserId = userId;

                    DataAccessHelper.LogInDatabase(unitOfWork,
                         sqlRecipe.CreatorUserId,
                         Dto.Log.ActionTypeEnum.Add,
                         Dto.Log.TableTypeEnum.Recipe,
                         $"Add recipe {sqlRecipe.Name} v{sqlRecipe.Version} for step {sqlRecipe.StepId.Value}", _logger);
                    unitOfWork.RecipeRepository.Add(sqlRecipe);

                    unitOfWork.Save();
                    return sqlRecipe.Id;
                }
            });
        }

        public Response<VoidResult> ArchiveAllVersionOfDataflow(Guid key, int userId)
        {
            _logger.Debug($"ArchiveAllVersion for dataflow {key} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    DataAccessHelper.ArchiveAllVerionsOfDataflow(key, userId, unitOfWork, _logger);

                    unitOfWork.Save();
                }
  
            });
        }

        public Response<VoidResult> RestoreAllVersionOfDataflow(Guid key, int userId)
        {
            _logger.Debug($"RestorellVersion for dataflow {key} by userId {userId}");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var dataflows = unitOfWork.DataflowRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
                    foreach (var dataflow in dataflows)
                    {
                        dataflow.IsArchived = false;
                    }

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Restore,
                       Dto.Log.TableTypeEnum.Dataflow,
                       $"RestoreAllVersion for dataflow {key}", _logger);

                    unitOfWork.Save();
                }
            });
        }

        public Response<VoidResult> ChangeDataflowSharedState(Guid key, int userId, bool isShared)
        {
            _logger.Debug($"ChangeDataflowSharedState for dataflow {key} by userId {userId} is shared {isShared} ");
            return InvokeVoidResponse(messagesContainer =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    var dataflows = unitOfWork.DataflowRepository.CreateQuery(true).Where(x => x.KeyForAllVersion == key).ToList();
                    foreach (var dataflow in dataflows)
                    {
                        dataflow.IsShared = isShared;
                    }

                    DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Edit,
                       Dto.Log.TableTypeEnum.Recipe,
                       $"Change shared state for datafow {key} is shared {isShared}", _logger);

                    unitOfWork.Save();
                }
            });
        }

        public Response<bool> IsConnectionAvailable()
        {
            return InvokeDataResponse(() =>
            {
                bool isConnectionOk = false;
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    isConnectionOk = unitOfWork.ToolRepository.CreateQuery(true).Any();
                }
                return isConnectionOk;
            });
        }

        public Response<int> SetExternalFile(ExternalFileBase externalFile, int recipeId, int userId)
        {
            _logger.Debug($"Set external file {externalFile.FileNameKey} recipe id {recipeId} user id {userId}");

            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    if (string.IsNullOrEmpty(externalFile.FileNameKey))
                        throw new InvalidOperationException("Extenal file name key is missing");

                    var recipe = unitOfWork.RecipeRepository.CreateQuery().SingleOrDefault(x => x.Id == recipeId);
                    if (recipe == null)
                        throw new InvalidOperationException($"Recipe id {recipeId} does not exist");

                    var newSqlRecipeFile = new SQL.RecipeFile();
                    newSqlRecipeFile.Created = DateTime.Now;
                    newSqlRecipeFile.CreatorUserId = userId;
                    newSqlRecipeFile.IsArchived = false;
                    newSqlRecipeFile.FileName = externalFile.FileNameKey;
                    newSqlRecipeFile.MD5 = Md5.ComputeHash(externalFile.Data);
                    newSqlRecipeFile.RecipeID = recipeId;
                    newSqlRecipeFile.FileType = externalFile.GetType().ToString();

                    bool fileMustBeSave = true;

                    var existingRecipeFile = unitOfWork.RecipeFileRepository.CreateQuery().FirstOrDefault(x => x.RecipeID == recipeId && x.FileName == externalFile.FileNameKey);
                    if (existingRecipeFile == null)
                    {
                        // Previous recipe
                        var previousRecipe = unitOfWork.RecipeRepository.CreateQuery(false).SingleOrDefault(x => x.Version == recipe.Version - 1 && x.KeyForAllVersion == recipe.KeyForAllVersion);
                        if (previousRecipe != null)
                            existingRecipeFile = unitOfWork.RecipeFileRepository.CreateQuery().Where(x => x.RecipeID == previousRecipe.Id).FirstOrDefault(x => x.FileName == externalFile.FileNameKey);
                    }

                    if (existingRecipeFile != null) // File already exist
                    {
                        if (newSqlRecipeFile.MD5 == existingRecipeFile.MD5)
                        {
                            fileMustBeSave = false;
                            newSqlRecipeFile = existingRecipeFile;
                            newSqlRecipeFile.RecipeID = recipeId;
                        }
                        else
                        {
                            newSqlRecipeFile.Version = existingRecipeFile.Version + 1;
                        }
                    }
                    else
                    {
                        newSqlRecipeFile.Version = 1;
                    }

                    if (fileMustBeSave)
                    {
                        string filePath = GetExternalFilePath(newSqlRecipeFile, recipe);
                        string folderPath = Path.GetDirectoryName(filePath);
                        if (!Directory.Exists(folderPath))
                            Directory.CreateDirectory(folderPath);

                        _logger.Information($"SetExternalFile save new file {filePath}");

                        externalFile.SaveToFile(filePath);
                        DataAccessHelper.LogInDatabase(unitOfWork,
                       userId,
                       Dto.Log.ActionTypeEnum.Add,
                       Dto.Log.TableTypeEnum.RecipeFile,
                       $"New recipe file {newSqlRecipeFile.FileName} V{newSqlRecipeFile.Version} in recipe  Name: {recipe.Name} Name: {recipe.Id}", _logger);
                    }

                    unitOfWork.RecipeFileRepository.Add(newSqlRecipeFile);

                    unitOfWork.Save();
                    return newSqlRecipeFile.Id;
                }
            });
        }

        public Response<ExternalFileBase> GetExternalFile(string fileNameKey, Guid recipeKey)
        {
            _logger.Debug($"Get external file {fileNameKey} key {recipeKey}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int recipeId = Buisness.GetLastRecipeIdByKey(unitOfWork, recipeKey, false);

                    // Split into two requests for optimization : Using Navigation properties on RecipeFile is slow
                    var recipe = unitOfWork.RecipeRepository.CreateQuery().SingleOrDefault(x => x.Id == recipeId);
                    if (recipe == null)
                        throw new InvalidOperationException($"Recipe id {recipeId} does not exist");

                    var recipeFiles = unitOfWork.RecipeFileRepository.CreateQuery().Where(x => x.RecipeID == recipeId);

                    var existingRecipeFile = recipeFiles.FirstOrDefault(x => x.FileName == fileNameKey);
                    if (existingRecipeFile == null)
                        throw new InvalidOperationException($"RecipeFile with filename {fileNameKey} does not exist");

                    var dtoAssembly = Assembly.Load("UnitySC.Shared.Data");
                    var t = dtoAssembly.GetType(existingRecipeFile.FileType);
                    var resImage = Activator.CreateInstance(t) as ExternalFileBase;
                    string filePath = GetExternalFilePath(existingRecipeFile, recipe);
                    _logger.Debug($"Get external file recipeFile {fileNameKey} filePath {filePath}");
                    resImage.LoadFromFile(filePath);
                    resImage.FileNameKey = fileNameKey;
                    return resImage;
                }
            });
        }

        public Response<List<ExternalFileBase>> GetExternalFiles(Guid recipeKey)
        {
            _logger.Debug($"Get external files key {recipeKey}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    int recipeId = Buisness.GetLastRecipeIdByKey(unitOfWork, recipeKey, false);
                    var recipe = unitOfWork.RecipeRepository.CreateQuery().SingleOrDefault(x => x.Id == recipeId);
                    var recipeFiles = unitOfWork.RecipeFileRepository.CreateQuery().Where(x => x.RecipeID == recipeId);

                    if (recipe == null)
                        throw new InvalidOperationException($"Recipe id {recipeId} does not exist");

                    var files = new List<ExternalFileBase>();
                    var dtoAssembly = Assembly.Load("UnitySC.Shared.Data");

                    foreach (var recipeFile in recipeFiles)
                    {
                        // Todo save type in DataBase
                        var t = dtoAssembly.GetType("UnitySC.Shared.Data.ExternalFile.ExternalImage");
                        var resImage = Activator.CreateInstance(t) as ExternalFileBase;
                        string filePath = GetExternalFilePath(recipeFile, recipe);
                        _logger.Debug($"Get external file recipeFile {recipeFile.FileName} filePath {filePath}");
                        resImage.LoadFromFile(filePath);
                        resImage.FileNameKey = recipeFile.FileName;
                        files.Add(resImage);
                    }

                    return files;
                }
            });
        }

        private string GetExternalFilePath(SQL.RecipeFile recipeFile, SQL.Recipe recipe)
        {
            var templateDico = new Dictionary<TemplateKeyword, string>();
            templateDico.Add(TemplateKeyword.RecipeKey, recipe.KeyForAllVersion.ToString());
            templateDico.Add(TemplateKeyword.FileNameWithoutExtension, Path.GetFileNameWithoutExtension(recipeFile.FileName));
            templateDico.Add(TemplateKeyword.Version, recipeFile.Version.ToString());
            templateDico.Add(TemplateKeyword.FileExtension, Path.GetExtension(recipeFile.FileName));
            string filePath = Path.Combine(DataAccessConfiguration.Instance.RootExternalFilePath, DataAccessConfiguration.Instance.TemplateExternalFilePath);

            foreach (var keyWord in templateDico)
            {
                filePath = filePath.Replace("{" + keyWord.Key + "}", keyWord.Value);
            }

            return filePath;
        }

        private bool CheckIfTemplateExternalFilePathIsValid()
        {
            var regex = new Regex("{(.+?)}");
            var stringKeyWordsInConfig = regex.Matches(DataAccessConfiguration.Instance.TemplateExternalFilePath).Cast<Match>()
                                    .Select(m => m.Groups[1].Value);
            bool result = true;
            foreach (string stringKeyWord in stringKeyWordsInConfig)
            {
                TemplateKeyword keyword;
                result = Enum.TryParse(stringKeyWord, out keyword);
                if (!result)
                {
                    _logger.Error($"Invalid template key word in TemplateExternalFilePath: {stringKeyWord}");
                    break;
                }
            }
            return result;
        }

        public Response<int> GetRecipeId(Guid key, int version)
        {
            _logger.Debug($"GetRecipeId key:{key} version:{version}");
            return InvokeDataResponse(() =>
            {
                using (var unitOfWork = new UnitOfWorkUnity(DataAccessConfiguration.Instance.DbConnectionString))
                {
                    return unitOfWork.RecipeRepository.CreateQuery()
                        .Where(x => x.KeyForAllVersion == key && x.Version == version)
                        .Select(x => x.Id)
                        .First();
                }
            });
        }

        private int GetChamberIdFromKeys(int chamberKey, int toolKey, UnitOfWorkUnity unitOfWorkUnity)
        {
            int toolid = GetToolIdFromToolKey(toolKey, unitOfWorkUnity);
            var chamber = unitOfWorkUnity.ChamberRepository.CreateQuery(false).FirstOrDefault(x => x.ToolId == toolid && x.ChamberKey == chamberKey);
            if (chamber == null)
                throw new InvalidOperationException($"No Chamber of toolkey(#{toolKey}) has a ChamberKey = {chamberKey}");
            return chamber.Id;
        }

        private int GetToolIdFromToolKey(int toolKey, UnitOfWorkUnity unitOfWorkUnity)
        {
            var tool = unitOfWorkUnity.ToolRepository.CreateQuery(false).FirstOrDefault(x => x.ToolKey == toolKey);
            if (tool == null)
                throw new InvalidOperationException($"No Tool has a ToolKey = {toolKey}");
            return tool.Id;
        }

    
    }
}
