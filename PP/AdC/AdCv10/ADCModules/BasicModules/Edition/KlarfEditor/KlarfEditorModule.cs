using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

using AcquisitionAdcExchange;

using AdcBasicObjects;

using ADCEngine;

using AdcTools;

using BasicModules.BorderRemoval;
using BasicModules.DataLoader;
using BasicModules.Edition.DataBase;
using BasicModules.Edition.DummyDefect;
using BasicModules.Edition.KlarfEditor;
using BasicModules.Edition.Rendering;
using BasicModules.Edition.Rendering.Message;

using CommunityToolkit.Mvvm.Messaging;

using Format001;

using LibProcessing;

using UnitySC.Shared.Tools;

using FontStyle = System.Drawing.FontStyle;

namespace BasicModules.KlarfEditor
{


    ///////////////////////////////////////////////////////////////////////
    //
    ///////////////////////////////////////////////////////////////////////
    public class KlarfEditorModule : DatabaseEditionModule
    {
        protected readonly String[] DefectTypeCategories =
        {
            "DEFECTID",
            "XREL",
            "YREL",
            "XINDEX",
            "YINDEX",
            "XSIZE",
            "YSIZE",
            "DEFECTAREA",
            "DSIZE",
            "CLASSNUMBER",
            "TEST",
            "CLUSTERNUMBER",
            "ROUGHBINNUMBER",
            "FINEBINNUMBER",
            "REVIEWSAMPLE",
            "IMAGECOUNT",
            "IMAGELIST"
        };

        protected DataKlarf DataKlarf;

        public DataKlarf MyDataKlarf
        {
            get
            {
                return DataKlarf;
            }
        }

        protected static ProcessingClass ProcessingClass = new ProcessingClassMil3D();
        public object Syncklarf;
        public PathString KlarfFilename { get; protected set; }

        protected Bitmap MultiTifImg = null;
        protected EncoderParameters MultiEncoderParameters = null;
        protected int NbMultitifPage = 0;
        protected string MultiTmpt02 = String.Empty;
        protected bool ErrorInMultiTiff;

        //=================================================================
        // Paramètres du XML
        //=================================================================
        public readonly BoolParameter ParamMultiTiff;
        public readonly BoolParameter ParamMultiTiffWithBinary;
        public readonly ConditionalIntParameter ParamDefectCountLimit;
        [ExportableParameter(false)]
        public readonly EnumParameter<PrmOrientationInstruction.oiType> ParamOrientationInstruction;
        public readonly EnumParameter<PrmOrientationMarkValue.omvType> ParamOrientationMark;
        public readonly BoolParameter ParamAdditionnalReport;
        [ExportableParameter(false)]
        public readonly IntParameter  ParamInspectionTest;
        public readonly BoolParameter ParamCenteringReport;
        public readonly BoolParameter ParamCoordinatesMirrored;
        public readonly BoolParameter ParamShiftedCoordinates;
        public readonly BoolParameter ParamCompactIndex;
        public readonly KlarfEditorRoughBinParameter ParamRoughBins;
        public readonly KlarfDefectColorParameters ParamDefectColor;


        public double WaferAngleDegree;
        public double WaferXShiftum;
        public double WaferYShiftum;
        public int UseCorrector;



        protected int LastClusterIndex = 0;

        // Requested for Edition and registration matters
        protected override List<int> RegisteredResultTypes()
        {
            List<int> Rtypes = new List<int>(1);
            Rtypes.Add((int)ResultTypeFile.Defect_001);
            return Rtypes;
        }

        //=================================================================
        // Constructeur
        //=================================================================
        public KlarfEditorModule(IModuleFactory factory, int id, Recipe recipe)
            : base(factory, id, recipe)
        {
            ParamMultiTiff = new BoolParameter(this, "MultiTiff");
            ParamMultiTiff.ValueChanged += (b) => { ParamMultiTiffWithBinary.IsEnabled = b; };
            ParamMultiTiffWithBinary = new BoolParameter(this, "WithBinaryPictures");
            ParamCenteringReport = new BoolParameter(this, "CenteringReport");
            ParamCompactIndex = new BoolParameter(this, "CompactClusterIndexes");
            ParamCoordinatesMirrored = new BoolParameter(this, "CoordinatesMirrored");
            ParamInspectionTest = new IntParameter(this, "Inspection Test");
            ParamInspectionTest.Value = 1;

            ParamOrientationInstruction = new EnumParameter<PrmOrientationInstruction.oiType>(this, "OrientationInstruction");
            ParamOrientationMark = new EnumParameter<PrmOrientationMarkValue.omvType>(this, "OrientationMark");
            
            ParamOrientationInstruction.ValueChanged += ChangeLabelSuffixForOrientation;
            ParamResultLabelSuffix.String = ParamResultLabelSuffix.String.Insert(0, GetSuffixLead(ParamOrientationInstruction.Value));

            if (!(Application.Current is null))
            {
                // Application.Current is null when running in AdaToAdc mode, visibility needn't be changed as there is no UI
                // Necessary for correct result handling in DB, setting must be hidden from UI
                ParamResultIndex.ParameterUI.Visibility = Visibility.Collapsed;    
            }
            
            ParamResultIndex.IsUsed = true;
            ParamOrientationInstruction.ValueChanged += ChangeResultIndexForOrientation;

            // Defect Count Limit
            ParamShiftedCoordinates = new BoolParameter(this, "CoordinatesShifted");
            ParamDefectCountLimit = new ConditionalIntParameter(this, "DefectCountLimit");
            ParamDefectCountLimit.Value = 20000;
            ParamDefectCountLimit.IsUsed = false; // paramétrer true/false

            // Additional report

            ParamAdditionnalReport = new BoolParameter(this, "AdditionnalReport");
            ParamAdditionnalReport.ValueChanged += (b) =>
            {
                ParamDefectColor.IsEnabled = b;
            };
            ParamDefectColor = new KlarfDefectColorParameters(this, "DefectLabel");
            ParamRoughBins = new KlarfEditorRoughBinParameter(this, "DefectRoughBins");

            Syncklarf = new object();
        }

