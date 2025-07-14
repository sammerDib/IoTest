using UnitySC.EFEM.Controller.HostInterface;
using UnitySC.EFEM.Controller.HostInterface.Enums;

namespace UnitySC.EquipmentController.Simulator.Driver
{
    public class ArmHistoryAndWaferPresenceEventArgs: System.EventArgs
    {
        public ArmHistoryAndWaferPresenceEventArgs(
            GetWaferPresenceOnArmLastAction lastAction,
            bool isPresentOnUpperArm,
            Constants.Stage stageUpperArm,
            uint slotUpperArm,
            bool isPresentOnLowerArm,
            Constants.Stage stageLowerArm,
            uint slotLowerArm)
        {
            LastAction          = lastAction;
            IsPresentOnUpperArm = isPresentOnUpperArm;
            StageUpperArm       = stageUpperArm;
            SlotUpperArm        = slotUpperArm;
            IsPresentOnLowerArm = isPresentOnLowerArm;
            StageLowerArm       = stageLowerArm;
            SlotLowerArm        = slotLowerArm;
        }

        public GetWaferPresenceOnArmLastAction LastAction { get; }
        public bool IsPresentOnUpperArm { get; }
        public Constants.Stage StageUpperArm { get; }
        public uint SlotUpperArm { get; }
        public bool IsPresentOnLowerArm { get; }
        public Constants.Stage StageLowerArm { get; }
        public uint SlotLowerArm { get; }
    }
}
