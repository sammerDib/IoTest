using System.Text;

namespace UnitySC.EFEM.Controller.HostInterface.Statuses
{
    public class E84InputsStatus
    {
        #region Constructors

        public E84InputsStatus(
            Constants.Port port,
            bool valid,
            bool cs_0,
            bool cs_1,
            bool tr_req,
            bool busy,
            bool compt,
            bool cont)
        {
            Port   = port;
            Valid  = valid;
            Cs_0   = cs_0;
            Cs_1   = cs_1;
            Tr_Req = tr_req;
            Busy   = busy;
            Compt  = compt;
            Cont   = cont;
        }

        #endregion Constructors

        #region Properties

        public Constants.Port Port { get; internal set; }

        public bool Valid { get; internal set; }

        public bool Cs_0 { get; internal set; }

        public bool Cs_1 { get; internal set; }

        public bool Tr_Req { get; internal set; }

        public bool Busy { get; internal set; }

        public bool Compt { get; internal set; }

        public bool Cont { get; internal set; }

        #endregion Properties

        public override string ToString()
        {
            var res = new StringBuilder(8);

            res.Append(((int)Port) + ",");
            res.Append((Valid ? "1" : "0") + ",");
            res.Append((Cs_0 ? "1" : "0") + ",");
            res.Append((Cs_1 ? "1" : "0") + ",");
            res.Append((Tr_Req ? "1" : "0") + ",");
            res.Append((Busy ? "1" : "0") + ",");
            res.Append((Compt ? "1" : "0") + ",");
            res.Append(Cont ? "1" : "0");

            return res.ToString();
        }
    }
}
