using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Components;
using Agileo.GUI.Services.LightTower;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions;
using UnitySC.Equipment.Abstractions.Devices.LightTower;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;
using UnitySC.GUI.Common.Views.TitlePanel;

using LightState = Agileo.GUI.Services.LightTower.LightState;

namespace UnitySC.EFEM.Controller.Views.TitlePanel
{
    /// <summary>
    /// TitlePanel ViewModel
    /// </summary>
    public class TitlePanel : GUI.Common.Vendor.Views.TitlePanel.TitlePanel
    {
        private readonly LightTower _lightTower;

        #region Constructor

        /// <summary>
        /// Initializes the <see cref="TitlePanel"/> class.
        /// </summary>
        static TitlePanel()
        {
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(TitlePanelResources)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePanel"/> class.
        /// </summary>
        public TitlePanel() : this(null)
        {
            // Design Time data
            if (IsInDesignMode)
            {
                CurrentUserName = "SUPERVISOR";
                Messages.Show(new UserMessage("Design time message")
                {
                    Level = MessageLevel.Info
                });
            }
            else
            {
                throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public TitlePanel(UserInterface userInterface) : base(userInterface)
        {
            if (GUI.Common.App.Instance.EquipmentManager is EfemEquipmentManager equipmentManager)
            {
                EquipmentManager = equipmentManager;
            }
            else
            {
                throw new InvalidOperationException(
                    $"Cannot instantiate {nameof(TitlePanel)} because selected equipment is not an {nameof(EfemEquipmentManager)}");
            }

            _lightTower = EquipmentManager.LightTower;
            if (_lightTower == null)
            {
                throw new InvalidOperationException(
                    $"Light tower not available in {nameof(EfemEquipmentManager)}");
            }

            SetupSignalTower();
        }

        #endregion

        #region Event Handlers

        private void LightTower_StatusValueChanged(object sender, StatusChangedEventArgs e)
        {
            if (GUI.Common.App.Instance != null
                && GUI.Common.App.Instance.Dispatcher != null
                && GUI.Common.App.Instance.Dispatcher.Thread != Thread.CurrentThread)
            {
                GUI.Common.App.Instance.Dispatcher.BeginInvoke(new Action(() => LightTower_StatusValueChanged(sender, e)));
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

        #endregion Event Handlers

        #region Properties

        public HardwareConnectionViewModel HardwareConnectionViewModel { get; } =
            new(App.EfemAppInstance.EfemEquipmentManager.CommunicatingDevices);

        /// <summary>
        /// Get the <see cref="LightTower.BuzzerState"/>
        /// </summary>
        public BuzzerState BuzzerState => _lightTower?.BuzzerState ?? BuzzerState.Off;


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
            get { return _lightTowerViewModel; }
            set { SetAndRaiseIfChanged(ref _lightTowerViewModel, value); }
        }

        private EfemEquipmentManager _equipmentManager;

        public EfemEquipmentManager EquipmentManager
        {
            get => _equipmentManager;
            set => SetAndRaiseIfChanged(ref _equipmentManager, value);
        }

        #endregion Properties

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

        #endregion Commands

        #region Methods

        private void DefineLightTowerState(int lightIndex, LightState lightState)
        {
            if (lightIndex < _lightTowerViewModel?.Lights.Count && lightIndex >= 0)
            {
                _lightTowerViewModel.Lights[lightIndex].StatusLightState = lightState;
            }
        }

        private void SetupSignalTower()
        {
            _lightTower.StatusValueChanged += LightTower_StatusValueChanged;

            // define state on startup
            DefineLightTowerState(0, _lightTower.RedLight);
            DefineLightTowerState(1, _lightTower.OrangeLight);
            DefineLightTowerState(2, _lightTower.GreenLight);
            DefineLightTowerState(3, _lightTower.BlueLight);
        }

        #endregion Methods

    }
}