        private void ChangeResultIndexForOrientation(PrmOrientationInstruction.oiType enumValue)
        {
            switch (enumValue)
            {
                case PrmOrientationInstruction.oiType.FRONT:
                    ParamResultIndex.Value = 0;
                    break;
                case PrmOrientationInstruction.oiType.BACK:
                    ParamResultIndex.Value = 1;
                    break;
                case PrmOrientationInstruction.oiType.BEVEL:
                    ParamResultIndex.Value = 2;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeLabelSuffixForOrientation(PrmOrientationInstruction.oiType enumValue)
        {
            if (ParamResultLabelSuffix.String.StartsWith("-FS") || ParamResultLabelSuffix.String.StartsWith("-BS") ||
                ParamResultLabelSuffix.String.StartsWith("-BV"))
            {
                ParamResultLabelSuffix.String = ParamResultLabelSuffix.String.Remove(0, 3).Insert(0, GetSuffixLead(enumValue));
            }
        }

        private static string GetSuffixLead(PrmOrientationInstruction.oiType enumValue)
        {
            switch (enumValue)
            {
                case PrmOrientationInstruction.oiType.FRONT:
                    return "-FS";
                case PrmOrientationInstruction.oiType.BACK:
                    return "-BS";
                case PrmOrientationInstruction.oiType.BEVEL:
                    return "-BV";
                default:
                    return "";
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnInit()
        {
            base.OnInit();

            InitCorrector();

            IEnumerable<ParameterBase> additionalReportParam = ParameterList.Where(x => x.Label == "AdditionnalReport");
            if (additionalReportParam.Count() > 0)
            {
                BoolParameter additionalReportRecipe;
                additionalReportRecipe = (BoolParameter)additionalReportParam.First();

                ParamAdditionnalReport.Value = additionalReportRecipe.Value;
            }
            ParamDefectColor.IsEnabled = ParamAdditionnalReport.Value;

            //// Obsolete PLSEditor //need refacto fuul feature
            //_PlsAncestor = FindAncestors(mod => mod is PLSEditorModule).FirstOrDefault() != null;
            //if (_PlsAncestor) {  ResultFileBaseName += "_PLS"; }

            KlarfFilename = GetResultFullPathName(ResultTypeFile.Defect_001);

            Wafer.waferInfo[AcquisitionAdcExchange.eWaferInfo.KlarfFileName] = KlarfFilename.Filename;
            DateTime dtNow = DateTime.Now;

            DataKlarf = new DataKlarf();
            string ADCRecipeFileName = Wafer.GetWaferInfo(eWaferInfo.ADCRecipeFileName);

            // // Original way to do in adc below, but some data are missing in ada or else...
            //var dbRecipeServiceProxy = ClassLocator.Default.GetInstance<DbRecipeServiceProxy>();
            //var dtorecipe = dbRecipeServiceProxy.GetLastRecipe(ADCRecipeFileName);
            UnitySC.DataAccess.Dto.Recipe dtorecipe = null; // en attendant les info de guid & version dans l'ada on retouner recette null pour avancer

            DataKlarf.SetupID = new PrmSetupId(Wafer.GetWaferInfo(eWaferInfo.ToolRecipe), dtorecipe?.Created ?? dtNow);

            EdgeRemoveModule EdgeRemove = (EdgeRemoveModule)FindAncestors(mod => mod is EdgeRemoveModule).FirstOrDefault();
            DataKlarf.edgeSize_um = EdgeRemove?.paramMargin.Value ?? 0.0;

            DataKlarf.ResultTimeStamp = dtNow;

            // "UNITY-SC" "ToolCategory" "ToolName"
            DataKlarf.InspectionStationID = String.Format("\"{0}\" \"{1}\" \"{2}\"", "UNITYSC", "4See", Recipe.Toolname);

            DataKlarf.LotID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.LotID));
            DataKlarf.EquipmentID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.EquipmentID));
            DataKlarf.DeviceID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.DeviceID));
            DataKlarf.WaferID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.WaferID));
            DataKlarf.StepID = String.Format("{0}", Wafer.GetWaferInfo(eWaferInfo.StepID));


            int nSlotId;
            if (int.TryParse(Wafer.GetWaferInfo(eWaferInfo.SlotID), out nSlotId))
                DataKlarf.SlotID = nSlotId;

            int nSampleSize_mm = 0;
            if (Wafer is NotchWafer)
            {
                nSampleSize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                nSampleSize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                nSampleSize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));

                WriteSqrFile();
            }

            DataKlarf.SampleSize = new PrmSampleSize(nSampleSize_mm);

            DataKlarf.DiePitch = new PrmPtFloat((float)nSampleSize_mm * 1000, (float)nSampleSize_mm * 1000); // wafer size x/y

            DataKlarf.SampleOrientationMarkType = GetWaferOrientationMark();
            DataKlarf.OrientationMarkLocation = new PrmOrientationMarkValue(ParamOrientationMark.Value.ToString());
            DataKlarf.OrientationInstructions = new PrmOrientationInstruction(ParamOrientationInstruction.Value.ToString());
            DataKlarf.CoordinatesMirrored = new PrmYesNo(ParamCoordinatesMirrored.Value);
            DataKlarf.CoordinatesCentered = new PrmYesNo(ParamCenteringReport.Value);
            DataKlarf.InspectionTest = ParamInspectionTest.Value;

            DataKlarf.SampleTestPlan = new PrmSampleTestPlan(0, 0);

            if (ParamShiftedCoordinates.Value == true)
                DataKlarf.SampleCenterLocation = new PrmPtFloat((float)0.0, (float)0.0); // bottom left corner
            else 
                DataKlarf.SampleCenterLocation = new PrmPtFloat((float)DataKlarf.SampleSize.waferDiameter_mm * 500.0f, (float)DataKlarf.SampleSize.waferDiameter_mm * 500.0f); // wafer size x/y  * 2

            DataKlarf.AddDefectTypes(DefectTypeCategories); // warning defect type should be registered or you will have to declare and defined type by yourself using DataKlarf.AddDefectType(Type p_type, String p_name)

            if (ParamMultiTiff.Value)
            {
                // need only file name not complete path
                DataKlarf.TiffFileName = Path.GetFileName(KlarfFilename.path.Replace(GetKlarfExt(), ".t01"));
            }

            ErrorInMultiTiff = false;

        }

        private void InitCorrector()
        {
            UseCorrector = 0; // no correction
            if (Wafer == null)
                return;

            string sField = eWaferInfo.CorrectorsEnabled.ToString();
            try
            {
                var sVal = Wafer.GetWaferInfo(eWaferInfo.CorrectorsEnabled);
                if (string.IsNullOrEmpty(sVal))
                {
                    logWarning("No <CorrectorsEnabled> field in wafer Info, correction will be disabled");
                    return;
                }
                UseCorrector = System.Convert.ToInt32(sVal);
                if (UseCorrector != 0)
                {
                    sField = eWaferInfo.WaferAngleDegrees.ToString();
                    WaferAngleDegree = System.Convert.ToDouble(Wafer.GetWaferInfo(eWaferInfo.WaferAngleDegrees));
                    sField = eWaferInfo.WaferXShiftum.ToString();
                    WaferXShiftum = System.Convert.ToDouble(Wafer.GetWaferInfo(eWaferInfo.WaferXShiftum));
                    sField = eWaferInfo.WaferYShiftum.ToString();
                    WaferYShiftum = System.Convert.ToDouble(Wafer.GetWaferInfo(eWaferInfo.WaferYShiftum));
                }
                sField = String.Empty;
            }
            catch (Exception ex) 
            {
                logError($"Cannot Init Corrections (miss <{sField}>), correction will be disabled : {ex.Message}");
                UseCorrector = 0;
            }
            finally 
            {
                if (UseCorrector != 0)
                    log($"Correction Enabled : shfX={WaferXShiftum} µm; shfY={WaferYShiftum} µm; cAng={WaferAngleDegree} °;");
            }
        }

        //=================================================================
        // 
        //=================================================================
        protected void AddMultiTiff_Image(System.Drawing.Image p_Img)
        {
            if (ErrorInMultiTiff)
                return;

            if (p_Img == null)
                throw new ApplicationException(" Try to add empty page to MultiTiff Image");

            try
            {
                if (MultiTifImg == null)
                {
                    // add first image - init 

                    MultiTmpt02 = KlarfFilename.path.Replace(GetKlarfExt(), ".t02");
                    string parentDirectory = Path.GetDirectoryName(MultiTmpt02);
                    if (!parentDirectory.IsNullOrEmpty() && !Directory.Exists(parentDirectory))
                    {
                        Directory.CreateDirectory(parentDirectory);
                    }
                    if (File.Exists(MultiTmpt02))
                        File.Delete(MultiTmpt02);

                    MultiTifImg = (Bitmap)p_Img.Clone();

                    // ici MIME="image/tiff" pourrait être
                    // "image/tiff"  ou
                    // "image/Jpeg"  etc suivant le type de fichier de sortie désiré
                    ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");

                    MultiEncoderParameters = new EncoderParameters(3);
                    MultiEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                    MultiEncoderParameters.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionNone);
                    MultiEncoderParameters.Param[2] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, (long)8);

                    MultiTifImg.Save(MultiTmpt02, myImageCodecInfo, MultiEncoderParameters);
                    Interlocked.Increment(ref NbMultitifPage);

                    // prepare next frame encoder parameters
                    MultiEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                }
                else
                {
                    // add image frame
                    MultiTifImg.SaveAdd(p_Img, MultiEncoderParameters);
                    Interlocked.Increment(ref NbMultitifPage);
                }
            }
            catch (Exception ex)
            {
                ErrorInMultiTiff = true;
                logWarning("Error during multi tiff creation (file too big ? ) " + ex.ToString());
            }
        }

        protected void CloseMultiTiff_Image(bool bAbort = false)
        {
            if (MultiTifImg == null)
                return;

            // close / free multitif 
            if (!bAbort)
            {
                MultiEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.SaveFlag, (long)EncoderValue.Flush);
                MultiTifImg.SaveAdd(MultiEncoderParameters);
            }

            MultiEncoderParameters.Dispose();
            MultiEncoderParameters = null;

            MultiTifImg.Dispose();
            MultiTifImg = null;

            // and rename file
            if (!bAbort)
            {
                String sOutpuFile = KlarfFilename.path.Replace(GetKlarfExt(), ".t01");
                if (File.Exists(sOutpuFile))
                    File.Delete(sOutpuFile);
                File.Move(MultiTmpt02, sOutpuFile);
            }
        }

        static protected ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            // ici MIME="image/tiff" pourrait être
            // "image/tiff"  ou
            // "image/Jpeg"  etc
            int j;
            ImageCodecInfo[] encoders;

            // contient des informations sur l'un des encodeurs d'images intégrés.
            encoders = ImageCodecInfo.GetImageEncoders();

            j = 0;
            while (j < encoders.Length)
            {
                // Chaîne qui contient le type MIME (Multipurpose Internet Mail Extensions) du codec.
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];   // et sort de la bouble for .. next
                j = j + 1;
            }
            return null;
        }

        //=================================================================
        // 
        //=================================================================
        public override void Process(ModuleBase parent, ObjectBase obj)
        {
            logDebug("process " + obj);
            Interlocked.Increment(ref nbObjectsIn);

            //-------------------------------------------------------------
            // Récuperation du cluster
            //-------------------------------------------------------------
            Cluster cluster = (Cluster)obj;
            if (cluster == null || cluster.blobList == null || cluster.blobList.Count == 0)
                throw new ApplicationException("Empty cluster");
            if (ParamCompactIndex.Value == true)
            {
                if (cluster.blobList.Count <= 1)   // pour IBM permet de ne pas avoir de trou dans les index de clusters (IBM limité à 32000)
                    cluster.Index = 0;
            }
            LastClusterIndex = Math.Max(cluster.Index, LastClusterIndex);
            int nClusterRoughBin = 0;
            if (ParamRoughBins.RoughBins.ContainsKey(cluster.DefectClass))
            {
                nClusterRoughBin = ParamRoughBins.RoughBins[cluster.DefectClass].RoughBinNum;
            }
            List<PrmDefect> ltmp = CreatePrmDefectList(cluster);

            //-------------------------------------------------------------
            // Update Klarf data defect list 
            //-------------------------------------------------------------
            lock (Syncklarf)
            {
                if ((!ParamDefectCountLimit.IsUsed) || (DataKlarf.NDEFECT < ParamDefectCountLimit.Value))
                {
                    //int nLastDefectId = cluster.blobList.Count <= 1 ? 0 : _dataKlarf.NDEFECT + 1;
                    int nLastDefectId = DataKlarf.NDEFECT + 1;

                    PrmImageData imgData = new PrmImageData();

                    if ((ParamMultiTiff.Value) && (cluster.DefectClass != "Pls-Remove"))
                        SaveThumbnails(cluster, imgData);

                    for (int i = 0; i < ltmp.Count; i++)
                    {
                        ltmp[i].Set("DEFECTID", nLastDefectId + i + 1);
                        ltmp[i].Set("IMAGECOUNT", imgData.List.Count);
                        ltmp[i].Set("IMAGELIST", imgData);
                        DataKlarf.AddDefect(ltmp[i]);
                    }
                }
            }

            if (ltmp.Any())
            {
                if ((!ParamDefectCountLimit.IsUsed) || (DataKlarf.NDEFECT < ParamDefectCountLimit.Value))
                {
                    using (KlarfCluster klarfCluster = CreateKlarfCluster(ltmp, cluster.DefectClass, nClusterRoughBin, (double)cluster.characteristics[SizingCharacteristics.TotalDefectSize]))
                    {
                        ProcessChildren(klarfCluster);
                    }
                    SendKlarfResult();
                }
            }
        }

        protected List<PrmDefect> CreatePrmDefectList(Cluster cluster)
        {
            int nClusterNum = cluster.Index;
            int nClusterRoughBin = 0;
            int nClusterFineBin = 0;
            int nClusterClassNumber = 0;
            if (ParamRoughBins.RoughBins.ContainsKey(cluster.DefectClass))
            {
                nClusterRoughBin = ParamRoughBins.RoughBins[cluster.DefectClass].RoughBinNum;
                nClusterFineBin = ParamRoughBins.RoughBins[cluster.DefectClass].FineBinNum;
                nClusterClassNumber = ParamRoughBins.RoughBins[cluster.DefectClass].ClassNumber;
            }

            bool bUseCenterRect = ParamCenteringReport.Value;

            List<PrmDefect> ltmp = new List<PrmDefect>(Math.Max(1, cluster.blobList.Count));
            foreach (Blob blb in cluster.blobList)
            {
                PrmDefect curDefect = DataKlarf.NewDefect();
                curDefect.SurroundingRectangleMicron = blb.micronQuad.SurroundingRectangle;

                RectangleF rect_um = blb.micronQuad.SurroundingRectangle;
                rect_um.Offset(DataKlarf.SampleCenterLocation.x, DataKlarf.SampleCenterLocation.y);

                if (UseCorrector == 1)
                {
                    // apply corrector
                    rect_um = ApplyCorrection(rect_um);
                }

                double corX = 0.0;
                double corY = 0.0;

                if (bUseCenterRect)
                {
                    PointF Mid = rect_um.Middle();
                    corX = (double)Mid.X;
                    corY = (double)Mid.Y;
                }
                else
                {
                    //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                    corX = (double)rect_um.Left;
                    corY = (double)rect_um.Bottom;                 
                }

                curDefect.Set("XREL", corX);
                curDefect.Set("YREL", corY);

                curDefect.Set("XSIZE", (double)blb.characteristics[SizingCharacteristics.sizeX]);
                curDefect.Set("YSIZE", (double)blb.characteristics[SizingCharacteristics.sizeY]);

                double area = (double)blb.characteristics[SizingCharacteristics.DefectArea];
                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", (double)blb.characteristics[SizingCharacteristics.DSize]);

                curDefect.Set("CLASSNUMBER", nClusterClassNumber);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", nClusterNum);

                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le roughbin bon doit être cherché en fonction du type de classification ?
                curDefect.Set("FINEBINNUMBER", nClusterFineBin);
                ltmp.Add(curDefect);
            }
            return ltmp;
        }

        private RectangleF ApplyCorrection(RectangleF defectRect)
        {
            // Note de RTI : pourquoi cette correction est appliquer seuleument ici et pas au niveau des matrixBase (AffineMatrix ...) 

            // Only defect rect center is transform to be conservative for thumbnail image and theirs size
            RectangleF recDef = defectRect;
            // translation from referencial origin to [0,0]
            recDef.Offset(-1.0f *DataKlarf.SampleCenterLocation.x, -1.0f * DataKlarf.SampleCenterLocation.y);
            // Apply wafer shift // important prior to angle rotation
            var midX = (double)(recDef.Left + recDef.Right) * 0.5;
            var midY = (double)(recDef.Top + recDef.Bottom) * 0.5;
            var shiftX = midX + WaferXShiftum;
            var shiftY = midY + WaferYShiftum;
            var angle_rd = WaferAngleDegree * Math.PI / 180.0;         // à verifier orientation de la correction
            var midX2 = shiftX *  Math.Cos(angle_rd) + shiftY * Math.Sin(angle_rd);
            var midY2 = shiftX * -Math.Sin(angle_rd) + shiftY * Math.Cos(angle_rd);
            // apply coorection to rectangle center
            recDef.Offset((float)(midX2-midX), (float)(midY2-midY));

            // translation back to referencial origin
            recDef.Offset(DataKlarf.SampleCenterLocation.x, DataKlarf.SampleCenterLocation.y);

            return recDef;
        }

        protected void SaveThumbnails(Cluster cluster, PrmImageData imgData)
        {
            using (ProcessingImage image8bit = ProcessingClass.ConvertTo8bit(cluster.OriginalProcessingImage))
            using (Bitmap greyScaleSampleImg = image8bit.ConvertToBitmap())
            {
                AddMultiTiff_Image(greyScaleSampleImg);
                imgData.List.Add(NbMultitifPage);
            }

            if (ParamMultiTiffWithBinary.Value && !cluster.IsDummy)
            {
                using (ProcessingImage image8bit = ProcessingClass.ConvertTo8bit(cluster.ResultProcessingImage))
                using (Bitmap BinarySampleImg = image8bit.ConvertToBitmap())
                {
                    AddMultiTiff_Image(BinarySampleImg);
                    imgData.List.Add(NbMultitifPage);
                }
            }
        }

        public KlarfCluster CreateKlarfCluster(List<PrmDefect> prmDefects, string className, int roughBinNum, double totalDefectSize)
        {
            KlarfCluster klarfCluster = new KlarfCluster();
            klarfCluster.ClassName = className;
            klarfCluster.RoughBinNum = roughBinNum;
            klarfCluster.TotalDefectSize = totalDefectSize;
            klarfCluster.KlarfDefects = prmDefects.Select(curDefect => new KlarfDefect()
            {
                DefectNumber = (int)curDefect.Get("DEFECTID"),
                ClusterNumber = (int)curDefect.Get("CLUSTERNUMBER"),
                Area = (double)curDefect.Get("DEFECTAREA"),
                PosX = (double)curDefect.Get("XREL"),
                PosY = (double)curDefect.Get("YREL")
            })
            .ToList();
            return klarfCluster;
        }

        //=================================================================
        // 
        //=================================================================
        protected override void OnStopping(eModuleState oldState)
        {
            logDebug("parent stopped, starting processing task");

            Scheduler.StartSingleTask("ProcessKlarf", () =>
            {
                try
                {
                    ProcessKlarf();
                }
                catch (Exception ex)
                {
                    RegisterResultInDatabase(ResultTypeFile.Defect_001, ResultState.Error);
                    string msg = "klarf generation failed: " + ex.Message;
                    HandleException(new ApplicationException(msg, ex));
                }
                finally
                {
                    base.OnStopping(oldState);
                }
            });
        }

        protected void AddDummyDefetcts()
        {
            List<Cluster> dummyClusters = FindAncestors(x => x is DummyDefectModule).OfType<DummyDefectModule>().Where(x => x.ClusterResult != null).Select(x => x.ClusterResult).ToList();
            bool bUseCenterRect = ParamCenteringReport.Value;

            foreach (Cluster cluster in dummyClusters)
            {
                LastClusterIndex++;
                PrmDefect curDefect = DataKlarf.NewDefect();
                RectangleF rect_um = cluster.micronQuad.SurroundingRectangle;
                rect_um.Offset(DataKlarf.SampleCenterLocation.x, DataKlarf.SampleCenterLocation.y);
                double area = Math.Abs(rect_um.Area());
                int nClusterRoughBin = 0;
                int nClusterFineBin = 0;
                if (ParamRoughBins.RoughBins.ContainsKey(cluster.DefectClass))
                {
                    nClusterRoughBin = ParamRoughBins.RoughBins[cluster.DefectClass].RoughBinNum;
                    nClusterFineBin = ParamRoughBins.RoughBins[cluster.DefectClass].FineBinNum;
                }

                if (bUseCenterRect)
                {
                    curDefect.Set("XREL", (double)rect_um.Left);
                    curDefect.Set("YREL", (double)0);
                }
                else
                {
                    //-- Orign blob BOTTOM LEFT of rect -- In viewer rect is defined like that
                    curDefect.Set("XREL", (double)rect_um.Left);
                    curDefect.Set("YREL", (double)rect_um.Bottom);
                }

                curDefect.Set("XSIZE", (double)rect_um.Width);
                curDefect.Set("YSIZE", (double)rect_um.Height);
                curDefect.Set("DEFECTAREA", area);
                curDefect.Set("DSIZE", Math.Sqrt(area));
                curDefect.Set("CLASSNUMBER", 0);
                curDefect.Set("TEST", 1);
                curDefect.Set("CLUSTERNUMBER", LastClusterIndex);
                curDefect.Set("ROUGHBINNUMBER", nClusterRoughBin); // le roughbin bon doit être cherché en fonction du type de classification ?             
                curDefect.Set("FINEBINNUMBER", nClusterFineBin);


                //-------------------------------------------------------------
                // Update Klarf data defect list 
                //-------------------------------------------------------------
                lock (Syncklarf)
                {
                    int nLastDefectId = DataKlarf.NDEFECT;

                    PrmImageData imgData = new PrmImageData();
                    if (ParamMultiTiff.Value)
                    {
                        using (ProcessingImage image8bit = ProcessingClass.ConvertTo8bit(cluster.OriginalProcessingImage))
                        using (Bitmap greyScaleSampleImg = image8bit.ConvertToBitmap())
                        {
                            AddMultiTiff_Image(greyScaleSampleImg);
                            imgData.List.Add(NbMultitifPage);
                        }
                    }

                    curDefect.Set("DEFECTID", nLastDefectId + 1);
                    curDefect.Set("IMAGECOUNT", imgData.List.Count);
                    curDefect.Set("IMAGELIST", imgData);
                    DataKlarf.AddDefect(curDefect);
                }
            }
        }

        //=================================================================
        //
        //=================================================================
        protected virtual void ProcessKlarf()
        {
            // Dummy defects
            //..............
            AddDummyDefetcts();

            // Close Multitiff
            //................
            CloseMultiTiff_Image();

            // écriture du klarf
            //..................
            if (State == eModuleState.Aborting)
            {
                logDebug("klarf generation aborted");
                RegisterResultInDatabase(ResultTypeFile.Defect_001, ResultState.Error);
            }
            else
            {
                DataKlarf.FileTimeStamp = DateTime.Now;



                log("Creating klarf: " + KlarfFilename);
                if (!Directory.Exists(KlarfFilename.Directory))
                {
                    Directory.CreateDirectory(KlarfFilename.Directory);
                }
                KlarfFile.Write(KlarfFilename.ToString(), DataKlarf);


                string carrierStatus = Wafer.GetWaferInfo(eWaferInfo.CarrierStatus);
                if ((carrierStatus == "2") || (carrierStatus == "3"))
                    KlarfFile.WriteLotStatusFile(GetlotStatusFileName(DataKlarf.OrientationInstructions.ToString()), DataKlarf, DestinationDirectory);

                Dictionary<eWaferInfo, string> info = Wafer.waferInfo;
                info[eWaferInfo.TotalDefectCount] = DataKlarf.NDEFECT.ToString();

                ResultState resstate = ResultState.Ok; // TO DO -- check grading reject , rework if exist, or partial result
                if (State == eModuleState.Aborting)
                    resstate = ResultState.Error;
                RegisterResultInDatabase(ResultTypeFile.Defect_001, resstate);
            }
            // écriture de l'additional report
            //..................
            if (State == eModuleState.Aborting)
            {

            }
            else
            {
                if (ParamAdditionnalReport)
                {
                    CreateAdditionnalReport();
                }
            }

        }

        //=================================================================
        //
        //=================================================================
        private void CreateAdditionnalReport()
        {
            String sFullKlarfPath = KlarfFilename;
            CWaferResult oWaferRes = null;
            int roughbinArea = 999999;

            String sDB_LotID = Recipe.Wafer.GetWaferInfo(eWaferInfo.LotID);
            String sDB_RecipeID = Recipe.Wafer.GetWaferInfo(eWaferInfo.ToolRecipe);
            String sDB_UniqueID = Recipe.Wafer.GetWaferInfo(eWaferInfo.Basename);
            String sDB_SlotID = Recipe.Wafer.GetWaferInfo(eWaferInfo.SlotID);

            //string slotId =_dataKlarf.LotID;
            //string sRecipe = _dataKlarf.StepID; //valueToolRecipeName;
            //string sWFRId = _dataKlarf.WaferID;
            //string sLotId = _dataKlarf.SlotID.ToString();

            // On génere l'image d'additionnal report selon le format --> Après test, 200 ppi est conservée
            int nReportImgW = 0; int nReportImgH = 0;
            nReportImgW = 2339; nReportImgH = 1654; //200PPi
            float res = 2.0f; //200ppi 

            // read klarf
            DateTime fileModificationDate = File.GetLastWriteTime(sFullKlarfPath);
            oWaferRes = new CWaferResult(sFullKlarfPath, roughbinArea);
            if (!oWaferRes.IsLoaded())
            {
                HandleException(new ApplicationException("Not able to load klarf file (" + sFullKlarfPath + ")", new Exception("")));
                return;
            }


            // pour info
            int iWaferSize_mm = 0;

            if (Wafer is NotchWafer)
            {
                iWaferSize_mm = (int)((Wafer as NotchWafer).Diameter / 1000.0);
            }
            else if (Wafer is FlatWafer)
            {
                iWaferSize_mm = (int)((Wafer as FlatWafer).Diameter / 1000.0);

            }
            else if (Wafer is RectangularWafer)
            {
                float width_mm = (Wafer as RectangularWafer).Width / 1000.0f;
                float Height_mm = (Wafer as RectangularWafer).Height / 1000.0f;
                iWaferSize_mm = (int)(Math.Sqrt(width_mm * width_mm + Height_mm * Height_mm));
            }

            // Contains for each roughbin the value of label and color from addReport Array
            Dictionary<int, DefectValues> defectLabelsAndColors = new Dictionary<int, DefectValues>();
            // contains for each roughbin the defect label from Bin Array
            Dictionary<String, int> defectRoughBin = new Dictionary<String, int>();

            ReadDefectInfoFromModule(ref defectLabelsAndColors, ref defectRoughBin);

            // give the information to wafer result
            oWaferRes.SetDefectLabelsAndColors(defectLabelsAndColors, defectRoughBin);


            System.Drawing.Image myReportImg = new Bitmap(nReportImgW, nReportImgH, PixelFormat.Format24bppRgb);
            using (Graphics gfx = Graphics.FromImage(myReportImg))
            {
                // on crée une image black
                using (SolidBrush brush = new SolidBrush(Color.FromArgb(0, 0, 0)))
                {
                    gfx.FillRectangle(brush, 0, 0, nReportImgW, nReportImgH);
                }

                //
                // WAFER IMAGE AND DEFECTS
                //
                int margin = (int)(10 * res);
                int size = (int)(620 * res);
                int squareSize = (int)(2 * res);
                float notchDiam = 10 * res;
                float penSize = 1 * res;
                oWaferRes.DrawWaferDefects(gfx, margin, size, notchDiam, penSize, squareSize);

                //
                // HEADER INFORMATION
                //
                float startPosCaptionX = 620 * res;
                float startPosValueX = 810 * res;
                float startPosY = 15 * res;
                float deltaY = 25 * res;

                using (Font fontRegular = new System.Drawing.Font("Arial", 10 * res, FontStyle.Regular))

                using (Font fontBold = new System.Drawing.Font("Arial", 10 * res, FontStyle.Bold))
                {
                    Brush brush = Brushes.White;

                    gfx.DrawString("Recipe : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(sDB_RecipeID, fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("LotID : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(sDB_LotID, fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("WaferID : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(oWaferRes.WaferParameters.WaferID, fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("SlotNR : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(sDB_SlotID.ToString(), fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("Sum of all Defects : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(oWaferRes.SumOfDefects.ToString(), fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("Date : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(fileModificationDate.ToString("yyyy-MM-dd"), fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("Time : ", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(fileModificationDate.ToString("HH:mm:ss"), fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;
                    gfx.DrawString("Wafer Diameter :", fontRegular, brush, startPosCaptionX, startPosY);
                    gfx.DrawString(iWaferSize_mm.ToString() + " mm (" + ConvertMmToIntegerInch(iWaferSize_mm) + "\'\')", fontBold, brush, startPosValueX, startPosY);
                    startPosY += deltaY;

                    //
                    // DEFECT CAPTIONS
                    //
                    Rectangle rectCaption = new Rectangle((int)(630 * res), (int)(270 * res), (int)(500 * res), (int)(340 * res));
                    oWaferRes.DrawDefectCaption(gfx, rectCaption);

                    //
                    // BAR CHART
                    //
                    using (Pen normalPen = new Pen(Brushes.Cyan, 1 * res))
                    using (Font font = new Font("Arial", 10 * res, FontStyle.Regular))
                    {
                        Rectangle rectBarChart = new Rectangle((int)(5 * res), (int)(630 * res), (int)(800 * res), (int)(180 * res));
                        oWaferRes.DrawBarChart(gfx, rectBarChart, normalPen, font, Brushes.Cyan, Brushes.DarkCyan, Brushes.LightGray, 200);
                    }

                    ////
                    //// AREA DEFECT INFO
                    ////
                    //using (Pen normalPen = new Pen(Brushes.Cyan, 1 * res))
                    //using (Font font = new Font("Arial", 10 * res, FontStyle.Regular))
                    //{
                    //    Rectangle rectBarChart = new Rectangle((int)(820 * res), (int)(630 * res), (int)(120 * res), (int)(180 * res));
                    //    oWaferRes.DrawAreaChart(gfx, rectBarChart, normalPen, font, Brushes.Cyan, Brushes.DarkCyan);
                    //}

                    //
                    // Save image (voir si ça passe en jpg)
                    //
                    // fdu ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Png);

                    // Create an Encoder object based on the GUID for the Quality parameter category.
                    // fdu System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                    // Create an EncoderParameters object.  An EncoderParameters object has an array of EncoderParameter objects. In this case, there is only one EncoderParameter object in the array.
                    // fdu EncoderParameters myEncoderParameters = new EncoderParameters(1);

                    // fdu long lQualityValue = 75L; // 0 <=> best compression, lowest quality; 100 <=> lowest compression, best quality 
                    // fdu EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, lQualityValue);
                    // fdu  myEncoderParameters.Param[0] = myEncoderParameter;

                    String sAdditionnalReportImgPng = Path.ChangeExtension(sFullKlarfPath, null) + "_AddReport001.png";
                    myReportImg.Save(sAdditionnalReportImgPng, ImageFormat.Png);

                }
            }
        }

        private void ReadDefectInfoFromModule(ref Dictionary<int, DefectValues> defectLabelsAndColors, ref Dictionary<String, int> defectRoughBin)
        {
            DefectValues defectValues = new DefectValues("Unknow", Color.AliceBlue);

            // OPI : TODO
            // lire dans le tableua des Bin, les labels assiciés aux roughbin
            // Lire à partir du module Addreport les couleur associé aux labels
            // creer une table defectLabelsAndColors  qui associe un roughtbin aux label/couleur via les labels           

            foreach (DefectRoughBin RoughBin in ParamRoughBins.RoughBins.Values)
            {
                defectRoughBin.Add(RoughBin.DefectLabel, RoughBin.RoughBinNum);
            }

            int Order = 0;
            foreach (KlarfDefectColorCategory defectClass in ParamDefectColor.LabelToCategoryMap.Values)
            {
                DefectValues defectValue = new DefectValues(defectClass.DefectLabel, Color.FromName(defectClass.Color));
                defectLabelsAndColors.Add(Order++, defectValue);
            }
        }

        public ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        //=================================================================
        //
        //=================================================================
        public PrmSampleOrientationMarkType GetWaferOrientationMark()
        {
            WaferBase wafer = Recipe.Wafer;
            PrmSampleOrientationMarkType.somtType type;

            if (Wafer is NotchWafer)
                type = PrmSampleOrientationMarkType.somtType.NOTCH;
            else if (Wafer is FlatWafer)
            {
                FlatWafer flatwafer = (FlatWafer)wafer;
                if (flatwafer.IsDoubleFlat)
                    type = PrmSampleOrientationMarkType.somtType.DFLAT;
                else
                    type = PrmSampleOrientationMarkType.somtType.FLAT;
            }
            else if (Wafer is RectangularWafer)
                return null;
            else
                throw new ApplicationException("Unknown Wafer Type: " + wafer.GetType());

            return new PrmSampleOrientationMarkType(type);
        }

        //=================================================================
        // 
        //=================================================================
        public void WriteSqrFile()
        {
            PathString sqrFilename = KlarfFilename;
            sqrFilename.ChangeExtension(".sqr");

            RectangularWafer wafer = (RectangularWafer)Recipe.Wafer;
            float wafer_size_X_mm = (float)(wafer.Width / 1000);
            float wafer_size_Y_mm = (float)(wafer.Height / 1000);

            using (StreamWriter outStreamViewer = new StreamWriter(sqrFilename, false))
            {
                outStreamViewer.WriteLine("<root>");
                outStreamViewer.WriteLine("<wafer_size_X_mm value=\"" + wafer_size_X_mm + "\" />");
                outStreamViewer.WriteLine("<wafer_size_Y_mm value=\"" + wafer_size_Y_mm + "\" />");
                outStreamViewer.WriteLine("</root>");
            }
        }

        /// <summary>
        /// Static pour gérer les instances de recettes différente entre l'affichage et l'exécution
        /// </summary>
        private static KlarfRenderingViewModel _klarfRenderingVm;
        private static UserControl _klarfRenderingcontrol;

        public override UserControl GetUI()
        {
            if (_klarfRenderingVm == null)
            {
                _klarfRenderingVm = new KlarfRenderingViewModel(this);
                _klarfRenderingcontrol = new ResultView();
                _klarfRenderingcontrol.DataContext = _klarfRenderingVm;
            }
            else if (_klarfRenderingVm.Module != this)
            {
                _klarfRenderingVm.Module = this;
            }

            return _klarfRenderingcontrol;
        }

        public override UserControl RenderingUI
        {
            get
            {
                return GetUI();
            }
        }

        public override void ClearRenderingObjects()
        {
            base.ClearRenderingObjects();
            if (_klarfRenderingVm != null)
            {
                _klarfRenderingVm.Clean();
                _klarfRenderingVm = null;
                _klarfRenderingcontrol = null;
            }
            if (DataKlarf != null)
            {
                DataKlarf.DefectList.Clear();
            }
        }

        /// <summary>
        /// Notification pour affichage des résultats 
        /// </summary>
        public void SendKlarfResult()
        {
            ClassLocator.Default.GetInstance<IMessenger>().Send(new KlarfResultMessage() { Module = this });
        }

        public int ConvertMmToIntegerInch(int mmValue)
        {
            double inchValue = (double)mmValue / 25.4;
            return (int)Math.Round(inchValue);
        }
    }
}
