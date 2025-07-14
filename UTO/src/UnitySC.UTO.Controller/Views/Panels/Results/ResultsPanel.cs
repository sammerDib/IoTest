using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.Result.CommonUI.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.UTO.Controller.Views.Panels.Integration;

namespace UnitySC.UTO.Controller.Views.Panels.Results
{
    public class ResultsPanel : BaseUnityIntegrationPanel
    {
        #region Properties

        public MainResultVM MainResult => ClassLocator.Default.GetInstance<MainResultVM>();

        #endregion

        static ResultsPanel()
        {
            DataTemplateGenerator.Create(typeof(ResultsPanel), typeof(ResultsPanelView));
        }

        public ResultsPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
        }

        #region Overrides of BaseUnityIntegrationPanel

        protected override void Register()
        {
            ClassLocator.Default.Register<MainResultVM>(true);
            MainResult.Init();
            MainResult.InitRessources();
        }

        #endregion
    }
}
