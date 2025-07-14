using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.DMT.CommonUI.ViewModel
{
    public interface ITabManager
    {
        void Display();
        bool CanHide();
        void Hide();
    }
}
