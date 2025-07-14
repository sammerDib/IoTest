using System.Windows.Input;

using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended
{
    /// <summary>
    /// A command that is not displayed on the right side of the businessPanel.
    /// It is used to obtain an element linked to access rights and configurable from the access rights panel.
    /// Use the <see cref="GraphicalElement.IsEnabled"/> property to condition the accessibility of your graphical elements and directly bind this <see cref="InvisibleBusinessPanelCommand"/> as an <see cref="ICommand"/> on your buttons.
    /// </summary>
    public class InvisibleBusinessPanelCommand : BusinessPanelCommand
    {
        public InvisibleBusinessPanelCommand(string id, ICommand command, IIcon icon = null) : base(id, command, icon)
        {
        }
    }
}
