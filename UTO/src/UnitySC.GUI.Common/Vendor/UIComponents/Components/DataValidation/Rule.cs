using System;

namespace UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation
{
    public abstract class Rule
    {
        /// <summary>
        /// Gets the name of the property this instance applies to.
        /// </summary>
        /// <value>The name of the property this instance applies to.</value>
        public string PropertyName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="propertyName">The name of the property this instance applies to.</param>
        protected Rule(string propertyName)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public abstract string Apply(object obj);
    }
}
