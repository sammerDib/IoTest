using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;

using UnitySC.Dataflow.Service.Interface;
using UnitySC.Shared.Dataflow.Shared;
using UnitySC.Shared.Logger;
using UnitySC.Shared.Tools.Service;

namespace UnitySC.Dataflow.Service.Implementation
{

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]

    public class DAPService :BaseService,  IDAP 
    {

        private readonly SortedDictionary<Guid, DAPWriteToken> _dataByWriteToken = new SortedDictionary<Guid, DAPWriteToken>();

        private readonly SortedDictionary<Guid, DAPReadToken> _dataByReadToken = new SortedDictionary<Guid, DAPReadToken>();

        public DAPService(ILogger logger) : base(logger, ExceptionType.DataflowException)
        {

        }

        Response<Guid> IDAP.GetWriteToken()
        {
            Guid writeToken = Guid.NewGuid();

            _dataByWriteToken.Add(writeToken, new DAPWriteToken() { WriteToken = writeToken } );

            return new Response<Guid>() { Result = writeToken };
        }

        Response<bool> IDAP.SendData(Guid dapWriteToken, DAPData data)
        {
            if( _dataByWriteToken.TryGetValue(dapWriteToken, out DAPWriteToken wt))
            {
                wt.Data = data;
                return new Response<bool>() { Result = true };

            }

            return new Response<bool>() { Result = false };
        }

        Response<List<Guid>> IDAP.GetReadToken(Guid dapWriteToken, int count)
        {

            if (_dataByWriteToken.TryGetValue(dapWriteToken, out DAPWriteToken wt))
            {

                List<DAPReadToken> lrt = new int[count].Select(i => new DAPReadToken() { ReadToken = Guid.NewGuid(), DAPWriteToken = wt }).ToList();

                lrt.ForEach(rt =>
                {
                    _dataByReadToken.Add(rt.ReadToken, rt);
                    wt.DAPReadTokens.Add(rt);

                });

                return new Response<List<Guid>>() { Result = lrt.Select(rt => rt.ReadToken).ToList() };

            }

            return new Response<List<Guid>>() { Result = new List<Guid>() };


        }

        Response<DAPData> IDAP.GetData(Guid dapReadToken)
        {
            if (_dataByReadToken.TryGetValue(dapReadToken, out DAPReadToken rt))
            {

                rt.IsRead = true;


                if (rt.DAPWriteToken.DAPReadTokens.Any(r => r.IsRead))
                {
                    // tous les ReadToken on etait utilisé.

                    _dataByReadToken.Remove(rt.ReadToken);
                    _dataByWriteToken.Remove(rt.DAPWriteToken.WriteToken);


                    //[DOTO] notifier la suppression des données 
                    // rt.DAPWriteToken.WriteToken.Data

                }

                return new Response<DAPData>() { Result = rt.DAPWriteToken.Data };

            }

            return new Response<DAPData>() { Result = null };

        }


    }




    public class DAPWriteToken
    {

        public Guid WriteToken { get; set; }

        public DAPData Data { get; set; }

        public List<DAPReadToken> DAPReadTokens { get; set; } = new List<DAPReadToken>();
    }

    public class DAPReadToken
    {
        public Guid ReadToken { get; set; }

        public bool IsRead { get; set; } = false;

        public DAPWriteToken DAPWriteToken { get; set; }

    }
}
