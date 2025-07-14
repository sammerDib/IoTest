using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.WOTAN.Common;

namespace UnitySC.PM.WOTAN.Processing
{
    public class Aligner : IAligner
    {
        public AlignResult Align(Wafer wafer)
        {
            BareWaferAligner bareWaferAligner = new BareWaferAligner();

            AlignResult alignResult = bareWaferAligner.Align(wafer);

            return alignResult;
        }
    }
}
