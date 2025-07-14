using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    /// <summary>
    ///     Class used to configure the UnloadFromPm activity.
    /// </summary>
    public class UnloadFromPmConfiguration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnloadFromPmConfiguration" /> class.
        /// </summary>
        /// <param name="equipment">The equipment instance.</param>
        /// <param name="destinationSlot">Destination slot of the substrate.</param>
        public UnloadFromPmConfiguration(Agileo.EquipmentModeling.Equipment equipment, byte destinationSlot, EffectorType effectorType)
        {
            if (equipment == null)
            {
                throw new ArgumentNullException(nameof(equipment), "Provided equipment cannot be null.");
            }

            if (destinationSlot == 0)
            {
                throw new ArgumentException(
                    "Provided destination slot must be greater than 0.",
                    nameof(destinationSlot));
            }

            Equipment = equipment;
            DestinationSlot = destinationSlot;
            EffectorType = effectorType;

            // Set default values
            LoadPort = equipment.AllDevices<Abstractions.Devices.LoadPort.LoadPort>().FirstOrDefault(lp => lp.InstanceId == 1);
            ProcessModule = equipment.AllDevices<Abstractions.Devices.DriveableProcessModule.DriveableProcessModule>()
                .FirstOrDefault(pm => pm.InstanceId == 1);
            RobotArm = RobotArm.Arm1;
        }

        /// <summary>
        ///     Gets the Equipment instance.
        /// </summary>
        public Agileo.EquipmentModeling.Equipment Equipment { get; }

        /// <summary>
        ///     Gets or sets the Load Port instance to which wafer will be transfer.
        /// </summary>
        public Abstractions.Devices.LoadPort.LoadPort LoadPort { get; private set; }

        /// <summary>
        ///     Gets or sets the Process Module instance from which wafer will be transfer.
        /// </summary>
        public Abstractions.Devices.DriveableProcessModule.DriveableProcessModule ProcessModule { get; private set; }

        /// <summary>
        ///     Gets the empty slot to which wafer will be transfer.
        /// </summary>
        public byte DestinationSlot { get; }

        /// <summary>
        /// Gets the effector type
        /// </summary>
        public EffectorType EffectorType { get; }

        /// <summary>
        ///     Gets or sets the robot arm to use for wafer transfer.
        /// </summary>
        public RobotArm RobotArm { get; private set; }

        /// <summary>
        ///     Sets the Load Port to which wafer will be transfer.
        /// </summary>
        /// <param name="loadPort">The load port instance.</param>
        public void SetLoadPort(Abstractions.Devices.LoadPort.LoadPort loadPort)
        {
            LoadPort = loadPort
                       ?? throw new ArgumentNullException(
                           nameof(loadPort),
                           "Provided Load Port cannot be null.");
        }

        /// <summary>
        ///     Sets the Process Module from which wafer will be transfer.
        /// </summary>
        /// <param name="processModule">The ProcessModule instance.</param>
        public void SetProcessModule(Abstractions.Devices.DriveableProcessModule.DriveableProcessModule processModule)
        {
            ProcessModule = processModule
                            ?? throw new ArgumentNullException(
                                nameof(processModule),
                                "Provided Process Module cannot be null.");
        }

        /// <summary>
        ///     Sets the robot arm to use for wafer transfer.
        /// </summary>
        /// <param name="arm"></param>
        public void SetRobotArm(RobotArm arm)
        {
            if (arm == RobotArm.Undefined)
            {
                throw new ArgumentException("Provided Robot Arm cannot be Undefined.", nameof(arm));
            }

            RobotArm = arm;
        }
    }
}
