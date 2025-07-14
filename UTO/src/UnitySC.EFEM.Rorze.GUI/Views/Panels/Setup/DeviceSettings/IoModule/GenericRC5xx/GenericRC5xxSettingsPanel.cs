using System;
using System.Collections.ObjectModel;
using System.Linq;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Services.Icons;

using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio1MediumSizeEfem;
using UnitySC.EFEM.Rorze.Devices.IoModule.RC530.Dio2;
using UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.Dio0;
using UnitySC.GUI.Common;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.Views.Panels.Setup;

namespace UnitySC.EFEM.Rorze.GUI.Views.Panels.Setup.DeviceSettings.IoModule.GenericRC5xx
{
    public class GenericRC5xxSettingsPanel : SetupPanel
    {
        #region Constructors

        static GenericRC5xxSettingsPanel()
        {
            DataTemplateGenerator.Create(typeof(GenericRC5xxSettingsPanel), typeof(GenericRC5xxSettingsPanelView));
            LocalizationManager.AddLocalizationProvider(
                new ResourceFileProvider(typeof(IoModuleSettingsResources)));
        }

        public GenericRC5xxSettingsPanel()
            : this("DesignTime Constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException(
                    "Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        public GenericRC5xxSettingsPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
        }

        #endregion

        #region Properties

        public ObservableCollection<IGenericRC5xxSettingsEditor> IoModules { get; } = new();

        protected override bool ChangesNeedRestart => true;

        #endregion

        #region Private Methods

        private void LoadConfiguration()
        {
            IoModules.Clear();

            var devices = App.Instance.EquipmentManager.Equipment
                .AllDevices<Devices.IoModule.GenericRC5xx.GenericRC5xx>();

            foreach (var device in devices)
            {
                switch (device)
                {
                    case Devices.IoModule.RC550.Dio0.Dio0 dio0:
                        var dio0SettingsEditor = new Dio0SettingsEditor<Devices.IoModule.RC550.Dio0.Dio0>(dio0, 0, Logger);
                        IoModules.Add(dio0SettingsEditor);
                        dio0SettingsEditor.OnSetup();
                        break;

                    case Dio1 dio1:
                        var dio1SettingsEditor = new GenericRC5xxSettingsEditor<Dio1>(dio1, 1, Logger);
                        IoModules.Add(dio1SettingsEditor);
                        dio1SettingsEditor.OnSetup();
                        break;

                    case Dio1MediumSizeEfem dio1MediumSize:
                        var dio1MediumSizeSettingsEditor = new GenericRC5xxSettingsEditor<Dio1MediumSizeEfem>(dio1MediumSize, 1, Logger);
                        IoModules.Add(dio1MediumSizeSettingsEditor);
                        dio1MediumSizeSettingsEditor.OnSetup();
                        break;

                    case Dio2 dio2:
                        var dio2SettingsEditor = new GenericRC5xxSettingsEditor<Dio2>(dio2, 2, Logger);
                        IoModules.Add(dio2SettingsEditor);
                        dio2SettingsEditor.OnSetup();
                        break;
                }
            }
        }

        #endregion

        #region Overrides of SetupPanel<ApplicationConfiguration>

        public override void OnSetup()
        {
            base.OnSetup();
            LoadConfiguration();
        }

        protected override void UndoChanges()
        {
            foreach (var ioModule in IoModules)
            {
                ioModule.UndoChanges();
            }
        }

        protected override bool ConfigurationEqualsCurrent()
        {
            return IoModules.All(editor => editor.ConfigurationEqualsCurrent());
        }

        protected override void SaveConfig()
        {
            foreach (var ioModule in IoModules)
            {
                ioModule.SaveConfig();
            }
        }

        public override bool SaveCommandCanExecute()
        {
            return base.SaveCommandCanExecute()
                   && IoModules.All(io => !io.CommunicationConfig.HasErrors);
        }

        #endregion
    }
}
