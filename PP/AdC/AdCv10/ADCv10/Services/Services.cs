using UnitySC.Shared.Tools;

namespace ADC.Services
{
    public class Services
    {

        public static Services Instance { get; } = new Services();


        public Services()
        {

        }

        public static void Register()
        {
            ClassLocator.Default.Register<ShutdownService>(true);
            ClassLocator.Default.Register<PopUpService>(true);
        }


        public ShutdownService ShutdownService { get { return ClassLocator.Default.GetInstance<ShutdownService>(); } }
        public PopUpService PopUpService { get { return ClassLocator.Default.GetInstance<PopUpService>(); } }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
