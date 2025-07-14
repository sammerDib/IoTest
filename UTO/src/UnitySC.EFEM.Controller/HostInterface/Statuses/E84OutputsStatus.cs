using System.Text;

namespace UnitySC.EFEM.Controller.HostInterface.Statuses
{
    public class E84OutputsStatus
    {
        #region Constructors

        public E84OutputsStatus(
            Constants.Port port,
            bool lReq,
            bool u_req,
            bool ready,
            bool ho_avbl,
            bool es)
        {
            Port    = port;
            L_Req   = lReq;
            U_Req   = u_req;
            Ready   = ready;
            Ho_Avbl = ho_avbl;
            Es      = es;
        }

        #endregion Constructors

        #region Properties

        public Constants.Port Port { get; internal set; }

        public bool L_Req { get; internal set; }

        public bool U_Req { get; internal set; }

        public bool Ready { get; internal set; }

        public bool Ho_Avbl { get; internal set; }

        public bool Es { get; internal set; }

        #endregion Properties

        public override string ToString()
        {
            var res = new StringBuilder(6);

            res.Append(((int)Port) + ",");
            res.Append((L_Req ? "1" : "0") + ",");
            res.Append((U_Req ? "1" : "0") + ",");
            res.Append((Ready ? "1" : "0") + ",");
            res.Append((Ho_Avbl ? "1" : "0") + ",");
            res.Append(Es ? "1" : "0");

            return res.ToString();
        }
    }
}
