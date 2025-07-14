using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.DMT.Modules.Settings.View.Designer
{
    public interface INotify
    {
        event EventHandler DragCompleted;

        void OnDragCompleted();
 

    }
}
