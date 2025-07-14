using System.Linq;
using System.Text;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.Recipes.Components;
using Agileo.Recipes.Components.Services;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;
using UnitySC.GUI.Common.Vendor.Recipes.Resources;

namespace UnitySC.GUI.Common.Vendor.Recipes.ValidationRules
{
    public class EquipmentMustContainDevicesValidationRule : IRecipeComponentValidationRule
    {
        public string Validate(RecipeComponent recipeComponent)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var instruction in recipeComponent.Instructions)
            {
                switch (instruction)
                {
                    case DeviceCommandInstruction deviceCommandInstruction:
                        {
                            if (App.Instance.EquipmentManager.Equipment.AllDevices().All(dev => dev.Name != deviceCommandInstruction.DeviceName))
                            {
                                sb.AppendLine(
                                    LocalizationManager.GetString(
                                        nameof(RecipeValidationResources.INSTRUCTION_UNKNOWN_DEVICE),
                                        instruction.PrettyLabel,
                                        deviceCommandInstruction.DeviceName));
                            }

                            break;
                        }
                    case WaitStatusThresholdInstruction waitStatusThresholdInstruction:
                        {
                            foreach (var threshold in waitStatusThresholdInstruction.Thresholds)
                            {
                                if (!(threshold is ThresholdOperand thresholdOperand))
                                {
                                    continue;
                                }

                                if (App.Instance.EquipmentManager.Equipment.AllDevices()
                                    .All(dev => dev.Name != thresholdOperand.DeviceName))
                                {
                                    sb.AppendLine(
                                        LocalizationManager.GetString(
                                            nameof(RecipeValidationResources.INSTRUCTION_UNKNOWN_DEVICE),
                                            instruction.PrettyLabel,
                                            thresholdOperand.DeviceName));
                                }
                            }

                            break;
                        }
                }
            }

            return sb.ToString();
        }
    }
}
