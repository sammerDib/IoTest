using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace UnitySC.PM.ANA.Service.Interface.ExternalProcessing
{
    [ServiceContract(CallbackContract = typeof(IExternalProcessingServiceCallback))]
    public interface IExternalProcessingService
    {
    }
}
