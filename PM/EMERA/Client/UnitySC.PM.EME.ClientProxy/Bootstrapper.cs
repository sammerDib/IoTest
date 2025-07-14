using UnitySC.PM.EME.Client.Proxy.Algo;
using UnitySC.PM.EME.Client.Proxy.Axes;
using UnitySC.PM.EME.Client.Proxy.Chamber;
using UnitySC.PM.EME.Client.Proxy.Chiller;
using UnitySC.PM.EME.Service.Interface.Algo;
using UnitySC.PM.EME.Service.Interface.Axes;
using UnitySC.PM.EME.Service.Interface.Chiller;
using UnitySC.PM.Shared.Hardware.ClientProxy.Referential;
using UnitySC.PM.Shared.Hardware.Service.Interface.Referential;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.Client.Proxy
{
    public static class Bootstrapper
    {
        public static void Register()
        {                        
            RegisterReferential();
            RegisterMotionAxes();
            RegisterChuck();           
            RegisterChamber();
            RegisterChiller();
            RegisterAlgoSupervisor();
        }

        private static void RegisterAlgoSupervisor()
        {
            ClassLocator.Default.Register<AlgoSupervisor>(true);
            ClassLocator.Default.Register<IAlgoSupervisor, AlgoSupervisor>(true);
        }

        private static void RegisterChamber()
        {
            ClassLocator.Default.Register<ChamberVM>(true);
            ClassLocator.Default.Register<ChamberSupervisor>(true);
        }   
        
        private static void RegisterChiller()
        {
            ClassLocator.Default.Register<ChillerSupervisor>(true);
            ClassLocator.Default.Register<IChillerService, ChillerSupervisor>(true);
            ClassLocator.Default.Register<IChillerSupervisor, ChillerSupervisor>(true);
        }
        
        private static void RegisterChuck()
        {
           
        }

        private static void RegisterMotionAxes()
        {
            ClassLocator.Default.Register<IEmeraMotionAxesService, EmeraMotionAxesSupervisor>(true);
            ClassLocator.Default.Register<EmeraMotionAxesSupervisor>(true);
            ClassLocator.Default.Register<AxesVM>(true);
        }

        private static void RegisterReferential()
        {
            ClassLocator.Default.Register<IReferentialService, ReferentialSupervisor>(true);
            ClassLocator.Default.Register<ReferentialSupervisor>(true);
        }           
       
    }
}
