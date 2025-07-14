using Agileo.GUI.Components;
using Agileo.Semi.Gem300.Abstractions.E87;

using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Equipment.LoadPort.Popup
{
    public class SetAccessModePopup : Notifier
    {
        #region Constructor

        static SetAccessModePopup()
        {
            DataTemplateGenerator.Create(typeof(SetAccessModePopup), typeof(SetAccessModePopupView));
        }

        #endregion

        #region Properties

        private AccessMode _accessMode;

        public AccessMode AccessMode
        {
            get => _accessMode;
            set
            {
                SetAndRaiseIfChanged(ref _accessMode, value);
            }
        }

        #endregion
    }
}
