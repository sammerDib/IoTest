using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.DMT.Service.Interface;
using UnitySC.PM.DMT.Service.Interface.Measure;
using UnitySC.PM.DMT.Service.Interface.Recipe;
using UnitySC.PM.DMT.Service.Interface.RecipeService;
using UnitySC.PM.DMT.Shared.UI.Message;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.DMT.Shared.UI.Proxy
{
    /// <summary>
    ///     Proxy to supervise the distant recipe service
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RecipeSupervisor : IDMTRecipeServiceCallback
    {
        private readonly InstanceContext _instanceContext;
        private readonly ILogger _logger;
        private readonly IMessenger _messenger;
        private readonly DuplexServiceInvoker<IDMTRecipeService> _recipeService;

        /// <summary>
        ///     Constructor
        /// </summary>
        public RecipeSupervisor(ILogger<RecipeSupervisor> logger, IMessenger messenger)
        {
            _instanceContext = new InstanceContext(this);
            // Recipe service
            _recipeService = new DuplexServiceInvoker<IDMTRecipeService>(_instanceContext, "DEMETERRecipeService",
                                                                         ClassLocator.Default
                                                                             .GetInstance<
                                                                                 SerilogLogger<IDMTRecipeService>>(),
                                                                         messenger, s => s.Subscribe());
            _logger = logger;
            _messenger = messenger;
        }

        public void RecipeProgress(RecipeStatus status)
        {
            _messenger.Send(new RecipeMessage { Status = status });
        }

        public void ResultGenerated(string name, Side side, string path)
        {
            _logger.Debug($"AcquisitionSupervisor ResultGenerated {name} {side} {path}");
            _messenger.Send(new ResultMessage
                            {
                                Name = name,
                                Side = side,
                                Path = path
                            });
        }

        public DMTRecipe CreateRecipe(string name = null, int stepId = -1, int userId = 0)
        {
            return _recipeService.Invoke(s => s.CreateRecipe(name, stepId, userId));
        }

        public DMTRecipe GetRecipe(Guid recipeKey, bool takeArchivedRecipes = false)
        {
            return _recipeService.Invoke(s => s.GetRecipeFromKey(recipeKey, takeArchivedRecipes));
        }

        public DMTRecipe GetLastRecipeWithProductAndStep(Guid recipeKey)
        {
            var response = _recipeService.InvokeAndGetMessages(s => s.GetLastRecipeWithProductAndStep(recipeKey));
            if (response.Messages?.Any(message => message.Level == MessageLevel.Error) ?? false)
            {
                var exception = new Exception("Failed to load recipe");
                exception.Data.Add(RecipeLoadImportExceptionDataKeys.LoadCheckErrors,
                    response.Messages
                        .Where(message => Enum.IsDefined(typeof(RecipeLoadImportExceptionDataKeys), message.UserContent))
                        .Select(message => (RecipeLoadImportExceptionDataKeys)Enum.Parse(typeof(RecipeLoadImportExceptionDataKeys), message.UserContent))
                        .ToList());
                if (((List<RecipeLoadImportExceptionDataKeys>)exception.Data[
                        RecipeLoadImportExceptionDataKeys.LoadCheckErrors]).Count != response.Messages.Count)
                {
                    exception.Data.Add(RecipeLoadImportExceptionDataKeys.LoadCheckErrors,
                        response.Messages
                            .Where(message => !Enum.IsDefined(typeof(RecipeLoadImportExceptionDataKeys), message.UserContent))
                            .Select(message => message.UserContent).ToList());
                }
                throw exception;
            }
            return response.Result;
        }

        public void Test()
        {
            _recipeService.Invoke(s => s.Test());
        }

        public List<RecipeInfo> GetRecipeList(int stepId, bool takeArchivedRecipes = false)
        {
            return _recipeService.Invoke(s => s.GetRecipeList(stepId, takeArchivedRecipes));
        }

        public void SaveRecipe(DMTRecipe recipe)
        {
            _recipeService.Invoke(s => s.SaveRecipe(recipe));
        }

        public DMTRecipe ImportRecipe(DMTRecipe recipe, int stepId, int userId)
        {
            var response = _recipeService.InvokeAndGetMessages(s => s.ImportRecipe(recipe, stepId, userId));
            if (response.Messages?.Any(message => message.Level == MessageLevel.Error) ?? false)
            {
                var exception = new Exception("Import Recipe Failed");
                exception.Data.Add(RecipeLoadImportExceptionDataKeys.ImportCheckErrors,
                    response.Messages
                        .Where(message => Enum.IsDefined(typeof(RecipeCheckError), message.UserContent))
                        .Select(message => (RecipeLoadImportExceptionDataKeys)Enum.Parse(typeof(RecipeCheckError), message.UserContent))
                        .ToList());
                if (((List<RecipeLoadImportExceptionDataKeys>)exception.Data[
                        RecipeLoadImportExceptionDataKeys.ImportCheckErrors]).Count != response.Messages.Count)
                {
                    exception.Data.Add(RecipeLoadImportExceptionDataKeys.ImportErrors,
                        response.Messages
                            .Where(message => !Enum.IsDefined(typeof(RecipeCheckError), message.UserContent))
                            .Select(message => message.UserContent)
                            .ToList());   
                }
                throw exception;
            }
            return response.Result;
        }

        public DMTRecipe ExportRecipe(Guid recipeKey)
        {
            return _recipeService.Invoke(s => s.GetRecipeForExport(recipeKey));
        }

        public async Task StartRecipeAsync(DMTRecipe recipe, string acqDestFolder, bool overwriteOutput)
        {
            await _recipeService.InvokeAsync(s => s.StartRecipeAsync(recipe, acqDestFolder, overwriteOutput, string.Empty));
        }

        public void Abort()
        {
            _recipeService.Invoke(s => s.Abort());
        }

        public async Task<Dictionary<CurvatureImageType, ServiceImage>> BaseCurvatureDynamicAcquisition(
            DeflectometryMeasure measure, Length waferDiameter, bool isDarkRequired = false)
        {
            return await _recipeService.InvokeAsync(s => s.BaseCurvatureDarkDynamicsAcquisition(measure, waferDiameter, isDarkRequired));
        }

        public async Task<Dictionary<CurvatureImageType, ServiceImage>> RecalculateCurvatureDynamic(
            DeflectometryMeasure measure)
        {
            return await _recipeService.InvokeAsync(s => s.RecalculateCurvatureDynamics(measure));
        }

        public async Task<ServiceImage> RecalculateDarkDynamics(DeflectometryMeasure measure)
        {
            return await _recipeService.InvokeAsync(s => s.RecalculateDarkDynamics(measure));
        }

        public void DisposeCurvatureDynamicAdjustmentMeasureExecution()
        {
            _recipeService.Invoke(s => s.DisposeCurvatureDarkDynamicsAdjustmentMeasureExecution());
        }

        public RemoteProductionInfo GetDefaultMaterial()
        {
            return _recipeService.Invoke(s => s.GetDefaultRemoteProductionInfo());
        }
    }
}
