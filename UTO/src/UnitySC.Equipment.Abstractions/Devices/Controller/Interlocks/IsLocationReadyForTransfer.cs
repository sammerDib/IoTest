using System;
using System.Collections.Generic;
using System.Globalization;

using Agileo.EquipmentModeling;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.Robot;
using UnitySC.Equipment.Abstractions.Devices.Robot.Configuration;
using UnitySC.Equipment.Abstractions.Devices.Robot.Resources;

namespace UnitySC.Equipment.Abstractions.Devices.Controller
{
    public class IsLocationReadyForTransfer : CSharpInterlockBehavior
    {
        public override void Check(Device responsible, CommandContext context)
        {
            // This interlock checks that pick/place can be done by robot
            // Interlocks are checked for any command, so "not a robot" can be nominal case (don't add error)
            if (context.Device is not Robot.Robot robot)
            {
                return;
            }

            switch (context.Command.Name)
            {
                case nameof(IRobot.Pick):
                case nameof(IRobot.Place):
                    {
                        var isPick = context.Command.Name.Equals(nameof(IRobot.Pick), StringComparison.Ordinal);
                        var arg = context.GetArgument("arm");
                        if (arg is not RobotArm arm)
                        {
                            context.AddContextError(Messages.ArgumentNotAnArm);
                            return;
                        }

                        arg = context.GetArgument(isPick ? "sourceDevice" : "destinationDevice");
                        if (arg is not IExtendedMaterialLocationContainer locationContainer)
                        {
                            context.AddContextError(Messages.ArgumentNotAnIExtendedMaterialLocationContainer);
                            return;
                        }

                        arg = context.GetArgument(isPick ? "sourceSlot" : "destinationSlot");
                        if (arg is not byte slot)
                        {
                            context.AddContextError(Messages.ArgumentNotASlot);
                            return;
                        }

                        var error = CheckIfLocationReady(robot, arm, locationContainer, slot, isPick);
                        if (!string.IsNullOrEmpty(error))
                        {
                            context.AddContextError(error);
                        }

                        break;
                    }

                default:
                    // Interlocks are checked for any command, so "command not supported" can be nominal case (don't add error)
                    return;
            }
        }

        private string CheckIfLocationReady(
            Robot.Robot robot,
            RobotArm arm,
            IExtendedMaterialLocationContainer locationContainer,
            byte slot,
            bool isPick)
        {
            // Get appropriate data depending on specified arm
            ArmConfiguration armConfig;
            Agileo.EquipmentModeling.Material armMaterial;
            switch (arm)
            {
                case RobotArm.Arm1:
                    armConfig = robot.Configuration.UpperArm;
                    armMaterial = robot.UpperArmLocation.Material;
                    break;

                case RobotArm.Arm2:
                    armConfig = robot.Configuration.LowerArm;
                    armMaterial = robot.LowerArmLocation.Material;
                    break;

                default:
                    return string.Format(CultureInfo.InvariantCulture, Messages.ArmIsInvalid, arm);
            }

            // Location should be in condition for transfer
            // (e.g. pins up for Aligner and/or PM, X/Y table at position for PM, door open for lP...)
            if (!locationContainer.IsReadyForTransfer(
                armConfig.EffectorType, out List<string> errors, isPick ? null : armMaterial, slot))
            {
                return string.Join(Environment.NewLine, errors);
            }

            // Everything ok
            return string.Empty;
        }
    }
}
