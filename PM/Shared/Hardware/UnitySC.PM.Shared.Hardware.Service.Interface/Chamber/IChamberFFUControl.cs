using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    public interface IChamberFFUControl
    {
        void TurnOnFFU();
        void TurnOffFFU();
        bool GetFFUErrorState();
        bool FFUState();

    }
}
