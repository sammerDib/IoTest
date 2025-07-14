using Agileo.EquipmentModeling;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Robot
{
    public class RobotModuleViewModel : MachineModuleViewModel
    {
        public UnitySC.Equipment.Abstractions.Devices.Robot.Robot Robot { get; }

        public override IMaterialLocationContainer MaterialLocation => Robot;

        #region Constructor

        static RobotModuleViewModel()
        {
            DataTemplateGenerator.Create(typeof(RobotModuleViewModel), typeof(RobotModule));
        }

        public RobotModuleViewModel(UnitySC.Equipment.Abstractions.Devices.Robot.Robot robot)
        {
            Robot = robot;
        }

        #endregion

        #region Public

        public virtual RobotOrientation? GetOrientation()
        {
            return null;
        }

        #endregion
    }
}
