using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger;

using LogWorker;
using Wrapper;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public partial class NanoIHMSelecter : Form
    {
        private Logger          m_oLog;
        private LogObsWorker    m_oLogObsWorker;
        private NanoCore        m_oNanoCore;

        // TO DO : passser les données suivante en prm appli
        const String g_sLogPath = @"..\Logs";
        const int g_nbBackupLogs = 10;
        const int g_nsizeInMBytes = 10;
        const int g_nlogLevel = 0;

        public NanoIHMSelecter()
        {
            InitializeComponent();

            // Log Initialisation
            string[] sLogSourceName = { "Nanotopo" };
            m_oLog = new Logger(g_sLogPath, g_nbBackupLogs, g_nsizeInMBytes, g_nlogLevel, sLogSourceName);
            m_oLog.RegisterLogger(m_oLog);

            m_oLogObsWorker = LogWrapper.Instance.GetLogObsWorker();
            m_oLogObsWorker.AddObserver(m_oLog);

            m_oNanoCore = new NanoCore();
        }

        private void LaunchProductionButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            ProdForm myForm = new ProdForm(ref m_oLog, ref m_oLogObsWorker, ref m_oNanoCore);
            myForm.ShowDialog();
            myForm.Dispose();
            this.Show();
        }

        private void LaunchManualButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            ManualForm myForm = new ManualForm(ref m_oLog, ref m_oLogObsWorker, ref m_oNanoCore);
            myForm.ShowDialog();
            myForm.Dispose();
            this.Show();
        }

        private void ConfigParametersButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            SimpleConfigPrmForm myForm = new SimpleConfigPrmForm();
            myForm.ShowDialog();
            myForm.Dispose();
            this.Show();
        }
    }
}
