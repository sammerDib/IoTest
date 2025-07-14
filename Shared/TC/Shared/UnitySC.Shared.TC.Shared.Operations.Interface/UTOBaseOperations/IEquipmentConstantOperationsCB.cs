using System.Collections.Generic;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IEquipmentConstantOperationsCB
    {
        void ECStateChanged(Dictionary<string, EquipmentConstant> ecStatesDico);
    }
}
