using System.Collections.Generic;

using UnitySC.Equipment.Abstractions.Devices.LoadPort.Configuration;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Equipment.LoadPort.Popup
{
    public class SetCarrierTypePopup
    {
        static SetCarrierTypePopup()
        {
            DataTemplateGenerator.Create(typeof(SetCarrierTypePopup), typeof(SetCarrierTypePopupView));
        }
        public SetCarrierTypePopup(List<CarrierType> availableProfiles)
        {
            AvailableProfiles = availableProfiles;
        }

        #region Properties

        public List<CarrierType> AvailableProfiles { get; }

        public CarrierType SelectedCarrierType { get; set; }

        #endregion
    }
}
