using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoIhm
{
  
    public partial class SplashScreen : Form
    {
        private double m_dblOpacityIncrement = .1;
        private double m_dblOpacityDecrement = .1;
        private const int TIMER_INTERVAL = 50;

        static Thread ms_oThread = null;

        static SplashScreen ms_frmSplash = null;
        // A static entry point to launch SplashScreen.
        static private void ShowForm()
        {
            ms_frmSplash = new SplashScreen();
            Application.Run(ms_frmSplash);
        }
        // A static method to close the SplashScreen
        static public void CloseForm()
        {
            if (ms_frmSplash != null)
            {
                // Make it start going away.
                ms_frmSplash.m_dblOpacityIncrement = -ms_frmSplash.m_dblOpacityDecrement;
            }
            ms_oThread = null;  // we do not need these any more.
            ms_frmSplash = null;
        }

        public SplashScreen()
        {
            InitializeComponent();

          //  this.ClientSize = this.BackgroundImage.Size;
            this.Opacity = .0;
            timer1.Interval = TIMER_INTERVAL;
            timer1.Start();
        }

        static public void ShowSplashScreen()
        {
          // Make sure it is only launched once.
          if( ms_frmSplash != null )
            return;
          ms_oThread = new Thread( new ThreadStart(SplashScreen.ShowForm));
          ms_oThread.IsBackground = true;
          ms_oThread.ApartmentState = ApartmentState.STA;
          ms_oThread.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_dblOpacityIncrement > 0)
            {
                if (this.Opacity < 1)
                    this.Opacity += m_dblOpacityIncrement;
            }
            else
            {
                if (this.Opacity > 0)
                    this.Opacity += m_dblOpacityIncrement;
                else
                    this.Close();
            }
        }
    }
}
