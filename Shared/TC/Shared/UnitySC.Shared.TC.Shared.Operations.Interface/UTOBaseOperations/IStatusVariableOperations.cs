using System;
using System.Collections.Generic;

using UnitySC.Shared.Data.Enum;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface IStatusVariableOperations
    {
        bool IsReadyToExchangeWithTC { get; }

        void Init(string configurationFilePath);

        List<StatusVariable> SVGetAllRequest();

        List<StatusVariable> SVGetRequest(List<int> ids);

        void SVSetRequest(List<StatusVariable> svid);

    }
}
