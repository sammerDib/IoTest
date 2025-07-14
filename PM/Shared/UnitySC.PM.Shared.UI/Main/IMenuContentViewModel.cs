using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.Shared.UI.Main
{
    public interface IMenuContentViewModel
    {
        void Refresh();

        bool CanClose();
        
        bool IsEnabled { get; }
    }
}
