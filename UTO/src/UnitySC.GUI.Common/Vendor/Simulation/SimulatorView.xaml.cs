using System.ComponentModel;
using System.Linq;
using System.Windows;

using Agileo.EquipmentModeling;

namespace UnitySC.GUI.Common.Vendor.Simulation
{
    public partial class SimulatorView : Window
    {
        private readonly SimulatorViewModel _viewModel;
        private bool _forceClose;

        private SimulatorView(Package package)
        {
            InitializeComponent();

            _viewModel = new SimulatorViewModel();
            _viewModel.Build(package);
            DataContext = _viewModel;
        }

        /// <summary>
        /// Creates and opens the simulator view.
        /// </summary>
        /// <param name="package">The package with equipments and devices with simulation view: <see cref="ISimDevice"/>.</param>
        /// <returns>The <see cref="SimulatorView"/> if any simulated devices; otherwise <c>null</c>.</returns>
        public static SimulatorView Open(Package package)
        {
            // Create and show SimulatorView only if there is ISimDevice executed in SimulationMode
            if (package?.GetDevicesWithSimView()
                    .All(simDevice => (simDevice as Agileo.EquipmentModeling.Device)?.ExecutionMode != ExecutionMode.Simulated) != false)
            {
                return null;
            }

            var simView = new SimulatorView(package);
            simView.Show();
            return simView;
        }

        /// <summary>
        /// Force the close of the view.
        /// </summary>
        public void ForceClose()
        {
            _forceClose = true;
            Close();
        }

        /// <inheritdoc />
        protected override void OnClosing(CancelEventArgs e)
        {
            if (_forceClose)
            {
                base.OnClosing(e);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
