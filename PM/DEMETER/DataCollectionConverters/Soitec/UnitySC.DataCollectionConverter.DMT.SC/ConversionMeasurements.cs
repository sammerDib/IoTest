using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnitySC.ADCAS300Like.Common.Protocol_Robot_ADC;
using UnitySC.Shared.Data.SecsGem;

namespace UnitySC.Shared.DMT.DataCollection
{
    #region ADC measurement result example
    /*
    ADC measurement result example:    
    
    List format:
    List of each VID data
    VID= 990060	
                L[2]
			            Wafer_Bow
			            L[5]
				                TWARP
				                1184.91918945313
				                um
				                0
				                0
    double format:
    VID= 990069	
                1184.91918945313
    */
    #endregion
    public class ConversionMeasurements
    {
        public static String NoVIDProcessMeasurement_Msg = "VID process measurement list is empty";
        public static List<SecsVariable> GetConvertSVFromMeasurements(List<CVIDProcessMeasurement> measurements, ref StringBuilder logResult)
        {
            var secsVariables = new List<SecsVariable>();
            if (measurements != null)
            {
                for (int i = 0; i < measurements.Count; i++)
                {
                    switch (measurements[i].m_enMeasurementType)
                    {
                        case enTypeMeasurement.en_EdgeMeasurement:
                        case enTypeMeasurement.en_BowWarpMeasurement:
                        case enTypeMeasurement.en_PolishedMeasurement:
                            SecsItem secsItem_List = GetConvertSecsItemFrom_ListFormatedMeasurement(measurements, measurements[i], ref logResult);
                            secsVariables.Add(new SecsVariable("PW_" + measurements[i].m_sVIDLabel, secsItem_List));
                            break;
                        case enTypeMeasurement.en_2DMetro:
                        case enTypeMeasurement.en_3DMetro:
                        case enTypeMeasurement.en_Haze:
                            SecsItem secsItem_Double = GetConvertSecsItemFrom_DoubleFormatedMeasurement(measurements[i], ref logResult);                                                   
                            secsVariables.Add(new SecsVariable("PW_" + measurements[i].m_sVIDLabel, secsItem_Double));
                            break;
                        default:
                            break;
                    }
                }
            }
            else
                throw new Exception("No measurements in report");
           
            return secsVariables;         
        }

