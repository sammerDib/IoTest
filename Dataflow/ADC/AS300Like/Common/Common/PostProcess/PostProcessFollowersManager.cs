using Common;
using Common2017.PostProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{


    public class PostProcessFollowersManagerForAPJID
    {
        String m_ControlJobID;
        String m_ProcessJobID;
        private List<PostProcessFollower> m_PostProcessFollowers = new List<PostProcessFollower>();
        CLogManager m_pLogManager;
        String m_LastComment = String.Empty;
        CProcessTypeUsed m_ProcessMode;
        PackEventPP m_pPackEventPP;
        PJData m_PJData;
        enumPPStatus m_PPFsManagerStatus = enumPPStatus.ppsNotStarted;

        public PostProcessFollowersManagerForAPJID(String controlJobID, PJData pjData,  PackEventPP packEventPP, CLogManager logManager)
        {
            m_ControlJobID = controlJobID;
            m_PJData = pjData;
            m_ProcessJobID = pjData.PJName;
            m_pLogManager = logManager;
            m_ProcessMode = pjData.ProcessUsed;
            m_pPackEventPP = packEventPP;


            PackEventPP newPack = new PackEventPP();
            newPack.EvtOnPPStateChanged = new EventPPStateChanged(PostProcessStateChanged);
            newPack.EvtOnPPUpdateDisplay = m_pPackEventPP.EvtOnPPUpdateDisplay;

            PostProcessFollowLog("=========================================================================================");
            PostProcessFollowLog("CJ=" + m_ControlJobID + "     PJ=" + m_ProcessJobID + " PM=" + m_ProcessMode.ProcessTypes.ToString());

            foreach (var serverType in pjData.ServerTypeUsedList)
            {
                PostProcessFollowLog("PPF added: " + m_ProcessJobID + " - " + serverType.ToString());
                PostProcessFollower newPPF = new PostProcessFollower(serverType, m_ControlJobID, m_ProcessJobID, newPack, m_pLogManager);
                PostProcessFollowers.Add(newPPF);
            }
        }

        //public String ControlJobID { get => m_ControlJobID; }

        public List<PostProcessFollower> PostProcessFollowers { get => m_PostProcessFollowers; }
        private Object SynchroStatus = new Object();
        public enumPPStatus Status 
        { 
            get => m_PPFsManagerStatus; 
            set
            {
                if (m_PPFsManagerStatus != value)
                {
                    m_PPFsManagerStatus = value;
                    m_pPackEventPP.EvtEventPPPJStateChanged(m_ControlJobID);
                }
            }
        }
        
        public PostProcessFollower this[enumConnection serverType]
        {
            get 
            {
                return PostProcessFollowers.Find(ppf => (ppf.ControlJobID == m_ControlJobID) && (ppf.ProcessJobID == m_ProcessJobID) && (ppf.ServerType == serverType));
            }
            
        }
        
        public void Dispose()
        {
            PostProcessFollowLog("PPFs associated with CJ=" + m_ControlJobID + " is cleared");
            foreach (var CurrPPF in PostProcessFollowers)
            {
                if(( CurrPPF.PPStatus.Status == enumPPStatus.ppsNotStarted) || (CurrPPF.PPStatus.Status == enumPPStatus.ppsInProgress))
                    CurrPPF.AbortCollection();
            }
            m_PostProcessFollowers.Clear();
            m_PostProcessFollowers = null;
            m_PJData.Dispose();
        }
        public void StartPostProcessDataCollection(String PJID, List<int> SlotList, bool allADCReady)
        {
            if (Status != enumPPStatus.ppsNotStarted) return;
            
            if ((PostProcessFollowers != null) && PostProcessFollowers.Count > 0)
            {
                foreach (var ppf in PostProcessFollowers)
                {
                    if (ppf.PPStatus.Status == enumPPStatus.ppsNotStarted)
                    {
                        if (allADCReady)
                        {
                            Status = enumPPStatus.ppsInProgress;
                            ppf.StartCollection(SlotList);
                        }
                        else
                        {
                            ppf.AbortCollection();
                        }
                    }
                }
            }
            else
            {
                PostProcessFollowLog("No PostProcessFollower found to start ADC data collection CJ= " + m_ControlJobID + " PJ=" + PJID);
                Status = enumPPStatus.ppsAborted;
            }            
        }       

        public void PostProcessFollowLog(String strLogMessage)
        {
            try
            {
                if (strLogMessage != m_LastComment)
                {
                    m_pLogManager.AddLogItem(CValues.POSTPROCESSFOLLOW_LOGS, CValues.POSTPROCESSFOLLOW_NAME, strLogMessage);
                    m_LastComment = strLogMessage;
                }
            }
            catch
            {

            }
        }

        public void OnADCWaferComplete(int serverType, string pJobID, int pLoadportSource, int pSlotID, string pLotID, string pWaferID, int pErrorStatus)
        {
            int pos = PostProcessFollowers.FindIndex(ppf => (ppf.ServerType == (enumConnection)serverType) && (ppf.ProcessJobID== pJobID));
            if (pos >= 0)
                PostProcessFollowers[pos].ADCWaferCompleted(pWaferID, pSlotID);
        }

        private void PostProcessStateChanged(enumConnection serverType, List<PostProcessDataSlotReceived> SlotList, PostProcessSatus ppStatus)
        {
            lock (SynchroStatus)
            {
                if ((Status != enumPPStatus.ppsAborted) && (Status != enumPPStatus.ppsComplete))
                {
                    bool bPostProcessFailed = false;
                    int nbPPFComplete = 0;
                    foreach (var ppf in m_PostProcessFollowers)
                    {
                        bPostProcessFailed = (ppf.PPStatus.Status == enumPPStatus.ppsAborted);

                        if (ppf.PPStatus.Status == enumPPStatus.ppsComplete)
                        {
                            nbPPFComplete++;
                        }
                    }
                    if (bPostProcessFailed)
                    {
                        foreach (var ppf in m_PostProcessFollowers)
                        {
                            if ((ppf.PPStatus.Status == enumPPStatus.ppsInProgress) || (ppf.PPStatus.Status == enumPPStatus.ppsNotStarted))
                                ppf.AbortCollection();
                        }
                        Status = enumPPStatus.ppsAborted;
                    }
                    else if (nbPPFComplete == m_PostProcessFollowers.Count)
                    {
                        Status = enumPPStatus.ppsComplete;
                    }
                }
            }
            m_pPackEventPP.EvtOnPPUpdateDisplay(m_ControlJobID, m_ProcessJobID, serverType, ppStatus.Message, ppStatus.Status);
        }
    }
}
