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

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public partial class ConfigPrmForm : Form
    {
        private LogObsWorker m_oLogObsWorker;
        private LogPanel m_oLogPanel;
        private NanoCore m_oNanoCore;

        // Treatment
        private ConfigTreatPrmPanel[] m_oTreatPrmCfgPanel;

        public ConfigPrmForm(ref LogObsWorker p_oLogObsWorker, ref NanoCore p_oNanoCore)
        {
            InitializeComponent();

            m_oLogPanel = new LogPanel();
            p_oLogObsWorker.AddObserver(m_oLogPanel);

            m_oLogObsWorker = p_oLogObsWorker;
            m_oNanoCore = p_oNanoCore;

            ConfLogPanel.Controls.Add(m_oLogPanel);
            m_oLogPanel.Dock = DockStyle.Fill;

            m_oNanoCore.ConfigureNanoCoreDLL(); 
            
            m_oTreatPrmCfgPanel = new ConfigTreatPrmPanel[Enum.GetNames(typeof(TreatID)).Length];
            for (uint uId = 0; uId < Enum.GetNames(typeof(TreatID)).Length; uId++)
            {
                m_oTreatPrmCfgPanel[uId] = new ConfigTreatPrmPanel(/*m_oNanoCore.m_sTreatName[uId]*/);

                TabPage myTabPage  = new TabPage(m_oNanoCore.m_sTreatName[uId]);
                myTabPage.Controls.Add(m_oTreatPrmCfgPanel[uId]);
                tabControl1.TabPages.Add(myTabPage);

            }
        }

        public new void Dispose()
        {
            m_oLogPanel.Dispose();
            base.Dispose();
        }

        private void ConfigPrmForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_oLogObsWorker.RemoveObserver(m_oLogPanel);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            // save & exit
            m_oNanoCore.ConfigureNanoCoreDLL();

            // save to do
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            // exit & discard changes
            this.Close();
        }
       
    }
}
