using System;

using Agileo.Common.Localization;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation
{
    /// <summary>
    /// Determines whether or not an object satisfies a rule and
    /// provides an error if it does not.
    /// </summary>
    public sealed class DelegateRule : Rule
    {
        private readonly Func<string> _rule;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateRule"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property the rules applies to.</param>
        /// <param name="rule">The rule to execute.</param>
        public DelegateRule(string propertyName, Func<string> rule) : base(propertyName)
        {
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));
        }

        #endregion

        #region Rule Members

        /// <summary>
        /// Applies the rule to the specified object.
        /// </summary>
        /// <param name="obj">The object to apply the rule to.</param>
        /// <returns>
        /// <c>true</c> if the object satisfies the rule, otherwise <c>false</c>.
        /// </returns>
        public override string Apply(object obj) => _rule();

        #endregion

        #region Generic rules

        /// <summary>
        /// Create a rule with the following criteria: The value must be greater than or equal to the parameter provided.
        /// </summary>
        public static DelegateRule GreaterThanOrEqual<T>(string propertyName, Func<T> getValueFunc, T minValue) where T : IComparable
        {
            return new DelegateRule(propertyName,
                () =>
                {
                    var value = getValueFunc();
                    if (value.CompareTo(minValue) < 0)
                    {
                        return LocalizationManager.GetString(
                            nameof(DelegateRuleResources.RULE_ERROR_GREATER_THAN),
                            minValue);
                    }

                    return null;
                });
        }

        /// <summary>
        /// Create a rule with the following criteria: The value must be less than or equal to the parameter provided.
        /// </summary>
        public static DelegateRule LowerThanOrEqual<T>(string propertyName, Func<T> getValueFunc, T maxValue) where T : IComparable
        {
            return new DelegateRule(propertyName,
                () =>
                {
                    var value = getValueFunc();
                    if (value.CompareTo(maxValue) > 0)
                    {
                        return LocalizationManager.GetString(
                            nameof(DelegateRuleResources.RULE_ERROR_LOWER_THAN),
                            maxValue);
                    }

                    return null;
                });
        }

        /// <summary>
        /// Create a rule with the following criteria: The value must be between the two parameters provided (inclusive).
        /// </summary>
        public static DelegateRule Range<T>(string propertyName, Func<T> getValueFunc, T minValue, T maxValue) where T : IComparable
        {
            return new DelegateRule(propertyName,
                () =>
                {
                    var value = getValueFunc();
                    if (value.CompareTo(minValue) < 0 || value.CompareTo(maxValue) > 0)
                    {
                        return LocalizationManager.GetString(
                            nameof(DelegateRuleResources.RULE_ERROR_RANGE),
                            minValue,
                            maxValue);
                    }

                    return null;
                });
        }

        #endregion
    }
}
