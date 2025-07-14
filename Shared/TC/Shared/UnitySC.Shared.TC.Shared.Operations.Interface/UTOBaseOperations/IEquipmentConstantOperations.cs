using System.Collections.Generic;

using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IEquipmentConstantOperations
    {
        void Init(string configurationFilePath);

        List<EquipmentConstant> ECGetAllRequest();

        List<EquipmentConstant> ECGetRequest(List<int> id);

        bool ECSetRequest(EquipmentConstant ecid);

        void SetECValues(List<EquipmentConstant> equipmentConstants);
    }
}
