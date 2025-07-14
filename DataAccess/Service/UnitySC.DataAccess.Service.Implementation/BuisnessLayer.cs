using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

using AutoMapper;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.SQL;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;

namespace UnitySC.DataAccess.Service.Implementation
{
    /// <summary>
    ///  Buisness layer for all data access service
    /// </summary>
    internal class BuisnessLayer
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public BuisnessLayer(ILogger logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        internal DataflowRecipeComponent GetLastDataflowComponent(UnitOfWorkUnity unitOfWork, string name, int stepId)
        {
            int lastVersion = GetLastDataflowVersionByName(unitOfWork, stepId, name, false);
            var recipe = unitOfWork.DataflowRepository.CreateQuery().SingleOrDefault(x => x.Name == name && x.Version == lastVersion);
            if (recipe == null)
                return null;
            var dataflowRecipe = (DataflowRecipeComponent)Deserialize<DataflowRecipeComponent>(recipe.XmlContent);
            return dataflowRecipe;
        }

        internal DataflowRecipeComponent GetLastDataflowComponent(UnitOfWorkUnity unitOfWork, Guid key)
        {
            int lastVersion = GetLastDataflowVersionByKey(unitOfWork, key, false);
            var recipe = unitOfWork.DataflowRepository.CreateQuery().SingleOrDefault(x => x.Version == lastVersion && x.KeyForAllVersion == key);
            if (recipe == null)
                return null;
            var dataflowRecipe = (DataflowRecipeComponent)Deserialize<DataflowRecipeComponent>(recipe.XmlContent);
            return dataflowRecipe;
        }

        internal DataflowRecipeComponent GetDataflowComponent(UnitOfWorkUnity unitOfWork, int id, bool lastVersionOfRecipes = true)
        {
            var recipe = unitOfWork.DataflowRepository.CreateQuery().SingleOrDefault(x => x.Id == id);
            if (recipe == null)
                return null;
            var DataflowRecipe = (DataflowRecipeComponent)Deserialize<DataflowRecipeComponent>(recipe.XmlContent);
            if (lastVersionOfRecipes)
            {
                // Update last version of recipe in Dataflow
                foreach (var child in DataflowRecipe.AllChilds().Where(x => x.ActorType != ActorType.DataflowManager))
                {
                    int lastVersion = GetLastRecipeVersionByKey(unitOfWork, child.Key);
                    var sqlchildRecipe = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Inputs, x => x.Outputs).SingleOrDefault(x => x.Version == lastVersion && x.KeyForAllVersion == child.Key);
                    if (sqlchildRecipe == null)
                    {
                        _logger.Error($"Cannot load dataflow {DataflowRecipe.Name} the recipe {child.Name} does not exist");
                        throw new InvalidOperationException($"Cannot load dataflow {DataflowRecipe.Name} the recipe {child.Name} does not exist");
                    }
                    child.Name = sqlchildRecipe.Name;
                    child.Key = sqlchildRecipe.KeyForAllVersion;
                    child.ActorType = (ActorType)sqlchildRecipe.ActorType;
                    child.Version = sqlchildRecipe.Version;
                    child.Comment = sqlchildRecipe.Comment;
                    child.Inputs = sqlchildRecipe.Inputs.Select(x => (ResultType)x.ResultType).ToList();
                    child.Outputs = sqlchildRecipe.Outputs.Select(x => (ResultType)x.ResultType).ToList();
                    child.IsShared = sqlchildRecipe.IsShared;
                }
            }

            return DataflowRecipe;
        }

        internal List<Guid> GetLinkedRecipe(List<Guid> linked, DataflowRecipeComponent dataflowRecipeComponent)
        {
            foreach (var sub in dataflowRecipeComponent.ChildRecipes)
            {
                linked.Add(sub.Component.Key);
                GetLinkedRecipe(linked, sub.Component);
            }

            return linked;
        }

        internal object Deserialize<T>(string xml)
        {
            using (Stream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;
                var deserializer = new DataContractSerializer(typeof(T));
                return deserializer.ReadObject(stream);
            }
        }

        internal string Serialize<T>(T objectToSerialize)
        {
            using (var output = new StringWriter())
            using (var writer = new XmlTextWriter(output) { Formatting = Formatting.Indented })
            {
                var serializer = new DataContractSerializer(typeof(T), null,
                    0x7FFF /*maxItemsInObjectGraph*/,
                    false /*ignoreExtensionDataObject*/,
                    true /*preserveObjectReferences : this is where the magic happens */,
                    null /*dataContractSurrogate*/);

                serializer.WriteObject(writer, objectToSerialize);
                return output.GetStringBuilder().ToString();
            }
        }

        //to do : exludenotvalidated
        internal int GetLastRecipeVersionByName(UnitOfWorkUnity unitOfWork, ActorType actorType, int stepId, string name, bool takeArchivedRecipes = false)
        {
            return unitOfWork.RecipeRepository.CreateQuery().Where(
                x => x.ActorType == (int)actorType
                && x.Name == name
                && (takeArchivedRecipes || !x.IsArchived)
                && x.StepId == stepId)
                .Select(x => x.Version).DefaultIfEmpty().Max();
        }

