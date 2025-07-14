using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoTools.ImageViewer
{
    public delegate void PreviewActionDelegate(object sender, EventArgs e);

    public partial class ImgPreviewer : UserControl
    {   
        public KeyPressEventHandler     m_dlgNavKeyPress = null;
        public EventHandler             m_dlgNavNext = null;
        public EventHandler             m_dlgNavBack = null;
        public EventHandler             m_dlgPanelMouseEnter = null;
        public MouseEventHandler        m_dlgPanelMouseUp = null;
        public MouseEventHandler        m_dlgPanelMouseMove = null;
        public MouseEventHandler        m_dlgPanelMouseDown = null;

        public Color MenuColor
        {
            get { return panelPreview.BackColor; }
            set
            {
                panelPreview.BackColor = value;
                panelNavigation.BackColor = value;
            }
        }

        public Color NavigationPanelColor
        {
            get { return panelNavigation.BackColor; }
            set
            {
                panelNavigation.BackColor = value;
            }
        }

        public Color PreviewPanelColor
        {
            get { return panelPreview.BackColor; }
            set
            {
                panelPreview.BackColor = value;
            }
        }

        public Color NavigationTextColor
        {
            get { return lblNavigation.ForeColor; }
            set { lblNavigation.ForeColor = value; }
        }

        public String NavigationText
        {
            get { return tbNavigation.Text; }
            set { tbNavigation.Text = value; }
        }

        public Color TextColor
        {
            get { return lblPreview.ForeColor; }
            set
            {
                lblPreview.ForeColor = value;
                lblNavigation.ForeColor = value;
            }
        }

        public Color PreviewTextColor
        {
            get { return lblPreview.ForeColor; }
            set { lblPreview.ForeColor = value; }
        }

        public string PreviewText
        {
            get { return lblPreview.Text; }
            set { lblPreview.Text = value; }
        }

        public Size PicBoxSize
        {
            get { return pbPanel.Size; }
            set { pbPanel.Size = value; }
        }

        public Image Image
        {
            set { pbPanel.Image = value; }
        }

        public ImgPreviewer()
        {
            InitializeComponent();
        }

        public bool IsPreviewVisible()
        {
            return pbPanel.Visible;
        }

        public void HidePreview(bool isMulti)
        {
            panelPreview.Hide();
            pbPanel.Hide();
            if (isMulti)
            {
                panelNavigation.Location = panelPreview.Location;
            }
        }

        public void ShowPreview(bool isMulti)
        {
            panelPreview.Show();
            pbPanel.Show();
            if (isMulti)
            {
                panelNavigation.Location = new Point(panelPreview.Location.X, (pbPanel.Location.Y + (pbPanel.Size.Height + 5)));
             
            }
        }

        public void ToggleMultiPage(bool isMultiPage, int n1, int n2, bool showPreview, bool isDistant)
        {
            if (isMultiPage)
            {
                if (!showPreview)
                {
                    panelNavigation.Location = panelPreview.Location;
                }
                else
                {
                    panelNavigation.Location = new Point(panelPreview.Location.X, (pbPanel.Location.Y + (pbPanel.Size.Height + 5)));
                }

                panelNavigation.Show();
                lblNavigation.Text = "/ " + n1.ToString();
                tbNavigation.Text = n2.ToString();
            }
            else
            {
                panelNavigation.Hide();
                lblNavigation.Text = "/ 0";
                tbNavigation.Text = "0";
            }
        }

        private void tbNavigation_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (m_dlgNavKeyPress != null)
                this.Invoke(m_dlgNavKeyPress, sender, e);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
           if(m_dlgNavNext != null)
              this.Invoke(m_dlgNavNext, sender, e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (m_dlgNavBack != null)
              this.Invoke(m_dlgNavBack, sender, e);
        }

        private void pbPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_dlgPanelMouseUp != null)
                this.Invoke(m_dlgPanelMouseUp, sender, e);
        }

        private void pbPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_dlgPanelMouseMove != null)
                this.Invoke(m_dlgPanelMouseMove, sender, e);
        }

        private void pbPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (m_dlgPanelMouseDown != null)
                this.Invoke(m_dlgPanelMouseDown, sender, e);
        }

        private void ImgPreviewer_MouseEnter(object sender, EventArgs e)
        {
            if (m_dlgPanelMouseEnter != null)
                this.Invoke(m_dlgPanelMouseEnter, sender, e);
        }
    }
}
