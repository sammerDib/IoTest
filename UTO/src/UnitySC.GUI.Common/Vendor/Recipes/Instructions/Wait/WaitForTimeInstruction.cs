using System;
using System.Collections.Generic;
using System.Xml.Serialization;

using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using Humanizer;

using Newtonsoft.Json;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.Wait
{
    public class WaitForTimeInstruction : BaseRecipeInstruction
    {
        public double WaitingTime { get; set; }

        public override string Id => nameof(WaitForTimeInstruction).Humanize();

        public override string PrettyLabel => String.Format(ScenarioResources.SCENARIO_WAIT, WaitingTime);

        protected override RecipeInstruction CloneInstruction()
        {
            return new WaitForTimeInstruction
            {
                WaitingTime = WaitingTime
            };
        }

        public override bool Equals(RecipeInstruction other)
        {
            var instruction = other as WaitForTimeInstruction;
            if (instruction == null)
            {
                return false;
            }

            if (ReferenceEquals(instruction, this))
            {
                return true;
            }

            if (!instruction.Id.Equals(Id)
                || !instruction.WaitingTime.Equals(WaitingTime))
            {
                return false;
            }

            return true;
        }

        [XmlIgnore]
        [JsonIgnore]
        public override List<AdvancedStringFormatDefinition> FormattedLabel
        {
            get
            {
                return new List<AdvancedStringFormatDefinition>
                {
                    new AdvancedStringFormatDefinition(ScenarioResources.SCENARIO_WAITFORMATTED),
                    new AdvancedStringFormatDefinition($"{WaitingTime}")
                    {
                        Bold = true,
                        Highlighted = true
                    },
                    new AdvancedStringFormatDefinition(ScenarioResources.SCENARIO_SECONDS)
                };
            }
        }

        public override string LocalizationKey => nameof(ScenarioResources.SCENARIO_INSTRUCTION_WAIT_TIME);

        public override Instruction ToProcessingInstruction()
        {
            return new WaitProcessInstruction(TimeSpan.FromSeconds(WaitingTime))
            {
                ExecutorId = "",
                Modifier = ExecutionModifier.Sync,
                Details = ToString(),
                FormattedLabel = FormattedLabel
            };
        }
    }
}
