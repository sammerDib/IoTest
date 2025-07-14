using UnitySC.Equipment.Abstractions.Vendor.Devices.Simulation;

namespace UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2.Simulation
{
    public class Dio2SimulationData : SimulationData
    {
        #region Constructor

        public Dio2SimulationData(Dio2 dio2)
            : base(dio2)
        {

        }

        #endregion

        #region Properties

        #region Inputs

        private bool _pm1DoorOpened;
        public bool I_PM1_DoorOpened
        {
            get => _pm1DoorOpened;
            set => SetAndRaiseIfChanged(ref _pm1DoorOpened, value, nameof(I_PM1_DoorOpened));
        }

        private bool _pm1ReadyToLoadUnload;
        public bool I_PM1_ReadyToLoadUnload
        {
            get => _pm1ReadyToLoadUnload;
            set => SetAndRaiseIfChanged(ref _pm1ReadyToLoadUnload, value, nameof(I_PM1_ReadyToLoadUnload));
        }

        private bool _pm2DoorOpened;
        public bool I_PM2_DoorOpened
        {
            get => _pm2DoorOpened;
            set => SetAndRaiseIfChanged(ref _pm2DoorOpened, value, nameof(I_PM2_DoorOpened));
        }

        private bool _pm2ReadyToLoadUnload;
        public bool I_PM2_ReadyToLoadUnload
        {
            get => _pm2ReadyToLoadUnload;
            set => SetAndRaiseIfChanged(ref _pm2ReadyToLoadUnload, value, nameof(I_PM2_ReadyToLoadUnload));
        }

        private bool _pm3DoorOpened;
        public bool I_PM3_DoorOpened
        {
            get => _pm3DoorOpened;
            set => SetAndRaiseIfChanged(ref _pm3DoorOpened, value, nameof(I_PM3_DoorOpened));
        }

        private bool _pm3ReadyToLoadUnload;
        public bool I_PM3_ReadyToLoadUnload
        {
            get => _pm3ReadyToLoadUnload;
            set => SetAndRaiseIfChanged(ref _pm3ReadyToLoadUnload, value, nameof(I_PM3_ReadyToLoadUnload));
        }

        #endregion Inputs

        #region Outputs

        private bool _robotArmNotExtendedPm1;
        public bool O_RobotArmNotExtended_PM1
        {
            get => _robotArmNotExtendedPm1;
            set => SetAndRaiseIfChanged(ref _robotArmNotExtendedPm1, value, nameof(O_RobotArmNotExtended_PM1));
        }

        private bool _robotArmNotExtendedPm2;
        public bool O_RobotArmNotExtended_PM2
        {
            get => _robotArmNotExtendedPm2;
            set => SetAndRaiseIfChanged(ref _robotArmNotExtendedPm2, value, nameof(O_RobotArmNotExtended_PM2));
        }

        private bool _robotArmNotExtendedPm3;
        public bool O_RobotArmNotExtended_PM3
        {
            get => _robotArmNotExtendedPm3;
            set => SetAndRaiseIfChanged(ref _robotArmNotExtendedPm3, value, nameof(O_RobotArmNotExtended_PM3));
        }

        #endregion Outputs

        #endregion
    }
}
