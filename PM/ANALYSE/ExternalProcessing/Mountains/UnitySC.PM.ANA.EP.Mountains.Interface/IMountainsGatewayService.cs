using System.Collections.Generic;
using System.ServiceModel;

using UnitySC.Shared.Image;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Interface
{
    [ServiceContract]
    public interface IMountainsGatewayService
    {
        [OperationContract]
        Response<List<ExternalProcessingResultItem>> Execute(MountainsExecutionParameters mountainsExecutionParameters, ExternalMountainsTemplate template = null, ServiceImage serviceImage = null);


        [OperationContract]
        Response<List<ExternalProcessingResultItem>> GetResultsDefinedInTemplate(string templateFile);

        [OperationContract]
        Response<ExternalMountainsTemplate> GetTemplateContent(string templateFile);

        [OperationContract]
        Response<List<string>> GetTemplateFilePaths();

    }
}
