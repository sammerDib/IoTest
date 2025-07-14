using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x.Popups
{
    public class EditMappingPositionPopupViewModel : NotifyDataError
    {

        static EditMappingPositionPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(EditMappingPositionPopupViewModel), typeof(EditMappingPositionPopupView));
        }

        public EditMappingPositionPopupViewModel(
            uint currentArmFirstMappingPosition = 0,
            uint currentArmSecondMappingPosition = 0)
        {
            ArmFirstMappingPosition = currentArmFirstMappingPosition;
            ArmSecondMappingPosition = currentArmSecondMappingPosition;
        }

        #region Properties

        private uint _armFirstMappingPosition;

        public uint ArmFirstMappingPosition
        {
            get => _armFirstMappingPosition;
            set => SetAndRaiseIfChanged(ref _armFirstMappingPosition, value);
        }

        private uint _armSecondMappingPosition;

        public uint ArmSecondMappingPosition
        {
            get => _armSecondMappingPosition;
            set => SetAndRaiseIfChanged(ref _armSecondMappingPosition, value);
        }

        #endregion

        #region Public

        public bool ValidateMappingPosition()
        {
            //No rules
            return true;
        }

        #endregion
    }
}
