using System;
using System.Collections.Generic;
using System.Linq;

using Agileo.EquipmentModeling;
using Agileo.GUI.Components;

using UnitySC.Equipment.Abstractions.Devices.AbstractDataFlowManager;
using UnitySC.Equipment.Abstractions.Devices.LoadPort;
using UnitySC.Equipment.Devices.Controller.JobDefinition;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.UTO.Controller.Views.Panels.DataFlow.Recipes;

namespace UnitySC.UTO.Controller.Views.Panels.Production.Equipment.Popup
{
    public class StartProcessJobPopupViewModel : Notifier, IDisposable
    {
        static StartProcessJobPopupViewModel()
        {
            DataTemplateGenerator.Create(typeof(StartProcessJobPopupViewModel), typeof(StartProcessJobPopupView));
        }

        public StartProcessJobPopupViewModel()
        {
            DataFlowTree = new DataFlowTree(
                App.ControllerInstance.ControllerEquipmentManager.Controller
                    .TryGetDevice<AbstractDataFlowManager>());

            OcrProfiles = new List<OcrProfile>(App.ControllerInstance.ControllerConfig.OcrProfiles);

            LoadPorts = GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<LoadPort>().ToList();

            foreach (var loadPort in LoadPorts)
            {
                SubstrateSelectionViewers.Add(new SubstrateSelectionViewModel(loadPort));
            }

            NumberOfExecutions = 1;
            LoopMode = false;
        }

        #region Properties

        public List<LoadPort> LoadPorts { get; }

        public List<SubstrateSelectionViewModel> SubstrateSelectionViewers { get; } = new();

        public DataFlowTree DataFlowTree { get; }
        public List<OcrProfile> OcrProfiles { get; }

        private uint _numberOfExecutions;

        public uint NumberOfExecutions
        {
            get { return _numberOfExecutions; }
            set { SetAndRaiseIfChanged(ref _numberOfExecutions, value); }
        }

        private bool _loopMode;

        public bool LoopMode
        {
            get { return _loopMode; }
            set { SetAndRaiseIfChanged(ref _loopMode, value); }
        }

        private bool _cyclingMode;

        public bool CyclingMode
        {
            get { return _cyclingMode; }
            set { SetAndRaiseIfChanged(ref _cyclingMode, value); }
        }

        private bool _ocrReading;

        public bool OcrReading
        {
            get { return _ocrReading; }
            set
            {
                SetAndRaiseIfChanged(ref _ocrReading, value);
                if (!_ocrReading)
                {
                    SelectedProfile = null;
                }
            }
        }

        private OcrProfile _selectedProfile;

        public OcrProfile SelectedProfile
        {
            get { return _selectedProfile; }
            set { SetAndRaiseIfChanged(ref _selectedProfile, value); }
        }

        public bool IsOcrPresent
            => App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderFront != null
               || App.ControllerInstance.ControllerEquipmentManager.SubstrateIdReaderBack != null;

        #endregion

        #region Public

        public bool ValidateJob()
        {
            return DataFlowTree.SelectedRecipe != null && (!OcrReading || (OcrReading && SelectedProfile != null));
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
            DataFlowTree?.Dispose();
            LoadPorts.Clear();
            SubstrateSelectionViewers.Clear();
        }

        #endregion
    }
}
