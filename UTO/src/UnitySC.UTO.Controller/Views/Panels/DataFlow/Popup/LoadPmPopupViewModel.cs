using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;
using Agileo.SemiDefinitions;

using UnitySC.Equipment.Abstractions.Devices.DriveableProcessModule;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.GUI.Common.Vendor.Helpers;

namespace UnitySC.UTO.Controller.Views.Panels.DataFlow.Popup
{
    public class LoadPmPopupViewModel : Notifier, IDisposable
    {
        static LoadPmPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(LoadPmPopupViewModel), typeof(LoadPmPopupView));
        }

        public LoadPmPopupViewModel()
        {
            LoadPorts = GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<LoadPort>().ToList();
            ProcessModules = GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<DriveableProcessModule>().ToList();

            if (ProcessModules.Count > 0)
            {
                SelectedProcessModule = ProcessModules.First();
            }

            foreach (var loadPort in LoadPorts)
            {
                SubstrateSelectionViewers.Add(new LoadPmSubstrateSelectionViewModel(loadPort));
            }
        }

        #region Properties

        public List<LoadPort> LoadPorts { get; }

        public List<DriveableProcessModule> ProcessModules { get; }

        public DriveableProcessModule SelectedProcessModule { get; set; }

        public List<LoadPmSubstrateSelectionViewModel> SubstrateSelectionViewers { get; } = new();

        private RobotArm _robotArm;
        public RobotArm RobotArm
        {
            get { return _robotArm; }
            set { SetAndRaiseIfChanged(ref _robotArm, value); }
        }
        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            LoadPorts.Clear();
            SubstrateSelectionViewers.Clear();
        }

        #endregion
    }
}
