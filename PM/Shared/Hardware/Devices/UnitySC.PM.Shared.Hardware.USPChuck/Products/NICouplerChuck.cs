using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnitySC.PM.Shared.Hardware.Controllers;
using UnitySC.PM.Shared.Hardware.Service.Interface;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.IOComponent;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.PM.Shared.Status.Service.Interface;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.Shared.Hardware.USPChuck
{
    /// <summary>
    /// Handles the communication, the polling and the output activations of a RackNI I/O device
    ///
    /// </summary>
    public class NICouplerChuck : USPChuckBase, IChuckClamp
    {
        private readonly NICouplerController _niController;
        private readonly NICouplerChuckConfig _niCouplerChuckConfig;
        private bool _waferIsClamped = false;
        private Valve _valve;
        private string _message;

        private Length _signleSizeSupported = new Length(300, LengthUnit.Millimeter);
        #region Constructor

        public NICouplerChuck(NICouplerChuckConfig config, NICouplerController niController, IGlobalStatusServer globalStatusServer, ILogger logger)
            : base(globalStatusServer, logger)
        {
            _niController = niController;
            _niCouplerChuckConfig = config;
        }

        #endregion Constructor

        public override bool IsMaterialPresenceRefreshed { get => true; }

        public void ClampWafer(Length size)
        {
            if (!_niCouplerChuckConfig.IsVacuumChuck)
            {
                _message = $"[{_niCouplerChuckConfig.Name}] Vacuum is not active in the chuck configuration.";
                Logger.Information(_message);
            }

            _valve = FindValve(size);
            if (_valve == null)
            {
                _message = $"No clamp available in the configuration for wafer size : {size.Millimeters}.";
                Logger.Error(_message);
                throw new Exception(_message);
            }
            UpdateVacuumState();
            WaitVacuumStabilisation();
        }

        private bool WaferIsClamped()
        {
            var vacuum1 = (DigitalInput)_niController.GetInput(_niCouplerChuckConfig.Vacuum1); // TODO: CRI = Pas compris comment faire correspondre (Vacuum1, Vacuum2) et Config des valves
            var vacuum2 = (DigitalInput)_niController.GetInput(_niCouplerChuckConfig.Vacuum2);

            bool vacuum1State = _niController.DigitalRead(vacuum1);
            bool vacuum2State = _niController.DigitalRead(vacuum2);

            return (_valve.Vacuum1 == vacuum1State) && (_valve.Vacuum2 == vacuum2State);
        }

        private Valve FindValve(Length size)
        {
            foreach (var clamp in _niCouplerChuckConfig.WaferClampList)
            {
                if (!clamp.Available)
                {
                    continue;
                }
                // TODO: When Frame will be supported with USP                
                //if (clamp.IsFilmFrame && wafer.IsFilmFrame || clamp.WaferSize == size)
                if (clamp.WaferSize == size)
                    return clamp.Valve;
            }
            return null;
        }

        private void UpdateVacuumState()
        {
            var vacuumValve1 = (DigitalOutput)_niController.GetOutput(_niCouplerChuckConfig.VacuumValve1);
            var vacuumValve2 = (DigitalOutput)_niController.GetOutput(_niCouplerChuckConfig.VacuumValve2);

            _niController.DigitalWrite(vacuumValve1, _valve.VacuumValve1);
            _niController.DigitalWrite(vacuumValve2, _valve.VacuumValve2);
        }

        private void WaitVacuumStabilisation()
        {
            //Waiting for the vacuum to be created
            _waferIsClamped = SpinWait.SpinUntil(() => WaferIsClamped()
            , _niCouplerChuckConfig.StabilisationTime_ms);

            if (!_waferIsClamped)
            {
                _message = "Wafer is not properly fixed, vacuum sensor is still OFF";
                Logger.Error(_message);
                throw new Exception(_message);
            }
            Logger.Information("The wafer is successfully clamped");
        }


        public override ChuckState GetState()
        {
            return CreateChuckState(_waferIsClamped, _waferIsClamped ? MaterialPresence.Present : MaterialPresence.NotPresent);
        }

        public ChuckState CreateChuckState(bool clamped, MaterialPresence presence)
        {
            Dictionary<Length, bool> clampStates = new Dictionary<Length, bool>();
            clampStates.Add(_signleSizeSupported, clamped);
            Dictionary<Length, MaterialPresence> presenceStates = new Dictionary<Length, MaterialPresence>();
            presenceStates.Add(_signleSizeSupported, presence);
            return new ChuckState(clampStates, presenceStates);
        }
        public override void Init()
        {
            if (!_niCouplerChuckConfig.IsVacuumChuck)
            {
                _message = $"NICouplerChuckConfig for chuck {_niCouplerChuckConfig.Name} does not contain active vacuum";
                Logger.Information(_message);
            }
        }


        private bool WaferIsReleased()
        {
            var vacuum1 = (DigitalInput)_niController.GetInput(_niCouplerChuckConfig.Vacuum1);
            var vacuum2 = (DigitalInput)_niController.GetInput(_niCouplerChuckConfig.Vacuum2);

            bool vacuum1IsReleased = _niController.DigitalRead(vacuum1);
            bool vacuum2IsReleased = _niController.DigitalRead(vacuum2);

            return vacuum1IsReleased && vacuum2IsReleased;
        }

        public void ReleaseWafer(Length size)
        {
            ChuckState state = GetState();
            if (!state.WaferClampStates.Any(k => k.Value == true))
            {
                Logger.Error($"No wafer clamped in the chuck {_niCouplerChuckConfig.Name}");
            }

            var vacuumValve1 = (DigitalOutput)_niController.GetOutput(_niCouplerChuckConfig.VacuumValve1);
            var vacuumValve2 = (DigitalOutput)_niController.GetOutput(_niCouplerChuckConfig.VacuumValve2);

            _niController.DigitalWrite(vacuumValve1, false);
            _niController.DigitalWrite(vacuumValve2, false);

            bool waferIsReleased = SpinWait.SpinUntil(() => WaferIsReleased()
            , _niCouplerChuckConfig.StabilisationTime_ms);

            if (!waferIsReleased)
            {
                _message = "Wafer is not released";
                Logger.Error(_message);
                throw new Exception(_message);
            }
            _waferIsClamped = false;

            Logger.Information("The wafer is successfully released");
        }

        public override List<Length> GetMaterialDiametersSupported()
        {
            var sizes = new List<Length>();
            foreach (var substSlot in _niCouplerChuckConfig.SubstrateSlotConfigs)
            {
                sizes.Add(substSlot.Diameter);
            }
            return sizes; 
        }

        public override bool IsSensorPresenceEnable(Length size)
        {
            return false;
        }

        public override void TriggerUpdateEvent()
        {
            
        }
    }
}
