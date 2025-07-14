using Agileo.GUI.Components.Navigations;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public interface IUserInterfaceInstruction
    {
        BusinessPanel TargetedBusinessPanel { get; set; }
    }
}
