using UnitySC.Shared.Tools;
using UnitySC.Shared.UI.ViewModel;

namespace UnitySC.Result.CommonUI.ViewModel
{
    public class ViewModelLocator
    {
        public static ViewModelLocator Instance { get; private set; }
        public MainResultVM MainResultVM => ClassLocator.Default.GetInstance<MainResultVM>();
        public NotifierVM NotifierVM => ClassLocator.Default.GetInstance<NotifierVM>();

        public ViewModelLocator()
        {
            Instance = this;
        }
    }
}