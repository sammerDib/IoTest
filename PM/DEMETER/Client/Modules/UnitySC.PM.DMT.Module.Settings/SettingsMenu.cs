using System;
using System.Collections.Generic;
using System.Windows.Controls;

using UnitySC.PM.DMT.Modules.Settings.View;
using UnitySC.PM.DMT.Modules.Settings.ViewModel;
using UnitySC.PM.Shared.UI.Main;
using UnitySC.Shared.Data;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.DMT.Modules.Settings
{
    public class SettingsMenu : IMenuItem
    {
        private IMenuContentViewModel _viewModel;
        private UserControl _userControl;
        public string Name => "Calibration";
        public string Description => "Tool calibration";
        public string Group => "Settings";
        public string ImageResourceKey => "GearGeometry";

        public int Priority => 310;

        public IMenuContentViewModel ViewModel
        {
            get
            {
                if (_viewModel == null)
                {
                    _viewModel = ClassLocator.Default.GetInstance<SettingsVM>();
                }
                return _viewModel;
            }
        }

        public UserControl UserControl
        {
            get
            {
                if (_userControl == null)
                {
                    _userControl = new SettingsView();
                    _userControl.DataContext = ViewModel;
                }
                return _userControl;
            }
        }

        public bool CanClose()
        {
            if (_viewModel == null)
                return true;
            bool canClose = _viewModel.CanClose();

            if (canClose && _viewModel is IDisposable)
                (_viewModel as IDisposable).Dispose();
            return canClose;
        }

        public IEnumerable<ApplicationMode> CompatibleWith => new List<ApplicationMode>() { ApplicationMode.Maintenance };

        public IEnumerable<UserRights> RequiredRights => new List<UserRights>() { UserRights.Calibration };

        public void ApplicationModeChange(ApplicationMode newMode)
        {
            // Nothing
        }
    }
}
