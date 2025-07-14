using Agileo.EquipmentModeling;

namespace UnitySC.Equipment.Abstractions.Devices.LoadPort.Simulation
{
    /// <summary>
    /// Interaction logic for LoadPortSimulationView.xaml
    /// </summary>
    public partial class LoadPortSimulationView : ISimDeviceView
    {
        public LoadPortSimulationView(LoadPortSimulationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
