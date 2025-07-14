using UnitySC.Shared.TC.PM.Operations.Implementation;
using UnitySC.Shared.TC.PM.Operations.Interface;
using UnitySC.Shared.TC.PM.Service.Implementation;
using UnitySC.Shared.TC.PM.Service.Interface;
using UnitySC.Shared.TC.Shared.Data;
using UnitySC.Shared.TC.Shared.Operations.Implementation;
using UnitySC.Shared.TC.Shared.Operations.Interface;
using UnitySC.Shared.TC.Shared.Service.Interface;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.EME.TC
{
    public static class Bootstrapper
    {
        public static object BootstrapperLock = new object();
        public static bool IsRegister { get; private set; }

        public static void Register()
        {
            lock (BootstrapperLock)
            {
                if (!IsRegister)
                {
                    // Services
                    ClassLocator.Default.Register<PMTransferManager, PMTransferManager>(singleton: true);
                    ClassLocator.Default.Register<IPMStateManager, PMTransferManager>(singleton: true);
                    ClassLocator.Default.Register<IAlarmOperationsCB, EMEPMTCManager>(singleton: true);
                    ClassLocator.Default.Register<ICommunicationOperationsCB, PMTransferManager>(singleton: true);
                    ClassLocator.Default.Register<IMaterialOperationsCB, PMTransferManager>(singleton: true);
                    ClassLocator.Default.Register<IPMHandlingStatesChangedCB, PMTransferManager>(singleton: true);

                    ClassLocator.Default.Register<IAlarmOperations, AlarmOperations>(singleton: true);
                    ClassLocator.Default.Register<ICommonEventOperations, CommonEventOperations>(singleton: true);
                    ClassLocator.Default.Register<IEquipmentConstantOperations, EquipmentConstantOperations>(singleton: true);
                    ClassLocator.Default.Register<IMaterialOperations, MaterialOperations>(singleton: true);
                    ClassLocator.Default.Register<IPMStatusVariableOperations, EMEStatusVariableOperations>(singleton: true);
                    ClassLocator.Default.Register<ICommunicationOperations, CommunicationOperations>(singleton: true);
                    ClassLocator.Default.Register<IUTOPMOperations, UTOPMOperations>(singleton: true);

                    ClassLocator.Default.Register<IUTOPMService, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IUTOPMServiceCB, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IAlarmService, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<ICommonEventService, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IEquipmentConstantService, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IMaterialService, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IStatusVariableService, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IAlarmServiceCB, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<ICommonEventServiceCB, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IEquipmentConstantServiceCB, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IMaterialServiceCB, UTOPMService>(singleton: true);
                    ClassLocator.Default.Register<IStatusVariableServiceCB, UTOPMService>(singleton: true);

                    ClassLocator.Default.Register(typeof(IPMTCManager), typeof(EMEPMTCManager), singleton: true);
                    ClassLocator.Default.Register(typeof(IEMEHandling), typeof(EMEHandlingManager), singleton: true);
                    

                    IsRegister = true;
                }
            }
        }
    }
}

