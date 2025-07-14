using System.Collections.Generic;

using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Robot.BrooksRobot.Popups
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
            XPosition = 0;
            ThetaPosition = 0;
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


        private double _xPosition;

        public double XPosition
        {
            get => _xPosition;
            set => SetAndRaiseIfChanged(ref _xPosition, value);
        }

        private double _thetaPosition;

        public double ThetaPosition
        {
            get => _thetaPosition;
            set => SetAndRaiseIfChanged(ref _thetaPosition, value);
        }

        #endregion

        #region Public

        public bool ValidateTransferLocation()
        {
            if (_transferLocation.Contains(NewTransferLocation))
            {
                return false;
            }

            return !double.IsNaN(XPosition) && !double.IsNaN(ThetaPosition);
        }

        #endregion
    }
}
