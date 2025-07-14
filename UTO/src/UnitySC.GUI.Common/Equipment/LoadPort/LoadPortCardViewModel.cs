using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Agileo.Common.Localization;
using Agileo.Common.Tracing;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Vendor.Devices.Enums;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.GUI.Common.Equipment.LoadPort.Popup;
using UnitySC.GUI.Common.Equipment.UnityDevice;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataValidation;

using CarrierEventArgs = UnitySC.Equipment.Abstractions.Material.CarrierEventArgs;
using SlotMapEventArgs = UnitySC.Equipment.Abstractions.Material.SlotMapEventArgs;
using SlotState = UnitySC.Equipment.Abstractions.Material.SlotState;

namespace UnitySC.GUI.Common.Equipment.LoadPort
{
    public class LoadPortCardViewModel : UnityDeviceCardViewModel, IDisposable
    {
        #region Constructor

        static LoadPortCardViewModel()
        {
            DataTemplateGenerator.Create(typeof(LoadPortCardViewModel), typeof(LoadPortCard));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(EquipmentResources)));
        }

        public LoadPortCardViewModel()
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public LoadPortCardViewModel(UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort loadPort)
        {
            LoadPort = loadPort;

            LoadPort.StatusValueChanged += LoadPortOnStatusValueChanged;
            LoadPort.CarrierPlaced += LoadPortOnCarrierPlaced;
            LoadPort.CarrierRemoved += LoadPortOnCarrierRemoved;
            LoadPort.CarrierIdChanged += LoadPortOnCarrierIdChanged;

            Rules.Add(new DelegateRule(nameof(EditingCarrierId), ValidateCarrierId));
            ApplyRules();

            DispatcherHelper.DoInUiThread(
                () =>
                {
                    if (LoadPort.CarrierPresence == CassettePresence.Correctly)
                    {
                        LoadPortOnCarrierPlacedAction(LoadPort.Carrier);
                    }
                });
        }

        #endregion

        #region Properties

        private UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort _loadPort;

        public UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort LoadPort
        {
            get => _loadPort;
            private set => SetAndRaiseIfChanged(ref _loadPort, value);
        }

        private string _editingCarrierId;

        public string EditingCarrierId
        {
            get
            {
                if (LoadPort != null && LoadPort.Carrier != null && !string.IsNullOrWhiteSpace(LoadPort.Carrier.Id))
                {
                    return LoadPort?.Carrier?.Id;
                }

                return _editingCarrierId;
            }
            set
            {
                if (LoadPort is { Carrier: { } } && LoadPort.Carrier.Id != value)
                {
                    LoadPort.Carrier.Id = string.IsNullOrEmpty(value) ? " " : value;
                }

                SetAndRaiseIfChanged(ref _editingCarrierId, value);
            }
        }

        private bool _hasCarrierIdError;

        public bool HasCarrierIdError
        {
            get => _hasCarrierIdError;
            set => SetAndRaiseIfChanged(ref _hasCarrierIdError, value);
        }

        public SimplifiedSlotMapViewModel SimplifiedSlotMapViewModel { get; } = new();

        public bool IsNotMapped => LoadPort.IsDoorOpen
                                   && (LoadPort.Carrier == null
                                       || LoadPort.Carrier.MappingTable == null
                                       || LoadPort.Carrier.MappingTable.Count == 0);

        #endregion

        #region Command

        #region Move in access

        private SafeDelegateCommand _moveInAccess;

        public SafeDelegateCommand MoveInAccessCommand
            => _moveInAccess ??= new SafeDelegateCommand(
                MoveInAccessCommandExecute,
                MoveInAccessCommandCanExecute);

        private void MoveInAccessCommandExecute()
        {
            Task.Factory.StartNew(
                () =>
                {
                    //Read the carrier id and do nothing if fail
                    try
                    {
                        if (LoadPort.Configuration.IsManualCarrierType
                            && LoadPort.CarrierTypeName == UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort.UnknownCarrierName)
                        {
                            var popupContent = new SetCarrierTypePopup(
                                LoadPort.Configuration.CarrierTypes
                                    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                                    .ToList());

                            var popup = new Agileo.GUI.Services.Popups.Popup(PopupButtons.OKCancel,
                                new LocalizableText(
                                    nameof(EquipmentResources.LOADPORT_SELECT_CARRIER_TYPE)))
                            {
                                Content = popupContent,
                            };

                            var result = App.Instance.UserInterface.Navigation.SelectedBusinessPanel?.Popups.ShowAndWaitResult(popup);
                            if (result == PopupResult.Cancel
                                || popupContent.SelectedCarrierType == null
                                || string.IsNullOrWhiteSpace(popupContent.SelectedCarrierType.Name))
                            {
                                return;
                            }

                            LoadPort.SetCarrierType(popupContent.SelectedCarrierType.Id);
                        }
                        LoadPort.Clamp();
                        LoadPort.ReadCarrierId();
                    }
                    catch
                    {
                        //Do nothing if carrier reading fail
                    }

                    //Open the load port
                    LoadPort.Open(true);
                });
        }

        private bool MoveInAccessCommandCanExecute()
        {
            if (LoadPort == null)
            {
                return false;
            }

            //If Open command is available, all other commands are available
            return LoadPort.CanExecute(nameof(ILoadPort.Open), out _, true);
        }

        #endregion

        #region Release carrier

        private SafeDelegateCommandAsync _releaseCarrier;

        public SafeDelegateCommandAsync ReleaseCarrierCommand
            => _releaseCarrier ??= new SafeDelegateCommandAsync(
                ReleaseCarrierCommandExecute,
                ReleaseCarrierCommandCanExecute);

        private Task ReleaseCarrierCommandExecute() => LoadPort.ReleaseCarrierAsync();

        private bool ReleaseCarrierCommandCanExecute()
        {
            if (LoadPort == null)
            {
                return false;
            }

            var context = LoadPort.NewCommandContext(nameof(LoadPort.ReleaseCarrier));
            return LoadPort.CanExecute(context);
        }

        #endregion

        #region Init

        private SafeDelegateCommandAsync _initialize;

        public SafeDelegateCommandAsync InitializeCommand
            => _initialize ??= new SafeDelegateCommandAsync(InitializeCommandExecute, InitializeCommandCanExecute);

        private Task InitializeCommandExecute() => LoadPort.InitializeAsync(false);

        private bool InitializeCommandCanExecute()
        {
            if (LoadPort == null)
            {
                return false;
            }

            var context = LoadPort.NewCommandContext(nameof(LoadPort.Initialize)).AddArgument("mustForceInit", false);
            return LoadPort.CanExecute(context);
        }

        #endregion

        #region Abort

        private SafeDelegateCommandAsync _abortCommand;

        public SafeDelegateCommandAsync AbortCommand
            => _abortCommand ??= new SafeDelegateCommandAsync(
                AbortCommandExecute,
                AbortCommandCanExecute);

        private Task AbortCommandExecute() => LoadPort.InterruptAsync(InterruptionKind.Abort);

        private bool AbortCommandCanExecute()
        {
            if (LoadPort == null)
            {
                return false;
            }

            return LoadPort.State != OperatingModes.Maintenance
                   && LoadPort.State != OperatingModes.Idle;
        }

        #endregion

        #region GoToCarrierView

        private SafeDelegateCommand _goToCarrierView;

        public SafeDelegateCommand GoToCarrierView
            => _goToCarrierView ??= new SafeDelegateCommand(GoToCarrierViewExecute, GoToCarrierViewCanExecute);

        private void GoToCarrierViewExecute() => ImplGoToCarrierViewExecute();

        private bool GoToCarrierViewCanExecute() => ImplGoToCarrierViewCanExecute();

        #endregion

        protected virtual void ImplGoToCarrierViewExecute()
        {

        }

        protected virtual bool ImplGoToCarrierViewCanExecute()
        {
            return false;
        }

        #endregion

        #region Private methods

        private void LoadPortOnStatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (e.Status.Name == nameof(ILoadPort.IsDoorOpen))
            {
                TraceManager.Instance().Trace(TraceLevelType.Debug, $"Door open on load port");
                OnPropertyChanged(nameof(IsNotMapped));
            }
        }

        private void LoadPortOnCarrierPlaced(object sender, CarrierEventArgs e)
        {
            LoadPortOnCarrierPlacedAction(e.Carrier);
        }

        private void LoadPortOnCarrierPlacedAction(UnitySC.Equipment.Abstractions.Material.Carrier carrier)
        {
            if (carrier != null)
            {
                TraceManager.Instance().Trace(TraceLevelType.Debug, $"Subscribe slot map changed event");
                carrier.SlotMapChanged += CarrierOnSlotMapChanged;
            }
            OnPropertyChanged(nameof(LoadPort));
        }

        private void LoadPortOnCarrierRemoved(object sender, CarrierEventArgs e)
        {
            if (e.Carrier != null)
            {
                TraceManager.Instance().Trace(TraceLevelType.Debug, $"Unsubscribe slot map changed event");
                e.Carrier.SlotMapChanged -= CarrierOnSlotMapChanged;
            }

            UpdateSlotMap();
            OnPropertyChanged(nameof(LoadPort));
        }

        private void LoadPortOnCarrierIdChanged(object sender, CarrierIdChangedEventArgs e)
        {
            HasCarrierIdError = e.Status == CommandStatusCode.Error;
            OnPropertyChanged(nameof(LoadPort));
        }

        private void CarrierOnSlotMapChanged(object sender, SlotMapEventArgs e)
        {
            TraceManager.Instance().Trace(TraceLevelType.Debug, $"UpdateSlotMap with slotmap : {e.SlotMap.Count}");
            UpdateSlotMap(e.SlotMap);
        }

        private Collection<Substrate> GetSimplifiedWaferIDs()
        {
            var simplifiedWaferIDs = new Collection<Substrate>();

            if (LoadPort.Carrier?.MaterialLocations != null)
            {
                foreach (var location in LoadPort.Carrier.MaterialLocations)
                {
                    simplifiedWaferIDs.Add((Substrate)location.Material);
                }
            }

            return simplifiedWaferIDs;
        }

        #region Protected methods

        protected virtual void UpdateSlotMap(IEnumerable<SlotState> slotStates = null)
        {
            SimplifiedSlotMapViewModel.UpdateSlotMap(slotStates ?? Enumerable.Empty<SlotState>(), GetSimplifiedWaferIDs());
            OnPropertyChanged(nameof(IsNotMapped));
        }

        #endregion

        #region Validation

        private string ValidateCarrierId()
        {
            if (LoadPort != null && LoadPort.Carrier != null)
            {
                if (string.IsNullOrEmpty(LoadPort.Carrier.Id) || string.IsNullOrWhiteSpace(LoadPort.Carrier.Id))
                {
                    return LocalizationManager.GetString(nameof(EquipmentResources.CARRIERS_CARRIER_ID_EMPTY));
                }

                var loadPorts = App.Instance.EquipmentManager.Equipment.AllOfType<UnitySC.Equipment.Abstractions.Devices.LoadPort.LoadPort>()
                    .Where(lp => lp.Name != LoadPort.Name)
                    .ToList();
                foreach (var loadPort in loadPorts)
                {
                    if (loadPort != null && loadPort.Carrier != null && loadPort.Carrier.Id.Equals(LoadPort.Carrier.Id))
                    {
                        return LocalizationManager.GetString(nameof(EquipmentResources.CARRIERS_ID_USED));
                    }
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Dispose

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
            {
                return;
            }

            if (disposing)
            {
                LoadPort.StatusValueChanged -= LoadPortOnStatusValueChanged;
                LoadPort.CarrierPlaced -= LoadPortOnCarrierPlaced;
                LoadPort.CarrierRemoved -= LoadPortOnCarrierRemoved;
                LoadPort.CarrierIdChanged -= LoadPortOnCarrierIdChanged;

                if (LoadPort.Carrier != null)
                {
                    LoadPort.Carrier.SlotMapChanged -= CarrierOnSlotMapChanged;
                }
            }

            _disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
