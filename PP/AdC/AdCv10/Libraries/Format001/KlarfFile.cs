using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Format001
{
    static public class KlarfFile
    {
        public const string fmtDatetime = "MM-dd-yyyy HH:mm:ss";

        static public void Write(string sFullPathFile, DataKlarf oKlarfdata)
        {

            // we sort defect by ascending clusternum if CLUSTERNUMBER is present
            oKlarfdata.SortByClusterNum();

            using (StreamWriter wstream = new StreamWriter(sFullPathFile))
            {
                wstream.WriteLine("FileVersion {0};", oKlarfdata.FileVersion);
                wstream.WriteLine("FileTimeStamp {0};", oKlarfdata.FileTimeStamp.ToString(fmtDatetime, CultureInfo.InvariantCulture));
                wstream.WriteLine("InspectionStationID {0};", oKlarfdata.InspectionStationID);
                wstream.WriteLine("SampleType {0};", oKlarfdata.SampleType);
                wstream.WriteLine("ResultTimestamp {0};", oKlarfdata.ResultTimeStamp.ToString(fmtDatetime, CultureInfo.InvariantCulture));
                wstream.WriteLine("LotID \"{0}\";", oKlarfdata.LotID);
                wstream.WriteLine("SampleSize {0};", oKlarfdata.SampleSize ?? new PrmSampleSize(0));
                wstream.WriteLine("DeviceID \"{0}\";", oKlarfdata.DeviceID);
                wstream.WriteLine("SetupID {0};", oKlarfdata.SetupID ?? new PrmSetupId(String.Empty, DateTime.MinValue));
                wstream.WriteLine("StepID \"{0}\";", oKlarfdata.StepID);
                wstream.WriteLine("SampleOrientationMarkType {0};", oKlarfdata.SampleOrientationMarkType ?? new PrmSampleOrientationMarkType(0));
                wstream.WriteLine("OrientationMarkLocation {0};", oKlarfdata.OrientationMarkLocation ?? new PrmOrientationMarkValue(0));
                if (!String.IsNullOrEmpty(oKlarfdata.TiffFileName))
                    wstream.WriteLine("TiffFileName {0};", oKlarfdata.TiffFileName);
                wstream.WriteLine("DiePitch {0};", oKlarfdata.DiePitch);
                wstream.WriteLine("DieOrigin {0};", oKlarfdata.DieOrigin);
                wstream.WriteLine("WaferID \"{0}\";", oKlarfdata.WaferID);
                wstream.WriteLine("Slot {0};", oKlarfdata.SlotID);
                wstream.WriteLine("SampleCenterLocation {0};", oKlarfdata.SampleCenterLocation);
                wstream.WriteLine("OrientationInstructions \"{0}\";", oKlarfdata.OrientationInstructions ?? new PrmOrientationInstruction(0));
                wstream.WriteLine("CoordinatesMirrored {0};", oKlarfdata.CoordinatesMirrored ?? new PrmYesNo(false));
                wstream.WriteLine("CoordinatesCentered {0};", oKlarfdata.CoordinatesCentered ?? new PrmYesNo(false));
                wstream.WriteLine("InspectionOrientation {0};", oKlarfdata.InspectionOrientation ?? new PrmOrientationMarkValue(0));
                wstream.WriteLine("InspectionTest {0};", oKlarfdata.InspectionTest);
                if (oKlarfdata.SampleTestPlan != null)
                    oKlarfdata.SampleTestPlan.SortIndxList();
                wstream.WriteLine("SampleTestPlan {0};", oKlarfdata.SampleTestPlan ?? new PrmSampleTestPlan(0, 0));
                wstream.WriteLine("AreaPerTest {0};", oKlarfdata.AreaPerTest.ToString("E10"));
                wstream.WriteLine("ClusterClassificationList {0};", oKlarfdata.ClusterClassificationList);

                // write defect category label recorded 
                wstream.WriteLine("DefectRecordSpec {0};", oKlarfdata.DefectRecordSpec);

                // Write defect list data
                oKlarfdata.WriteDefectList(wstream);

                //Footer / End Of File
                oKlarfdata.WriteFooter(wstream);
            }
        }

        static public DataKlarf Read(string sFullPathFile, out string sErrMsg)
        {
            sErrMsg = String.Empty;

            if (!File.Exists(sFullPathFile))
            {
                sErrMsg = "This file path {" + sFullPathFile + "} doesn't exist !";
                return null;
            }

            bool IsSquaredWafer = false;
            float WaferSquareSize_X_mm = 0.0f;
            float WaferSquareSize_Y_mm = 0.0f;
            // on teste la précence d'un fichier .sqr synonyme de résultats pour un wafer square
            String strKlarfSquaredFilePathName = sFullPathFile.Replace(Path.GetExtension(sFullPathFile), ".sqr");
            if (File.Exists(strKlarfSquaredFilePathName))
            {
                // on lit le fichier sqr (format xml) pour lire la taille du wafer
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(strKlarfSquaredFilePathName);
                    XmlNode rootNode = doc.SelectSingleNode(".//root");
                    if (rootNode != null)
                    {
                        XmlNode SizeXNode = doc.SelectSingleNode(".//wafer_size_X_mm");
                        if (SizeXNode != null)
                        {
                            if (float.TryParse(SizeXNode.Attributes["value"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out WaferSquareSize_X_mm))
                            {
                                XmlNode SizeYNode = doc.SelectSingleNode(".//wafer_size_Y_mm");
                                if (SizeYNode != null)
                                {
                                    if (float.TryParse(SizeYNode.Attributes["value"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out WaferSquareSize_Y_mm))
                                        IsSquaredWafer = true;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    String sMsg = ex.Message;
                    WaferSquareSize_X_mm = 0.0f;
                    WaferSquareSize_Y_mm = 0.0f;
                    IsSquaredWafer = false;
                }
            }
            else
                IsSquaredWafer = false;


            DataKlarf oKlarfdata = new DataKlarf();
            oKlarfdata.isSquaredWafer = IsSquaredWafer;
            if (IsSquaredWafer)
            {
                oKlarfdata.SquareSizemm = new PrmPtFloat(WaferSquareSize_X_mm, WaferSquareSize_Y_mm);
            }

            String sFileBuffer = String.Empty;
            int pos = sFullPathFile.LastIndexOf('\\');
            String lName = sFullPathFile.Remove(0, pos + 1);
            using (StreamReader sr = new StreamReader(sFullPathFile))
            {
                sFileBuffer = sr.ReadToEnd();
            }

            ParseKlarfBuffer(sFileBuffer, ref oKlarfdata);

            return oKlarfdata;

        }

        static private void ParseKlarfBuffer(string p_sFileBuffer, ref DataKlarf p_oKlarfdata)
        {
            try
            {
                StringBuilder sbFooter = new StringBuilder();
                String[] sSections = p_sFileBuffer.Split(';');
                foreach (string sSection in sSections)
                {
                    string strLine = sSection;
                    String[] sTab = strLine.Split(' ');

                    if (strLine.ToUpper().Contains("FILEVERSION"))
                    {
                        // FileVersion
                        if (sTab.Length >= 3)
                            p_oKlarfdata.FileVersion = sTab[1] + " " + sTab[2];
                        else
                            p_oKlarfdata.FileVersion = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("FILETIMESTAMP"))
                    {
                        // FileTimeStamp
                        if (sTab.Length >= 3)
                        {
                            //should be format dd-MM-yyyy HH:mm:ss 
                            string sDate = sTab[1] + " " + sTab[2];
                            p_oKlarfdata.FileTimeStamp = DateTime.ParseExact(sDate, fmtDatetime, CultureInfo.InvariantCulture);
                        }
                        else if (sTab.Length == 2)
                        {
                            //should be old format ddMMyyyy
                            p_oKlarfdata.FileTimeStamp = DateTime.ParseExact(sTab[1], "ddMMyyyy", CultureInfo.InvariantCulture);
                        }
                        else
                            p_oKlarfdata.FileTimeStamp = DateTime.MinValue;
                        continue;
                    }
                    if (strLine.ToUpper().Contains("TIFFFILENAME"))
                    {
                        // TiffFileName
                        if (sTab.Length >= 2)
                            p_oKlarfdata.TiffFileName = sTab[1];
                        else
                            p_oKlarfdata.TiffFileName = String.Empty;
                        continue;
                    }
                    if (strLine.ToUpper().Contains("INSPECTIONSTATIONID"))
                    {
                        // InspectionStationID
                        if (sTab.Length >= 4)
                        {
                            //p_oKlarfdata.InspectionStationID = sTab[1].Replace('\"', ' ').Trim() + "-" + sTab[2].Replace('\"', ' ').Trim() + "-" + sTab[3].Replace('\"', ' ').Trim();
                            p_oKlarfdata.InspectionStationID = sTab[1] + " " + sTab[2] + " " + sTab[3];
                        }
                        else
                            p_oKlarfdata.InspectionStationID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLETYPE"))
                    {
                        // SampleType
                        if (sTab.Length >= 2)
                            p_oKlarfdata.SampleType = sTab[1];
                        else
                            p_oKlarfdata.SampleType = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("RESULTTIMESTAMP"))
                    {
                        // ResultTimeStamp
                        if (sTab.Length >= 3)
                        {
                            //should be format dd-MM-yyyy HH:mm:ss 
                            string sDate = sTab[1] + " " + sTab[2];
                            p_oKlarfdata.ResultTimeStamp = DateTime.ParseExact(sDate, fmtDatetime, CultureInfo.InvariantCulture);
                        }
                        else if (sTab.Length == 2)
                        {
                            //should be old format ddMMyyyy
                            p_oKlarfdata.ResultTimeStamp = DateTime.ParseExact(sTab[1], "ddMMyyyy", CultureInfo.InvariantCulture);
                        }
                        else
                            p_oKlarfdata.ResultTimeStamp = DateTime.MinValue;
                        continue;
                    }
                    if (strLine.ToUpper().Contains("LOTID"))
                    {
                        // LotID
                        if (sTab.Length >= 2)
                            p_oKlarfdata.LotID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            p_oKlarfdata.LotID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLESIZE"))
                    {
                        // SampleSize
                        if (sTab.Length >= 3)
                        {
                            int NbSample;
                            if (!int.TryParse(sTab[1], out NbSample))
                                NbSample = 0;
                            int nWaferDiameterSize_mm;
                            if (!int.TryParse(sTab[2], out nWaferDiameterSize_mm))
                                nWaferDiameterSize_mm = 0;

                            p_oKlarfdata.SampleSize = new PrmSampleSize(nWaferDiameterSize_mm, NbSample);
                            continue; ;
                        }
                        else
                            p_oKlarfdata.SampleSize = new PrmSampleSize(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("DEVICEID"))
                    {
                        // DeviceID
                        if (sTab.Length >= 2)
                            p_oKlarfdata.DeviceID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            p_oKlarfdata.DeviceID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SETUPID"))
                    {
                        // SetupID
                        if (sTab.Length >= 2)
                        {
                            DateTime? dt = null;
                            if (sTab.Length >= 3)
                            {
                                //should be format dd-MM-yyyy HH:mm:ss 
                                string sDate = sTab[2] + " " + sTab[3];
                                dt = DateTime.ParseExact(sDate, fmtDatetime, CultureInfo.InvariantCulture);
                            }
                            else if (sTab.Length == 2)
                            {
                                //should be old format ddMMyyyy
                                dt = DateTime.ParseExact(sTab[1], "ddMMyyyy", CultureInfo.InvariantCulture);
                            }

                            string sId = sTab[1].Replace('\"', ' ').Trim();
                            p_oKlarfdata.SetupID = new PrmSetupId(sId, dt);
                        }
                        else
                            p_oKlarfdata.SetupID = new PrmSetupId("Unknown", null);

                        continue;
                    }
                    if (strLine.ToUpper().Contains("STEPID"))
                    {
                        // StepID
                        if (sTab.Length >= 2)
                            p_oKlarfdata.StepID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            p_oKlarfdata.StepID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLEORIENTATIONMARKTYPE"))
                    {
                        // SampleOrientationMarkType
                        if (sTab.Length >= 2)
                        {
                            int nval;
                            if (int.TryParse(sTab[1], out nval))
                            {
                                p_oKlarfdata.SampleOrientationMarkType = new PrmSampleOrientationMarkType(nval); ;
                            }
                            else
                            {
                                p_oKlarfdata.SampleOrientationMarkType = new PrmSampleOrientationMarkType(sTab[1]); ;
                            }
                        }
                        else
                            p_oKlarfdata.SampleOrientationMarkType = new PrmSampleOrientationMarkType(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("ORIENTATIONMARKLOCATION"))
                    {
                        // OrientationMarkLocation
                        if (sTab.Length >= 2)
                        {
                            int nval;
                            if (int.TryParse(sTab[1], out nval))
                            {
                                p_oKlarfdata.OrientationMarkLocation = new PrmOrientationMarkValue(nval); ;
                            }
                            else
                            {
                                p_oKlarfdata.OrientationMarkLocation = new PrmOrientationMarkValue(sTab[1]); ;
                            }
                        }
                        else
                            p_oKlarfdata.OrientationMarkLocation = new PrmOrientationMarkValue(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("WAFERID"))
                    {   // Le WaferId peut contenor des espaces.
                        // On ne peut donc pas utiliser l'espace comme séparateur

                        sTab = strLine.Split('\\');

                        // WaferID
                        if (sTab.Length == 1)
                        {
                            string tmp = sTab[0].Replace('\"', ' ').Trim();
                            p_oKlarfdata.WaferID = tmp.Remove(0, tmp.IndexOf(' ')).TrimStart(' ');
                        }
                        else
                            p_oKlarfdata.WaferID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SLOT"))
                    {
                        // SlotID
                        if (sTab.Length >= 2)
                        {
                            int nval;
                            if (!int.TryParse(sTab[1], out nval))
                                nval = 0;
                            p_oKlarfdata.SlotID = nval;
                        }
                        else
                            p_oKlarfdata.SlotID = 0;
                        continue;
                    }
                    if (strLine.ToUpper().Contains("ORIENTATIONINSTRUCTION"))
                    {
                        // OrientationInstruction
                        if (sTab.Length >= 2)
                        {
                            p_oKlarfdata.OrientationInstructions = new PrmOrientationInstruction(sTab[1].Replace('\"', ' ').Trim());
                        }
                        else
                            p_oKlarfdata.OrientationInstructions = new PrmOrientationInstruction(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("COORDINATESMIRRORED"))
                    {
                        // CoordinatesMirrored
                        if (sTab.Length >= 2)
                        {
                            int nval;
                            if (int.TryParse(sTab[1], out nval))
                            {
                                p_oKlarfdata.CoordinatesMirrored = new PrmYesNo(nval != 0); ;
                            }
                            else
                            {
                                p_oKlarfdata.CoordinatesMirrored = new PrmYesNo(sTab[1].ToUpper() == "YES");
                            }
                        }
                        continue;
                    }
                    if (strLine.ToUpper().Contains("COORDINATESCENTERED"))
                    {
                        // CoordinatesCentered
                        if (sTab.Length >= 2)
                        {
                            int nval;
                            if (int.TryParse(sTab[1], out nval))
                            {
                                p_oKlarfdata.CoordinatesCentered = new PrmYesNo(nval != 0); ;
                            }
                            else
                            {
                                p_oKlarfdata.CoordinatesCentered = new PrmYesNo(sTab[1].ToUpper() == "YES");
                            }
                        }
                        continue;
                    }
                    if (strLine.ToUpper().Contains("INSPECTIONORIENTATION"))
                    {
                        // InspectionOrientation
                        if (sTab.Length >= 2)
                        {
                            int nval;
                            if (int.TryParse(sTab[1], out nval))
                            {
                                p_oKlarfdata.InspectionOrientation = new PrmOrientationMarkValue(nval); ;
                            }
                            else
                            {
                                p_oKlarfdata.InspectionOrientation = new PrmOrientationMarkValue(sTab[1]); ;
                            }
                        }
                        //else p_oKlarfdata.InspectionOrientation = new PrmOrientationMarkValue(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLECENTERLOCATION"))
                    {
                        // SampleCenterLocation
                        if (sTab.Length >= 3)
                            p_oKlarfdata.SampleCenterLocation = new PrmPtFloat((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat),
                                                                    (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                        else
                            p_oKlarfdata.SampleCenterLocation = new PrmPtFloat(float.NaN, float.NaN);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLETESTPLAN"))
                    {
                        // SampleTestPlan
                        String[] lStringSeparator = new String[] { "\n" };
                        strLine = strLine.Trim().Replace("\n\r", "");
                        strLine = strLine.Trim().Replace("\r", " ");
                        sTab = strLine.Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (sTab.Length >= 2)
                        {
                            int lNbDies = 0;
                            for (int k = 0; k < sTab.Length; k++)
                            {
                                String[] sTabDiesIndexes = sTab[k].Trim().Split(' ');
                                if (sTabDiesIndexes.Length % 2 != 0)
                                    throw new Exception("SampleTestPlan BAD format - ODD number of parameters");
                                int nConsumed = 0;
                                if (sTabDiesIndexes[0].Contains("SampleTestPlan"))
                                {
                                    lNbDies = Convert.ToInt32(sTabDiesIndexes[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                                    p_oKlarfdata.SampleTestPlan.ClearPlan();
                                    // ON DOIT GERER LE CAS pas de retour ligne "SampleTestPlan 1 0 0;" ou "SampleTestPlan n 0 0 1 1 2 2"
                                    nConsumed = 2;
                                    while ((sTabDiesIndexes.Length - nConsumed) > 0)
                                    {
                                        p_oKlarfdata.SampleTestPlan.Add(Convert.ToInt32(sTabDiesIndexes[nConsumed + 0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                                                        Convert.ToInt32(sTabDiesIndexes[nConsumed + 1].Trim(), CultureInfo.InvariantCulture.NumberFormat));

                                        nConsumed += 2;
                                    }
                                    continue;
                                }

                                while ((sTabDiesIndexes.Length - nConsumed) > 0)
                                {
                                    p_oKlarfdata.SampleTestPlan.Add(Convert.ToInt32(sTabDiesIndexes[nConsumed + 0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                                                    Convert.ToInt32(sTabDiesIndexes[nConsumed + 1].Trim(), CultureInfo.InvariantCulture.NumberFormat));

                                    nConsumed += 2;
                                }
                            }
                        }
                        continue;
                    }
                    if (strLine.ToUpper().Contains("DIEPITCH"))
                    {
                        // DiePitch
                        if (sTab.Length >= 3)
                            p_oKlarfdata.DiePitch = new PrmPtFloat((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat),
                                                                   (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                        else
                            p_oKlarfdata.DiePitch = new PrmPtFloat(float.NaN, float.NaN);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("DIEORIGIN"))
                    {
                        // DieOrigin
                        if (sTab.Length >= 3)
                            p_oKlarfdata.DieOrigin = new PrmPtFloat((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat),
                                                                    (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                        else
                            p_oKlarfdata.DieOrigin = new PrmPtFloat(float.NaN, float.NaN);
                        continue;
                    }

                    if (strLine.ToUpper().Contains("DEFECTRECORDSPEC"))
                    {
                        // DefectRecordSpec
                        String[] lStringSeparator = new String[] { "DEFECTID" };
                        sTab = strLine.Trim().Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        lStringSeparator = new String[] { " " };
                        String[] sTabHeader = sTab[0].Trim().Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (sTabHeader.Length >= 2)
                        {
                            int lNbColumns = Convert.ToInt32(sTabHeader[1], CultureInfo.InvariantCulture.NumberFormat);
                            String[] strNameTab = sTab[1].Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                            if (strNameTab.Length + 1 == lNbColumns)
                            {
                                p_oKlarfdata.AddDefectType("DEFECTID");
                                p_oKlarfdata.AddDefectTypes(strNameTab);
                            }
                        }
                        continue;
                    }

                    if (strLine.ToUpper().Contains("DEFECTLIST"))
                    {
                        String[] lSeparator = new String[] { "\r\n" };
                        String[] lDefData = strLine.Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
                        for (int nidlin = 1; nidlin < lDefData.Length; nidlin++) // l'index 0 should contained only DEFECTLIST tag
                        {
                            PrmDefect def = p_oKlarfdata.NewDefect();
                            def.SetFromString(lDefData[nidlin]);
                            p_oKlarfdata.AddDefect(def);
                        }
                        continue;
                    }

                    if (strLine.ToUpper().Contains("SUMMARYSPEC"))
                    {
                        sbFooter.AppendLine(strLine);
                        continue; ;
                    }

                    if (strLine.ToUpper().Contains("SUMMARYLIST"))
                    {
                        sbFooter.AppendLine(strLine);

                        // seul NDIE et NDEFDIE sont des données spécifique et non récupérable à prtir des données précédentes
                        String[] lSeparator = new String[] { "\r\n" };
                        String[] SumSplit = strLine.Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
                        String[] lSeparatorSpace = new String[] { " " };
                        String[] ldatafooter = SumSplit[1].Split(lSeparatorSpace, StringSplitOptions.RemoveEmptyEntries);
                        p_oKlarfdata.NDIE = Convert.ToInt32(ldatafooter[3], CultureInfo.InvariantCulture.NumberFormat);
                        p_oKlarfdata.NDEFDIE = Convert.ToInt32(ldatafooter[4], CultureInfo.InvariantCulture.NumberFormat);
                        continue; ;
                    }

                    if (strLine.ToUpper().Contains("ENDOFFILE"))
                    {
                        sbFooter.AppendLine(strLine);
                        break; ;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// fichier de fin de lot pour chargeur automation
        /// </summary>
        public static void WriteLotStatusFile(string lotStatusFileName, DataKlarf oKlarfdata, string outputdir)
        {
            using (StreamWriter wstream = new StreamWriter(lotStatusFileName))
            {
                wstream.WriteLine("FileVersion {0};", oKlarfdata.FileVersion);
                wstream.WriteLine("FileTimeStamp {0};", oKlarfdata.FileTimeStamp.ToString(fmtDatetime, CultureInfo.InvariantCulture));
                wstream.WriteLine("StepID \"{0}\";", oKlarfdata.StepID);
                wstream.WriteLine("InspectionStationID {0};", oKlarfdata.InspectionStationID);
                wstream.WriteLine("OrientationInstructions \"{0}\";", oKlarfdata.OrientationInstructions);
                //wstream.WriteLine("SampleType {0};", oKlarfdata.SampleType);
                wstream.WriteLine("ResultTimestamp {0};", oKlarfdata.ResultTimeStamp.ToString(fmtDatetime, CultureInfo.InvariantCulture));
                wstream.WriteLine("LotID \"{0}\";", oKlarfdata.LotID);
                //wstream.WriteLine("SampleSize {0};", oKlarfdata.SampleSize ?? new PrmSampleSize(0));
                //wstream.WriteLine("DeviceID \"{0}\";", oKlarfdata.DeviceID);
                //wstream.WriteLine("SetupID {0};", oKlarfdata.SetupID ?? new PrmSetupId(String.Empty, DateTime.MinValue));
                wstream.WriteLine("EndOfLotInspection;");
                wstream.WriteLine("EndOfFile;");
                //m_sReportTxt = "OrientationInstructions \"" + oKlarfdata.v pWafer.valueHandleRecipeAccess.getOrientationInstruction + "\";";
            }
        }
    }
}
