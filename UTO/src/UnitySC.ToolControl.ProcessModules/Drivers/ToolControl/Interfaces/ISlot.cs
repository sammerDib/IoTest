using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true),
     Guid("CC927D38-059F-458B-BA6F-86EBD91ABA6E")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface ISlot
    {
        string Id { get; }
        string Name { get; }
        Substrate Substrate { get; }
    }

    [ComVisible(true),
     ClassInterface(ClassInterfaceType.None),
     ComDefaultInterface(typeof(ISlot)),
     Guid(Constants.SlotInterfaceString)]
    public class Slot : ISlot
    {
        #region Properties

        public string Id { get; }
        public string Name { get; }
        public Substrate Substrate { get; }

        #endregion

        #region Constructor

        public Slot(string id, string name, Substrate substrate)
        {
            Id = id;
            Name = name;
            Substrate = substrate;
        }

        #endregion
    }
}
