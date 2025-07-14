using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Shared.DMT.DataCollection
{
    #region APC results example
    /*
         VID= 990097	L[2]
			                APCPSDBS
			                L[3]
				                L[3]
					                 ModuleID
					                1
					                L[13]
						                L[2]
								                TotalDefect
								                1
						                L[2]
								                HazeValue
								                0
						                L[2]
								                MinValue
								                0
						                L[2]
								                MaxValue
								                193
						                L[2]
								                AverageValue
								                56.1393499664749
						                L[2]
								                StdDevValue
								                56.9199424637161
						                L[2]
								                IntegrationTime
								                34
						                L[2]
								                ROI_X
								                416
						                L[2]
								                ROI_Y
								                1280
						                L[2]
								                ROI_Width
								                2050
						                L[2]
								                ROI_Height
								                100
						                L[2]
								                AutoExposure_Status
								                0
						                L[2]
								                ModuleID
								                1
				                L[3]
					                 ModuleID
					                3
					                L[13]
						                L[2]
								                TotalDefect
								                1
						                L[2]
								                HazeValue
								                0
						                L[2]
								                MinValue
								                0
						                L[2]
								                MaxValue
								                236
						                L[2]
								                AverageValue
								                150.260413806085
						                L[2]
								                StdDevValue
								                89.7177065767774
						                L[2]
								                IntegrationTime
								                31
						                L[2]
								                ROI_X
								                500
						                L[2]
								                ROI_Y
								                650
						                L[2]
								                ROI_Width
								                2200
						                L[2]
								                ROI_Height
								                1600
						                L[2]
								                AutoExposure_Status
								                0
						                L[2]
								                ModuleID
								                3
				                L[3]
					                 ModuleID
					                13
					                L[13]
						                L[2]
								                TotalDefect
								                1
						                L[2]
								                HazeValue
								                0
						                L[2]
								                MinValue
								                0
						                L[2]
								                MaxValue
								                255
						                L[2]
								                AverageValue
								                107.570493954387
						                L[2]
								                StdDevValue
								                52.3261789636708
						                L[2]
								                IntegrationTime
								                1774
						                L[2]
								                ROI_X
								                500
						                L[2]
								                ROI_Y
								                650
						                L[2]
								                ROI_Width
								                2200
						                L[2]
								                ROI_Height
								                1600
						                L[2]
								                AutoExposure_Status
								                0
						                L[2]
								                ModuleID
								                13
    */
    #endregion
    public static class ConversionAPCs
    {

        public static String NoVIDProcessAPC_Msg = "VID process apc list is empty";
        internal static List<SecsVariable> GetConvertSVFromAPCs(List<CVIDProcessAPC> apcs, ref StringBuilder logResult)
        {
            // APC data             
            var secsVariables = new List<SecsVariable>();
            if (apcs != null)
            {
                List<int> verifiedIds = new List<int>();
                // Regroup vids data by id from apcs (Sometimes ADC generate CVIDProcessAPC data with the same VID but separatly in the list :/)
                for (int i = 0; i < apcs.Count; i++)
                {
                    int idToCheck = apcs[i].m_iVIDValue;
                    List<CVIDProcessAPC> sameVIDProcessApcs = new List<CVIDProcessAPC>();
                    sameVIDProcessApcs.Clear();
                    for (int j = 0; j < apcs.Count; j++)
                    {
                        // VID Num
                        int idSelected = apcs[j].m_iVIDValue;
                        int pos = verifiedIds.IndexOf(j);
                        if ((idToCheck == idSelected) && (pos < 0))
                        {
                            verifiedIds.Add(j);
                            sameVIDProcessApcs.Add(apcs[j]);
                        }
                    }
                    if (sameVIDProcessApcs.Count > 0)
                    {
                        logResult.AppendLine("VID= " + idToCheck.ToString());
                        var sv = GetAPCDataInAList(sameVIDProcessApcs, ref logResult);
                        var secsItem = new SecsItem(SecsFormat.List, new SecsItemList(sv));
                        secsVariables.Add(new SecsVariable("PW_" + sameVIDProcessApcs[0].m_sVIDLabel, secsItem));
                    }
                }
            }
            else
                throw new Exception(NoVIDProcessAPC_Msg);

            return secsVariables;
        }

        public static List<SecsItem> GetAPCDataInAList(List<CVIDProcessAPC> apcs, ref StringBuilder logResult)
        {
            // apcs ne contient qu'un element...Normalement
            var secsItems = new List<SecsItem>(); // List of each VID data as example VID= 990097 
            String tabulations = "\t";
            // APC data list
            for (int i = 0; i < apcs.Count; i++)
            {
                String tabulations_2 = "\t";

                // Insertion List 0
                logResult.AppendLine(tabulations + "L[2]");

                // Main label -- Add it at L[0] in result list
                String mainLabel = apcs[i].m_sVIDLabel;
                secsItems.Add(new SecsItem(SecsFormat.Ascii, mainLabel)); // Add item L[0] 
                logResult.AppendLine(tabulations + "\t\t" + mainLabel);

                CVIDProcessAPC lCurrentAPCData = apcs[i]; // APCData = Data of Module List

                // Modules list [3 items]  -- Add it at L[1] in result list
                List<SecsItem> modules = new List<SecsItem>();
                logResult.AppendLine(tabulations + "\t\t" + "L[" + lCurrentAPCData.m_DataAPCModule.Count.ToString() + "]");
                tabulations_2 = tabulations_2 + tabulations + "\t\t";

                // APC Module list
                CVIDProcessAPCModule lAPCModule;
                for (int j = 0; j < lCurrentAPCData.m_DataAPCModule.Count; j++)
                {
                    String tabulations_3 = "\t";
                    List<SecsItem> modulesData = new List<SecsItem>();
                    logResult.AppendLine(tabulations_2 + "L[3]");

                    // ModuleID title
                    lAPCModule = lCurrentAPCData.m_DataAPCModule[j];
                    modulesData.Add(new SecsItem(SecsFormat.Ascii, "ModuleID"));
                    logResult.AppendLine(tabulations_2 + "\t ModuleID");

                    // ModuleID value
                    double moduleIDvalue = Convert.ToDouble(lAPCModule.m_ModuleID);
                    modulesData.Add(new SecsItem(SecsFormat.Float8, moduleIDvalue));
                    logResult.AppendLine(tabulations_2 + "\t" + moduleIDvalue.ToString());

                    List<SecsItem> moduleDataItems = new List<SecsItem>();
                    logResult.AppendLine(tabulations_2 + "\tL[" + lAPCModule.m_lsLabel.Count.ToString() + "]");

                    tabulations_3 = tabulations_3 + tabulations_2 + "\t";
                    for (int k = 0; k < lAPCModule.m_lsLabel.Count; k++)
                    {
                        List<SecsItem> dataItems = new List<SecsItem>(); // List of 2 items = title + value
                        logResult.AppendLine(tabulations_3 + "L[2]");

                        // Title
                        String title = lAPCModule.m_lsLabel[k];
                        dataItems.Add(new SecsItem(SecsFormat.Ascii, title));
                        logResult.AppendLine(tabulations_3 + "\t\t" + title);
                        // Value
                        double valueDouble = Convert.ToDouble(lAPCModule.m_lsValue[k]);
                        dataItems.Add(new SecsItem(SecsFormat.Float8, valueDouble));
                        logResult.AppendLine(tabulations_3 + "\t\t" + valueDouble.ToString());

                        moduleDataItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(dataItems))); // Add L[2] for each value of module
                    }
                    modulesData.Add(new SecsItem(SecsFormat.List, new SecsItemList(moduleDataItems)));
                    modules.Add(new SecsItem(SecsFormat.List, new SecsItemList(modulesData)));
                }
                secsItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(modules)));
            }
            return secsItems;
        }
    }
}
