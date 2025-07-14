using Agileo.GUI.Services.Icons;

using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.ServiceMode;

namespace UnitySC.UTO.Controller.Views.Panels.Maintenance.ServiceMode
{
    public class UnityServiceModePanel : ServiceModePanel
    {
        #region Constructors

        static UnityServiceModePanel()
        {
            DataTemplateGenerator.Create(typeof(UnityServiceModePanel), typeof(UnityServiceModePanelView));
        }

        public UnityServiceModePanel(
            Agileo.EquipmentModeling.Equipment equipment,
            string id,
            IIcon icon = null)
            : base(equipment, id, icon)
        {

        }

        #endregion

        #region Properties

        public bool IsMaintenanceMode
            => App.ControllerInstance.ControllerEquipmentManager.Controller.State
               == OperatingModes.Maintenance;

        #endregion

        #region Overrides

        public override void OnSetup()
        {
            base.OnSetup();

            App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged +=
                Controller_StatusValueChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                App.ControllerInstance.ControllerEquipmentManager.Controller.StatusValueChanged -=
                    Controller_StatusValueChanged;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Event Handlers

        private void Controller_StatusValueChanged(object sender, Agileo.EquipmentModeling.StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(Equipment.Abstractions.Devices.Controller.Controller.State))
            {
                OnPropertyChanged(nameof(IsMaintenanceMode));
            }
        }

        #endregion
    }
}
