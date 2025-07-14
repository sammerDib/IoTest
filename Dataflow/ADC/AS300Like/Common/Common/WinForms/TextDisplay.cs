using System;
using System.Windows.Forms;

namespace Common.WinForms
{
    /// <summary>
    /// Displays a value in a text control, using a formatter.
    /// </summary>
    public class TextDisplay<T>
    {
        public TextDisplay(Control control, Func<T, string> formatter)
        {
            Control = control;
            _formatter = formatter;
        }
        readonly Func<T, string> _formatter;

        /// <summary>
        /// Displays the given value.
        /// </summary>
        public virtual T Value
        {
            get
            {
                return _value;
            }

            set
            {
                _value = value;
                Control.Text = _formatter(value);
            }
        }
        T _value;

        /// <summary>
        /// The associated control.
        /// </summary>
        public readonly Control Control;
    }
}
