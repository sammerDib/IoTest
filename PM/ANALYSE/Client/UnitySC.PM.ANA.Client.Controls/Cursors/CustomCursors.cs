using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Resources;

namespace UnitySC.PM.ANA.Client.Controls
{
    public static class CustomCursors
    {
        public static Cursor Cross;

        static CustomCursors()
        {
            StreamResourceInfo crossCurs = Application.GetResourceStream(new Uri("pack://application:,,,/UnitySC.PM.ANA.Client.Controls;Component/Cursors/Cross.cur"));
            Cross = new Cursor(crossCurs.Stream);
        }
    }
}
