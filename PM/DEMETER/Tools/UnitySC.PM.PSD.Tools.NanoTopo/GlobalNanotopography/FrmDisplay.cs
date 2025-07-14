using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public partial class FrmDisplay : Form
    {
        public FrmDisplay(String Title, string Msg)
        {
            InitializeComponent();
            String[] sTab = Msg.Split('#');
            LbErrorMsg.Text = sTab[0];
            if (sTab.Length > 1)
                LbErrorMsg2.Text = sTab[1];
            else
                LbErrorMsg2.Text = "";
            LbTitle.Text = Title;
        }

        private void FrmHandShakeError_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel) // IF click corner cross THEN
                e.Cancel = true;                     //     Cancel closing  
        }

        private void CmdOK_Click(object sender, EventArgs e)
        {

        }

        private void FrmError_Shown(object sender, EventArgs e)
        {
            this.TopMost = this.Visible;   
        }
    }
}
