using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;

using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Common;
using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.Logger;

using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.IniFile;

using LogWorker;
using Wrapper;


namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public partial class ManualForm : Form
    {
        private Logger m_oLog;
        private LogObsWorker m_oLogObsWorker;
        private LogPanel m_oLogPanel;
        private NanoCore m_oNanoCore;
        public IniFile m_oIniFile;
        private Thread m_oLocalThread;

        public delegate void Delg_EmergencyStop();
        public delegate void Delg_Enabled(bool p_bEnabled);

        private Delg_EmergencyStop m_delgStop;
        private Delg_Enabled m_delgEnabled;
        private Delg_Enabled m_delgSaveData;

        public ManualForm(ref Logger p_oLogger, ref LogObsWorker p_oLogObsWorker, ref NanoCore p_oNanoCore)
        {
            InitializeComponent();

            SplashScreen.ShowSplashScreen();

            m_oLogPanel = new LogPanel();
            p_oLogObsWorker.AddObserver(m_oLogPanel);

            m_oLog = p_oLogger;
            m_oLogObsWorker = p_oLogObsWorker;
            m_oNanoCore = p_oNanoCore;

            ManualLogPanel.Controls.Add(m_oLogPanel);
            m_oLogPanel.Dock = DockStyle.Fill;

            m_delgStop = new Delg_EmergencyStop(EmergencyStop_Delegate);
            m_delgEnabled = new Delg_Enabled(Enabled_Delegate);
            m_delgSaveData = new Delg_Enabled(EnabledSaveData_Delegate);

            // Read Data from ini file
            m_oIniFile = m_oNanoCore.m_oIniFile;
            string sInputPath = m_oIniFile.IniReadValue("LocalMode", "InputsPath", "C:\\Altasight\\Nano\\Data");
            string sOutputPath = m_oIniFile.IniReadValue("LocalMode", "ResultsPath", "C:\\Altasight\\Nano\\Data\\Res");
            string sLotID = m_oIniFile.IniReadValue("LocalMode", "LotID", "");
            //int nUse = m_oIniFile.IniReadValue("LocalMode", "UsePreCalibrateFile", 0);
            int nFringePixelNum = m_oIniFile.IniReadValue("LocalMode", "FringePixelNum", 16);

            //checkBoxPreCalibrate.Checked = (nUse != 0);
            textBoxInputPath.Text = sInputPath;
            textBoxOutputPath.Text = sOutputPath;
            textBoxLotID.Text = sLotID;
            textBoxPeriod.Text = nFringePixelNum.ToString();

            m_oNanoCore.UpdateTreatPrmFromIni();
            m_oNanoCore.InitNanoCoreDLL();

            int noffsetX = m_oIniFile.IniReadValue("Main_Parameters", "ExpdOffsetX", 0);
            int noffsetY = m_oIniFile.IniReadValue("Main_Parameters", "ExpdOffsetY", 0);
            m_oNanoCore.SetExpandOffsetsDLL(noffsetX, noffsetY);
            int nNUIENable = m_oIniFile.IniReadValue("Main_Parameters", "UseNUI", 1);
            m_oNanoCore.SetNUIDLL(nNUIENable);

            SplashScreen.CloseForm();
        }

        private void Enabled_Delegate(bool p_bEnable)
        {
            button1.Enabled = p_bEnable;
            Stop.Enabled = !p_bEnable;
            textBoxInputPath.Enabled = p_bEnable;
            textBoxOutputPath.Enabled = p_bEnable;
            textBoxPeriod.Enabled = p_bEnable;
            textBoxLotID.Enabled = p_bEnable;
            _PhasesTiff.Enabled = p_bEnable;
            _PhasesBin.Enabled = p_bEnable;
            //_NormalsHbf.Enabled = p_bEnable;
            _NormalsTiff.Enabled = p_bEnable;
            if (p_bEnable)
                Cursor.Current = Cursors.Default;         // Back to normal 
            else
                Cursor.Current = Cursors.WaitCursor;       // Waiting / hour glass 
        }

        private void EnabledSaveData_Delegate(bool p_bEnable)
        {
            long uFlag = 0;
            if (p_bEnable)
                uFlag |= 0x1;
            m_oNanoCore.SetFilesGenerationDLL(uFlag);
        }

        private void LaunchMes()
        {
            try
            {
                bool bUnwrappedPhases = false;
                int Period_pixels = Convert.ToInt32(textBoxPeriod.Text);
                string sFoupId = "";
                string path = textBoxInputPath.Text + "\\Pic_" + textBoxLotID.Text + "_dat.bin";
                try
                {
                    if (File.Exists(path))
                    {
                        using (StreamReader sr = new StreamReader(path))
                        {
                            while (sr.Peek() >= 0)
                            {
                                string sval = sr.ReadLine();
                                var parts = sval.Split('=');
                                if (parts[0] == "PixelPeriod")
                                {
                                    Period_pixels = Convert.ToInt32(parts[1]);
                                }
                                else if (parts[0] == "LotID")
                                {
                                    sFoupId = parts[1];
                                }
                            }
                            sr.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    //  Console.WriteLine("The process failed: {0}", e.ToString());
                }

                int nRes = m_oNanoCore.LaunchMesureDLL(
                    textBoxInputPath.Text,
                    textBoxOutputPath.Text,
                    textBoxLotID.Text,
                    _PhasesTiff.Checked ? NanoCore.FilesType.Tiff : (_PhasesBin.Checked ? NanoCore.FilesType.Bin : NanoCore.FilesType.Tiff),
                    Period_pixels,
                    sFoupId,
                    bUnwrappedPhases);
            }
            catch
            {
                System.Diagnostics.Trace.WriteLine("### LaunchMes : EXCEPTION");
                DateTime NowDT = DateTime.Now;
                m_oLog.WriteLogEntry(NowDT, 1, TypeOfEvent.ERROR, "### LaunchMes : EXCEPTION");
            }
            this.Invoke(m_delgEnabled, true);
            m_oLocalThread.Abort();
        }

        private void EmergencyStop_Delegate()
        {
            m_oNanoCore.EmergencyStop();
        }

        public new void Dispose()
        {
            if (m_oLocalThread != null)
            {
                m_oLocalThread.Abort();
            }
            m_oLogPanel.Dispose();
            base.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Invoke(m_delgStop);

            // Save Data to ini file
            m_oIniFile.IniWriteValue("LocalMode", "InputsPath", textBoxInputPath.Text);
            m_oIniFile.IniWriteValue("LocalMode", "ResultsPath", textBoxOutputPath.Text);
            m_oIniFile.IniWriteValue("LocalMode", "LotID", textBoxLotID.Text);
            //int nUsePrecalibrate = Convert.ToInt32(checkBoxPreCalibrate.Checked);
            //m_oIniFile.IniWriteValue("LocalMode", "UsePreCalibrateFile", nUsePrecalibrate);
            int nFringePixelNum = Convert.ToInt32(textBoxPeriod.Text);
            m_oIniFile.IniWriteValue("LocalMode", "FringePixelNum", nFringePixelNum);

            m_oLogObsWorker.RemoveObserver(m_oLogPanel);
            m_oNanoCore.ReleaseNanoCoreDLL();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Invoke(m_delgEnabled, false);
            m_oLocalThread = new Thread(new ThreadStart(LaunchMes));
            m_oLocalThread.Start();
        }

        private void DbgTest_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(m_delgStop);
        }

        private void checkBoxSaveData_CheckedChanged(object sender, EventArgs e)
        {
            this.BeginInvoke(m_delgSaveData, checkBoxSaveData.Checked);
        }
    }
}
