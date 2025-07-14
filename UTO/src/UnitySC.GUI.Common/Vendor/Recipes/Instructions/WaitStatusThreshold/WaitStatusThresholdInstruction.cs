using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;

using Agileo.ProcessingFramework.Instructions;
using Agileo.Recipes.Components;

using Humanizer;

using Newtonsoft.Json;

using UnitsNet;

using UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;
using UnitySC.GUI.Common.Vendor.UIComponents.Behaviors;
using UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold
{
    [Serializable]
    [XmlInclude(typeof(QuantityThresholdOperand))]
    [XmlInclude(typeof(BooleanThresholdOperand))]
    [XmlInclude(typeof(EnumerableThresholdOperand))]
    [XmlInclude(typeof(NumericThresholdOperand))]
    [XmlInclude(typeof(StringThresholdOperand))]
    [XmlInclude(typeof(ThresholdOperator))]
    public class WaitStatusThresholdInstruction : BaseRecipeInstruction
    {
        #region Constructor

        #endregion

        #region Properties

        [XmlArray(nameof(Thresholds))]
        [XmlArrayItem(nameof(Threshold), typeof(Threshold))]
        public List<Threshold> Thresholds { get; set; } = new List<Threshold>();

        [XmlElement(nameof(TimeOut))]
        public string TimeOutAsString
        {
            get;
            set;
        } = Duration.FromSeconds(0).ToString(CultureInfo.InvariantCulture);

        [XmlIgnore]
        public Duration TimeOut
        {
            get
            {
                return Duration.Parse(TimeOutAsString, SerializationCulture);
            }
            set
            {
                TimeOutAsString = value.ToString(SerializationCulture);
                OnPropertyChanged(nameof(TimeOut));
            }
        }

        private bool _isTimeoutEnabled;

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

        #endregion Properties

        #region Override

        public override string Id => nameof(WaitStatusThresholdInstruction).Humanize();

        public override string PrettyLabel => string.Concat(FormattedLabel.Select(fl => fl.Text));

        protected override RecipeInstruction CloneInstruction()
        {
            return new WaitStatusThresholdInstruction
            {
                Thresholds = Thresholds.Select(t => t.Clone() as Threshold).ToList(),
                TimeOut = Duration.FromSeconds(TimeOut.Value),
                IsTimeoutEnabled = IsTimeoutEnabled
            };
        }

        public override bool Equals(RecipeInstruction other)
        {
            var instruction = other as WaitStatusThresholdInstruction;
            if (instruction == null) return false;
            if (ReferenceEquals(instruction, this)) return true;

            if (!instruction.Id.Equals(Id)
                || !instruction.TimeOut.Equals(TimeOut)
                || !instruction.Thresholds.SequenceEqual(Thresholds)) return false;

            return true;
        }

        [XmlIgnore]
        [JsonIgnore]
        public override List<AdvancedStringFormatDefinition> FormattedLabel
        {
            get
            {
                var formattedLabel = new List<AdvancedStringFormatDefinition>();

                if (Thresholds.Count != 0)
                {
                    formattedLabel.Add(new AdvancedStringFormatDefinition(ScenarioResources.SCENARIO_WAITFORMATTED));
                }

                formattedLabel.AddRange(BuildPrettyLabel());

                if (IsTimeoutEnabled)
                {
                    formattedLabel.Add(new AdvancedStringFormatDefinition(String.Format(ScenarioResources.SCENARIO_TIMEOUTSECONDS, TimeOut.Seconds)));
                }

                return formattedLabel;
            }
        }

        #endregion Override

        #region private

        protected virtual List<AdvancedStringFormatDefinition> BuildPrettyLabel()
        {
            var formattedThresholds = new List<AdvancedStringFormatDefinition>();

            // Format all thresholds
            Thresholds.ToList().ForEach(threshold => FormatThreshold(threshold, formattedThresholds));

            // If there are 5 thresholds, there are two logical operators.
            // So, we need to manage operator priority and add brackets.
            if (Thresholds?.Count == 5)
            {
                var thresholdOperators = Thresholds.OfType<ThresholdOperator>().ToList();
                var onlyOneOperatorType = thresholdOperators.All(op => op.Operator == LogicalOperator.And)
                                          || thresholdOperators.All(op => op.Operator == LogicalOperator.Or);

                // If first logical operator is AND, add brackets like this
                // (Threshold1 AND Threshold2) OR Threshold3
                if (!onlyOneOperatorType && thresholdOperators.First().Operator == LogicalOperator.And)
                {
                    formattedThresholds.Insert(0, new AdvancedStringFormatDefinition("( "));
                    formattedThresholds.Insert(8, new AdvancedStringFormatDefinition(" )"));
                }

                // If first logical operator is OR, add brackets like this
                // Threshold1 OR (Threshold2 AND Threshold3)
                else if (!onlyOneOperatorType && thresholdOperators.First().Operator == LogicalOperator.Or)
                {
                    formattedThresholds.Insert(4, new AdvancedStringFormatDefinition("( "));
                    formattedThresholds.Insert(12, new AdvancedStringFormatDefinition(" )"));
                }
            }

            if (Thresholds?.Count > 5)
            {
                throw new NotSupportedException("For now, thresholds count is limited to 5.");
            }

            return formattedThresholds;
        }

        private void FormatThreshold(Threshold notFormattedThreshold, List<AdvancedStringFormatDefinition> formattedThresholds)
        {
            if (notFormattedThreshold == null)
            {
                throw new ArgumentNullException(nameof(notFormattedThreshold));
            }

            if (formattedThresholds == null)
            {
                throw new ArgumentNullException(nameof(formattedThresholds));
            }

            switch (notFormattedThreshold)
            {
                // operand format
                case ThresholdOperand thresholdOperand:
                    formattedThresholds.Add(new AdvancedStringFormatDefinition($"{thresholdOperand.HumanizedStatusName}")
                    {
                        Bold = true,
                        Highlighted = true
                    });
                    formattedThresholds.Add(new AdvancedStringFormatDefinition($" {thresholdOperand.WaitingOperator.ToHumanizedString()}"));
                    formattedThresholds.Add(new AdvancedStringFormatDefinition($" {thresholdOperand.ValueAsString()}")
                    {
                        Highlighted = true
                    });
                    break;

                // operator format
                case ThresholdOperator thresholdOperator:
                    formattedThresholds.Add(new AdvancedStringFormatDefinition($"{thresholdOperator.PrettyLabel.ToLowerInvariant()}"));
                    break;
            }
        }

        #endregion private

        public override string LocalizationKey => nameof(ScenarioResources.SCENARIO_INSTRUCTION_WAIT_STATUS);

        public override Instruction ToProcessingInstruction()
        {
            return new WaitStatusThresholdProcessInstruction(App.Instance.EquipmentManager.Equipment, this)
            {
                Modifier = ExecutionModifier.Sync,
                Details = ToString(),
                FormattedLabel = FormattedLabel,
                ExecutorId = ""
            };
        }
    }
}
