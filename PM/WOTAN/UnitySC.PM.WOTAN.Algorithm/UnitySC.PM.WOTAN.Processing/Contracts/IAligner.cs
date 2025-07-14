using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitySC.PM.WOTAN.Common;

namespace UnitySC.PM.WOTAN.Processing
{
    public interface IAligner
    {
        AlignResult Align(Wafer wafer);
    }
}
