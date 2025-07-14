using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Implementation
{
    [ServiceContract]
    public interface IMountainsActiveXService
    {
        [OperationContract]
        Response<List<ExternalProcessingResultItem>> Execute(MountainsExecutionParameters mountainsExecutionParameters);

        [OperationContract]
        Response<List<ExternalProcessingResultItem>> GetResultsDefinedInTemplate(string templateFile);

        [OperationContract]
        Response<ExternalMountainsTemplate> GeTemplateContent(string templateFile);

        [OperationContract]
        Response<VoidResult> CheckActiveXExternal();

        Response<VoidResult> Close();
    }
}
