using System;

using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.Recipes.Management.ViewModel;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.UTO.Controller.Views.Panels.Integration;

namespace UnitySC.UTO.Controller.Views.Panels.Production.RecipeRun
{
    public sealed class RecipeRunLiveManagementPanel : BaseUnityIntegrationPanel
    {
        private readonly DriveableProcessModule _processModule;

        static RecipeRunLiveManagementPanel()
        {
            DataTemplateGenerator.Create(typeof(RecipeRunLiveManagementPanel), typeof(RecipeRunLiveManagementPanelView));
        }

        public RecipeRunLiveManagementPanel(
            string id,
            DriveableProcessModule processModule,
            IIcon icon = null)
        : base(id, icon)
        {
            _processModule = processModule;
        }

        private RecipeRunLiveViewModel _recipeRunLive;

        public RecipeRunLiveViewModel RecipeRunLive
        {
            get => _recipeRunLive;
            set => SetAndRaiseIfChanged(ref _recipeRunLive, value);
        }

        #region Overrides of BaseUnityIntegrationPanel

        public override void OnShow()
        {
            base.OnShow();

            if (RecipeRunLive == null)
            {
                return;
            }

            try
            {
                RecipeRunLive.Display();
            }
            catch (Exception e)
            {
                Popups.Show(PopupHelper.Error(e.Message));
            }
        }

        protected override void Register()
        {
            base.Register();

            RegisterIDbRecipeService();

            RegisterSharedSupervisors();
            if (!ClassLocator.Default.IsRegistered<GlobalStatusSupervisor>())
            {
                ClassLocator.Default.Register<GlobalStatusSupervisor>(() => ClassLocator.Default.GetInstance<SharedSupervisors>().GetGlobalStatusSupervisor(ActorType.ANALYSE), true);
            }

            RegisterExternalUserControls();

            // Client Configuration already registered by DataFlowClientConfigurationManager

            RegisterUserSupervisor();
        }

        public override void OnHide()
        {
            base.OnHide();

            try
            {
                RecipeRunLive?.Hide();
            }
            catch (Exception e)
            {
                Popups.Show(PopupHelper.Error(e.Message));
            }
        }

        public override void OnSetup()
        {
            base.OnSetup();
            RecipeRunLive = new RecipeRunLiveViewModel();
        }

        public override void OnFinalizeSetup()
        {
            base.OnFinalizeSetup();

            // It is necessary to set the actor type after the setup of all
            // unity integration panels because the first call to
            // ClassLocator.Default.GetInstance prevents the setup of subsequent panels.
            RecipeRunLive.Actor = _processModule.ActorType;
        }

        #endregion

        #region Overrides of IdentifiableElement

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RecipeRunLive?.Hide();
                RecipeRunLive = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
