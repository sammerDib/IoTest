using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using UnitySC.PM.ANA.Service.Interface.Algo;

namespace UnitySC.PM.ANA.Client.Controls.WaferMap
{
    public class DiePositionPixels
    {
        public DieIndex Position { get; set; }
        public Rect DieRect { get; internal set; }
    }
}
