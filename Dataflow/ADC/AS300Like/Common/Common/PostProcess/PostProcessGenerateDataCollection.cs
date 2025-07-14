using Common;
using Common.EventsPackages;
using Common2017.CVIDObj;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VALUELib;

namespace Common2017.PostProcess
{
    public class PostProcessGenerateDataCollection
    {
        Thread m_Thread;
        bool m_bExitThread = false;
        AutoResetEvent m_EvtWaitingForStartResultsEventCollection = new AutoResetEvent(false);        
        CLogManager m_pLogManager;
        String m_LogResult; 
        private List<CVID> m_VIDObjectList = new List<CVID>();
        PackEventClientApplication m_pPackEventClientApplication;
        PackEventPP m_pPackEventPP;
        String m_LastComment = String.Empty;
        List<CJData> m_CJDataList = new List<CJData>();
        AutoResetEvent m_EvtCJdataAdded = new AutoResetEvent(false);
        Object m_SynchroLockCJDataList = new Object();

        public PostProcessGenerateDataCollection(PackEventClientApplication packEventClientApplication, PackEventPP packEventPP, CLogManager logManager)
        {
            m_pPackEventClientApplication = packEventClientApplication;
            m_pPackEventPP = packEventPP;
            m_pLogManager = logManager;

            m_Thread = new Thread(new ParameterizedThreadStart(PostProcessGenerateDataCollection_Execute));
            m_Thread.Name = "PPGenerateData_Execute";
            m_Thread.Start(null);
        }

        public void AddCJData(CJData cjData)
        {
            lock(m_SynchroLockCJDataList)
            {
                m_CJDataList.Add((CJData)cjData.Clone());
                m_EvtCJdataAdded.Set();
            }
        }

        private void PostProcessGenerateDataCollection_Execute(object obj)
        {
            m_bExitThread = false;
            while (!m_bExitThread)
            {
                int nbGeneration = 0;
                m_EvtCJdataAdded.WaitOne(-1);

                if (m_bExitThread)
                    break;

                lock (m_SynchroLockCJDataList)
                {
                    m_EvtCJdataAdded.Reset();
                    nbGeneration = m_CJDataList.Count;
                }
                for (int i = 0; i < nbGeneration; i++)
                {
                    CJData currCJData = m_CJDataList[0];
                    try
                    {
                        m_LogResult = String.Empty;
                        try
                        {
                            // Value object
                            CxValueObjectClass lNewVIDData = new CxValueObjectClass();
                            int lVID = CConfiguration.PostProcess_GetDVID;
                            m_LogResult = "============================================================================\r\n";
                            m_LogResult = m_LogResult + "VID= " + lVID.ToString();
                            SetPostProcessDataListInVID(currCJData, ref lNewVIDData);
                            // Create VID
                            CVID newVID = new CVID(lVID, lNewVIDData, "PostProcessData List");
                            m_VIDObjectList.Add(newVID);

                            foreach (var vid in m_VIDObjectList)
                            {
                                m_pPackEventClientApplication.EvtSetDataVariable(vid.VID_ID.ToString(), vid.VID_DataList);
                            }

                            if (currCJData.PostProcessFollowersManager_PJActive.Status == enumPPStatus.ppsComplete)
                            {
                                m_LogResult = m_LogResult + "Trigger EVENT  EventAllPostProcess_Complete\r\n";
                                m_pPackEventClientApplication.EvtTriggerEvent(AllEventID.eEventAllPostProcess_Complete, "", "");
                            }
                            else
                            if (currCJData.PostProcessFollowersManager_PJActive.Status == enumPPStatus.ppsAborted)
                            {
                                m_LogResult = m_LogResult + "Trigger EVENT  EventAllPostProcess_Failed\r\n";
                                m_pPackEventClientApplication.EvtTriggerEvent(AllEventID.eEventAllPostProcess_Failed, "One postprocess has failed at least", "PostProcess failed");
                            }
                            for (int j = 0; j < currCJData.PJDataList.Count; j++)
                            {
                                foreach (var serverType in currCJData.PJDataList[j].ServerTypeUsedList)
                                {
                                    PostProcessSatus ppStatus = currCJData.PostProcessFollowersManager_PJActive[serverType].PPStatus;
                                    if (currCJData.PostProcessFollowersManager_PJActive.Status == enumPPStatus.ppsComplete)
                                        m_pPackEventPP.EvtOnPPUpdateDisplay(currCJData.CJName, currCJData.PJDataList[j].PJName, serverType, "#+ - Event Complete sent", ppStatus.Status);
                                    else
                                        m_pPackEventPP.EvtOnPPUpdateDisplay(currCJData.CJName, currCJData.PJDataList[j].PJName, serverType, "#+ - Event ERROR sent", ppStatus.Status);
                                }
                            }
                            PostProcessFollowLog(m_LogResult);
                            PostProcessFollowLog("============================================================================\r\n");
                        }
                        catch (Exception ex)
                        {
                            PostProcessFollowLog("Postprocess data generation failed - Exception = " + ex.Message + ex.StackTrace);
                            m_pPackEventClientApplication.EvtTriggerEvent(AllEventID.eEventAllPostProcess_Failed, "postprocess data generation failed", "PostProcess failed");
                            PostProcessFollowLog("####----####----####----####----####----####----####----####----####----####\r\n");
                        }
                    }
                    finally
                    {
                        m_CJDataList.RemoveAt(0);
                    }
                }
            }
        }

        public void Shutdown()
        {
            m_bExitThread = true;
        }
        private void PostProcessFollowLog(String strLogMessage)
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
        private void SetPostProcessDataListInVID(CJData cjData, ref CxValueObjectClass pVIDData)
        {
            String sTabulation = "\t\t\t\t";
            int iHandle2 = 0;
            m_LogResult = m_LogResult + "L[2]\r\n";
            m_LogResult = m_LogResult + sTabulation + "\t" + cjData.CJName + "\r\n"; 
            pVIDData.AppendValueAscii(iHandle2, cjData.CJName);
            m_LogResult = m_LogResult + sTabulation + "\t" + cjData.CarrierID + "\r\n";
            pVIDData.AppendValueAscii(iHandle2, cjData.CarrierID);
        }
    }
}
