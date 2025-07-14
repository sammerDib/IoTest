using System;
using System.Collections.Generic;
using System.Linq;

using UnitySC.DataAccess.Dto;
using UnitySC.DataAccess.Dto.ModelDto.LocalDto;
using UnitySC.DataAccess.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.TC.API.Operations.Interface;
using UnitySC.Shared.TC.API.Service.Interface;
using UnitySC.Shared.TC.Shared;
using UnitySC.Shared.TC.Shared.Types;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Shared.TC.API.Operations.Implementation
{
    public class RecipeOperations : IRecipeOperations, IUTODFServiceCB
    {
        private ILogger _logger;
        private IPMStateManager _pmStatesManager;
        private IUTODFServiceCB _utoService;
        private ServiceInvoker<IDbRecipeService> _dbRecipeService = null;

        private List<DataflowFullInfo> _recipeList = new List<DataflowFullInfo>();
        public List<DataflowFullInfo> RecipeList { get => _recipeList; set => _recipeList = value; }

        /// <summary>
        /// list of WF recipes indexed by Guid and version. the key is KeyForAllVersion
        /// </summary>
        private SortedDictionary<Guid, DataFlowRecipe> _listDataFlowRecipe = new SortedDictionary<Guid, DataFlowRecipe>();

        public RecipeOperations()
        {
            _logger = ClassLocator.Default.GetInstance<ILogger<RecipeOperations>>();
            _dbRecipeService = ClassLocator.Default.GetInstance<ServiceInvoker<IDbRecipeService>>();
            _utoService = ClassLocator.Default.GetInstance<IUTODFServiceCB>();
        }

        public void Init()
        {
           // _pmStatesManager = ClassLocator.Default.GetInstance<IPMStateManager>();           
        }

        #region private methods

        private void AddRecipe(DataflowFullInfo recipe)
        {
            List<DataflowFullInfo> newRecipes = new List<DataflowFullInfo>();
            newRecipes.Add(recipe);
            AddRecipes(newRecipes);
        }

        public void AddRecipes(List<DataflowFullInfo> recipesList)
        {
            foreach (DataflowFullInfo currRecipe in recipesList)
            {
                if (_recipeList.FindIndex(r => (r.IdGuid == currRecipe.IdGuid)) < 0)
                {
                    _recipeList.Add(currRecipe);
                    _utoService.DFRecipeAdded(currRecipe);
                }
            }
            _logger.Debug("Recipes: " + string.Join(",", _recipeList.Select<DataflowFullInfo, string>(r => r.Name)));
        }

        private int DeleteRecipes(String recipe)
        {
            List<String> recipes = new List<String>();
            recipes.Add(recipe);
            return DeleteRecipes(recipe);
        }

        public void DeleteRecipes(List<DataflowFullInfo> recipesList)
        {
            List<DataflowFullInfo> recipesDeletedToSent = new List<DataflowFullInfo>();
            foreach (DataflowFullInfo currRecipe in recipesList)
            {
                if (_recipeList.Find(r => (r.IdGuid == currRecipe.IdGuid)) != null)
                {
                    _recipeList.Remove(currRecipe);
                    _utoService.DFRecipeDeleted(currRecipe);
                }
            }
            _logger.Debug("Recipes deleted: " + string.Join(",", _recipeList.Select<DataflowFullInfo, String>(r => r.Name)));
        }

        #endregion private methods

        #region IRecipeOperations

        public void AbortRecipe(DataflowFullInfo dfRecipe)
        {
            _logger.Debug("[Request from TC] RequestRecipeService - AbortRecipe " + dfRecipe.Name);
            _pmStatesManager.AbortRecipeNotification(dfRecipe);            
        }

        public void DFRecipeListUpdated()
        {
            _logger.Debug("UpdateTCRecipeList - Recipes list changed");

            // Get all recipes
            List<DataflowFullInfo> allRecipesList = _pmStatesManager.GetRecipesList();

            // Search all new recipes not in RecipeList
            List<DataflowFullInfo> newRecipesList = allRecipesList.FindAll(r => !RecipeList.Exists(currRecipe => currRecipe.IdGuid == r.IdGuid));
            List<DataflowFullInfo> deletedRecipesList = RecipeList.FindAll(currRecipe => !allRecipesList.Exists(r => currRecipe.IdGuid == r.IdGuid));

            if (newRecipesList != null && newRecipesList.Count != 0)
            {
                // Send recipes to TC and update TRecipeList to know all recipes known by TC
                AddRecipes(newRecipesList);
            }
            if (deletedRecipesList != null && deletedRecipesList.Count != 0)
            {
                // Send recipes to TC and update TRecipeList to know all recipes known by TC
                DeleteRecipes(deletedRecipesList);
            }
        }

        public List<DataflowFullInfo> GetAllRecipe(List<ActorType> actors, int toolId)
        {
            _logger.Debug("Star GetAllRecipe service methode.");
            var dfRecipes = new List<DataflowFullInfo>();
            try
            {
                dfRecipes = _dbRecipeService.Invoke(x => x.GetAllDataflow(actors, toolId, false));
            }
            catch (Exception ex)
            {
                _logger.Debug("GetAllRecipe Exception: {0}", ex.Message);
            }

            return dfRecipes;
        }

        public UTOJobProgram StartRecipeDF(DataflowFullInfo dfRecipe, List<UnitySC.Shared.TC.API.Shared.Material> wafers)
        {
            _logger.Debug("StartRecipe ");
            var jobResult = new UTOJobProgram();
            try
            {
                DataFlowRecipe currentDataFlowRecipe = LoadDataFlowRecipe(dfRecipe, wafers);

                DataflowID dfId = new DataflowID() { Id = dfRecipe.Id, IdGuid = dfRecipe.IdGuid, Wafers = wafers };

                DataflowInstance df = new DataflowInstance(dfRecipe, currentDataFlowRecipe, dfId);

                currentDataFlowRecipe.StartedDataflow.Add(dfRecipe.IdGuid, df);

                //return
                var actorTypes = currentDataFlowRecipe.Actors.Values.Select(x => x.ActorType).ToList();
                jobResult.PMItems = new List<PMItem>();
                foreach (var actorType in actorTypes)
                {
                    jobResult.PMItems.Add(new PMItem() { PMType = actorType, OrientationAngle = 90 });
                }
                jobResult.OCRReadingParameters = new OCRReadingParameters() { OCRRecipeName = "Front", OCRAngle = 0 };
                
                _utoService.DFRecipeAdded(dfRecipe);
                _utoService.DFRecipeProcessStarted(dfRecipe);
            }
            catch (Exception ex)
            {
                _logger.Debug("StartRecipeDF Exception: {0}", ex.Message);
            }

            return jobResult;
        }

        public void SelectRecipe(DataflowFullInfo dfRecipe)
        {
            throw new NotImplementedException();
        }

        // TODO DATAFLOW : cette méthode doit être utilisé côté Dataflow.
        private DataFlowRecipe LoadDataFlowRecipe(DataflowFullInfo dfRecipe, List<UnitySC.Shared.TC.API.Shared.Material> wafers)
        {
            bool toLoad = false;

            DataflowRecipeComponent recipeDatabase = _dbRecipeService?.Invoke(x => x.GetLastDataflow(dfRecipe.IdGuid, false));
            Guid keyForAllVersion = recipeDatabase.Key;
            if (!_listDataFlowRecipe.TryGetValue(keyForAllVersion, out DataFlowRecipe currentDataFlowRecipe))
            {
                toLoad = true;
            }
            else
            {
                // if we find the recipe and it is not of the same version, we recharge it.
                toLoad = recipeDatabase.Version != currentDataFlowRecipe.Version;
            }
            if (toLoad)
            {
                currentDataFlowRecipe = new DataFlowRecipe();

                currentDataFlowRecipe.Id = "";
                currentDataFlowRecipe.KeyForAllVersion = keyForAllVersion;
                currentDataFlowRecipe.DataflowRecipeComponent = recipeDatabase;
                currentDataFlowRecipe.Version = recipeDatabase.Version;

                var composents = recipeDatabase.AllChilds();

                var recipeActors = composents.Where(c => c.ActorType != ActorType.DataflowManager).ToList();

                // creating DataflowActor
                DataflowManagerActor dataflowManagerActor = null;
                foreach (var ra in recipeActors)
                {
                    switch (ra.ActorCategory)
                    {
                        case ActorCategory.ProcessModule:
                            dataflowManagerActor = new PMActor(ra.ActorType.ToString(), ra.ActorType);
                            break;

                        case ActorCategory.PostProcessing:
                            break;

                        default:
                            break;
                    }
                    currentDataFlowRecipe.Actors.Add(
                        ra.ActorType.ToString(),

                        new DataflowActor(ra.ActorType.ToString(), ra.ActorType)
                        {
                            DataflowManagerActor = dataflowManagerActor,
                            DataflowActorRecipe = new DataflowActorRecipe() { KeyForAllVersion = ra.Key, Name = ra.Name },
                            Inputs = ra.Inputs.Select(i => new InputOutputDataType() { ResultType = i }).ToList(),
                            Outputs = ra.Outputs.Select(i => new InputOutputDataType() { ResultType = i }).ToList(),
                        }
                        );
                }
                // once the DataflowActors are created, we re-browse to create the links with the children
                foreach (var ra in recipeActors)
                {
                    if (currentDataFlowRecipe.Actors.TryGetValue(ra.ActorType.ToString(), out DataflowActor wfa))
                    {
                        foreach (var rac in ra.ChildRecipes)
                        {
                            if (currentDataFlowRecipe.Actors.TryGetValue(rac.Component.ActorType.ToString(), out DataflowActor wfac))
                            {
                                wfa.ChildActors.Add(wfac);
                            }
                        }
                    }
                }
                _listDataFlowRecipe.Add(
                keyForAllVersion,
                currentDataFlowRecipe
                );
            }
            return currentDataFlowRecipe;
        }

        public void StartJob_Material(UnitySC.Shared.TC.API.Shared.Material material)
        {
        }

        public void OnChangedDFStatus(string value)
        {
            throw new NotImplementedException();
        }

        public void SetAlarm(List<int> alarms)
        {
            throw new NotImplementedException();
        }

        public void ResetAlarm(List<int> alarms)
        {
            throw new NotImplementedException();
        }

        public void FireEvent(CommonEvent ecid)
        {
            throw new NotImplementedException();
        }

        public bool SetECValues(List<EquipmentConstant> equipmentConstants)
        {
            throw new NotImplementedException();
        }

        public void DFRecipeProcessStarted(DataflowFullInfo dfRecipe)
        {
            throw new NotImplementedException();
        }

        public void DFRecipeProcessComplete(DataflowFullInfo dfRecipe, string status)
        {
            throw new NotImplementedException();
        }

        public void DFRecipeAdded(DataflowFullInfo dfRecipe)
        {
            throw new NotImplementedException();
        }

        public void DFRecipeDeleted(DataflowFullInfo dfRecipe)
        {
            throw new NotImplementedException();
        }

        public void PMRecipeProcessStarted(ActorType pmType, DataflowFullInfo pmRrecipe)
        {
            throw new NotImplementedException();
        }

        public void PMRecipeProcessComplete(ActorType pmType, DataflowFullInfo pmRrecipe, string status)
        {
            throw new NotImplementedException();
        }

        public bool SVSetMessage(List<StatusVariable> statusVariables)
        {
            throw new NotImplementedException();
        }

        #endregion IRecipeOperations
    }
}
