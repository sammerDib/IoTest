using System.Collections.Generic;

using CommunityToolkit.Mvvm.Messaging;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.Shared.Image;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Proxy
{
    public class MountainsSupervisor : IMountainsGatewayService
    {
        private ServiceInvoker<IMountainsGatewayService> _mountainsGatewayService;
             

        public MountainsSupervisor(ServiceAddress customAddress)
        {
            _mountainsGatewayService = new ServiceInvoker<IMountainsGatewayService>("MountainsGatewayService", ClassLocator.Default.GetInstance<SerilogLogger<IMountainsGatewayService>>(), ClassLocator.Default.GetInstance<IMessenger>(), customAddress);
        }     

        public Response<List<ExternalProcessingResultItem>> Execute(MountainsExecutionParameters mountainsExecutionParameters, ExternalMountainsTemplate template = null, ServiceImage serviceImage = null)
        {
           return _mountainsGatewayService.TryInvokeAndGetMessages(x => x.Execute(mountainsExecutionParameters, template, serviceImage));
        }

        public Response<ExternalMountainsTemplate> GetTemplateContent(string templateFile)
        {
            return _mountainsGatewayService.TryInvokeAndGetMessages(x => x.GetTemplateContent(templateFile));
        }

        public Response<List<ExternalProcessingResultItem>> GetResultsDefinedInTemplate(string templateFile)
        {
            return _mountainsGatewayService.TryInvokeAndGetMessages(x => x.GetResultsDefinedInTemplate(templateFile));
        }
        public Response<List<string>> GetTemplateFilePaths()
        {
            return _mountainsGatewayService.TryInvokeAndGetMessages(x => x.GetTemplateFilePaths());
        }
    }
}
