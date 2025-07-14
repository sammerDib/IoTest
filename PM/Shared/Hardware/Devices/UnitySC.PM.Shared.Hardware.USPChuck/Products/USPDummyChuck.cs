using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.USPChuck
{
    public class USPDummyChuck : USPChuckBase, IChuckClamp, IChuckMaterialPresence, IChuckAirBearing, IChuckInitialization
    {
        private ChuckState _chuckState = new ChuckState(new Dictionary<Length, bool>(), new Dictionary<Length, MaterialPresence>());

        private IMessenger _messenger;

        protected IMessenger Messenger
        {
            get
            {
                return _messenger ?? (_messenger = ClassLocator.Default.GetInstance<IMessenger>());
            }
        }

        public override bool IsMaterialPresenceRefreshed { get =>  true; }

        public USPDummyChuck(IGlobalStatusServer globalStatusServer, ILogger logger, ChuckBaseConfig config)
            : base(globalStatusServer, logger)
        {
            Configuration = config;
        }

        public override void Init()
        {
            base.Init();
            Logger.Information("Init of USPChuck as dummy");
            InitStatesWithChuckConfiguration();
        }

        public override ChuckState GetState()
        {
            return _chuckState;
        }

        public MaterialPresence CheckWaferPresence(Length size)
        {
            bool success = _chuckState.WaferPresences.TryGetValue(size, out MaterialPresence presence);
            return success ? presence : MaterialPresence.Unknown;
        }

        public void ClampWafer(Length size)
        {
            _chuckState.WaferClampStates[size] = true;
            var chuckServiceCallback = ClassLocator.Default.GetInstance<IChuckServiceCallbackProxy>();
            chuckServiceCallback.StateChanged(_chuckState);
            Messenger.Send(new DataAttributesChuckMessage() { State = _chuckState });
        }

        public void ReleaseWafer(Length size)
        {
            _chuckState.WaferClampStates[size] = false;
            var chuckServiceCallback = ClassLocator.Default.GetInstance<IChuckServiceCallbackProxy>();
            chuckServiceCallback.StateChanged(_chuckState);
            Messenger.Send(new DataAttributesChuckMessage() { State = _chuckState });
        }

        public void InitAirbearing()
        {
            Task.Delay(1000).Wait();
            if (new Random().NextDouble() > 0.9)
            {
                throw new Exception("Arbitrary Exception");
            }
        }

        public Dictionary<string, float> GetAirBearingPressuresValues()
        {
            var pressures = new Dictionary<string, float>
            {
                ["AirbearingVacuumSensor0"] = 10,
                ["AirbearingVacuumSensor1"] = 100,
                ["AirbearingVacuumSensor"] = 1000
            };
            return pressures;
        }

        public override List<Length> GetMaterialDiametersSupported()
        {
            var sizes = new List<Length>();
            var substrateSlotConfigs = Configuration.GetSubstrateSlotConfigs();
            if (substrateSlotConfigs != null)
            {
                foreach (var substSlot in substrateSlotConfigs)
                {
                    sizes.Add(substSlot.Diameter);
                }

                return sizes;
            }
            else
            {
                throw new NotImplementedException("GetSubstrateSlotConfigs is not implemented for this kind of Chuck Configuration");
            }
        }

        public override bool IsSensorPresenceEnable(Length size)
        {
            return false;
        }

        private void InitStatesWithChuckConfiguration()
        {
            var substrateSlotConfigs = Configuration.GetSubstrateSlotConfigs();
            if (substrateSlotConfigs != null)
            {
                foreach (var substSlot in substrateSlotConfigs)
                {
                    _chuckState.WaferPresences.Add(substSlot.Diameter, MaterialPresence.Unknown);
                    _chuckState.WaferClampStates.Add(substSlot.Diameter, false);
                }
            }
            else
            {
                throw new NotImplementedException("GetSubstrateSlotConfigs is not implemented for this kind of Chuck Configuration");
            }
        }

        public override void TriggerUpdateEvent()
        {
        }

        public void InitWaferStage()
        {
            Task.Delay(1000).Wait();
            if (new Random().NextDouble() > 0.9)
            {
                throw new Exception("Arbitrary Exception");
            }
        }
    }
}
