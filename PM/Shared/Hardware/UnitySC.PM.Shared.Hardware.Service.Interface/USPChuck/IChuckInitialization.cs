using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.Hardware.Service.Interface.Chuck
{    public interface IChuckInitialization
    {
        /// <summary>
        /// Initialize Wafer Stage through ACSController
        /// </summary>
        void InitWaferStage();
    }
}
