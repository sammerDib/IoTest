using UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;

namespace UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface
{
    public interface IProcessModuleInstruction<in T> where T : ProcessModule
    {
        void SetProcessModule(T processModule);

        InstructionType InstructionType { get; }

    }
}
