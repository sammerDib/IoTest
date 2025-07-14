using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.EP.Mountains.Interface;
using UnitySC.PM.ANA.EP.Mountains.Server.Implementation;
using UnitySC.Shared.Data.ExternalFile;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.EP.Mountains.Server.ActiveXHost
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class MountainsActiveXService : BaseService, IMountainsActiveXService
    {        

        public MountainsActiveXService(ILogger logger) : base(logger, ExceptionType.ExternalProcessingException)
        {
        }

        public Response<VoidResult> Close()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                CheckActiveX();
                MountainsForm.CurrentInstance.Close();
            });
        }

        public Response<List<ExternalProcessingResultItem>> Execute(MountainsExecutionParameters mountainsExecutionParameters)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                CheckActiveX();                
                return MountainsForm.CurrentInstance.Execute(mountainsExecutionParameters);
            });
        }

        public Response<ExternalMountainsTemplate> GeTemplateContent(string templateFile)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                CheckActiveX();
                var template = new ExternalMountainsTemplate();
                template.LoadFromFile(templateFile);
                return template;
            });
        }

        public Response<List<ExternalProcessingResultItem>> GetResultsDefinedInTemplate(string templateFile)
        {
            return InvokeDataResponse(messagesContainer =>
            {
                CheckActiveX();
                return MountainsForm.CurrentInstance.GetResultsDefinedInTemplate(templateFile);
            });
        }

        private void CheckActiveX()
        {
            if (MountainsForm.CurrentInstance == null)
                throw new Exception("Mountains ActiveX Init error: Check license");
        }

        public Response<VoidResult> CheckActiveXExternal()
        {
            return InvokeVoidResponse(messagesContainer =>
            {
                CheckActiveX();
            });
        }
    }
}