        //to do : exludenotvalidated
        internal int GetLastDataflowVersionByName(UnitOfWorkUnity unitOfWork, int stepId, string name, bool takeArchivedRecipes = false)
        {
            return unitOfWork.DataflowRepository.CreateQuery().Where(
                x => x.Name == name
                && (takeArchivedRecipes || !x.IsArchived)
                && x.StepId == stepId)
                .Select(x => x.Version).DefaultIfEmpty().Max();
        }

        //to do : exludenotvalidated
        internal int GetLastRecipeVersionByKey(UnitOfWorkUnity unitOfWork, Guid key, bool takeArchivedRecipes = false)
        {
            return unitOfWork.RecipeRepository.CreateQuery().Where(
                x => x.KeyForAllVersion == key
                && (takeArchivedRecipes || !x.IsArchived))
                .Select(x => x.Version).DefaultIfEmpty().Max();
        }

        //to do : exludenotvalidated
        internal int GetLastRecipeIdByKey(UnitOfWorkUnity unitOfWork, Guid key, bool takeArchivedRecipes = false)
        {
            int version = GetLastRecipeVersionByKey(unitOfWork, key, takeArchivedRecipes);
            return unitOfWork.RecipeRepository.CreateQuery().Where(x => x.Version == version && x.KeyForAllVersion == key).Select(x => x.Id).First();
        }

        //to do : exludenotvalidated
        internal TCPMRecipeInfo GetLastTCPMRecipeInfo(UnitOfWorkUnity unitOfWork, Guid key, ActorType actorType, bool takeArchivedRecipes = false)
        {
            int version = GetLastRecipeVersionByKey(unitOfWork, key, takeArchivedRecipes);
            return unitOfWork.RecipeRepository.CreateQuery(false, x => x.Step.Product, x => x.User).Where(x => x.Version == version && x.KeyForAllVersion == key && x.ActorType == (int)actorType)
                .Select(x => new TCPMRecipeInfo()
                {
                    StepName = x.Step.Name,
                    ProductName = x.Step.Product.Name,
                    RecipeName = x.Name,
                    Comment = x.Comment,
                    UserInfo = x.User
                }).FirstOrDefault();
        }

        //to do : exludenotvalidated
        internal int GetLastDataflowVersionByKey(UnitOfWorkUnity unitOfWork, Guid key, bool takeArchivedRecipes = false)
        {
            return unitOfWork.DataflowRepository.CreateQuery().Where(
                x => x.KeyForAllVersion == key
                && (takeArchivedRecipes || !x.IsArchived))
                .Select(x => x.Version).DefaultIfEmpty().Max();
        }

        internal List<Dto.RecipeFile> GetRecipeFilesInfo(UnitOfWorkUnity unitOfWork, int recipeId)
        {
            var dtoRecipeFiles = new List<Dto.RecipeFile>();
            IEnumerable<SQL.RecipeFile> sqlRecipeFiles = unitOfWork.RecipeFileRepository.CreateQuery().Where(x => x.RecipeID == recipeId);

            foreach (var recipeSqlFile in sqlRecipeFiles)
                dtoRecipeFiles.Add(_mapper.Map<Dto.RecipeFile>(recipeSqlFile));

            return dtoRecipeFiles;
        }

        internal bool DataflowIsValid(UnitOfWorkUnity unitOfWork, DataflowRecipeComponent dataflowRecipe, int stepId)
        {
            return RecursiveValidity(unitOfWork, dataflowRecipe, stepId);
        }

        private bool RecursiveValidity(UnitOfWorkUnity unitOfWork, DataflowRecipeComponent dataflowRecipeComponent, int stepId)
        {
            if (RecipeComponentIsValid(unitOfWork, dataflowRecipeComponent, stepId))
            {
                bool res = true;
                foreach (var sub in dataflowRecipeComponent.ChildRecipes)
                {
                    res = RecursiveValidity(unitOfWork, sub.Component, stepId);
                    if (!res)
                    {
                        break;
                    }
                }

                return res;
            }
            else
            {
                return false;
            }
        }

        private bool RecipeComponentIsValid(UnitOfWorkUnity unitOfWork, DataflowRecipeComponent dataflowRecipeComponent, int stepId)
        {
            bool res = false;

            if (dataflowRecipeComponent.ActorType != ActorType.DataflowManager)
            {
                int version = GetLastRecipeVersionByKey(unitOfWork, dataflowRecipeComponent.Key);
                var recipe = unitOfWork.RecipeRepository.CreateQuery().SingleOrDefault(x => x.Version == version && x.KeyForAllVersion == dataflowRecipeComponent.Key);

                res = recipe != null && (recipe.StepId == stepId || recipe.IsShared);
            }
            else
            {
                res = true;
            }

            if (!res)
                _logger.Information($"DataflowRecipeComponent {dataflowRecipeComponent.Name} is not valid : Bad stepId or Unshare recipe");

            return res;
        }

