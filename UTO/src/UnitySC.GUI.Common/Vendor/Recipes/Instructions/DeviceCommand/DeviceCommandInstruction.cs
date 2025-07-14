using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using Agileo.EquipmentModeling;
using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using Humanizer;

using Newtonsoft.Json;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.Parameters;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand.ValidationRules;
using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.DeviceCommand
{
    [Serializable]
    public class DeviceCommandInstruction : BaseRecipeInstruction
    {
        private string _deviceName;
        private int _deviceInstanceId;
        private string _commandName;
        private List<DeviceCommandParameter> _commandParameters = new List<DeviceCommandParameter>();
        private bool _isTimeoutEnabled;
        private long _timeout;

        public DeviceCommandInstruction()
        {
            ValidationRules.Add(new DeviceCommandInstructionValidationRule());
        }

        [XmlElement(nameof(DeviceName))]
        public string DeviceName
        {
            get
            {
                return _deviceName;
            }
            set
            {
                if (string.Equals(_deviceName, value, StringComparison.Ordinal))
                {
                    return;
                }

                _deviceName = value;
                OnPropertyChanged();
            }
        }

        [XmlElement(nameof(DeviceInstanceId))]
        public int DeviceInstanceId
        {
            get
            {
                return _deviceInstanceId;
            }
            set
            {
                if (_deviceInstanceId == value)
                {
                    return;
                }

                _deviceInstanceId = value;
                OnPropertyChanged();
            }
        }

        [XmlElement(nameof(CommandName))]
        public string CommandName
        {
            get
            {
                return _commandName;
            }
            set
            {
                if (string.Equals(_commandName, value, StringComparison.Ordinal))
                {
                    return;
                }

                _commandName = value;
                OnPropertyChanged();
            }
        }

        [XmlArray(nameof(CommandParameters), IsNullable = true)]
        [XmlArrayItem("Parameter")]
        public List<DeviceCommandParameter> CommandParameters
        {
            get
            {
                return _commandParameters;
            }
            set
            {
                _commandParameters = value;
                OnPropertyChanged();
            }
        }

        [XmlElement(nameof(IsTimeoutEnabled))]
        public bool IsTimeoutEnabled
        {
            get
            {
                return _isTimeoutEnabled;
            }
            set
            {
                if (_isTimeoutEnabled == value)
                {
                    return;
                }

                _isTimeoutEnabled = value;
                OnPropertyChanged();
            }
        }

        [XmlElement(nameof(Timeout))]
        public long Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                if (_timeout == value)
                {
                    return;
                }

                _timeout = value;
                OnPropertyChanged();
            }
        }

        protected override RecipeInstruction CloneInstruction()
        {
            return new DeviceCommandInstruction
            {
                DeviceName = DeviceName,
                DeviceInstanceId = DeviceInstanceId,
                CommandName = CommandName,
                CommandParameters = CommandParameters.Select(cp => cp.Clone() as DeviceCommandParameter).ToList(),
                IsTimeoutEnabled = IsTimeoutEnabled,
                Timeout = Timeout
            };
        }

        public override bool Equals(RecipeInstruction other)
        {
            var instruction = other as DeviceCommandInstruction;
            if (instruction == null)
            {
                return false;
            }

            return DeviceName == instruction.DeviceName
                   && DeviceInstanceId == instruction.DeviceInstanceId
                   && CommandName == instruction.CommandName
                   && CommandParameters.Count == instruction.CommandParameters.Count
                   && CommandParameters.SequenceEqual(instruction.CommandParameters)
                   && IsTimeoutEnabled == instruction.IsTimeoutEnabled
                   && Timeout == instruction.Timeout;
        }

        public override string Id => nameof(DeviceCommandInstruction).Humanize();

        public override string PrettyLabel => string.Concat(FormattedLabel.Select(fl => fl.Text));

        #region Overrides of BaseRecipeInstruction

        [XmlIgnore]
        [JsonIgnore]
        public override List<AdvancedStringFormatDefinition> FormattedLabel
        {
            get
            {
                var list = new List<AdvancedStringFormatDefinition>
                {
                    new AdvancedStringFormatDefinition($"[{DeviceName}] {CommandName?.Humanize()}")
                    {
                        Bold = true,
                        Highlighted = true
                    }
                };

                if (CommandParameters.Any())
                {
                    for (var index = 0; index < CommandParameters.Count; index++)
                    {
                        var commandParameter = CommandParameters[index];

                        var commandLabel = $"{commandParameter.Name.Humanize()}: ";

                        list.Add(new AdvancedStringFormatDefinition(index == 0 ? $" ({commandLabel}" : commandLabel));
                        list.Add(new AdvancedStringFormatDefinition(commandParameter.ValueAsString()) { Highlighted = true });
                        list.Add(new AdvancedStringFormatDefinition(index < CommandParameters.Count - 1 ? ", " : ")"));
                    }
                }

                if (IsTimeoutEnabled)
                {
                    list.Add(new AdvancedStringFormatDefinition($" (Timeout: {Timeout} seconds)"));
                }

                return list;
            }
        }

        public override string LocalizationKey => nameof(ScenarioResources.SCENARIO_INSTRUCTION_COMMAND);

        public override Instruction ToProcessingInstruction()
        {
            var device = App.Instance.EquipmentManager.Equipment.AllDevices()
                .FirstOrDefault(d => d.Name.Equals(DeviceName) && d.InstanceId == DeviceInstanceId);

            return new DeviceCommandProcessInstruction(device, this)
            {
                ExecutorId = device?.Name,
                Modifier = ExecutionModifier.Sync,
                Details = ToString(),
                FormattedLabel = FormattedLabel
            };
        }

        #endregion
    }
}
