using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UnitySC.PM.Shared.UI.Main
{
    public class GroupViewModel:ObservableRecipient
    {
       public string Name { get; set; }

        public List<IMenuItem> MenuItems { get; set; }
    }
}
