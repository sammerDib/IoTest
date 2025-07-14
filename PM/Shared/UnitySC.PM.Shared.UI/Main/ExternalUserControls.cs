using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Reflection;

using UnitySC.PM.Shared.UC;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;

namespace UnitySC.PM.Shared.UI.Main
{
    public class ExternalUserControls
    {
        [ImportMany(typeof(IMenuItem))]
        public IEnumerable<Lazy<IMenuItem>> MenuItems { get; set; }

        [ImportMany(typeof(IPmInit))]
        public IEnumerable<Lazy<IPmInit, IUCMetadata>> PmInits { get; set; }

        [ImportMany(typeof(IRecipeSummaryUc))]
        public IEnumerable<Lazy<IRecipeSummaryUc, IUCMetadata>> RecipeSummaries { get; set; }

        [ImportMany(typeof(IRecipeEditorUc))]
        public IEnumerable<Lazy<IRecipeEditorUc, IUCMetadata>> RecipeEditors { get; set; }

        [ImportMany(typeof(IRecipeRunLiveViewUc))]
        public IEnumerable<Lazy<IRecipeRunLiveViewUc, IUCMetadata>> RecipeRunLiveViews { get; set; }

        public bool IsInit { get; private set; }

        private ILogger _logger;

        public ExternalUserControls(ILogger logger)
        {
            _logger = logger;
        }

        public void Init(string directoryPath = null)
        {
            string path;
            if (string.IsNullOrEmpty(directoryPath))
            {
                var uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                path = System.IO.Path.GetDirectoryName(uri.AbsolutePath);
            }
            else
            {
                path = System.IO.Path.GetFullPath(directoryPath);
            }
            try
            {
                _logger.Information("Load ExternalUserControls: " + path);
                var catalog = new DirectoryCatalog(path, "UnitySC.*.dll");
                var container = new CompositionContainer(catalog);
                container.ComposeParts(this);
                IsInit = true;
            }
            catch (ReflectionTypeLoadException ex)
            {
                string errorMessage = "Error during loading MEF container part for external user controls in " + path;
                foreach (var error in ex.LoaderExceptions)
                {
                    errorMessage = errorMessage + Environment.NewLine + "- " + error.Message;
                }
                _logger.Error(errorMessage, ex);
                throw new Exception(errorMessage, ex);
            }
            catch (Exception ex)
            {
                string errorMessage = "Error during loading MEF container part for external user controls in " + path;
                _logger.Error(errorMessage, ex);
                throw new Exception(errorMessage, ex);
            }
        }


        public IRecipeSummaryUc GetRecipeSummary(ActorType actorType)
        {
            IRecipeSummaryUc result = null;

            if (RecipeSummaries != null)
            {
                var uc = RecipeSummaries.FirstOrDefault(x => (ActorType)x.Metadata.ActorType == actorType);

                if (uc == null)
                {
                    uc = RecipeSummaries.FirstOrDefault(x => (ActorType)(x.Metadata.ActorType) == ActorType.Unknown);
                }

                if (uc != null)
                    result = uc.Value;
            }

            return result;
        }

        public IRecipeEditorUc GetRecipeEditor(ActorType actorType)
        {
            IRecipeEditorUc result = null;

            if (RecipeEditors != null)
            {
                var uc = RecipeEditors.FirstOrDefault(x => (ActorType)x.Metadata.ActorType == actorType);

                if (uc == null)
                {
                    uc = RecipeEditors.FirstOrDefault(x => (ActorType)(x.Metadata.ActorType) == ActorType.Unknown);
                    var dummy = uc?.Value as DummyPM.DummyRecipeEditor;
                    if (dummy != null)
                        dummy.ActorType = actorType;

                }

                if (uc != null)
                    result = uc.Value;
            }

            return result;
        }

        public IRecipeRunLiveViewUc GetRecipeRunLiveView(ActorType actorType)
        {
            IRecipeRunLiveViewUc result = null;

            if (RecipeRunLiveViews != null)
            {
                var uc = RecipeRunLiveViews.FirstOrDefault(x => (ActorType)x.Metadata.ActorType == actorType);

                if (uc == null)
                {
                    //uc = RecipeRunLiveViews.FirstOrDefault(x => (ActorType)(x.Metadata.ActorType) == ActorType.Unknow);
                }

                if (uc != null)
                    result = uc.Value;
            }

            return result;
        }
    }
}
