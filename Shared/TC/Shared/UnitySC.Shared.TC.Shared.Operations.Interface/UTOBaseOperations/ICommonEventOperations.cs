using System.Collections.Generic;

using UnitySC.Shared.Data;
using UnitySC.Shared.Data.DVID;
using UnitySC.Shared.Data.SecsGem;
using UnitySC.Shared.TC.Shared.Data;

namespace UnitySC.Shared.TC.Shared.Operations.Interface
{
    public interface ICommonEventOperations
    {
        void Init(string configurationFilePath);

        List<CommonEvent> CEGetAll();

        void Fire_CE(List<CEName> ceNameList);

        void Fire_CE(CEName ceName, SecsVariableList dvids);

       
    }
}
