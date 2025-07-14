using System;

using UnitySC.PM.ANA.Hardware;
using UnitySC.PM.ANA.Service.Core.Shared;
using UnitySC.PM.ANA.Service.Interface.Algo;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.PM.Shared.Hardware.Service.Interface.Chuck;
using UnitySC.PM.Shared.Hardware.Service.Interface.USPChuck;
using UnitySC.Shared.Data;
using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Units;

namespace UnitySC.PM.ANA.Service.Core.AdvancedFlow
{
    public class CheckWaferPresenceFlow : FlowComponent<CheckWaferPresenceInput, CheckWaferPresenceResult, AxesMovementConfiguration>
    {
        private AnaHardwareManager _hardwareManager;

        public CheckWaferPresenceFlow(CheckWaferPresenceInput input) : base(input, "CheckWaferPresenceFlow")
        {
            _hardwareManager = ClassLocator.Default.GetInstance<AnaHardwareManager>();
        }

        protected override void Process()
        {
            var initialPosition = HardwareUtils.GetAxesPosition(_hardwareManager.Axes);
            var slotConfig = _hardwareManager?.Chuck?.Configuration.GetSubstrateSlotConfigByWafer(Input.MaterialDiameter);
            if (slotConfig?.PositionPark == null)
            {
                throw new Exception("PositionPark is not defined for the given wafer diameter.");
            }
            _hardwareManager.Axes.GotoPosition(slotConfig.PositionPark, Configuration.Speed);            
            _hardwareManager.Axes.WaitMotionEnd(20000);
            var isWaferPresent = false; 
            if(_hardwareManager.Chuck is IChuckMaterialPresence materialpresenceHandler)             
                isWaferPresent = materialpresenceHandler.CheckWaferPresence(Input.MaterialDiameter) == MaterialPresence.Present;            
            else
                throw new Exception("IMaterialPresenceHandler not implemented on this Chuck");
            Result.IsWaferPresent = isWaferPresent;
            HardwareUtils.MoveAxesTo(_hardwareManager.Axes, initialPosition, Configuration.Speed);
        }        
    }
}
