using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components;
using Agileo.GUI.Interfaces;
using Agileo.GUI.Services.LightTower;
using Agileo.Semi.Gem.Abstractions.E30;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Views.TitlePanel;
using UnitySC.UTO.Controller.Remote;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.UTO.Controller.Views.TitlePanel
{
    public class UnityTitlePanel : GUI.Common.Vendor.Views.TitlePanel.TitlePanel
    {
        #region Fields

        private readonly LightTower _lightTower;
        private GemController _gemController;

        #endregion

        #region Event Handlers

        private void LightTowerStatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (GUI.Common.App.Instance != null
                && GUI.Common.App.Instance.Dispatcher != null
                && GUI.Common.App.Instance.Dispatcher.Thread != Thread.CurrentThread)
            {
                GUI.Common.App.Instance.Dispatcher.BeginInvoke(new Action(() => LightTowerStatusValueChanged(sender, e)));
                return;
            }

            switch (e.Status.Name)
            {
                case nameof(_lightTower.RedLight):
                    DefineLightTowerState(0, (LightState)e.NewValue);
                    break;
                case nameof(_lightTower.OrangeLight):
                    DefineLightTowerState(1, (LightState)e.NewValue);
                    break;
                case nameof(_lightTower.GreenLight):
                    DefineLightTowerState(2, (LightState)e.NewValue);
                    break;
                case nameof(_lightTower.BlueLight):
                    DefineLightTowerState(3, (LightState)e.NewValue);
                    break;
                case nameof(_lightTower.BuzzerState):
                    OnPropertyChanged(nameof(BuzzerState));
                    break;
            }
        }

        private void ControlServicesOnOnHsmsStateChanged(object sender, HSMSStateChangedEventArgs e)
            => OnPropertyChanged(nameof(HsmsState));

        private void ControlServicesOnOnControlStateChanged(object sender, ControlStateChangedEventArgs e)
            => OnPropertyChanged(nameof(ControlState));

        private void ControlServicesOnOnCommunicationStateChanged(object sender, CommunicationStateChangedEventArgs e)
            => OnPropertyChanged(nameof(CommunicationState));

        #endregion

        #region Constructor

        static UnityTitlePanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(UnityTitlePanelResources)));
        }

        public UnityTitlePanel()
        {
        }

        public UnityTitlePanel(UserInterface userInterface)
            : base(userInterface)
        {
            if (GUI.Common.App.Instance.EquipmentManager is ControllerEquipmentManager equipmentManager)
            {
                EquipmentManager = equipmentManager;
                DataFlowManager = equipmentManager.Controller.TryGetDevice<AbstractDataFlowManager>();
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cannot instantiate {nameof(UnityTitlePanel)} because selected equipment is not an {nameof(ControllerEquipmentManager)}");
            }

            _lightTower = EquipmentManager.LightTower;
            if (_lightTower == null)
            {
                throw new InvalidOperationException(
                    $"Light tower not available in {nameof(ControllerEquipmentManager)}");
            }

            SetupSignalTower();
        }

        #endregion

        #region Properties

        public Visibility IsGemComponentEnabled
        {
            get
            {
                if (IsInDesignMode)
                {
                    return Visibility.Visible;
                }

                return App.ControllerInstance?.IsGemEnabled == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Get the <see cref="GemController.ControlServices" /> communication state
        /// </summary>
        public CommunicationState CommunicationState
            => _gemController?.ControlServices.CommunicationState ?? CommunicationState.Disabled;

        /// <summary>
        /// Get the <see cref="GemController.ControlServices" /> control state
        /// </summary>
        public ControlState ControlState => _gemController?.ControlServices.ControlState ?? ControlState.HostOffLine;

        /// <summary>
        /// Get the <see cref="GemController.ControlServices" /> HSMS state
        /// </summary>
        public HSMSState HsmsState => _gemController?.ControlServices.HsmsState ?? HSMSState.NotConnected;

        public GemCommandsViewModel GemCommandsViewModel { get; } = new();

        public HardwareConnectionViewModel HardwareConnectionViewModel { get; } =
            new(App.ControllerInstance.ControllerEquipmentManager.CommunicatingDevices);

        /// <summary>
        ///     Get the <see cref="LightTower.BuzzerState" />
        /// </summary>
        public BuzzerState BuzzerState => _lightTower.BuzzerState;

        private LightTowerViewModel _lightTowerViewModel =
            new LightTowerViewModel(new List<LightViewModel>
            {
                new(Brushes.SeverityErrorBrush),
                new(Brushes.SeverityWarningBrush),
                new(Brushes.SeveritySuccessBrush),
                new(Brushes.SeverityInformationBrush)
            });

        public LightTowerViewModel LightTowerViewModel
        {
            get => _lightTowerViewModel;
            set => SetAndRaiseIfChanged(ref _lightTowerViewModel, value);
        }

        private ControllerEquipmentManager _equipmentManager;

        public ControllerEquipmentManager EquipmentManager
        {
            get => _equipmentManager;
            set => SetAndRaiseIfChanged(ref _equipmentManager, value);
        }

        private AbstractDataFlowManager _dataFlowManager;

        public AbstractDataFlowManager DataFlowManager
        {
            get => _dataFlowManager;
            set => SetAndRaiseIfChanged(ref _dataFlowManager, value);
        }
        #endregion

        #region Commands

        #region MuteBuzzerCommand

        private ICommand _muteBuzzerCommand;

        public ICommand MuteBuzzerCommand
            => _muteBuzzerCommand ??= new SafeDelegateCommandAsync(
                MuteBuzzerCommandExecute,
                MuteBuzzerCommandCanExecute);

        private bool MuteBuzzerCommandCanExecute()
            => _lightTower.IsCommunicating && _lightTower.BuzzerState != BuzzerState.Off;

        private Task MuteBuzzerCommandExecute() => _lightTower.DefineBuzzerModeAsync(BuzzerState.Off);

        #endregion

        #endregion

        #region Overrides

        public override void Accept(IGuiElementVisitor visitor)
        {
            base.Accept(visitor);

            // Apply Agileo.GUI services to GEM commands
            GemCommandsViewModel.RefreshAccessibility();
        }

        public override void OnSetup()
        {
            base.OnSetup();

            if (App.ControllerInstance.IsGemEnabled)
            {
                _gemController = App.ControllerInstance.GemController;

                OnPropertyChanged(nameof(CommunicationState));
                OnPropertyChanged(nameof(ControlState));
                OnPropertyChanged(nameof(HsmsState));

                _gemController.ControlServices.OnCommunicationStateChanged +=
                    ControlServicesOnOnCommunicationStateChanged;
                _gemController.ControlServices.OnControlStateChanged += ControlServicesOnOnControlStateChanged;
                _gemController.ControlServices.OnHsmsStateChanged += ControlServicesOnOnHsmsStateChanged;

                GemCommandsViewModel.OnSetup();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (App.ControllerInstance.IsGemEnabled && _gemController != null)
                {
                    _gemController.ControlServices.OnCommunicationStateChanged -=
                        ControlServicesOnOnCommunicationStateChanged;
                    _gemController.ControlServices.OnControlStateChanged -= ControlServicesOnOnControlStateChanged;
                    _gemController.ControlServices.OnHsmsStateChanged -= ControlServicesOnOnHsmsStateChanged;
                }

                GemCommandsViewModel.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private void DefineLightTowerState(int lightIndex, LightState lightState)
        {
            if (lightIndex < _lightTowerViewModel?.Lights.Count && lightIndex >= 0)
            {
                _lightTowerViewModel.Lights[lightIndex].StatusLightState = lightState;
            }
        }

        private void SetupSignalTower()
        {
            _lightTower.StatusValueChanged += LightTowerStatusValueChanged;

            // define state on startup
            DefineLightTowerState(0, _lightTower.RedLight);
            DefineLightTowerState(1, _lightTower.OrangeLight);
            DefineLightTowerState(2, _lightTower.GreenLight);
            DefineLightTowerState(3, _lightTower.BlueLight);
        }

        #endregion
    }
}
