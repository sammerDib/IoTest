using Agileo.GUI.Components;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Enums;
using UnitySC.Equipment.Abstractions.Material;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.GUI.Common.Equipment.Popup
{
    public class SetWaferPresencePopup : Notifier
    {
        #region fields

        private readonly SubstrateLocation _location;

        #endregion

        #region Properties

        private SampleDimension _waferSize;
        public SampleDimension WaferSize
        {
            get => _waferSize;
            set => SetAndRaiseIfChanged(ref _waferSize, value);
        }

        private bool _waferPresence;
        public bool WaferPresence
        {
            get => _waferPresence;
            set => SetAndRaiseIfChanged(ref _waferPresence, value);
        }

        private MaterialType _materialType;
        public MaterialType MaterialType
        {
            get => _materialType;
            set => SetAndRaiseIfChanged(ref _materialType, value);
        }

        #endregion

        #region constructor

        static SetWaferPresencePopup()
        {
            DataTemplateGenerator.Create(typeof(SetWaferPresencePopup), typeof(SetWaferPresencePopupView));
        }

        public SetWaferPresencePopup(SubstrateLocation location)
        {
            _location = location;
            WaferPresence = location.Substrate != null;
            WaferSize = location.Substrate?.MaterialDimension ?? SampleDimension.NoDimension;
            MaterialType = ((Wafer)location.Material)?.MaterialType ?? MaterialType.SiliconWithNotch;
        }

        #endregion

        #region Public

        public void ValidateModifications()
        {
            if (WaferPresence && _location.Substrate == null)
            {
                if (_location.Substrate == null)
                {
                    var substrate = new Wafer("Unknown")
                    {
                        MaterialDimension = WaferSize,
                        MaterialType = MaterialType
                    };

                    try
                    {
                        substrate.SetLocation(_location);
                    }
                    catch
                    {
                        //Do nothing in case of exception
                    }
                }
                else
                {
                    _location.Substrate.MaterialDimension = WaferSize;
                    ((Wafer)_location.Material).MaterialType = MaterialType;
                }
            }
            else if (!WaferPresence && _location.Substrate != null)
            {
                try
                {
                    _location.Substrate.SetLocation(null);
                }
                catch
                {
                    //Do nothing in case of exception
                }
            }
            else if (_location.Substrate != null && (WaferSize != _location.Substrate.MaterialDimension || MaterialType != ((Wafer)_location.Material).MaterialType))
            {
                _location.Substrate.MaterialDimension = WaferSize;
                ((Wafer)_location.Material).MaterialType = MaterialType;
            }
        }


        #endregion
    }
}
