using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using ADCEngine;
using ADCEngine.Parameters;

using BasicModules.Edition.DataBase;

using HazeLSModule.Properties;

using UnitySC.Shared.Tools;

namespace HazeLSModule
{
    ///////////////////////////////////////////////////////////////////////
    // Module
    ///////////////////////////////////////////////////////////////////////
    public class HazeLSAddReportModule : DatabaseEditionModule
    {
        private int _wafersize_mm = 0;
        private string _adcRecipeName = string.Empty;
        private string _waferID = string.Empty;
        private string _toolID = string.Empty;
        private String _ReportLotId = String.Empty;
        private int _ReportSlotID = -1;
        private DateTime _startProcessDate = DateTime.MinValue;

        internal sealed class WaferPortion
        {
            public int IndexX;
            public int IndexY;
            public System.Drawing.Rectangle BoxInImage;
            public double AveragePpm;
        }


        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.HazeLS_HAZE);
            return Rtypes;
        }

        protected string _Filename = String.Empty;

        protected HazeLSMeasure _hazeMeasure = null;

        protected static Color[] _colorMapArray = null;
        protected const uint _cstNbHistoStep = 1024;
        protected const double _cstHistoMin_ppm = 0.0;

        public readonly SeparatorParameter paramSeparatorXml;
        public readonly IntParameter paramRasterSizeX;
        public readonly IntParameter paramRasterSizeY;

        //=================================================================
        // Constructeur
        //=================================================================
        public HazeLSAddReportModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            if (_colorMapArray == null)
                _colorMapArray = InitColorMapRef();

            paramSeparatorXml = new SeparatorParameter(this, "XML wafer grid");
            paramRasterSizeX = new IntParameter(this, "RasterSizeX", 1);
            paramRasterSizeX.Value = 50;
            paramRasterSizeY = new IntParameter(this, "RasterSizeY", 1);
            paramRasterSizeY.Value = 50;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            _Filename = GetResultFullPathName(ResultTypeFile.HazeLS_HAZE);
            _Filename = Path.Combine(Path.GetDirectoryName(_Filename), Path.GetFileNameWithoutExtension(_Filename));

            WaferBase wafer = Recipe.Wafer;
            if (Wafer is NotchWafer)
            {
                _wafersize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                _wafersize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);
            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                _wafersize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }

            _ReportLotId = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.LotID);
            int nSlotID = -1;
            if (!int.TryParse(Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID), NumberStyles.Integer, CultureInfo.InvariantCulture, out nSlotID))
            {
                String sErrMsg = String.Format("Error Unable to Parse Wafer SlotId (\"{0}\")", Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.SlotID));
                logError(sErrMsg);
                throw new Exception("Init Exception : " + sErrMsg);
            }
            _ReportSlotID = nSlotID;

            _waferID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.WaferID);
            _toolID = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.EquipmentID);
            if (String.IsNullOrEmpty(_toolID) || _toolID == "DEFAULT_EQUIPMENT_ID")
            {
                _toolID = Recipe.Toolname;
            }
            _adcRecipeName = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.ADCRecipeFileName);
            try
            {
                string strdate = Wafer.GetWaferInfo(AcquisitionAdcExchange.eWaferInfo.StartProcess);
                _startProcessDate = DateTime.ParseExact(strdate.TrimDuplicateSpaces(), "M-d-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                _startProcessDate = DateTime.Now;
            }
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("Process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            HazeLSMeasure mes = (HazeLSMeasure)obj;

            //-------------------------------------------------------------
            // Stockage des Mesures
            //-------------------------------------------------------------
            mes.AddRef();
            _hazeMeasure = mes;

        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("ProcessHazeAddReport", () =>
            {
                try
                {
                    if (oldState == eModuleState.Running)
                    {
                        try
                        {
                            ProcessHazeAddReport_Image();
                            ProcessHazeAddReport_Xml();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        finally
                        {
                            PurgeHazeAddReport(); // on en doit libérer qu'une fois les données pour img et/ou haze
                        }

                    }
                    else if (oldState == eModuleState.Aborting)
                    {
                        PurgeHazeAddReport();
                    }
                    else
                        throw new ApplicationException("invalid state");
                }
                catch (Exception ex)
                {
                    string msg = "HAZE Additionnal report generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        //=================================================================
        // 
        //=================================================================
        private void PurgeHazeAddReport()
        {
            // Purge de la liste interne des mesure
            //......................................
            _hazeMeasure?.DelRef();
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessHazeAddReport_Xml()
        {
            try
            {

                string sEquipmentVendor = "UnitySC";
                string sMF_ID = "none";
                string sGravure = _waferID;

                //-------------------------------------------------------------
                // Write HAZE Additionnale Report Xml file
                //-------------------------------------------------------------
                String sType = "TOT";
                if (_hazeMeasure.Data.nId == (int)AcquisitionAdcExchange.eChannelID.Haze_Forward)
                    sType = "FW";
                else if (_hazeMeasure.Data.nId == (int)AcquisitionAdcExchange.eChannelID.Haze_Backward)
                    sType = "BW";
                String sAdditionnalReportXml = $"{_Filename}_{sType}.xml";

                log($"Creating LS HAZE Additionnal report Xml file {sAdditionnalReportXml}");

                // on génére la grille des sections du wafer on calcul la resolution et la taille de la grille en fct de la taille de l'image
                // on se fixe un nombre de decoupe en X et en Y en parametre => on en déduit la résolution en mm
                int nRasterSizeX = paramRasterSizeX.Value;
                int nRasterSizeY = paramRasterSizeY.Value;
                int portionWidth_px = ((int)Math.Ceiling((double)_hazeMeasure.Data.nWidth / (double)nRasterSizeX));
                while (portionWidth_px * (nRasterSizeX - 1) >= _hazeMeasure.Data.nWidth)
                    portionWidth_px--;// assure que la taille des nRasterSizeX - 1 colonnes ne dépasse pas la largeur de l'image
                int portionHeight_px = ((int)Math.Ceiling((double)_hazeMeasure.Data.nHeigth / (double)nRasterSizeY));
                while (portionHeight_px * (nRasterSizeY - 1) >= _hazeMeasure.Data.nHeigth)
                    portionHeight_px--;// assure que la taille des nRasterSizeY - 1 lignes ne dépasse pas la hauteur de l'image

                if (portionWidth_px <= 0)
                {
                    throw new Exception($"X Section Number is too big or Haze data width is to small [W={_hazeMeasure.Data.nWidth}]");
                }

                if (portionHeight_px <= 0)
                {
                    throw new Exception($"Y Section Number is too big or Haze data height is to small [H={_hazeMeasure.Data.nHeigth}]");
                }

                // la resolution en mm pour le report xml, est calculé en se basant sur le wafer size 
                // car l’image qu’on découpe, contenu dans le fichier haze ne fait pas la taille du wafer (elle peut être plus petite ou plus grande)
                double portionResolutionX_mm = (double)_wafersize_mm / (double)nRasterSizeX;
                double portionResolutionY_mm = (double)_wafersize_mm / (double)nRasterSizeY;

                List<WaferPortion> HazeSections = new List<WaferPortion>();
                // create all portions
                for (int colIndex = 1; colIndex <= nRasterSizeX; colIndex++)
                {
                    // the last column may not be the same width than others, in this case it is truncated
                    int rectWidth = (colIndex == nRasterSizeX) ? (_hazeMeasure.Data.nWidth - (nRasterSizeX - 1) * portionWidth_px) : portionWidth_px;
                    for (int rowIndex = 1; rowIndex <= nRasterSizeY; rowIndex++)
                    {
                        WaferPortion portion = new WaferPortion();
                        portion.IndexX = colIndex;
                        portion.IndexY = rowIndex;

                        // the last row may not be the same height than others, in this case it is truncated
                        int rectHeight = (rowIndex == nRasterSizeY) ? (_hazeMeasure.Data.nHeigth - (nRasterSizeY - 1) * portionHeight_px) : portionHeight_px;

                        portion.BoxInImage = new Rectangle((colIndex - 1) * portionWidth_px, (rowIndex - 1) * portionHeight_px, rectWidth, rectHeight);
                        HazeSections.Add(portion);
                    }
                }


                // pour chaque section de la grille on calcule le haze de la section

                // each section has RECT ara and IndexX and IndexY number --> class WaferPortion 
                // /!\ a ce que les rectangle aera ne sortent pas de notre image !
                Parallel.ForEach(HazeSections, portion =>
                //foreach (WaferPortion portion in HazeSections)
                {
                    ComputeAvgHazeArea(portion);
                });


                ///
                // Xml Document writing
                //
                XmlDocument xmlReport = new XmlDocument();

                // root node
                XmlElement rootNode = xmlReport.CreateElement("MeasureData");
                xmlReport.AppendChild(rootNode);

                //
                // HEADER
                //
                XmlElement headerNode = xmlReport.CreateElement("Header");
                rootNode.AppendChild(headerNode);

                // Wafer Ident
                XmlElement waferIndetNode = xmlReport.CreateElement("WaferIdent");
                headerNode.AppendChild(waferIndetNode);
                waferIndetNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "WaferID", _waferID));
                waferIndetNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "MF_ID", sMF_ID));
                waferIndetNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "Gravure", sGravure));
                waferIndetNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "LotName", _ReportLotId));
                waferIndetNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "SlotNR", _ReportSlotID.ToString()));
                waferIndetNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "WaferDiameter", ConvertMmToIntegerInch(_wafersize_mm).ToString()));

                // MeasureInfo
                XmlElement measureInfoNode = xmlReport.CreateElement("MeasureInfo");
                headerNode.AppendChild(measureInfoNode);
                measureInfoNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "MeasureTime", _startProcessDate.ToString("dd.MM.yyyy HH:mm:ss")));
                measureInfoNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "RasterSizeX", nRasterSizeX.ToString()));
                measureInfoNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "RasterSizeY", nRasterSizeY.ToString()));
                measureInfoNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "MeasurementResolution", portionResolutionX_mm.ToString("0.#") + "mm"));
                measureInfoNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "MeasurementResolutionY", portionResolutionY_mm.ToString("0.#") + "mm"));

                // Custom section
                XmlElement customSectionNode = xmlReport.CreateElement("MeasureInfo");
                headerNode.AppendChild(customSectionNode);
                customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", "Equipment", _toolID));
                customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", "EquipmentVendor", sEquipmentVendor));
                double edgeexl_mm = (double)_hazeMeasure.EdgeExlusion_um / 1000.0;

                customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", "Edge Exclusion", edgeexl_mm.ToString("0.##")));
                customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", "Haze Average", _hazeMeasure.Data.mean_ppm.ToString("0.00")));

                int nbRanges = _hazeMeasure.Data.Ranges.Count;
                if (nbRanges > 2) // 3 ranges needed at least
                {
                    customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", "Below Haze Range", $"{_hazeMeasure.Data.Ranges[0].area_pct:#0.00}"));
                    for (int i = 1; i < nbRanges - 1; i++)
                    {
                        customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", $"{_hazeMeasure.Data.Ranges[i].min_ppm} - {_hazeMeasure.Data.Ranges[i].max_ppm}", $"{_hazeMeasure.Data.Ranges[i].area_pct:#0.00}"));
                    }
                    customSectionNode.AppendChild(CreateXmlNodeKeyWithContent(xmlReport, "Setting", "Above Haze Range", $"{_hazeMeasure.Data.Ranges[nbRanges - 1].area_pct:#0.00}"));
                }

                //
                // Measurements
                //
                XmlElement measurementsNode = xmlReport.CreateElement("Measurements");
                rootNode.AppendChild(measurementsNode);
                XmlElement paramNode = xmlReport.CreateElement("ParameterName");
                XmlAttribute valueName = xmlReport.CreateAttribute("Name");
                valueName.Value = "Haze";
                paramNode.Attributes.Append(valueName);
                measurementsNode.AppendChild(paramNode);
                foreach (WaferPortion portion in HazeSections)
                {
                    XmlElement measureItemNode = xmlReport.CreateElement("MeasurementItem");

                    measureItemNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "X", portion.IndexX.ToString()));
                    measureItemNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "Y", portion.IndexY.ToString()));
                    measureItemNode.AppendChild(CreateXmlNodeWithContent(xmlReport, "Value", String.Format("{0:0.##}", portion.AveragePpm)));

                    paramNode.AppendChild(measureItemNode);
                }


                //
                // Save Xml report
                //
                xmlReport.Save(sAdditionnalReportXml);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                throw;
            }
        }

        private void ComputeAvgHazeArea(WaferPortion wafersection)
        {
            wafersection.AveragePpm = 0.0;
            int nW = _hazeMeasure.Data.nWidth;
            double dsumppmavg = 0.0;
            double dtot = 0.0;
            for (int xx = wafersection.BoxInImage.Left; xx < wafersection.BoxInImage.Right; xx++)
            {
                for (int yy = wafersection.BoxInImage.Top; yy < wafersection.BoxInImage.Bottom; yy++)
                {
                    try
                    {
                        float ppmVal = _hazeMeasure.Data.HazeMeasures[yy * nW + xx];
                        if (ppmVal == 0.0f || float.IsNaN(ppmVal))
                            continue;

                        dsumppmavg += (double)ppmVal;
                        dtot++;
                    }
                    catch (Exception ex)
                    {
                        ex.Data.Add("UserMessage", $"catch in wafer section [{wafersection.IndexX},{wafersection.IndexY}]");
                        throw;
                    }
                }
            }
            if (dtot != 0.0)
                wafersection.AveragePpm = (dsumppmavg / dtot);
        }

        private static int ConvertMmToIntegerInch(int mmValue)
        {
            double inchValue = (double)mmValue / 25.4;
            return (int)Math.Round(inchValue);
        }

        private static XmlNode CreateXmlNodeWithContent(XmlDocument xmlDoc, string sNodeName, string sContent)
        {
            XmlNode node;
            node = xmlDoc.CreateElement(sNodeName);
            node.InnerText = sContent;
            return node;
        }

        private static XmlNode CreateXmlNodeKeyWithContent(XmlDocument xmlDoc, string sNodeName, string sNodeKey, string sContent)
        {
            XmlNode node;
            XmlAttribute valueAttribute;
            node = xmlDoc.CreateElement(sNodeName);
            valueAttribute = xmlDoc.CreateAttribute("Key");
            valueAttribute.Value = sNodeKey;
            node.Attributes.Append(valueAttribute);
            node.InnerText = sContent;
            return node;
        }

        //=================================================================
        // 
        //=================================================================
        private void ProcessHazeAddReport_Image()
        {
            try
            {
                //-------------------------------------------------------------
                // Write HAZE Additionnale Report Image file
                //-------------------------------------------------------------
                String sType = "TOT";
                if (_hazeMeasure.Data.nId == (int)AcquisitionAdcExchange.eChannelID.Haze_Forward)
                    sType = "FW";
                else if (_hazeMeasure.Data.nId == (int)AcquisitionAdcExchange.eChannelID.Haze_Backward)
                    sType = "BW";
                String sAdditionnalReportImg = $"{_Filename}_{sType}.png";

                log($"Creating LS HAZE Additionnal report Image file {sAdditionnalReportImg}");

                // Image Settings and percentage ratio
                const int nReportImgW = 1920;
                const int nReportImgH = 1080;

                const int allBorderMargin_px = 5;
                const int lineSepVMargin_px = 40;
                const int lineSepHMargin_px = 50;
                const int colorMapWidth_px = 30;
                const int fontsize_px = 20;
                const int colormargin_px = 18;
                const float statSpacing_px = 12;

                const double sepV_wratio = 0.655;
                const double sepH_hratio = 0.51;
                const double waferArea_wratio = 0.42;
                const double binningendarea_hratio = 0.90;
                const double InfoMiddleArea_wratio = 0.79;

                double hazeppm_MaxUse = _hazeMeasure.HazeMax_ppmCal;
                double hazeppm_MinUse = _cstHistoMin_ppm;

                StringFormat strFormatAlignRight_C = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.Character };
                StringFormat strFormatAlignLeft_C = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.Character };
                StringFormat strFormatAlignCenter_C = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.Character };

                StringFormat strFormatAlignRight_Top = new StringFormat() { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Near, Trimming = StringTrimming.Character };
                StringFormat strFormatAlignLeft_Top = new StringFormat() { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near, Trimming = StringTrimming.Character };

                using (System.Drawing.Image reportImg = new Bitmap(nReportImgW, nReportImgH, PixelFormat.Format24bppRgb))
                using (Graphics g = Graphics.FromImage(reportImg))
                using (SolidBrush blackBrush = new SolidBrush(Color.FromArgb(0, 0, 0)))
                using (SolidBrush whiteBrush = new SolidBrush(Color.FromArgb(255, 255, 255)))
                using (Pen whitePen = new Pen(whiteBrush))
                using (Pen whitePen5 = new Pen(whiteBrush, 5f))
                using (Font fontRegular = new System.Drawing.Font("Calibri Light", fontsize_px, FontStyle.Regular))
                using (Font fontBold = new System.Drawing.Font("Calibri Light", fontsize_px, FontStyle.Bold))
                {
                    // fond noir
                    g.Clear(Color.Black);

                    float xSepV = (float)Math.Floor((double)nReportImgW * sepV_wratio);
                    float ySepH = (float)Math.Floor((double)nReportImgH * sepH_hratio);
                    float xEndWaferArea = (float)Math.Floor((double)nReportImgW * waferArea_wratio);
                    float waferAreaSize = xEndWaferArea - allBorderMargin_px - 2.0f;
                    float binningBottomArea = (float)Math.Floor((double)nReportImgH * binningendarea_hratio);
                    float xMiddleStatArea = (xSepV + xEndWaferArea) / 2.0f;
                    float xMiddleStatAreaWidth = (xSepV - xEndWaferArea) / 2.0f;

                    float xMiddleInfoDecalArea = (float)Math.Floor((double)nReportImgW * InfoMiddleArea_wratio);
                    float xMiddleOffset_px = ((xSepV + nReportImgW) / 2.0f) - xMiddleInfoDecalArea;
                    float xMiddleInfoArea = xMiddleInfoDecalArea; //old formula = (xSepV + nReportImgW) / 2.0f - xMiddleOffset_px;
                    float yInfoOffset_px = 80.0f;
                    float xMiddleInfoAreaWidth = (nReportImgW - xSepV - 2.0f * statSpacing_px) / 2.0f;
                    float xMiddleInfoAreaWidth_Left = xMiddleInfoAreaWidth - xMiddleOffset_px;
                    float xMiddleInfoAreaWidth_Right = xMiddleInfoAreaWidth + xMiddleOffset_px;

                    //Area
                    var colorMapArearc = new Rectangle((int)xEndWaferArea + colormargin_px, colormargin_px, colorMapWidth_px, (int)waferAreaSize - 2 * colormargin_px);
                    var leftStatArearc = new RectangleF(xMiddleStatArea - statSpacing_px - xMiddleStatAreaWidth, colormargin_px, xMiddleStatAreaWidth, colorMapArearc.Height);
                    var rightStatArearc = new RectangleF(xMiddleStatArea + statSpacing_px, colormargin_px, xMiddleStatAreaWidth, colorMapArearc.Height);
                    var waferArearc = new RectangleF(allBorderMargin_px, allBorderMargin_px, waferAreaSize, waferAreaSize);
                    var histoArearc = new RectangleF(allBorderMargin_px, waferArearc.Bottom + allBorderMargin_px, xSepV - allBorderMargin_px, nReportImgH - (waferArearc.Height + 3 * allBorderMargin_px));
                    var leftInfoArearc = new RectangleF(xMiddleInfoArea - statSpacing_px - xMiddleInfoAreaWidth_Left, allBorderMargin_px + yInfoOffset_px, xMiddleInfoAreaWidth_Left, ySepH - 2 * allBorderMargin_px - yInfoOffset_px);
                    var rightInfoArearc = new RectangleF(xMiddleInfoArea + statSpacing_px, allBorderMargin_px + yInfoOffset_px, xMiddleInfoAreaWidth_Right - statSpacing_px, ySepH - 2 * allBorderMargin_px - yInfoOffset_px);

                    var binTitleArearc = new RectangleF(xSepV + allBorderMargin_px, ySepH + allBorderMargin_px,
                                nReportImgW - xSepV - 2 * allBorderMargin_px - xMiddleOffset_px,
                                fontRegular.Size + fontRegular.GetHeight());

                    var leftBinArearc = new RectangleF(xMiddleInfoArea - statSpacing_px - xMiddleInfoAreaWidth_Left, binTitleArearc.Bottom, xMiddleInfoAreaWidth_Left, binningBottomArea - binTitleArearc.Bottom);
                    var rightBinArearc = new RectangleF(xMiddleInfoArea + statSpacing_px, binTitleArearc.Bottom, xMiddleInfoAreaWidth_Right, leftBinArearc.Height);
                    var sizeBinfont = g.MeasureString("Area %", fontBold); //100.00 plus petit que la legende
                    var rightBinPourcentageArearc = new RectangleF(rightBinArearc.X, rightBinArearc.Y, sizeBinfont.Width, rightBinArearc.Height);


#if DEBUG
#pragma warning disable CS0162 // Unreachable code detected
                    if (false)
                    {
                        g.DrawRectangle(new Pen(Brushes.Orange), waferArearc.X, waferArearc.Y, waferArearc.Width, waferArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.Cyan), histoArearc.X, histoArearc.Y, histoArearc.Width, histoArearc.Height);

                        g.DrawRectangle(new Pen(Brushes.GreenYellow), leftInfoArearc.X, leftInfoArearc.Y, leftInfoArearc.Width, leftInfoArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.Crimson), rightInfoArearc.X, rightInfoArearc.Y, rightInfoArearc.Width, rightInfoArearc.Height);

                        g.DrawRectangle(new Pen(Brushes.Violet), binTitleArearc.X, binTitleArearc.Y, binTitleArearc.Width, binTitleArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.BlueViolet), leftBinArearc.X, leftBinArearc.Y, leftBinArearc.Width, leftBinArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.Purple), rightBinArearc.X, rightBinArearc.Y, rightBinArearc.Width, rightBinArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.DarkSalmon), rightBinPourcentageArearc.X, rightBinPourcentageArearc.Y, rightBinPourcentageArearc.Width, rightBinPourcentageArearc.Height);

                        g.DrawRectangle(new Pen(Brushes.Pink), colorMapArearc.X, colorMapArearc.Y, colorMapArearc.Width, colorMapArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.Green), leftStatArearc.X, leftStatArearc.Y, leftStatArearc.Width, leftStatArearc.Height);
                        g.DrawRectangle(new Pen(Brushes.Blue), rightStatArearc.X, rightStatArearc.Y, rightStatArearc.Width, rightStatArearc.Height);
                    }
