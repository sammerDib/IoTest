using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger;

using System.Threading;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public delegate void AddLogEntryDelegate(DateTime Time, int Source, TypeOfEvent EventType, string Entry);

    public partial class LogPanel : UserControl, LoggerObserver
    {
        private AddLogEntryDelegate m_addLogEntry;
        private Queue<LogEntry> m_entryQueue;
        private Semaphore m_addLogSemaphore;
        private Thread m_pumpThread;
        private bool m_bRun = true;
        private int m_nLimitLog = 200;
        public int LimitLog
        {
            get { return m_nLimitLog; }
            set { m_nLimitLog = value; }
        }

        public LogPanel()
        {
            InitializeComponent();

            m_addLogSemaphore = new Semaphore(0, 100);
            m_pumpThread = new Thread(new ThreadStart(PumpMessages));
            m_addLogEntry = new AddLogEntryDelegate(WriteLogEntry_Delegate);
            m_entryQueue = new Queue<LogEntry>();
            m_pumpThread.Start();

            System.Diagnostics.Trace.Indent();
        }

        private void PumpMessages()
        {
            while (m_bRun)
            {
                if (IsHandleCreated && m_addLogSemaphore.WaitOne(2000))
                {
                    try
                    {
                        if (m_entryQueue.Count > 0)
                        {
                            LogEntry Entry = m_entryQueue.Dequeue();
                            this.BeginInvoke(m_addLogEntry, Entry.Time, Entry.LogSource, Entry.EventType, Entry.Entry); 
                        }
                    }
                    catch
                    {
                        System.Diagnostics.Trace.WriteLine("### PumpMessages : EXCEPTION");
                    }
                }
            }
        }

        public void WriteLogEntry(DateTime Time, int Source, TypeOfEvent EventType, string Entry)
        {
//             if (this.Visible && this.IsHandleCreated)
//             {
//                 this.Invoke(m_addLogEntry, Time, Source, EventType, Entry);
//             }
//             else
            {
                LogEntry logEntry = new LogEntry(Time, Source, EventType, Entry);
                m_entryQueue.Enqueue(logEntry);
                m_addLogSemaphore.Release();
            }
        }

        protected void UpdateLogEntries(ref ListViewItem p_LVi)
        {
            //To prevent listbox jumping about as we delete and scroll
            LoglistView.BeginUpdate();
            //Work out if we have too many items and have to delete
            if (LoglistView.Items.Count >= m_nLimitLog)
            {
                //Remove top items
                int nDiff = (LoglistView.Items.Count - m_nLimitLog) + 1;
                while (nDiff > 0)
                {
                    LoglistView.Items.Remove(LoglistView.Items[0]);
                    --nDiff;
                }
            }
            LoglistView.EndUpdate();

//             string csmsg = "### ** @@@@ UpdateLogEntries : Add ";
//             csmsg += p_LVi.SubItems[2].ToString();
//             System.Diagnostics.Trace.WriteLine(csmsg);

            //Add new item
            LoglistView.Items.Add(p_LVi);  
            p_LVi.EnsureVisible(); // allow scrolling down
   
        }

        protected void WriteLogEntry_Delegate(DateTime Time, int Source, TypeOfEvent EventType, string Entry)
        {
            try
            {
                ListViewItem LVi = new ListViewItem();
                LVi.Text = Time.ToString("MM-dd-yyyy hh:mm:ss.fff tt");
                LVi.SubItems.Add(EventType.ToString());
                LVi.SubItems.Add(Entry);
                switch (EventType)
                {
                    case TypeOfEvent.TIMING:
                        break;
                    case TypeOfEvent.ACTION:
                        LVi.ForeColor = Color.Green;
                        break;
                    case TypeOfEvent.ERROR:
                        LVi.ForeColor = Color.Red;
                        break;
                    case TypeOfEvent.INFO:
                        break;
                    case TypeOfEvent.WARNING:
                        LVi.ForeColor = Color.Orange;
                        break;
                    default:
                        break;
                }
                UpdateLogEntries(ref LVi);
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine("### WriteLogEntry_Delegate : EXCEPTION");
            
            }
        }

        public new void Dispose()
        {
            m_bRun = false;
            m_pumpThread.Abort();
            base.Dispose();
        }

        private void LoglistView_SizeChanged(object sender, EventArgs e)
        {
           (sender as ListView).Columns[(sender as ListView).Columns.Count - 1].Width = -2;
        }
    }
}
