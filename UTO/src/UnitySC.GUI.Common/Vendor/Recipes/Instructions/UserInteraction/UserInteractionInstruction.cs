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

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.UserInteraction
{
    public class UserInteractionInstruction : BaseRecipeInstruction
    {
        #region Properties

        public string Message { get; set; }

        public double? TimeOut { get; set; }

        public UserInteractionCommands Commands { get; set; }

        #endregion

        protected override RecipeInstruction CloneInstruction()
        {
            return new UserInteractionInstruction
            {
                Message = Message,
                TimeOut = TimeOut,
                Commands = Commands
            };
        }

        public override bool Equals(RecipeInstruction other)
        {
            var instruction = other as UserInteractionInstruction;
            if (instruction == null) return false;

            if (ReferenceEquals(instruction, this)) return true;

            return Equals(Message, instruction.Message) &&
                   Equals(TimeOut, instruction.TimeOut) &&
                   Commands == instruction.Commands;
        }

        public override string Id => nameof(UserInteractionInstruction).Humanize();

        public override string PrettyLabel => String.Format(ScenarioResources.SCENARIO_USERINTERACTIONLABEL,
            Message,
            Commands == UserInteractionCommands.AbortContinue ? ScenarioResources.SCENARIO_ABORTORCONTINUE : ScenarioResources.SCENARIO_CONTINUEINSTRUCTION,
            TimeoutLabel);

        private string TimeoutLabel => TimeOut.HasValue ? String.Format(ScenarioResources.SCENARIO_TIMEOUT, TimeOut.Value) : string.Empty;


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
                    },
                    new AdvancedStringFormatDefinition(ScenarioResources.SCENARIO_ANDWAITING),
                    new AdvancedStringFormatDefinition(Commands == UserInteractionCommands.AbortContinue ? ScenarioResources.SCENARIO_ABORTORCONTINUE : ScenarioResources.SCENARIO_CONTINUEINSTRUCTION)
                    {
                        Bold = true,
                        Highlighted = true
                    },
                    new AdvancedStringFormatDefinition(String.Format(ScenarioResources.SCENARIO_FOROPERATOR, TimeoutLabel)),
                };
            }
        }

        public override string LocalizationKey => nameof(ScenarioResources.SCENARIO_INSTRUCTION_INTERACTION);

        public override Instruction ToProcessingInstruction()
        {
            TimeSpan? delay = null;
            if (TimeOut.HasValue)
            {
                delay = TimeSpan.FromSeconds(TimeOut.Value);
            }
            return new UserInteractionProcessInstruction(Message, Commands, delay)
            {
                ExecutorId = "",
                Modifier = ExecutionModifier.Sync,
                Details = ToString(),
                FormattedLabel = FormattedLabel
            };
        }
    }
}
