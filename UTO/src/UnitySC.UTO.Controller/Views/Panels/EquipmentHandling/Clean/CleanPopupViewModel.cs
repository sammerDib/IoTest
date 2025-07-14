using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.Common.Logging;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Services.Popups;
using Agileo.GUI.Services.UserMessages;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Abstractions.Vendor;
using UnitySC.Equipment.Abstractions.Vendor.Material;
using UnitySC.GUI.Common.Equipment.LoadPort;
using UnitySC.GUI.Common.Equipment.Popup;
using UnitySC.GUI.Common.Resources;
using UnitySC.GUI.Common.Vendor;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.Commands;
using UnitySC.GUI.Common.Vendor.UIComponents.Components.DataTables;

namespace UnitySC.UTO.Controller.Views.Panels.EquipmentHandling.Clean
{
    public class CleanPopupViewModel : Notifier, IDisposable
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Properties

        public DataTableSource<SubstrateLocation> SubstrateLocations { get; } = new();

        public SubstrateLocation SelectedLocation { get; set; }

        public List<CleanLoadPort> LoadPorts { get; } = new();

        public LoadPort SelectedLoadPort { get; set; }

        public IndexedSlotState SelectedSlot { get; set; }

        public UserMessageDisplayer Messages { get; set; }

        #endregion

        #region Constructor

        static CleanPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(CleanPopupViewModel), typeof(CleanPopupView));
        }

        public CleanPopupViewModel(
            UnitySC.Equipment.Abstractions.Devices.Controller.Controller controller,
            ILogger logger)
        {
            _logger = logger;
            Messages = new UserMessageDisplayer();

            SubstrateLocations.Reset(
                controller.GetSubstrateLocations().Where(x => x.Substrate != null));

            var loadPorts = controller.AllDevices<LoadPort>().ToList();
            foreach (var lp in loadPorts)
            {
                LoadPorts.Add(new CleanLoadPort(lp));
            }

            var availableSubstrate = controller.GetSubstrates();
            foreach (var substrate in availableSubstrate)
            {
                if (substrate.Source != null
                    || substrate.SourcePort <= 0
                    || substrate.SourceSlot <= 0)
                {
                    continue;
                }

                var loadPort =
                    loadPorts.FirstOrDefault(lp => lp.InstanceId == substrate.SourcePort);

                if (loadPort == null
                    || loadPort.Carrier == null
                    || loadPort.Carrier.MaterialLocations == null
                    || loadPort.Carrier.MaterialLocations.Count < substrate.SourceSlot)
                {
                    continue;
                }

                var materialLocation = loadPort.Carrier.MaterialLocations[substrate.SourceSlot - 1];

                if (materialLocation is SubstrateLocation substrateLocation)
                {
                    substrate.SetSource(
                        substrateLocation,
                        substrate.SourcePort,
                        substrate.SourceSlot,
                        DateTime.Now);
                }
            }
        }

        #endregion

        #region Commands

        #region Set Size Command

        private SafeDelegateCommand _setSizeCommand;

        public SafeDelegateCommand SetSizeCommand
            => _setSizeCommand ??= new SafeDelegateCommand(
                SetSizeCommandExecute,
                SetSizeCommandCanExecute);

        private void SetSizeCommandExecute()
        {
            var popupContent = new SetWaferPresencePopup(SelectedLocation);
            var popup =
                new Popup(new LocalizableText(nameof(EquipmentResources.POPUP_SET_WAFER_PRESENCE)))
                {
                    Content = popupContent
                };

            popup.Commands.Add(
                new PopupCommand(
                    Agileo.GUI.Properties.Resources.S_OK,
                    new DelegateCommand(
                        () =>
                        {
                            popupContent.ValidateModifications();
                            if (!popupContent.WaferPresence)
                            {
                                SubstrateLocations.Remove(SelectedLocation);
                            }

                            SubstrateLocations.UpdateCollection();
                        },
                        () => !(popupContent.WaferPresence
                                && popupContent.WaferSize == SampleDimension.NoDimension))));
            popup.Commands.Add(new PopupCommand(nameof(Agileo.GUI.Properties.Resources.S_CANCEL)));

            AgilControllerApplication.Current.UserInterface.Popups.Show(popup);
        }

        private bool SetSizeCommandCanExecute()
        {
            return SelectedLocation != null;
        }

        #endregion

        #region Set Destination Command

        private SafeDelegateCommand _setDestinationCommand;

        public SafeDelegateCommand SetDestinationCommand
            => _setDestinationCommand ??= new SafeDelegateCommand(
                SetDestinationCommandExecute,
                SetDestinationCommandCanExecute);

        private void SetDestinationCommandExecute()
        {
            var slot = (byte)SelectedSlot.Index;
            SelectedLocation.Substrate.SetSource(
                SelectedLoadPort.Carrier.MaterialLocations[slot - 1] as SubstrateLocation,
                (byte)SelectedLoadPort.InstanceId,
                (byte)SelectedSlot.Index,
                DateTime.Now);
            SubstrateLocations.UpdateCollection();
        }

        private bool SetDestinationCommandCanExecute()
        {
            return SelectedLocation != null && SelectedLoadPort != null && SelectedSlot != null;
        }

        #endregion

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            SubstrateLocations?.Dispose();

            foreach (var lp in LoadPorts)
            {
                lp.Dispose();
            }
        }

        #endregion
    }
}
