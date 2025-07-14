using System.Linq;

using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.Saliences;

using Popup = Agileo.GUI.Services.Popups.Popup;

namespace UnitySC.GUI.Common.Vendor.Helpers
{
    public static class PopupHelper
    {
        /// <summary>
        /// Instantiate a popup with error severity.
        /// </summary>
        /// <param name="message"> Message to be displayed in popup.</param>
        /// <returns>The instance of Popup with Error severity.</returns>
        public static Popup Error(string message)
        {
            return new Popup(PopupButtons.OK, "Error", message)
            {
                SeverityLevel = MessageLevel.Error
            };
        }

        public static void ShowPopupWithSalience(BusinessPanel businessPanel, Popup popup, SalienceType salienceType = SalienceType.UserAttention)
        {
            if (businessPanel == null)
            {
                return;
            }
            
            businessPanel.Saliences.Add(salienceType);
            businessPanel.Popups.ShowAndWaitResult(popup);
            businessPanel.Saliences.Remove(salienceType);
        }

        public static void ShowPopupWithSalience(string panelRelativeId, Popup popup, SalienceType salienceType = SalienceType.UserAttention)
        {
            var businessPanel = AgilControllerApplication.Current.UserInterface.BusinessPanels.FirstOrDefault(p => p.RelativeId == panelRelativeId);

            ShowPopupWithSalience(businessPanel,popup, salienceType);
        }
    }
}
