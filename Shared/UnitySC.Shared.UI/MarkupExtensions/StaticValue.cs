using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace UnitySC.Shared.UI.MarkupExtensions
{
    public class StaticValue : Binding
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticValue"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public StaticValue(object value)
        {
            this.Value = value;
            this.Source = this;
            this.Path = new PropertyPath("Value");
        }

        public StaticValue()
        {
            this.Source = this;
            this.Path = new PropertyPath("Value");
        }

        #endregion Constructors and Destructors

        #region Public Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [DefaultValue(null)]
        public object Value { get; set; }

        #endregion Public Properties
    }
}