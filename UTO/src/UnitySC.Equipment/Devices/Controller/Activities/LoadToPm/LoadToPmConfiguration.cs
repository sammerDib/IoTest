using System;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner.Enums;
using UnitySC.Equipment.Abstractions.Enums;

namespace UnitySC.Equipment.Devices.Controller.Activities
{
    /// <summary>
    ///     Class used to configure the LoadToPm activity.
    /// </summary>
    public class LoadToPmConfiguration
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LoadToPmConfiguration" /> class.
        /// </summary>
        /// <param name="equipment">The equipment instance.</param>
        /// <param name="sourceSlot">Source slot of the substrate.</param>
        public LoadToPmConfiguration(Agileo.EquipmentModeling.Equipment equipment, byte sourceSlot, EffectorType effectorType)
        {
            if (equipment == null)
            {
                throw new ArgumentNullException(nameof(equipment), "Provided equipment cannot be null.");
            }

            if (sourceSlot == 0)
            {
                throw new ArgumentException("Provided source slot must be greater than 0.", nameof(sourceSlot));
            }

            Equipment = equipment;
            SourceSlot = sourceSlot;
            EffectorType = effectorType;

            // Set default values
            LoadPort = equipment.AllDevices<Abstractions.Devices.LoadPort.LoadPort>().FirstOrDefault(lp => lp.InstanceId == 1);
            ProcessModule = equipment.AllDevices<Abstractions.Devices.DriveableProcessModule.DriveableProcessModule>()
                .FirstOrDefault(pm => pm.InstanceId == 1);
            AlignAngle = Angle.Zero;
            AlignType = AlignType.AlignWaferForMainO_FlatCheckingSubO_FlatLocation;
            RobotArm = RobotArm.Arm1;
        }

        /// <summary>
        ///     Gets the Equipment instance.
        /// </summary>
        public Agileo.EquipmentModeling.Equipment Equipment { get; }

        /// <summary>
        ///     Gets or sets the Load Port instance from which wafer will be transfer.
        /// </summary>
        public Abstractions.Devices.LoadPort.LoadPort LoadPort { get; private set; }

        /// <summary>
        ///     Gets or sets the Process Module instance to which wafer will be transfer.
        /// </summary>
        public Abstractions.Devices.DriveableProcessModule.DriveableProcessModule ProcessModule { get; private set; }

        /// <summary>
        ///     Gets the slot containing wafer to transfer to Process Module.
        /// </summary>
        public byte SourceSlot { get; }

        /// <summary>
        ///     Gets or sets the angle to use for wafer alignment.
        /// </summary>
        public Angle AlignAngle { get; private set; }

        /// <summary>
        ///     Gets or sets the fiducial to use for wafer alignment.
        /// </summary>
        public AlignType AlignType { get; private set; }

        /// <summary>
        /// Gets the effector type
        /// </summary>
        public EffectorType EffectorType { get; }

        /// <summary>
        ///     Gets or sets the robot arm to use for wafer transfer.
        /// </summary>
        public RobotArm RobotArm { get; private set; }

        /// <summary>
        ///     Sets the Load Port from which wafer will be transfer.
        /// </summary>
        /// <param name="loadPort">The load port instance.</param>
        public void SetLoadPort(Abstractions.Devices.LoadPort.LoadPort loadPort)
        {
            LoadPort = loadPort ?? throw new ArgumentNullException(nameof(loadPort), "Provided Load Port cannot be null.");
        }

        /// <summary>
        ///     Sets the Process Module to which wafer will be transfer.
        /// </summary>
        /// <param name="processModule">The ProcessModule instance.</param>
        public void SetProcessModule(Abstractions.Devices.DriveableProcessModule.DriveableProcessModule processModule)
        {
            ProcessModule = processModule ?? throw new ArgumentNullException(nameof(processModule), "Provided Process Module cannot be null.");
        }

        /// <summary>
        ///     Sets the angle to use for wafer alignment.
        /// </summary>
        /// <param name="angle"></param>
        public void SetAlignAngle(Angle angle)
        {
            AlignAngle = angle;
        }

        /// <summary>
        ///     Sets the align type to use for wafer alignment.
        /// </summary>
        /// <param name="alignType"></param>
        public void SetAlignType(AlignType alignType) => AlignType = alignType;

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
