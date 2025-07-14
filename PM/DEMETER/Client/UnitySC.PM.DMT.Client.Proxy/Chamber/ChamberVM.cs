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

namespace UnitySC.PM.DMT.Client.Proxy.Chamber
{
    public class ChamberVM : ObservableRecipient
    {
        private readonly ILogger _logger;

        private readonly ChamberSupervisor _chamberSupervisor;
        private ChamberConfig _chamberConfig;
        private DMTChamberConfig _psdChamberConfig;

        public ChamberVM(ChamberSupervisor supervisor)
        {
            _logger = ClassLocator.Default.GetInstance<ILogger>();
            _chamberSupervisor = supervisor;
            Init();

            Messenger.Register<IsInMaintenanceChangedMessage>(this, (r, m) => { UpdateIsInMaintenance(m.IsInMaintenance); });
            Messenger.Register<ArmNotExtendedChangedMessage>(this, (r, m) => { UpdateArmNotExtended(m.ArmNotExtended); });
            Messenger.Register<EfemSlitDoorOpenPositionChangedMessage>(this, (r, m) => { UpdateEfemSlitDoorOpenPosition(m.EfemSlitDoorOpenPosition); });
            Messenger.Register<IsReadyToLoadUnloadChangedMessage>(this, (r, m) => { UpdateIsReadyToLoadUnload(m.IsReadyToLoadUnload); });
            Messenger.Register<InterlockMessage>(this, (r, m) => { UpdateInterlocks(m); });
            Messenger.Register<SlitDoorPositionChangedMessage>(this, (r, m) => { UpdateSlitDoorPosition(m.SlitDoorPosition); });            
            Messenger.Register<SlitDoorOpenPositionChangedMessage>(this, (r, m) => { UpdateOpenSlitDoorPosition(m.SlitDoorOpenPosition); });
            Messenger.Register<SlitDoorClosePositionChangedMessage>(this, (r, m) => { UpdateCloseSlitDoorPosition(m.SlitDoorClosePosition); });
            Messenger.Register<CdaPneumaticValveChangedMessage>(this, (r, m) => { UpdateCdaPneumaticValve(m.ValveIsOpened); });
        }

