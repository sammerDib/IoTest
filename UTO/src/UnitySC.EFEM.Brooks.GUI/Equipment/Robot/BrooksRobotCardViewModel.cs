using System.Threading.Tasks;

using Agileo.Common.Localization;

using UnitySC.EFEM.Brooks.Devices.Robot.BrooksRobot;
using UnitySC.EFEM.Brooks.GUI.Resources;
using UnitySC.GUI.Common.Equipment.Robot;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;

namespace UnitySC.EFEM.Brooks.GUI.Equipment.Robot
{
    public class BrooksRobotCardViewModel : RobotCardViewModel
    {
        static BrooksRobotCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(BrooksRobotCardViewModel), typeof(BrooksRobotCard));

            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(EquipmentResources)));
        }

        public BrooksRobotCardViewModel(
            BrooksRobot robot) : base(robot)
        {
            Robot = robot;
        }

        #region Properties

        public new BrooksRobot Robot { get; }

        private string _selectedSelectedMotionProfile;

        public string SelectedMotionProfile
        {
            get => _selectedSelectedMotionProfile;
            set => SetAndRaiseIfChanged(ref _selectedSelectedMotionProfile, value);
        }

        #endregion

        #region SetMotionProfile

        private SafeDelegateCommandAsync _setMotionProfileCommand;

        public SafeDelegateCommandAsync SetMotionProfileCommand
            => _setMotionProfileCommand ??= new SafeDelegateCommandAsync(
                SetMotionProfileCommandExecute,
                SetMotionProfileCommandCanExecute);

        private Task SetMotionProfileCommandExecute()
        {
            return Robot.SetMotionProfileAsync(SelectedMotionProfile);
        }

        private bool SetMotionProfileCommandCanExecute()
        {
            if (Robot == null)
            {
                return false;
            }

            var context = Robot.NewCommandContext(nameof(Robot.SetMotionProfile))
                .AddArgument("motionProfile", SelectedMotionProfile);
            return Robot.CanExecute(context);
        }

        #endregion
    }
}
