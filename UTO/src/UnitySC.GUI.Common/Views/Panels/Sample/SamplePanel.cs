using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Agileo.Common.Localization;
using Agileo.EquipmentModeling;
using Agileo.GUI.Commands;
using Agileo.GUI.Components;
using Agileo.GUI.Components.Commands;
using Agileo.GUI.Components.Navigations;
using Agileo.GUI.Services.Icons;
using Agileo.GUI.Services.Popups;

using UnitySC.Equipment.Abstractions.Devices.ProcessModule;
using UnitySC.GUI.Common.Vendor.Helpers;
using UnitySC.GUI.Common.Vendor.UIComponents.XamlResources.Shared;

namespace UnitySC.GUI.Common.Views.Panels.Sample
{
    public class SamplePanel : BusinessPanel
    {
        #region DesignTime constructor

        public SamplePanel() : base("DesignTime constructor")
        {
            if (!IsInDesignMode)
            {
                throw new InvalidOperationException("Default constructor (without parameter) is only used for the Design Mode. Please use constructor with parameters.");
            }
        }

        #endregion

        static SamplePanel()
        {
            DataTemplateGenerator.Create(typeof(SamplePanel), typeof(SamplePanelView));
            LocalizationManager.AddLocalizationProvider(new ResourceFileProvider(typeof(SamplePanelResources)));
        }

        public SamplePanel(string relativeId, IIcon icon = null) : base(relativeId, icon)
        {
            // Add command to panel
            Commands.Add(new BusinessPanelCommand(nameof(SamplePanelResources.BP_SAMPLE_CMD), SampleCommand, PathIcon.Maintenance));
        }

        #region Commands

        #region BusinessPanelCommand

        private ICommand _sampleCommand;

        /// <summary>
        /// Command
        /// </summary>
        public ICommand SampleCommand
        {
            get
            {
                if (_sampleCommand != null)
                {
                    return _sampleCommand;
                }

                _sampleCommand = new DelegateCommand(SampleCommandExecuteMethod, SampleCommandCanExecuteMethod);

                return _sampleCommand;
            }
        }


        private void SampleCommandExecuteMethod()
        {
            // custom execution code here
        }


        private bool SampleCommandCanExecuteMethod()
        {
            return true; // custom condition here
        }

        #endregion

        #region MvvmCommand

        private ICommand _mvvmCommand;

        /// <summary>
        /// Command
        /// </summary>
        public ICommand MvvmCommand
        {
            get
            {
                if (_mvvmCommand != null)
                {
                    return _mvvmCommand;
                }

                _mvvmCommand = new DelegateCommand(MvvmCommandExecuteMethod, MvvmCommandCanExecuteMethod);

                return _mvvmCommand;
            }
        }

        private void MvvmCommandExecuteMethod()
        {
            App.Instance.EquipmentManager.Equipment.AllDevices<ProcessModule>().FirstOrDefault()?.StartCommunication();
        }

        private bool MvvmCommandCanExecuteMethod()
        {
            // custom condition here
            return true;
        }

        private ICommand _showBusyIndicatorCommand;

        public ICommand ShowBusyIndicatorCommand
            => _showBusyIndicatorCommand ??= new DelegateCommand(
                ShowBusyIndicatorCommandExecute,
                ShowBusyIndicatorCommandCanExecute);

        private bool ShowBusyIndicatorCommandCanExecute()
        {
            return BusyIndicatorWaitingTime>-1 && BusyIndicatorWaitingTime<10000;
        }

        private void ShowBusyIndicatorCommandExecute()
        {
            Task.Run(() => Popups.ShowDuring(new BusyIndicator((IText)null), () => Thread.Sleep(BusyIndicatorWaitingTime)));
        }

        #endregion

        #endregion

        #region Properties

        private int _busyIndicatorWaitingTime = 20;
        public int BusyIndicatorWaitingTime
        {
            get { return _busyIndicatorWaitingTime; }
            set { SetAndRaiseIfChanged(ref _busyIndicatorWaitingTime, value); }
        }

        #endregion
    }
}
