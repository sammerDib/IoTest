using System.Collections.Generic;
using System.Linq;

using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.RR75x.Popups
{
    public class AddStoppingPositionPopupViewModel : NotifyDataError
    {
        #region Constructor

        static AddStoppingPositionPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(AddStoppingPositionPopupViewModel), typeof(AddStoppingPositionPopupView));
        }

        public AddStoppingPositionPopupViewModel(
            TransferLocation transferLocation,
            List<StoppingPositionModel> stoppingPositionModels,
            string currentInnerModulePosition = "",
            uint currentArmStoppingPosition = 0)
        {
            _transferLocation = transferLocation;
            _stoppingPositionModels = stoppingPositionModels;
            IsInnerModuleEditable = _transferLocation < TransferLocation.PreAlignerA;
            NewInnerModulePosition = currentInnerModulePosition;
            NewArmStoppingPosition = currentArmStoppingPosition;
        }

        #endregion

        #region Fields

        private readonly List<StoppingPositionModel> _stoppingPositionModels;
        private readonly TransferLocation _transferLocation;

        #endregion

        #region Properties

        private string _newInnerModulePosition;

        public string NewInnerModulePosition
        {
            get => _newInnerModulePosition;
            set => SetAndRaiseIfChanged(ref _newInnerModulePosition, value);
        }

        private uint _newArmStoppingPosition;

        public uint NewArmStoppingPosition
        {
            get => _newArmStoppingPosition;
            set => SetAndRaiseIfChanged(ref _newArmStoppingPosition, value);
        }

        public bool IsInnerModuleEditable { get; set; }

        #endregion

        #region Public

        public bool ValidateStoppingPosition(bool isEdit)
        {
            if (_transferLocation >= TransferLocation.PreAlignerA)
            {
                if (!string.IsNullOrEmpty(NewInnerModulePosition))
                {
                    return false;
                }

                var expectedCount = isEdit
                    ? 1
                    : 0;

                if (_stoppingPositionModels.Count != expectedCount)
                {
                    return false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(NewInnerModulePosition)
                    || NewInnerModulePosition.Length != 4)
                {
                    return false;
                }

                if (!IsInnerModuleEditable
                    && _stoppingPositionModels.FirstOrDefault(
                        m => m.InnerModulePosition == NewInnerModulePosition)
                    != null)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