        public void CheckRecipeValidity(Dto.Recipe recipe)
        {
            if (!recipe.StepId.HasValue)
                throw new InvalidOperationException("The step id is required");

            if (recipe.CreatorUserId == 0)
                throw new InvalidOperationException("The creator user id is required");

            if (recipe.Type == ActorType.Unknown)
                throw new InvalidOperationException("The actor type is required");

            if (recipe.CreatorChamberId == 0)
                throw new InvalidOperationException("The creator chamber id is required");

            var category = recipe.Type.GetCatgory();
            switch (category)
            {
                case ActorCategory.Unknown:
                case ActorCategory.Manager:
                    throw new InvalidOperationException("Invalid actor type");

                case ActorCategory.ProcessModule:
                    if (!recipe.Outputs.Any())
                        throw new InvalidOperationException("Some output is requied for Process Module recipe");
                    break;

                case ActorCategory.PostProcessing:
                    if (!recipe.Inputs.Any())
                        throw new InvalidOperationException("Some input is requied for Post Processing recipe");
                    if (!recipe.Outputs.Any())
                        throw new InvalidOperationException("Some output is requied for Post Processing recipe");
                    break;

                default:
                    throw new InvalidOperationException("Bad actor type");
            }
        }

        public List<SQL.Dataflow> GetLastDataflowsForTC(UnitOfWorkUnity unitOfWork, int toolId)
        {
            var dataflows = unitOfWork.DataflowRepository.CreateQuery(false)
                       .Where(x => (x.CreatorTool == toolId || x.IsShared == true)
                               && !x.IsArchived).ToList();

            // Get last Dataflows
            var lastDataflow = new List<SQL.Dataflow>();
            foreach (var key in dataflows.Select(x => x.KeyForAllVersion).Distinct())
            {
                int lastVersion = dataflows.Where(x => x.KeyForAllVersion == key).Select(x => x.Version).DefaultIfEmpty().Max();
                lastDataflow.Add(dataflows.Single(x => x.KeyForAllVersion == key && x.Version == lastVersion));
            }

            return lastDataflow;
        }

        public List<SQL.Recipe> GetLastRecipesForTC(UnitOfWorkUnity unitOfWork, ActorType actorType)
        {
            var recipes = unitOfWork.RecipeRepository.CreateQuery(false, x => x.Step.Product, x => x.User)
                       .Where(x => x.ActorType == (int)actorType && !x.IsArchived).ToList();

            // Get last recipe
            var lastRecipes = new List<SQL.Recipe>();
            foreach (var key in recipes.Select(x => x.KeyForAllVersion).Distinct())
            {
                int lastVersion = recipes.Where(x => x.KeyForAllVersion == key).Select(x => x.Version).DefaultIfEmpty().Max();
                lastRecipes.Add(recipes.Single(x => x.KeyForAllVersion == key && x.Version == lastVersion));
            }

            return lastRecipes;
        }

        internal List<DataflowRecipeInfo> GetAllDataflow(UnitOfWorkUnity unitOfWork, int toolId, bool takeArchivedDataflow)
        {
            return unitOfWork.DataflowRepository.CreateQuery()
                   .Where(sql => (sql.CreatorTool == toolId && sql.IsArchived == takeArchivedDataflow))
                   .GroupBy(x => x.KeyForAllVersion)
                   .Select(grp => new
                   {
                       grp.Key,
                       Dataflow = grp
                        .OrderByDescending(x => x.Version)
                        .FirstOrDefault()
                   }).Select(x => new DataflowRecipeInfo() { Id = x.Dataflow.Id, IdGuid = x.Dataflow.KeyForAllVersion, Name = x.Dataflow.Name, StepId = x.Dataflow.StepId, StepName = x.Dataflow.Step.Name, ProductId = x.Dataflow.Step.ProductId, ProductName = x.Dataflow.Step.Product.Name , CreatedDate = x.Dataflow.Created }).ToList();

        }
        public List<ActorType> GetActorTypes(DataflowRecipeComponent component)
        {
            var actorTypes = new List<ActorType>();
            if (component != null)
            {
                // Ajout de l'ActorType de l'instance actuelle (sauf ActorType.DataflowManager)
                if (component.ActorType != ActorType.DataflowManager)
                {
                    actorTypes.Add(component.ActorType);
                }

                // Appel récursif pour chercher dans les ChildRecipes
                foreach (var childRecipe in component.ChildRecipes)
                {
                    actorTypes.AddRange(GetActorTypes(childRecipe.Component));
                }
            }
            return actorTypes;
        }
        internal class TCPMRecipeInfo
        {
            public string StepName { get; set; }
            public string ProductName { get; set; }
            public string RecipeName { get; set; }
            public string Comment { get; set; }
            public SQL.User UserInfo { get; set; }
            public string Author => UserInfo != null ? UserInfo.Name : string.Empty;
        }
    }
}
