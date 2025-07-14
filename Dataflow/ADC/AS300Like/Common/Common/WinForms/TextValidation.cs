using System;
using System.Drawing;
using System.Windows.Forms;

namespace Common.WinForms
{
    /// <summary>
    /// Validation helper for text-based controls, using background colors as validated / error signals.
    /// </summary>
    public class TextValidation<T> : TextDisplay<T>
    {
        public TextValidation(Control control, Parser<T> parser, Func<T, string> formatter, Func<T, bool> validation = null)
            :base(control, formatter)
        {
            _parser = parser;
            _validation = validation;

            Control.TextChanged += (object sender, EventArgs e) =>
            {
                if (!_softwareChange)
                {
                    try
                    {
                        //.FormatException
                        T t = Value;

                        OnUserChange?.Invoke(t);
                        Control.BackColor = Validated;
                    }
                    catch (FormatException)
                    {
                        Control.BackColor = Error;
                    }
                }
            };
        }
        readonly Parser<T> _parser;
        readonly Func<T, bool> _validation;

        /// <summary>
        /// Notification when the user changed the value, and it passed validation.
        /// </summary>
        public Action<T> OnUserChange;

        /// <summary>
        /// Get: FormatException
        /// </summary>
        public override T Value
        {
            get
            {
                //>FormatException
                T ret = _parser.Parse(Control.Text);

                if (_validation != null)
                {
                    if (!_validation(ret))
                    {
                        throw new FormatException();
                    }
                }

                return ret;
            }

            set
            {
                _softwareChange = true;

                base.Value = value;
                Control.BackColor = Validated;

                _softwareChange = false;
            }
        }

        bool _softwareChange = false;

        public Color Validated = Color.White;
        public Color Error = Color.Red;
    }
}
