using System;

using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LightTower.Configuration;
using UnitySC.Equipment.Abstractions.Devices.LightTower.Enums;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

namespace UnitySC.GUI.Common.Views.Panels.Setup.DeviceSettings.LightTower
{
    public class LightTowerDetails : NotifyDataError
    {
        private readonly LightTowerStatus _lightTowerStatus;

        #region Constructors

        public LightTowerDetails(LightTowerStatus lightTowerStatus)
        {
            _lightTowerStatus = lightTowerStatus;
        }

        #endregion

        #region Properties

        public LightTowerState LightTowerState
        {
            get => _lightTowerStatus.LightTowerState;
            set
            {
                if (_lightTowerStatus.LightTowerState == value)
                {
                    return;
                }

                _lightTowerStatus.LightTowerState = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _lightTowerStatus.Description;
            set
            {
                if (string.Equals(_lightTowerStatus.Description, value, StringComparison.Ordinal))
                {
                    return;
                }

                _lightTowerStatus.Description = value;
                OnPropertyChanged();
            }
        }

        public LightState Red
        {
            get => _lightTowerStatus.Red;
            set
            {
                if (_lightTowerStatus.Red == value)
                {
                    return;
                }

                _lightTowerStatus.Red = value;
                OnPropertyChanged();
            }
        }

        public LightState Orange
        {
            get => _lightTowerStatus.Orange;
            set
            {
                if (_lightTowerStatus.Orange == value)
                {
                    return;
                }

                _lightTowerStatus.Orange = value;
                OnPropertyChanged();
            }
        }

        public LightState Green
        {
            get => _lightTowerStatus.Green;
            set
            {
                if (_lightTowerStatus.Green == value)
                {
                    return;
                }

                _lightTowerStatus.Green = value;
                OnPropertyChanged();
            }
        }

        public LightState Blue
        {
            get => _lightTowerStatus.Blue;
            set
            {
                if (_lightTowerStatus.Blue == value)
                {
                    return;
                }

                _lightTowerStatus.Blue = value;
                OnPropertyChanged();
            }
        }

        public BuzzerState BuzzerState
        {
            get => _lightTowerStatus.BuzzerState;
            set
            {
                if (_lightTowerStatus.BuzzerState == value)
                {
                    return;
                }

                _lightTowerStatus.BuzzerState = value;
                OnPropertyChanged();
            }
        }

        #endregion Properties
    }
}
