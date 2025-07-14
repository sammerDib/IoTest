using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace UnitySC.PM.PSD.Tools.NanoTopo.NanoViewer
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new NanoViewerForm(args));
        }
    }
}
