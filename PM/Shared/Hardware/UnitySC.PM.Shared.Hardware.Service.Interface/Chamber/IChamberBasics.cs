using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chamber
{
    public interface IChamberBasics
    {
        bool CdaIsReady();
        bool IsInMaintenance();
        bool PrepareToTransferState();
    }
}
