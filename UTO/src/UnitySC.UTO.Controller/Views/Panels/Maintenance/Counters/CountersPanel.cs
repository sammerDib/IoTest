using Agileo.Common.Localization;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.UTO.Controller.Counters;
using UnitySC.UTO.Controller.Views.Panels.EquipmentHandling;

namespace UnitySC.UTO.Controller.Views.Panels.Maintenance.Counters
{
    public class CountersPanel : BusinessPanel
    {
        #region Constructor

        static CountersPanel()
        {
            DataTemplateGenerator.Create(typeof(CountersPanel), typeof(CountersPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(CountersPanelResources)));
        }

        public CountersPanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            var resetCommand = new SafeDelegateCommand<object>(ResetCommandExecute, ResetCommandCanExecute);
            ResetCommand = new InvisibleBusinessPanelCommand(
                nameof(EquipmentHandlingResources.EQUIPMENT_RESET),
                resetCommand,
                PathIcon.Restore);
        }

        #endregion

        #region Commands

        #region Reset Command

        public InvisibleBusinessPanelCommand ResetCommand { get; }

        private bool ResetCommandCanExecute(object counter)
        {
            return ResetCommand?.IsEnabled ?? false;
        }

        private void ResetCommandExecute(object counter)
        {
            if (counter is not CounterDefinition value)
            {
                return;
            }
            App.ControllerInstance.CountersManager.IncrementCounter(value, true);
        }

        #endregion

        #endregion
    }
}
