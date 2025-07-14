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

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInformation
{
    public class UserInformationInstruction : BaseRecipeInstruction
    {
        public string Message { get; set; }

        public double? AutoHideDelay { get; set; }

        public override string Id => nameof(UserInformationInstruction).Humanize();

        public override string PrettyLabel => String.Format(ScenarioResources.SCENARIO_DISPLAYMESSAGE, Message);

        protected override RecipeInstruction CloneInstruction()
        {
            return new UserInformationInstruction
            {
                Message = Message,
                AutoHideDelay = AutoHideDelay
            };
        }

        public override bool Equals(RecipeInstruction other)
        {
            var instruction = other as UserInformationInstruction;
            if (instruction == null) return false;

            if (ReferenceEquals(instruction, this)) return true;

            return Equals(Message, instruction.Message) && AutoHideDelay.Equals(instruction.AutoHideDelay);
        }

        [XmlIgnore]
        [JsonIgnore]
        public override List<AdvancedStringFormatDefinition> FormattedLabel
        {
            get
            {
                return new List<AdvancedStringFormatDefinition>
                {
                    new AdvancedStringFormatDefinition(ScenarioResources.SCENARIO_DISPLAY),
                    new AdvancedStringFormatDefinition(Message)
                    {
                        Bold = true,
                        Highlighted = true
                    }
                };
            }
        }

        public override string LocalizationKey => nameof(ScenarioResources.SCENARIO_INSTRUCTION_MESSAGE);

        public override Instruction ToProcessingInstruction()
        {
            TimeSpan? autoHideDelay = null;
            if (AutoHideDelay.HasValue)
            {
                autoHideDelay = TimeSpan.FromSeconds(AutoHideDelay.Value);
            }

            return new UserInformationProcessInstruction(Message, autoHideDelay)
            {
                ExecutorId = "",
                Modifier = ExecutionModifier.Sync,
                Details = ToString(),
                FormattedLabel = FormattedLabel
            };
        }
    }
}
