using UnitySC.PM.ANA.Client.Proxy;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.UI.Proxy;
using UnitySC.PM.Shared.UI.ViewModels;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;
using UnitySC.Shared.UI.AutoRelayCommandExt;

namespace UnitySC.PM.ANA.Client.Modules.TestHardware.ViewModel
{
    public class IOsViewModel : TabViewModelBase
    {
        private ControllersSupervisor _controllerSupervisor;
        private AlgosSupervisor _algosSupervisor;

        public IOsViewModel()
        {
            _controllerSupervisor = ClassLocator.Default.GetInstance<ControllersSupervisor>();
            _algosSupervisor = ClassLocator.Default.GetInstance<AlgosSupervisor>();
            ClassLocator.Default.GetInstance<AlgosSupervisor>().CheckWaferPresenceChangedEvent += CheckWaferPresenceVM_ChckWaferPresenceChangedEvent;
        }

        private AutoRelayCommand _checkWaferPresenceCommand;

        public AutoRelayCommand CheckWaferPresenceCommand
        {
            get
            {
                return _checkWaferPresenceCommand ?? (_checkWaferPresenceCommand = new AutoRelayCommand(
                () =>
                {
                    CheckWaferPresence = "";
                    var input = new CheckWaferPresenceInput(new Length(300, LengthUnit.Millimeter)); //TODO: Selectonner la taille du wafer à checker. ANA => 300mm pour le moment
                    _algosSupervisor.CheckWaferPresence(input);
                },
                () => true));
            }
        }

        private void CheckWaferPresenceVM_ChckWaferPresenceChangedEvent(CheckWaferPresenceResult waferPresenceResult)
        {
            if (waferPresenceResult.Status.IsFinished)
            {
                if (waferPresenceResult.IsWaferPresent)
                {
                    CheckWaferPresence = "Wafer is present";
                }
                else
                {
                    CheckWaferPresence = "Wafer isn't present";
                }
                OnPropertyChanged(nameof(CheckWaferPresence));
            }
        }

        private string _checkWaferPresence;

        public string CheckWaferPresence
        {
            get => _checkWaferPresence; set { if (_checkWaferPresence != value) { _checkWaferPresence = value; OnPropertyChanged(); } }
        }
    }
}
