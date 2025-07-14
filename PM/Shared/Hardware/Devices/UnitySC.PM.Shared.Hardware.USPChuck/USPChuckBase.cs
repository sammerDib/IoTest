using System;
using System.Collections.Generic;

using UnitySC.PM.Shared.Hardware.Core;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.USPChuck
{
    public abstract class USPChuckBase : DeviceBase, IUSPChuck                                                 
    {
#pragma warning disable CS0067 // Event not used. Not detected by the compiler

        public event ChuckStateChangedDelegate ChuckStateChangedEvent;

        public override DeviceFamily Family => DeviceFamily.Chuck;

        public IChuckController ChuckController { get; set; } // To get and access to all child controllers type of all child chucks.
                                                              // Interface is used to acces of a part of a global controller (as ACSController) concerning chuck only
        public virtual ChuckBaseConfig Configuration { get; set; }

        public abstract bool IsSensorPresenceEnable(Length size);

        // For test purpose only
        public USPChuckBase(IGlobalStatusServer globalStatusServer, ILogger logger) : base(globalStatusServer, logger)
        {
        }

        public USPChuckBase(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config, IChuckController controller)
             : base(globalStatusServer, logger)
        {
            Configuration = config;
            ChuckController = controller;
        }

        public virtual void Init()
        {
            Name = Configuration.Name;
            DeviceID = Configuration.DeviceID;
            base.Logger.Information($"Init device {Family}-{Name}");
        }

        public abstract ChuckState GetState();
        public abstract void TriggerUpdateEvent();
        public abstract bool IsMaterialPresenceRefreshed { get; }

        public abstract List<Length> GetMaterialDiametersSupported();
        protected ChuckState CreateDefaultChuckStateFromConfig()
        {
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            foreach (var subSlotConfig in Configuration.GetSubstrateSlotConfigs())
            {
                clampStates.Add(subSlotConfig.Diameter, false);
                presenceStates.Add(subSlotConfig.Diameter, MaterialPresence.Unknown);
            }
            return new ChuckState(clampStates, presenceStates);
        }
    }
}
