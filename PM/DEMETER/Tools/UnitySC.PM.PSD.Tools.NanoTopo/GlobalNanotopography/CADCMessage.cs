using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
using System.Drawing.Imaging;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public enum enumCommand {acGetVersion = 0, acGetStatus = 1};
    public enum enumWaferStatus {wsNotStarted, wsProcessing, wsProcessed, wsError};

    [Serializable]
    public class CADCMessage
    {
        public List<WaferStatus> WaferStatusList;
        public enumCommand acCommand;
        public String Description;

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
