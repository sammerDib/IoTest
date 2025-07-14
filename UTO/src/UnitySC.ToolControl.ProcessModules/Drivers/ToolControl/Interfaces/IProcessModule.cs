using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true),
     Guid("65C3EEF7-0031-46BE-A993-72A885698560")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IProcessModule
    {
        #region Public Properties

        string Id { get; }
        bool IsSubstratePresent { get; }

        string Name { get; }
        ModuleState State { get; }

        #endregion Public Properties
    }

    [ComVisible(true),
     Guid(Constants.ProcessModuleInterfaceString),
     ClassInterface(ClassInterfaceType.None)]
    public class ProcessModule : IProcessModule
    {
        #region Public Constructors

        public ProcessModule(string id, string name, ModuleState state, bool isSubstratePresent)
        {
            Id = id;
            Name = name;
            State = state;
            IsSubstratePresent = isSubstratePresent;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Id { get; }
        public bool IsSubstratePresent { get; }
        public string Name { get; }
        public ModuleState State { get; set; }

        #endregion Public Properties
    }
}
