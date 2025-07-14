using UnitySC.ADCAS300Like.Service;
using UnitySC.ADCAS300Like.Service.ADCInterfaces;
using UnitySC.dataflow.Service.Implementation;
using UnitySC.Dataflow.Operations.Implementation;
using UnitySC.Dataflow.Operations.Implementation.UTODF;
using UnitySC.Dataflow.Operations.Interface;
using UnitySC.Dataflow.Operations.Interface.UTODF;
using UnitySC.Dataflow.Service.Implementation;
using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Dataflow.PM.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.TC.PM.Operations.Interface.UTOOperations;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.Dataflow.Manager
{
    public static class Bootstrapper_Service
    {
        public static object BootstrapperLock = new object();
        public static bool IsRegister { get; private set; }
   
        public static void Register()
        {
            lock (BootstrapperLock)
            {
                if (!IsRegister)
                {                   
                    ClassLocator.Default.Register<IAlarmOperations, AlarmOperations>(singleton: true);
                    ClassLocator.Default.Register<IEquipmentConstantOperations, EquipmentConstantOperations>(singleton: true);
                    ClassLocator.Default.Register<ICommonEventOperations, CommonEventOperations>(singleton: true);
                    ClassLocator.Default.Register<ICommunicationOperations, CommunicationOperations>(singleton: true);
                    ClassLocator.Default.Register<ICommunicationOperationsCB, DFManager>(singleton: true);
                    ClassLocator.Default.Register<IAlarmOperationsCB, DFManager>(singleton: true);

                    ClassLocator.Default.Register<IUTODFOperations, UTODFOperations>(singleton: true);
                    ClassLocator.Default.Register<IRecipeOperations, RecipeOperations>(singleton: true);                   
                    ClassLocator.Default.Register<IPMDFOperations, PMDFOperations>(singleton: true);

                    ClassLocator.Default.Register(typeof(IDFStatusVariableOperations), typeof(DFStatusVariableOperations), true);

                    ClassLocator.Default.Register<IUTODFService, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<IUTODFServiceCB, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<IStatusVariableServiceCB, UTODFService>(singleton: true);                    
                    ClassLocator.Default.Register<IAlarmService, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<IAlarmServiceCB, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<ICommonEventService, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<IEquipmentConstantService, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<IStatusVariableService, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<ICommonEventServiceCB, UTODFService>(singleton: true);
                    ClassLocator.Default.Register<IEquipmentConstantServiceCB, UTODFService>(singleton: true);                                     

                    ClassLocator.Default.Register<IPMDFService, PMDFService>(singleton: true);
                    ClassLocator.Default.Register<IPMDFServiceCB, PMDFService>(singleton: true);

                    ClassLocator.Default.Register<IDAP, DAPService>(singleton: true);

                    //Dataflow Manager
                    ClassLocator.Default.Register<IDFManager, DFManager>(singleton: true);
                    ClassLocator.Default.Register<IDFPostProcessCB, DFManager>(singleton: true);

                    ClassLocator.Default.Register<IADC, CADCService>(singleton: true);

                    IsRegister = true;
                }
            }
        }
    }
}
