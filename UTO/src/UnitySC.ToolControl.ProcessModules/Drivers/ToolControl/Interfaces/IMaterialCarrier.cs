using System.Runtime.InteropServices;

namespace UnitySC.ToolControl.ProcessModules.Drivers.ToolControl.Interfaces
{
    [ComVisible(true),
     Guid("F229E23D-3CD6-4663-8355-F319E35CB2B9")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IMaterialCarrier
    {
        string Id { get; }
        string Name { get; }
        SlotCollection Slots { get; }
    }

    [ComVisible(true),
     ClassInterface(ClassInterfaceType.None),
     ComDefaultInterface(typeof(IMaterialCarrier)),
     Guid(Constants.MaterialCarrierInterfaceString)]
    public class MaterialCarrier : IMaterialCarrier
    {
        #region Properties

        public string Id { get; }
        public string Name { get; }
        public SlotCollection Slots { get; }

        #endregion

        #region Constructor

        public MaterialCarrier(string id, string name, Slot[] slots)
        {
            Id = id;
            Name = name;
            Slots = new SlotCollection(slots);
        }

        #endregion
    }
}
