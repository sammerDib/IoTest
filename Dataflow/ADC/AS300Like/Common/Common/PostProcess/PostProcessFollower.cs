using Common;
using Common2017.PostProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{ 
    public class PackEventPP
    {
        public EventPPStateChanged EvtOnPPStateChanged;
        public EventPPPJStateChanged EvtEventPPPJStateChanged;
        public EventPPUpdateDisplay EvtOnPPUpdateDisplay;
    }

    public delegate void EventPPStateChanged(enumConnection serverType, List<PostProcessDataSlotReceived> SlotList, PostProcessSatus status);
    public delegate void EventPPUpdateDisplay(String CJID, String PJID, enumConnection serverType, String Status, enumPPStatus ThreadStatus);
    public delegate void EventPPPJStateChanged(String CJID);

    public class PostProcessDataSlotReceived
    {
        public int Slot;
        public String SubstrateID;

        public PostProcessDataSlotReceived(int slot, string substrateID)
        {
            Slot = slot;
            SubstrateID = substrateID;
        }
    } 


    // A PostProcessFollower is associated with a Process Job (not a control job)
    public class PostProcessFollower
    {
        enumConnection m_ServerType;
        String m_ControlJobID;
        String m_ProcessJobID;
        List<PostProcessDataSlotReceived> m_ReceivedSlots = new List<PostProcessDataSlotReceived>();

        List<int> m_ExpectedSlotList = new List<int>();
        int m_ExpectedSlotNumber = 0;
        Thread m_Thread;
        bool m_bExitThread = false;
        AutoResetEvent m_EvtWaitingForStartResultsEventCollection = new AutoResetEvent(false);
        PackEventPP m_pPackEventPP;
        bool m_DCStarted = false;
        PostProcessSatus m_PostProcesSatus = new PostProcessSatus(enumPPStatus.ppsNotStarted, "");

        CLogManager m_pLogManager;
        String m_LasComment_ComparisonSlotsReceived = String.Empty;

        public PostProcessFollower(enumConnection serverType, String controlJobID, String processJobID, PackEventPP packEventPP, CLogManager logManager)
        {
            ServerType = serverType;
            m_ControlJobID = controlJobID;
            ProcessJobID= processJobID;
            m_pLogManager = logManager;

            m_pPackEventPP = packEventPP;
            m_EvtWaitingForStartResultsEventCollection.Reset();
            m_Thread = new Thread(new ParameterizedThreadStart(PostProcessFollower_Execute));
            m_Thread.Name = processJobID + "_PPFollower_Execute";
            m_Thread.Start(null);
        }

        public string ProcessJobID { get => m_ProcessJobID; set => m_ProcessJobID = value; }
        public string ControlJobID { get => m_ControlJobID; set => m_ControlJobID = value; }
        public enumConnection ServerType { get => m_ServerType; set => m_ServerType = value; }
        public PostProcessSatus PPStatus
        {
            get { return m_PostProcesSatus; }
            set
            {
                if(m_PostProcesSatus != value)
                {
                    m_PostProcesSatus = value;
                    m_pPackEventPP.EvtOnPPStateChanged(m_ServerType, m_ReceivedSlots, m_PostProcesSatus);
                }
            }
        }

        private void PostProcessFollower_Execute(object obj)
        {
            bool stopTimeout = false;
            bool stopCollectionComplete = false;
            int timeout = CConfiguration.WaitingPostProcessResultsTimeout_sec(m_ServerType);
            m_bExitThread = false;

            try
            {
                m_EvtWaitingForStartResultsEventCollection.WaitOne(-1, false);
                DateTime startTime = DateTime.Now;           
                m_LasComment_ComparisonSlotsReceived = String.Empty;
                PPStatus = new PostProcessSatus(enumPPStatus.ppsInProgress, "Post process started");
                // Infinity loop
                while (!m_bExitThread && !stopCollectionComplete && !stopTimeout)
                {
                    Thread.Sleep(1000);
                    stopTimeout = DateTime.Now.Subtract(startTime).TotalSeconds > timeout;
                    stopCollectionComplete = m_ReceivedSlots.Count >= m_ExpectedSlotNumber;
                    String PPFComment = "PPF " + ProcessJobID + " - " + ServerType.ToString() + " Slot received = " + m_ReceivedSlots.Count.ToString() + "/" + m_ExpectedSlotNumber.ToString();
                    if (m_LasComment_ComparisonSlotsReceived != PPFComment)
                    {
                        m_pPackEventPP.EvtOnPPUpdateDisplay(m_ControlJobID, m_ProcessJobID, m_ServerType, "Waiting next ADC results - " + SlotsStatus, PPStatus.Status);
                        PostProcessFollowLog(PPFComment);
                        m_LasComment_ComparisonSlotsReceived = PPFComment;
                        startTime = DateTime.Now; 
                    }
                }

                if (!m_bExitThread)
                {
                    if (stopTimeout)
                    {
                        String msg = "ADC results collection timeout - " + SlotsStatus;
                        PPStatus = new PostProcessSatus(enumPPStatus.ppsAborted, msg);
                        m_pPackEventPP.EvtOnPPStateChanged(m_ServerType, m_ReceivedSlots, PPStatus);
                        PostProcessFollowLog("PPF " + ProcessJobID + " - " + ServerType.ToString() + " - " + msg);
                    }
                    if (stopCollectionComplete)
                    {
                        if (m_ReceivedSlots.Count == m_ExpectedSlotNumber)
                        {
                            String msg = "ADC results collection complete - " + SlotsStatus; 
                            PPStatus = new PostProcessSatus(enumPPStatus.ppsComplete, msg);
                            m_pPackEventPP.EvtOnPPStateChanged( m_ServerType, m_ReceivedSlots, PPStatus);
                            PostProcessFollowLog("PPF " + ProcessJobID + " - " + ServerType.ToString() + " - " + msg);
                        }
                    }
                }
                if((PPStatus.Status == enumPPStatus.ppsInProgress) || (PPStatus.Status == enumPPStatus.ppsNotStarted))
                {
                    string msg = "ADC results collection failed - " + SlotsStatus;
                    PPStatus = new PostProcessSatus(enumPPStatus.ppsAborted, msg);
                    m_pPackEventPP.EvtOnPPStateChanged(m_ServerType, m_ReceivedSlots, PPStatus);
                    PostProcessFollowLog("PPF " + ProcessJobID + " - " + ServerType.ToString() + " - " + msg);
                }
            }
            catch(Exception ex)
            {
                String msg = "ADC results collection failed - " + SlotsStatus;
                PPStatus = new PostProcessSatus(enumPPStatus.ppsAborted, msg);
                m_pPackEventPP.EvtOnPPStateChanged(m_ServerType, m_ReceivedSlots, PPStatus);
                PostProcessFollowLog("PPF " + ProcessJobID + " - " + ServerType.ToString() + " - " + msg + " - Error=" + ex.Message + " - " + ex.StackTrace);
            }
        }

        public String SlotsStatus
        {
            get {
                    List<string> slotListReceived = m_ReceivedSlots.Select(rs => rs.Slot.ToString()).ToList();
                    //for (int i = 0; i < m_ReceivedSlots.Count; i++)
                    //{
                    //    slotListReceived.Add(m_ReceivedSlots[i].Slot);
                    //}
                    return "Slot received = " + String.Join("-", slotListReceived) + " [" + m_ReceivedSlots.Count.ToString() + " / " + m_ExpectedSlotNumber.ToString() + "]"; }
        }

        public void ADCWaferCompleted(String pSubstrateID, int pSlotID)
        {
            PostProcessFollowLog("PPF " + ProcessJobID + " - " + ServerType.ToString() + " - " + " ADCWaferCompleted - Slot " + pSlotID.ToString() + " received");
            PostProcessDataSlotReceived newSolt = new PostProcessDataSlotReceived(pSlotID, pSubstrateID);
            if (m_ExpectedSlotList.FindIndex(idx => idx == newSolt.Slot) >= 0)
            {
                m_ExpectedSlotList.Remove(newSolt.Slot);
                m_ReceivedSlots.Add(newSolt);
            }
        }

        public void StartCollection(List<int> pSlotList)
        {
            if (!m_DCStarted)
            {
                PostProcessFollowLog("PPF " + ProcessJobID + " collection STARTED");
                m_ExpectedSlotList = pSlotList.ToList<int>();
                m_ExpectedSlotNumber = m_ExpectedSlotList.Count;
                m_ReceivedSlots.Clear();
                m_DCStarted = true;
                m_EvtWaitingForStartResultsEventCollection.Set();
            }
        }

        public void AbortCollection()
        {
            PostProcessFollowLog("PPF " + ProcessJobID + ": AbortCollection called");
            m_bExitThread = true;
            m_EvtWaitingForStartResultsEventCollection.Set();            
        }

        private String m_LastComment = String.Empty;
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
    }
}
