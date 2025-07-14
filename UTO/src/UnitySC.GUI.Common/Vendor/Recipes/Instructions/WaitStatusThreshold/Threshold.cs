using System;
using System.Xml.Serialization;

using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operand;
using UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold.Operators;

namespace UnitySC.GUI.Common.Vendor.Recipes.Instructions.WaitStatusThreshold
{
    [Serializable]
    [XmlInclude(typeof(QuantityThresholdOperand))]
    [XmlInclude(typeof(BooleanThresholdOperand))]
    [XmlInclude(typeof(EnumerableThresholdOperand))]
    [XmlInclude(typeof(NumericThresholdOperand))]
    [XmlInclude(typeof(StringThresholdOperand))]
    [XmlInclude(typeof(ThresholdOperator))]
    [XmlRoot(nameof(Threshold))]
    public abstract class Threshold : IEquatable<Threshold>, ICloneable
    {
        [XmlIgnore]
        public abstract string PrettyLabel { get; }

        public abstract bool Equals(Threshold other);

        public abstract object Clone();
    }
}
