using System;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitsNet;

using UnitySC.Equipment.Abstractions.Devices.Aligner;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Devices.Robot.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;

namespace UnitySC.Equipment.Abstractions.Devices.Robot
{
    public partial class Robot
    {
        protected virtual void InternalSimulateGoToHome(Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(3));
            Position = TransferLocation.Robot;
            UpperArmState = ArmState.Retracted;
            LowerArmState = ArmState.Retracted;
        }

        protected virtual void InternalSimulateGoToLocation(
            IMaterialLocationContainer destinationDevice,
            Tempomat tempomat)
        {
            if (destinationDevice is IAligner && Position != TransferLocation.PreAlignerA)
            {
                tempomat.Sleep(Duration.FromMilliseconds(1500));
            }
            else if (destinationDevice is IProcessModule
                     && Position != TransferLocation.ProcessModuleA)
            {
                tempomat.Sleep(Duration.FromMilliseconds(650));
            }
            else if (destinationDevice is IProcessModule
                     && Position != TransferLocation.ProcessModuleB)
            {
                tempomat.Sleep(Duration.FromMilliseconds(1500));
            }

            Position = RegisteredLocations[destinationDevice.Name];
        }

        protected virtual void InternalSimulateGoToTransferLocation(
            TransferLocation location,
            RobotArm arm,
            byte slot,
            Tempomat tempomat)
        {
            if (location == TransferLocation.PreAlignerA
                && Position != TransferLocation.PreAlignerA)
            {
                tempomat.Sleep(Duration.FromMilliseconds(1500));
            }
            else if (location == TransferLocation.ProcessModuleA
                     && Position != TransferLocation.ProcessModuleA)
            {
                tempomat.Sleep(Duration.FromMilliseconds(650));
            }
            else if (location == TransferLocation.ProcessModuleB
                     && Position != TransferLocation.ProcessModuleB)
            {
                tempomat.Sleep(Duration.FromMilliseconds(1500));
            }

            Position = location;
        }

        protected virtual void InternalSimulateGoToSpecifiedLocation(
            IMaterialLocationContainer destinationDevice,
            byte destinationSlot,
            RobotArm arm,
            bool isPickUpPosition,
            Tempomat tempomat)
        {
            if (destinationDevice is IAligner && Position != TransferLocation.PreAlignerA)
            {
                tempomat.Sleep(Duration.FromMilliseconds(1500));
            }
            else if (destinationDevice is IProcessModule
                     && Position != TransferLocation.ProcessModuleA)
            {
                tempomat.Sleep(Duration.FromMilliseconds(650));
            }
            else if (destinationDevice is IProcessModule
                     && Position != TransferLocation.ProcessModuleB)
            {
                tempomat.Sleep(Duration.FromMilliseconds(1500));
            }

            Position = RegisteredLocations[destinationDevice.Name];
        }

        protected virtual void InternalSimulatePick(
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

        protected virtual void InternalSimulatePlace(
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

            switch (arm)
            {
                case RobotArm.Arm1:
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

        protected virtual void InternalSimulateExtendArm(
            RobotArm arm,
            TransferLocation location,
            byte slot,
            Tempomat tempomat)
        {
            tempomat.Sleep(Duration.FromSeconds(1));
            Position = location;
            tempomat.Sleep(Duration.FromSeconds(1));
            switch (arm)
            {
                case RobotArm.Arm1:
                    UpperArmState = ArmState.Extended;
                    break;
                case RobotArm.Arm2:
                    LowerArmState = ArmState.Extended;
                    break;
            }
        }

        protected virtual void InternalSimulateClamp(RobotArm arm, Tempomat tempomat)
        {
            throw new NotImplementedException("Command Clamp has not been implemented");
        }

        protected virtual void InternalSimulateUnclamp(RobotArm arm, Tempomat tempomat)
        {
            throw new NotImplementedException("Command Unclamp has not been implemented");
        }

        protected virtual void InternalSimulateSetMotionSpeed(Ratio percentage, Tempomat tempomat)
        {
            Speed = percentage;
        }

        protected virtual void InternalSimulateSetDateAndTime(Tempomat tempomat)
        {
            throw new NotImplementedException("Command Place has not been implemented");
        }

        protected virtual void InternalSimulateTransfer(RobotArm pickArm, IMaterialLocationContainer sourceDevice, byte sourceSlot, RobotArm placeArm, IMaterialLocationContainer destinationDevice, byte destinationSlot, Tempomat tempomat)
        {
            InternalSimulatePick(pickArm, sourceDevice, sourceSlot, tempomat);
            InternalSimulatePlace(placeArm, destinationDevice, destinationSlot, tempomat);
        }

        protected virtual void InternalSimulateSwap(RobotArm pickArm, IMaterialLocationContainer sourceDevice, byte sourceSlot, Tempomat tempomat)
        {
            var placeArm = pickArm == RobotArm.Arm1
                ? RobotArm.Arm2
                : RobotArm.Arm1;

            InternalSimulatePick(pickArm, sourceDevice, sourceSlot, tempomat);
            InternalSimulatePlace(placeArm, sourceDevice, sourceSlot, tempomat);
        }


        #region Internal Methods

        /// <summary>Retrieve SubstrateLocation using provided parameters</summary>
        /// <param name="materialLocationContainer">The instance of device holding the SubstrateLocation.</param>
        /// <param name="locationIndex">The index of SubstrateLocation to retrieve.</param>
        /// <returns>Instance of required SubstrateLocation if found.</returns>
        internal SubstrateLocation RetrieveSubstrateLocation(
            IMaterialLocationContainer materialLocationContainer,
            byte locationIndex = 0)
        {
            return MaterialLocationHelper.RetrieveSubstrateLocation(
                DeviceExtensions.GetEquipment(this),
                materialLocationContainer,
                locationIndex);
        }

        #endregion
    }
}
