using UnitySC.GUI.Common.UIComponents.Components.Equipment;
using UnitySC.GUI.Common.UIComponents.Components.Equipment.Modules.Robot;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.EFEM.Rorze.GUI.UIComponents.Components.Equipment.Modules.Robot.MapperRR75x
{
    public class MapperRR75xModuleViewModel : RobotModuleViewModel
    {
        #region Constructor

        static MapperRR75xModuleViewModel()
        {
            DataTemplateGenerator.Create(typeof(MapperRR75xModuleViewModel), typeof(RobotModule));
        }

        public MapperRR75xModuleViewModel(Devices.Robot.MapperRR75x.MapperRR75x robot) : base(robot)
        {
        }

        #endregion

        #region Public


        public override RobotOrientation? GetOrientation()
        {
            //Specific use case for robot mapper
            if (Robot is Devices.Robot.MapperRR75x.MapperRR75x { RobotPositionReverted: true })
            {
                return RobotOrientation.Up;
            }

            return null;
        }

        #endregion
    }
}
