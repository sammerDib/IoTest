using System;
using System.Collections.Generic;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger;

namespace LogWorker
{
    public class LogObsWorker
    {
        private List<LoggerObserver> m_ListLoggerObs;

        public LogObsWorker()
        {
            m_ListLoggerObs = new List<LoggerObserver>();
        }

        public void AddObserver(LoggerObserver oLogObs)
        {
              if (!m_ListLoggerObs.Contains(oLogObs))
                    m_ListLoggerObs.Add(oLogObs);
        }
        
        public void RemoveObserver(LoggerObserver oLogObs)
        {
            if (m_ListLoggerObs.Contains(oLogObs))
                m_ListLoggerObs.Remove(oLogObs);
        }

        public void WriteEntry(int nSource, int nEventType, String Entry)
        {
            DateTime NowDT = DateTime.Now;
            foreach (LoggerObserver LObserver in m_ListLoggerObs)
            {
                LObserver.WriteLogEntry(NowDT,  nSource, (TypeOfEvent) nEventType, Entry);
            }
        }
    }
}
