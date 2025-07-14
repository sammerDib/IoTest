using System;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Recipes
{
    public class DataFlowRecipePanel : BusinessPanel
    {
        #region Properties

        public DataFlowTree DataFlowTree { get; private set; }

        #endregion

        #region Constructor
        static DataFlowRecipePanel()
        {
            DataTemplateGenerator.Create(typeof(DataFlowRecipePanel), typeof(DataFlowRecipePanelView));
        }

        public DataFlowRecipePanel() : this("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public DataFlowRecipePanel(string id, IIcon icon = null)
            : base(id, icon)
        {
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing)
        {
            DataFlowTree?.Dispose();
            base.Dispose(disposing);
        }

        public override void OnSetup()
        {
            base.OnSetup();

            var dataFlowManager = App.ControllerInstance.ControllerEquipmentManager.Controller
                .TryGetDevice<AbstractDataFlowManager>();

            DataFlowTree = new DataFlowTree(dataFlowManager);
        }

        public override void OnShow()
        {
            base.OnShow();
            DataFlowTree?.Refresh();
        }

        #endregion
    }
}
