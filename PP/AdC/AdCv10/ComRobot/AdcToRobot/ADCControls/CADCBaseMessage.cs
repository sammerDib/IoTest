using System;

using ADCControls;

namespace CADCServiceObject.SocketClient
{
    [Serializable]
    public class CADCBaseMessage
    {
        public CErrorParameters Error;
        public enumCommandExchangeADC Command;
        public String Description;
        public enumStatusExchangeADC Status;
        //------------------
        public virtual String MessageLog
        {
            get
            {
                switch (Command)
                {
                    default:
                        return "CD=" + Command.ToString().Remove(0, 2) + " | S=" + Status.ToString().Remove(0, 2) + " | CMT=" + Description;
                }
            }
        }
    }
}
