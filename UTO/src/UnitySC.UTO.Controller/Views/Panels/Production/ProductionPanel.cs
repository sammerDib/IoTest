using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.Production
{
    class ProductionPanel : BusinessPanel
    {

        static ProductionPanel()
        {
            DataTemplateGenerator.Create(typeof(ProductionPanel), typeof(ProductionPanelView));
        }

        /// <inheritdoc />
        public ProductionPanel(string id, IIcon icon = null) : base(id, icon)
        {
            Commands.Add(new BusinessPanelCommand("ALARM_LIST", new DelegateCommand(() => { })));
        }

        /// <inheritdoc />
        public override void OnSetup()
        {
        }
    }
}
