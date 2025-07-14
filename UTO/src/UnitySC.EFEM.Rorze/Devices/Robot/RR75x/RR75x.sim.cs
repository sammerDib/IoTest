using System;

using Agileo.Drivers;
using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.EFEM.Rorze.Devices.Robot.RR75x.Simulation;
using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.EFEM.Rorze.Devices.Robot.RR75x
{
    public partial class RR75x : ISimDevice
    {
        #region Fields

        /// <summary>
        /// Indicates the size of the substrate involved in the last robot's command sent to the driver.
        /// </summary>
        protected SampleDimension LastMovedSubstrateSize;

        #endregion Fields

        #region Properties

        protected internal RR75xSimulationData SimulationData { get; private set; }

        #endregion

        #region ISimDevice

        public ISimDeviceView SimDeviceView
            => new RR75xSimulatorUserControl { DataContext = SimulationData };

        #endregion ISimDevice

        #region Commands

        protected virtual void InternalSimulateGetStatuses(Tempomat tempomat)
        {
            //Do nothing, in simulation mode statuses are automatically updated
        }

        protected override void InternalSimulateInitialize(bool mustForceInit, Tempomat tempomat)
        {
            base.InternalSimulateInitialize(mustForceInit, tempomat);
            Position = TransferLocation.Robot;
            UpperArmState = ArmState.Retracted;
            LowerArmState = ArmState.Retracted;
            HasBeenInitialized = true;
            if (UpperArmWaferPresence == WaferPresence.Unknown)
            {
                UpperArmWaferPresence = WaferPresence.Absent;
            }

            if (LowerArmWaferPresence == WaferPresence.Unknown)
            {
                LowerArmWaferPresence = WaferPresence.Absent;
            }
        }

        protected override void InternalSimulatePick(
            RobotArm arm,
            IMaterialLocationContainer sourceDevice,
            byte sourceSlot,
            Tempomat tempomat)
        {
            var sourceLocation = RetrieveSubstrateLocation(sourceDevice, sourceSlot);
            if (sourceDevice is ILoadPort)
            {
                tempomat.Sleep(Duration.FromSeconds(1));
            }

            Position = RegisteredLocations[sourceDevice.Name];

            // Ask confirmation to extend the arm
            var guid = Guid.NewGuid();
            SynchronizedRobotWrapper.Set(
                guid,
                delegate
                {
                    var action = new RobotAction
                    {
                        Command = RobotCommands.Pick,
                        SourceLocation = Position,
                        SourceSlotNumber = sourceSlot,
                        DestinationLocation = TransferLocation.Robot,
                        DestinationSlotNumber = 1,
                        ArmLoad = arm
                    };
                    OnCommandConfirmationRequested(
                        new CommandConfirmationRequestedEventArgs(guid, action));
                });
            LastMovedSubstrateSize = sourceLocation.Substrate.MaterialDimension;
            switch (arm)
            {
                case RobotArm.Arm1:
                    sourceLocation.Material.SetLocation(UpperArmLocation);
                    UpperArmWaferPresence = WaferPresence.Present;
                    if (sourceDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (sourceDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (sourceDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    UpperArmState = ArmState.Extended;
                    if (sourceDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (sourceDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (sourceDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    UpperArmState = ArmState.Retracted;
                    break;
                case RobotArm.Arm2:
                    sourceLocation.Material.SetLocation(LowerArmLocation);
                    LowerArmWaferPresence = WaferPresence.Present;
                    if (sourceDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (sourceDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (sourceDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    LowerArmState = ArmState.Extended;
                    if (sourceDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (sourceDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (sourceDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    LowerArmState = ArmState.Retracted;
                    break;
                case RobotArm.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
            }
        }

        protected override void InternalSimulatePlace(
            RobotArm arm,
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot,
            Tempomat tempomat)
        {
            var destinationLocation = RetrieveSubstrateLocation(destinationDevice, destinationSlot);
            if (destinationDevice is ILoadPort)
            {
                tempomat.Sleep(Duration.FromSeconds(1));
            }

            Position = RegisteredLocations[destinationDevice.Name];

            // Ask confirmation to extend arm
            var guid = Guid.NewGuid();
            SynchronizedRobotWrapper.Set(
                guid,
                delegate
                {
                    var action = new RobotAction
                    {
                        Command = RobotCommands.Place,
                        SourceLocation = TransferLocation.Robot,
                        SourceSlotNumber = 1,
                        DestinationLocation = Position,
                        DestinationSlotNumber = destinationSlot,
                        ArmUnLoad = arm
                    };
                    OnCommandConfirmationRequested(
                        new CommandConfirmationRequestedEventArgs(guid, action));
                });
            switch (arm)
            {
                case RobotArm.Arm1:
                    LastMovedSubstrateSize =
                        (UpperArmLocation.Material as Substrate).MaterialDimension;
                    UpperArmLocation.Material.SetLocation(destinationLocation);
                    UpperArmWaferPresence = WaferPresence.Absent;
                    if (destinationDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (destinationDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (destinationDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    UpperArmState = ArmState.Extended;
                    if (destinationDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (destinationDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (destinationDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    UpperArmState = ArmState.Retracted;
                    break;
                case RobotArm.Arm2:
                    LastMovedSubstrateSize =
                        (LowerArmLocation.Material as Substrate).MaterialDimension;
                    LowerArmLocation.Material.SetLocation(destinationLocation);
                    LowerArmWaferPresence = WaferPresence.Absent;
                    if (destinationDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (destinationDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (destinationDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    LowerArmState = ArmState.Extended;
                    if (destinationDevice is ILoadPort)
                    {
                        tempomat.Sleep(Duration.FromSeconds(1));
                    }
                    else if (destinationDevice is IAligner)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(850));
                    }
                    else if (destinationDevice is IProcessModule)
                    {
                        tempomat.Sleep(Duration.FromMilliseconds(1250));
                    }

                    LowerArmState = ArmState.Retracted;
                    break;
                case RobotArm.Undefined:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arm), arm, null);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>Retrieve SubstrateLocation using provided parameters</summary>
        /// <param name="materialLocationContainer">The instance of device holding the SubstrateLocation.</param>
        /// <param name="locationIndex">The index of SubstrateLocation to retrieve.</param>
        /// <returns>Instance of required SubstrateLocation if found.</returns>
        internal SubstrateLocation RetrieveSubstrateLocation(
            IMaterialLocationContainer materialLocationContainer,
            byte locationIndex = 0)
            => MaterialLocationHelper.RetrieveSubstrateLocation(
                this.GetEquipment(),
                materialLocationContainer,
                locationIndex);

        private void SetUpSimulatedMode() => SimulationData = new RR75xSimulationData(this);

        #endregion
    }
}
