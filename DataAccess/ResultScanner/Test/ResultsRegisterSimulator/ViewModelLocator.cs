using UnitySC.Shared.Tools;

namespace ResultsRegisterSimulator
{
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; private set; }
        public MainRegisterVM MainRegisterVM => ClassLocator.Default.GetInstance<MainRegisterVM>();

        public ViewModelLocator()
        {
            Instance = this;
        }

        public static void Register()
        {
            ClassLocator.Default.Register<MainRegisterVM>(true);
        }
    }
}
