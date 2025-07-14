using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using Common;

namespace Common
{

    public class CLogItem: IComparer
    {
        public String m_DestinationFileName;
        private String m_LogName;
        public String m_Msg1;
        public String m_Msg2;        
        
        public CLogItem()
        {
        }

        public CLogItem(String pDestinationFileName, String pLogName, String pMsg1, String pMsg2)
        {
            m_DestinationFileName = pDestinationFileName;
            m_LogName = pLogName;
            m_Msg1 = pMsg1;
            m_Msg2 = pMsg2;
        }

        public String LogName
        {
            get { return m_LogName; }
            set { m_LogName = value; }
        }
        
        public String DestinationFileName
        {
            get { return m_DestinationFileName; }
        }

        public String Msg1
        {
            get { return m_Msg1; }
        }

        public String Msg2
        {
            get { return m_Msg2; }
        }

        public CLogItem Clone()
        {
            CLogItem lResult = new CLogItem(m_DestinationFileName, m_LogName, m_Msg1, m_Msg2);
            return lResult;
        }

        public int Compare(Object x, Object y)
        {
            if (((CLogItem)x).DestinationFileName == ((CLogItem)y).DestinationFileName)
                return 0;
            else
                return 1;
        }
    }

    public class CLogManager
    {
        List<CLogItem> m_LogItemsList = new List<CLogItem>();
        Thread m_LogManagerThread ;
        Object m_Parameters = new Object();
        AutoResetEvent m_EvtLogManager = new AutoResetEvent(false);
        public bool m_bLogManagerThreadExit;
        Object m_SynchroLogItems = new Object();

        private Dictionary<String, int> m_SizeInMo = new Dictionary<string,int> ();
        private Dictionary<string,int> m_RotationFilesNbr = new Dictionary<string, int>();
        private Dictionary<string, bool> m_bEnable = new Dictionary<String, bool>();
        private String m_LogSettingsFile;

        public CLogManager(String pLogSettingsFile)
        {
            m_LogSettingsFile = pLogSettingsFile;
            m_LogManagerThread = new Thread(new ParameterizedThreadStart(LogManagerThread_Execute));
            m_LogManagerThread.Name = "LogManagerThread";
            m_LogManagerThread.Start(m_Parameters);
        }

        public void LogManagerThread_Execute(Object Data)
        {
            m_bLogManagerThreadExit = false;
            CLogItem CurrentLogItem = new CLogItem();
            while (!m_bLogManagerThreadExit)
            {
                m_EvtLogManager.WaitOne(5000, true);
                while (!m_bLogManagerThreadExit)
                {
                    lock (m_SynchroLogItems)
                    {
                        CurrentLogItem = null;  
                        if (m_LogItemsList.Count > 0)
                        {
                            CurrentLogItem = m_LogItemsList[0].Clone();
                            m_LogItemsList.RemoveAt(0);
                        }
                    }
                    if (CurrentLogItem != null) 
                        SetLogItemInFile(CurrentLogItem);
                    else
                        break;
                }
            }
        }

        public void SetLogItemInFile(CLogItem LogItem)
        {
            if(!m_SizeInMo.ContainsKey(LogItem.LogName))
                m_SizeInMo.Add(LogItem.LogName, Win32Tools.LogParametersSizeInMo(m_LogSettingsFile, LogItem.LogName));
            if (!m_RotationFilesNbr.ContainsKey(LogItem.LogName))
                m_RotationFilesNbr.Add(LogItem.LogName, Win32Tools.LogParametersRotationNbrFiles(m_LogSettingsFile, LogItem.LogName));

            int lSizeInMo = m_SizeInMo[LogItem.LogName];
            int lRotationFilesNbr = m_RotationFilesNbr[LogItem.LogName];
            String strMsg = LogItem.m_Msg1;
            if (LogItem.m_Msg2.Length > 0)
                strMsg = strMsg + " - " + LogItem.m_Msg2;
            Win32Tools.MyLog(strMsg, LogItem.m_DestinationFileName, lSizeInMo, lRotationFilesNbr);
        }

        public void AddLogItem(String pFileName, String pLogName, String pMsg1)
        {
            AddLogItem(pFileName, pLogName, pMsg1, "");
        }
        public void AddLogItem(String pFileName, String pLogName, String pMsg1, String pMsg2)
        {
            CLogItem NewLogItem = new CLogItem(pFileName, pLogName, pMsg1, pMsg2 );
            lock (m_SynchroLogItems)
            {
                m_LogItemsList.Add(NewLogItem);                                
            }
            m_EvtLogManager.Set();
        }

        public void Shutdown()
        {
            m_bLogManagerThreadExit = true;
            Thread.Sleep(1000);
        }
    }
}
