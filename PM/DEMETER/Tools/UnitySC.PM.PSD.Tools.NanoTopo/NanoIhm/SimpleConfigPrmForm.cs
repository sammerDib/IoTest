using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;


using UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.IniFile;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public partial class SimpleConfigPrmForm : Form
    {
        public IniFile m_oIniFile;
        public const string g_sIniPath = @"C:\Altasight\Nano\IniRep\NanoTopo.ini";
        public string m_sTreatGenerateResultSection;
        public string m_sTreatPrepSection;
        public double m_dSiteLimitFactor;
        public double m_dSiteTxtFactor;
        public SimpleConfigPrmForm()
        {
            InitializeComponent();

            try
            {
                m_oIniFile = new IniFile(g_sIniPath);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString(), "Fatal Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                // System.Windows.Forms.Application.Exit(); // warning l'exe reste ca kill pas le process
            }

            m_sTreatPrepSection = m_oIniFile.IniReadValue("Treatments", "T1", "");
            m_sTreatPrepSection = System.IO.Path.GetFileNameWithoutExtension(m_sTreatPrepSection);

            string sGenerateResultTreatID = "T4";
            string sTreatName = m_oIniFile.IniReadValue("Treatments", sGenerateResultTreatID, "");
            if (!String.IsNullOrWhiteSpace(sTreatName))
            {
                // Remove ".local" if needed
                String sTreatNameRdx = System.IO.Path.GetFileNameWithoutExtension(sTreatName);
                m_sTreatGenerateResultSection = sTreatNameRdx;

                string sPVDisk = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "UseDiskPV", "0");
                int nUsePVDisk = Convert.ToInt32(sPVDisk);
                checkBoxUseDiskPV.Checked = (nUsePVDisk != 0);
                string sValue = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "EdgeExclusion", "3");
                float fval = Convert.ToSingle(sValue, CultureInfo.InvariantCulture.NumberFormat);
                if (fval < 2.0f)
                {
                    radioButton1.Checked = true;
                }
                else if (fval < 3.0f)
                {
                    radioButton2.Checked = true;
                }
                else
                {
                    radioButton3.Checked = true;
                }

                string sResumeLot = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "UseResumeLotStats", "0");
                int nResumeLot = Convert.ToInt32(sResumeLot);
                checkBoxResumeLotEnabled.Checked = (nResumeLot != 0);

                string sUseSitePV = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "UseSitePVMap", "0");
                int nUseSitePv = Convert.ToInt32(sUseSitePV);
                checkBoxSitePV.Checked = (nUseSitePv != 0);

                textBoxSiteWidth.Text = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteWidthmm", "25.0");
                textBoxSiteHeight.Text = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteHeightmm", "8.0");
                textBoxSiteOffsetX.Text = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteOffsetXmm", "0.0");
                textBoxSiteOffsetY.Text = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteOffsetYmm", "0.0");
                textBoxThresh1.Text = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteThresh1nm", "4.0");
                textBoxThresh2.Text = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteThresh2nm", "8.0");

                m_dSiteLimitFactor = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteLimitFactor", 0.5);
                m_dSiteTxtFactor = m_oIniFile.IniReadValue(m_sTreatGenerateResultSection, "SiteTxtFactor", 1.0);

            }
        }

        private void WriteDataIni(int p_nEdgeExclusion)
        {
            NanoCore.CalibPaths calibPaths = new NanoCore.CalibPaths(NanoCore.GetCalibFolderStructure());
            string sIniFileEE = Path.Combine(calibPaths.LastCalibPath, calibPaths.UWPhiSubfolder, "NanEEleK.dat");
            IniFile NanoEdgeIniFile;
            if (File.Exists(sIniFileEE))
            {
                NanoEdgeIniFile = new IniFile(sIniFileEE);
            }
            else
            {
                // create it
                FileStream fs = null;
                using (fs = File.Create(sIniFileEE))
                {

                }

                NanoEdgeIniFile = new IniFile(sIniFileEE);

                // Add element
                string newsection = "0";
                NanoEdgeIniFile.IniWriteValue(newsection, "Order", 4);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepErode", 19);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepDilate", 41);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepPro", 25.0);
                NanoEdgeIniFile.IniWriteValue(newsection, "GenErode", 5);

                newsection = "1";
                NanoEdgeIniFile.IniWriteValue(newsection, "Order", 4);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepErode", 25);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepDilate", 67);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepPro", 48.0);
                NanoEdgeIniFile.IniWriteValue(newsection, "GenErode", 5);

                newsection = "2";
                NanoEdgeIniFile.IniWriteValue(newsection, "Order", 4);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepErode", 29);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepDilate", 89);
                NanoEdgeIniFile.IniWriteValue(newsection, "PrepPro", 64.0);
                NanoEdgeIniFile.IniWriteValue(newsection, "GenErode", 5);
            }

            string section = p_nEdgeExclusion.ToString();
            int nVal;
            nVal = NanoEdgeIniFile.IniReadValue(section, "Order", 4);
            m_oIniFile.IniWriteValue("TreatDegauchy0", "Order", nVal);

            nVal = NanoEdgeIniFile.IniReadValue(section, "PrepErode", 25);
            m_oIniFile.IniWriteValue(m_sTreatPrepSection, "ErodeRadius", nVal);

            nVal = NanoEdgeIniFile.IniReadValue(section, "PrepDilate", 60);
            m_oIniFile.IniWriteValue(m_sTreatPrepSection, "DilateRadius", nVal);

            double dVal = NanoEdgeIniFile.IniReadValue(section, "PrepPro", 35.0);
            m_oIniFile.IniWriteValue(m_sTreatPrepSection, "ProlongeDistStop", dVal);

            nVal = NanoEdgeIniFile.IniReadValue(section, "GenErode", 60);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "ErodeRadius", nVal);

            // Site Pv Image Info
            int nResumeLot = Convert.ToInt32(checkBoxResumeLotEnabled.Checked);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "UseResumeLotStats", nResumeLot);

            int nUseSitePv = Convert.ToInt32(checkBoxSitePV.Checked);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "UseSitePVMap", nUseSitePv);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteWidthmm", textBoxSiteWidth.Text);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteHeightmm", textBoxSiteHeight.Text);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteOffsetXmm", textBoxSiteOffsetX.Text);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteOffsetYmm", textBoxSiteOffsetY.Text);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteThresh1nm", textBoxThresh1.Text);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteThresh2nm", textBoxThresh2.Text);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteLimitFactor", m_dSiteLimitFactor.ToString(CultureInfo.InvariantCulture));
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "SiteTxtFactor", m_dSiteTxtFactor.ToString(CultureInfo.InvariantCulture));
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            int nUsePVDisk = Convert.ToInt32(checkBoxUseDiskPV.Checked);
            m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "UseDiskPV", nUsePVDisk);

            if (radioButton1.Checked == true) // 1.5 mm
            {
                m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "EdgeExclusion", "1.5");
                WriteDataIni(0);
            }
            else if (radioButton2.Checked == true) // 2 mm
            {
                m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "EdgeExclusion", "2.0");
                WriteDataIni(1);
            }
            else if (radioButton3.Checked == true) // 3mm
            {
                m_oIniFile.IniWriteValue(m_sTreatGenerateResultSection, "EdgeExclusion", "3.0");
                WriteDataIni(2);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Error no edge exclusion selected");
            }

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
