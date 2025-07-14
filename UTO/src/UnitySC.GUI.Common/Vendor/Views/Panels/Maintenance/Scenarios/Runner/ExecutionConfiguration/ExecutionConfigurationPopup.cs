using Agileo.GUI.Components;

namespace UnitySC.GUI.Common.Vendor.Views.Panels.Maintenance.Scenarios.Runner.ExecutionConfiguration
{
    /// <summary>
    /// Template Model representing the ViewModel (DataContext) of <see cref="ExecutionConfigurationPopupView"/>
    /// </summary>
    public class ExecutionConfigurationPopup : Notifier
    {
        private int _numberOfExecution;

        public int NumberOfExecution
        {
            get { return _numberOfExecution; }
            set { SetAndRaiseIfChanged(ref _numberOfExecution, value); }
        }

        private bool _loopModeEnabled;

        public bool LoopModeEnabled
        {
            get { return _loopModeEnabled; }
            set { SetAndRaiseIfChanged(ref _loopModeEnabled, value); }
        }
    }
}