        private static SecsItem GetConvertSecsItemFrom_DoubleFormatedMeasurement(CVIDProcessMeasurement measurement, ref StringBuilder logResult)
        {            
            String tabulations = "\t";
            int lVID = measurement.m_iVIDValue;
            logResult.AppendLine("VID= " + lVID.ToString());
            logResult.AppendLine(tabulations + measurement.m_lfMeasurementValue.ToString());           
            return new SecsItem(SecsFormat.Float8, measurement.m_lfMeasurementValue); // // return SecsItem in double format
        }
        private static SecsItem GetConvertSecsItemFrom_ListFormatedMeasurement(List<CVIDProcessMeasurement> measurements, CVIDProcessMeasurement measurement, ref StringBuilder logResult)
        {
            List<SecsItem> secsItems = new List<SecsItem>(); // List of each VID data as example VID= 990069	
            // Value object
            List<int> verifiedIds = new List<int>();
            int idToCheck = measurement.m_iVIDValue;
            List<CVIDProcessMeasurement> sameVIDProcessMeasurements = new List<CVIDProcessMeasurement>();
            sameVIDProcessMeasurements.Clear();
            for (int j = 0; j < measurements.Count; j++)
            {
                // VID Num
                int idCurrent = measurements[j].m_iVIDValue;
                int iPos = verifiedIds.IndexOf(j);
                if ((idToCheck == idCurrent) && (iPos < 0))
                {
                    verifiedIds.Add(j);
                    sameVIDProcessMeasurements.Add(measurements[j]);
                }
            }
            if (sameVIDProcessMeasurements.Count > 0)
            {
                logResult.AppendLine("VID= " + idToCheck.ToString());
                secsItems = GetMeasurementDataInAList(sameVIDProcessMeasurements, ref logResult);
            }
            return new SecsItem(SecsFormat.List, new SecsItemList(secsItems)); // return SecsItem in list format
        }
        public static List<SecsItem> GetMeasurementDataInAList(List<CVIDProcessMeasurement> measurements, ref StringBuilder logResult)
        {
            List<SecsItem> secsItems = new List<SecsItem>(); // List of each VID data as example VID= 990060
            String sTabulation = "\t";
            logResult.AppendLine(sTabulation + "L[" + (measurements.Count + 1).ToString() + "]");
            // Main label data
            String name = measurements[0].m_sVIDLabel;
            secsItems.Add(new SecsItem(SecsFormat.Ascii, name));
            sTabulation = sTabulation + "\t\t";
            logResult.AppendLine(sTabulation + "\t" + name);
            for (int i = 0; i < measurements.Count; i++) //==>  Normalement 1 seule mesure desormais
            {
                // Measure Type = Format data 
                var valueToAdd = measurements[0].m_enMeasurementType.ToString().Remove(0, 3);
                secsItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                logResult.AppendLine(sTabulation + "\t" + valueToAdd);

                // Add Item List with values below
                List<SecsItem> subItems = new List<SecsItem>();
                logResult.AppendLine(sTabulation + "\tL[5]");

                // Sub label data
                valueToAdd = measurements[i].m_sSubVIDLabel.ToString();
                subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                logResult.AppendLine(sTabulation + "\t\t" + valueToAdd);

                // Measurement value
                var valueF8 = measurements[i].m_lfMeasurementValue;
                subItems.Add(new SecsItem(SecsFormat.Ascii, valueF8.ToString()));
                logResult.AppendLine(sTabulation + "\t\t" + valueF8.ToString());

                // Unit
                valueToAdd = measurements[i].m_sUnitValue.ToString();
                subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                logResult.AppendLine(sTabulation + "\t\t" + valueToAdd);
                try
                {
                    enTypeMeasurement lTryTypeExist = measurements[i].m_enMeasurementType;
                    switch (lTryTypeExist)
                    {
                        case enTypeMeasurement.en_BowWarpMeasurement:
                        case enTypeMeasurement.en_PolishedMeasurement:
                            // Radius
                            valueToAdd = measurements[i].m_RadiusValue.ToString();
                            subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                            logResult.AppendLine(sTabulation + "\t\t" + valueToAdd);

                            // Measure nbr
                            valueToAdd = measurements[i].m_MeasurementNumber.ToString();
                            subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                            logResult.AppendLine(sTabulation + "\t\t" + valueToAdd);
                            break;
                        case enTypeMeasurement.en_2DMetro:
                        case enTypeMeasurement.en_3DMetro:
                        case enTypeMeasurement.en_Haze:
                            // Type not used in format List - do nothing in case
                            break;
                        case enTypeMeasurement.en_EdgeMeasurement:
                        default:
                            // X Y Coordinates
                            valueToAdd = measurements[i].m_iXcoordinate.ToString() + "," + measurements[i].m_iYcoordinate.ToString();
                            subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                            logResult.AppendLine(sTabulation + "\t\t" + valueToAdd);

                            // Description
                            valueToAdd = measurements[i].m_iSubVIDValue.ToString();
                            subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                            logResult.AppendLine(sTabulation + "\t\t" + valueToAdd);
                            break;
                    }
                    secsItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(subItems)));
                }
                catch
                {
                    // Type Edge par defaut si Measurement Type n'existe pas dans l'objet
                    // X Y Coordinates
                    valueToAdd = measurements[i].m_iXcoordinate.ToString() + "," + measurements[i].m_iYcoordinate.ToString();
                    subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                    logResult.AppendLine(sTabulation + "\t\t" + name);

                    // Description
                    valueToAdd = measurements[i].m_iSubVIDValue.ToString();
                    subItems.Add(new SecsItem(SecsFormat.Ascii, valueToAdd));
                    logResult.AppendLine(sTabulation + "\t\t" + name);
                    secsItems.Add(new SecsItem(SecsFormat.List, new SecsItemList(subItems)));
                }
            }
            return secsItems; // return the list of each VID data as example VID= 990060
        }
    }
}
