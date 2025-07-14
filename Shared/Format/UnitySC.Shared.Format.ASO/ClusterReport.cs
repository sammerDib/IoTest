using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace UnitySC.Shared.Format.ASO
{
    public class ClusterReport
    {
        private const char __carsep = '|'; // caracteristic feature separator

        private readonly Dictionary<string, string> _caracteristicsFeatures;
        public Dictionary<string, string> CharacFeatures { get => _caracteristicsFeatures; }

        public Color Color { get; set; } = Color.White;

        public int ClusterNumber { get; set; }
        public int BlocNumber { get; set; }
        public int BlocSelected { get; set; }
        public int NumberOfDefect { get; set; }

        //
        public string UserLabel { get; set; }

        public string UnitUsed { get; set; }

        // données micron
        public double TotalclusterSize { get; set; }

        public double MaxClusterSize { get; set; }
        public int MicronPositionX { get; set; }
        public int MicronPositionY { get; set; }
        public double MicronSizeX { get; set; }
        public double MicronSizeY { get; set; }

        // données pixel
        public int PixelPositionX { get; set; }

        public int PixelPositionY { get; set; }
        public int PixelSizeX { get; set; }
        public int PixelSizeY { get; set; }

        // données pixel image
        public int PicturePositionX { get; set; }

        public int PicturePositionY { get; set; }

        //
        public string ThumbnailGreyLevelFilePath { get; set; }

        public string ThumbnailBinaryFilePath { get; set; }

        //
        public int SrcImageMosaic_Column { get; set; }

        public int SrcImageMosaic_Line { get; set; }
        public int SrcImageMosaic_VirtualBloc { get; set; }
        public int SrcImageMosaic_DieX { get; set; }
        public int SrcImageMosaic_DieY { get; set; }

        public string CustomerReportLabel { get; set; }
        public string ReportTypeForSize { get; set; }
        public bool IsKillingDefect { get; set; }

        public ClusterReport()
        {
            _caracteristicsFeatures = new Dictionary<string, string>();
        }

        public bool ParseCluster(string[] sHeaderInfo)
        {
            bool bSelectedforReport = true;
            try
            {
                // 0;num_cluster; num_bloc; BlocSelect; category; nb_defect; totalclustersize; maxclustersize; micronposx; micronposy; micronsizex; micronsizey; unit;
                // pixelposx; pixelposy; pictureposx; pictureposy;pixelsizex; pixelsizey; thumbnailgreylevel; thumbnailbinary; columnnumber; linenumber; virtualblocnumber;
                // colorname; diex; diey; customerlabel; typereportsize; iskilling; [caracteristicsfeatures]
                ClusterNumber = Convert.ToInt32(sHeaderInfo[1], CultureInfo.InvariantCulture.NumberFormat);
                BlocNumber = Convert.ToInt32(sHeaderInfo[2], CultureInfo.InvariantCulture.NumberFormat);
                BlocSelected = Convert.ToInt32(sHeaderInfo[3], CultureInfo.InvariantCulture.NumberFormat);
                NumberOfDefect = Convert.ToInt32(sHeaderInfo[5], CultureInfo.InvariantCulture.NumberFormat);

                if (BlocNumber != BlocSelected)
                    bSelectedforReport = false;
                else
                {
                    //
                    UserLabel = sHeaderInfo[4];
                    UnitUsed = sHeaderInfo[12];
                    // données micron
                    TotalclusterSize = Convert.ToDouble(sHeaderInfo[6], CultureInfo.InvariantCulture.NumberFormat);
                    MaxClusterSize = Convert.ToDouble(sHeaderInfo[7], CultureInfo.InvariantCulture.NumberFormat);
                    MicronPositionX = Convert.ToInt32(sHeaderInfo[8], CultureInfo.InvariantCulture.NumberFormat);
                    MicronPositionY = Convert.ToInt32(sHeaderInfo[9], CultureInfo.InvariantCulture.NumberFormat);
                    MicronSizeX = Convert.ToDouble(sHeaderInfo[10], CultureInfo.InvariantCulture.NumberFormat);
                    MicronSizeY = Convert.ToDouble(sHeaderInfo[11], CultureInfo.InvariantCulture.NumberFormat);
                    // données pixel
                    PixelPositionX = Convert.ToInt32(sHeaderInfo[13], CultureInfo.InvariantCulture.NumberFormat);
                    PixelPositionY = Convert.ToInt32(sHeaderInfo[14], CultureInfo.InvariantCulture.NumberFormat);
                    PixelSizeX = Convert.ToInt32(sHeaderInfo[17], CultureInfo.InvariantCulture.NumberFormat);
                    PixelSizeY = Convert.ToInt32(sHeaderInfo[18], CultureInfo.InvariantCulture.NumberFormat);
                    // données pixel image
                    PicturePositionX = Convert.ToInt32(sHeaderInfo[15], CultureInfo.InvariantCulture.NumberFormat);
                    PicturePositionY = Convert.ToInt32(sHeaderInfo[16], CultureInfo.InvariantCulture.NumberFormat);
                    //
                    ThumbnailGreyLevelFilePath = sHeaderInfo[19];
                    ThumbnailBinaryFilePath = sHeaderInfo[20];
                    //
                    SrcImageMosaic_Column = Convert.ToInt32(sHeaderInfo[21], CultureInfo.InvariantCulture.NumberFormat);
                    SrcImageMosaic_Line = Convert.ToInt32(sHeaderInfo[22], CultureInfo.InvariantCulture.NumberFormat);
                    SrcImageMosaic_VirtualBloc = Convert.ToInt32(sHeaderInfo[23], CultureInfo.InvariantCulture.NumberFormat);

                    Color = Color.FromName(sHeaderInfo[24]);

                    SrcImageMosaic_DieX = Convert.ToInt32(sHeaderInfo[25], CultureInfo.InvariantCulture.NumberFormat);
                    SrcImageMosaic_DieY = Convert.ToInt32(sHeaderInfo[26], CultureInfo.InvariantCulture.NumberFormat);

                    CustomerReportLabel = sHeaderInfo[27];
                    ReportTypeForSize = sHeaderInfo[28];
                    IsKillingDefect = Convert.ToBoolean(Convert.ToInt32(sHeaderInfo[29], CultureInfo.InvariantCulture.NumberFormat), CultureInfo.InvariantCulture.NumberFormat);

                    // characteristics features
                    try
                    {
                        for (int i = 30; i < sHeaderInfo.Length; i++)
                        {
                            string[] sCaract = sHeaderInfo[i].Split(__carsep);

                            if (sCaract.Length == 2)
                                AddCaracteristic(sCaract[0], sCaract[1]);
                            else // absolute position
                            {
                                // the old way (adcv8) to do it  ==> String sValue = sCaract[1] + __carsep + sCaract[2] + __carsep + sCaract[3] + __carsep + sCaract[4];
                                var sb = new StringBuilder();
                                for (int j = 1; j < 4; j++)
                                {
                                    sb.Append(sCaract[j]);
                                    sb.Append(__carsep);
                                }
                                sb.Append(sCaract[4]);
                                AddCaracteristic(sCaract[0], sb.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Invalid cluster caracteristic - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Invalid header cluster parameters - {ex.Message}");
            }

            return bSelectedforReport;
        }

        public void AddCaracteristic(string sUserLabel, string sValue)
        {
            if (!_caracteristicsFeatures.ContainsKey(sUserLabel))
                _caracteristicsFeatures.Add(sUserLabel, sValue);
            else
                _caracteristicsFeatures[sUserLabel] += "[+]" + sValue;
        }

        public void WriteCluster(StreamWriter sw, char sep, char eol)
        {
            var sb = new StringBuilder(2048);
            sb.Append("CLUSTER_DESCR"); sb.Append(sep); sb.Append(ClusterNumber); sb.Append(sep);
            sb.Append(BlocNumber); sb.Append(sep); sb.Append(BlocSelected); sb.Append(sep); sb.Append(UserLabel); sb.Append(sep); sb.Append(NumberOfDefect); sb.Append(sep);
            sb.Append(TotalclusterSize); sb.Append(sep); sb.Append(MaxClusterSize); sb.Append(sep);
            sb.Append(MicronPositionX); sb.Append(sep); sb.Append(MicronPositionY); sb.Append(sep);
            sb.Append(MicronSizeX); sb.Append(sep); sb.Append(MicronSizeY); sb.Append(sep); sb.Append(UnitUsed); sb.Append(sep);
            sb.Append(PixelPositionX); sb.Append(sep); sb.Append(PixelPositionY); sb.Append(sep); sb.Append(PicturePositionX); sb.Append(sep); sb.Append(PicturePositionY); sb.Append(sep);
            sb.Append(PixelSizeX); sb.Append(sep); sb.Append(PixelSizeY); sb.Append(sep);
            sb.Append(ThumbnailGreyLevelFilePath); sb.Append(sep); sb.Append(ThumbnailBinaryFilePath); sb.Append(sep);
            sb.Append(SrcImageMosaic_Column); sb.Append(sep); sb.Append(SrcImageMosaic_Line); sb.Append(sep); sb.Append(SrcImageMosaic_VirtualBloc); sb.Append(sep); sb.Append(Color.Name); sb.Append(sep);
            sb.Append(SrcImageMosaic_DieX); sb.Append(sep); sb.Append(SrcImageMosaic_DieY); sb.Append(sep); sb.Append(CustomerReportLabel); sb.Append(sep); sb.Append(ReportTypeForSize); sb.Append(sep);
            sb.Append(IsKillingDefect ? "1" : "0");

            foreach (var kvp in _caracteristicsFeatures)
            {
                sb.Append(sep);
                sb.Append(kvp.Key);
                sb.Append(__carsep);
                sb.Append(kvp.Value);
            }
            sb.Append(eol);

            sw.WriteLine(sb.ToString());
        }
    }
}