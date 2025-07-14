using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{    
    public partial class FrmConnectionStatus : UserControl
    {
        EventHandlerParam0 m_EvtOnStartClicked;
        EventHandlerParam0 m_EvtOnStopClicked;
        public FrmConnectionStatus()
        {
            InitializeComponent();

            LbTitle.Text = "Connection serveur control";
            LbModule1Name.Text = "No client connected";
            LbModule2Name.Text = "Port#13900";
            CmdStart.Enabled = false;
        }
        public FrmConnectionStatus(String pTitle, String pLights1, String pLights2)
            :this()
        {
            LbTitle.Text = pTitle;
            LbModule1Name.Text = pLights1;
            LbModule2Name.Text = pLights2;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------
        // Properties
        public enumConnectionLights this[enumSelectedLights Index]
        {
            set { 
                    //m_Lights[(int)Index] = value;
                    switch (Index)
                    {
                        case enumSelectedLights.sConnectionState:
                            PB_M1_ABS.Visible = (value == enumConnectionLights.clABS);
                            PB_M1_ON.Visible = (value == enumConnectionLights.clOK);
                            PB_M1_OFF.Visible = (value == enumConnectionLights.clNC); 
                            break;                        
                        default:
                            break;
                    }
                    if (value == enumConnectionLights.clOK)
                    {
                        LbModule1Name.Text = "Client connected";
                    }else
                        if (value == enumConnectionLights.clNC)
                            LbModule1Name.Text = "No client connected";
                }
        }

        public String ClientName
        {
            get { return LbModule1Name.Text; }
        }

        private void CmdStart_Click(object sender, EventArgs e)
        {
            m_EvtOnStartClicked.Invoke();
            CmdStart.Enabled = false;
            CmdStop.Enabled = !CmdStart.Enabled;
        }

        private void CmdStop_Click(object sender, EventArgs e)
        {
            m_EvtOnStopClicked.Invoke();
            CmdStart.Enabled = true;
            CmdStop.Enabled = !CmdStart.Enabled;
        }

        public void SetEventClick(EventHandlerParam0 pEvtOnStartClicked, EventHandlerParam0 pEvtOnStopClicked)
        {
            m_EvtOnStartClicked = pEvtOnStartClicked;
            m_EvtOnStopClicked = pEvtOnStopClicked;
        }

    }

    public enum enumSelectedLights { sConnectionState}
    public enum enumConnectionLights { clABS, clNC, clOK }

    public delegate void EventHandlerParam0();
    
}
