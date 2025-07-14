using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace UnitySC.Shared.Format._001
{
    static public class KlarfFile
    {
        // ADCv9 date format
        public const string FmtDatetime = "MM-dd-yyyy HH:mm:ss";

        static public void Write(string fullPathFile, DataKlarf oKlarfdata)
        {
            // we sort defect by ascending clusternum if CLUSTERNUMBER is present
            oKlarfdata.SortByClusterNum();

            using (var wstream = new StreamWriter(fullPathFile))
            {
                wstream.WriteLine("FileVersion {0};", oKlarfdata.FileVersion);
                wstream.WriteLine("FileTimeStamp {0};", oKlarfdata.FileTimeStamp.ToString(FmtDatetime, CultureInfo.InvariantCulture));
                wstream.WriteLine("InspectionStationID {0};", oKlarfdata.InspectionStationID);
                wstream.WriteLine("SampleType {0};", oKlarfdata.SampleType);
                wstream.WriteLine("ResultTimestamp {0};", oKlarfdata.ResultTimeStamp.ToString(FmtDatetime, CultureInfo.InvariantCulture));
                wstream.WriteLine("LotID \"{0}\";", oKlarfdata.LotID);
                wstream.WriteLine("SampleSize {0};", oKlarfdata.SampleSize ?? new PrmSampleSize(0));
                wstream.WriteLine("DeviceID \"{0}\";", oKlarfdata.DeviceID);
                wstream.WriteLine("SetupID {0};", oKlarfdata.SetupID ?? new PrmSetupId(string.Empty, DateTime.MinValue));
                wstream.WriteLine("StepID \"{0}\";", oKlarfdata.StepID);
                wstream.WriteLine("SampleOrientationMarkType {0};", oKlarfdata.SampleOrientationMarkType ?? new PrmSampleOrientationMarkType(0));
                wstream.WriteLine("OrientationMarkLocation {0};", oKlarfdata.OrientationMarkLocation ?? new PrmOrientationMarkValue(0));
                if (!string.IsNullOrEmpty(oKlarfdata.TiffFileName))
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

        static public bool Read(string fullPathFile, DataKlarf oKlarfdata, out string errMsg)
        {
            errMsg = string.Empty;
            oKlarfdata.ResFilePath = string.Empty;
            if (!File.Exists(fullPathFile))
            {
                errMsg = "This file path {" + fullPathFile + "} doesn't exist !";
                return false;
            }
            oKlarfdata.ResFilePath = fullPathFile;

            bool IsSquaredWafer = false;
            float WaferSquareSize_X_mm = 0.0f;
            float WaferSquareSize_Y_mm = 0.0f;
            // on teste la précence d'un fichier .sqr synonyme de résultats pour un wafer square
            string strKlarfSquaredFilePathName = fullPathFile.Replace(".001", ".sqr");
            if (File.Exists(strKlarfSquaredFilePathName))
            {
                // on lit le fichier sqr (format xml) pour lire la taille du wafer
                try
                {
                    var doc = new XmlDocument();
                    doc.Load(strKlarfSquaredFilePathName);
                    var rootNode = doc.SelectSingleNode(".//root");
                    if (rootNode != null)
                    {
                        var SizeXNode = doc.SelectSingleNode(".//wafer_size_X_mm");
                        if (SizeXNode != null)
                        {
                            if (float.TryParse(SizeXNode.Attributes["value"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out WaferSquareSize_X_mm))
                            {
                                var SizeYNode = doc.SelectSingleNode(".//wafer_size_Y_mm");
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
                    string sMsg = ex.Message;
                    WaferSquareSize_X_mm = 0.0f;
                    WaferSquareSize_Y_mm = 0.0f;
                    IsSquaredWafer = false;
                }
            }
            else
                IsSquaredWafer = false;

            oKlarfdata.IsSquaredWafer = IsSquaredWafer;
            if (IsSquaredWafer)
            {
                oKlarfdata.SquareSizemm = new PrmPtFloat(WaferSquareSize_X_mm, WaferSquareSize_Y_mm);
            }

            string sFileBuffer = string.Empty;
            int pos = fullPathFile.LastIndexOf('\\');
            string lName = fullPathFile.Remove(0, pos + 1);
            using (var sr = new StreamReader(fullPathFile))
            {
                sFileBuffer = sr.ReadToEnd();
            }

            ParseKlarfBuffer(sFileBuffer, ref oKlarfdata);

            return true;
        }

        static private void ParseKlarfBuffer(string fileContentBuffer, ref DataKlarf oKlarfdata)
        {
            try
            {
                var sbFooter = new StringBuilder();
                string[] sSections = fileContentBuffer.Split(';');
                foreach (string sSection in sSections)
                {
                    string strLine = sSection;
                    string[] sTab = strLine.Split(' ');

                    if (strLine.ToUpper().Contains("FILEVERSION"))
                    {
                        // FileVersion
                        if (sTab.Length >= 3)
                            oKlarfdata.FileVersion = sTab[1] + " " + sTab[2];
                        else
                            oKlarfdata.FileVersion = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("FILETIMESTAMP"))
                    {
                        try
                        {
                            // FileTimeStamp
                            if (sTab.Length >= 3)
                            {
                                //should be format dd-MM-yyyy HH:mm:ss
                                string sDate = sTab[1] + " " + sTab[2];
                                try
                                {
                                    oKlarfdata.FileTimeStamp = DateTime.ParseExact(sDate, FmtDatetime, CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    oKlarfdata.ResultTimeStamp = DateTime.ParseExact(sDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (sTab.Length == 2)
                            {
                                //should be old format ddMMyyyy
                                oKlarfdata.FileTimeStamp = DateTime.ParseExact(sTab[1], "ddMMyyyy", CultureInfo.InvariantCulture);
                            }
                            else
                                oKlarfdata.FileTimeStamp = DateTime.MinValue;
                        }
                        catch
                        {
                            oKlarfdata.FileTimeStamp = DateTime.MinValue;
                            // wrong format (old format adv 8 ?)
                        }

                        continue;
                    }
                    if (strLine.ToUpper().Contains("TIFFFILENAME"))
                    {
                        // TiffFileName
                        if (sTab.Length >= 2)
                            oKlarfdata.TiffFileName = sTab[1];
                        else
                            oKlarfdata.TiffFileName = string.Empty;
                        continue;
                    }
                    if (strLine.ToUpper().Contains("INSPECTIONSTATIONID"))
                    {
                        // InspectionStationID
                        if (sTab.Length >= 4)
                        {
                            //p_oKlarfdata.InspectionStationID = sTab[1].Replace('\"', ' ').Trim() + "-" + sTab[2].Replace('\"', ' ').Trim() + "-" + sTab[3].Replace('\"', ' ').Trim();
                            oKlarfdata.InspectionStationID = sTab[1] + " " + sTab[2] + " " + sTab[3];
                        }
                        else
                            oKlarfdata.InspectionStationID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLETYPE"))
                    {
                        // SampleType
                        if (sTab.Length >= 2)
                            oKlarfdata.SampleType = sTab[1];
                        else
                            oKlarfdata.SampleType = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("RESULTTIMESTAMP"))
                    {
                        try
                        {
                            // ResultTimeStamp
                            if (sTab.Length >= 3)
                            {
                                //should be format MM-dd-yyyy HH:mm:ss or dd-MM-yyyy HH:mm:ss
                                string sDate = sTab[1] + " " + sTab[2];
                                try
                                {
                                    oKlarfdata.ResultTimeStamp = DateTime.ParseExact(sDate, FmtDatetime, CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    oKlarfdata.ResultTimeStamp = DateTime.ParseExact(sDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                                }
                            }
                            else if (sTab.Length == 2)
                            {
                                //should be old format ddMMyyyy
                                oKlarfdata.ResultTimeStamp = DateTime.ParseExact(sTab[1], "ddMMyyyy", CultureInfo.InvariantCulture);
                            }
                            else
                                oKlarfdata.ResultTimeStamp = DateTime.MinValue;
                        }
                        catch
                        {
                            oKlarfdata.FileTimeStamp = DateTime.MinValue;
                            // wrong format (old format adv 8 ?)
                        }
                        continue;
                    }
                    if (strLine.ToUpper().Contains("LOTID"))
                    {
                        // LotID
                        if (sTab.Length >= 2)
                            oKlarfdata.LotID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            oKlarfdata.LotID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLESIZE"))
                    {
                        // SampleSize
                        if (sTab.Length >= 3)
                        {
                            if (!int.TryParse(sTab[1], out int NbSample))
                                NbSample = 0;
                            if (!int.TryParse(sTab[2], out int nWaferDiameterSize_mm))
                                nWaferDiameterSize_mm = 0;

                            oKlarfdata.SampleSize = new PrmSampleSize(nWaferDiameterSize_mm, NbSample);
                            continue; ;
                        }
                        else
                            oKlarfdata.SampleSize = new PrmSampleSize(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("DEVICEID"))
                    {
                        // DeviceID
                        if (sTab.Length >= 2)
                            oKlarfdata.DeviceID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            oKlarfdata.DeviceID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SETUPID"))
                    {
                        // SetupID
                        if (sTab.Length >= 2)
                        {
                            try
                            {
                                DateTime? dt = null;
                                string sId = ""; int FirstDateIdx = 1;
                                for (int vl = 1; vl < sTab.Length; vl++)
                                {
                                    sId += sTab[vl];
                                    if (sTab[vl].EndsWith("\""))
                                    {
                                        FirstDateIdx = vl + 1;
                                        break;
                                    }
                                }

                                string sDate = "";
                                for (int vl = FirstDateIdx; vl < sTab.Length; vl++)
                                {
                                    sDate += sTab[vl];
                                    if (vl < sTab.Length - 1)
                                        sDate += " ";
                                }

                                if (!DateTime.TryParseExact(sDate, FmtDatetime, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtParse1))
                                {
                                    if (DateTime.TryParseExact(sDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtParse21))
                                    {
                                        dt = dtParse21;
                                    }
                                    else if (DateTime.TryParseExact(sDate, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dtParse22))
                                    {
                                        dt = dtParse22;
                                    }
                                }
                                else
                                    dt = dtParse1;

                                oKlarfdata.SetupID = new PrmSetupId(sId.Replace('\"', ' ').Trim(), dt);
                            }
                            catch (Exception ex)
                            {
                                string serr = ex.Message;
                                oKlarfdata.SetupID = new PrmSetupId("ParseError", null);
                            }
                        }
                        else
                            oKlarfdata.SetupID = new PrmSetupId("Unknown", null);

                        continue;
                    }
                    if (strLine.ToUpper().Contains("STEPID"))
                    {
                        // StepID
                        if (sTab.Length >= 2)
                            oKlarfdata.StepID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            oKlarfdata.StepID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLEORIENTATIONMARKTYPE"))
                    {
                        // SampleOrientationMarkType
                        if (sTab.Length >= 2)
                        {
                            if (int.TryParse(sTab[1], out int nval))
                            {
                                oKlarfdata.SampleOrientationMarkType = new PrmSampleOrientationMarkType(nval); ;
                            }
                            else
                            {
                                oKlarfdata.SampleOrientationMarkType = new PrmSampleOrientationMarkType(sTab[1]); ;
                            }
                        }
                        else
                            oKlarfdata.SampleOrientationMarkType = new PrmSampleOrientationMarkType(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("ORIENTATIONMARKLOCATION"))
                    {
                        // OrientationMarkLocation
                        if (sTab.Length >= 2)
                        {
                            if (int.TryParse(sTab[1], out int nval))
                            {
                                oKlarfdata.OrientationMarkLocation = new PrmOrientationMarkValue(nval); ;
                            }
                            else
                            {
                                oKlarfdata.OrientationMarkLocation = new PrmOrientationMarkValue(sTab[1]); ;
                            }
                        }
                        else
                            oKlarfdata.OrientationMarkLocation = new PrmOrientationMarkValue(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("WAFERID"))
                    {
                        // WaferID
                        if (sTab.Length >= 2)
                            oKlarfdata.WaferID = sTab[1].Replace('\"', ' ').Trim();
                        else
                            oKlarfdata.WaferID = "Unknown";
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SLOT"))
                    {
                        // SlotID
                        if (sTab.Length >= 2)
                        {
                            if (!int.TryParse(sTab[1], out int nval))
                                nval = 0;
                            oKlarfdata.SlotID = nval;
                        }
                        else
                            oKlarfdata.SlotID = 0;
                        continue;
                    }
                    if (strLine.ToUpper().Contains("ORIENTATIONINSTRUCTION"))
                    {
                        // OrientationInstruction
                        if (sTab.Length >= 2)
                        {
                            oKlarfdata.OrientationInstructions = new PrmOrientationInstruction(sTab[1].Replace('\"', ' ').Trim());
                        }
                        else
                            oKlarfdata.OrientationInstructions = new PrmOrientationInstruction(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("COORDINATESMIRRORED"))
                    {
                        // CoordinatesMirrored
                        if (sTab.Length >= 2)
                        {
                            if (int.TryParse(sTab[1], out int nval))
                            {
                                oKlarfdata.CoordinatesMirrored = new PrmYesNo(nval != 0); ;
                            }
                            else
                            {
                                oKlarfdata.CoordinatesMirrored = new PrmYesNo(sTab[1].ToUpper() == "YES");
                            }
                        }
                        continue;
                    }
                    if (strLine.ToUpper().Contains("COORDINATESCENTERED"))
                    {
                        // CoordinatesCentered
                        if (sTab.Length >= 2)
                        {
                            if (int.TryParse(sTab[1], out int nval))
                            {
                                oKlarfdata.CoordinatesCentered = new PrmYesNo(nval != 0); ;
                            }
                            else
                            {
                                oKlarfdata.CoordinatesCentered = new PrmYesNo(sTab[1].ToUpper() == "YES");
                            }
                        }
                        continue;
                    }
                    if (strLine.ToUpper().Contains("INSPECTIONORIENTATION"))
                    {
                        // InspectionOrientation
                        if (sTab.Length >= 2)
                        {
                            if (int.TryParse(sTab[1], out int nval))
                            {
                                oKlarfdata.InspectionOrientation = new PrmOrientationMarkValue(nval); ;
                            }
                            else
                            {
                                oKlarfdata.InspectionOrientation = new PrmOrientationMarkValue(sTab[1]); ;
                            }
                        }
                        //else p_oKlarfdata.InspectionOrientation = new PrmOrientationMarkValue(0);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLECENTERLOCATION"))
                    {
                        // SampleCenterLocation
                        if (sTab.Length >= 3)
                            oKlarfdata.SampleCenterLocation = new PrmPtFloat((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat),
                                                                    (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                        else
                            oKlarfdata.SampleCenterLocation = new PrmPtFloat(float.NaN, float.NaN);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("SAMPLETESTPLAN"))
                    {
                        // SampleTestPlan
                        string[] lStringSeparator = new string[] { "\n" };
                        strLine = strLine.Trim().Replace("\n\r", "");
                        strLine = strLine.Trim().Replace("\r", " ");
                        sTab = strLine.Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (sTab.Length >= 2)
                        {
                            int lNbDies = 0;
                            for (int k = 0; k < sTab.Length; k++)
                            {
                                string[] sTabDiesIndexes = sTab[k].Trim().Split(' ');
                                if (sTabDiesIndexes.Length % 2 != 0)
                                    throw new Exception("SampleTestPlan BAD format - ODD number of parameters");
                                int nConsumed = 0;
                                if (sTabDiesIndexes[0].Contains("SampleTestPlan"))
                                {
                                    lNbDies = Convert.ToInt32(sTabDiesIndexes[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                                    oKlarfdata.SampleTestPlan.ClearPlan();
                                    // ON DOIT GERER LE CAS pas de retour ligne "SampleTestPlan 1 0 0;" ou "SampleTestPlan n 0 0 1 1 2 2"
                                    nConsumed = 2;
                                    while ((sTabDiesIndexes.Length - nConsumed) > 0)
                                    {
                                        oKlarfdata.SampleTestPlan.Add(Convert.ToInt32(sTabDiesIndexes[nConsumed + 0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
                                                                        Convert.ToInt32(sTabDiesIndexes[nConsumed + 1].Trim(), CultureInfo.InvariantCulture.NumberFormat));

                                        nConsumed += 2;
                                    }
                                    continue;
                                }

                                while ((sTabDiesIndexes.Length - nConsumed) > 0)
                                {
                                    oKlarfdata.SampleTestPlan.Add(Convert.ToInt32(sTabDiesIndexes[nConsumed + 0].Trim(), CultureInfo.InvariantCulture.NumberFormat),
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
                            oKlarfdata.DiePitch = new PrmPtFloat((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat),
                                                                   (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                        else
                            oKlarfdata.DiePitch = new PrmPtFloat(float.NaN, float.NaN);
                        continue;
                    }
                    if (strLine.ToUpper().Contains("DIEORIGIN"))
                    {
                        // DieOrigin
                        if (sTab.Length >= 3)
                            oKlarfdata.DieOrigin = new PrmPtFloat((float)Convert.ToDouble(sTab[1], CultureInfo.InvariantCulture.NumberFormat),
                                                                    (float)Convert.ToDouble(sTab[2], CultureInfo.InvariantCulture.NumberFormat));
                        else
                            oKlarfdata.DieOrigin = new PrmPtFloat(float.NaN, float.NaN);
                        continue;
                    }

                    if (strLine.ToUpper().Contains("DEFECTRECORDSPEC"))
                    {
                        // DefectRecordSpec
                        string[] lStringSeparator = new string[] { "DEFECTID" };
                        sTab = strLine.Trim().Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        lStringSeparator = new string[] { " " };
                        string[] sTabHeader = sTab[0].Trim().Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                        if (sTabHeader.Length >= 2)
                        {
                            int lNbColumns = Convert.ToInt32(sTabHeader[1], CultureInfo.InvariantCulture.NumberFormat);
                            string[] strNameTab = sTab[1].Split(lStringSeparator, StringSplitOptions.RemoveEmptyEntries);
                            if (strNameTab.Length + 1 == lNbColumns)
                            {
                                oKlarfdata.AddDefectType("DEFECTID");
                                oKlarfdata.AddDefectTypes(strNameTab);
                            }
                        }
                        continue;
                    }

                    if (strLine.ToUpper().Contains("DEFECTLIST"))
                    {
                        string[] lSeparator = new string[] { "\r\n" };
                        string[] lDefData = strLine.Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
                        for (int nidlin = 1; nidlin < lDefData.Length; nidlin++) // l'index 0 should contained only DEFECTLIST tag
                        {
                            var def = oKlarfdata.NewDefect();
                            def.SetFromString(lDefData[nidlin]);
                            oKlarfdata.AddDefect(def);
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
                        string[] lSeparator = new string[] { "\r\n" };
                        string[] SumSplit = strLine.Split(lSeparator, StringSplitOptions.RemoveEmptyEntries);
                        string[] lSeparatorSpace = new string[] { " " };
                        string[] ldatafooter = SumSplit[1].Split(lSeparatorSpace, StringSplitOptions.RemoveEmptyEntries);
                        oKlarfdata.NDIE = Convert.ToInt32(ldatafooter[3], CultureInfo.InvariantCulture.NumberFormat);
                        oKlarfdata.NDEFDIE = Convert.ToInt32(ldatafooter[4], CultureInfo.InvariantCulture.NumberFormat);
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
    }
}
