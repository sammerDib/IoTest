using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface.Calibration
{
    [Flags]
    public enum CalibrationFlag
    {
        //decimal                  //binary
        Calib = 0,                 // 0000
        PreAlign = 1 << 0,         // 0001
        Test = 1 << 1,             // 0010 

        PreAlignTest = PreAlign | Test  // 0011
    }
}
