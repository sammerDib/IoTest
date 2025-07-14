using System;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger
{
    public class LogEntry
    {
        private int m_logSource;
        private TypeOfEvent m_eventType;
        private String m_entry;
        private DateTime m_time;
        public int LogSource
        {
            get { return m_logSource; }
            set { m_logSource = value; }
        }
        public TypeOfEvent EventType
        {
            get { return m_eventType; }
            set { m_eventType = value; }
        }
        public String Entry
        {
            get { return m_entry; }
            set { m_entry = value; }
        }

        public DateTime Time
        {
            get { return m_time; }
            set { m_time = value; }
        }

        public LogEntry(DateTime Time,int Source, TypeOfEvent EventType, String Entry)
        {
            m_time = Time;
            m_logSource = Source;
            m_eventType = EventType;
            m_entry = Entry;
        }
    }
}
