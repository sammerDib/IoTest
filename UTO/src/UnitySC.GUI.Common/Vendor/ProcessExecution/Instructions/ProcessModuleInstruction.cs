using UnitySC.Equipment.Abstractions.Vendor.Devices.ProcessModule;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Enums;
using UnitySC.Equipment.Abstractions.Vendor.ProcessExecution.Interface;

namespace UnitySC.GUI.Common.Vendor.ProcessExecution.Instructions
{
    public abstract class ProcessModuleInstruction<T> : ProcessInstruction, IProcessModuleInstruction<T>
        where T : ProcessModule
    {
        #region Fields

        private InstructionType _instructionActivity = InstructionType.NotDefined;

        #endregion Fields

        #region Protected Fields

        protected T ProcessModule { get; private set; }

        #endregion

        #region IProcessModuleInstruction

        public InstructionType InstructionType
        {
            get => _instructionActivity;
            set
            {
                if (_instructionActivity == value)
                {
                    return;
                }

                _instructionActivity = value;
                OnPropertyChanged(nameof(InstructionType));
            }
        }

        public void SetProcessModule(T processModule) => ProcessModule = processModule;

        #endregion
    }
}
