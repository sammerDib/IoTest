using System;
using System.Globalization;
using System.Windows;

using ADCCommon;

using UnitySC.ADCAS300Like.Common.CVIDObj;

using Microsoft.Win32;

namespace UnitySC.ADCAS300Like.Common.ProcessModule
{
    public static class PMFunctions
    {
        public static string SetItemDataInAList(String itemData, String format, string logData, ref string sTabulation, int iHandleIdentification, CxValueObject list)
        {
            switch (format)
            {
                case "U1":
                    list.AppendValueU1(iHandleIdentification, Convert.ToByte(itemData));
                    break;
                case "U2":
                    list.AppendValueU2(iHandleIdentification, Convert.ToInt16(itemData));
                    break;
                case "U4":
                    list.AppendValueU4(iHandleIdentification, Convert.ToInt32(itemData));
                    break;
                case "U8":
                    list.AppendValueU8(iHandleIdentification, Convert.ToInt32(itemData));
                    break;
                case "F4":
                    list.AppendValueF4(iHandleIdentification, Convert.ToSingle(itemData));
                    break;
                case "F8":
                    list.AppendValueF8(iHandleIdentification, Convert.ToDouble(itemData, CultureInfo.InvariantCulture));
                    break;
                case "ASCII":
                default:
                    list.AppendValueAscii(iHandleIdentification, itemData);
                    break;
            }
            logData += sTabulation + "\t\t" + itemData + "\r\n";
            return logData;
        }

        public static string msToolInitializedKey = "SOFTWARE\\Cimetrix\\ToolInitialized";
        public static string msToolInitializedValueKey = "Initialized";
        public static string msToolSwapAlignerInProgressValueKey = "SwapAlignerInProgress";
        public static string msToolInitializedChamberOccupiedSubKey = "ChamberOccupied";

        public static void SetChamberOccupied(int ChamberNumber, bool bOccupied)
        {
            RegistryKey vpRegistryKey = null;
            RegistryKey vpSubKeyChamberDirectory = null;
            RegistryKey vpSubKeyChamber = null;
            short shValue = 0;
            try
            {
                try
                {
                    vpRegistryKey = Registry.LocalMachine.CreateSubKey(msToolInitializedKey);
                    vpSubKeyChamberDirectory = vpRegistryKey.CreateSubKey(msToolInitializedChamberOccupiedSubKey);
                    vpSubKeyChamber = vpSubKeyChamberDirectory.CreateSubKey(ChamberNumber.ToString());
                    if (bOccupied) shValue = 1;
                    vpSubKeyChamber.SetValue(msToolInitializedChamberOccupiedSubKey, shValue, RegistryValueKind.DWord);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                }
            }
            finally
            {
                if (vpRegistryKey != null)
                    vpRegistryKey.Close();
                if (vpSubKeyChamberDirectory != null)
                    vpSubKeyChamberDirectory.Close();
                if (vpSubKeyChamber != null)
                    vpSubKeyChamber.Close();
            }
        }

        public static bool GetChamberOccupied(int ChamberNumber)
        {
            RegistryKey vpRegistryKey = null;
            RegistryKey vpSubKeyChamberDirectory = null;
            RegistryKey vpSubKeyChamber = null;
            int iOccuppied = 1;
            try
            {
                try
                {
                    vpRegistryKey = Registry.LocalMachine.CreateSubKey(msToolInitializedKey);
                    vpSubKeyChamberDirectory = vpRegistryKey.CreateSubKey(msToolInitializedChamberOccupiedSubKey);
                    vpSubKeyChamber = vpSubKeyChamberDirectory.CreateSubKey(ChamberNumber.ToString());
                    iOccuppied = (int)vpSubKeyChamber.GetValue(msToolInitializedChamberOccupiedSubKey, false, RegistryValueOptions.None);
                    return (iOccuppied==1);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.ToString());
                    return true;
                }
            }
            finally
            {
                if (vpRegistryKey != null)
                    vpRegistryKey.Close();
                if (vpSubKeyChamberDirectory != null)
                    vpSubKeyChamberDirectory.Close();
                if (vpSubKeyChamber != null)
                    vpSubKeyChamber.Close();
            }
        }
    }

}
