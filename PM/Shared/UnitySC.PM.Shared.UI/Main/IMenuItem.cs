using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data;

namespace UnitySC.PM.Shared.UI.Main
{
    [InheritedExport(typeof(IMenuItem))]
    public interface IMenuItem
    {
        string Name { get; }
        string Description { get; }
        string Group { get; }
        string ImageResourceKey { get; }
        IMenuContentViewModel ViewModel { get; }
        UserControl UserControl { get; }
        /// <summary>
        /// Priroty : Convention abc : a => Colonne, b => ligne , c => reserved for new menu
        /// </summary>
        int Priority { get; }

        bool CanClose();

        IEnumerable<ApplicationMode> CompatibleWith { get; }
        void ApplicationModeChange(ApplicationMode newMode);

        IEnumerable<UserRights> RequiredRights { get; }
    }
}
