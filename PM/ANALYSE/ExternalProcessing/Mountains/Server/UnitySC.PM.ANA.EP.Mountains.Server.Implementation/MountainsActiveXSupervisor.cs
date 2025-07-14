using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;
using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Server.Implementation
{
    public class MountainsActiveXSupervisor : IMountainsActiveXService
    {

        private ServiceInvoker<IMountainsActiveXService> _mountainsActiveXService;

        public MountainsActiveXSupervisor()
        {
            _mountainsActiveXService = new ServiceInvoker<IMountainsActiveXService>("MountainsActiveXService", ClassLocator.Default.GetInstance<SerilogLogger<IMountainsActiveXService>>(), ClassLocator.Default.GetInstance<IMessenger>());
        }

        public Response<VoidResult> Close()
        {
            return _mountainsActiveXService.TryInvokeAndGetMessages(x => x.Close());
        }

        public Response<List<ExternalProcessingResultItem>> Execute(MountainsExecutionParameters mountainsExecutionParameters)
        {
            return _mountainsActiveXService.TryInvokeAndGetMessages(x => x.Execute(mountainsExecutionParameters));
        }

        public Response<ExternalMountainsTemplate> GeTemplateContent(string templateFile)
        {
            return _mountainsActiveXService.TryInvokeAndGetMessages(x => x.GeTemplateContent(templateFile));
        }

        public Response<List<ExternalProcessingResultItem>> GetResultsDefinedInTemplate(string templateFile)
        {
            return _mountainsActiveXService.TryInvokeAndGetMessages(x => x.GetResultsDefinedInTemplate(templateFile));
        }

        public Response<VoidResult> CheckActiveXExternal()
        {
            return _mountainsActiveXService.TryInvokeAndGetMessages(x => x.CheckActiveXExternal());
        }
    }
}
