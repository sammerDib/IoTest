using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using UnitySC.Shared.UI.AutoRelayCommandExt;

using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Main

{
    public class MainMenuViewModel : ObservableObject
    {
        public List<GroupViewModel> Groups { get; private set; }
        private ExternalUserControls _externalMenuItem;
        public MainMenuViewModel()
        {
            _externalMenuItem = ClassLocator.Default.GetInstance<ExternalUserControls>();
            _externalMenuItem.Init();
        }

        public void Refresh(ApplicationMode mode, UnifiedUser user)
        {
           Groups = new List<GroupViewModel>();

            if (user != null)
            {

                var menus = _externalMenuItem.MenuItems
                    .Select(x => x.Value)
                    .Where(item => item.RequiredRights.Any(j => user.Rights.Contains(j)))
                    .Where(x => x.CompatibleWith.Contains(mode))
                    .OrderBy(x => x.Priority);
                foreach (var menuItem in menus)
                {
                    var group = Groups.SingleOrDefault(x => x.Name == menuItem.Group);
                    if (group == null)
                    {
                        group = new GroupViewModel() { Name = menuItem.Group, MenuItems = new List<IMenuItem>() };
                        Groups.Add(group);
                    }
                    group.MenuItems.Add(menuItem);
                    menuItem.ApplicationModeChange(mode);
                }
            }
            OnPropertyChanged(nameof(Groups));
        }

        internal bool CanClose()
        {
            bool canClose = true;
            foreach (var recipeEditor in _externalMenuItem.RecipeEditors)
            {
                if (!recipeEditor.Value.CanClose())
                    canClose = false;
            }

            foreach (var menuitem in _externalMenuItem.MenuItems)
            {
                if(menuitem.Value == null)
                {
                    continue;
                }
                if (!menuitem.Value.CanClose())
                {
                    canClose = false;
                }
            }


            return canClose;
        }
    }
}
