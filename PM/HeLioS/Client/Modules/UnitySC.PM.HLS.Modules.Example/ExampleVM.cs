using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.UI.Main;

namespace UnitySC.PM.HLS.Modules.Example
{
    public class ExampleVM : IMenuContentViewModel
    {
        public bool IsEnabled => true;

        public bool CanClose()
        {
            return true;
        }

        public void Refresh()
        {
            // Nothing
        }
    }
}
