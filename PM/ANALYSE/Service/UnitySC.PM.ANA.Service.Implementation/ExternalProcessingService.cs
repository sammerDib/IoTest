using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

using UnitySC.PM.ANA.Service.Interface.ExternalProcessing;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.ANA.Service.Implementation
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ExternalProcessingService : DuplexServiceBase<IExternalProcessingServiceCallback>, IExternalProcessingService
    {
        public ExternalProcessingService(ILogger logger) : base(logger, ExceptionType.ExternalProcessingException)
        {

        }
    }
}
