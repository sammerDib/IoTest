using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chamber;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Collection;

namespace UnitySC.PM.EME.Client.Proxy.Chamber
{
    public class ChamberVM : ObservableRecipient
    {
        private readonly ILogger _logger;

        private readonly ChamberSupervisor _chamberSupervisor;
        private EMEChamberConfig _emeChamberConfig;

        public ChamberVM(ChamberSupervisor supervisor)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _chamberSupervisor = supervisor;
            Init();

            Messenger.Register<IsInMaintenanceChangedMessage>(this, (r, m) => { UpdateIsInMaintenance(m.IsInMaintenance); });
            Messenger.Register<ArmNotExtendedChangedMessage>(this, (r, m) => { UpdateArmNotExtended(m.ArmNotExtended); });
            Messenger.Register<InterlockMessage>(this, (r, m) => { UpdateInterlocks(m); });
            Messenger.Register<SlitDoorPositionChangedMessage>(this, (r, m) => { UpdateSlitDoorPosition(m.SlitDoorPosition); });
        }

        private void Init()
        {
            try
            {
                if (_emeChamberConfig == null)
                {
                    _emeChamberConfig = _chamberSupervisor.GetChamberConfiguration()?.Result;
                }

                if (_emeChamberConfig?.Interlocks != null && !_emeChamberConfig.Interlocks.InterlockPanels.IsNullOrEmpty())
                {
                    foreach (var interlock in _emeChamberConfig.Interlocks.InterlockPanels)
                    {
                        InterlockPanels.Add(interlock.InterlockID, new InterlockVM
                        {
                            Description = interlock.Description,
                            StateValue = interlock.State,
                        });
                    }
                }

                Task.Run(() => _chamberSupervisor.TriggerUpdateEvent());
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, $"ChamberViewModel Init Failed");
            }
        }

        private void UpdateIsInMaintenance(bool isInMaintenance)
        {
            IsInMaintenance = isInMaintenance ? "Maintenance" : "Run";
        }

        private void UpdateArmNotExtended(bool value)
        {
            ArmNotExtended = value;
        }

        private void UpdateInterlocks(InterlockMessage interlock)
        {
            if (InterlockPanels.ContainsKey(interlock.InterlockID))
            {
                InterlockPanels[interlock.InterlockID].StateValue = interlock.State;
            }
            else
            {
                _logger?.Error($"The interlock panel ID:{interlock.InterlockID} isn't present");
            }
        }

        private void UpdateSlitDoorPosition(SlitDoorPosition position)
        {
            switch (position)
            {
                case UnitySC.Shared.Data.Enum.SlitDoorPosition.OpenPosition:
                    SlitDoorOpenSelected = true;
                    SlitDoorPosition = "Open";
                    break;

                case UnitySC.Shared.Data.Enum.SlitDoorPosition.ClosePosition:
                    SlitDoorOpenSelected = false;
                    SlitDoorPosition = "Close";
                    break;

                case UnitySC.Shared.Data.Enum.SlitDoorPosition.UnknownPosition:
                    SlitDoorPosition = "Unknown";
                    break;

                case UnitySC.Shared.Data.Enum.SlitDoorPosition.ErrorPosition:
                    SlitDoorPosition = "Error";
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }
        }

        private Dictionary<uint, InterlockVM> _interlockPanels = new Dictionary<uint, InterlockVM>();

        public Dictionary<uint, InterlockVM> InterlockPanels
        {
            get => _interlockPanels;
            set
            {
                if (value != _interlockPanels)
                {
                    _interlockPanels = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _slitDoorPosition;

        public string SlitDoorPosition
        {
            get => _slitDoorPosition;
            set
            {
                if (_slitDoorPosition != value)
                {
                    _slitDoorPosition = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _safetyState;

        public string SafetyState
        {
            get => _safetyState;
            set
            {
                if (_safetyState != value)
                {
                    _safetyState = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _safetyProcessed;

        public string SafetyProcessed
        {
            get => _safetyProcessed;
            set
            {
                if (_safetyProcessed != value)
                {
                    _safetyProcessed = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _safetyError;

        public string SafetyError
        {
            get => _safetyError;
            set
            {
                if (_safetyError != value)
                {
                    _safetyError = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _isInMaintenance;

        public string IsInMaintenance
        {
            get => _isInMaintenance;
            set
            {
                if (_isInMaintenance != value)
                {
                    _isInMaintenance = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _armNotExtended;

        public bool ArmNotExtended
        {
            get => _armNotExtended;
            set
            {
                if (_armNotExtended != value)
                {
                    _armNotExtended = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _slitDoorOpenSelected = true;

        public bool SlitDoorOpenSelected
        {
            get => _slitDoorOpenSelected;
            set
            {
                if (value != _slitDoorOpenSelected)
                {
                    _slitDoorOpenSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _interlockEfemArmExtendedEnable;

        public string InterlockEfemArmExtendedEnable
        {
            get => _interlockEfemArmExtendedEnable;
            set
            {
                if (_interlockEfemArmExtendedEnable != value)
                {
                    _interlockEfemArmExtendedEnable = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _interlockEfemSlitDoor;

        public string InterlockEfemSlitDoor
        {
            get => _interlockEfemSlitDoor;
            set
            {
                if (_interlockEfemSlitDoor != value)
                {
                    _interlockEfemSlitDoor = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _slitDoorOpenCommand;

        public RelayCommand SlitDoorOpenCommand
        {
            get => _slitDoorOpenCommand ?? (_slitDoorOpenCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chamberSupervisor.OpenSlitDoor();
                });
            }));
        }

        private RelayCommand _slitDoorCloseCommand;

        public RelayCommand SlitDoorCloseCommand
        {
            get => _slitDoorCloseCommand ?? (_slitDoorCloseCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chamberSupervisor.CloseSlitDoor();
                });
            }));
        }
    }
}