        public void Init()
        {
            try
            {
                if (_psdChamberConfig == null)
                {
                    _psdChamberConfig = _chamberSupervisor.GetChamberConfiguration()?.Result;
                }

                if (_psdChamberConfig?.Interlocks != null && !_psdChamberConfig.Interlocks.InterlockPanels.IsNullOrEmpty())
                {
                    foreach (var interlock in _psdChamberConfig.Interlocks.InterlockPanels)
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
            if (position == UnitySC.Shared.Data.Enum.SlitDoorPosition.OpenPosition)
            {
                //if (!SlitDoorOpenSelected)
                {
                    SlitDoorOpenSelected = true;
                    SlitDoorPosition = "Open";
                }
            }
            else if (position == UnitySC.Shared.Data.Enum.SlitDoorPosition.ClosePosition)
            {
                //if (SlitDoorOpenSelected)
                {
                    SlitDoorOpenSelected = false;
                    SlitDoorPosition = " Closed";
                }
            }
            else
            {
                if (SlitDoorPosition != "Unknown")
                    SlitDoorPosition = "Unknown";
            }
        }

        private void UpdateEfemSlitDoorOpenPosition(bool openPosition)
        {
            EfemSlitDoorOpen = openPosition;
        }

        private void UpdateIsReadyToLoadUnload(bool isReady)
        {
            IsReadyToLoadUnload = isReady;
        }        

        private void UpdateOpenSlitDoorPosition(bool position)
        {
            SlitDoorOpen = position;
        }

        private void UpdateCloseSlitDoorPosition(bool position)
        {
            SlitDoorClose = position;
        }

        private void UpdateCdaPneumaticValve(bool valveIsOpened)
        {
            CdaValveIsOpen = valveIsOpened ? "Open" : "Closed";
            if (CdaValveOpenSelected != valveIsOpened)
                CdaValveOpenSelected = valveIsOpened;
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

        private bool _efemSlitDoorOpen;

        public bool EfemSlitDoorOpen
        {
            get => _efemSlitDoorOpen;
            set
            {
                if (_efemSlitDoorOpen != value)
                {
                    _efemSlitDoorOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isReadyToLoadUnload;

        public bool IsReadyToLoadUnload
        {
            get => _isReadyToLoadUnload;
            set
            {
                if (_isReadyToLoadUnload != value)
                {
                    _isReadyToLoadUnload = value;
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

        private bool _slitDoorOpen;

        public bool SlitDoorOpen
        {
            get => _slitDoorOpen;
            set
            {
                if (_slitDoorOpen != value)
                {
                    _slitDoorOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _slitDoorClose;

        public bool SlitDoorClose
        {
            get => _slitDoorClose;
            set
            {
                if (_slitDoorClose != value)
                {
                    _slitDoorClose = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _cdaValveIsOpen;

        public string CdaValveIsOpen
        {
            get => _cdaValveIsOpen;
            set
            {
                if (_cdaValveIsOpen != value)
                {
                    _cdaValveIsOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _cdaHigh;

        public string CdaHigh
        {
            get => _cdaHigh;
            set
            {
                if (_cdaHigh != value)
                {
                    _cdaHigh = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _cdaLow;

        public string CdaLow
        {
            get => _cdaLow;
            set
            {
                if (_cdaLow != value)
                {
                    _cdaLow = value;
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

        private bool _slitDoorOpenSelected;

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

        private bool _cdaValveOpenSelected;

        public bool CdaValveOpenSelected
        {
            get => _cdaValveOpenSelected;
            set
            {
                if (value != _cdaValveOpenSelected)
                {
                    _cdaValveOpenSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _cdaValveOpenCommand;

        public RelayCommand CdaValveOpenCommand
        {
            get => _cdaValveOpenCommand ?? (_cdaValveOpenCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CdaValveOpenSelected = true;
                    _chamberSupervisor.OpenCdaPneumaticValve();
                });
            }));
        }

        private RelayCommand _cdaValveCloseCommand;

        public RelayCommand CdaValveCloseCommand
        {
            get => _cdaValveCloseCommand ?? (_cdaValveCloseCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    CdaValveOpenSelected = false;
                    _chamberSupervisor.CloseCdaPneumaticValve();
                });
            }));
        }

        private bool _plcRestartRunSelected;

        public bool PlcRestartRunSelected
        {
            get => _plcRestartRunSelected;
            set
            {
                if (value != _plcRestartRunSelected)
                {
                    _plcRestartRunSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _plcRestartRunCommand;

        public RelayCommand PlcRestartRunCommand
        {
            get => _plcRestartRunCommand ?? (_plcRestartRunCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlcRestartRunSelected = true;
                });
            }));
        }

        private RelayCommand _plcRestartStopCommand;

        public RelayCommand PlcRestartStopCommand
        {
            get => _plcRestartStopCommand ?? (_plcRestartStopCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlcRestartRunSelected = false;
                });
            }));
        }

        private RelayCommand _plcRebootRunCommand;

        public RelayCommand PlcRebootRunCommand
        {
            get => _plcRebootRunCommand ?? (_plcRebootRunCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                });
            }));
        }

        private RelayCommand _plcRebootStopCommand;

        public RelayCommand PlcRebootStopCommand
        {
            get => _plcRebootStopCommand ?? (_plcRebootStopCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PlcRestartRunSelected = false;
                });
            }));
        }

        private bool _plcRebootStopSelected;

        public bool PlcRebootStopSelected
        {
            get => _plcRebootStopSelected;
            set
            {
                if (value != _plcRebootStopSelected)
                {
                    _plcRebootStopSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _safetyRunSelected;

        public bool SafetyRunSelected
        {
            get => _safetyRunSelected;
            set
            {
                if (value != _safetyRunSelected)
                {
                    _safetyRunSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        private RelayCommand _safetyRunCommand;

        public RelayCommand SafetyRunCommand
        {
            get => _safetyRunCommand ?? (_safetyRunCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SafetyRunSelected = true;
                });
            }));
        }

        private RelayCommand _safetyStopCommand;

        public RelayCommand SafetyStopCommand
        {
            get => _safetyStopCommand ?? (_safetyStopCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SafetyRunSelected = false;
                });
            }));
        }

        private RelayCommand _safetyErrAckClearCommand;

        public RelayCommand SafetyErrAckClearCommand
        {
            get => _safetyErrAckClearCommand ?? (_safetyErrAckClearCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                });
            }));
        }

        private RelayCommand _safetyErrAckCommand;

        public RelayCommand SafetyErrAckCommand
        {
            get => _safetyErrAckCommand ?? (_safetyErrAckCommand = new RelayCommand(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SafetyErrAckSelected = false;
                });
            }));
        }

        private bool _safetyErrAckSelected;

        public bool SafetyErrAckSelected
        {
            get => _safetyErrAckSelected;
            set
            {
                if (value != _safetyErrAckSelected)
                {
                    _safetyErrAckSelected = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
