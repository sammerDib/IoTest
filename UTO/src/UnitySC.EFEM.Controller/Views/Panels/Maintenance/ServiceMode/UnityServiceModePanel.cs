using System;

using Agileo.Common.Localization;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode;

namespace UnitySC.EFEM.Controller.Views.Panels.Maintenance.ServiceMode
{
    public class UnityServiceModePanel : BusinessPanel
    {
        public ServiceModePanel ServiceModePanel { get; }

        public bool IsViewEnabled => App.EfemAppInstance.ControlState == ControlState.Local;

        static UnityServiceModePanel()
        {
            DataTemplateGenerator.Create(typeof(UnityServiceModePanel), typeof(UnityServiceModePanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(UnityServiceModeResources)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceModePanel" /> class ONLY FOR the DESIGN MODE.
        /// </summary>
        public UnityServiceModePanel() : this(null, $"{nameof(UnityServiceModePanel)} DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityServiceModePanel" /> class.
        /// </summary>
        public UnityServiceModePanel(
            ServiceModePanel serviceModePanel,
            string id,
            IIcon icon = null) : base(id, icon)
        {
            ServiceModePanel = serviceModePanel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServiceModePanel.Dispose();
            }

            base.Dispose(disposing);
        }

        public override void OnSetup()
        {
            base.OnSetup();
            ServiceModePanel.OnSetup();
        }
    }
}
