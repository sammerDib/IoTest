using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Access;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Rights
{
    public class MenuItemRightsViewModel : GraphicalElementRightsViewModel<MenuItem>
    {
        public List<CommandElementRightsViewModel> Commands { get; } = new();

        public List<MenuItemRightsViewModel> SubMenu { get; } = new();

        public MenuItemRightsViewModel(MenuItem element, IAccessManager accessManager, Action<RightViewModel> onLevelChanged) : base(element, accessManager, onLevelChanged)
        {
            switch (element)
            {
                case BusinessPanel businessPanel:
                    {
                        foreach (var command in businessPanel.Commands)
                        {
                            if (command is ApplicationCommandReference) continue;
                            Commands.Add(new CommandElementRightsViewModel(command, accessManager, OnLevelChanged));
                        }

                        break;
                    }
                case Menu menu:
                    {
                        foreach (var subMenuItem in menu.Items)
                        {
                            SubMenu.Add(new MenuItemRightsViewModel(subMenuItem, accessManager, OnLevelChanged));
                        }

                        break;
                    }
            }
        }

        protected override void OnLevelChanged(RightViewModel obj)
        {
            if (EnabledRight.HasModified || VisibilityRight.HasModified)
            {
                HasModified = true;
            }
            else if (SubMenu.Any(model => model.HasModified))
            {
                HasModified = true;
            }
            else if (Commands.Any(model => model.HasModified))
            {
                HasModified = true;
            }
            else
            {
                HasModified = false;
            }

            OnPropertyChanged(nameof(HighestLevel));

            OnLevelChangedAction(obj);
        }
    }
}
