using System;
using System.Xml.Serialization;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators
{
    [Serializable]
    public class ThresholdOperator : Threshold
    {
        [XmlElement(nameof(Operator))]
        public LogicalOperator Operator { get; set; }

        public override string PrettyLabel => string.Concat(" ", Operator, " ");

        public override bool Equals(Threshold other)
        {
            var operatorThreshold = other as ThresholdOperator;

            if (ReferenceEquals(operatorThreshold, this)) return true;
            if (operatorThreshold == null) return false;

            return operatorThreshold.Operator == Operator;
        }

        public override object Clone()
        {
            return new ThresholdOperator { Operator = Operator };
        }
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}
