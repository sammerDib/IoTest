using UnitsNet;

using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;
using UnitySC.Equipment.Abstractions.Devices.Efem.Enums;

namespace UnitySC.EquipmentController.Simulator.EquipmentData
{
    /// <summary>
    /// Class containing all data about device robot.
    /// It's objective is not to provide any logic or automatic behavior.
    /// It only aims to concentrate all known data sent from the EfemController.
    /// </summary>
    internal class RobotData
    {
        internal RobotStatus RobotStatus { get; set; }

        internal Ratio RobotSpeed { get; set; }

        internal GetWaferPresenceOnArmLastAction LastAction { get; set; }

        internal bool IsPresentOnUpperArm { get; set; }

        internal Constants.Stage StageUpperArm { get; set; }

        internal uint SlotUpperArm { get; set; }

        internal bool IsPresentOnLowerArm { get; set; }

        internal Constants.Stage StageLowerArm { get; set; }

        internal uint SlotLowerArm { get; set; }
    }
}