#pragma warning restore CS0162 // Unreachable code detected
#endif // DEBUG

                    // area separation
                    g.DrawLine(whitePen, new PointF(xSepV, lineSepVMargin_px), new PointF(xSepV, nReportImgH - lineSepVMargin_px));
                    g.DrawLine(whitePen, new PointF(xSepV + lineSepHMargin_px, ySepH), new PointF(nReportImgW - lineSepHMargin_px, ySepH));

                    // colormap
                    using (Bitmap colormapbmp = new Bitmap(Resources.ColorMapGTR))
                    {
                        colormapbmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        g.DrawImage(colormapbmp, colorMapArearc);
                    }
                    g.DrawString($"{hazeppm_MaxUse:#0.00} ppm", fontRegular, whiteBrush, colorMapArearc.Right + allBorderMargin_px, colorMapArearc.Top);
                    g.DrawString($"{hazeppm_MinUse:#0.00} ppm", fontRegular, whiteBrush, colorMapArearc.Right + allBorderMargin_px, colorMapArearc.Bottom - fontRegular.Size);

                    StringBuilder sb = new StringBuilder(512);
                    sb.AppendLine($"Max");
                    sb.AppendLine($"Min");
                    sb.AppendLine($"Average");
                    sb.AppendLine($"Median");
                    g.DrawString(sb.ToString(), fontRegular, whiteBrush, leftStatArearc, strFormatAlignRight_C);

                    string sHazevalfmt = "#0.00";
                    sb.Clear();
                    sb.AppendLine($"{_hazeMeasure.Data.max_ppm.ToString(sHazevalfmt)} ppm");
                    sb.AppendLine($"{_hazeMeasure.Data.min_ppm.ToString(sHazevalfmt)} ppm");
                    sb.AppendLine($"{_hazeMeasure.Data.mean_ppm.ToString(sHazevalfmt)} ppm");
                    sb.AppendLine($"{_hazeMeasure.Data.median_ppm.ToString(sHazevalfmt)} ppm");
                    g.DrawString(sb.ToString(), fontBold, whiteBrush, rightStatArearc, strFormatAlignLeft_C);

                    sb.Clear();
                    sb.AppendLine($"Tool ID");
                    sb.AppendLine($"Lot ID");
                    sb.AppendLine($"Wafer ID");
                    sb.AppendLine($"Slot");
                    sb.AppendLine($"Date");
                    sb.AppendLine($"Time");
                    sb.AppendLine($"Wafer diameter");
                    sb.AppendLine();
                    sb.AppendLine($"Edge Exclusion");
                    sb.AppendLine($"Type");
                    sb.AppendLine($"ADC Recipe");
                    g.DrawString(sb.ToString(), fontRegular, whiteBrush, leftInfoArearc, strFormatAlignRight_Top);

                    sb.Clear();
                    sb.AppendLine($"{_toolID}");
                    sb.AppendLine($"{_ReportLotId}");
                    sb.AppendLine($"{_waferID}");
                    sb.AppendLine($"{_ReportSlotID}");
                    sb.AppendLine($"{_startProcessDate:yyyy-MM-dd}"); // ToString("yyyy - MM - dd--HH - mm - ss");
                    sb.AppendLine($"{_startProcessDate:HH:mm:ss}");
                    sb.AppendLine($"{_wafersize_mm} mm (" + ConvertMmToIntegerInch(_wafersize_mm) + "\'\')");
                    sb.AppendLine();
                    double edgeexl_mm = (double)_hazeMeasure.EdgeExlusion_um / 1000.0;
                    sb.AppendLine($"{edgeexl_mm:#0.00} mm");
                    sb.AppendLine($"{sType}");
                    sb.AppendLine($"{_adcRecipeName}");
                    g.DrawString(sb.ToString(), fontBold, whiteBrush, rightInfoArearc, strFormatAlignLeft_Top);

                    // Haze binning
                    g.DrawString("Haze binning", fontRegular, whiteBrush, binTitleArearc, strFormatAlignCenter_C);

                    // haze bin ranges 
                    int maxid = _hazeMeasure.Data.Ranges.Count;
                    sb.Clear();
                    sb.AppendLine($"Range ppm");
                    sb.AppendLine();
                    sb.AppendLine($"Below {_hazeMeasure.Data.Ranges[0].max_ppm}");
                    for (int i = 1; i < maxid - 1; i++)
                    {
                        sb.AppendLine($"{_hazeMeasure.Data.Ranges[i].min_ppm} - {_hazeMeasure.Data.Ranges[i].max_ppm}");
                    }
                    sb.AppendLine($"Above {_hazeMeasure.Data.Ranges[maxid - 1].min_ppm}");
                    g.DrawString(sb.ToString(), fontRegular, whiteBrush, leftBinArearc, strFormatAlignRight_C);

                    // haze bin pourcentage
                    sb.Clear();
                    sb.AppendLine($"Area %");
                    sb.AppendLine();
                    for (int i = 0; i < maxid; i++)
                    {
                        sb.AppendLine($"{_hazeMeasure.Data.Ranges[i].area_pct:#0.00}");
                    }
                    g.DrawString(sb.ToString(), fontBold, whiteBrush, rightBinPourcentageArearc, strFormatAlignRight_C);

                    //
                    // Draw Haze map display
                    // 

                    using (Bitmap bmp = new Bitmap(_hazeMeasure.Data.nWidth, _hazeMeasure.Data.nHeigth, PixelFormat.Format24bppRgb))
                    {
                        int nColoMaxIdx = _colorMapArray.Length;
                        double fColoMaxIdx = (float)nColoMaxIdx;
                        float a = (float)((double)nColoMaxIdx / (hazeppm_MaxUse - hazeppm_MinUse));
                        float b = (float)(-hazeppm_MinUse * (double)nColoMaxIdx / (hazeppm_MaxUse - hazeppm_MinUse));
                        unsafe
                        {
                            //lock the new bitmap in memory
                            BitmapData newData = bmp.LockBits(new Rectangle(0, 0, _hazeMeasure.Data.nWidth, _hazeMeasure.Data.nHeigth), ImageLockMode.WriteOnly, bmp.PixelFormat);

                            // get source bitmap pixel format size
                            int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat);
                            if (nDepth != 24)
                                throw new Exception("Haze Image map is not 24bpp aligned");

                            // Get color components count
                            int cCount = nDepth / 8;
                            fixed (float* pfDATA = _hazeMeasure.Data.HazeMeasures)
                            {
                                float* pStartArray = pfDATA;
                                Parallel.For(0, _hazeMeasure.Data.nHeigth, y =>
                                //for (int y = 0; y < _hazeMeasure.Data.nHeigth; y++)
                                {
                                    //get the data from the new image
                                    byte* pRow = (byte*)newData.Scan0 + (y * newData.Stride);
                                    float* pfRow = (float*)(pStartArray + (y * _hazeMeasure.Data.nWidth));

                                    for (int x = 0; x < _hazeMeasure.Data.nWidth; x++)
                                    {
                                        // convert float to index -- colormap
                                        float fVal = pfRow[0] * a + b;
                                        int nVal = Math.Min(Math.Max(0, (int)Math.Round(fVal)), nColoMaxIdx - 1);
                                        //set the new image's pixel with current color map
                                        pRow[0] = _colorMapArray[nVal].B;
                                        pRow[1] = _colorMapArray[nVal].G;
                                        pRow[2] = _colorMapArray[nVal].R;
                                        pRow += cCount;
                                        pfRow++;
                                    }
                                }); // Parallel.For
                            }

                            //unlock the bitmaps
                            bmp.UnlockBits(newData);

                            using (Graphics gMap = Graphics.FromImage(bmp))
                            {
                                float centerX = _hazeMeasure.Data.nWidth * 0.5f;
                                float centerY = _hazeMeasure.Data.nHeigth * 0.5f;
                                float radius_px = _wafersize_mm * 500.0f / _hazeMeasure.Data.fPixelSize_um - 1.0f;
                                gMap.DrawEllipse(whitePen5, centerX - radius_px, centerY - radius_px, 2.0f * radius_px, 2.0f * radius_px);
                            }
                        }
                        g.DrawImage(bmp, waferArearc);
                    }

                    var histo = new UInt32[_cstNbHistoStep];
                    int histoMaxIdx = histo.Length - 1;
                    double histoMin = hazeppm_MinUse;
                    double histoMax = hazeppm_MaxUse;
                    double histoA = (double)histo.Length / (histoMax - histoMin);
                    double histoB = -histoMin * (double)histo.Length / (histoMax - histoMin);
                    double histoStep = (histoMax - histoMin) / (double)histoMaxIdx;
                    UInt32 Totalcount = 0;
                    foreach (var ppmValue in _hazeMeasure.Data.HazeMeasures)
                    {
                        if (ppmValue > 0.0f && !float.IsNaN(ppmValue))
                        {
                            // convert float to index -- histo
                            int nIdx = Math.Min(Math.Max(0, (int)Math.Floor(ppmValue * histoA + histoB)), histoMaxIdx);
                            histo[nIdx]++;
                            Totalcount++;
                        }
                    }

                    var histoPercent = new double[_cstNbHistoStep];
                    if (Totalcount > 0)
                    {
                        for (int i = 0; i < histo.Length; i++)
                        {
                            histoPercent[i] = 100.0 * (double)histo[i] / (double)Totalcount;
                        }
                    }
                    double dMaxPercent = histoPercent.Max();

                    //
                    // Draw Haze map display
                    // 

                    float ratioHist = 1.0f;
                    int nHistW = 1265;
                    int nHistH = 268;

                    using (Bitmap histobmp = new Bitmap(nHistW, nHistH, PixelFormat.Format24bppRgb))
                    using (Graphics ghist = Graphics.FromImage(histobmp))
                    using (var bkgHistBrush = new SolidBrush(Color.FromArgb(15, 15, 15)))
                    using (var bkgLineBrush = new SolidBrush(Color.FromArgb(40, 40, 40)))
                    using (Pen bkgPenLine = new Pen(bkgLineBrush, 1.0f))
                    using (Pen whitePenHistThin = new Pen(whiteBrush, 1.0f))
                    using (Pen whitePenHistLarge = new Pen(whiteBrush, 2.0f))
                    using (Font fontScale = new System.Drawing.Font("Calibri Light", 16.0f, FontStyle.Regular))
                    {
                        float margRatioX = 0.05f;
                        float margRatioY = 0.05f;
                        float margEndRatioY = 0.90f;
                        float margEndRatioX = 0.95f;
                        float areaW = (margEndRatioX - margRatioX) * nHistW;
                        float areaH = (margEndRatioY - margRatioY) * nHistH;
                        PointF ptOrgin = new PointF(margRatioX * nHistW, margEndRatioY * nHistH);
                        PointF ptYaxisEnd = new PointF(margRatioX * nHistW, margRatioY * nHistH);
                        PointF ptXaxisEnd = new PointF(margEndRatioX * nHistW, margEndRatioY * nHistH);

                        //
                        // Axes Configuration
                        //
                        float decalOriginPpm = 2.0f; // offest on X axis of origin to avoid axees to mask some colorbar
                        int divisorX = 10;
                        double MaxPPMDisplay = FindScaleMax(histoMax);
                        double MinPPMDisplay = FindScaleMin(histoMin);
                        double PPMDisplayStep = (MaxPPMDisplay - MinPPMDisplay) / divisorX;
                        double Xpixelppmsize = areaW / (MaxPPMDisplay - MinPPMDisplay);
                        int divisorY = 10;
                        double maxpctDisplay = FindScaleMax(dMaxPercent);
                        double pctDisplayStep = maxpctDisplay / divisorY; // pct min always 0;
                        double Ypixelpctsize = areaH / maxpctDisplay;

                        // 
                        // Draw Histogram Bar Chart
                        //

                        // draw background rect
                        ghist.FillRectangle(bkgHistBrush, ptYaxisEnd.X, ptYaxisEnd.Y, areaW, areaH);
                        ghist.DrawRectangle(bkgPenLine, ptYaxisEnd.X, ptYaxisEnd.Y, areaW, areaH);

                        // draw colored bars
                        if (Totalcount > 0)
                        {
                            for (int i = 0; i < histo.Length; i++)
                            {
                                float xPos = ptOrgin.X + (float)(((double)i * histoStep + histoMin) * Xpixelppmsize) + decalOriginPpm * ratioHist;
                                float yPos = ptOrgin.Y - (float)(histoPercent[i] * Ypixelpctsize);
                                using (Pen colorBarPen = new Pen(_colorMapArray[i], 1.5f))
                                {
                                    ghist.DrawLine(colorBarPen, xPos, ptOrgin.Y, xPos, yPos);
                                }
                            }
                        }

                        //
                        // Draw Scales
                        //

                        // Draw axes
                        ghist.DrawLine(whitePenHistLarge, ptOrgin, ptXaxisEnd); // Axis X - ppm
                        ghist.DrawLine(whitePenHistLarge, ptOrgin, ptYaxisEnd); // Axis Y - % pourcentage 

                        // Scale X           
                        String fmtAxisX = StrFormatfromValue(PPMDisplayStep);
                        float yMargThindiv = 3.0f;
                        float yMargMLargediv = 6.0f;
                        for (int i = 0; i <= divisorX; i++)
                        {
                            double ppmVal = ((double)i * PPMDisplayStep);
                            float ppmPos = ptOrgin.X + (float)(ppmVal * Xpixelppmsize) + decalOriginPpm * ratioHist;
                            if (i % 2 == 0)
                            {
                                //Add legend mark
                                string sMark = ppmVal.ToString(fmtAxisX);
                                var sizefont = ghist.MeasureString(sMark, fontScale);
                                ghist.DrawString(sMark, fontScale, whiteBrush, new RectangleF(ppmPos - sizefont.Width * 0.5f, ptXaxisEnd.Y, sizefont.Width, sizefont.Height + 4.0f), strFormatAlignCenter_C);

                                //Large Mark
                                ghist.DrawLine(whitePenHistLarge, ppmPos, ptOrgin.Y - yMargMLargediv, ppmPos, ptOrgin.Y + yMargMLargediv);

                            }
                            else
                            {
                                //Small Mark
                                ghist.DrawLine(whitePenHistThin, ppmPos, ptOrgin.Y - yMargThindiv, ppmPos, ptOrgin.Y + yMargThindiv);
                            }
                        }
                        string sLastMark = "ppm";
                        var ppmsizefont = ghist.MeasureString(sLastMark, fontRegular);
                        ghist.DrawString(sLastMark, fontRegular, whiteBrush, new RectangleF(ptXaxisEnd.X + 5.0f, ptXaxisEnd.Y - ppmsizefont.Height * 0.7f, ppmsizefont.Width + 4.0f, ppmsizefont.Height + 2.0f), strFormatAlignLeft_C);

                        //Scale Y
                        String fmtAxisY = StrFormatfromValue(pctDisplayStep);
                        float xMargThindiv = 2.0f;
                        float xMargMLargediv = 4.0f;
                        for (int j = 0; j <= divisorY; j++)
                        {
                            double pctVal = ((double)j * pctDisplayStep);
                            float pctPos = ptOrgin.Y - (float)(pctVal * Ypixelpctsize);
                            if (j % 2 == 0)
                            {
                                //Add legend mark
                                string sMark = pctVal.ToString(fmtAxisY);
                                var sizefont = ghist.MeasureString(sMark, fontScale);
                                ghist.DrawString(sMark, fontScale, whiteBrush, new RectangleF(ptOrgin.X - sizefont.Width - 2.0f, pctPos - sizefont.Height * 0.5f, sizefont.Width, sizefont.Height + 2.0f), strFormatAlignRight_C);

                                //Large Mark
                                ghist.DrawLine(whitePenHistLarge, ptOrgin.X - xMargMLargediv, pctPos, ptOrgin.X + xMargMLargediv, pctPos);
                            }
                            else
                            {
                                //Small Mark
                                ghist.DrawLine(whitePenHistThin, ptOrgin.X - xMargThindiv, pctPos, ptOrgin.X + xMargThindiv, pctPos);
                            }
                        }
                        sLastMark = "%";
                        ppmsizefont = ghist.MeasureString(sLastMark, fontRegular);
                        ghist.DrawString(sLastMark, fontRegular, whiteBrush, new RectangleF(0.0f, 0.0f, ppmsizefont.Width, ppmsizefont.Height), strFormatAlignLeft_Top);

                        g.DrawImage(histobmp, histoArearc);
                    }

                    //
                    // Save image 
                    //
                    reportImg.Save(sAdditionnalReportImg, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                throw;
            }
        }

        static public Color[] InitColorMapRef()
        {
            Bitmap bmpClr;

            try
            {
                bmpClr = new Bitmap(Resources.ColorMapGTR);
            }
            catch (System.Exception ex)
            {
                string sMsg = $"Could not load Embedded HAZE LS Colormap : {ex.Message}\n{ex.StackTrace}";
                throw new Exception(sMsg);
            }

            int nbPaletteColors = bmpClr.Width;
            Color[] outColorMapRef = new Color[nbPaletteColors];
            unsafe
            {

                //lock the new bitmap in memory
                BitmapData newData = bmpClr.LockBits(new Rectangle(0, 0, bmpClr.Width, bmpClr.Height), ImageLockMode.WriteOnly, bmpClr.PixelFormat);
                int nDepth = System.Drawing.Bitmap.GetPixelFormatSize(bmpClr.PixelFormat);
                int cCount = nDepth / 8;
                byte* nRow = (byte*)newData.Scan0 + (0 * newData.Stride);
                Parallel.For(0, nbPaletteColors, i =>
                {
                    int a = 255;
                    int r = 0;
                    int g = 0;
                    int b = 0;

                    if (nDepth == 32) // For 32 bpp set Red, Green, Blue and Alpha
                    {
                        b = nRow[i * cCount];
                        g = nRow[i * cCount + 1];
                        r = nRow[i * cCount + 2];
                        a = nRow[i * cCount + 3];
                    }
                    else if (nDepth == 24) // For 24 bpp set Red, Green and Blue
                    {
                        b = nRow[i * cCount];
                        g = nRow[i * cCount + 1];
                        r = nRow[i * cCount + 2];
                    }
                    else if (nDepth == 8) // For 8 bpp set color value (Red, Green and Blue values are the same)
                    {
                        b = g = r = nRow[i * cCount];
                    }
                    outColorMapRef[i] = Color.FromArgb(a, r, g, b);
                }); // Parallel.For
                //unlock the bitmaps
                bmpClr.UnlockBits(newData);

            }
            bmpClr.Dispose();
            return outColorMapRef;
        }


        public static double FindScaleMax(double dValue)
        {
            if (dValue <= 0.0)
                return 0.0;

            const double cst_stepRatioDecade = 0.05; // we look for the closest superior largest decade (ex: 0.75 => 0.75; 8.2 => 8.5;  26 => 30; 235 => 250; 4999.8 =>5000; 5001 => 5500
            double decade10 = Math.Pow(10, Math.Ceiling(Math.Log10(dValue)));
            double normalizedDecade10 = dValue / decade10;
            double MaxScale = Math.Ceiling(normalizedDecade10 / cst_stepRatioDecade) * cst_stepRatioDecade * decade10;
            return MaxScale;
        }

        public static double FindScaleMin(double dValue)
        {
            if (dValue <= 0.0)
                return 0.0;

            const double cst_stepRatioDecade = 0.05; // we look for the closest superior largest decade (ex: 0.75 => 0.75; 8.2 => 8.5;  26 => 30; 235 => 250; 4999.8 =>5000; 5001 => 5500
            double decade10 = Math.Pow(10, Math.Ceiling(Math.Log10(dValue)));
            double normalizedDecade10 = dValue / decade10;
            double MinScale = Math.Floor(normalizedDecade10 / cst_stepRatioDecade) * cst_stepRatioDecade * decade10;
            return MinScale;
        }

        public static string StrFormatfromValue(double dVal)
        {
            string sfmt = "#0";
            if (dVal == 0.0)
                return sfmt;

            int nbZero = (int)Math.Floor(Math.Log10(dVal));
            if (nbZero < 0)
            {
                sfmt += ".";
                for (int p = 0; p < Math.Abs(nbZero); p++)
                    sfmt += "0";
            }
            return sfmt;
        }
    }
}
