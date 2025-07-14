using System;
using System.Linq;
using System.Windows.Input;

using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Services.Icons;

using UnitySC.DataFlow.ProcessModules.Devices.DataFlowManager;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Views.Panels.Setup.AppPathConfig;

namespace UnitySC.UTO.Controller.Views.Panels.Setup.UtoAppPathConfig
{
    public class UtoAppPathConfigPanel : AppPathConfigPanel
    {
        #region Constructors

        static UtoAppPathConfigPanel()
        {
            DataTemplateGenerator.Create(typeof(UtoAppPathConfigPanel), typeof(UtoAppPathConfigView));
        }

        public UtoAppPathConfigPanel(string id, IIcon icon = null)
            : base(id, icon)
        {
        }

        #endregion

        #region Properties

        public bool IsDfClientConfigVisible
            => GUI.Common.App.Instance.EquipmentManager.Equipment.AllOfType<DataFlowManager>()
                .Any();

        public string DfClientConfigurationFolderPath
        {
            get => ModifiedApplicationPathConfig.DfClientConfigurationFolderPath;
            set
            {
                if (string.Equals(
                        ModifiedApplicationPathConfig.DfClientConfigurationFolderPath,
                        value,
                        StringComparison.Ordinal))
                {
                    return;
                }

                ModifiedApplicationPathConfig.DfClientConfigurationFolderPath = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Commands

        private DelegateCommand _defineDfClientConfigPathCommand;

        public ICommand DefineDfClientConfigPathCommand
            => _defineDfClientConfigPathCommand ??=
                new DelegateCommand(DefineDfClientConfigPathExecute);

        private void DefineDfClientConfigPathExecute()
        {
            ShowOpenFolderDialog<UtoAppPathConfigPanel>(p => p.DfClientConfigurationFolderPath);
        }

        #endregion
    }
}
