using System;
using System.Collections.Generic;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Data;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.PM.DMT.Service.Interface.Proxy
{
    public class DapProxy : IDAP
    {
        private ServiceInvoker<IDAP> _dapService;

        public DapProxy()
        {            
            _dapService = new ServiceInvoker<IDAP>("DAP", ClassLocator.Default.GetInstance<SerilogLogger<IDAP>>(), null, ClassLocator.Default.GetInstance<ModuleConfiguration>().DataFlowAddress);
        }

        public Response<DAPData> GetData(Guid dapReadToken)
        {
            return _dapService.InvokeAndGetMessages(x => x.GetData(dapReadToken));
        }

        public Response<List<Guid>> GetReadToken(Guid dapWriteToken, int count)
        {
            return _dapService.InvokeAndGetMessages(x => x.GetReadToken(dapWriteToken, count));
        }

        public Response<Guid> GetWriteToken()
        {
            return _dapService.InvokeAndGetMessages(x => x.GetWriteToken());
        }

        public Response<bool> SendData(Guid dapWriteToken, DAPData data)
        {
            return _dapService.InvokeAndGetMessages(x => x.SendData(dapWriteToken, data));
        }
    }
}
