using System.Collections.Generic;

using Agileo.SemiDefinitions;

using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.Robot.MapperRR75x.Popups
{
    public enum MappingLocation
    {
        LoadPort1 = 1,
        LoadPort2 = 2,
        LoadPort3 = 3,
        LoadPort4 = 4,
        LoadPort5 = 5,
        LoadPort6 = 6,
        LoadPort7 = 7,
        LoadPort8 = 8,
        LoadPort9 = 9,
    }

    public class AddTransferLocationPopupViewModel : NotifyDataError
    {
        static AddTransferLocationPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(AddTransferLocationPopupViewModel), typeof(AddTransferLocationPopupView));
        }

        public AddTransferLocationPopupViewModel(List<TransferLocation> transferLocation)
        {
            _transferLocation = transferLocation;
            NewTransferLocation = TransferLocation.LoadPort1;
        }

        #region Fields

        private readonly List<TransferLocation> _transferLocation;

        #endregion

        #region Properties

        public MappingLocation MappingLocation
        {
            get
            {
                return (MappingLocation)NewTransferLocation;
            }

            set
            {
                NewTransferLocation = (TransferLocation)value;
            }
        }

        private TransferLocation _newTransferLocation;

        public TransferLocation NewTransferLocation
        {
            get => _newTransferLocation;
            set => SetAndRaiseIfChanged(ref _newTransferLocation, value);
        }

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
