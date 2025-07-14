using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.Shared.UI.Main;

namespace UnitySC.PM.AGS.Modules.TestHardware.RecipeExecutor
{
    public class RecipeExecutorViewModel:IMenuContentViewModel
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
