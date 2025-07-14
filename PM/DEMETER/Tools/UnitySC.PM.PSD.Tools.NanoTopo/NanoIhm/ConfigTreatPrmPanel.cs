using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
    public partial class ConfigTreatPrmPanel : UserControl
    {
        public ConfigTreatPrmPanel()
        {
            InitializeComponent();

            for (int i = 0; i < 7; i++)
            {
                Label lbl = new Label();
                TextBox Text = new TextBox();
                lbl.Text = "lbl" + i.ToString();
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                Text.Text = "Text" + i.ToString();
                Text.Dock = DockStyle.Fill;
                Text.TextAlign = HorizontalAlignment.Left;
                tableLayoutPanel1.Controls.Add(lbl, 0, i);
                tableLayoutPanel1.Controls.Add(Text, 1, i);
          
            }
        }
    }
}
