using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    public class PJData : ICloneable, IDisposable
    {
        public String PJName;
        public String PJStatus;
        public List<int> SlotSelected = new List<int>();
        public CProcessTypeUsed ProcessUsed=null;
        public bool AllADCReady;

        public PJData(string pJName, string slotSelected, CProcessTypeUsed processUsed, bool allADCReady)
        {
            PJName = pJName;
            PJStatus = "Unknown";
            if (slotSelected.Length > 0)
            {
                String[] sTab = slotSelected.Split(';');
                foreach (var slot in sTab)
                {
                    SlotSelected.Add(Convert.ToInt32(slot));
                }
            }
            ProcessUsed = processUsed;
            AllADCReady = allADCReady;
        }

        public List<enumConnection> ServerTypeUsedList
        {
            get
            {
                return GetAllServerTypeFromProcessMode(ProcessUsed);
            }
        }

        private List<enumConnection> GetAllServerTypeFromProcessMode(CProcessTypeUsed processMode)
        {
            List<enumConnection> serverTypeList = new List<enumConnection>();
            if (processMode.bPSDFrontside)
                serverTypeList.Add(enumConnection.CONNECT_ADC_FRONT);
            if (processMode.bPSDBackside)
                serverTypeList.Add(enumConnection.CONNECT_ADC_BACK);
            if (processMode.bBrightField1)
                serverTypeList.Add(enumConnection.CONNECT_ADC_BF1);
            if (processMode.bBrightField2)
                serverTypeList.Add(enumConnection.CONNECT_ADC_BF2);
            if (processMode.bBrightField3)
                serverTypeList.Add(enumConnection.CONNECT_ADC_BF3);
            if (processMode.bBrightField4)
                serverTypeList.Add(enumConnection.CONNECT_ADC_BF4);
            if (processMode.bDarkview)
                serverTypeList.Add(enumConnection.CONNECT_ADC_DF);
            if (processMode.bPMEdge)
                serverTypeList.Add(enumConnection.CONNECT_ADC_EDGE);
            if (processMode.bPMLS)
                serverTypeList.Add(enumConnection.CONNECT_ADC_LS);
            return serverTypeList;
        }

        public override string ToString()
        {
            return PJName + " - " + String.Join(",", SlotSelected) + " - " + ProcessUsed.ProcessTypes.ToString() + " - AllADCReady=" + AllADCReady.ToString();
        }

        public object Clone()
        {
            PJData clonePJData = (PJData)this.MemberwiseClone();
            //clonePJData.SlotSelected = new List<int>(this.SlotSelected);
            //clonePJData.ProcessUsed = new CProcessTypeUsed(this.ProcessUsed.ProcessTypes);

            return clonePJData;
        }

        public void Dispose()
        {
        }
    }

    public class CJData : ICloneable
    {
        public String CJName;
        public String CJStatus;
        public String CarrierID;
        public List<PJData> PJDataList = new List<PJData>();
        public PostProcessFollowersManagerForAPJID PostProcessFollowersManager_PJActive;

        public CJData(string cJName)
        {
            CJName = cJName;
        }

        public object Clone()
        {
            CJData cloneCJData = (CJData)this.MemberwiseClone();            
            return cloneCJData;
        }

        public void Dispose()
        {
            if (PostProcessFollowersManager_PJActive != null) PostProcessFollowersManager_PJActive.Dispose();
            PJDataList.ForEach(pj => pj.Dispose());
            PJDataList.Clear();
        }


    }
}
