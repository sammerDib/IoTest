using UnitySC.Shared.Tools;

namespace ADCConfiguration.Services
{
    public class Services
    {
        public static Services Instance { get; } = new Services();

        public Services() { }

        public static void Register()
        {
            ClassLocator.Default.Register<PopUpService>(true);
            ClassLocator.Default.Register<NavigationService>(true);
            ClassLocator.Default.Register<ShutdownService>(true);
            ClassLocator.Default.Register<AuthentificationService>(true);
            ClassLocator.Default.Register<LogService>(true);
            ClassLocator.Default.Register<FileService>(true);
            ClassLocator.Default.Register<MapperService>(true);
            ClassLocator.Default.Register<MilService>(true);
        }

        public NavigationService NavigationService { get { return ClassLocator.Default.GetInstance<NavigationService>(); } }
        public PopUpService PopUpService { get { return ClassLocator.Default.GetInstance<PopUpService>(); } }
        public ShutdownService ShutdownService { get { return ClassLocator.Default.GetInstance<ShutdownService>(); } }
        public AuthentificationService AuthentificationService { get { return ClassLocator.Default.GetInstance<AuthentificationService>(); } }
        public LogService LogService { get { return ClassLocator.Default.GetInstance<LogService>(); } }
        public FileService FileService { get { return ClassLocator.Default.GetInstance<FileService>(); } }
        public MapperService MapperService { get { return ClassLocator.Default.GetInstance<MapperService>(); } }
        public MilService MilService { get { return ClassLocator.Default.GetInstance<MilService>(); } }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
