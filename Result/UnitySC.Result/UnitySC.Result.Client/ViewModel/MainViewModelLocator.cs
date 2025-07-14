using UnitySC.Result.CommonUI.ViewModel;
using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.Client.ViewModel
{
    public class MainViewModelLocator
    {
        public static MainViewModelLocator Instance { get; private set; }
        public MainResultVM MainResultVM => ClassLocator.Default.GetInstance<MainResultVM>();
        public NotifierVM NotifierVM => ClassLocator.Default.GetInstance<NotifierVM>();

        public MainViewModelLocator()
        {
            Instance = this;
        }

        public static void Register()
        {
            ClassLocator.Default.Register<MainResultVM>(true);
            ClassLocator.Default.Register<NotifierVM>(true);
        }
    }
}