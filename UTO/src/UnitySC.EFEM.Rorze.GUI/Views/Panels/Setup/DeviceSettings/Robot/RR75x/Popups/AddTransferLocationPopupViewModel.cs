using System.Collections.Generic;

using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.RR75x.Popups
{
    public class AddTransferLocationPopupViewModel : NotifyDataError
    {
        #region Constructor

        static AddTransferLocationPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(AddTransferLocationPopupViewModel), typeof(AddTransferLocationPopupView));
        }


        public AddTransferLocationPopupViewModel(List<TransferLocation> transferLocation)
        {
            _transferLocation = transferLocation;
            NewTransferLocation = TransferLocation.LoadPort1;
        }

        #endregion

        #region Fields

        private readonly List<TransferLocation> _transferLocation;

        #endregion

        #region Properties

        private TransferLocation _newTransferLocation;

        public TransferLocation NewTransferLocation
        {
            get => _newTransferLocation;
            set => SetAndRaiseIfChanged(ref _newTransferLocation, value);
        }

        #endregion

        #region Public

        public bool ValidateTransferLocation()
        {
            if (_transferLocation.Contains(NewTransferLocation))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
