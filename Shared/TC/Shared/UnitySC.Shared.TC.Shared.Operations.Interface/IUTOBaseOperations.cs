using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IUTOBaseOperations
    {
        IAlarmOperations AlarmOperations { get; }
        IEquipmentConstantOperations ECOperations { get; }
        ICommonEventOperations CEOperations { get; }
        ICommunicationOperations CommunicationOperations { get; set; }
    }
}
