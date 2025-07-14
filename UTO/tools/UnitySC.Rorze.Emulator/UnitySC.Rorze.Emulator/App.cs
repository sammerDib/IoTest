using System;
using System.Windows.Forms;
using System.IO.Ports;

namespace UnitySC.Rorze.Emulator
{
    public class App
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new EquipmentTestForm());

        }
    }
}
