using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using Microsoft.Win32;

namespace UnitySC.PM.PSD.Tools.NanoTopo.GlobalNanotopography
{
    public class CConfiguration
    {
        const String SETTINGS_INI = @"C:\Altasight\Nano\IniRep\NanoTopo.ini";

        //ProductionMode        
        public static short ProductionModePortNumber
        {
            get { return Convert.ToInt16(Win32Tools.GetIniFileInt(SETTINGS_INI, "ProdMode", "ConnectionPortNumber", 13800)); }
        }       
    }
}
