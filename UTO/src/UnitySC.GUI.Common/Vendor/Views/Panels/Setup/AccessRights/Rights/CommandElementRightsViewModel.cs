using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Access;
using Agileo.GUI.Components.Commands;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Setup.AccessRights.Rights
{
    public class CommandElementRightsViewModel : GraphicalElementRightsViewModel<BusinessPanelCommandElement>
    {
        public CommandElementRightsViewModel(BusinessPanelCommandElement element, IAccessManager accessManager, Action<RightViewModel> onLevelChanged) : base(element, accessManager, onLevelChanged)
        {
            if (element is BusinessPanelCommandGroup group)
            {
                foreach (var subCommand in group.Commands)
                {
                    if (subCommand is ApplicationCommandReference) continue;
                    Commands.Add(new CommandElementRightsViewModel(subCommand, accessManager, OnLevelChanged));
                }
            }
        }

        public List<CommandElementRightsViewModel> Commands { get; set; } = new();

        protected override void OnLevelChanged(RightViewModel obj)
        {
            if (EnabledRight.HasModified || VisibilityRight.HasModified)
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
