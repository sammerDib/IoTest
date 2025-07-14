using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation
{
    /// <summary>
    /// A collection of rules.
    /// </summary>
    public sealed class RuleCollection : Collection<Rule>
    {
        #region Public Methods

        /// <summary>
        /// Applies the <see cref="Rule"/>'s contained in this instance to <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object to apply the rules to.</param>
        /// <param name="propertyName">Name of the property we want to apply rules for. <c>null</c>
        /// to apply all rules.</param>
        /// <returns>A collection of errors.</returns>
        public IEnumerable<string> Apply(object obj, string propertyName)
        {
            return this.Where(r => string.IsNullOrEmpty(propertyName) || r.PropertyName.Equals(propertyName))
                .Select(r => r.Apply(obj))
                .Where(error => !string.IsNullOrWhiteSpace(error));
        }

        #endregion
    }
}
