using UnitySC.PM.AGS.Service.Interface.RecipeService;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.PM.Shared.UserManager.Service.Interface;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.AGS.Service.Implementation
{
    public class ArgosServer : BaseServer
    {
        private IGlobalStatusService _globalStatusService;
        private IPMUserService _pmUserService;
        private IRecipeService _recipeService;

        public ArgosServer(ILogger logger) : base(logger)
        {
            _recipeService = ClassLocator.Default.GetInstance<IRecipeService>();
            _globalStatusService = ClassLocator.Default.GetInstance<IGlobalStatusService>();
            _pmUserService = ClassLocator.Default.GetInstance<IPMUserService>();

            // Todo Ge instance
        }

        public override void Start()
        {
            StartService((BaseService)_recipeService);
            StartService((BaseService)_globalStatusService);
            StartService((BaseService)_pmUserService);

            // Todo start service
        }

        public override void Stop()
        {
            StopAllServiceHost();
        }
    }
}
