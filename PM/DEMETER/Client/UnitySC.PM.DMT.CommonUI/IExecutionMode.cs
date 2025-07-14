using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.DMT.CommonUI
{

    public enum ExecutionModes
    {
        StandAlone,
        Integrated
    }

    public interface IExecutionMode
    {
        ExecutionModes CurrentExecutionMode { get; set; }
    }
}
