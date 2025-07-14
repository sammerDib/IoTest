using System.Collections.Generic;
using System.Linq;

using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using Humanizer;

using UnitsNet;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;
using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitProcessModuleStatusThreshold
{
    public class WaitProcessModuleStatusThresholdInstruction : WaitStatusThresholdInstruction
    {
        public override string Id => nameof(WaitProcessModuleStatusThresholdInstruction).Humanize();

        protected override RecipeInstruction CloneInstruction()
            => new WaitProcessModuleStatusThresholdInstruction
            {
                Thresholds = Thresholds.Select(t => t.Clone() as Threshold).ToList(),
                TimeOut = Duration.FromSeconds(TimeOut.Value),
                IsTimeoutEnabled = IsTimeoutEnabled
            };

        public override bool Equals(RecipeInstruction other)
        {
            if (other is not WaitProcessModuleStatusThresholdInstruction instruction)
            {
                return false;
            }

            if (ReferenceEquals(instruction, this))
            {
                return true;
            }

            if (!instruction.Id.Equals(Id)
                || !instruction.TimeOut.Equals(TimeOut)
                || !instruction.Thresholds.SequenceEqual(Thresholds))
            {
                return false;
            }

            return true;
        }

        public override string LocalizationKey => nameof(ScenarioResources.SCENARIO_INSTRUCTION_WAIT_PM_STATUS);

        public override Instruction ToProcessingInstruction()
            => new WaitProcessModuleStatusThresholdProcessInstruction(App.Instance.EquipmentManager.Equipment, this)
            {
                Modifier = ExecutionModifier.Sync,
                Details = ToString(),
                FormattedLabel = FormattedLabel,
                ExecutorId = string.Empty
            };

        protected override List<AdvancedStringFormatDefinition> BuildPrettyLabel()
        {
            var thresholds = new List<Threshold>(Thresholds);
            foreach (var threshold in thresholds)
            {
                switch (threshold)
                {
                    // operand format
                    case ThresholdOperand thresholdOperand:
                        thresholdOperand.DeviceName = "Process module";
                        break;
                }
            }

            return base.BuildPrettyLabel();
        }
    }
}
