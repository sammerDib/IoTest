using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Shared.DMT.DataCollection
{
    #region ADC defect result example
    /*
    ADC defect result example:
    VID= 990011	L[4]
                    stain
                    FA3
                    L[5]
                            Count
                            L[2]
                                    Bin1
					                0
				            L[2]
                                    Bin2
					                0
				            L[2]
                                    Bin3
					                0
				            L[2]
                                    Bin4
					                0
			        L[5]
                            Size
                            L[2]
                                    Bin1
					                0
				            L[2]
                                    Bin2
					                0
				            L[2]
                                    Bin3
					                0
				            L[2]
                                    Bin4
					                0
    */
    #endregion
    public static class ConversionDefects
    {
        public static String NoVIDProcessDefect_Msg = "VID process defect list is empty";
        public static List<SecsVariable> GetConvertSVsFromDefects(List<CVIDProcessDefect> defects, ref StringBuilder logResult)
        {
            var secsVariables = new List<SecsVariable>();
            if (defects != null)
            {
                List<int> verifiedIds = new List<int>();
                // Regroup vids data by id from defects (Sometimes ADC generate CVIDProcessDefect data with the same VID but separatly in the list :/)
                for (int i = 0; i < defects.Count; i++)
                {
                    int idToCheck = defects[i].m_iVIDValue;
                    List<CVIDProcessDefect> sameVIDProcessDefects = new List<CVIDProcessDefect>();
                    sameVIDProcessDefects.Clear();
                    for (int j = 0; j < defects.Count; j++)
                    {
                        // VID Num
                        int idSelected = defects[j].m_iVIDValue;
                        int pos = verifiedIds.IndexOf(j);
                        if ((idToCheck == idSelected) && (pos < 0))
                        {
                            verifiedIds.Add(j);
                            sameVIDProcessDefects.Add(defects[j]);
                        }
                    }
                    if (sameVIDProcessDefects.Count > 0)
                    {
                        logResult.AppendLine("VID= " + idToCheck.ToString());                        
                        var sv = GetDefectDataInAList(sameVIDProcessDefects, ref logResult);
                        var secsItem = new SecsItem(SecsFormat.List, new SecsItemList(sv));
                        secsVariables.Add(new SecsVariable("PW_"+ sameVIDProcessDefects[0].m_sVIDLabel, secsItem));
                    }
                }
            }
            else
                throw new Exception(NoVIDProcessDefect_Msg);            
            return secsVariables;
        }

        public static List<SecsItem> GetDefectDataInAList(List<CVIDProcessDefect> defects, ref StringBuilder logResult)
        {
            var secsItems = new List<SecsItem>(); // List of each VID data as example VID= 990011
            String tabulations = "\t";
            int nbitems = (defects.Count * 3 + 1);
            logResult.AppendLine(tabulations + "L[" + nbitems.ToString() + "]");

            // Main label -- Add it at L[0] in destination list
            String mainLabel = defects[0].m_sVIDLabel;
            secsItems.Add(new SecsItem(SecsFormat.Ascii, mainLabel)); // Add item L[0]  
            tabulations = tabulations + "\t\t";
            logResult.AppendLine(tabulations + mainLabel);

            // Defect list
            for (int i = 0; i < defects.Count; i++)
            {
                // Sub label --  Add it at L[1] in destination list
                String subLabel = defects[0].m_sSubVIDLabel;
                secsItems.Add(new SecsItem(SecsFormat.Ascii, subLabel)); // Add item L[1]   
                logResult.AppendLine(tabulations + subLabel);

                CVIDProcessDefect currentDefect = defects[i];

                // Count Sub list [5 items]  -- Add it at L[2] in destination list
                List<SecsItem> countItems = new List<SecsItem>();
                List<SecsItem> countBinItems = new List<SecsItem>();
                logResult.AppendLine(tabulations + "L[" + (currentDefect.m_lsCountDefefectBin.Count + 1).ToString() + "]");
                // Title Name  
                String name = "Count";
                countBinItems.Add(new SecsItem(SecsFormat.Ascii, name));
                logResult.AppendLine(tabulations + "\t" + name);
                // Add 4 bin items  
                for (int j = 0; j < currentDefect.m_lsCountDefefectBin.Count; j++)
                {
                    var c_binItems = GetBinValuesInAList(currentDefect.m_lsCountDefefectBin[j], j, SecsFormat.UInt4, ref logResult);
                    countBinItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(c_binItems)));
                }
                countItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(countBinItems)));
                secsItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(countItems)));

                // Size Sub list [5 items] -- Add it at L[3] in destination list
                List<SecsItem> sizeItems = new List<SecsItem>();
                List<SecsItem> sizeBinItems = new List<SecsItem>();
                logResult.AppendLine(tabulations + "L[" + (currentDefect.m_lsSizeDefectBin.Count + 1).ToString() + "]");
                // Title Name 
                name = "Size";
                sizeBinItems.Add(new SecsItem(SecsFormat.Ascii, name));
                logResult.AppendLine(tabulations + "\t" + name);
                //  Add 4 bin items 
                for (int j = 0; j < currentDefect.m_lsSizeDefectBin.Count; j++)
                {
                    var s_binItems = GetBinValuesInAList(currentDefect.m_lsSizeDefectBin[j], j, SecsFormat.Float8, ref logResult);
                    sizeBinItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(s_binItems)));
                }
                sizeItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(sizeBinItems)));
                secsItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(sizeItems)));
            }
            return secsItems;
        }
        private static List<SecsItem> GetBinValuesInAList(double binValue, int binIndex, SecsFormat pFormat, ref StringBuilder logResult)
        {
            List<SecsItem> binItems = new List<SecsItem>(); // 2 items in list L[2] = Title + data
            String sTabulation = "\t\t\t\t";
            
            logResult.AppendLine(sTabulation + "L[2]");
            String strName = "Bin" + Convert.ToString(binIndex + 1);
            String strValue = binValue.ToString();
            binItems.Add(new SecsItem(SecsFormat.Ascii, strName));
            logResult.AppendLine(sTabulation + "\t" + strName);
            switch (pFormat)
            {
                case SecsFormat.UInt1:
                    binItems.Add(new SecsItem(pFormat, Convert.ToByte(strValue)));
                    break;

                case SecsFormat.UInt2:
                    binItems.Add(new SecsItem(pFormat, Convert.ToUInt16(strValue)));
                    break;

                case SecsFormat.UInt4:
                    binItems.Add(new SecsItem(pFormat, Convert.ToUInt32(strValue)));
                    break;

                case SecsFormat.UInt8:
                    binItems.Add(new SecsItem(pFormat, Convert.ToUInt64(strValue)));
                    break;

                case SecsFormat.Float4:
                    binItems.Add(new SecsItem(pFormat, Convert.ToSingle(strValue)));
                    break;

                case SecsFormat.Float8:
                    binItems.Add(new SecsItem(pFormat, Convert.ToDouble(strValue, CultureInfo.InvariantCulture)));
                    break;

                default:
                    binItems.Add(new SecsItem(SecsFormat.Float8, Convert.ToDouble(strValue, CultureInfo.InvariantCulture)));
                    break;
            }
            logResult.AppendLine(sTabulation + "\t" + strValue);
            
            return binItems;
        }

    }
}
