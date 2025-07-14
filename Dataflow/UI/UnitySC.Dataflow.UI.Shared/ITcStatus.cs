using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.Dataflow.UI.Shared
{
    public interface ITcStatus
    {
        bool SomeControlJobsAreExecuting { get; }  
    }
}
