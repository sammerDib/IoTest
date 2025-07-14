using UnitySC.Result.CommonUI.Proxy;
using UnitySC.Result.CommonUI.ViewModel;
using UnitySC.Shared.Tools;

namespace UnitySC.Result.CommonUI
{
    public class Bootstrapper
    {
        public static void Register()
        {
            // Service interne pour la gestion des format de resultats
            ClassLocator.Default.Register(typeof(Shared.Format.Base.IResultDataFactory), typeof(Shared.Format.Factory.ResultDataFactory), true);

            ClassLocator.Default.Register<ResultSupervisor>(true);
        }

        public static void Init()
        {
            ClassLocator.Default.GetInstance<MainResultVM>().Init();
        }
    }
}