using System;
using System.Collections.Generic;
using System.Text;

namespace UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC
{
    public enum enumCommand {acGetVersion = 0, acGetStatus = 1};
    public enum enumWaferStatus {wsNotStarted, wsProcessing, wsProcessed, wsError};

    [Serializable]
    public class CADCMessage : CADCBaseMessage
    {
        public List<WaferStatus> WaferStatusList;

        public CADCMessage()
        {
            WaferStatusList = new List<WaferStatus>();
        }
    }
    [Serializable]
    public class WaferStatus
    {
        public int UniqueWaferID;
        public List<CEdgeDefects> Defects;
        public enumWaferStatus state;
        public int Slot;
        public int LoadportID;
        public String LotID;

        public WaferStatus()
        {
            Defects = new List<CEdgeDefects>(); // Up, UpSide, Side, SideDown, Down, UpSideDown
        }
    }


    public class CEdgeDefects
    {
        public int DefectCount;
        public int DefectVID;
        public String Label;
    }
}
