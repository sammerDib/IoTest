using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public enum enumADCType { atFRONTSIDE = 0, atBACKSIDE, atEDGE, atDARKVIEW, atBRIGHTFIELD1, atBRIGHTFIELD2, atBRIGHTFIELD3, atBRIGHTFIELD4, atLIGHTSPEED, atCOUNT }

    public class CADCConfiguration
    {
        String m_FilePathName = "";
        String m_Section ="";
        enumADCType m_ADCType;

        public bool bEnabled
        {
            get { return (Win32Tools.GetIniFileInt(m_FilePathName, m_Section, "Enabled", 0) == 1); }            
        }

        public bool bEnabledCheckingConnection
        {
            get { return (Win32Tools.GetIniFileInt(m_FilePathName, m_Section, "EnabledCheckingConnection", 0) == 1); }            
        }

        public bool bEnabledCheckingRecipeExist
        {
            get { return (Win32Tools.GetIniFileInt(m_FilePathName, m_Section, "EnabledCheckingRecipeExist", 0) == 1); }
        }
        public String ServerName
        {
            get { return Win32Tools.GetIniFileString(m_FilePathName, m_Section, "ServerName", ""); }            
        }

        public String PortNum
        {
            get { return Win32Tools.GetIniFileString(m_FilePathName, m_Section, "PortNum", ""); }
        }

        public int TimeOut
        {
            get { return (Win32Tools.GetIniFileInt(m_FilePathName, m_Section, "TimeOut", 0));}            
        }

        public int Mode_AutoConnect
        {
            get { return (Win32Tools.GetIniFileInt(m_FilePathName, m_Section, "Mode_AutoConnect", 0)); }
        }

        public int DelayAutoConnect_ms
        {
            get { return (Win32Tools.GetIniFileInt(m_FilePathName, m_Section, "DelayAutoConnect_ms", 0)); }            
        }

        public String Communication
        {
            get { return Win32Tools.GetIniFileString(m_FilePathName, m_Section, "Communication", ""); }            
        }

        public CADCConfiguration(String pFilePathName, enumADCType pADCType)
        {
            m_FilePathName = pFilePathName;
            m_ADCType = pADCType;

            switch (pADCType)
            {
                case enumADCType.atFRONTSIDE:       m_Section = "ADC FRONTSIDE";        break;
                case enumADCType.atBACKSIDE:        m_Section = "ADC BACKSIDE";         break;
                case enumADCType.atEDGE:            m_Section = "ADC EDGE";             break;
                case enumADCType.atDARKVIEW:        m_Section = "ADC DARKVIEW";        break;
                case enumADCType.atBRIGHTFIELD1:    m_Section = "ADC BRIGHTFIELD 1";    break;
                case enumADCType.atBRIGHTFIELD2:    m_Section = "ADC BRIGHTFIELD 2";    break;
                case enumADCType.atBRIGHTFIELD3:    m_Section = "ADC BRIGHTFIELD 3";    break;
                case enumADCType.atBRIGHTFIELD4:    m_Section = "ADC BRIGHTFIELD 4"; break;
                case enumADCType.atLIGHTSPEED:      m_Section = "ADC LIGHTSPEED"; break;
                case enumADCType.atCOUNT:
                default: m_Section = "UNKNOWN"; break;
            }
        }
    }
}
