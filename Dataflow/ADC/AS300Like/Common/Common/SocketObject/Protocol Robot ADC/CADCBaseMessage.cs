using Common.FDC;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Protocol_Robot_ADC
{
    [Serializable]
    public class CADCBaseMessage
    {
        public CErrorParameters Error;
        public enumCommandExchangeADC Command;
        public String Description;
        public enumStatusExchangeADC Status;
        #if !WITHOUT_FDC
        public FDCInfo FdcInfoData { get; set; }
        #endif
        //------------------
        public virtual String MessageLog
        {
            get
            {
                String strError = " | ERR= Database connected - ";
                if (Error != null)
                {
                    if (!Error.DataBaseIsConnected)
                        strError = " | ERR= Database not connected - ";
                    if ((strError == " | ERR= Database not connected - ") && (Error.iErrorNumber == enError.en_NoError))
                        strError = " | ERR= Database not connected";
                    else
                        strError += Error.iErrorNumber.ToString().Remove(0, 3);
                    if (Error.DataBaseIsConnected && (Error.iErrorNumber == enError.en_NoError))
                        strError = "";
                }else
                    strError = "";
                String lstrComment = ((Description.Length>0)? " | CMT=" + Description: "");
                String lstrCommand = "CD=" + Command.ToString().Remove(0, 2);
                String lstrStatus = " | S=" + Status.ToString().Remove(0, 2);
                switch (Command)
                {
                    case enumCommandExchangeADC.caGetStatus :
                        strError = "";
                        goto default;
                    default:
                        return lstrCommand + lstrStatus + strError + lstrComment;
                }
            }
        }
    }
}
