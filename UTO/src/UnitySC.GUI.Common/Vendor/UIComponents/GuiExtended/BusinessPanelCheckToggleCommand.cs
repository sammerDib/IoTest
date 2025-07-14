using System;
using System.Windows.Input;

using Agileo.GUI.Commands;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Services.Icons;

namespace UnitySC.GUI.Common.Vendor.UIComponents.GuiExtended
{
    /// <summary>
    /// A command represented by a toggle button with an isChecked state.
    /// It is used to display a button with an active state unlike the classic <see cref="BusinessPanelToggleCommand"/>
    /// which is represented by two separate commands with a same icon and text.
    /// </summary>
    public class BusinessPanelCheckToggleCommand : BusinessPanelToggleCommand
    {
        public BusinessPanelCheckToggleCommand(string id, ICommand checkedCommand, ICommand uncheckedCommand, IIcon icon = null) :
            base(id, new BusinessPanelCommand("CheckedCommand", checkedCommand), new BusinessPanelCommand("UncheckedCommand", uncheckedCommand))
        {
            Icon = icon;
        }

        public BusinessPanelCheckToggleCommand(string id, Action checkedAction, Action uncheckedAction, IIcon icon = null) :
            base(id, new BusinessPanelCommand("CheckedCommand", new DelegateCommand(checkedAction)), new BusinessPanelCommand("UncheckedCommand", new DelegateCommand(uncheckedAction)))
        {
            Icon = icon;
        }
    }
}
