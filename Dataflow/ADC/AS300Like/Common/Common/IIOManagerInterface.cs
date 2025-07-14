using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum IOControllerStatus
    {
        Ready,
        Error,
        UNKNOWN,
    };
    public interface IIOManagerObserver
    {
        void UpdateInputs(CIOManagerMessage Message);
    }
}
