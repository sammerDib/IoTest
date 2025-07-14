using Common.Science;
using System;
using System.Windows.Forms;

namespace Common.WinForms
{
    public class UnitDisplay : TextDisplay<Unit.ValueStruct>
    {
        public UnitDisplay(Control control, Int32 significantDigits = 2)
            : base(control, (Unit.ValueStruct value) =>
            {
                return value.AutoUnit().ToString(significantDigits);
            })
        { }
    }
}
