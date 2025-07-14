using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.ClientProxy.Chamber;
using UnitySC.PM.Shared.Hardware.ClientProxy.FDC;
using UnitySC.PM.Shared.Hardware.ClientProxy.Global;
using UnitySC.PM.Shared.Hardware.ClientProxy.Plc;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;

namespace UnitySC.PM.Shared.UI.Proxy
{
    public class SharedSupervisors
    {
        public SharedSupervisors()
        {
            _globalStatus = new Dictionary<ActorType, GlobalStatusSupervisor>();
            _pMUsers = new Dictionary<ActorType, IUserSupervisor>();
            _chambers = new Dictionary<ActorType, ChamberSupervisor>();
            _plcs = new Dictionary<ActorType, PlcSupervisor>();
            _devices = new Dictionary<ActorType, GlobalDeviceSupervisor>();
            _fdcs = new Dictionary<ActorType, FDCSupervisor>();
            _clientFdcs = new Dictionary<ActorType, ClientFDCsSupervisor>();
            _dbMaintenances = new Dictionary<ActorType, DBMaintenanceSupervisor>();
        }

        private Dictionary<ActorType, GlobalStatusSupervisor> _globalStatus;
        private Dictionary<ActorType, IUserSupervisor> _pMUsers;
        private Dictionary<ActorType, ChamberSupervisor> _chambers;
        private Dictionary<ActorType, PlcSupervisor> _plcs;
        private Dictionary<ActorType, GlobalDeviceSupervisor> _devices;
        private Dictionary<ActorType, FDCSupervisor> _fdcs;
        private Dictionary<ActorType, ClientFDCsSupervisor> _clientFdcs;
        private Dictionary<ActorType, DBMaintenanceSupervisor> _dbMaintenances;


        public GlobalDeviceSupervisor GetGlobalDeviceSupervisor(ActorType actorType)
        {
            GlobalDeviceSupervisor res;
            if (!_devices.TryGetValue(actorType, out res))
            {
                res = new GlobalDeviceSupervisor(ClassLocator.Default.GetInstance<ILogger<GlobalDeviceSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>(), actorType);
                _devices.Add(actorType, res);
            }
            return res;
        }

        public GlobalStatusSupervisor GetGlobalStatusSupervisor(ActorType actorType)
        {
            GlobalStatusSupervisor res;
            if (!_globalStatus.TryGetValue(actorType, out res))
            {
                res = new GlobalStatusSupervisor(actorType, true, ClassLocator.Default.GetInstance<ILogger<GlobalStatusSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>());
                _globalStatus.Add(actorType, res);
            }
            return res;
        }

        public IUserSupervisor GetUserSupervisor(ActorType actorType)
        {
            IUserSupervisor res;
            if (!_pMUsers.TryGetValue(actorType, out res))
            {
                res = new PMUserSupervisor(ClassLocator.Default.GetInstance<SerilogLogger<IUserSupervisor>>(), actorType);
                _pMUsers.Add(actorType, res);
            }
            return res;
        }

        public ChamberSupervisor GetChamberSupervisor(ActorType actorType)
        {
            ChamberSupervisor res;
            if (!_chambers.TryGetValue(actorType, out res))
            {
                res = new ChamberSupervisor(ClassLocator.Default.GetInstance<IMessenger>(), actorType);
                _chambers.Add(actorType, res);
            }
            return res;
        }

        public FDCSupervisor GetFDCSupervisor(ActorType actorType)
        {
            FDCSupervisor res;
            if (!_fdcs.TryGetValue(actorType, out res))
            {
                res = new FDCSupervisor(ClassLocator.Default.GetInstance<ILogger<FDCSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>(), actorType);
                _fdcs.Add(actorType, res);
            }
            return res;
        }

        public ClientFDCsSupervisor GetClientFDCsSupervisor(ActorType actorType)
        {
            ClientFDCsSupervisor res;
            if (!_clientFdcs.TryGetValue(actorType, out res))
            {
                res = new ClientFDCsSupervisor(ClassLocator.Default.GetInstance<ILogger<ClientFDCsSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>(), actorType);
                _clientFdcs.Add(actorType, res);
            }
            return res;
        }

        public DBMaintenanceSupervisor GetDBMaintenanceSupervisor(ActorType actorType)
        {
            DBMaintenanceSupervisor res;
            if (!_dbMaintenances.TryGetValue(actorType, out res))
            {
                res = new DBMaintenanceSupervisor(ClassLocator.Default.GetInstance<ILogger<DBMaintenanceSupervisor>>(), ClassLocator.Default.GetInstance<IMessenger>());
                _dbMaintenances.Add(actorType, res);
            }
            return res;
        }
    }
}
