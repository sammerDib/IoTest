using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Brooks.GUI.Views.Panels.Setup.DeviceSettings.Robot.BrooksRobot.Popups
{
    public class EditPositionPopupViewModel : NotifyDataError
    {
        #region Constructor

        static EditPositionPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(EditPositionPopupViewModel), typeof(EditPositionPopupView));
        }

        public EditPositionPopupViewModel(double currentXPosition = 0, double currentThetaPosition = 0)
        {
            XPosition = currentXPosition;
            ThetaPosition = currentThetaPosition;
        }

        #endregion

        #region Properties

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

        public bool ValidateStoppingPosition()
        {
            return !double.IsNaN(XPosition) && !double.IsNaN(ThetaPosition);
        }

        #endregion
    }
}
