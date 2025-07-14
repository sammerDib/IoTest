using Common.Science;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Common.WinForms
{
    /// <summary>
    /// Displays Unit.ValueStruct, using a text control and a combobox.
    /// Auto choses the best unit when the value comes from code, lets the user chose its unit when changing the value from the GUI.
    /// </summary>
    public class UnitEdit
    {
        /// <summary>
        /// Initializes the comboBox with the units.
        /// </summary>
        public UnitEdit(Control value, ComboBox unitCombo, Unit unit, Unit big = null, Unit small = null)
        {
            ValueControl = value;
            UnitCombo = unitCombo;

            if (big == null)
            {
                unit = unit.Biggest;
                _BiggestUnitIdx_s32 = 0;
            }
            else
            {
                unit = big;
                _BiggestUnitIdx_s32 = big.UnitIndex;
            }

            unitCombo.Items.Clear();
            do
            {
                unitCombo.Items.Add(unit);

                // Next smaller unit.
                unit = unit.Equals(small) ? null : unit.NextSmaller;
            } while (unit != null);

            ValueControl.TextChanged += OnChange;
            UnitCombo.SelectedIndexChanged += OnChange;
        }

        /// <summary>
        /// Index of the biggest unit in the list of all units (corresponds to index 0 in the combo box).
        /// </summary>
        Int32 _BiggestUnitIdx_s32;

        private void OnChange(object sender, EventArgs e)
        {
            if (!_softwareChange)
            {
                try
                {
                    //.FormatException
                    Unit.ValueStruct value = Value;

                    OnUserChange?.Invoke(value);
                    ValueControl.BackColor = Validated;
                }
                catch (FormatException)
                {
                    ValueControl.BackColor = Error;
                }
            }
        }

        public Control ValueControl;
        public ComboBox UnitCombo;

        /// <summary>
        /// Get: FormatException
        /// </summary>
        public Unit.ValueStruct Value
        {
            get
            {
                //>FormatException
                Unit.ValueStruct value = ((Unit)UnitCombo.SelectedItem).ParseValue(ValueControl.Text);

                if (!Validation(value))
                {
                    throw new FormatException();
                }

                return value;
            }

            set
            {
                _softwareChange = true;

                // Chose best unit.
                value = value.AutoUnit();

                ValueControl.Text = value.ValueToString(SignificantDigits);
                UnitCombo.SelectedIndex = value.Unit.UnitIndex - _BiggestUnitIdx_s32;

                ValueControl.BackColor = Validated;

                _softwareChange = false;
            }
        }

        bool _softwareChange = false;

        /// <summary>
        /// Notification when the user changed the value, and it passed validation.
        /// </summary>
        public Action<Unit.ValueStruct> OnUserChange = (Unit.ValueStruct value) => { };

        /// <summary>
        /// Only validated values are returned by Value{get}.
        /// Default = no constraint.
        /// </summary>
        public Func<Unit.ValueStruct, bool> Validation = (Unit.ValueStruct value) => { return true; };

        /// <summary>
        /// Significant digits to display.
        /// </summary>
        public Int32 SignificantDigits = 2;

        public Color Validated = Color.White;
        public Color Error = Color.Red;
    }
}
